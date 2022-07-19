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
using Bottle.Core.GridObjectAbility;
using Bottle.Core.DetectionSystem;
using System.Linq;
namespace Bottle.Core.Manager
{
    public class GameplayManager : PersistentObject<GameplayManager>
    {
        [Range(0, 10)]
        public int currentTurn = 0;
        public enum GlobalDirection { NONE, POSITIVE_X, NEGATIVE_X, POSITIVE_Z, NEGATIVE_Z };
        public GlobalDirection globalFrontDirection = GlobalDirection.NONE;
        private GridEntity[] _allGridEntities => GridManager.Instance.EntityHolder.GetComponentsInChildren<GridEntity>();
        private GridTile[] _allGridTiles => GridManager.Instance.TileHolder.GetComponentsInChildren<GridTile>();

        [SerializeField]
        private int _turnCount;

        [ShowInInspector]
        public GridEntity controllableMainGridEntity;

        public bool isTurnInProgress = false;

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

        public void UpdateGameTurn(Dictionary<string, object> message)
        {
            currentTurn += 1;
            _turnCount += 1;
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

        private void GetControllableMainGridEntity(Dictionary<string, object> message)
        {
            foreach (var entity in _allGridEntities)
            {
                if (entity.isControllable == true)
                {
                    controllableMainGridEntity = entity;
                    break;
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
            GetControllableMainGridEntity(null);
            RegisterGlobalEvents();
            SaveSceneState(0);
        }
        private void Update()
        {
        }
        protected override void OnApplicationQuit()
        {
            if (File.Exists(DatabaseHelper.sceneStateDatabaseFileName))
            {
                File.Delete(DatabaseHelper.sceneStateDatabaseFileName);
            }
        }
        private void RegisterGlobalEvents()
        {
            EventManager.Instance.StartListening("ChangeMainControllableGridEntity", GetControllableMainGridEntity);
            EventManager.Instance.StartListening("ChangeMainControllableGridEntityPosition", UpdateGameTurn);
            EventManager.Instance.StartListening("RecalculateDetectionView", DetectionView.CheckTargetInView);
        }
    }
}

