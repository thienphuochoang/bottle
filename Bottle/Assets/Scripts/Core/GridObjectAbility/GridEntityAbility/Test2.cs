using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Bottle.Core.PathSystem;
public class Test2 : MonoBehaviour
{
    [OnInspectorGUI("AdjustColor")]
    public StringReference testStringReference = new StringReference();
    [ReadOnly]
    [SerializeField]
    private PathCreator testString;
    private void OnEnable()
    {
        AdjustColor();
    }
    private void AdjustColor()
    {
        if (testString == null)
            testString = default(PathCreator);

        if (testString != null)
            testString = this.testStringReference;
    }
    private void Start()
    {
        //Debug.Log(testString);
    }
}
