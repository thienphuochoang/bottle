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
    [System.Serializable]
    public abstract class GameData
    {
        public event UnityAction ChangedAction;
        protected void SendChanged()
        {
            if (ChangedAction != null)
                ChangedAction.Invoke();
        }

        public abstract void Save();
        public abstract void Load();
    }

    /// <summary>
    ///      GameData for setting which value we want to save and load. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public class GameData<T> : GameData
    {
        [ShowInInspector]
        public T Value { get; protected set; }
        public GameData(T defaultValue, UnityAction onChanged = null)
        {
            Value = defaultValue;
            ChangedAction += onChanged;
            //GameplayManager.Instance.RegisterData(this);
        }
        public override void Save()
        {
            Debug.Log("Save");
        }
        public override void Load()
        {
            Debug.Log("Load");
        }
        public void Set(T value)
        {
            Value = value;
            SendChanged();
        }
        public void ResetValue() => Value = default;
        //public void Remove() => DataManager.Instance.DeregisterData(this);
    }
}

