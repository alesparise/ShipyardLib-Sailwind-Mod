using UnityEngine;

namespace ShipyardLib
{
    /// <summary>
    /// A generic bridge component, attach this to objects that need to be assigned custom components that
    /// do not inherit from MonoBehaviour (e.g. buttons)
    /// </summary>
    public class Bridge : MonoBehaviour
    {
        [Header("Category Buttons")]
        [Tooltip("Set to true if the object is a category button")]
        public bool isCatButton;
        [Tooltip("Set to true if the button should open the sliders panel")]
        public bool useSlider;
        [Tooltip("Category Index")]
        public int index = 0;
        public CatButton.ButtonType type;

        public void Awake()
        {
            if (isCatButton)
            {
                gameObject.AddComponent<CatButton>().Init(type, useSlider, index);
            }

            Destroy(this);  //self remove once done
        }
    }
}
