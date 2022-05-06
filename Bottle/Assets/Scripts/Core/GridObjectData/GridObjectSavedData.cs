using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using Bottle.Core.Manager;
using Newtonsoft.Json;
using Bottle.Extensions.Helper;
namespace Bottle.Core.GridObjectData
{
    [System.Serializable]
    public struct GridObjectSaveData
    {
        public List<GridObject> gridObjectList;
        public GridObjectSaveData(List<GridObject> gridObjectList)
        {
            this.gridObjectList = gridObjectList;
        }
    }
}

