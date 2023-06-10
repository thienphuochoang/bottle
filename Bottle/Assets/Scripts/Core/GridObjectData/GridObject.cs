using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
using Bottle.Core.Manager;
using Newtonsoft.Json;
using Bottle.Core.GridObjectAbility;
namespace Bottle.Core.GridObjectData
{
    [JsonObject(MemberSerialization.OptIn)]
    [ExecuteInEditMode]
    public abstract class GridObject : MonoBehaviour
    {
        [JsonProperty]
        [ReadOnly]
        [BoxGroup("Grid Object General Settings", true, true)]
        [Tooltip("The unique ID of this grid object in the Grid.")]
        [SerializeField]
        private string _uID;
        public string uID => _uID;
        [BoxGroup("Grid Object General Settings", true, true)]
        [Tooltip("The ability to turn this grid object into preview grid object.")]
        [SerializeField]
        private bool _isPreviewObject = false;

        public bool IsPreviewObject
        {
            get => _isPreviewObject;
            set
            {
                if (_isPreviewObject == value) return;
                _isPreviewObject = value;
                if (OnPreviewObjectChanged != null && _isPreviewObject)
                {
                    OnPreviewObjectChanged(_isPreviewObject);
                }
            }
        }
        
        [JsonProperty]
        [ReadOnly]
        [BoxGroup("Grid Object General Settings", true, true)]
        [Tooltip("The position of this grid object in the Grid.")]
        [SerializeField]
        private Vector2Int _gridPosition = new Vector2Int(int.MaxValue, int.MaxValue);
        public Vector2Int gridPosition
        {
            get => _gridPosition;
            set
            {
                if (_gridPosition == value) return;
                _gridPosition = value;
                if (OnPositionChanged != null)
                {
                    OnPositionChanged(_gridPosition, (int)_gridHeight);
                    if (GetComponent<DetectionViewCreator>() != null)
                    {
                        EventManager.Instance.TriggerEvent("RecalculateDetectionView", new Dictionary<string, object> { { "CurrentGridEntity", this }, 
                            { "TargetGridEntity", GameplayManager.Instance.controllableMainGridEntity }, 
                            { "DetectionViewXZBoundingBoxSize", GetComponent<DetectionViewCreator>().xzBoundingBoxSize}, 
                            { "DetectionViewYBoundingBoxSize", GetComponent<DetectionViewCreator>().yBoundingBoxSize}});
                    }
                }    
            }
        }

        [JsonProperty]
        [ReadOnly]
        [BoxGroup("Grid Object General Settings", true, true)]
        [Tooltip("The height of this grid object in the Grid.")]
        [SerializeField]
        private float _gridHeight = float.MaxValue;

        public float gridHeight
        {
            get => _gridHeight;
            set
            {
                if (_gridHeight == value) return;
                _gridHeight = value;
                if (OnPositionChanged != null)
                {
                    OnPositionChanged(_gridPosition, (int)_gridHeight);
                    if (GetComponent<DetectionViewCreator>() != null)
                    {
                        EventManager.Instance.TriggerEvent("RecalculateDetectionView", new Dictionary<string, object> { { "CurrentGridEntity", this }, 
                            { "TargetGridEntity", GameplayManager.Instance.controllableMainGridEntity }, 
                            { "DetectionViewXZBoundingBoxSize", GetComponent<DetectionViewCreator>().xzBoundingBoxSize}, 
                            { "DetectionViewYBoundingBoxSize", GetComponent<DetectionViewCreator>().yBoundingBoxSize}});
                    }
                }
            }
        }

        [JsonProperty]
        [BoxGroup("Grid Object General Settings", true, true)]
        [Tooltip("The pivot offset value of this grid object in the Grid.")]
        public Vector3 pivotOffset;

        [JsonProperty]
        [BoxGroup("Grid Object General Settings", true, true)]
        [Tooltip("The blockable ability of this grid object in the Grid.")]
        public bool isBlockable = true;

        public delegate void OnPositionChangedDelegate(Vector2Int newGridPosition, int newGridHeight);
        public event OnPositionChangedDelegate OnPositionChanged;
        public delegate void OnPreviewObjectChangedDelegate(bool isPreviewObj);
        public event OnPreviewObjectChangedDelegate OnPreviewObjectChanged;

        protected virtual void Update()
        {
            if (EditorApplication.isPlaying) return;
            if (this.transform.hasChanged)
            {
                Vector3Int gridPosAndGridHeight = GridManager.Instance.ConvertWorldPositionToGridPosition(this);
                gridPosition = new Vector2Int(gridPosAndGridHeight.x, gridPosAndGridHeight.z);
                gridHeight = gridPosAndGridHeight.y;
            }
        }
        protected virtual void OnEnable()
        {
            this.OnPositionChanged += PositionChangedHandler;
            this.OnPreviewObjectChanged += PreviewObjectChangedHandler;
            _uID = this.GetInstanceID().ToString();
        }

        protected virtual void Awake()
        {
            Vector3Int gridPosAndGridHeight = GridManager.Instance.ConvertWorldPositionToGridPosition(this);
            gridPosition = new Vector2Int(gridPosAndGridHeight.x, gridPosAndGridHeight.z);
            gridHeight = gridPosAndGridHeight.y;
        }

        protected virtual void Start()
        {
        }

        private void PositionChangedHandler(Vector2Int newGridPosition, int newGridHeight)
        {
            Vector3Int newCellPos = new Vector3Int(newGridPosition.x, newGridPosition.y, newGridHeight);
            Vector3 newWorldPos = GridManager.Instance.grid.GetCellCenterWorld(newCellPos);
            newWorldPos.y = newGridHeight;
            this.transform.position = newWorldPos;
        }
        
        private void PreviewObjectChangedHandler(bool isPreviewObj)
        {
            SetupPreviewGridObject(this.transform);
        }
        
        private Transform SetupPreviewGridObject(Transform transform)
        {
            // Attempt to get reference to GameObject Renderer
            Renderer meshRenderer = transform.gameObject.GetComponent<Renderer>();

            // If a Renderer was found
            if (meshRenderer != null)
            {
                // Define temporary Material used to create transparent copy of GameObject Material
                Material tempMat = new Material(Shader.Find("Bottle/Preview_Tiles_PBL"));
                Material[] tempMats = new Material[meshRenderer.sharedMaterials.Length];
                
                // Loop through each material in GameObject
                for (int i = 0; i < meshRenderer.sharedMaterials.Length; i++)
                {
                    // Get material from GameObject
                    tempMat = new Material(meshRenderer.sharedMaterials[i]);

                    // Change Shader to "Standard"
                    tempMat.shader = Shader.Find("Bottle/Preview_Tiles_PBL");
                    

                    // Replace GameObject Material with transparent one
                    tempMats[i] = tempMat;
                }
                
                meshRenderer.sharedMaterials = tempMats;
            }

            // Recursively run this method for each child transform
            foreach (Transform child in transform)
            {
                SetupPreviewGridObject(child);
            }

            return transform;
        }
    }
}

