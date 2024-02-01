using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.AI.FSM;
using WPP.AI.GRID.STATE;
using WPP.AI.SPAWNER;
using System;

namespace WPP.AI.GRID
{
    public class GridController : MonoBehaviour
    {
        LandFormation _landFormation;
        public LandFormation LandFormation { get { return _landFormation; } }

        // 함수로 콜백 받아서 State 변환 진행
        public enum ControlState
        {
            Ready,
            Select,
            Plant
        }

        protected StateMachine<ControlState> _fsm;
        public StateMachine<ControlState> FSM { get { return _fsm; } }

        GridStorage _gridStorage;
        GridSelectComponent _selectComponent;
        public GridSelectComponent SelectComponent { get { return _selectComponent; } }

        GridFillComponent _fillComponent;
        public GridFillComponent FillComponent { get { return _fillComponent; } }

        public void Initialize(LandFormation landFormation)
        {
            _gridStorage = GetComponent<GridStorage>();
            if (_gridStorage == null) return;

            _gridStorage.Initialize();

            _selectComponent = GetComponent<GridSelectComponent>();
            if (_selectComponent == null) return;

            _selectComponent.Initialize(_gridStorage);

            _fillComponent = GetComponent<GridFillComponent>();
            if (_fillComponent == null) return;

            _fillComponent.Initialize(_gridStorage);

            _landFormation = landFormation;
            _fillComponent.OnLandFormationAssigned(_landFormation);

            InitializeFSM();
        }

        private void Update()
        {
            _fsm.OnUpdate();
        }

        public void OnSelect(OffsetRect offsetFromCenter) => _fsm.OnSelect(offsetFromCenter);

        public void OnCancelSelect() => _fsm.OnCancelSelect();

        public void OnPlant(int entityId, int playerId, int clientId, float duration) => _fsm.OnPlant(entityId, playerId, clientId, duration);

        public void OnPlant(int[] entityIds, int playerId, int clientId, Vector3[] offsets, float duration) => _fsm.OnPlant(entityIds, playerId, clientId, offsets, duration);

        public void OnTowerConditionChange(TowerCondition towerCondition)
        {
            _fillComponent.OnTowerConditionChange(towerCondition);
            _fsm.OnTowerConditionChange();
            // 여기에서 이벤트 날리기
        }

        void InitializeFSM()
        {
            Spawner spawner = FindObjectOfType<Spawner>();
            if (spawner == null) return;

            Dictionary<ControlState, BaseState> states = new Dictionary<ControlState, BaseState>()
            {
                {ControlState.Ready, new ReadyState(this)},
                {ControlState.Select, new SelectState(this)},
                {ControlState.Plant, new PlantState(this, spawner.Spawn, spawner.Spawn)}
            };

            _fsm = new StateMachine<ControlState>();
            _fsm.Initialize(states);
            _fsm.SetState(ControlState.Ready);
        }
    }
}
