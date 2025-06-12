using UnityEngine;

namespace ShipyardLib
{
    public class ColorButton : GoPointerButton
    {
        private Color color;
        public override void OnActivate()
        {
            Debug.LogWarning("Clicked color: " + color);

            CustomUI.ColorTarget target = CustomUI.instance.target;
            if (target == null) return;
            if (target.mat != null) CustomUI.instance.colorizer.Colorize(color, target.mat);
            if (target.objects != null && target.objects.Length > 0) CustomUI.instance.colorizer.Colorize(color, target.objects);
        }

        public void SetColor(Color c)
        {
            Debug.LogWarning("Setting button color to " + c);
            color = c;
        }
    }
}
