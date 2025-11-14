using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using System.Collections.Generic;
using SpouseRooms.Menu;

namespace SpouseRooms.answerDialoguePatch
{   
    [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.answerDialogue))]
    public static class CarpenterAnswerDialoguePatch
    {
        public static bool Prefix(GameLocation __instance, Response answer)
        {
            if (__instance.lastQuestionKey != "carpenter")
                return true;
            if (answer?.responseKey != "MoveSpouseRooms")
                return true;

            string oldLocation = Game1.currentLocation.NameOrUniqueName;
            Vector2 oldTile = Game1.player.Tile;
            int oldFacing = Game1.player.FacingDirection;

            Game1.warpFarmer("FarmHouse", 10, 7, false);
            
            Game1.activeClickableMenu = new SpouseRoomMenu(oldLocation, oldTile, oldFacing);

            return false;
        }

    }
}
