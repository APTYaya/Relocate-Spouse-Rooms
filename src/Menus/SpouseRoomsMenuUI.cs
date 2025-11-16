using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using SpouseRooms.Entry;
using SpouseRooms.Relocation;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;


namespace SpouseRooms.Menu
{
    public partial class SpouseRoomMenu : IClickableMenu
    {
        private const int SpouseRowHeight = 24;

        private Rectangle _spousePanelBounds;
        private Rectangle _exitButtonBounds;

        private int _hoveredRow = -1;
        private string? _statusMessage;

        private bool HandleLeftClickUI(int x, int y)
        {
            if (_spousePanelBounds.Contains(x, y) && SpouseRoomRelocationManager.Rooms.Count > 0)
            {
                int startY = _spousePanelBounds.Y + 24;
                int index = (y - startY) / SpouseRowHeight;

                if (index >= 0 && index < SpouseRoomRelocationManager.Rooms.Count)
                {
                    SpouseRoomRelocationManager.SelectedIndex = index;

                    var room = SpouseRoomRelocationManager.SelectedRoom;
                    if (room != null)
                    {
                        _statusMessage = $"Selected spouse room: {room.SpouseName}. Choose a new location.";
                        _statusTimer = 180;
                    
                        var tile  = room.CenterTile;   
                        var rect  = room.Bounds;

                        ModEntry.Instance.Monitor.Log(
                            $"[SpouseRooms] Selected '{room.SpouseName}' at tile ({tile.X}, {tile.Y}), " +
                            $"bounds {rect.X},{rect.Y} size {rect.Width}x{rect.Height}.",
                            StardewModdingAPI.LogLevel.Debug
                        );
                    }
                    Game1.playSound("smallSelect");
                    return true;
                }
            }

            if (_exitButtonBounds.Contains(x, y))
            {
                Game1.playSound("bigDeSelect");
                ReturnFromMenu();
                return true;
            }

            return false;
        }

        public override void receiveGamePadButton(Buttons b)
        {

            if (b == Buttons.DPadUp)
            {
                if (SpouseRoomRelocationManager.Rooms.Count > 0)
                {
                    SpouseRoomRelocationManager.SelectedIndex--;
                    if (SpouseRoomRelocationManager.SelectedIndex < 0)
                        SpouseRoomRelocationManager.SelectedIndex = SpouseRoomRelocationManager.Rooms.Count - 1;

                    Game1.playSound("shiny4");
                }
                return;
            }

            if (b == Buttons.DPadDown)
            {
                if (SpouseRoomRelocationManager.Rooms.Count > 0)
                {
                    SpouseRoomRelocationManager.SelectedIndex++;
                    if (SpouseRoomRelocationManager.SelectedIndex >= SpouseRoomRelocationManager.Rooms.Count)
                        SpouseRoomRelocationManager.SelectedIndex = 0;

                    Game1.playSound("shiny4");
                }
                return;
            }

            if (b == Buttons.A)
            {
                if (SpouseRoomRelocationManager.Rooms.Count > 0)
                {
                    var room = SpouseRoomRelocationManager.SelectedRoom;
                    if (room != null)
                    {
                        _statusMessage = $"Selected spouse room: {room.SpouseName}. Choose a new location.";
                        _statusTimer = 180;

                        Game1.playSound("smallSelect");
                    }
                }
                return;
            }

            if (b == Buttons.B)
            {
                Game1.playSound("bigDeSelect");
                ReturnFromMenu();
                return;
            }

            base.receiveGamePadButton(b);
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);

            _hoveredRow = -1;

            if (_spousePanelBounds.Contains(x, y) && SpouseRoomRelocationManager.Rooms.Count > 0)
            {
                int startY = _spousePanelBounds.Y + 24;
                int index = (y - startY) / SpouseRowHeight;

                if (index >= 0 && index < SpouseRoomRelocationManager.Rooms.Count)
                    _hoveredRow = index;
            }
        }

        private void DrawUI(SpriteBatch b)
        {
            int margin = 16;
            int vw = Game1.uiViewport.Width;
            int vh = Game1.uiViewport.Height;

            int roomCount = SpouseRoomRelocationManager.Rooms.Count;
            if (roomCount < 1)
                roomCount = 1;

            int panelHeight =
                24 +
                roomCount * SpouseRowHeight +
                16;

            _spousePanelBounds = new Rectangle(
                margin,
                margin,
                320,
                panelHeight
            );

            _exitButtonBounds = new Rectangle(
                vw - 96 - margin,
                vh - 96 - margin,
                96,
                96
            );

            Texture2D tex = Game1.menuTexture;
            Rectangle frame = new Rectangle(0, 256, 60, 60);
            SpriteFont font = Game1.smallFont;

            IClickableMenu.drawTextureBox(
                b,
                tex,
                frame,
                _spousePanelBounds.X,
                _spousePanelBounds.Y,
                _spousePanelBounds.Width,
                _spousePanelBounds.Height,
                Color.White,
                1f,
                drawShadow: false
            );

            Vector2 headerPos = new Vector2(
                _spousePanelBounds.X + 16,
                _spousePanelBounds.Y + 4
            );
            Utility.drawTextWithShadow(b, "Spouse rooms", font, headerPos, Game1.textColor);

            int lineIndex = 0;
            int selected = SpouseRoomRelocationManager.SelectedIndex;

            foreach (var room in SpouseRoomRelocationManager.Rooms)
            {
                string label = $"Spouse room: {room.SpouseName}";
                Vector2 pos = new Vector2(
                    _spousePanelBounds.X + 16,
                    _spousePanelBounds.Y + 24 + lineIndex * SpouseRowHeight
                );

                Color color = Game1.textColor;

                if (lineIndex == selected)
                    color = Color.Gold;
                else if (lineIndex == _hoveredRow)
                    color = Color.LightGoldenrodYellow;

                Utility.drawTextWithShadow(b, label, font, pos, color);

                lineIndex++;
            }

            if (SpouseRoomRelocationManager.Rooms.Count == 0)
            {
                Vector2 nonePos = new Vector2(
                    _spousePanelBounds.X + 16,
                    _spousePanelBounds.Y + 24
                );
                Utility.drawTextWithShadow(b, "(no spouse room)", font, nonePos, Game1.textColor);
            }

            IClickableMenu.drawTextureBox(
                b,
                tex,
                frame,
                _exitButtonBounds.X,
                _exitButtonBounds.Y,
                _exitButtonBounds.Width,
                _exitButtonBounds.Height,
                Color.White,
                1f,
                drawShadow: false
            );

            string exitText = "Exit";
            Vector2 exitSize = font.MeasureString(exitText);
            Vector2 exitPos = new Vector2(
                _exitButtonBounds.X + (_exitButtonBounds.Width - exitSize.X) / 2f,
                _exitButtonBounds.Y + (_exitButtonBounds.Height - exitSize.Y) / 2f
            );
            Utility.drawTextWithShadow(b, exitText, font, exitPos, Game1.textColor);

            if (_statusTimer > 0 && !string.IsNullOrEmpty(_statusMessage))
            {
                Vector2 statusPos = new Vector2(16, vh - 32);
                Utility.drawTextWithShadow(b, _statusMessage, font, statusPos, Game1.textColor);
            }
        }
    }
}

