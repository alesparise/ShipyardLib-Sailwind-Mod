using System.Reflection;
using UnityEngine;

namespace ShipyardLib
{
    public class CatButton : GoPointerButton
    {
        private TextMesh label;

        private CustomUI customUI;

        private ButtonType type;

        private int index;

        private float defaultSize;

        private bool useSliders;
        
        public void Init(ButtonType t, bool us, int i)
        {
            label = GetComponentInChildren<TextMesh>();
            defaultSize = label.characterSize;
            customUI = GetComponentInParent<CustomUI>();
            type = t;
            useSliders = us;
            index = i;
        }
        public override void OnActivate()
        {
            Debug.LogWarning("CatButton clicked, category: " + label.text);

            customUI.CloseVanillaUI();
            if (type == ButtonType.Parts)
            {
                ShipyardUI.instance.ResetPartsErrorTexts();
                ShipyardUI.instance.ChangeMenuCategory(index);

                customUI.CloseAllPanels();
            }
            else if (type == ButtonType.Selection)
            {   //currently this panel is only used for the color system
                customUI.CloseVanillaUI();
                customUI.CloseAllPanels();
                customUI.OpenPanel(customUI.selectionPanel);
            }
            else if (type == ButtonType.Color)
            {
                customUI.CloseVanillaUI();
                customUI.CloseAllPanels();
                customUI.OpenColorPanel();
            }
            if (useSliders)
            {   //probably unnecessary for most situations
                customUI.OpenPanel(customUI.sliderPanel);
            }
        }
        public void SetButtonName(string s)
        {
            if (s.Length > 8)
            {   //assume it won't fit properly inside the button
                label.characterSize = defaultSize * 8f / s.Length;
            }
            label.text = s;
        }
        public void SetButtonType(ButtonType t)
        {
            type = t;
        }
        public enum ButtonType
        {
            Parts = 0,
            Selection = 1,
            Color = 2
        }
    }
}
