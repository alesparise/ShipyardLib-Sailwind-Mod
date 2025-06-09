using System.Collections.Generic;
using UnityEngine;

namespace ShipyardLib
{   ///helper class to store all references to the various bits of the Custom Shipyard UI and other useful UI pieces stuff
    public class CustomUI : MonoBehaviour
    {
        private Transform t;
        public List<GameObject> vanillaUI;
        public Slider slider1;      // We have three sliders at most, e.g. 
        public Slider slider2;      // mast height, position, rake, rgb colors, sail width, height, rotation
        public Slider slider3;      //we only need two for stays attachments

        public CatButton button1;
        public CatButton button2;
        public CatButton button3;
        public CatButton button4;
        public CatButton button5;

        public void Awake()
        {
            t = transform;
            vanillaUI = new List<GameObject>();
            foreach (Transform child in transform.parent)
            {   //store reference to the vanilla part bits so we can close them when clicking a custom category
                if (child != null && child != t)
                {
                    switch (child.name)
                    {
                        case ("panel Sails Menu"):
                            vanillaUI.Add(child.gameObject);
                            break;
                        case ("panel Boat Parts (Other)"):
                            vanillaUI.Add(child.gameObject);
                            break;
                        case ("panel NEW Boat Parts (EVERYTHING) (1)"):
                            vanillaUI.Add(child.gameObject);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        private void CloseVanillaUI()
        {   //close vanilla UI parts we don't need
            ShipyardUI.instance.CloseSailCategoryMenu();
            foreach (GameObject obj in vanillaUI)
            {
                obj.SetActive(false);
            }
        }
        private void OpenVanillaUI()
        {   //open back the vanilla ui bits
            vanillaUI[0]?.SetActive(true);
        }
        
        ///PLAN: the object with this script attached is the anchor for the modded UI.
        ///Sliders for mast and stays go into a specific parchments up top (I should try make this customizable for modders)
        ///Extra categories button should probably go over the middle parchment, so they are not in the way
        ///I need a way to "disable" and "enable" the vanilla UI at will (rip it from DynamicShipyard?)
        ///Stays scaling and mast moving happens in the SAIL panel, when you select the mast or stay it will allow you to move it, 
        ///unless you click the add sail button, or select a sail I guess
        ///Based on what extra categories the boat needs, the correct amount of buttons should appear
    }
}
