﻿namespace Trinity
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Helpers;
    using Items;
    using Oasys.Common;
    using Oasys.Common.Enums.GameEnums;
    using Oasys.Common.EventsProvider;
    using Oasys.Common.GameObject.Clients;
    using Oasys.Common.Menu;
    using Oasys.SDK;
    using Oasys.SDK.Menu;
    using Oasys.SDK.Tools;

    public class Bootstrap
    {
        public static List<Champion> AllChampions = new();
        private static List<ActiveItemBase> AllItems = new();
        private static readonly List<ActiveItemBase> InitializedTickItems = new();

        [Oasys.SDK.OasysModuleEntryPoint]
        public static void Execute()
        {
            GameEvents.OnGameLoadComplete += GameEvents_OnGameLoadComplete;
            GameEvents.OnGameMatchComplete += GameEvents_OnGameMatchComplete;
        }

        private static async Task GameEvents_OnGameLoadComplete()
        {
            AllItems.AddRange(ConsumableItems);
            AllItems.AddRange(CleanseItems);

            Initialize();
            ReCacheHeroes();
            CoreEvents.OnCoreMainTick += CoreEvents_OnCoreMainTick;
        }

        private static async Task GameEvents_OnGameMatchComplete()
        {
            CoreEvents.OnCoreMainTick -= CoreEvents_OnCoreMainTick;
        }

        private static readonly List<ActiveItem> CleanseItems = new()
        {
            // item: Quicksilver_Sash
            new ActiveItem(100, ItemID.Quicksilver_Sash, Enums.TargetingType.ProximityAlly, 1100, 
                new [] {Enums.ActivationType.CheckAuras, Enums.ActivationType.CheckOnlyOnMe }),

            // item: Mercurial_Scimitar
            new ActiveItem(100, ItemID.Mercurial_Scimitar, Enums.TargetingType.ProximityAlly, 1100, 
                new [] {Enums.ActivationType.CheckAuras, Enums.ActivationType.CheckOnlyOnMe }),

            // item: Dervish_Blade
            new ActiveItem(100, ItemID.Dervish_Blade, Enums.TargetingType.ProximityAlly, 1100,
                new[] { Enums.ActivationType.CheckAuras, Enums.ActivationType.CheckOnlyOnMe }),

            // item: Mikaels_Crucible
            new ActiveItem(20, ItemID.Mikaels_Crucible, Enums.TargetingType.ProximityAlly, 600,
                new[] { Enums.ActivationType.CheckAuras, Enums.ActivationType.CheckOnlyOnMe, Enums.ActivationType.CheckAllyLowHP  }),
        };

        private static readonly List<ActiveItem> ConsumableItems = new()
        {
            // item: Health_Potion
            new ActiveItem(55, ItemID.Health_Potion, Enums.TargetingType.ProximityAlly, float.MaxValue,
                new[] { Enums.ActivationType.CheckOnlyOnMe, Enums.ActivationType.CheckAllyLowHP }, 
                "Item2003"),
            
            // item: Refillable_Potion
            new ActiveItem(55, ItemID.Refillable_Potion, Enums.TargetingType.ProximityAlly, float.MaxValue,
                new[] { Enums.ActivationType.CheckOnlyOnMe, Enums.ActivationType.CheckAllyLowHP }, 
                "ItemCrystalFlask"),
            
            // item: Corrupting_Potion
            new ActiveItem(55, ItemID.Corrupting_Potion, Enums.TargetingType.ProximityAlly, float.MaxValue,
                new[] { Enums.ActivationType.CheckOnlyOnMe, Enums.ActivationType.CheckAllyLowHP, Enums.ActivationType.CheckAllyLowMP },
                "ItemDarkCrystalFlask"),
            
            // item: Total_Biscuit_of_Rejuvenation
            new ActiveItem(55, ItemID.Total_Biscuit_of_Everlasting_Will, Enums.TargetingType.ProximityAlly, float.MaxValue,
                new[] { Enums.ActivationType.CheckOnlyOnMe, Enums.ActivationType.CheckAllyLowHP, Enums.ActivationType.CheckAllyLowMP }, 
                "Item2010"),
            
            // item: Elixir_of_Iron
            new ActiveItem(100, ItemID.Elixir_of_Iron, Enums.TargetingType.ProximityAlly, float.MaxValue,
                new[] {Enums.ActivationType.CheckOnlyOnMe }, 
                "ElixirOfIron"),
            
            // item: Elixir_of_Wrath
            new ActiveItem(100, ItemID.Elixir_of_Wrath, Enums.TargetingType.ProximityAlly, float.MaxValue,
                new[] {Enums.ActivationType.CheckOnlyOnMe }, 
                "ElixirOfWrath"),
            
            // item: Elixir_of_Sorcery
            new ActiveItem(100, ItemID.Elixir_of_Sorcery, Enums.TargetingType.ProximityAlly, float.MaxValue,
                new[] {Enums.ActivationType.CheckOnlyOnMe }, 
                "ElixirOfSorcery"),
            
            // item: Your_Cut (Pyke Assist)
            new ActiveItem(100, ItemID.Your_Cut, Enums.TargetingType.ProximityAlly, float.MaxValue,
                new[] { Enums.ActivationType.CheckOnlyOnMe }),
        };

        private static void Initialize()
        {
            var consumablesItemMenu = new Tab("Trinity: Regen");

            foreach (var item in ConsumableItems)
            {
                item.OnItemInitialize += () => InitializedTickItems.Add(item);
                item.OnItemDispose += () => InitializedTickItems.Remove(item);
                item.Initialize(consumablesItemMenu);
            }

            MenuManager.AddTab(consumablesItemMenu);

            var cleanseItemMenu = new Tab("Trinity: Cleanse");

            foreach (var item in CleanseItems)
            {
                item.OnItemInitialize += () => InitializedTickItems.Add(item);
                item.OnItemDispose += () => InitializedTickItems.Remove(item);
                item.Initialize(cleanseItemMenu);
            }

            MenuManager.AddTab(cleanseItemMenu);
        }

        private static void ReCacheHeroes()
        {
            foreach (var u in ObjectManagerExport.HeroCollection)
            {
                var hero = u.Value;
                if (hero.IsAlive)
                {
                    AllChampions.Add(new Champion(hero));
                }
            }
        }

        private static async Task CoreEvents_OnCoreMainTick()
        {
            foreach (var initializedNormalTickItem in InitializedTickItems)
            {
                initializedNormalTickItem.OnTick();
            }
        }
    }

}
