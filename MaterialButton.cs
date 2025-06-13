using cakeslice;
using UnityEngine;

namespace ShipyardLib
{
    //Select the associated material or list of parts
    public class MaterialButton : GoPointerButton
    {
        public GameObject[] objects;
        public static GameObject selectedButton;

        public TextMesh textMesh;

        private Renderer renderer;

        public Material targetMaterial;         //this is the material to be recolored
        public Material defaultMat;             //this is the button material
        public static Material clickedMat;      //clicked material for the button

        public int targetIndex = -1;

        private void Awake()
        {
            textMesh = GetComponentInChildren<TextMesh>();
            renderer = GetComponent<Renderer>();
            defaultMat = renderer.sharedMaterial;
            if (clickedMat == null) clickedMat = CustomUI.instance.clickedMaterial;
        }
        public override void OnActivate()
        {
            if (selectedButton == gameObject)
            {   //reset color if clicked again and then do nothing
                CustomUI.instance.target = new ColorGroup();
                renderer.sharedMaterial = defaultMat;
                selectedButton = null;
                return;
            }
            CustomUI.instance.target.material = targetMaterial;
            CustomUI.instance.target.objects = objects;
            CustomUI.instance.target.index = targetIndex;
            renderer.sharedMaterial = clickedMat;
            if (selectedButton != null)
            {
                selectedButton.GetComponent<Renderer>().sharedMaterial = defaultMat;
            }
            selectedButton = gameObject;
        }
    }
}
