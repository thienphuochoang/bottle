using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bottle.Extensions.Singleton;
using Bottle.Core.GridObjectData;
namespace Bottle.Core.Manager
{
    public class GameplayManager : PersistentObject<GameplayManager>
    {
        private int turn;
        public void SaveSceneState()
        {
            GridTile[] allGridTiles = GridManager.Instance.TileHolder.GetComponentsInChildren<GridTile>();
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

