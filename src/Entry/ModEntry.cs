using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

using HarmonyLib;

namespace SpouseRooms.Entry
{  
    public class ModEntry : Mod {

        private ModConfig Config = new();

        internal static ModEntry Instance { get; private set; } = null!;

        public override void Entry(IModHelper helper)
        {   
            Instance = this;

            Config = helper.ReadConfig<ModConfig>();

            var harmony = new Harmony(this.ModManifest.UniqueID);
            harmony.PatchAll();

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;

        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
        CreateGenericModMenu();
        }   
        
        private void CreateGenericModMenu()
        {
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            configMenu.Register (
            mod: this.ModManifest,
            reset: () => this.Config = new ModConfig(),
            save: () => this.Helper.WriteConfig(this.Config)
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Enable Spouse Rooms",
                tooltip: () => "This will enable spouse rooms if checked.",
                getValue: () => this.Config.EnableSpouseRooms,
                setValue: value => this.Config.EnableSpouseRooms = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Enable Custom Spouse Rooms",
                tooltip: () => "This will enable custom spouse rooms that will allow you to design the room however you want.",
                getValue: () => this.Config.EnableCustomSpouseRooms, 
                setValue: value => this.Config.EnableCustomSpouseRooms = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Enable cost for moving spouse rooms",
                tooltip: () => "This enables optional cost for moving spouse rooms to different location within the farm house.",
                getValue: () => this.Config.EnableCostForMovingSpouseRooms,
                setValue: value => this.Config.EnableCostForMovingSpouseRooms = value 
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Cost for Moving Spouse Rooms",
                tooltip: () => "Set the cost for moving spouse rooms min 0 gold max 10000 gold.",
                getValue: () => this.Config.MoveSpouseRoomsCost,
                setValue: value => this.Config.MoveSpouseRoomsCost = value,
                min: 0,
                max: 10000
            );
        }
    }
}