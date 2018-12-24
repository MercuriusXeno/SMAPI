using System;
using StardewModdingAPI.Events;

namespace StardewModdingAPI.Framework.Events
{
    /// <summary>Events serving specialised edge cases that shouldn't be used by most mods.</summary>
    internal class ModSpecialisedEvents : ModEventsBase, ISpecialisedEvents
    {
        /*********
        ** Accessors
        *********/
        /// <summary>Raised immediately after the player loads a save slot, but before the world is fully initialised. The save and game data are available at this point, but some in-game content (like location maps) haven't been initialised yet.</summary>
        public event EventHandler<SavePreloadedEventArgs> SavePreloaded
        {
            add => this.EventManager.SavePreloaded.Add(value);
            remove => this.EventManager.SavePreloaded.Remove(value);
        }

        /// <summary>Raised before the game state is updated (≈60 times per second), regardless of normal SMAPI validation. This event is not thread-safe and may be invoked while game logic is running asynchronously. Changes to game state in this method may crash the game or corrupt an in-progress save. Do not use this event unless you're fully aware of the context in which your code will be run. Mods using this event will trigger a stability warning in the SMAPI console.</summary>
        public event EventHandler<UnvalidatedUpdateTickingEventArgs> UnvalidatedUpdateTicking
        {
            add => this.EventManager.UnvalidatedUpdateTicking.Add(value);
            remove => this.EventManager.UnvalidatedUpdateTicking.Remove(value);
        }

        /// <summary>Raised after the game state is updated (≈60 times per second), regardless of normal SMAPI validation. This event is not thread-safe and may be invoked while game logic is running asynchronously. Changes to game state in this method may crash the game or corrupt an in-progress save. Do not use this event unless you're fully aware of the context in which your code will be run. Mods using this event will trigger a stability warning in the SMAPI console.</summary>
        public event EventHandler<UnvalidatedUpdateTickedEventArgs> UnvalidatedUpdateTicked
        {
            add => this.EventManager.UnvalidatedUpdateTicked.Add(value);
            remove => this.EventManager.UnvalidatedUpdateTicked.Remove(value);
        }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="mod">The mod which uses this instance.</param>
        /// <param name="eventManager">The underlying event manager.</param>
        internal ModSpecialisedEvents(IModMetadata mod, EventManager eventManager)
            : base(mod, eventManager) { }
    }
}
