using UnityEngine;
namespace Bottle.ProceduralMeshes
{
    public enum ProceduralMeshType
    {
        None,
        Sphere
    }

    /// <summary>
    /// General base class for Primitive Procedural Meshes, which is for setting up
    /// based logic and shared methods for all the generated mesh classes.
    /// </summary>
    [ExecuteInEditMode]
    public abstract class PMBaseMesh : MonoBehaviour
    {
        public ProceduralMeshType meshType = ProceduralMeshType.None;
        public Material meshMaterial;

        [Range(0, 5)] public float size = 1;
        [Range(0, 5)] public int segment = 0;

        protected bool _isInit = false;
        protected float _currentSize;
        protected int _currentSegment;
        protected Material _currentMat;

        /// <summary>
        /// Abstract method for initializing mesh components such as mesh
        /// filer & renderer.
        /// </summary>
        protected abstract void InitMesh();

        /// <summary>
        /// Abstract method for re-creating the mesh whenever there's a
        /// change happened to 'size' and 'segment' parameters.
        /// </summary>
        public abstract void UpdateMesh();

        /// <summary>
        /// Abstract method for getting all the vertices of the current mesh.
        /// </summary>
        /// <param name="vertexPerRow">How many vertex of a row based on the mesh resolution.</param>
        /// <param name="totalVertices">The count of all vertices of the mesh.</param>
        /// <returns>An array of vertices (Vector3)</returns>
        protected abstract Vector3[] GetVertices(int vertexPerRow, int totalVertices);

        //protected abstract void SetMaterial(Material mat);

        private void OnEnable()
        {
            InitMesh();
        }

        private void OnValidate()
        {
            if (_currentSize != size || _currentSegment != segment || _currentMat != meshMaterial)
                UpdateMesh();

            _currentSize = size;
            _currentSegment = segment;
        }

        protected int GetVertexPerRow(int resolution)
        {
            int vertexCount = 2;
            for (int i = 0; i < resolution; i++)
                vertexCount += (int)Mathf.Pow(2, i);

            return vertexCount;
        }

        protected int[] GetTriangles(int vertexPerRow)
        {
            int vertexCount = vertexPerRow * vertexPerRow;

            int bytePerTriangle = 6;
            // Total number of vertex faces of a plane side
            int trianglesNumber = 2 * (int)Mathf.Pow(2, segment + segment) * 3;
            // Number of vertex faces of a whole object
            int[] triangles = new int[trianglesNumber * bytePerTriangle];

            int triangleCounter = 0;

            // Iterate through each row of every plane side
            for (int i = 0; triangleCounter < vertexCount - vertexPerRow; i += 6)
            {
                // Skip last edge vertex
                if (i != 0 && (i / bytePerTriangle + 1) % vertexPerRow == 0)
                {
                    triangleCounter++;
                    continue;
                }

                triangles[i] = triangleCounter;
                triangles[i + 1] = triangleCounter + vertexPerRow + 1;
                triangles[i + 2] = triangleCounter + vertexPerRow;

                triangles[i + 3] = triangleCounter;
                triangles[i + 4] = triangleCounter + 1;
                triangles[i + 5] = triangleCounter + vertexPerRow + 1;

                triangleCounter++;
            }

            return triangles;
        }
    }
}