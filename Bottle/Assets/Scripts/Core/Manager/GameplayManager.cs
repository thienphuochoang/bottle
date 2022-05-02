using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Bottle.Extensions.Singleton;
using Bottle.Core.GridObjectData;
using Sirenix.OdinInspector;
using Newtonsoft.Json;
using System.IO;
using Bottle.Extensions.Helper;
namespace Bottle.Core.Manager
{
    public class GameplayManager : PersistentObject<GameplayManager>
    {
        [Range(0, 10)]
        public int currentTurn = 0;

        private GridEntity[] _allGridEntities => GridManager.Instance.EntityHolder.GetComponentsInChildren<GridEntity>();
        private GridTile[] _allGridTiles => GridManager.Instance.TileHolder.GetComponentsInChildren<GridTile>();

        [SerializeField]
        private int _turnCount;

        public Dictionary<int, GridObjectSaveData> gridObjectSavedDatas = new Dictionary<int, GridObjectSaveData>();

        private void SaveSceneState()
        {
            List<GridObject> gridObjectSaveDataList = new List<GridObject>();
            for (int i = 0; i < _allGridTiles.Length; i++)
            {
                //GridObjectSavedData currentGridObjectData = new GridObjectSavedData();
                //currentGridObjectData.savedGridEntity = null;
                //currentGridObjectData.savedGridTile = _allGridTiles[i];
                //currentGridObjectData.savedGridPosition = _allGridTiles[i].gridPosition;
                //currentGridObjectData.savedGridHeight = _allGridTiles[i].gridHeight;
                //gridObjectSaveDataList.Add(_allGridTiles[i]);
            }
            for (int i = 0; i < _allGridEntities.Length; i++)
            {
                //json = JsonUtility.ToJson(_allGridEntities[i]);
                //GridObjectSavedData currentGridObjectData = new GridObjectSavedData();
                //currentGridObjectData.savedGridEntity = _allGridEntities[i];
                //currentGridObjectData.savedGridTile = null;
                //currentGridObjectData.savedGridPosition = _allGridEntities[i].gridPosition;
                //currentGridObjectData.savedGridHeight = _allGridEntities[i].gridHeight;
                gridObjectSaveDataList.Add(_allGridEntities[i]);
            }
            GridObjectSaveData gridObjectSaveData = new GridObjectSaveData();
            gridObjectSaveData.gridObjectList = gridObjectSaveDataList;
            if (!gridObjectSavedDatas.ContainsKey(_turnCount))
            {
                gridObjectSavedDatas.Add(_turnCount, gridObjectSaveData);
            } 
            else
            {
                gridObjectSavedDatas[_turnCount] = gridObjectSaveData;
            }
            if (!File.Exists(DatabaseHelper.sceneStateDatabaseFileName))
            {
                DatabaseHelper.AddNewDatabase(DatabaseHelper.sceneStateDatabaseFileName, gridObjectSavedDatas);
            }
            else
            {
                //DatabaseHelper.UpdateDatabase(DatabaseHelper.sceneStateDatabaseFileName, gridObjectSavedDatas);
            }
        }

        private void LoadSceneState()
        {
            if (currentTurn >= 0)
            {
                var currentSceneStateDatabase = DatabaseHelper.GetDatabase(DatabaseHelper.sceneStateDatabaseFileName);
                var chosenCurrentTurnSceneStateDatabase = currentSceneStateDatabase[currentTurn];
                foreach (var ahihi in chosenCurrentTurnSceneStateDatabase.gridObjectList)
                {
                    Debug.Log(ahihi.gridPosition);
                }
                //foreach (var gridObjectSaveData in chosenGridObjectSaveDataList.gridObjectList)
                //{
                //    for (int i = 0; i < _allGridTiles.Length; i++)
                //    {
                //        if (_allGridTiles[i].uID == gridObjectSaveData.uID)
                //        {
                //            _allGridTiles[i].gridPosition = gridObjectSaveData.gridPosition;
                //            _allGridTiles[i].gridHeight = gridObjectSaveData.gridHeight;
                //        }
                //    }
                //    for (int i = 0; i < _allGridEntities.Length; i++)
                //    {
                //        if (_allGridEntities[i].uID == gridObjectSaveData.uID)
                //        {
                //            _allGridEntities[i].gridPosition = gridObjectSaveData.gridPosition;
                //            _allGridEntities[i].gridHeight = gridObjectSaveData.gridHeight;
                //        }
                //    }
                    //Vector3Int savedCellPos = new Vector3Int(gridObjectSaveData.savedGridPosition.x, gridObjectSaveData.savedGridPosition.y, (int)gridObjectSaveData.savedGridHeight);
                    //Vector3 savedWorldPos = GridManager.Instance.grid.GetCellCenterWorld(savedCellPos);
                    //savedWorldPos.y = gridObjectSaveData.savedGridHeight;
                    //if (gridObjectSaveData.savedGridEntity != null)
                    //    gridObjectSaveData.savedGridEntity.transform.position = savedWorldPos;
                    //else if (gridObjectSaveData.savedGridTile != null)
                    //    gridObjectSaveData.savedGridTile.transform.position = savedWorldPos;
                //}
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
        }
        private void Update()
        {
            if (Input.GetButtonUp("Vertical"))
            {
                _turnCount++;
                SaveSceneState();
                currentTurn = _turnCount;
            }
            if (Input.GetButtonUp("Horizontal"))
            {
                //currentTurn = currentTurn - 1;
                LoadSceneState();
            }
        }
        protected override void OnApplicationQuit()
        {
            if (File.Exists(DatabaseHelper.sceneStateDatabaseFileName))
            {
                File.Delete(DatabaseHelper.sceneStateDatabaseFileName);
            }
        }
    }
}

