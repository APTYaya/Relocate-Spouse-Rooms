using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using SpouseRooms.SpouseRoomsScanner;
using SpouseRooms.Relocation;

namespace SpouseRooms.Menu
{
    public partial class SpouseRoomMenu : IClickableMenu
    {
        private readonly string _oldLocationName;
        private readonly Vector2 _oldTile;
        private readonly int _oldFacing;

        public SpouseRoomMenu(string oldLocationName, Vector2 oldTile, int oldFacing)
        {
            _oldLocationName = oldLocationName;
            _oldTile = oldTile;
            _oldFacing = oldFacing;

            SpouseRoomRelocationManager.ScanCurrentFarmHouse();

            DelayedAction.functionAfterDelay(() =>
            {
                Game1.viewportFreeze = true;
                var house = Game1.currentLocation as FarmHouse;
                SpouseRoomInfo? room = null;

                if (house != null)
                {
                    room = SpouseRoomProvider.GetRoom(house);
                }

                if (room != null)
                {
                    CenterViewportOnTile(room.CenterTile);
                }
                else
                {
                    CenterViewportOnTile(Game1.player.TilePoint);
                }
                Game1.displayHUD = false;
                Game1.displayFarmer = false;
            }, 700);
        }

        private void CenterViewportOnTile(Point tile)
        {
            GameLocation loc = Game1.currentLocation;

            int pixelX = tile.X * Game1.tileSize;
            int pixelY = tile.Y * Game1.tileSize;

            Game1.viewport.X = pixelX - Game1.viewport.Width  / 2;
            Game1.viewport.Y = pixelY - Game1.viewport.Height / 2;

            ClampViewport(loc);
        }

        private void ClampViewport(GameLocation loc)
        {
            int maxX = loc.map.DisplayWidth - Game1.viewport.Width;
            int maxY = loc.map.DisplayHeight - Game1.viewport.Height;

            if (maxX < 0 ) maxX = 0;
            if (maxY < 0 ) maxY = 0;

            Game1.viewport.X = Math.Max(0, Math.Min(Game1.viewport.X, maxX));
            Game1.viewport.Y = Math.Max(0, Math.Min(Game1.viewport.Y, maxY));
        }

        private void ReturnFromMenu()
        {
            Game1.warpFarmer(_oldLocationName, (int)_oldTile.X, (int)_oldTile.Y, false);
            Game1.player.FacingDirection = _oldFacing;
            DelayedAction.functionAfterDelay(() => 
            {
                Game1.displayHUD = true;
                Game1.displayFarmer = true;
                Game1.viewportFreeze = false;
            }, 500);
            exitThisMenu();
        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);
            
            DrawUI(b);

            drawMouse(b);
        }
    }
}


