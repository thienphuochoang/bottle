using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bottle.Core.GridObjectData;
using Bottle.Core.Manager;
using Sirenix.OdinInspector;
namespace Bottle.Core.GridObjectAbility
{
    public class GridEntityPushAndDragPassiveAbility : GridObjectPassiveAbility<GridEntity>
    {
        private GridEntity _mainGridEntity;
        [ShowInInspector]
        [ReadOnly]
        private GridTile _nextTargetTile;
        [ShowInInspector]
        [ReadOnly]
        [BoxGroup("Acceleration Settings", true, true)]
        public float maximumSpeed = 4;
        [BoxGroup("Acceleration Settings", true, true)]
        public float acceleration = 5;
        [BoxGroup("Acceleration Settings", true, true)]
        public float currentSpeed;
        [BoxGroup("Acceleration Settings", true, true)]
        public float maximumSpeedMultiplier = 1f;
        [BoxGroup("Acceleration Settings", true, true)]
        public float accelerationMultiplier = 1f;

        protected override void Start()
        {
            base.Start();
            EventManager.Instance.StartListening("PushAndDragPassiveEvent", BeingPushedOrDragged);
        }

        private void BeingPushedOrDragged(Dictionary<string, object> message)
        {
            bool isTriggeredValue = (bool)message["isTriggered"];
            GridEntity currentInteractingObject = (GridEntity)message["interactingGridObject"];
            _mainGridEntity = (GridEntity)message["controllableMainGridEntity"];
            if (currentInteractingObject == _currentGridObject)
            {
                isPassiveAbilityTriggered = isTriggeredValue;
            }
        }

        protected override void Update()
        {
            base.Update();
            ApplyAcceleration();
            if (isPassiveAbilityTriggered)
            {
                isInProgress = true;
                GameplayManager.Instance.isTurnInProgress = true;
                isPassiveAbilityTriggered = false;
                _nextTargetTile = _mainGridEntity.currentStandingGridTile;
            }
            if (isInProgress == true && isPassiveAbilityTriggered == false && _mainGridEntity != null)
            {
                if (_mainGridEntity.currentStandingGridTile.gridPosition != _nextTargetTile.gridPosition || _mainGridEntity.currentStandingGridTile.gridHeight != _nextTargetTile.gridHeight)
                {
                    if (_mainGridEntity.GetComponent<GridEntityPushAndDragAbility>().currentInteractingGridObject != null)
                        FollowMainGridEntityStep();
                }
                
            }
        }

        private void FollowMainGridEntityStep()
        {
            Vector3 destination = new Vector3(_nextTargetTile.transform.position.x, _nextTargetTile.transform.position.y + 1, _nextTargetTile.transform.position.z);
            Vector3 newPosition = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * currentSpeed);
            if (this.transform.position != newPosition)
            {
                this.transform.position = newPosition;
                isInProgress = true;
                GameplayManager.Instance.isTurnInProgress = true;
            }
            else
            {
                var gridPos = GridManager.Instance.ConvertWorldPositionToGridPosition(this._currentGridObject);
                this._currentGridObject.gridPosition = new Vector2Int(gridPos.x, gridPos.z);
                this._currentGridObject.gridHeight = gridPos.y;
                //isInProgress = false;
                GameplayManager.Instance.isTurnInProgress = false;
                _nextTargetTile = _mainGridEntity.currentStandingGridTile;
                currentSpeed = 0;
            }
        }
        private void ApplyAcceleration()
        {
            if (currentSpeed < maximumSpeed * maximumSpeedMultiplier)
            {
                currentSpeed = currentSpeed + acceleration * accelerationMultiplier * Time.deltaTime;
                currentSpeed = Mathf.Clamp(currentSpeed, 0f, maximumSpeed * maximumSpeedMultiplier);
            }
        }
    }
}

