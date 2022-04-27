using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Bottle.Extensions.Singleton;
using Bottle.Core.GridObjectData;
using Sirenix.OdinInspector;
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

        [SerializeField]
        protected Dictionary<int, List<GridObjectSavedData>> gridObjectSavedDatas = new Dictionary<int, List<GridObjectSavedData>>();

        private void SaveSceneState()
        {
            List<GridObjectSavedData> gridObjectSaveDataList = new List<GridObjectSavedData>();
            for (int i = 0; i < _allGridTiles.Length; i++)
            {
                GridObjectSavedData currentGridObjectData = new GridObjectSavedData();
                currentGridObjectData.savedGridEntity = null;
                currentGridObjectData.savedGridTile = _allGridTiles[i];
                currentGridObjectData.savedGridPosition = _allGridTiles[i].gridPosition;
                currentGridObjectData.savedGridHeight = _allGridTiles[i].gridHeight;
                gridObjectSaveDataList.Add(currentGridObjectData);
            }
            for (int i = 0; i < _allGridEntities.Length; i++)
            {
                GridObjectSavedData currentGridObjectData = new GridObjectSavedData();
                currentGridObjectData.savedGridEntity = _allGridEntities[i];
                currentGridObjectData.savedGridTile = null;
                currentGridObjectData.savedGridPosition = _allGridEntities[i].gridPosition;
                currentGridObjectData.savedGridHeight = _allGridEntities[i].gridHeight;
                gridObjectSaveDataList.Add(currentGridObjectData);
            }
            if (!gridObjectSavedDatas.ContainsKey(_turnCount))
            {
                gridObjectSavedDatas.Add(_turnCount, gridObjectSaveDataList);
            } 
            else
            {
                gridObjectSavedDatas[_turnCount] = gridObjectSaveDataList;
            }
        }

        private void LoadSceneState()
        {
            if (currentTurn >= 0)
            {
                List<GridObjectSavedData> chosenGridObjectSaveDataList = gridObjectSavedDatas[currentTurn];
                foreach (var gridObjectSaveData in chosenGridObjectSaveDataList)
                {
                    Vector3Int savedCellPos = new Vector3Int(gridObjectSaveData.savedGridPosition.x, gridObjectSaveData.savedGridPosition.y, (int)gridObjectSaveData.savedGridHeight);
                    Vector3 savedWorldPos = GridManager.Instance.grid.GetCellCenterWorld(savedCellPos);
                    savedWorldPos.y = gridObjectSaveData.savedGridHeight;
                    if (gridObjectSaveData.savedGridEntity != null)
                        gridObjectSaveData.savedGridEntity.transform.position = savedWorldPos;
                    else if (gridObjectSaveData.savedGridTile != null)
                        gridObjectSaveData.savedGridTile.transform.position = savedWorldPos;
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

