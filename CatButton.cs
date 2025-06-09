using UnityEngine;

namespace ShipyardLib
{
    public class CatButton : GoPointerButton
    {
        private TextMesh label;

        private Category cat;

        public void Init( Category c)
        {
            label = GetComponentInChildren<TextMesh>();
            cat = c;

            switch(cat)
            {
                case Category.structures:
                    label.text = "Structure";
                    break;
                case Category.decorations:
                    label.text = "Decoration";
                    break;
                case Category.shrouds:
                    label.text = "Shrouds";
                    break;
                case Category.color:
                    label.text = "Color";
                    break;
                case Category.engines:
                    label.text = "Engine";
                    break;
                default:
                    label.text = "Error";
                    break;
            }
        }

        public override void OnActivate()
        {
            Debug.LogWarning("CatButton clicked, category: " + cat);
        }

        public void SetButtonName(string s)
        {
            label.text = s;
        }
    }
}
