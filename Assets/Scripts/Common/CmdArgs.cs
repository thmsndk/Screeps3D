using Assets.Scripts.Common.SettingsManagement;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Common
{
    static class CmdArgs
    {
        /// <summary>
        /// Flags the enabled/disabled state feature for idle camera
        /// </summary>
        public static bool DisableCameraIdle { get; private set; } = false;

        /// <summary>
        /// Flags the enabled/disabled state for twitch controls
        /// </summary>
        public static bool ForceEnableTwitch { get; private set; } = false;

        /// <summary>
        /// The timer between switching pvp views
        /// </summary>
        [Setting("Gameplay/Spectate", "PVP Spectate Timer", "Change the time between room swap in PVP spectate mode")]
        public static int PvPTimerSwitch { get; private set; } = 60;

        /// <summary>
        /// The timer for switching in spectate mode
        /// </summary>
        [Setting("Gameplay/Spectate", "Spectate Timer", "Change the time between room swap in spectate mode")]
        public static int SpectateTimerSwitch { get; private set; } = 60;

        /// <summary>
        /// Dragon names!?
        /// </summary>
        public static string[] Dragons { get; private set; }

        /// <summary>
        /// Flag to let us know if we need to process the arguments
        /// </summary>
        private static bool _loadedArgs = false;

        /// <summary>
        /// Loads any arguments in to the to the getter switches above
        /// </summary>
        static CmdArgs()
        {
            // Only process if we've not already loaded the args
            if (!_loadedArgs)
            {
                var args = Environment.GetCommandLineArgs();

                foreach (var arg in args)
                {
                    // Parse the argument to get the key value paris, if there is only a key value, then the value is true
                    var kvp = arg.Contains('=') ? new KeyValuePair<string, string>(arg.Split('=')[0], arg.Split('=')[1]) : new KeyValuePair<string, string>(arg, "true");
                    
                    var key = kvp.Key.ToLowerInvariant().Replace("-", string.Empty);

                    // Switch over the commands and assign any required values
                    switch (key)
                    {
                        case "disablecameraidle":
                            UnityEngine.Debug.Log($"Handling: {kvp.Key} with value {kvp.Value}");
                            DisableCameraIdle = true;
                            break;

                        case "twitch":
                            UnityEngine.Debug.Log($"Handling: {kvp.Key} with value {kvp.Value}");
                            ForceEnableTwitch = true;
                            break;

                        case "specatetimwerswitch":
                            UnityEngine.Debug.Log($"Handling: {kvp.Key} with value {kvp.Value}");
                            SpectateTimerSwitch = GetInterval(kvp);
                            break;

                        case "pvptimerswitch":
                            UnityEngine.Debug.Log($"Handling: {kvp.Key} with value {kvp.Value}");
                            PvPTimerSwitch = GetInterval(kvp);
                            break;

                        case "dragons":
                            UnityEngine.Debug.Log($"Handling: {kvp.Key} with value(s) {kvp.Value}");
                            Dragons = kvp.Value.Split(';');
                            break;
                    }
                }

                _loadedArgs = true;
            }
        }

        /// <summary>
        /// Takes a key value pair and converts the value to an int, defaulting the value if conversion fails
        /// </summary>
        /// <param name="kvp">The Key Value Pair to parse</param>
        /// <returns>An int value representing the number of seconds</returns>
        private static int GetInterval(KeyValuePair<string, string> kvp)
        {
            int value;

            if (!int.TryParse(kvp.Value, out value))
            {
                // Log error with parsing argument to correct output
                UnityEngine.Debug.LogWarning($"Detected arg for {kvp.Key} but could not parse the value {kvp.Value}");

                // Set default of 60 seconds for switching
                value = 60;
            }

            return value;
        }
    }
}
