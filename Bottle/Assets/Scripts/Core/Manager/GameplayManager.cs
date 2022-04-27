using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Bottle.Extensions.Singleton;
using Bottle.Core.GridObjectData;
using Sirenix.OdinInspector;
namespace Bottle.Core.Manager
{
    [System.Serializable]
    public struct GridObjectSavedData
    {
        [ShowInInspector]
        public GridTile savedGridTile;
        [ShowInInspector]
        public GridEntity savedGridEntity;
        [ShowInInspector]
        public Vector2Int savedGridPosition;
        [ShowInInspector]
        public float savedGridHeight;

        public GridObjectSavedData(GridObjectSavedData gridObjectSavedData)
        {
            this.savedGridTile = gridObjectSavedData.savedGridTile;
            this.savedGridEntity = gridObjectSavedData.savedGridEntity;
            this.savedGridPosition = gridObjectSavedData.savedGridPosition;
            this.savedGridHeight = gridObjectSavedData.savedGridHeight;
        }
    }

    public class GameplayManager : PersistentObject<GameplayManager>
    {
        public int currentTurn = 0;

        private GridEntity[] _allGridEntities => GridManager.Instance.EntityHolder.GetComponentsInChildren<GridEntity>();
        private GridTile[] _allGridTiles => GridManager.Instance.TileHolder.GetComponentsInChildren<GridTile>();

        [SerializeField]
        private int _turnCount;

        private List<int> _turns = new List<int>();
        public List<int> Turns
        {
            get
            {
                return _turns;
            }
            set
            {
                _turns = value;
            }
        }
        [SerializeField]
        protected Dictionary<int, List<GridObjectSavedData>> gridObjectSavedDatas = new Dictionary<int, List<GridObjectSavedData>>();

        T CopyComponent<T>(T original, GameObject destination) where T : Component
        {
            System.Type type = original.GetType();
            var dst = destination.GetComponent(type) as T;
            if (!dst) dst = destination.AddComponent(type) as T;
            var fields = type.GetFields();
            foreach (var field in fields)
            {
                if (field.IsStatic) continue;
                field.SetValue(dst, field.GetValue(original));
            }
            var props = type.GetProperties();
            foreach (var prop in props)
            {
                if (!prop.CanWrite || !prop.CanWrite || prop.Name == "name") continue;
                prop.SetValue(dst, prop.GetValue(original, null), null);
            }
            return dst as T;
        }

        private void SaveSceneState()
        {
            //List<GridObjectSavedData> gridTileSaveDataList = new List<GridObjectSavedData>();
            //foreach (GridTile gridTile in allGridTiles)
            //{
            //    GridObjectSavedData currentGridObjectData = new GridObjectSavedData();
            //    currentGridObjectData.gridTile = gridTile;
            //    currentGridObjectData.gridEntity = null;
            //    gridTileSaveDataList.Add(currentGridObjectData);
            //}
            //gridObjectSavedDatas.Add(_turnCount, gridTileSaveDataList);
            List<GridObjectSavedData> gridEntitySaveDataList = new List<GridObjectSavedData>();
            for (int i = 0; i < _allGridEntities.Length; i++)
            {
                GridObjectSavedData currentGridObjectData = new GridObjectSavedData();
                currentGridObjectData.savedGridEntity = _allGridEntities[i];
                currentGridObjectData.savedGridTile = null;
                currentGridObjectData.savedGridPosition = _allGridEntities[i].gridPosition;
                currentGridObjectData.savedGridHeight = _allGridEntities[i].gridHeight;
                gridEntitySaveDataList.Add(currentGridObjectData);
            }
            if (!gridObjectSavedDatas.ContainsKey(_turnCount))
                gridObjectSavedDatas.Add(_turnCount, gridEntitySaveDataList);
            else
                gridObjectSavedDatas[_turnCount] = gridEntitySaveDataList;
        }

        private void LoadSceneState()
        {
            List<GridObjectSavedData> chosenGridObjectSaveDataList = gridObjectSavedDatas[currentTurn];
            foreach(var gridObjectSaveData in chosenGridObjectSaveDataList)
            {
                GridEntity newGridEntity = new GridEntity();
                newGridEntity.gridPosition = gridObjectSaveData.savedGridPosition;
                newGridEntity.gridHeight = gridObjectSaveData.savedGridHeight;
                CopyComponent<GridEntity>(newGridEntity, gridObjectSaveData.savedGridEntity.gameObject);
            }
        }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
            SaveSceneState();
            //Debug.Log(gridObjectSavedDatas[0][0].gridEntity.gridPosition);
        }
        private void Update()
        {
            if (Input.GetButtonUp("Vertical"))
            {
                _turnCount++;
                SaveSceneState();
                //Debug.Log(gridObjectSavedDatas[_turnCount][0].gridEntity.gridPosition);
                currentTurn = _turnCount;
            }
            if (Input.GetButtonUp("Horizontal"))
            {
                currentTurn = currentTurn - 1;
                LoadSceneState();
            }
        }
    }
}

