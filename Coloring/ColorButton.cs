using UnityEngine;

namespace ShipyardLib
{
    public class ColorButton : GoPointerButton
    {
        private Color color;
        public override void OnActivate()
        {
            //apply color to the correct group
            ColorGroup target = CustomUI.instance.target;
            if (target == null) return;
            if (target.material != null) CustomUI.instance.colorizer.Colorize(color, target.material);
            if (target.objects != null && target.objects.Length > 0) CustomUI.instance.colorizer.Colorize(color, target.objects);
            CustomUI.instance.customShipyard.colorGroups[target.index].currentColor = color;

            //add paintjob to the shipyard order
        }

        public void SetColor(Color c)
        {
            Debug.LogWarning("Setting button color to " + c);
            color = c;
        }
    }
}
