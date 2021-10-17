using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using StardewModdingAPI.Internal.Patching;
using StardewValley.GameData;
using StardewValley.GameData.HomeRenovations;
using StardewValley.GameData.Movies;

namespace StardewModdingAPI.Mods.ErrorHandler.Patches
{
    /// <summary>Harmony patches for <see cref="Dictionary{TKey,TValue}"/> which add the accessed key to <see cref="KeyNotFoundException"/> exceptions.</summary>
    /// <remarks>Patch methods must be static for Harmony to work correctly. See the Harmony documentation before renaming patch arguments.</remarks>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Argument names are defined by Harmony and methods are named for clarity.")]
    [SuppressMessage("ReSharper", "IdentifierTypo", Justification = "Argument names are defined by Harmony and methods are named for clarity.")]
    internal class DictionaryPatcher : BasePatcher
    {
        /*********
        ** Fields
        *********/
        /// <summary>Simplifies access to private code.</summary>
        private static IReflectionHelper Reflection;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="reflector">Simplifies access to private code.</param>
        public DictionaryPatcher(IReflectionHelper reflector)
        {
            DictionaryPatcher.Reflection = reflector;
        }

        /// <inheritdoc />
        public override void Apply(Harmony harmony, IMonitor monitor)
        {
            Type[] keyTypes = { typeof(int), typeof(string) };
            Type[] valueTypes = { typeof(int), typeof(string), typeof(HomeRenovation), typeof(MovieData), typeof(SpecialOrderData) };

            foreach (Type keyType in keyTypes)
            {
                foreach (Type valueType in valueTypes)
                {
                    Type dictionaryType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);

                    harmony.Patch(
                        original: AccessTools.Method(dictionaryType, "get_Item") ?? throw new InvalidOperationException($"Can't find method {PatchHelper.GetMethodString(dictionaryType, "get_Item")} to patch."),
                        finalizer: this.GetHarmonyMethod(nameof(DictionaryPatcher.Finalize_GetItem))
                    );

                    harmony.Patch(
                        original: AccessTools.Method(dictionaryType, "Add") ?? throw new InvalidOperationException($"Can't find method {PatchHelper.GetMethodString(dictionaryType, "Add")} to patch."),
                        finalizer: this.GetHarmonyMethod(nameof(DictionaryPatcher.Finalize_Add))
                    );
                }
            }
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method to call after the dictionary indexer throws an exception.</summary>
        /// <param name="key">The dictionary key being fetched.</param>
        /// <param name="__exception">The exception thrown by the wrapped method, if any.</param>
        /// <returns>Returns the exception to throw, if any.</returns>
        private static Exception Finalize_GetItem(object key, Exception __exception)
        {
            if (__exception is KeyNotFoundException)
                DictionaryPatcher.AddKey(__exception, key);

            return __exception;
        }

        /// <summary>The method to call after a dictionary insert throws an exception.</summary>
        /// <param name="key">The dictionary key being inserted.</param>
        /// <param name="__exception">The exception thrown by the wrapped method, if any.</param>
        /// <returns>Returns the exception to throw, if any.</returns>
        private static Exception Finalize_Add(object key, Exception __exception)
        {
            if (__exception is ArgumentException)
                DictionaryPatcher.AddKey(__exception, key);

            return __exception;
        }

        /// <summary>Add the dictionary key to an exception message.</summary>
        /// <param name="exception">The exception whose message to edit.</param>
        /// <param name="key">The dictionary key.</param>
        private static void AddKey(Exception exception, object key)
        {
            DictionaryPatcher.Reflection
                .GetField<string>(exception, "_message")
                .SetValue($"{exception.Message}\nkey: '{key}'");
        }
    }
}
