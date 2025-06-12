using UnityEngine;

namespace ShipyardLib
{
    public class Colorizer : MonoBehaviour
    {
        private MaterialPropertyBlock propBlock;

        private void Awake()
        {
            propBlock = new MaterialPropertyBlock();
        }

        //Apply Color through property blocks to avoid creating new materials
        public void Colorize(Color col, Renderer ren)
        {
            ren.GetPropertyBlock(propBlock);
            propBlock.SetColor("_Color", col);
            if (ren.sharedMaterials.Length > 1) ren.SetPropertyBlock(propBlock, 1); //case of the hull, with dirt texture in slot 0
            else ren.SetPropertyBlock(propBlock, 0);    //all other objects should only have one material
        }
        public void Colorize(Color col, GameObject obj)
        {
            Renderer ren = obj.GetComponent<Renderer>();
            if (ren != null)
            {
                Colorize(col, ren);
            }
        }
        public void Colorize(Color col, Renderer[] rens)
        {
            foreach (Renderer r in rens)
            {
                Colorize(col, r);
            }
        }
        public void Colorize(Color col, GameObject[] objs)
        {
            foreach (GameObject obj in objs)
            {
                Colorize(col, obj);
            }
        }
        public void Colorize(Color[] cols, Renderer[] rens)
        {
            if (cols.Length == rens.Length)
            {
                for (int i = 0; i < cols.Length; i++)
                {
                    Colorize(cols[i], rens[i]);
                }
            }
        }
        public void Colorize(Color[] cols, GameObject[] objs)
        {
            if (cols.Length == objs.Length)
            {
                for (int i = 0; i < objs.Length; i++)
                {
                    Colorize(cols[i], objs[i]);
                }
            }
        }
        public void Colorize(Color[] cols, GameObject[] objs, bool isButton)
        {
            if (cols.Length == objs.Length)
            {
                for (int i = 0; i < objs.Length; i++)
                {
                    GameObject swatch = objs[i].transform.GetChild(0).gameObject;
                    Colorize(cols[i], swatch);
                    ColorButton cb = objs[i].GetComponent<ColorButton>() ?? objs[i].AddComponent<ColorButton>();
                    cb?.SetColor(cols[i]);
                }
            }
        }
        public void Colorize(Color col, Material mat)
        {
            mat.SetColor("_Color", col);
        }
    }
}
