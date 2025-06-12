using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShipyardLib
{
    public class CustomShipyard : MonoBehaviour
    {   //this component enables the custom shipyard system for the boat entering the shipyard

        [Header("Coloring")]
        [Tooltip("Names for the color groups")]
        public string[] groupNames;
        [Tooltip("All objects sharing these materials will be recolored toghether")]
        public Renderer[] renderers;
        [Tooltip("All objects listed here will be recolored toghether")]
        public GameObject[] objects0;
        [Tooltip("All objects listed here will be recolored toghether")]
        public GameObject[] objects1;
        [Tooltip("All objects listed here will be recolored toghether")]
        public GameObject[] objects2;
        [Tooltip("All objects listed here will be recolored toghether")]
        public GameObject[] objects3;
        [Tooltip("All objects listed here will be recolored toghether")]
        public GameObject[] objects4;

        private GameObject[][] objects;

        [HideInInspector] public ColorGroup[] colorGroups;  //Can't use this directly because of a Unity issue with serializable classes in custom dlls
                                                            //see https://issuetracker.unity3d.com/issues/assetbundle-is-not-loaded-correctly-when-they-reference-a-script-in-custom-dll-which-contains-system-dot-serializable-in-the-build
        [Header("Extra Categories")]
        [Tooltip("Extra category names")]
        public string[] categories;

        public void Awake()
        {
            colorGroups = new ColorGroup[5];
            objects = new GameObject[5][];

            objects[0] = objects0;
            objects[1] = objects1;
            objects[2] = objects2;
            objects[3] = objects3;
            objects[4] = objects4;

            for (int i = 0; i < colorGroups.Length; i++)
            {
                ColorGroup cg = new ColorGroup(groupNames[i], renderers[i], objects[i]);
                if (cg != null) cg.CacheDefault();
                colorGroups[i] = cg;
            }
        }
    }
    [Serializable]
    public class ColorGroup
    {
        [Tooltip("Name for the color group")]
        public string groupName;
        [Tooltip("All objects sharing this materials will be recolored")]
        public Renderer renderer;
        [Tooltip("All objects listed here will be recolored")]
        public GameObject[] objects;
        
        [HideInInspector] public Color sharedDefault;
        [HideInInspector] public Color[] objectsDefault;

        public ColorGroup(string s, Renderer r, GameObject[] o)
        {
            renderer = r;
            objects = o;
            groupName = s;
        }
        
        public void CacheDefault()
        {
            if (renderer != null) sharedDefault = renderer.sharedMaterial.GetColor("_Color");
            objectsDefault = new Color[objects.Length];

            if (objects.Length != 0)
            {
                for (int i = 0; i < objects.Length; i++)
                {
                    Renderer ren = objects[i].GetComponent<Renderer>();
                    if (ren.sharedMaterials.Length > 1) objectsDefault[i] = ren.sharedMaterials[1].GetColor("_Color");  //hull
                    else objectsDefault[i] = ren.sharedMaterial.GetColor("_Color");                                     //everything else
                }
            }
        }
    }
}
