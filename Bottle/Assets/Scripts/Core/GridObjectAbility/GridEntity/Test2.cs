using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class Test2 : MonoBehaviour
{
    [OnInspectorGUI("AdjustColor")]
    public StringReference testStringReference = new StringReference();
    [ReadOnly]
    [SerializeField]
    private string testString;
    private void OnEnable()
    {
        AdjustColor();
    }
    private void AdjustColor()
    {
        if (testString == "")
            testString = this.gameObject.name;

        if (testString != "")
            testString = this.testStringReference;
    }
    private void Start()
    {
        Debug.Log(testString);
    }
}
