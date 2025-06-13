using BepInEx;
using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;

namespace ShipyardLib
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    public class ShipyardLibPatches : BaseUnityPlugin
    {
        public const string pluginGuid = "pr0skynesis.shipyardLib";
        public const string pluginName = "Shipyard Lib";
        public const string pluginVersion = "1.0.0";
        public const string shortName = "pr0.shipyardLib";
        public void Awake()
        {
            Harmony harmony = new Harmony(pluginGuid);
            //initial setup
            MethodInfo setupOg = AccessTools.Method(typeof(FloatingOriginManager), "Start");
            MethodInfo setupP = AccessTools.Method(typeof(ShipyardLibPatches), "Setup");
            harmony.Patch(setupOg, new HarmonyMethod(setupP));

            //enable modded menu
            MethodInfo shipCheckOG = AccessTools.Method(typeof(Shipyard), "ToggleMenu");
            MethodInfo shipCheckP = AccessTools.Method(typeof(ShipyardLibPatches), "EnableCustomShipyard");
            harmony.Patch(shipCheckOG, new HarmonyMethod(shipCheckP));

            //vanilla category button patches
            MethodInfo buttonOG = AccessTools.Method(typeof(ShipyardButton), "OnActivate");
            MethodInfo buttonP = AccessTools.Method(typeof(ShipyardLibPatches), "CloseModdedPanels");
            harmony.Patch(buttonOG, new HarmonyMethod(buttonP));

            //patching ShipyardExpansion
            //this is pretty janky, but it works without adding the reference to Shipyard Expansion!
            #region SE patch
            string typeName = "ShipyardExpansion.ShipyardUIPatches";

            Assembly seAssembly;
            Type t = null;
            foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
            {
                t = ass.GetType(typeName);
                if (t != null)
                {
                    seAssembly = ass;
                    Debug.LogWarning("Found SE!");
                    break;
                }
            }
            if (t != null)
            {   //only run this if the correct type is found
                MethodInfo seOG = AccessTools.Method(t, "RefreshPatch");
                MethodInfo seP = AccessTools.Method(typeof(ShipyardLibPatches), "SEPatch");
                harmony.Patch(seOG, new HarmonyMethod(seP));
            }
            #endregion

            //save mod data
            MethodInfo saveOG = AccessTools.Method(typeof(SaveLoadManager), "SaveModData");
            MethodInfo saveP = AccessTools.Method(typeof(SaveLoader), "SaveCustomShipyard");
            harmony.Patch(saveOG, new HarmonyMethod(saveP));

            //load mod data
            MethodInfo loadOG = AccessTools.Method(typeof(SaveLoadManager), "LoadModData");
            MethodInfo loadP = AccessTools.Method(typeof(SaveLoader), "LoadCustomShipyard");
            harmony.Patch(loadOG, new HarmonyMethod(loadP));
        }
        public static void Setup() => ShipyardHelpers.Setup();
        public static void EnableCustomShipyard(bool state, Shipyard __instance)
        {   //runs the check to see if the modded ui is to be enabled
            CustomShipyard cs = __instance.GetCurrentBoat()?.GetComponent<CustomShipyard>();
            if (!state) return; 
            if (cs == null) return;
            Debug.LogWarning("Toggling moddedUI for " + cs.name);
            ShipyardHelpers.ToggleUI(state, cs);
        }
        public static void CloseModdedPanels(ShipyardButton __instance)
        {   //patch ShipyardButton so that we can close all modded panels when clicking a vanilla category
            if (__instance.function == ShipyardButton.ButtonFunction.changeCategory)
            {
                CustomUI.instance.CloseAllPanels();
            }
        }
        public static bool SEPatch()
        {
            CustomShipyard cs = GameState.currentShipyard.GetCurrentBoat().GetComponent<CustomShipyard>();
            if (cs != null) return false;
            Debug.LogWarning("Patching SE");
            return true;
        }
    }
}
