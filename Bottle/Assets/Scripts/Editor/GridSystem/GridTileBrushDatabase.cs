using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Bottle.Core.GridObjectData;
namespace Bottle.Editor.GridSystem
{
    public struct GridTileBrushDatabaseList
    {
        public List<GridTileBrushDatabase> brushDatabases;

        public GridTileBrushDatabaseList(string[] guids)
        {
            brushDatabases = new List<GridTileBrushDatabase>();
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                brushDatabases.Add(AssetDatabase.LoadAssetAtPath<GridTileBrushDatabase>(path));
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
    public class GridTileBrushDatabase : ScriptableObject
    {
#if UNITY_EDITOR
        [SerializeField]
        private List<GridTileData> _gridTileDatas = new List<GridTileData>();
        public List<GridTileData> GridTileDatas
        {
            get
            {
                return _gridTileDatas;
            }
            set
            {
                if (value == null)
                {
                    _gridTileDatas = new List<GridTileData>();
                }
                else
                    _gridTileDatas = value;
            }
        }
        public static GridTileBrushDatabaseList GetAllGridTileDatabaseGUIDs()
        {
            string[] guids = AssetDatabase.FindAssets("t:GridTileBrushDatabase");
            return new GridTileBrushDatabaseList(guids);
        }
#endif
    }
}

