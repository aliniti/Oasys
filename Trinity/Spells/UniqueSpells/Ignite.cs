﻿namespace Trinity.Spells.UniqueSpells
{
    #region

    using System.Linq;
    using Helpers;
    using Oasys.Common;
    using Oasys.Common.Extensions;
    using Oasys.Common.GameObject.Clients;
    using Oasys.Common.Menu.ItemComponents;
    using Oasys.SDK;

    #endregion

    internal class Ignite : AutoSpell
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Ignite" /> class.
        /// </summary>
        /// <param name="usePct">The use PCT.</param>
        /// <param name="championName">Name of the champion.</param>
        /// <param name="spellName">Name of the spell.</param>
        /// <param name="tType">Type of the t.</param>
        /// <param name="range">The range.</param>
        /// <param name="aType">a type.</param>
        public Ignite(int usePct, string championName, string spellName, TargetingType tType, float range, ActivationType[] aType)
            : base(usePct, championName, spellName, tType, range, aType)
        {
        }

        #endregion

        #region Override Methods

        /// <summary>
        ///     Creates the tab.
        /// </summary>
        public override void CreateTab()
        {
            SpellSwitch["igcombo"] = new Switch
            {
                IsOn = true,
                Title = "Ignite on Combo"
            };

            SpellTab.AddItem(SpellSwitch["igcombo"]);

            SpellCounter["igminhp"] = new Counter
            {
                Title = "Ignite Min Target HP (%)",
                MaxValue = 100,
                MinValue = 5,
                Value = 25,
                ValueFrequency = 5
            };

            SpellTab.AddItem(SpellCounter["igminhp"]);

            SpellCounter["igmaxhp"] = new Counter
            {
                Title = "Ignite Max Target HP (%)",
                MaxValue = 100,
                MinValue = 5,
                Value = 70,
                ValueFrequency = 5
            };

            SpellTab.AddItem(SpellCounter["igmaxhp"]);

            SpellSwitch["igks"] = new Switch
            {
                IsOn = false,
                Title = "Ignite on KS"
            };

            SpellTab.AddItem(SpellSwitch["igks"]);
        }

        /// <summary>
        ///     Called when [OnTick].
        /// </summary>
        public override void OnTick()
        {
            if (!SpellClass.IsSpellReady) return;

            if (SpellSwitch["igcombo"].IsOn)
                if (TargetSelector.GetBestChampionTarget() is AIHeroClient target && target.IsValidTarget(Range))
                {
                    var hpPct = target.HealthPercent;
                    if (hpPct > SpellCounter["igminhp"].Value &&
                        hpPct < SpellCounter["igmaxhp"].Value)
                        this.UseSpell(target);
                }

            if (SpellSwitch["igks"].IsOn)
                foreach (var unit in Bootstrap.Enemies.Select(x => x.Value))
                {
                    var igniteDmg = (float) 50 + 20 * ObjectManagerExport.LocalPlayer.Level;
                    if (igniteDmg > unit.Instance.Health) this.UseSpell(unit.Instance);
                }
        }

        #endregion
    }
}