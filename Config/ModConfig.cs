using StardewModdingAPI.Utilities;

namespace SpouseRooms {
    public sealed class ModConfig 
    {
    public bool EnableSpouseRooms { get; set;} = true;
    public bool EnableCustomSpouseRooms { get; set;} = false;
    public bool EnableCostForMovingSpouseRooms { get; set;} = false;
    
    public int MoveSpouseRoomsCost { get; set;} = 0;
    }
}