using UnityEngine;

namespace ShipyardLib
{
    public class ObjectMover : MonoBehaviour
    {
        [Tooltip("The objects you want to move when this object is enabled")]
        public Transform[] moveWhenEnabled;
        [Tooltip("Final position the objects will be moved at")]
        public Vector3[] finalPos;
        [Tooltip("Final rotation the objects will be moved at")]
        public Vector3[] finalRot;

        private Vector3[] startPos;
        private Vector3[] startRot;

        public void Awake()
        {
            int l = moveWhenEnabled.Length;
            startPos = new Vector3[l];
            startRot = new Vector3[l];

            for (int i = 0; i < l; i++)
            {
                startPos[i] = moveWhenEnabled[i].localPosition;
                startRot[i] = moveWhenEnabled[i].localEulerAngles;
            }
        }
        public void OnEnable()
        {
            for (int i = 0; i < moveWhenEnabled.Length; i++)
            {
                
            }
        }
        public void OnDisable()
        {

        }

        public void OnDrawGizmosSelected()
        {
            for (int i = 0; i < moveWhenEnabled.Length;i++)
            {
                Transform t = moveWhenEnabled[i]?.transform;
                if (t == null) continue;

                Mesh mesh = t.GetComponent<MeshFilter>()?.sharedMesh;
                if (mesh == null) continue;

                Transform parent = t.parent;

                Vector3 worldPos = parent != null ? parent.TransformPoint(finalPos[i]) : finalPos[i];
                Quaternion worldRot = parent != null ? parent.rotation * Quaternion.Euler(finalRot[i]) : Quaternion.Euler(finalRot[i]);

                Vector3 scale = t.lossyScale; // Keep the scale as-is (world scale)

                Gizmos.color = Color.green;
                Gizmos.matrix = Matrix4x4.TRS(worldPos, worldRot, scale);
                Gizmos.DrawMesh(mesh);
            }
        }
    }
}
