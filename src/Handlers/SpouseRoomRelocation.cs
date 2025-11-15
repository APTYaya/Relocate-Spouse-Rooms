using System.Collections.Generic;
using StardewValley;
using StardewValley.Locations;
using SpouseRooms.SpouseRoomsScanner;

namespace SpouseRooms.Relocation
{
    internal static class SpouseRoomRelocationManager
    {
        internal static readonly List<SpouseRoomInfo> Rooms = new();
        internal static int SelectedIndex { get; set; }

        internal static SpouseRoomInfo? SelectedRoom =>
            (SelectedIndex >= 0 && SelectedIndex < Rooms.Count)
                ? Rooms[SelectedIndex]
                : null;

        internal static void ScanCurrentFarmHouse()
        {
            Rooms.Clear();
            SelectedIndex = 0;

            FarmHouse? house = Game1.currentLocation as FarmHouse;
            if (house == null)
                house = Game1.RequireLocation<FarmHouse>("FarmHouse");
            if (house == null)
                return;

            SpouseRoomInfo? info = SpouseRoomProvider.GetRoom(house);
            if (info != null)
                Rooms.Add(info);
        }
    }
}
