﻿namespace Trinity.Spells
{
    #region

    using System.Collections.Generic;
    using Helpers;
    using Oasys.Common.Enums.GameEnums;
    using Oasys.Common.GameObject.Clients.ExtendedInstances.Spells;
    using Oasys.Common.Menu;
    using Oasys.Common.Menu.ItemComponents;
    using Oasys.SDK;
    using Oasys.SDK.SpellCasting;
    using Oasys.SDK.Tools;

    #endregion

    public delegate void OnSpellInitialize();
    public delegate void OnSpellDispose();

    public abstract class AutoSpellBase
    {
        #region Fields

        private bool _disposed;
        private bool _initialized;

        /// <summary>
        /// The spell counters
        /// </summary>
        public Dictionary<string, Counter> SpellCounter = new();

        /// <summary>
        /// The spell groups
        /// </summary>
        public Dictionary<string, Group> SpellGroup = new();

        /// <summary>
        /// The spell switchs
        /// </summary>
        public Dictionary<string, Switch> SpellSwitch = new();

        #endregion

        #region Properties and Encapsulation

        /// <summary>
        ///     Gets or sets the slot.
        /// </summary>
        /// <value>
        ///     The slot.
        /// </value>
        public CastSlot Slot { get; set; }

        /// <summary>
        ///     Gets or sets the range.
        /// </summary>
        /// <value>
        ///     The range.
        /// </value>
        public float Range { get; set; }

        /// <summary>
        ///     Gets or sets the spell class.
        /// </summary>
        /// <value>
        ///     The spell class.
        /// </value>
        public SpellClass SpellClass { get; set; }

        /// <summary>
        ///     Gets or sets the name of the champion.
        /// </summary>
        /// <value>
        ///     The name of the champion.
        /// </value>
        public string ChampionName { get; set; }

        /// <summary>
        ///     Gets or sets the name of the spell.
        /// </summary>
        /// <value>
        ///     The name of the spell.
        /// </value>
        public string SpellName { get; set; }

        /// <summary>
        ///     Gets or sets the spell tab.
        /// </summary>
        /// <value>
        ///     The spell tab.
        /// </value>
        public Tab SpellTab { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Initializes the specified parent tab/item.
        /// </summary>
        /// <param name="parentTab">The parent tab.</param>
        public void Initialize(Tab parentTab)
        {
            SpellTab = parentTab;

            if (UnitManager.MyChampion.ModelName == ChampionName)
                InitializeSpell();
            else
                switch (string.IsNullOrEmpty(SpellName))
                {
                    case false:
                        InitializeSpell();
                        break;
                    default:
                        DisposeSpell();
                        break;
                }
        }

        /// <summary>
        ///     Initializes the spell.
        /// </summary>
        public virtual void InitializeSpell()
        {
            if (_initialized) return;

            _disposed = false;
            _initialized = true;

            switch (string.IsNullOrEmpty(SpellName))
            {
                case false:
                    this.GetSpellClassByName();
                    break;
                default:
                    SpellClass = UnitManager.MyChampion.GetSpellBook().GetSpellClass((SpellSlot) Slot - 16);
                    break;
            }

            if (SpellClass != null)
            {
                CreateTab();
                OnSpellInitialize?.Invoke();
                Logger.Log("Initialized " + ChampionName + " " + Slot);
            }
            else
            {
                DisposeSpell();
            }
        }

        /// <summary>
        ///     Disposes the spell.
        /// </summary>
        public virtual void DisposeSpell()
        {
            if (_disposed) return;

            _disposed = true;
            _initialized = false;

            SpellClass = null;
            //Logger.Log("Disposed " + ChampionName + " " + Slot);
            OnSpellDispose?.Invoke();
        }

        public abstract void OnTick();
        public abstract void CreateTab();

        #endregion

        #region Public Events

        public event OnSpellInitialize OnSpellInitialize;
        public event OnSpellDispose OnSpellDispose;

        #endregion
    }
}