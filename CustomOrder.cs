using System.Collections.Generic;
using UnityEngine;

namespace ShipyardLib
{
    /// <summary>
    /// Allows adding stuff to the shipyard, works in conjunction with the patch of Shipyard.UpdateOrder()
    /// Not sure if it would be better to have it as a static class or a MonoBehaviour attached to the CustomUI?
    /// </summary>
    public class CustomOrder : MonoBehaviour
    {
        public List<string> lines = new List<string>();

        public int customTotal;

        public void AddToTotal(int value) => customTotal += value;
        public int AddTotal()
        {
            int total = customTotal;
            customTotal = 0;        //reset for future updates

            return total;
        }
        public void AddLine(string line) => lines.Add(line);
        public void RemoveLine(string line) => lines.Remove(line);
        public void AppendOrder()
        {   //add the order to the actual order text (this is called in the UpdateOrder patch)
            ShipyardUIOrderText order = ShipyardUI.instance.shipyardOrderText;
            //order.AddLine("Paint: ");   //Debug: For now it's only paint so let's give it a header of sorts
            foreach (string line in lines)
            {
                order.AddLine(line);
            }
            //order.DisplayCurrentText();
        }
        public void UpdateTotalLine(int value, int region)
        {
            ShipyardUIOrderText order = ShipyardUI.instance.shipyardOrderText;
            string currencyName = PlayerGold.GetCurrencyName(region);

            if (value < 0)
            {
                order.SetTotalLine("TOTAL: " + -value + " " + currencyName + " refund");
            }
            else
            {
                order.SetTotalLine("TOTAL: " + value + " " + currencyName);
            }
            order.DisplayCurrentText();
        }
    }
}
