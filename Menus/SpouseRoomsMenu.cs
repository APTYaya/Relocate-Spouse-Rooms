using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using StardewValley.BellsAndWhistles;
using StardewValley.Extensions;
using xTile.Dimensions;

namespace SpouseRooms.Menu
{
    public class SpouseRoomMenu : IClickableMenu
    {
        private readonly string _oldLocationName;
        private readonly Vector2 _oldTile;
        private readonly int _oldFacing;

        //private bool _dragging;
        //private Point _lastMouse;

        public SpouseRoomMenu(string oldLocationName, Vector2 oldTile, int oldFacing)
        {
            _oldLocationName = oldLocationName;
            _oldTile = oldTile;
            _oldFacing = oldFacing;

            DelayedAction.functionAfterDelay(() =>
            {
                Game1.viewportFreeze = true;
                CenterViewportOnCurrentLocation();
                Game1.displayHUD = false;
                Game1.displayFarmer = false;
            }, 900);
        }

        public void CenterViewportOnCurrentLocation()
        {
            GameLocation loc = Game1.currentLocation;

            int mapWidth = loc.map.DisplayWidth;
            int mapHeight = loc.map.DisplayHeight;

            Game1.viewport.X = mapWidth / 2 - Game1.viewport.Width / 2;
            Game1.viewport.Y = mapHeight / 2 - Game1.viewport.Height / 2;

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

        public override void receiveKeyPress(Keys key)
        {
            if (Game1.options.doesInputListContain(Game1.options.menuButton, key))
            {
                ReturnFromMenu();
                return;
            }

            base.receiveKeyPress(key);
        }

        private void ReturnFromMenu()
        {
            Game1.displayHUD = true;
            Game1.displayFarmer = true;
            Game1.viewportFreeze = false;

            Game1.warpFarmer(_oldLocationName, (int)_oldTile.X, (int)_oldTile.Y, false);
            Game1.player.FacingDirection = _oldFacing;

            exitThisMenu();
        }
    }
}


