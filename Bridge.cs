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
        public bool isCatButton;
        public Category category;

        public void Awake()
        {
            if (isCatButton)
            {
                gameObject.AddComponent<CatButton>().Init(category);
            }

            Destroy(this);  //self remove once done
        }
    }
}
