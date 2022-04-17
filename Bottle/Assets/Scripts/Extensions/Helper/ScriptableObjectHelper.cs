using UnityEngine;
using UnityEditor;
namespace Bottle.Extensions.Helper
{
    public class ScriptableObjectHelper
    {
        public static void CreateScriptableObject<T>(string assetName, string assetPath) where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();

            AssetDatabase.CreateAsset(asset, assetPath + assetName + ".asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
    }

}
