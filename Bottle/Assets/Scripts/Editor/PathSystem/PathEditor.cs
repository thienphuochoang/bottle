using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Bottle.Core.PathSystem;
using Bottle.Core.Manager;
using Bottle.Core.GridObjectData;
[CustomEditor(typeof(PathCreator))]
public class PathEditor : Editor
{
    static int selectedControlPointId = -1;
    static PathCreator pathCreator = null;
    public HandleNode isHoveringHandleNode = new HandleNode(new Vector3(0, 0, 0), -1);
    public List<HandleNode> handleNodeList = new List<HandleNode>();
    public bool isHoveringShift = false;
    public enum StateTypes
    {
        DEFAULT,
        HOVERED
    }
    void ProcessPathInput(Event e)
    {
        /*
        // Find which handle mouse is over. Start by looking at previous handle index first, as most likely to still be closest to mouse
        int previousMouseOverHandleIndex = (mouseOverHandleIndex == -1) ? 0 : mouseOverHandleIndex;
        mouseOverHandleIndex = -1;
        for (int i = 0; i < bezierPath.NumPoints; i += 3)
        {

            int handleIndex = (previousMouseOverHandleIndex + i) % bezierPath.NumPoints;
            float handleRadius = GetHandleDiameter(globalDisplaySettings.anchorSize * data.bezierHandleScale, bezierPath[handleIndex]) / 2f;
            Vector3 pos = MathUtility.TransformPoint(bezierPath[handleIndex], creator.transform, bezierPath.Space);
            float dst = HandleUtility.DistanceToCircle(pos, handleRadius);
            if (dst == 0)
            {
                mouseOverHandleIndex = handleIndex;
                break;
            }
        }
        */
        // Control click to remove point
        if (e.control && e.type == EventType.MouseDown && e.button == 0 && isHoveringHandleNode.StateType == StateTypes.HOVERED)
        {
            Undo.RecordObject(pathCreator, "Delete node");
            pathCreator.nodes.RemoveAt(isHoveringHandleNode.Index);
            isHoveringHandleNode = new HandleNode(new Vector3(0, 0, 0), -1);
            Repaint();
        }
        if (e.shift && isHoveringShift == false)
        {
            Undo.RecordObject(pathCreator, "Add node");
            pathCreator.nodes.Add(pathCreator.nodes[pathCreator.nodes.Count - 1]);
            selectedControlPointId = pathCreator.nodes.Count - 1;
            isHoveringShift = true;
        }
        else if (!e.shift && isHoveringShift == true)
        {
            isHoveringShift = false;
        }
    }
    void DrawPathSceneEditor()
    {
        HandleNode previousNode = handleNodeList[0];
        for (int i = 0; i < handleNodeList.Count; i++)
        {
            float nodeDiameter = handleNodeList[i].GetNodeDiameter();
            float dst = HandleUtility.DistanceToCircle(handleNodeList[i].WorldPos, nodeDiameter);
            if (dst == 0)
            {
                handleNodeList[i].StateType = StateTypes.HOVERED;
                isHoveringHandleNode = handleNodeList[i];
            }
            else
            {
                handleNodeList[i].StateType = StateTypes.DEFAULT;
            }
            handleNodeList[i].DrawNode();
            // Check if this current node is the second node from now on, then draw a path
            if (i > 0)
            {
                NodePath connectedPath = new NodePath(previousNode.WorldPos, handleNodeList[i].WorldPos);
                connectedPath.DrawPath();
            }
            if (selectedControlPointId == i)
            {
                handleNodeList[i].DrawPositionHandle();
            }
            // At the end, the previous node will be updated as a new node
            previousNode = handleNodeList[i];
        }
    }
    public class HandleNode
    {
        private StateTypes _stateType = StateTypes.DEFAULT;
        private Vector3 _worldPos;
        private int _index;
        private Vector2Int _gridPosition;
        private float _gridHeight;
        public Vector3 WorldPos
        {
            get { return _worldPos; }
            set { _worldPos = value; }
        }
        public StateTypes StateType
        {
            get { return _stateType; }
            set { _stateType = value; }
        }
        public int Index
        {
            get { return _index; }
        }
        public HandleNode(Vector3 worldPos, int index)
        {
            _worldPos = worldPos;
            _index = index;
        }
        public Vector2Int gridPosition
        {
            get { return _gridPosition; }
            set
            {
                if (_gridPosition == value) return;
                _gridPosition = value;
                Vector3Int cellPosition = GridManager.Instance.grid.WorldToCell(_worldPos);
                Vector3 finalPos = GridManager.Instance.grid.GetCellCenterWorld(cellPosition);
                _gridPosition = new Vector2Int((int)(finalPos.x - 0.5f), (int)(finalPos.z - 0.5f));
            }
        }
        public float gridHeight
        {
            get { return _gridHeight; }
            set { _gridHeight = value; }
        }

        public void SetState()
        {
            if (_stateType == StateTypes.HOVERED)
                Handles.color = pathCreator.hoveredColor;
            else
                Handles.color = pathCreator.defaultColor;
        }
        public float GetNodeDiameter()
        {
            float constantHandleSize = HandleUtility.GetHandleSize(_worldPos);
            return constantHandleSize * pathCreator.handleSize;
        }
        public void DrawNode()
        {
            SetState();
            float nodeDiameter = GetNodeDiameter();
            int ctrlId = GUIUtility.GetControlID(FocusType.Passive);
            Vector3 nodeHandle = Handles.FreeMoveHandle(ctrlId, _worldPos, Quaternion.identity, nodeDiameter, Vector3.one, Handles.SphereHandleCap);
            DrawBarName();
            // Left Mouse click to point then update ID
            if (ctrlId == EditorGUIUtility.hotControl)
                selectedControlPointId = _index;
        }
        public void DrawPositionHandle()
        {
            EditorGUI.BeginChangeCheck();
            Vector3 newPos = Handles.PositionHandle(_worldPos, Quaternion.identity);
            if (EditorGUI.EndChangeCheck() && newPos != _worldPos)
            {
                Vector3Int cellPosition = GridManager.Instance.grid.WorldToCell(newPos);
                Vector3 finalPos = GridManager.Instance.grid.GetCellCenterWorld(cellPosition);
                finalPos.y = Mathf.Ceil(newPos.y);
                _gridPosition = new Vector2Int((int)(finalPos.x - 0.5f), (int)(finalPos.z - 0.5f));
                _gridHeight = finalPos.y;
                Undo.RecordObject(pathCreator, "Move Node");
                pathCreator.nodes[_index] = pathCreator.transform.InverseTransformPoint(finalPos);
                _worldPos = pathCreator.transform.InverseTransformPoint(newPos);
            }
        }
        public void DrawBarName()
        {
            Vector3 newWorldPos = new Vector3(_worldPos.x, _worldPos.y + 0.2f, _worldPos.z);
            GUIStyle style = new GUIStyle(GUI.skin.textArea);
            style.normal.textColor = pathCreator.defaultColor;
            style.fontSize = 12;
            Handles.Label(newWorldPos, _index.ToString(), style);
        }
    }
    public class NodePath
    {
        private Vector3 _firstWorldPos;
        private Vector3 _secondWorldPos;
        public NodePath(Vector3 firstWorldPos, Vector3 secondWorldPos)
        {
            _firstWorldPos = firstWorldPos;
            _secondWorldPos = secondWorldPos;
        }

        public void DrawPath()
        {
            Handles.color = pathCreator.defaultColor;
            Handles.DrawLine(_firstWorldPos, _secondWorldPos);
        }
    }
    void OnEnable()
    {
        pathCreator = (target as PathCreator);
        Vector3[] localPoints = pathCreator.nodes.ToArray();
        Vector3[] worldPoints = new Vector3[pathCreator.nodes.Count];
        for (int i = 0; i < worldPoints.Length; i++)
        {
            worldPoints[i] = pathCreator.transform.TransformPoint(localPoints[i]);
            HandleNode currentNode = new HandleNode(worldPoints[i], i);
            currentNode.StateType = StateTypes.DEFAULT;
            if (!handleNodeList.Contains(currentNode))
                handleNodeList.Add(currentNode);
        }
    }
    void OnSceneGUI()
    {
        Event currentEvent = Event.current;
        EventType currentEventType = Event.current.type;
        if (currentEventType != EventType.Repaint && currentEventType != EventType.Layout)
        {
            ProcessPathInput(currentEvent);
        }
        // Avoid Ctrl+Z action, re-create handleNodeList
        if (pathCreator.nodes.Count != handleNodeList.Count)
        {
            Vector3[] localPoints = pathCreator.nodes.ToArray();
            Vector3[] worldPoints = new Vector3[pathCreator.nodes.Count];
            handleNodeList.Clear();
            for (int i = 0; i < worldPoints.Length; i++)
            {
                worldPoints[i] = pathCreator.transform.TransformPoint(localPoints[i]);
                HandleNode currentNode = new HandleNode(worldPoints[i], i);
                currentNode.StateType = StateTypes.DEFAULT;
                if (!handleNodeList.Contains(currentNode))
                    handleNodeList.Add(currentNode);
            }
        }
        for (int i = 0; i < pathCreator.nodes.Count; i++)
        {
            handleNodeList[i].WorldPos = pathCreator.transform.TransformPoint(pathCreator.nodes.ToArray()[i]);
        }
        DrawPathSceneEditor();
        // Don't allow clicking over empty space to deselect the object
        if (currentEventType == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(0);
        }
    }
}

