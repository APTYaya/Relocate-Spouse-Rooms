using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Characters;
using StardewValley.GameData.Characters;
using StardewValley.Locations;

namespace SpouseRooms.SpouseRoomsScanner
{
    public class SpouseRoomInfo
    {
        public string SpouseId { get; }
        public string SpouseName { get; }
        public string LocationName { get; }

        public Point CornerTile { get; }
        public Rectangle Bounds { get; }
        public Point CenterTile { get; }

        public SpouseRoomInfo(
            string spouseId,
            string spouseName,
            string locationName,
            Point cornerTile,
            Rectangle bounds,
            Point centerTile
        )
        {
            SpouseId = spouseId;
            SpouseName = spouseName;
            LocationName = locationName;
            CornerTile = cornerTile;
            Bounds = bounds;
            CenterTile = centerTile;
        }
    }

    internal static class SpouseRoomProvider
    {
        public static SpouseRoomInfo? GetRoom(FarmHouse house)
        {
            if (house == null)
                return null;

            if (!house.HasNpcSpouseOrRoommate())
                return null;

            Point corner = house.GetSpouseRoomCorner();

            var rect = CharacterSpouseRoomData.DefaultMapSourceRect;
            if (NPC.TryGetData(house.owner?.spouse, out var data)
                && data.SpouseRoom?.MapSourceRect != null)
            {
                rect = data.SpouseRoom.MapSourceRect;
            }

            var roomBounds = new Rectangle(
                corner.X,
                corner.Y,
                rect.Width,
                rect.Height
            );

            var centerTile = new Point(
                roomBounds.X + roomBounds.Width / 2,
                roomBounds.Y + roomBounds.Height / 2
            );
            
            string spouseId = house.owner?.spouse ?? "UnknownSpouse";
            string spouseName = spouseId;

            NPC? npc = Game1.getCharacterFromName(spouseId);
            if (npc != null)
                spouseName = npc.displayName;

            string locationName = house.NameOrUniqueName;

            return new SpouseRoomInfo(
                spouseId,
                spouseName,
                locationName,
                corner,
                roomBounds,
                centerTile
            );
        }
    }
}


