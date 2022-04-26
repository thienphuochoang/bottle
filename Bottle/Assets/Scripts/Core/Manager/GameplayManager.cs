using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bottle.Extensions.Singleton;
using Bottle.Core.GridObjectData;
using Sirenix.OdinInspector;
namespace Bottle.Core.Manager
{
    public class GridObjectSavedData
    {
        public GameObject gridGameObject;
        public GridTile gridTile;
        public GridEntity gridEntity;

        public GridObjectSavedData() { }
        public GridObjectSavedData(GridObjectSavedData gridObjectSavedData)
        {
            this.gridGameObject = gridObjectSavedData.gridGameObject;
            this.gridTile = gridObjectSavedData.gridTile;
            this.gridEntity = gridObjectSavedData.gridEntity;
        }
    }

    public class GameplayManager : PersistentObject<GameplayManager>
    {
        [ValueDropdown("_turns")]
        public int currentTurn;


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
        private List<GridObjectSavedData> gridObjectSavedDatas = new List<GridObjectSavedData>();
        public void SaveSceneState()
        {
            GridTile[] allGridTiles = GridManager.Instance.TileHolder.GetComponentsInChildren<GridTile>();
            foreach (GridTile gridTile in allGridTiles)
            {
                GridObjectSavedData currentGridObjectData = new GridObjectSavedData();
                currentGridObjectData.gridGameObject = gridTile.gameObject;
                currentGridObjectData.gridTile = gridTile;
                currentGridObjectData.gridEntity = null;
            }
            GridEntity[] allGridEntities = GridManager.Instance.EntityHolder.GetComponentsInChildren<GridEntity>();
        }

        protected override void Awake()
        {
            base.Awake();
            //Core.Settings.Init();
        }

        protected override void Start()
        {
            base.Start();
            //Core.Settings.EventInit();
        }
    }
}

