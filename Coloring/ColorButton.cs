using System.Collections.Generic;
using UnityEngine;

namespace ShipyardLib
{
    public class ColorButton : GoPointerButton
    {
        private Color color;

        private static int lastTarget;
        public override void OnActivate()
        {
            //apply color to the correct group
            ColorGroup target = CustomUI.instance.target;
            if (target == null) return;
            if (target.material != null) CustomUI.instance.colorizer.Colorize(color, target.material);
            if (target.objects != null && target.objects.Length > 0) CustomUI.instance.colorizer.Colorize(color, target.objects);
            CustomUI.instance.customShipyard.colorGroups[target.index].currentColor = color;

            //add paintjob to the shipyard order
            AddToOrder(target.groupName, target.index);
            lastTarget = target.index;      //save last target index to use to check if we are updating the same material
        }
        private void AddToOrder(string groupName, int targetIndex)
        {   //Add the painting to the current shipyard order
            //DEBUG: ISSUE: this is not showing the group name! wtf? idk anymore
            Shipyard yard = GameState.currentShipyard;
            CustomOrder order = CustomUI.instance.customOrder;

            int orderValue = yard.GetCurrentOrderTotal();
            string text = yard.GetCurrentOrderText();

            float currencyRate = yard.GetCurrencyRate();

            if (lastTarget == targetIndex)
            {
                order.RemoveLine(groupName + " paint: " + (1000 * currencyRate));
                order.AddToTotal(-Mathf.RoundToInt(1000 * currencyRate));
            }
            order.AddLine(groupName + " paint: " + (1000 * currencyRate));
            order.AddToTotal(Mathf.RoundToInt(1000 * currencyRate));
            yard.UpdateOrder();
        }
        public void SetColor(Color c)
        {   //use to set button color
            color = c;
        }
    }
}
