using UnityEngine;

namespace ShipyardLib
{   
    /// <summary>
    /// reset the color of the selected color group
    /// </summary>
    public class ResetColorButton : GoPointerButton
    {
        public ColorGroup[] cgs;

        public override void OnActivate()
        {
            CustomUI.ColorTarget target = CustomUI.instance.target;
            ColorGroup cg = CustomUI.instance.customShipyard.colorGroups[target.index];

            if (target == null) return;
            if (target.mat != null) CustomUI.instance.colorizer.Colorize(cg.sharedDefault, target.mat);
            if (target.objects != null && target.objects.Length > 0)
            {
                for (int i = 0; i < cg.objects.Length; i++)
                {
                    if (cg.objects[i] != null)
                    {
                        Color color = cg.objectsDefault[i];
                        CustomUI.instance.colorizer.Colorize(color, target.objects);
                    }
                }
            }
        }
    }
}
