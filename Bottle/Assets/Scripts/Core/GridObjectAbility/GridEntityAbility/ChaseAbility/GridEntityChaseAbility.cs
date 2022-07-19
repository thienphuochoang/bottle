using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bottle.Core.GridObjectAbility;
using Bottle.Core.GridObjectData;
using Sirenix.OdinInspector;
using Bottle.Core.Manager;
public class GridEntityChaseAbility : GridEntityAbility
{
    [BoxGroup("Chase Ability Settings")]
    [Tooltip("The target Grid Entity which is also the final destination")]
    [SerializeField]
    private List<GridEntity> targetEntity = new List<GridEntity>();
    [SerializeField]
    private List<GridTile> finalPath = new List<GridTile>();
    public static Vector2Int[] eightDirections = new Vector2Int[]
    {
            new Vector2Int(0, 1),
            new Vector2Int(1, 1),
            new Vector2Int(1, 0),
            new Vector2Int(1, -1),
            new Vector2Int(0, -1),
            new Vector2Int(-1, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(-1, 1)
    };
    public override void AbilityOnAwake()
    {
        
    }

    public override void AbilityOnEnable()
    {
        
    }

    public override void AbilityStart()
    {
        
    }

    public override void AbilityUpdate()
    {

    }
}
