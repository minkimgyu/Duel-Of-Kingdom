using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.AI.FSM;
using WPP.AI.GRID.STATE;
using WPP.AI.SPAWNER;
using WPP.DeckManagement;
using System;

namespace WPP.AI.GRID
{
    public class GridController : MonoBehaviour
    {
        LandFormation _landFormation;
        public LandFormation LandFormation { get { return _landFormation; } }

        // �Լ��� �ݹ� �޾Ƽ� State ��ȯ ����
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
        public void OnSelect(float radius) => _fsm.OnSelect(radius);

        public void OnCancelSelect() => _fsm.OnCancelSelect();

        public void OnPlant(Card card, int level) => _fsm.OnPlant(card, level);

        public void OnTowerConditionChange(TowerCondition towerCondition)
        {
            _fillComponent.OnTowerConditionChange(towerCondition);
            _fsm.OnTowerConditionChange();
            // ���⿡�� �̺�Ʈ ������
        }

        void InitializeFSM()
        {
            Spawner spawner = FindObjectOfType<Spawner>();
            if (spawner == null) return;

            Dictionary<ControlState, BaseState> states = new Dictionary<ControlState, BaseState>()
            {
                {ControlState.Ready, new ReadyState(this)},
                {ControlState.Select, new SelectState(this)},
                {ControlState.Plant, new PlantState(this, spawner)}
            };

            _fsm = new StateMachine<ControlState>();
            _fsm.Initialize(states);
            _fsm.SetState(ControlState.Ready);
        }
    }
}
