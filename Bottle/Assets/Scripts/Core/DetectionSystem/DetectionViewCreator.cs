using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Bottle.Core.GridObjectAbility;
using Bottle.Core.Manager;
using Bottle.Core.GridObjectData;
namespace Bottle.Core.DetectionSystem
{
    [ExecuteInEditMode]
    public class DetectionViewCreator : MonoBehaviour
    {
        private GridEntity currentGridEntity;
        public static Vector2Int[] eightDirections = new Vector2Int[]
        {
            new Vector2Int(0, 1),
            new Vector2Int(1, 1),
            new Vector2Int(1, 0),
            new Vector2Int(1, -1),
            new Vector2Int(0, -1),
            new Vector2Int(-1, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(-1, 1)
        };
        [BoxGroup("Detection View Settings", true, true)]
        [Tooltip("The X size of this detection view in the Grid.")]
        [SerializeField]
        private int _xBoundingBoxSize = 1;
        public int xBoundingBoxSize
        {
            get => _xBoundingBoxSize;
            set
            {
                if (_xBoundingBoxSize == value) return;
                _xBoundingBoxSize = value;
                if (OnSizeChanged != null)
                    OnSizeChanged(_xBoundingBoxSize);
            }
        }
        [BoxGroup("Detection View Settings", true, true)]
        [Tooltip("The Y size of this detection view in the Grid.")]
        [SerializeField]
        private int _yBoundingBoxSize = 1;
        public int yBoundingBoxSize
        {
            get => _yBoundingBoxSize;
            set
            {
                if (_yBoundingBoxSize == value) return;
                _yBoundingBoxSize = value;
                if (OnSizeChanged != null)
                    OnSizeChanged(_yBoundingBoxSize);
            }
        }
        [BoxGroup("Detection View Settings", true, true)]
        [Tooltip("The Z size of this detection view in the Grid.")]
        [SerializeField]
        private int _zBoundingBoxSize = 1;
        public int zBoundingBoxSize
        {
            get => _zBoundingBoxSize;
            set
            {
                if (_zBoundingBoxSize == value) return;
                _zBoundingBoxSize = value;
                if (OnSizeChanged != null)
                    OnSizeChanged(_zBoundingBoxSize);
            }
        }
        [TitleGroup("Color Settings", alignment: TitleAlignments.Centered), Tooltip("Color Settings of nodes and paths")]
        public Color defaultColor = Color.white;
        public Color hoveredColor = Color.red;
        public Color selectedColor = Color.green;

        public delegate void OnSizeChangedDelegate(int newBoundingBoxSize);
        public event OnSizeChangedDelegate OnSizeChanged;

        private void OnDetectionViewChangedHandler(Vector2Int newGridPosition, int newGridHeight)
        {
        }
        public void CalculateDetectionView(Dictionary<string, object> message)
        {
            if ((GridEntity)message["GridEntity"] == currentGridEntity)
            {
                for (int i = 1; i <= xBoundingBoxSize; i++)
                {
                    if (i == 2)
                    {
                        foreach (Vector2Int direction in eightDirections)
                        {
                            Vector2Int targetGridEntity = new Vector2Int(currentGridEntity.gridPosition.x, currentGridEntity.gridPosition.y) + direction;
                            List<GridEntity> gridEntitiesAtPosition = GridManager.Instance.GetGridObjectAtPosition<GridEntity>(targetGridEntity, (int)currentGridEntity.gridHeight);
                            if (gridEntitiesAtPosition.Count > 0)
                            {
                                Debug.Log(gridEntitiesAtPosition[0]);
                            }
                        }
                    }
                    if (i == 4)
                    {
                        for (int n = 0; n < eightDirections.Length; n++)
                        {
                            Vector2Int currentTargetEntity = new Vector2Int(currentGridEntity.gridPosition.x, currentGridEntity.gridPosition.y) + eightDirections[n] * (i - 1);
                            Vector2Int nextTargetEntity = new Vector2Int(currentGridEntity.gridPosition.x, currentGridEntity.gridPosition.y) + eightDirections[n + 1] * (i - 1);
                            Debug.Log(currentTargetEntity);
                            Debug.Log(nextTargetEntity);
                        }
                        //foreach (Vector2Int direction in eightDirections)
                        //{
                        //    Vector2Int targetGridEntity = new Vector2Int(currentGridEntity.gridPosition.x, currentGridEntity.gridPosition.y) + direction * (i - 1);
                        //    List<GridEntity> gridEntitiesAtPosition = GridManager.Instance.GetGridObjectAtPosition<GridEntity>(targetGridEntity, (int)currentGridEntity.gridHeight);
                        //    if (gridEntitiesAtPosition.Count > 0)
                        //    {
                        //        Debug.Log(gridEntitiesAtPosition[0]);
                        //    }
                        //}
                    }
                }
            }

        }
        private void Awake() 
        {
            currentGridEntity = GetComponent<GridEntity>();
        }
        public void Start()
        {
            EventManager.Instance.StartListening("RecalculateDetectionView", CalculateDetectionView);
            //_thisGridObject.OnPositionChanged += OnDetectionViewChangedHandler;
        }
        private void Update()
        {
            //if (xBoundingBoxSize % 2 == 0)
            //{
            //    xBoundingBoxSize = xBoundingBoxSize + 1;
            //}
            //if (zBoundingBoxSize % 2 == 0)
            //{
            //    zBoundingBoxSize = zBoundingBoxSize + 1;
            //}
            if (yBoundingBoxSize % 2 != 0)
            {
                yBoundingBoxSize = yBoundingBoxSize + 1;
            }
        }
        private void OnValidate()
        {
            if (xBoundingBoxSize < 1)
                xBoundingBoxSize = 1;
            if (zBoundingBoxSize < 1)
                zBoundingBoxSize = 1;
            if (yBoundingBoxSize < 2)
                yBoundingBoxSize = 2;
        }
#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Gizmos.color = new Color(1, 0, 0, 0.3f);
            Gizmos.DrawCube(transform.position, new Vector3(xBoundingBoxSize + xBoundingBoxSize - 1, yBoundingBoxSize, zBoundingBoxSize + zBoundingBoxSize - 1));
        }
#endif
    }
}

