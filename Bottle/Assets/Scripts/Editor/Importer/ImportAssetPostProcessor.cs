using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using Bottle.Extensions.Helper;
namespace Bottle.Editor.Importer
{
    public class ImportAssetPostProcessor : AssetPostprocessor
    {
        void OnPostprocessModel (GameObject g)
        {
            // TO DO: Need to implement this feature, it is inconvenient
            // Need to open the tool to use auto import function
            //if (ImportEditor.Instance == null) return;
            //if (assetPath.EndsWith(".fbx") == false) return;
            /*
            string metaFilePath = assetPath.Replace(".fbx", ".fbx.meta");
            var (keyData, valueData) = DatabaseHelper.GetDataInMetaFile(metaFilePath, "importAnimation");
            DatabaseHelper.EditDataInMetaFile(metaFilePath, "importAnimation", "0");
            */
            
        }
    }
}

