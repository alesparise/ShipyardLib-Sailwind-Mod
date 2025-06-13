using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShipyardLib
{   ///Save and load the data for the ShipyardLib, like the colors
    public static class SaveLoader
    {
        public static List<CustomShipyard> customShipyards;

        public const string colorKeyPrefix = ShipyardLibPatches.shortName + ".color.";

        public static void SaveCustomShipyard()
        {   //patch to save mod data
            foreach (CustomShipyard cs in customShipyards)
            {
                //create key
                string key = colorKeyPrefix + cs.boatIndex;
                //serialize data
                string serialized = SerializeColorGroupArray(cs.colorGroups);
                GameState.modData[key] = serialized;
            }
        }
        public static void LoadCustomShipyard()
        {   //patch to load mod data
            foreach (var kvp in GameState.modData)
            {
                if (kvp.Key.StartsWith(colorKeyPrefix))
                {
                    //deserialize
                    string boatIndexStr = kvp.Key.Substring(colorKeyPrefix.Length);
                    int index = int.Parse(boatIndexStr);
                    ColorGroup[] colorGroups = DeserializeColorGroupArray(kvp.Value);

                    //find the right boat
                    CustomShipyard[] css = GameObject.FindObjectsOfType<CustomShipyard>();
                    CustomShipyard rightBoat = null;
                    foreach (CustomShipyard c in css)
                    {
                        if (c.boatIndex == index) rightBoat = c;
                    }
                    if (rightBoat == null) return;  //No boat found with the right thing

                    //assign modded data to the boat
                    rightBoat.colorGroups = colorGroups;
                    foreach (ColorGroup cg in rightBoat.colorGroups)
                    {   //apply saved colors
                        Color color = cg.currentColor;
                        if (cg == null) return;
                        if (cg.material != null) CustomUI.instance.colorizer.Colorize(color, cg.material);
                        if (cg.objects != null && cg.objects.Length > 0) CustomUI.instance.colorizer.Colorize(color, cg.objects);
                        cg.currentColor = color;
                    }
                }
            }
        }
        private static string SerializeColorGroupArray(ColorGroup[] cgs)
        {   //serialize the ColorGroup array into a string where each ColorGroup is separated by a $ symbol
            string s = "";
            foreach (ColorGroup cg in cgs)
            {
                s += JsonUtility.ToJson(cg) + '$';
            }

            return s;
        }
        private static ColorGroup[] DeserializeColorGroupArray(string serialized)
        {
            string[] substrings = serialized.Split('$');    //split at the closing $

            Array.Resize(ref substrings, substrings.Length - 1); //scrap the last one, since it's extra

            ColorGroup[] cgs = new ColorGroup[substrings.Length];

            //issue, this gets 6 strings instead of 5, also the first one is empty
            for (int i = 0; i < substrings.Length; i++)
            {
                ColorGroup cg = JsonUtility.FromJson<ColorGroup>(substrings[i]);
                cgs[i] = cg;
            }

            return cgs;
        }
    }
}
