using UnityEngine;

namespace ShipyardLib
{
    public class ObjectToggler : MonoBehaviour
    {
        public GameObject[] objectToEnable;
        public GameObject[] objectToDisable;

        private void OnEnable()
        {   //enable all objects in the array
            foreach (GameObject obj in objectToEnable)
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                }
            }
            //disable all objects in the array
            foreach (GameObject obj in objectToDisable)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }
        }
        private void OnDisable()
        {   //disable all objects in the array
            foreach (GameObject obj in objectToEnable)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }
            //enable all obejcts in the array
            foreach (GameObject obj in objectToDisable)
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                }
            }
        }
    }
}
