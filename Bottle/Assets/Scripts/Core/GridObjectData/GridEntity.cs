using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
namespace Bottle.Core.GridObjectData
{
    public class GridEntity : GridObject
    {
        [BoxGroup("Grid Entity Settings", true, true)]
        [Tooltip("The specific grid entity setup.")]
        public bool isControllable;
    }
}