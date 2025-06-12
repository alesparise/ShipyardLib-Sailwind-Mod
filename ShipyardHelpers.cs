using System.IO;
using System.Reflection;
using UnityEngine;

namespace ShipyardLib
{
    public static class ShipyardHelpers
    {
        //Data
        private static AssetBundle bundle;
        
        //Prefabs
        public static GameObject slider;
        public static GameObject UIPrefab;

        //References
        public static Transform baseUI;
        public static CustomUI customUI;

        public static void Setup()
        {
            //load bundle
            string folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string bundlePath = Path.Combine(folder, "shipyardlib");
            bundle = AssetBundle.LoadFromFile(bundlePath);
            if (bundle == null)
            {
                Debug.LogError("ShipyarLib: Could not load the bundle!");
                return;
            }

            //load prefabs
            string sliderPath = "Assets/ShipyardLib/shipyard_slider.prefab";
            slider = bundle.LoadAsset<GameObject>(sliderPath);
            string customUIPath = "Assets/ShipyardLib/CustomUI.prefab";
            UIPrefab  = bundle.LoadAsset<GameObject>(customUIPath); 

            //find vanilla UI
            baseUI = ShipyardUI.instance?.transform.Find("UI");
            if (baseUI == null) Debug.LogError("ShipyardUI not found during setup!");

            //Prepare the UI
            Debug.LogWarning("Instantiate Custom UI");
            Transform shifting = GameObject.Find("_shifting world").transform;

            customUI = Object.Instantiate(UIPrefab, shifting).GetComponent<CustomUI>();
            customUI.name = "CustomUI";
            Transform customUITra = customUI.transform;
            customUITra.parent = baseUI;
            customUITra.localPosition = Vector3.zero;
            customUITra.localRotation = Quaternion.identity;
            customUITra.localScale = Vector3.one;

            //debug slider:
            GameObject s = Object.Instantiate(slider, customUITra);
            s.transform.localPosition = Vector3.up * 5f;
            customUI.slider1 = s.GetComponent<Slider>();

            Debug.LogWarning("ShipyardLib: Setup done!");
        }

        //HELPER METHODS
        public static void ToggleUI(bool state, CustomShipyard cs)
        {
            if (!state) return; //do nothing if we are closing the ui
            customUI.OpenCustomUI(IsWideUi(), cs);
        }

        //Vector3 stuff:
        public static Vector3 Clamp(Vector3 v, float min, float max, Axis a)
        {   //helper method to clamp a vector
            if (a == Axis.X)
            {
                return new Vector3 (Mathf.Clamp(v.x, min, max), v.y, v.z);
            }
            else if (a == Axis.Y)
            {
                return new Vector3(v.x, Mathf.Clamp(v.y, min, max), v.z);
            }
            else //(a == Axis.Z)
            {
                return new Vector3(v.x, v.y, Mathf.Clamp(v.z, min, max));
            }
        }
        public static Vector3 Clamp(Vector3 v, float limit, Axis a)
        {   //helper method to clamp a vector with only one limit

            return Clamp(v, -limit, limit, a);
        }
        public static Vector3 GetAxis(Axis a)
        {   //Axis to Vector3 direction
            if (a == Axis.X) return Vector3.right;
            else if (a == Axis.Y) return Vector3.up;
            else return Vector3.forward; //if (a == Axis.Z)
        }
        public static bool AreClose(Vector3 v1, Vector3 v2)
        {   //returns true if two vectors are "close enough", avoiding floating point errors like direcly comparing them
            float tol = 0.001f;
            tol *= tol; //square it

            return Vector3.SqrMagnitude(v1 - v2) < tol;
        }
        public static bool TolerantCompare(Vector3 v1, Vector3 v2)
        {   //compare two vectors in a looser way than the == operator
            float tol = 0.01f;  //tolerance

            float dx = Mathf.Abs(v1.x - v2.x);
            float dy = Mathf.Abs(v1.y - v2.y);
            float dz = Mathf.Abs(v1.z - v2.z);

            if (dx > tol || dy > tol || dz > tol) return false;   //not the same position
            else return true;   //more or less the same position
        }
        public enum Axis
        {   //helper for storing axis
            X = 0,
            Y = 1,
            Z = 2,
        }

        //DynamicPart helpers:
        public static void ScaleMesh(Mesh mesh, Vector3[] originalVertices, float scale)
        {
            Vector3[] newVerts = new Vector3[originalVertices.Length];
            for (int i = 0; i < newVerts.Length; i++)
            {
                Vector3 v = originalVertices[i];
                v.z *= scale;
                newVerts[i] = v;
            }
            mesh.vertices = newVerts;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
        }

        //Various helpers
        public static bool IsWideUi()
        {
            float exitButtonX = baseUI.Find("shipyard ui button exit").localPosition.x;
            if (exitButtonX < -11)
            {
                return true;
            }
            else return false;
        }
    }
}
