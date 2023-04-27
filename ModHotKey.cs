// Thanks to Yoraiz0r for the useful Properties approach.
namespace Terraria.ModLoader
{
    using System.Collections.Generic;

    using Terraria.GameInput;

    public class ModHotKey
    {
        #region Fields

        internal readonly string defaultKey; // from mod.Load
        internal readonly string displayName; // display name: "Example Mod: Random Buff" -- unique AKA _keyName
        internal readonly Mod mod;
        internal readonly string name; // name from modder: "Random Buff"

        #endregion Fields

        #region Constructors

        internal ModHotKey(Mod mod, string name, string defaultKey)
        {
            this.mod = mod;
            this.name = name;
            this.defaultKey = defaultKey;
            this.displayName = mod.Name + ": " + name;
        }

        #endregion Constructors

        #region Properties

        public bool Current
        {
            get { return PlayerInput.Triggers.Current.KeyStatus[displayName]; }
        }

        public bool JustPressed
        {
            get { return PlayerInput.Triggers.JustPressed.KeyStatus[displayName]; }
        }

        public bool JustReleased
        {
            get { return PlayerInput.Triggers.JustReleased.KeyStatus[displayName]; }
        }

        public bool Old
        {
            get { return PlayerInput.Triggers.Old.KeyStatus[displayName]; }
        }

        public bool RetroCurrent
        {
            get
            {
                if (Main.drawingPlayerChat || Main.player[Main.myPlayer].talkNPC != -1 || Main.player[Main.myPlayer].sign != -1) return false;
                return Current;
            }
        }

        #endregion Properties

        #region Methods

        // Gets the currently assigned keybindings. Useful for prompts, tooltips, informing users.
        public List<string> GetAssignedKeys(InputMode mode = InputMode.Keyboard)
        {
            return PlayerInput.CurrentProfile.InputModes[mode].KeyStatus[displayName];
        }

        #endregion Methods
    }
}