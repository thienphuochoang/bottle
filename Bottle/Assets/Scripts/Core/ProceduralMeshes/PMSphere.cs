using UnityEngine;

namespace Bottle.ProceduralMeshes
{
    public class PMSphere : PMBaseMesh
    {
        private const int SIDE_COUNT = 6;

        private readonly Vector3[] _sphereSides =
        {
            Vector3.up,
            Vector3.down,
            Vector3.right,
            Vector3.left,
            Vector3.forward,
            Vector3.back
        };

        private int _currentSide;

        private MeshFilter[] _meshFilters = new MeshFilter[SIDE_COUNT];
        private MeshRenderer[] _meshRenderers = new MeshRenderer[SIDE_COUNT];

        protected override void InitMesh()
        {
            if (_isInit) return;

            name = "Sphere";
            meshType = ProceduralMeshType.Sphere;

            _currentSize = size;
            _currentSegment = segment;
            _currentMat = meshMaterial;

            for (int i = 0; i < SIDE_COUNT; i++)
            {
                GameObject children = new GameObject("Sphere Side");
                children.transform.parent = transform;
                _meshFilters[i] = children.AddComponent<MeshFilter>();
                _meshRenderers[i] = children.AddComponent<MeshRenderer>();
            }

            UpdateMesh();

            _isInit = true;
        }

        public override void UpdateMesh()
        {
            // Start from the first side of the sphere (in this case is 'up' side)
            _currentSide = 0;

            int vertexPerRow = GetVertexPerRow(segment);
            int numberOfVertices = vertexPerRow * vertexPerRow;

            int[] triangles = GetTriangles(vertexPerRow);

            for (int i = 0; i < SIDE_COUNT; i++)
            {
                Mesh mesh = new Mesh();
                mesh.vertices = GetVertices(vertexPerRow, numberOfVertices);
                mesh.triangles = triangles;
                mesh.RecalculateNormals();
                
                _meshFilters[i].mesh = mesh;
                _meshRenderers[i].material = meshMaterial;
                
                // Continue to the next side
                _currentSide++;
            }
        }
        
        /// <summary>
        /// Update vertex position function
        /// </summary>
        /// <param name="vertexPerRow">all vertex of one row of a plane side</param>
        /// <param name="totalVertices">a vertex array of a plane side</param>
        /// <returns></returns>
        protected override Vector3[] GetVertices(int vertexPerRow, int totalVertices)
        {
            Vector3[] vertices = new Vector3[totalVertices];

            // Get vector direction
            Vector3 localUp = _sphereSides[_currentSide];
            // Get a grow direction vector of a plane side
            Vector3 Dx = new Vector3(localUp.y, localUp.z, localUp.x);
            Vector3 Dy = Vector3.Cross(localUp, Dx);

            // Iterate every row
            for (int i = 0; i < vertexPerRow; i++)
            {
                // Iterate every vertex
                for (int j = 0; j < vertexPerRow; j++)
                {
                    float yVertexPercent = (float)i / (vertexPerRow - 1);
                    float xVertexPercent = (float)j / (vertexPerRow - 1);
                    // Calculate the direction for growing
                    vertices[j + i * vertexPerRow] =
                        localUp + (xVertexPercent - 0.5f) * 2 * Dx + (yVertexPercent - 0.5f) * 2 * Dy;
                    // Update the position based on size
                    vertices[j + i * vertexPerRow] = vertices[j + i * vertexPerRow].normalized * size;
                }
            }
            return vertices;
        }
    }
}