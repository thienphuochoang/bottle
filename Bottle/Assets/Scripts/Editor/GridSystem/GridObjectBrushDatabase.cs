using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Bottle.Core.GridObjectData;
using Sirenix.OdinInspector;
namespace Bottle.Editor.GridSystem
{
    public struct GridObjectBrushDatabaseList
    {
        public List<GridObjectBrushDatabase> brushDatabases;

        public GridObjectBrushDatabaseList(string[] guids)
        {
            brushDatabases = new List<GridObjectBrushDatabase>();
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                brushDatabases.Add(AssetDatabase.LoadAssetAtPath<GridObjectBrushDatabase>(path));
            }
        }

        public string[] GetNameList()
        {
            List<string> names = new List<string>();
            for (int i = 0; i < brushDatabases.Count; i++)
            {
                if (brushDatabases[i] != null)
                    names.Add(brushDatabases[i].name);
                else
                    brushDatabases.Remove(brushDatabases[i]);
            }
            return names.ToArray();
        }

    }
    public class GridObjectBrushDatabase : ScriptableObject
    {
#if UNITY_EDITOR
        [ReadOnly]
        public int selectedGridBrushIndex = 0;
        [SerializeField]
        private List<GridObjectBrushData> _gridBrushDatas = new List<GridObjectBrushData>();
        public List<GridObjectBrushData> GridBrushDatas
        {
            get
            {
                return _gridBrushDatas;
            }
            set
            {
                if (value == null)
                {
                    _gridBrushDatas = new List<GridObjectBrushData>();
                }
                else
                    _gridBrushDatas = value;
            }
        }

        public GridObjectBrushData SelectedGridBrush
        {
            get
            {
                if (GridBrushDatas.Count > 0 && selectedGridBrushIndex != -1 && GridBrushDatas.Count > selectedGridBrushIndex)
                {
                    return GridBrushDatas[selectedGridBrushIndex];
                }
                else
                    return null;
            }
        }

        public static GridObjectBrushDatabaseList GetAllGridObjectDatabaseGUIDs()
        {
            string[] guids = AssetDatabase.FindAssets("t:GridObjectBrushDatabase");
            return new GridObjectBrushDatabaseList(guids);
        }
#endif
    }
}

