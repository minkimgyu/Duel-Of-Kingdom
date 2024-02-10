using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.AI.GRID;
using WPP.DeckManagement;

namespace WPP.AI.FSM
{
    abstract public class State : BaseState
    {
        public override void OnTowerConditionChangeRequested() { }

        public override void OnPlantRequested(Card card, int level) { }
        public override void OnCancelSelectRequested() { }
        public override void OnSelectRequested(OffsetRect offsetFromCenter) { }


        public override void OnActiveRequested() { }
        public override void OnAttackRequested() { }

        public override void OnCancelAttackRequested() { }


        public override void OnMessageRequested(string info) { }
        public override void OnMessageRequested(string info, OffsetRect offsetFromCenter) { }
        public override void OnMessageRequested(string info, Card card, int level, Vector3 pos) { }

        public override void CheckStateChange() { }
        public override void OnStateEnter() { }
        public override void OnStateUpdate() { }
        public override void OnStateExit() { }
    }

    abstract public class BaseState
    {
        public abstract void OnTowerConditionChangeRequested();
        public abstract void OnPlantRequested(Card card, int level);
        public abstract void OnCancelSelectRequested();
        public abstract void OnSelectRequested(OffsetRect offsetFromCenter);

        public abstract void OnActiveRequested();
        public abstract void OnAttackRequested();

        public abstract void OnCancelAttackRequested();


        public abstract void OnMessageRequested(string info);
        public abstract void OnMessageRequested(string info, OffsetRect offsetFromCenter);
        public abstract void OnMessageRequested(string info, Card card, int level, Vector3 pos);

        public abstract void CheckStateChange();
        public abstract void OnStateEnter();
        public abstract void OnStateUpdate();
        public abstract void OnStateExit();
    }

    public class StateMachine<T>
    {
        Dictionary<T, BaseState> _stateDictionary = new Dictionary<T, BaseState>();

        //현재 상태를 담는 프로퍼티.
        BaseState _currentState;
        BaseState _previousState;

        public void Initialize(Dictionary<T, BaseState> stateDictionary)
        {
            _currentState = null;
            _previousState = null;

            _stateDictionary = stateDictionary;
        }

        public void OnUpdate()
        {
            if (_currentState == null) return;
            _currentState.OnStateUpdate();
            _currentState.CheckStateChange();
        }

        public void OnAttack()
        {
            if (_currentState == null) return;
            _currentState.OnAttackRequested();
        }

        public void OnActive()
        {
            if (_currentState == null) return;
            _currentState.OnActiveRequested();
        }

        public void OnTowerConditionChange()
        {
            if (_currentState == null) return;
            _currentState.OnTowerConditionChangeRequested();
        }

        public void OnCancelAttack()
        {
            if (_currentState == null) return;
            _currentState.OnCancelAttackRequested();
        }

        public void OnSelect(OffsetRect offsetFromCenter)
        {
            if (_currentState == null) return;
            _currentState.OnSelectRequested(offsetFromCenter);
        }

        public void OnCancelSelect()
        {
            if (_currentState == null) return;
            _currentState.OnCancelSelectRequested();
        }

        public void OnPlant(Card card, int level)
        {
            if (_currentState == null) return;
            _currentState.OnPlantRequested(card, level);
        }

        public bool RevertToPreviousState()
        {
            return ChangeState(_previousState);
        }

        #region SetState

        public bool SetState(T stateName)
        {
            return ChangeState(_stateDictionary[stateName]);
        }

        public bool SetState(T stateName, string info)
        {
            return ChangeState(_stateDictionary[stateName], info);
        }

        public bool SetState(T stateName, string info, OffsetRect offsetFromCenter)
        {
            return ChangeState(_stateDictionary[stateName], info, offsetFromCenter);
        }

        public bool SetState(T stateName, string info, Card card, int level, Vector3 pos)
        {
            return ChangeState(_stateDictionary[stateName], info, card, level, pos);
        }
        #endregion


        #region ChangeState

        bool ChangeState(BaseState state)
        {
            if (_stateDictionary.ContainsValue(state) == false) return false;

            if (_currentState == state) // 같은 State로 전환하지 못하게 막기
            {
                return false;
            }

            if (_currentState != null) //상태가 바뀌기 전에, 이전 상태의 Exit를 호출
                _currentState.OnStateExit();

            _previousState = _currentState;

            _currentState = state;


            if (_currentState != null) //새 상태의 Enter를 호출한다.
            {
                _currentState.OnStateEnter();
            }

            return true;
        }

        bool ChangeState(BaseState state, string info)
        {
            if (_stateDictionary.ContainsValue(state) == false) return false;

            if (_currentState == state) // 같은 State로 전환하지 못하게 막기
            {
                return false;
            }

            if (_currentState != null) //상태가 바뀌기 전에, 이전 상태의 Exit를 호출
                _currentState.OnStateExit();

            _previousState = _currentState;

            _currentState = state;
            _currentState.OnMessageRequested(info);

            if (_currentState != null) //새 상태의 Enter를 호출한다.
            {
                _currentState.OnStateEnter();
            }

            return true;
        }

        bool ChangeState(BaseState state, string info, OffsetRect offsetFromCenter)
        {
            if (_stateDictionary.ContainsValue(state) == false) return false;

            if (_currentState == state) // 같은 State로 전환하지 못하게 막기
            {
                return false;
            }

            if (_currentState != null) //상태가 바뀌기 전에, 이전 상태의 Exit를 호출
                _currentState.OnStateExit();

            _previousState = _currentState;

            _currentState = state;
            _currentState.OnMessageRequested(info, offsetFromCenter);

            if (_currentState != null) //새 상태의 Enter를 호출한다.
            {
                _currentState.OnStateEnter();
            }

            return true;
        }

        bool ChangeState(BaseState state, string info, Card card, int level, Vector3 pos)
        {
            if (_stateDictionary.ContainsValue(state) == false) return false;

            if (_currentState == state) // 같은 State로 전환하지 못하게 막기
            {
                return false;
            }

            if (_currentState != null) //상태가 바뀌기 전에, 이전 상태의 Exit를 호출
                _currentState.OnStateExit();

            _previousState = _currentState;

            _currentState = state;
            _currentState.OnMessageRequested(info, card, level, pos);

            if (_currentState != null) //새 상태의 Enter를 호출한다.
            {
                _currentState.OnStateEnter();
            }

            return true;
        }

        #endregion
    }
}