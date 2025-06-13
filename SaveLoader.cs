using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShipyardLib
{   ///Save and load the data for the ShipyardLib, like the colors
    public static class SaveLoader
    {
        public static List<CustomShipyard> customShipyards;

        public static void SaveCustomShipyard()
        {   //patch to save mod data
            foreach (CustomShipyard cs in customShipyards)
            {
                //store data
                SaveData saveData = new SaveData(cs.colorGroups, cs.boatIndex);
                //create key
                string key = ShipyardLibPatches.shortName + "." + cs.boatIndex;
                //serialize data
                GameState.modData[key] = JsonUtility.ToJson(saveData);

                Debug.LogWarning("ShipyardLib: saved data for " + cs.boatIndex);
            }
        }
        public static void LoadCustomShipyard()
        {   //patch to load mod data

            string prefix = ShipyardLibPatches.shortName + ".";

            foreach (var kvp in GameState.modData)
            {
                if (kvp.Key.StartsWith(prefix))
                {
                    Debug.LogWarning("found a shipyardLib key: " + kvp.Key);
                    Debug.LogWarning("value is: " + kvp.Value);

                    //deserialize
                    string boatIndexStr = kvp.Key.Substring(prefix.Length);
                    int index = int.Parse(boatIndexStr);
                    SaveData data = JsonUtility.FromJson<SaveData>(kvp.Value);

                    //find the right boat
                    CustomShipyard[] css = GameObject.FindObjectsOfType<CustomShipyard>();
                    CustomShipyard rightBoat = null;
                    foreach (CustomShipyard c in css)
                    {
                        if (c.boatIndex == index) rightBoat = c;
                    }
                    if (rightBoat == null) return;  //No boat found with the right thing

                    //assign modded data to the boat
                    rightBoat.colorGroups = data.colorGroups;
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
            Debug.LogWarning("ShipyardLib: loaded data!");
        }
    }
    [Serializable]
    public class SaveData
    {
        public ColorGroup[] colorGroups;

        public int boatIndex;

        public SaveData(ColorGroup[] cgs, int i)
        {
            colorGroups = cgs;
            boatIndex = i;
        }
    }
}
