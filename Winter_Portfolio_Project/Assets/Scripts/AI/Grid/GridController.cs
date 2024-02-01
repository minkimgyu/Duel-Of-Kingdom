using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.AI.FSM;
using WPP.AI.GRID.STATE;

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
        SelectComponent _selectComponent;
        public SelectComponent SelectComponent { get { return _selectComponent; } }

        FillComponent _fillComponent;
        public FillComponent FillComponent { get { return _fillComponent; } }

        private void Start()
        {
            _gridStorage = GetComponent<GridStorage>();
            _gridStorage.Initialize();

            _selectComponent = GetComponent<SelectComponent>();
            _selectComponent.Initialize(_gridStorage);

            _fillComponent = GetComponent<FillComponent>();
            _fillComponent.Initialize(_gridStorage);
        }

        public void Initialize(LandFormation landFormation)
        {
            _landFormation = landFormation;
            _fillComponent.OnLandFormationAssigned(_landFormation);

            InitializeFSM();
        }

        void InitializeFSM()
        {
            Dictionary<ControlState, BaseState> states = new Dictionary<ControlState, BaseState>()
            {
                {ControlState.Ready, new ReadyState(this)},
                {ControlState.Select, new SelectState(this)},
                {ControlState.Plant, new PlantState(this)}
            };

            _fsm.Initialize(states);
            _fsm.SetState(ControlState.Ready);
        }
    }
}
