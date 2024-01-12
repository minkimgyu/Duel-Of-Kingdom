using System.Collections.Generic;

namespace WPP.Battle.Fsm
{
    public enum FsmStep
    {
        Enter,
        Update,
        LateUpdate,
        FixedUpdate,
        OnCollisionEnter,
        OnTriggerEnter,
        OnTriggerStay,
        OnTriggerExit,
        Exit
    }
    public class Fsm<T> where T : System.Enum
    {

        public delegate void StateCallback(Fsm<T> fsm, FsmStep step);
        public event System.Action<T> OnStateTransition;

        private Dictionary<T, StateCallback> _states;
        private T _currentState;
        private T _previousState;

        public T CurrentState => _currentState;
        public T PreviousState => _previousState;

        public Fsm()
        {
            _states = new Dictionary<T, StateCallback>();
        }

        public void Add(T state, StateCallback callback)
        {
            _states.Add(state, callback);
        }

        public void SetInitialState(T state)
        {
            _currentState = state;
            _previousState = state;
        }

        public void OnFsmStep(FsmStep step)
        {
            _states[_currentState].Invoke(this, step);
        }

        public void TransitionTo(T state)
        {
            _states[_currentState]?.Invoke(this, FsmStep.Exit);

            _previousState = _currentState;
            _currentState = state;

            _states[_currentState].Invoke(this, FsmStep.Enter);

            OnStateTransition?.Invoke(_currentState);
        }
    }
}