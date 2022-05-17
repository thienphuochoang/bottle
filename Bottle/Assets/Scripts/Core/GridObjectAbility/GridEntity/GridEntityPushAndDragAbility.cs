using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bottle.Core.GridObjectData;
namespace Bottle.Core.GridObjectAbility
{
    public class GridEntityPushAndDragAbility : GridEntityInteractAbility
    {
        protected override void Update()
        {
            base.Update();
            if (currentInteractingGridObject != null)
            {
                var ahihi = GetTheOppositeMovementDirection(_currentGridObject.currentFacingDirection);
            }
        }
        private GridEntity.FacingDirections GetTheOppositeMovementDirection(GridEntity.FacingDirections currentFacingDirection)
        {
            GridEntity.FacingDirections oppositeFacingDirection = GridEntity.FacingDirections.NONE;
            switch (currentFacingDirection)
            {
                case GridEntity.FacingDirections.PositiveZ:
                    oppositeFacingDirection = GridEntity.FacingDirections.NegativeZ;
                    break;
                //case GridEntity.FacingDirections.NegativeZ:
            }
            return oppositeFacingDirection;
        }
    }

}
