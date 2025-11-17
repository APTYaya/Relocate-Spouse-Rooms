using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Locations;
using SpouseRooms.Relocation;

namespace SpouseRooms.SpouseRoomsPatches
{
    [HarmonyPatch(typeof(FarmHouse), nameof(FarmHouse.GetSpouseRoomCorner))]
    internal static class FarmHouseGetSpouseCornerPatch
    {
        public static void Postfix(FarmHouse __instance, ref Point __result)
        {
            string? spouseId = __instance.owner?.spouse;
            __result = SpouseRoomRelocationManager.GetCornerForSpouse(spouseId, __result);
        }
    }
}


