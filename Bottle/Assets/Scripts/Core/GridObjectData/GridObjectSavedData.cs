using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using Bottle.Core.Manager;
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
        public GridObject savedGridObject;
        public event UnityAction Changed;
        protected void SendChanged() => Changed?.Invoke();

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
        public GameData(GridObject assignedGridObject, T defaultValue, UnityAction onChanged = null)
        {
            savedGridObject = assignedGridObject;
            Value = defaultValue;
            Changed += onChanged;
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

