using UnityEngine;

using static ShipyardLib.ShipyardHelpers;

namespace ShipyardLib
{
    public class DynamicAttachment : MonoBehaviour
    {
        private Transform t;    //this object transform

        public Slider slider;   //the slider that controls the movement

        public Axis axis;       //the axis you want to move this along

        public float min;       //minimum level you want this to move to
        public float max;       //maximum level you want this to move to

        public void Start()
        {
            t = transform;
            slider = customUI.slider1;
            slider.OnValueChanged += UpdatePos;
        }
        private void UpdatePos(float value)
        {
            float l = Mathf.Lerp(min, max, value);
            Vector3 targetPos = Clamp(GetAxis(axis) * l, min, max, axis);
            t.localPosition = targetPos;
        }
    }
}
