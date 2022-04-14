using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bottle.Extensions.Singleton;
namespace Bottle.Core.Manager
{
    public class GameplayManager : PersistentObject<GameplayManager>
    {
        public static class Core
        {
            //public static GameplaySettings Settings { get { return GameplaySettings.Instance; } }
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

