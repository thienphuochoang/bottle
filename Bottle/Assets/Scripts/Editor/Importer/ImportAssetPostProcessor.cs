using UnityEngine;
using UnityEditor;
using System.Collections;

public class ImportAssetPostProcessor : AssetPostprocessor
{
    void OnPostprocessModel (GameObject g)
    {
        Debug.Log(assetPath);
        // Only operate on FBX files
        if (assetPath.IndexOf(".fbx") == -1)
        {
            Debug.Log(assetPath);
        }
    }
}
