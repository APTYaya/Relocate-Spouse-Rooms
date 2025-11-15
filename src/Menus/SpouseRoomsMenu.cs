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

        private bool _dragging;
        private Point _lastMouse;

        public SpouseRoomMenu(string oldLocationName, Vector2 oldTile, int oldFacing)
        {
            _oldLocationName = oldLocationName;
            _oldTile = oldTile;
            _oldFacing = oldFacing;

            InitUI();

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

        public override void receiveLeftClick(int x, int y, bool playSound = false)
        {
            if (HandleLeftClickUI(x, y))
                return;

            base.receiveLeftClick(x, y, playSound);

            _dragging = true;
            _lastMouse = new Point(x, y);
        }


        public override void leftClickHeld(int x, int y)
        {
            base.leftClickHeld(x, y);

            if (!_dragging)
                return;

            int dx = x - _lastMouse.X;
            int dy = y - _lastMouse.Y;
            _lastMouse = new Point(x, y);

            Game1.viewport.X -= dx;
            Game1.viewport.Y -= dy;

            ClampViewport(Game1.currentLocation);
        }

        public override void releaseLeftClick(int x, int y)
        {
            base.releaseLeftClick(x, y);
            _dragging = false;
        }
        public override void update(GameTime time)
        {
            base.update(time);

            GamepadKeyboardPanning();
        }
        private void GamepadKeyboardPanning()
        {   
            int speed = 8;
            int dx = 0;
            int dy = 0;

            var kb = Keyboard.GetState();
            if (kb.IsKeyDown(Keys.W) || kb.IsKeyDown(Keys.Up))
                dy -= speed;
            if (kb.IsKeyDown(Keys.S) || kb.IsKeyDown(Keys.Down))
                dy += speed;
            if (kb.IsKeyDown(Keys.A) || kb.IsKeyDown(Keys.Left))
                dx -= speed; 
            if (kb.IsKeyDown(Keys.D)  || kb.IsKeyDown(Keys.Right))
                dx += speed;

            var pad = Game1.input.GetGamePadState();
            if (pad.DPad.Up == ButtonState.Pressed)
                dy -= speed;
            if (pad.DPad.Down == ButtonState.Pressed)
                dy += speed;
            if (pad.DPad.Left == ButtonState.Pressed)
                dx -= speed;
            if (pad.DPad.Right == ButtonState.Pressed)
                dx += speed;

            if (dx != 0 || dy != 0)
            {
                Game1.viewport.X += dx;
                Game1.viewport.Y += dy;
                ClampViewport(Game1.currentLocation);
            }
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
        
        public override void receiveGamePadButton(Buttons b)
        {
            if (b == Buttons.A)
            {
                if (currentlySnappedComponent == _spouseButton)
                {
                    Game1.playSound("smallSelect");
                    return;
                }
                
                if (currentlySnappedComponent == _confirmButton)
                {
                    Game1.playSound("smallSelect");
                    ReturnFromMenu();
                    return;
                }
            }

            if (b == Buttons.B || b == Buttons.Back)
            {
                ReturnFromMenu();
                return;
            }

            base.receiveGamePadButton(b);
        }

        private void ReturnFromMenu()
        {
            Game1.warpFarmer(_oldLocationName, (int)_oldTile.X, (int)_oldTile.Y, false);
            Game1.player.FacingDirection = _oldFacing;

            Game1.displayHUD = true;
            Game1.displayFarmer = true;
            Game1.viewportFreeze = false;

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


