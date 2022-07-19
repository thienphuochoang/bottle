using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class DetectionViewCreator : MonoBehaviour
{
    [BoxGroup("Detection View Settings", true, true)]
    [SerializeField]
    private bool isHidingGizmoWhenDeselect = false;
    [BoxGroup("Detection View Settings", true, true)]
    [Tooltip("The X and Z size of this detection view in the Grid.")]
    [SerializeField]
    private int _xzBoundingBoxSize = 1;
    public int xzBoundingBoxSize
    {
        get => _xzBoundingBoxSize;
        set
        {
            if (_xzBoundingBoxSize == value) return;
            _xzBoundingBoxSize = value;
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
        }
    }
    private void Update()
    {
        if (yBoundingBoxSize % 2 != 0)
        {
            yBoundingBoxSize = yBoundingBoxSize + 1;
        }
    }
    private void OnValidate()
    {
        if (xzBoundingBoxSize < 1)
            xzBoundingBoxSize = 1;
        if (yBoundingBoxSize < 2)
            yBoundingBoxSize = 2;
    }

    private void DrawDetectionView()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawCube(transform.position, new Vector3(xzBoundingBoxSize + xzBoundingBoxSize - 1, yBoundingBoxSize, xzBoundingBoxSize + xzBoundingBoxSize - 1));
    }
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        DrawDetectionView();
    }
    private void OnDrawGizmos()
    {
        if (isHidingGizmoWhenDeselect == false)
        {
            DrawDetectionView();
        }
    }
#endif
}
