using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;

namespace SpouseRooms.Menu
{
    public partial class SpouseRoomMenu : IClickableMenu
    {
        private bool _isDragging;
        private Point _lastMouse;
        private int _statusTimer;

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (HandleLeftClickUI(x, y))
                return;

            base.receiveLeftClick(x, y, playSound);

            _isDragging = true;
            _lastMouse = new Point(x, y);
        }

        public override void leftClickHeld(int x, int y)
        {
            base.leftClickHeld(x, y);

            if (!_isDragging)
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
            _isDragging = false;
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

        public override void update(GameTime time)
        {
            base.update(time);

            GamepadKeyboardPanning();

            if (_statusTimer > 0)
                _statusTimer--;
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
            if (kb.IsKeyDown(Keys.D) || kb.IsKeyDown(Keys.Right))
                dx += speed;

            var pad = Game1.input.GetGamePadState();
            float lx = pad.ThumbSticks.Left.X;
            float ly = pad.ThumbSticks.Left.Y;

            const float deadzone = 0.2f;

            if (Math.Abs(lx) > deadzone || Math.Abs(ly) > deadzone)
            {
                dx += (int)(lx * speed);
                dy -= (int)(ly * speed); 
            }

            if (dx != 0 || dy != 0)
            {
                Game1.viewport.X += dx;
                Game1.viewport.Y += dy;
                ClampViewport(Game1.currentLocation);
            }
        }
    }
}


