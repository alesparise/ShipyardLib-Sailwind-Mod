using System.Collections.Generic;
using UnityEngine;

namespace ShipyardLib
{   ///helper class to store all references to the various bits of the Custom Shipyard UI and other useful UI pieces stuff
    public class CustomUI : MonoBehaviour
    {
        private Transform t;

        [HideInInspector] public List<GameObject> openedPanels;
        [HideInInspector] public List<GameObject> vanillaUI;

        [Header("Category Buttons")]
        public GameObject[] catButtons;

        [Header("Color Buttons")]
        public GameObject[] colorButtons;

        [Header("Selection Buttons")]
        public GameObject[] selectionButtons;
        public GameObject[] resetButtons;
        public Material clickedMaterial;

        [Header("Panels")]
        public GameObject sliderPanel;
        public GameObject colorPanel;
        public GameObject selectionPanel;


        public Slider slider1;      // We have three sliders at most, e.g. 
        public Slider slider2;      // mast height, position, rake, rgb colors, sail width, height, rotation
        public Slider slider3;      // we only need two for stays attachments

        [HideInInspector] public Colorizer colorizer;

        [HideInInspector] public CustomShipyard customShipyard;

        [HideInInspector] public ColorGroup target;

        public static CustomUI instance;

        private Vector3[] widePos = {           //stores the position to use if the UI is wide (NAND Tweaks)
            new Vector3(13.2f, 5.95f, 9.9f),    //slider panel
            new Vector3(13.2f, 5.95f, 9.9f),    //color panel
            new Vector3(3.5f, -0.4f, 0f)        //selector panel
        };

        private Vector3[] standardPos = new Vector3[3]; //regular width ui

        #region Colors
        private readonly Color[] grcColors = {
            new Color (1f, 1f, 1f, 1f),
            new Color (1f, 1f, 1f, 1f),
            new Color (1f, 1f, 1f, 1f),
            new Color (1f, 1f, 1f, 1f),

            new Color (1f, 1f, 1f, 1f),
            new Color (1f, 1f, 1f, 1f),
            new Color (1f, 1f, 1f, 1f),
            new Color (1f, 1f, 1f, 1f),

            new Color (1f, 1f, 1f, 1f),
            new Color (1f, 1f, 1f, 1f),
            new Color (1f, 1f, 1f, 1f),
            new Color (1f, 1f, 1f, 1f)
        };
        private readonly Color[] dcColors = {
            new Color (1f, 1f, 1f, 1f),
            new Color (1f, 1f, 1f, 1f),
            new Color (1f, 1f, 1f, 1f),
            new Color (1f, 1f, 1f, 1f),

            new Color (1f, 1f, 1f, 1f),
            new Color (1f, 1f, 1f, 1f),
            new Color (1f, 1f, 1f, 1f),
            new Color (1f, 1f, 1f, 1f),

            new Color (1f, 1f, 1f, 1f),
            new Color (1f, 1f, 1f, 1f),
            new Color (1f, 1f, 1f, 1f),
            new Color (1f, 1f, 1f, 1f)
        };
        private readonly Color[] faColors = {
            new Color (1f, 1f, 1f, 1f),
            new Color (1f, 1f, 1f, 1f),
            new Color (1f, 1f, 1f, 1f),
            new Color (1f, 1f, 1f, 1f),

            new Color (1f, 1f, 1f, 1f),
            new Color (1f, 1f, 1f, 1f),
            new Color (1f, 1f, 1f, 1f),
            new Color (1f, 1f, 1f, 1f),

            new Color (1f, 1f, 1f, 1f),
            new Color (1f, 1f, 1f, 1f),
            new Color (1f, 1f, 1f, 1f),
            new Color (1f, 1f, 1f, 1f)
        };
        private readonly Color[] kColors = {
            new Color (0f, 0f, 0f, 1f),
            new Color (1f, 1f, 1f, 1f),
            new Color (0f, 0f, 1f, 1f),
            new Color (0f, 1f, 0f, 1f),

            new Color (1f, 0f, 0f, 1f),
            new Color (.5f, .5f, .5f, 1f),
            new Color (.5f, 0f, .5f, 1f),
            new Color (.5f, .5f, 0f, 1f),

            new Color (0f, .5f, .5f, 1f),
            new Color (.25f, .5f, .25f, 1f),
            new Color (.5f, .25f, .25f, 1f),
            new Color (.25f, .25f, .5f, 1f)
        };
        #endregion

        public void Awake()
        {
            Debug.LogWarning("CustomUI Awakening");
            t = transform;
            openedPanels = new List<GameObject>();

            colorizer = gameObject.AddComponent<Colorizer>();
            target = new ColorGroup();

            vanillaUI = new List<GameObject>();
            Transform baseUI = ShipyardUI.instance?.transform.Find("UI");
            foreach (Transform child in baseUI)
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
            sliderPanel.SetActive(false);
            colorPanel.SetActive(false);
            selectionPanel.SetActive(false);

            standardPos[0] = sliderPanel.transform.localPosition;
            standardPos[1] = colorPanel.transform.localPosition;
            standardPos[2] = selectionPanel.transform.localPosition;

            instance = this;
        }
        public void CloseVanillaUI()
        {   //close vanilla UI parts we don't need
            ShipyardUI.instance.CloseSailCategoryMenu();
            foreach (GameObject obj in vanillaUI)
            {
                obj.SetActive(false);
            }
        }
        public void OpenVanillaUI()
        {   //open back the vanilla ui bits
            vanillaUI[0]?.SetActive(true);
        }
        public void OpenCustomUI(bool isWide, CustomShipyard cs)
        {
            if (isWide)    //DEBUG, flipped the check to test things since I have NAND tweaks...
            {
                //move each bit where it should go
                sliderPanel.transform.localPosition = widePos[0];
                colorPanel.transform.localPosition = widePos[1];
                selectionPanel.transform.localPosition = widePos[2];
            }
            else
            {   //standard width ui
                sliderPanel.transform.localPosition = standardPos[0];
                colorPanel.transform.localPosition = standardPos[1];
                selectionPanel.transform.localPosition = standardPos[2];
            }
            //1) Set category buttons names
            int catNumber = cs.categories.Length;
            for (int i = 0; i < catNumber; i++)
            {
                string cat = cs.categories[i];
                if (cat == "Color")
                {   //make this the coloring button
                    CatButton button = catButtons[i].GetComponent<CatButton>();
                    button.SetButtonName(cs.categories[i]);
                    button.SetButtonType(CatButton.ButtonType.Color);
                }
                else if (cat != "")
                {   //general boat part button
                    catButtons[i].GetComponent<CatButton>().SetButtonName(cs.categories[i]);
                }
                else
                {   //if there is no category name, disable the button
                    catButtons[i].SetActive(false);
                }
            }
            //2) Disable unused category buttons
            if (catNumber < 5)
            {
                for (int j = catNumber; j < 5; j++)
                {   //disable unused buttons after the last one
                    catButtons[j].SetActive(false);
                }
            }
            //3) Cache customShipyard
            customShipyard = cs;
        }
        public void OpenPanel(GameObject panel)
        {
            panel.SetActive(true);
            openedPanels.Add(panel);
        }
        public void OpenColorPanel()
        {   //open the color panel and color the buttons as they should be colored
            //color panel proper
            Color[] colors;
            switch (GameState.currentShipyard.name)
            {
                case ("shipyard Al'Ankh"):
                    colors = grcColors;
                    break;
                case ("shipyard E DragonCliffs"):
                    colors = dcColors;
                    break;
                case ("shipyard Aestrin"):
                    colors = faColors;
                    break;
                //case ("shipyard Lagoon"):
                //    break;
                default:
                    colors = kColors;
                    break;
            }
            
            colorizer.Colorize(colors, colorButtons, true);
            OpenPanel(colorPanel);

            //selection panel for parts / material selection
            OpenPanel(selectionPanel);
            selectionPanel.transform.Find("label").GetComponentInChildren<TextMesh>().text = "Select a group";
            ColorGroup[] cgs = customShipyard.colorGroups;
            for (int i = 0; i < cgs.Length; i++)
            {
                MaterialButton mb = selectionButtons[i].GetComponent<MaterialButton>() ?? selectionButtons[i].AddComponent<MaterialButton>();

                mb.targetMaterial = cgs[i]?.renderer?.sharedMaterial;
                mb.objects = cgs[i]?.objects;
                mb.targetIndex = i;
                mb.textMesh.text = cgs[i].groupName;

                //if one of the values were assigned, enable the button
                if (mb.targetMaterial != null || mb.objects.Length != 0) selectionButtons[i].SetActive(true);
                else selectionButtons[i].SetActive(false);
            }
            if (cgs.Length < 5)
            {
                for (int j = cgs.Length; j < 5; j++)
                {   //disable unused buttons after the last one
                    selectionButtons[j].SetActive(false);
                }
            }
            //set up the reset buttons
            foreach (GameObject button in resetButtons)
            {
                ResetColorButton rcb = button.GetComponent<ResetColorButton>() ?? button.AddComponent<ResetColorButton>();
                rcb.cgs = cgs;
            }
        }
        public void CloseAllPanels()
        {   //close all custom panels
            foreach(GameObject panel in openedPanels)
            {
                panel.SetActive(false);
            }
        }

        //public class ColorTarget
        //{
        //    public Material mat;
        //    public GameObject[] objects;
        //    public int index;
        //}

        ///PLAN: the object with this script attached is the anchor for the modded UI.
        ///Sliders for mast and stays go into a specific parchments up top (I should try make this customizable for modders)
        ///Stays scaling and mast moving happens in the SAIL panel, when you select the mast or stay it will allow you to move it, 
        ///unless you click the add sail button, or select a sail I guess
        ///Based on what extra categories the boat needs, the correct amount of buttons should appear
    }
}
