using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Locations;
using StardewModdingAPI;
using SpouseRooms.Entry;
using SpouseRooms.SpouseRoomsScanner;

namespace SpouseRooms.Relocation
{
    public class SpousePlacementData
    {
        public string SpouseId { get; set; } = string.Empty;
        public string SpouseName { get; set; } = string.Empty;
        public Point CornerTile { get; set; }
    }

    internal static class SpouseRoomRelocationManager
    {

        internal static readonly List<SpouseRoomInfo> Rooms = new();
        internal static int SelectedIndex { get; set; } = -1;

        internal static SpouseRoomInfo? SelectedRoom =>
            SelectedIndex >= 0 && SelectedIndex < Rooms.Count
                ? Rooms[SelectedIndex]
                : null;

        internal static void ClearRooms()
        {
            Rooms.Clear();
            SelectedIndex = -1;
        }

        internal static void RefreshRooms(FarmHouse house)
        {
            ClearRooms();

            var info = SpouseRoomProvider.GetRoom(house);
            if (info != null)
            {
                Rooms.Add(info);
                SelectedIndex = 0;

                ModEntry.Instance.Monitor.Log(
                    $"[SpouseRooms] Found spouse room for {info.SpouseName} at bounds {info.Bounds}.",
                    StardewModdingAPI.LogLevel.Trace
                );
            }
            else
            {
                ModEntry.Instance.Monitor.Log(
                    "[SpouseRooms] No spouse room found in farmhouse.",
                    StardewModdingAPI.LogLevel.Trace
                );
            }
        }

        internal static readonly Dictionary<string, SpousePlacementData> Placements = new();

        internal static void LoadPlacementsFromSave(
            IDictionary<string, SpousePlacementData>? loaded
        )
        {
            Placements.Clear();

            if (loaded == null)
                return;

            foreach (var pair in loaded)
            {
                if (string.IsNullOrEmpty(pair.Key) || pair.Value == null)
                    continue;

                Placements[pair.Key] = pair.Value;
            }

            ModEntry.Instance.Monitor.Log(
                $"[SpouseRooms] Loaded {Placements.Count} saved spouse placements.",
                StardewModdingAPI.LogLevel.Trace
            );
        }

        internal static Dictionary<string, SpousePlacementData> ExportPlacementsForSave()
        {
            return new Dictionary<string, SpousePlacementData>(Placements);
        }

        internal static bool IsPlacing { get; private set; }
        internal static Point PlacementOrigin { get; private set; } = Point.Zero;

        internal static Rectangle PlacementBounds =>
            SelectedRoom?.Bounds ?? Rectangle.Empty;

        internal static void StartPlacement(SpouseRoomInfo room)
        {
            IsPlacing = true;
            PlacementOrigin = new Point(room.Bounds.X, room.Bounds.Y);

            ModEntry.Instance.Monitor.Log(
                $"[SpouseRooms] Start placement for '{room.SpouseName}' at {PlacementOrigin.X},{PlacementOrigin.Y}.",
                StardewModdingAPI.LogLevel.Trace
            );
        }

        internal static void CancelPlacement()
        {
            IsPlacing = false;
        }

        internal static void UpdatePlacementFromMouse(GameLocation location)
        {
            if (!IsPlacing)
                return;

            var room = SelectedRoom;
            if (room == null)
                return;

            Point mouse = Game1.getMousePosition();
            int tileX = (mouse.X + Game1.viewport.X) / 64;
            int tileY = (mouse.Y + Game1.viewport.Y) / 64;

            Rectangle rect = room.Bounds;

            int originX = tileX - rect.Width / 2;
            int originY = tileY - rect.Height / 2;

            int mapWidth = location.map.Layers[0].LayerWidth;
            int mapHeight = location.map.Layers[0].LayerHeight;

            originX = Math.Clamp(originX, 0, mapWidth - rect.Width);
            originY = Math.Clamp(originY, 0, mapHeight - rect.Height);

            PlacementOrigin = new Point(originX, originY);
        }

        internal static void DrawPlacementOverlay(SpriteBatch b)
        {
            if (!IsPlacing)
                return;

            var room = SelectedRoom;
            if (room == null)
                return;

            GameLocation loc = Game1.currentLocation;
            int mapWidth = loc.map.Layers[0].LayerWidth;
            int mapHeight = loc.map.Layers[0].LayerHeight;

            Rectangle rect = room.Bounds;
            Point origin = PlacementOrigin;

            Texture2D fill = Game1.staminaRect;
            Color color = Color.Lime * 0.4f;

            for (int x = 0; x < rect.Width; x++)
            {
                for (int y = 0; y < rect.Height; y++)
                {
                    int tileX = origin.X + x;
                    int tileY = origin.Y + y;

                    if (tileX < 0 || tileY < 0 || tileX >= mapWidth || tileY >= mapHeight)
                        continue;

                    Rectangle dest = new Rectangle(
                        tileX * 64 - Game1.viewport.X,
                        tileY * 64 - Game1.viewport.Y,
                        64,
                        64
                    );

                    b.Draw(fill, dest, color);
                }
            }
        }

        internal static void CommitPlacement(GameLocation location)
        {
            if (!IsPlacing)
                return;

            var room = SelectedRoom;
            if (room == null)
                return;

            Point newCorner = PlacementOrigin;

            if (string.IsNullOrEmpty(room.SpouseId))
            {
                ModEntry.Instance.Monitor.Log(
                    "[SpouseRooms] CommitPlacement: selected room has empty SpouseId, skipping.",
                    StardewModdingAPI.LogLevel.Warn
                );
                IsPlacing = false;
                return;
            }

            var data = new SpousePlacementData
            {
                SpouseId   = room.SpouseId,
                SpouseName = room.SpouseName ?? room.SpouseId,
                CornerTile = newCorner
            };

            Placements[room.SpouseId] = data;

            ModEntry.Instance.Monitor.Log(
                $"[SpouseRooms] Saved placement for '{data.SpouseName}' at corner {newCorner.X},{newCorner.Y}.",
                StardewModdingAPI.LogLevel.Info
            );

            if (location is FarmHouse farmHouse)
            {
                farmHouse.updateFarmLayout();
                farmHouse.showSpouseRoom();
            }

            IsPlacing = false;
        }

        internal static Point GetCornerForSpouse(string? spouseId, Point vanillaCorner)
        {
            if (string.IsNullOrEmpty(spouseId))
                return vanillaCorner;

            if (Placements.TryGetValue(spouseId, out var data))
                return data.CornerTile;

            return vanillaCorner;
        }
    }
}



