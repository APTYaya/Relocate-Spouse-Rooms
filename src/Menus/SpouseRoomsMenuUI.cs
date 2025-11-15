using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using SpouseRooms.Relocation;

namespace SpouseRooms.Menu
{
    public partial class SpouseRoomMenu : IClickableMenu
    {
        private const int SpouseRowHeight = 24;

        private Rectangle _spousePanelBounds;

        private ClickableTextureComponent? _spouseButton;   
        private ClickableTextureComponent? _confirmButton;

        private void InitUI()
        {
            _spouseButton = new ClickableTextureComponent(
                "SpouseSelector",
                Rectangle.Empty,
                null,
                "Select spouse room",
                Game1.mouseCursors,
                new Rectangle(128, 256, 64, 64),
                1f
            );

            _confirmButton = new ClickableTextureComponent(
                "Confirm",
                Rectangle.Empty,
                null,
                "Confirm placement",
                Game1.mouseCursors,
                new Rectangle(128, 256, 64, 64),
                1f
            );

            allClickableComponents ??= new();
            allClickableComponents.Clear();

            if (_spouseButton != null)
            {
                _spouseButton.myID = 0;
                allClickableComponents.Add(_spouseButton);
            }

            if (_confirmButton != null)
            {
                _confirmButton.myID = 1;
                allClickableComponents.Add(_confirmButton);
            }

            if (_spouseButton != null && _confirmButton != null)
            {
                _spouseButton.rightNeighborID = _confirmButton.myID;
                _confirmButton.leftNeighborID  = _spouseButton.myID;
            }

            if (Game1.options.SnappyMenus)
            {
                populateClickableComponentList();
                snapToDefaultClickableComponent();
            }
        }

        public override void snapToDefaultClickableComponent()
        {
            if (!Game1.options.SnappyMenus)
                return;

            if (_spouseButton != null)
            {
                currentlySnappedComponent = _spouseButton;
                snapCursorToCurrentSnappedComponent();
            }
            else if (_confirmButton != null)
            {
                currentlySnappedComponent = _confirmButton;
                snapCursorToCurrentSnappedComponent();
            }
        }

        private bool HandleLeftClickUI(int x, int y)
        {
            if (_spousePanelBounds.Contains(x, y) && SpouseRoomRelocationManager.Rooms.Count > 0)
            {
                int startY = _spousePanelBounds.Y + 24; 
                int index = (y - startY) / SpouseRowHeight;

                if (index >= 0 && index < SpouseRoomRelocationManager.Rooms.Count)
                {
                    SpouseRoomRelocationManager.SelectedIndex = index;
                    Game1.playSound("smallSelect");
                    return true;
                }
            }

            if (_confirmButton != null && _confirmButton.containsPoint(x, y))
            {
                Game1.playSound("smallSelect");
                ReturnFromMenu();
                return true;
            }

            return false;
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);

            if (_confirmButton != null)
                _confirmButton.scale = _confirmButton.containsPoint(x, y) ? 1.2f : 1f;
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
                260,
                panelHeight
            );

            Rectangle confirmButtonRect = new Rectangle(
                vw - 96 - margin,
                vh - 96 - margin,
                96,
                96
            );

            if (_spouseButton != null)
            {
                _spouseButton.bounds = new Rectangle(
                    _spousePanelBounds.X + 16,
                    _spousePanelBounds.Y + 24,
                    48,
                    48
                );
            }

            if (_confirmButton != null)
            {
                _confirmButton.bounds = confirmButtonRect;
            }

            Texture2D tex = Game1.menuTexture;
            Rectangle source = new Rectangle(0, 256, 60, 60);
            SpriteFont font = Game1.smallFont;

            IClickableMenu.drawTextureBox(
                b,
                tex,
                source,
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
                string name = $"Spouse room: {room.SpouseName}";
                Vector2 namePos = new Vector2(
                    _spousePanelBounds.X + 16,
                    _spousePanelBounds.Y + 24 + lineIndex * SpouseRowHeight
                );

                Color textColor = (lineIndex == selected)
                    ? Game1.textColor
                    : Color.SandyBrown; 

                Utility.drawTextWithShadow(b, name, font, namePos, textColor);
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

            if (_confirmButton != null)
            {
                IClickableMenu.drawTextureBox(
                    b,
                    tex,
                    source,
                    _confirmButton.bounds.X,
                    _confirmButton.bounds.Y,
                    _confirmButton.bounds.Width,
                    _confirmButton.bounds.Height,
                    Color.White,
                    1f,
                    drawShadow: false
                );

                string confirmText = "Confirm";
                Vector2 size = font.MeasureString(confirmText);
                Vector2 textPos = new Vector2(
                    _confirmButton.bounds.X + (_confirmButton.bounds.Width - size.X) / 1.5f,
                    _confirmButton.bounds.Y + (_confirmButton.bounds.Height - size.Y) / 1.5f
                );

                Utility.drawTextWithShadow(b, confirmText, font, textPos, Game1.textColor);
            }
        }
    }
}

