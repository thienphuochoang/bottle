using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Bottle.ProceduralMeshes;
using Bottle.ProceduralMeshes.Tree;
using Bottle.Extensions.Helper;
using Random = UnityEngine.Random;

namespace Bottle.Editor.ProceduralTree
{
    public class TreeCreator : EditorWindow
    {
        private Material _treeMat;
        private Material _leafMat;
        private GameObject _treeGameObj;
        private List<GameObject> _leafObjects;
        [Header("Branch Parameters")]
        [Range(0, 5)]
        public int numberOfSplit = 1;
        [Range(6, 50)]
        public int ringPolygon = 8;
        [Range(2, 20)]
        public int floorNumber = 8;
        [Range(0.5f, 20f)]
        public float floorHeight = 1f;
        [Range(1, 60)]
        public int bendLevel = 3;
        [Range(0f, 10f)]
        public float trunkRadius = 1f;
        [Header("Leaf Parameters")]
        public LeafType leafType = LeafType.Sphere;
        public float leafSize = 3f;
        private float _currentLeafSize;

        [MenuItem("Bottle/Tree Creator")]
        public static void ShowWindow()
        {
            EditorWindow editorWindow = EditorWindow.GetWindow(typeof(TreeCreator));
            editorWindow.titleContent = new GUIContent("Tree Creator");
        }

        private void OnEnable()
        {
            _leafObjects = new List<GameObject>();
        }

        private void OnValidate()
        {
            if (leafSize != _currentLeafSize && _leafObjects.Count > 0)
            {
                foreach (GameObject leaf in _leafObjects)
                {
                    leaf.GetComponent<PMBaseMesh>().size = Random.Range(leafSize - 0.5f, leafSize + 0.5f);
                    leaf.GetComponent<PMBaseMesh>().UpdateMesh();
                }
            }
            _currentLeafSize = leafSize;
        }

        private void OnGUI()
        {
            UnityEditor.Editor e = UnityEditor.Editor.CreateEditor(this);
            EditorGUILayout.BeginVertical();
            e.DrawDefaultInspector();
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button("Generate", GUILayout.Height(50)))
            {
                GenerateTree();
            }
            EditorGUILayout.EndVertical();
        }
        /// <summary>
        /// Function to generate a whole tree (trunk, branches and leaves)
        /// </summary>
        void GenerateTree()
        {
            #region Setup Initial Properties
            // Load material for the tree
            if (!_treeMat)
                _treeMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art_Fixed/Isometric Pack 3d/Props/Models/Materials/Trunk.mat");
            // Load material for the leaf
            if (!_leafMat)
                _leafMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art_Fixed/Isometric Pack 3d/Props/Models/Materials/Leaf.mat");
            // If the tree game object is available, delete it and then re-create it again to refresh the data
            _treeGameObj = GameObject.Find("Tree");
            _leafObjects = new List<GameObject>();
            if (_treeGameObj)
                DestroyImmediate(_treeGameObj);
            _treeGameObj = new GameObject("Tree");
            // Create the first main trunk ring
            Vector3[] initialFirstTrunkRing = InitializeFirstTrunkRing();
            // Create the trunk result for later use
            var previousTrunkResult = new TrunkData();
            // The leaf position list
            List<Vector3> leafPos = new List<Vector3>();
            #endregion

            #region Generate The Tree Mesh
            for (int i = 0; i <= numberOfSplit; i++)
            {
                #region Generate Main Trunk Data And Mesh
                // Generate the main trunk data and mesh
                if (i == 0)
                {
                    // Create the main trunk game object
                    var trunkGameObj = new GameObject("Trunk");
                    trunkGameObj.transform.parent = _treeGameObj.transform;
                    
                    // Generate the trunk data
                    previousTrunkResult = GenerateTreeData(trunkGameObj,
                        ringPolygon,
                        floorNumber,
                        initialFirstTrunkRing,
                        Vector3.up,
                        Vector3.zero,
                        trunkRadius);

                    // Assign the mesh component to the Tree GameObject
                    trunkGameObj.AddComponent<MeshFilter>().mesh = previousTrunkResult.trunkMesh;
                    trunkGameObj.AddComponent<MeshRenderer>().material = _treeMat;
                    // If there is no split, then there are leaves on tpreviousTrunkResultop of the trunk
                    if (numberOfSplit == 0)
                    {
                        leafPos.Add(previousTrunkResult.nextTrunkRingDatas[0].trunkRingPivot);
                    }
                }
                #endregion
                #region Generate The Branches
                // The branches
                else
                {
                    // Create the branch game objects
                    var branchGameObj1 = new GameObject("Branch" + i);
                    branchGameObj1.transform.parent = _treeGameObj.transform;
                    var branchGameObj2 = new GameObject("Branch" + i);
                    branchGameObj2.transform.parent = _treeGameObj.transform;
                    
                    // Generate the first branch data
                    var firstSplitTrunk = GenerateTreeData(branchGameObj1, 
                        previousTrunkResult.nextTrunkRingDatas[0].trunkRingVertexPos.Length, 
                        floorNumber, 
                        previousTrunkResult.nextTrunkRingDatas[0].trunkRingVertexPos, 
                        previousTrunkResult.nextTrunkRingDatas[0].trunkRingGrowDirection, 
                        previousTrunkResult.nextTrunkRingDatas[0].trunkRingPivot,
                        previousTrunkResult.nextTrunkRingDatas[0].trunkRingRadius);

                    // Assign the mesh component to the Tree GameObject
                    branchGameObj1.AddComponent<MeshFilter>().mesh = firstSplitTrunk.trunkMesh;
                    branchGameObj1.AddComponent<MeshRenderer>().material = _treeMat;
                    
                    // Generate the second branch data
                    var secondSplitTrunk = GenerateTreeData(branchGameObj2, 
                        previousTrunkResult.nextTrunkRingDatas[1].trunkRingVertexPos.Length, 
                        floorNumber, 
                        previousTrunkResult.nextTrunkRingDatas[1].trunkRingVertexPos, 
                        previousTrunkResult.nextTrunkRingDatas[1].trunkRingGrowDirection, 
                        previousTrunkResult.nextTrunkRingDatas[1].trunkRingPivot,
                        previousTrunkResult.nextTrunkRingDatas[1].trunkRingRadius);

                    // Assign the mesh component to the Tree GameObject
                    branchGameObj2.AddComponent<MeshFilter>().mesh = secondSplitTrunk.trunkMesh;
                    branchGameObj2.AddComponent<MeshRenderer>().material = _treeMat;
                    // If this is the last loop of the branches, add leaves on top of those branches
                    if (i == numberOfSplit)
                    {
                        leafPos.Add(secondSplitTrunk.nextTrunkRingDatas[0].trunkRingPivot);
                        leafPos.Add(firstSplitTrunk.nextTrunkRingDatas[1].trunkRingPivot);
                    }
                    // Otherwise, add leaves on one of the split trunk
                    else
                    {
                        // A random function to get which branch will have leaves on top of it
                        int randomSplitTrunk = Random.Range(0, 1);
                        if (randomSplitTrunk == 0)
                        {
                            leafPos.Add(secondSplitTrunk.nextTrunkRingDatas[0].trunkRingPivot);
                            previousTrunkResult = firstSplitTrunk;
                        }
                        else
                        {
                            leafPos.Add(firstSplitTrunk.nextTrunkRingDatas[1].trunkRingPivot);
                            previousTrunkResult = secondSplitTrunk;
                        }
                    }
                }
                #endregion
            }
            #endregion
            #region Generate The Leaves
            if (leafPos.Count > 0)
                GenerateLeaves(leafPos);
            #endregion
        }

        Vector3[] InitializeFirstTrunkRing()
        {
            // First ring of trunk
            // Number of vertex that create the trunk
            Vector3[] vertex = new Vector3[ringPolygon];

            // (Pi * 2) rad = 360 degrees
            float angleBetweenTwoConsecutiveLines = Mathf.PI * 2 / ringPolygon;
            for (int i = 0; i < ringPolygon; i++)
            {
                Vector3 randomness = new Vector3(Random.Range(-3, 3),
                    Random.Range(-3, 3),
                    Random.Range(-3, 3)) / 10;
                //Adjust the Vertex position randomly
                Vector3 pos = new Vector3(Mathf.Cos(i * angleBetweenTwoConsecutiveLines), 0f,
                    Mathf.Sin(i * angleBetweenTwoConsecutiveLines)) + randomness;
                pos *= trunkRadius;
                vertex[i] = pos;
            }
            return vertex;
        }

        TrunkData GenerateTreeData(GameObject treeGO,
            int polygon,
            int floorNumber, 
            Vector3[] initialFirstTrunkRing,
            Vector3 initialGrowDirection,
            Vector3 initialPivot,
            float newTrunkRadius)
        {
            #region Setup Initial Properties Of The Trunk Data
            // Create a mesh component
            Mesh mesh = new Mesh();
            // Calculate the angle
            float angleBetweenTwoConsecutiveLines = 2f * Mathf.PI / polygon;
            // All vertex of the trunk
            Vector3[] vertices = new Vector3[polygon * floorNumber];
            // All vertex faces
            int[] triangles;
            for (int i = 0; i < initialFirstTrunkRing.Length; i++)
            {
                vertices[i] = initialFirstTrunkRing[i];
            }
            // Number of vertex faces
            triangles = new int[6 * (vertices.Length - polygon)];
            // Get the grow direction of the previous ring
            Vector3 growDirection = initialGrowDirection;
            // Get the ring pivot of the previous ring
            Vector3 lastPivot = initialPivot;
            // Initialize the last trunk ring List
            List<Vector3> lastRingVertices = new List<Vector3>();
            // The trunk data that needs to be returned at the end
            TrunkData result = new TrunkData();
            // Every branch will be split into 2 more smaller branches
            TrunkRingData[] resultTrunkRingData = new TrunkRingData[2];
            #endregion
            #region Calculate And Update Each Vertex
            // Iterate through each floor height (begin with the second floor)
            for (int i = 1; i < vertices.Length / polygon; i++)
            {
                // Update the pivot position on every floor
                Vector3 pivot = lastPivot + floorHeight * growDirection;
                lastPivot = pivot;
                
                // Iterate through each vertex on that floor height
                for (int j = 0; j < polygon; j++)
                {
                    // Adjust the vertex position randomly
                    Vector3 randomness = new Vector3(Random.Range(-0.5f, 0.5f), 
                        Random.Range(-0.5f, 0.5f),
                        Random.Range(-0.5f, 0.5f)) / 10;
                    Vector3 pos = new Vector3(Mathf.Cos(j * angleBetweenTwoConsecutiveLines), 0f,
                        Mathf.Sin(j * angleBetweenTwoConsecutiveLines)) + randomness;
                    pos *= newTrunkRadius;
                    // Make the ring smaller
                    pos /= 2;
                    pos = VectorHelper.RotateAPointAroundAnAxis(pos, Vector3.up, growDirection);
                    pos += pivot;
                    triangles[6 * ((i - 1) * polygon + j)]     = (i - 1) * polygon + j;
                    triangles[6 * ((i - 1) * polygon + j) + 1] = i * polygon + j;
                    triangles[6 * ((i - 1) * polygon + j) + 2] = (i - 1) * polygon + (j + 1) % polygon;
                    triangles[6 * ((i - 1) * polygon + j) + 3] = (i - 1) * polygon + (j + 1) % polygon;
                    triangles[6 * ((i - 1) * polygon + j) + 4] = i * polygon + j;
                    triangles[6 * ((i - 1) * polygon + j) + 5] = i * polygon + (j + 1) % polygon;
                    vertices[i * polygon + j] = pos;
                    // Get all vertex of the last ring of trunk
                    if (i * polygon + j >= vertices.Length - polygon)
                    {
                        Vector3 vertexWorldPos = treeGO.transform.TransformPoint(pos);
                        lastRingVertices.Add(vertexWorldPos);
                    }
                }
                // Update the grow direction for the next trunk ring
                growDirection = VectorHelper.GenerateRandomVectorInDirection(bendLevel, growDirection);
            }
            #endregion
            #region Branch Split Condition And Branch Setup
            // If split is available
            if (numberOfSplit > 0)
            {
                // The number of polygon of the first split branch
                int firstSubRingPolygon = Random.Range(4, polygon - 1);
                // Get a random grow direction
                Vector3 firstSubRingGrowDirection = VectorHelper.GenerateRandomVectorInDirection(bendLevel, growDirection);
                // Setup the ring data of the first branch
                TrunkRingData firstSubTrunkRingData = SetupNextTrunkRingData(lastRingVertices.ToArray(), firstSubRingPolygon, firstSubRingGrowDirection);
                resultTrunkRingData[0] = firstSubTrunkRingData;
                
                // Create and store the remaining vertex of the last ring
                List<Vector3> secondLastRingVertices = new List<Vector3>();
                secondLastRingVertices.Add(lastRingVertices[0]);
                for (int i = firstSubRingPolygon - 1; i < lastRingVertices.Count; i++)
                {
                    secondLastRingVertices.Add(lastRingVertices[i]);
                }
                
                // The remaining number of polygon of the second split branch
                int secondSubRingPolygon = polygon - firstSubRingPolygon + 2;
                // Get a random grow direction
                Vector3 secondSubRingGrowDirection = VectorHelper.GenerateRandomVectorInDirection(bendLevel, growDirection);
                Vector3 axis = Vector3.Cross(firstSubRingGrowDirection, growDirection);
                secondSubRingGrowDirection = Quaternion.AngleAxis(Random.Range(10, 30), axis) * secondSubRingGrowDirection;
                // Setup the ring data of the second branch
                TrunkRingData secondSubTrunkRingData = SetupNextTrunkRingData(secondLastRingVertices.ToArray(), secondSubRingPolygon, secondSubRingGrowDirection);
                resultTrunkRingData[1] = secondSubTrunkRingData;
            }
            else
            {
                TrunkRingData lastTrunkRingData = SetupNextTrunkRingData(lastRingVertices.ToArray(), polygon, Vector3.up);
                resultTrunkRingData[0] = lastTrunkRingData;
            }
            #endregion
            #region Save The Data To Trunk Data
            mesh.vertices = vertices;
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
            result.trunkMesh = mesh;
            result.nextTrunkRingDatas = resultTrunkRingData;
            #endregion
            return result;
        }

        /// <summary>
        /// Function to generate the leaves at specific positions
        /// </summary>
        void GenerateLeaves(List<Vector3> leafPos)
        {
            switch (leafType)
            {
                case LeafType.Sphere:
                    foreach (Vector3 eachLeaf in leafPos)
                    {
                        GameObject leaf = new GameObject();
                        var leafData = leaf.AddComponent<PMSphere>();
                        leafData.segment = 1;
                        leafData.meshMaterial = _leafMat;
                        leafData.size = Random.Range(leafSize - 0.5f, leafSize + 0.5f);
                        leafData.UpdateMesh();
                        leaf.transform.position = eachLeaf;
                        leaf.transform.parent = _treeGameObj.transform;
                        _leafObjects.Add(leaf);
                    }
                    break;
                case LeafType.Cube:
                    foreach (Vector3 eachLeaf in leafPos)
                    {
                        GameObject leaf = new GameObject();
                        var leafData = leaf.AddComponent<PMSphere>();
                        leafData.segment = 0;
                        leafData.meshMaterial = _leafMat;
                        leafData.size = Random.Range(leafSize - 0.5f, leafSize + 0.5f);
                        leafData.UpdateMesh();
                        leaf.transform.position = eachLeaf;
                        leaf.transform.parent = _treeGameObj.transform;
                        _leafObjects.Add(leaf);
                    }
                    break;
            }
        }
        /// <summary>
        /// Function to store the necessary data for setting up the following branch
        /// </summary>
        TrunkRingData SetupNextTrunkRingData(Vector3[] lastRingVertices, int subRingPolygon, Vector3 lastRingGrowDirection)
        {
            TrunkRingData result = new TrunkRingData();
            Vector3[] subStartVertices = new Vector3[subRingPolygon];
            Vector3 subRingPivotPoint = new Vector3();
            for (int i = 0; i < subRingPolygon; i++)
            {
                subStartVertices[i] = lastRingVertices[i];
                subRingPivotPoint += subStartVertices[i];
            }

            subRingPivotPoint /= subStartVertices.Length;
            result.trunkRingPivot = subRingPivotPoint;
            result.trunkRingVertexPos = subStartVertices;
            result.trunkRingRadius = Vector3.Distance(subStartVertices[0], subRingPivotPoint);
            result.trunkRingGrowDirection = lastRingGrowDirection;
            return result;
        }
    }
}
