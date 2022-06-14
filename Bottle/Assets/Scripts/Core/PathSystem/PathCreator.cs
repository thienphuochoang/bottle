using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Bottle.Extensions.Helper;
namespace Bottle.Core.PathSystem
{
    public class PathCreator : MonoBehaviour
    {
        public enum PathTypes { ONE_WAY, ROUND_TRIP }
        [Space]
        [TitleGroup("General", alignment: TitleAlignments.Centered), Tooltip("General Path Settings")]
        public PathTypes pathTypes = PathTypes.ONE_WAY;
        public float handleSize = 0.2f;
        [Space]
        public List<Vector3> nodes = new List<Vector3>(new Vector3[] { new Vector3(-1.5f, 1f, 0.5f), new Vector3(1.5f, 1f, 0.5f) });
        [TitleGroup("Color Settings", alignment: TitleAlignments.Centered), Tooltip("Color Settings of nodes and paths")]
        public Color defaultColor = Color.white;
        public Color hoveredColor = Color.red;

        public class PathCreatorDatabase : ValueAsset<PathCreator>
        {

        }
        [System.Serializable]
        public class PathCreatorReference : ValueReference<PathCreator, PathCreatorDatabase>
        {

        }


#if UNITY_EDITOR

        void OnDrawGizmos()
        {
            // Only draw path gizmo if the path object is not selected
            GameObject selectedObj = UnityEditor.Selection.activeGameObject;
            if (selectedObj != gameObject)
            {
                if (nodes != null)
                {
                    Gizmos.color = new Color(2f, 2f, 2f);
                    Vector3 previousWorldSpacePos = new Vector3(0, 0, 0);
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        Vector3 worldSpacePos = this.transform.TransformPoint(nodes[i]);
                        float constantHandleSize = HandleUtility.GetHandleSize(worldSpacePos);
                        Gizmos.DrawSphere(worldSpacePos, constantHandleSize * handleSize * 0.5f);
                        if (i > 0)
                        {
                            Gizmos.DrawLine(previousWorldSpacePos, worldSpacePos);
                        }
                        previousWorldSpacePos = worldSpacePos;
                    }
                }
            }
        }

#endif
    }
}
