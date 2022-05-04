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

        private void SaveSceneState(int turn)
        {
            Dictionary<int, GridObjectSaveData> gridObjectSavedDatas = new Dictionary<int, GridObjectSaveData>();
            List<GridObject> gridObjectSaveDataList = new List<GridObject>();
            for (int i = 0; i < _allGridTiles.Length; i++)
            {
                gridObjectSaveDataList.Add(_allGridTiles[i]);
            }
            for (int i = 0; i < _allGridEntities.Length; i++)
            {
                gridObjectSaveDataList.Add(_allGridEntities[i]);
            }
            GridObjectSaveData gridObjectSaveData = new GridObjectSaveData();
            gridObjectSaveData.gridObjectList = gridObjectSaveDataList;
            if (!gridObjectSavedDatas.ContainsKey(turn))
            {
                gridObjectSavedDatas.Add(turn, gridObjectSaveData);
            } 
            else
            {
                gridObjectSavedDatas[turn] = gridObjectSaveData;
            }
            if (turn == 0)
            {
                DatabaseHelper.AddNewDatabase(DatabaseHelper.sceneStateDatabaseFileName, gridObjectSavedDatas);
            }
            else
            {
                foreach (KeyValuePair<int, GridObjectSaveData> pairData in gridObjectSavedDatas)
                {
                    if (pairData.Key < turn)
                    {
                        gridObjectSavedDatas.Remove(pairData.Key);
                    }
                }
                DatabaseHelper.AppendDatabase(DatabaseHelper.sceneStateDatabaseFileName, gridObjectSavedDatas);
            }
        }

        private void LoadSceneState(int turn)
        {
            if (turn >= 0)
            {
                var currentSceneStateDatabase = DatabaseHelper.GetDatabase(DatabaseHelper.sceneStateDatabaseFileName);
                var chosenCurrentTurnSceneStateDatabase = currentSceneStateDatabase[turn];
                for (int i = 0; i < _allGridEntities.Length; i++)
                {
                    foreach (var data in chosenCurrentTurnSceneStateDatabase.gridObjectList)
                    {
                        if (_allGridEntities[i].uID == data.uID)
                        {
                            _allGridEntities[i].gridPosition = data.gridPosition;
                            _allGridEntities[i].gridHeight = data.gridHeight;
                            //break;
                        }
                    }
                }
                for (int i = 0; i < _allGridTiles.Length; i++)
                {
                    foreach (var data in chosenCurrentTurnSceneStateDatabase.gridObjectList)
                    {
                        if (_allGridTiles[i].uID == data.uID)
                        {
                            _allGridTiles[i].gridPosition = data.gridPosition;
                            _allGridTiles[i].gridHeight = data.gridHeight;
                            //break;
                        }
                    }
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
            SaveSceneState(0);
        }
        private void Update()
        {
            if (Input.GetButtonUp("Vertical"))
            {
                _turnCount++;
                currentTurn = _turnCount;
                SaveSceneState(currentTurn);
            }
            if (Input.GetButtonUp("Horizontal"))
            {
                currentTurn = currentTurn - 1;
                LoadSceneState(currentTurn);
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

