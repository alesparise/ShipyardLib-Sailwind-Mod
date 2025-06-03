using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace ShipyardLib
{   /// <summary>
    /// Scales a part between a start and an end point, as the points move.
    /// This makes some assumptions:
    /// 1) the part is scaled along the (local) z axis (forward, blue arrow in the editor)
    /// 2) the two points are not children of the part (although actually it might work regardless?)
    /// 3) The part is a Mast or BoatPartOption. This might be better be changed in the future?
    /// 4) The part sharedMesh is the same as the walkCol corresponding part (we scale the sharedMesh, which affects both)
    /// Known Issues:
    /// 1) Currently if you are scaling a Mast it doesn't change the extra bottom height, which might be relevant
    /// 
    /// Usage:
    /// This is mostly meant to scale stays, but hopefully should work with shrouds too, if they are set up correctly,
    /// or at least should only need minor changes.
    /// Attach this Component to the part you want to make dynamic in unity, assign the end and start points
    /// That should be it. Now if one point moves (for whatever reason) the part should adjust itself accordingly.
    /// </summary>
    public class DynamicPart : MonoBehaviour
    {
        private Transform t;
        private Transform walkCol;
        public Transform start;  //point one. This is where the dynamic object start
        public Transform end;    //point two. This is where the dynamic object ends.

        private MeshFilter meshFilter;

        private Mesh mesh;

        private Mast mast;

        private CapsuleCollider capsule;

        private List<Sail> sails;
        
        private Vector3 originalScale;
        private Vector3 lastStartPos;
        private Vector3 lastEndPos;
        private Vector3[] originalVertices;
        
        private float originalDistance;
        private float originalOffset;
        private float originalRadius;
        private float originalCapsuleHeight;
        private float originalMastHeight;
        private float previousMastHeight;

        private bool isMast;
        
        public void Awake()
        {
            t = transform;

            //get mesh so we can change the mesh scale instead of the transform scale
            //this allow the attached sail and all children to keep their original scale
            meshFilter = GetComponent<MeshFilter>();
            mesh = meshFilter.sharedMesh;
            originalVertices = mesh.vertices;

            //get some of the original data so we can use it later
            Vector3 ogDir = (end.position - start.position);
            originalDistance = ogDir.magnitude;
            originalScale = t.localScale;
            originalOffset = ((start.position + ogDir * 0.5f) - t.position).magnitude;

            //figure out if it's a mast or not, get walk col, capsule, etc
            mast = GetComponent<Mast>();
            if (mast != null)
            {
                isMast = true;
                capsule = GetComponent<CapsuleCollider>();
                walkCol = mast.walkColMast;
                originalRadius = capsule.radius;
                originalCapsuleHeight = capsule.height;
                originalMastHeight = mast.mastHeight;
            }
            else
            {
                walkCol = GetComponent<BoatPartOption>().walkColObject.transform;
            }
        }
        public void Update()
        {
            if (GameState.currentShipyard == null || (lastEndPos == end.position && lastStartPos == start.position)) return;

            KeepChildrenStill(t, Connect);
        }
        public void Connect()
        {   //moves, rotates and scales the object so that it "connects" the two attachments

            Vector3 startPos = start.position;
            Vector3 endPos = end.position;
            Vector3 dir = endPos - startPos;

            float distance = dir.magnitude;

            //1) Find the midpoint and move there
            Vector3 midPoint = startPos + dir * 0.5f;
            t.position = midPoint;
            
            //2) Rotate to align with direction
            t.rotation = Quaternion.LookRotation(dir, t.up);

            //3) Scale the mesh to match distance
            Vector3 scale = originalScale;
            scale.z = distance / originalDistance;
            Vector3[] newVerts = new Vector3[originalVertices.Length];
            for (int i = 0; i < newVerts.Length; i++)
            {
                Vector3 v = originalVertices[i];
                v.z *= scale.z;
                newVerts[i] = v;
            }
            mesh.vertices = newVerts;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            //4) Offset to match mesh offset
            float offset = originalOffset * scale.z;
            t.position += t.forward * offset;

            //5) Apply local rotation and position to the walkCol
            walkCol.localRotation = t.localRotation;
            walkCol.localPosition = t.localPosition;

            //6) if this is a mast, adjust mast height and capsule collider
            if (isMast)
            {
                //mast fix
                float radius = originalRadius * scale.z;
                float mastHeight = originalMastHeight * scale.z;

                previousMastHeight = mast.mastHeight;
                mast.mastHeight = mastHeight;

                //capsule collider fix
                float capsuleHeight = originalCapsuleHeight * scale.z;
                capsule.direction = 2;
                capsule.radius = radius;
                capsule.height = capsuleHeight;
                capsule.center = new Vector3(0f, 0f, -(mastHeight / 2f));   //debug: this might have to be adjusted
                mast.mastCols = new CapsuleCollider[1]; //this currently does not support having shrouds colliders
                mast.mastCols[0] = capsule;

                foreach (Sail s in sails)
                {
                    if (s != null) UpdateSail(s);
                }
            }

            //7) Store last end and start position so we can use them to check if Connect should be called
            lastEndPos = endPos;
            lastStartPos = startPos;
        }
        private void KeepChildrenStill(Transform parent, Action doTransformChange)
        {   //Make sure the children object of the Dynamic Part don't get moved around
            List<Transform> children = new List<Transform>();
            List<Sail> ss = new List<Sail>();

            //1) Cache world positions and rotations
            foreach (Transform child in parent)
            {
                Sail s = child.GetComponent<Sail>();
                if (s != null)
                {
                    ss.Add(s);
                    continue;
                }
                children.Add(child);
            }

            List<Vector3> childPositions = children.Select(c => c.position).ToList();
            List<Quaternion> childRotations = children.Select(c => c.rotation).ToList();

            sails = ss;
            //2) Perform transformation
            doTransformChange();

            //3) Restore world transforms
            for (int i = 0; i < children.Count; i++)
            {
                children[i].position = childPositions[i];
                children[i].rotation = childRotations[i];
            }
        }
        private void UpdateSail(Sail s)
        {   //make sure the sail is in the allowed range of install positions

            float currentHeight = s.GetCurrentInstallHeight();
            float t = Mathf.InverseLerp(s.installHeight, previousMastHeight, currentHeight);    //get at what point on the mast the sail was
            float targetHeight = Mathf.Lerp(s.installHeight, mast.mastHeight, t);               //get at what point on the mast the sail should be now
            float scale =  mast.mastHeight / previousMastHeight;
            float distance = targetHeight - currentHeight;

            Debug.LogWarning("current: " + currentHeight + " scale: " + scale + " target: " + targetHeight + " distance: " + distance);

            if (distance > 0f && currentHeight + distance > mast.mastHeight + 0.1f)
            {
                distance = mast.mastHeight - currentHeight;
            }
            if (distance < 0f && currentHeight + distance - s.installHeight < -0.1f)
            {
                distance = s.installHeight - currentHeight;
            }

            s.ChangeInstallHeight(distance);
            s.UpdateInstallPosition();

            //update everyrhing
            mast.UpdateSailOrder();
            mast.UpdateControllerAttachments();

            ShipyardUI.instance.UpdateMastSailsList();
            ShipyardUI.instance.UpdateButtonSelections();

            Shipyard shipyard = GameState.currentShipyard;
            shipyard.sailInstaller.RecheckAllSailsCols();
            shipyard.UpdateOrder();
        }
    }
}
