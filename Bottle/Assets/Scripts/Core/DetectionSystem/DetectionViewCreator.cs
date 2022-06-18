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
        private GridEntity thisGridObject;
        [BoxGroup("Detection View Settings", true, true)]
        [Tooltip("The size of this detection view in the Grid.")]
        [SerializeField]
        private Vector3Int _boundingBoxSize = new Vector3Int(1, 1, 1);
        public Vector3Int boundingBoxSize
        {
            get => _boundingBoxSize;
            set
            {
                if (_boundingBoxSize == value) return;
                _boundingBoxSize = value;
                if (OnSizeChanged != null)
                    OnSizeChanged(_boundingBoxSize);
            }
        }
        [TitleGroup("Color Settings", alignment: TitleAlignments.Centered), Tooltip("Color Settings of nodes and paths")]
        public Color defaultColor = Color.white;
        public Color hoveredColor = Color.red;
        public Color selectedColor = Color.green;

        public delegate void OnSizeChangedDelegate(Vector3Int newBoundingBoxSize);
        public event OnSizeChangedDelegate OnSizeChanged;

        private void OnSizeChangedHandler(Vector3Int newBoundingBoxSize)
        {
            
        }
        private void Awake()
        {
            thisGridObject = GetComponent<GridEntity>();
        }
        private void Start()
        {
            this.OnSizeChanged += OnSizeChangedHandler;
        }
        private void Update()
        {
            if (boundingBoxSize.x % 2 == 0)
            { }
                //var newX = boundingBoxSize.x + 1;
        }
#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            // Draw a semitransparent blue cube at the transforms position
            Gizmos.color = new Color(1, 0, 0, 0.3f);
            Gizmos.DrawCube(transform.position, boundingBoxSize);
        }
#endif
    }
}

