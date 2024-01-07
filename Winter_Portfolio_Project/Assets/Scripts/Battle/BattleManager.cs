using System;
using UnityEngine;
using WPP.Battle.Fsm;

namespace WPP.Battle
{
    public class BattleManager : MonoBehaviour
    {
        [Header("Battle Timer")]
        [SerializeField] private BattleTimer _battleTimer;
        [SerializeField] private float _battleLength = 3f;
        [SerializeField] private float _overtimeLength = 2f;

        public enum Status
        {
            PreBattle,
            Battle,
            Overtime,
            Tiebreaker,
            PostBattle
        }

        public event Action<Status> OnStatusChange;

        private Status _status;
        public Status CurrentStatus => _status;

        private Fsm<Status> _fsm;

        private void Awake()
        {
            _fsm = new Fsm<Status>();
            _fsm.OnStateTransition += (status) => { OnStatusChange?.Invoke(status); };

            _fsm.Add(Status.PreBattle, PreBattle);
            _fsm.Add(Status.Battle, Battle);
            _fsm.Add(Status.Overtime, Overtime);
            _fsm.Add(Status.Tiebreaker, Tiebreaker);
            _fsm.Add(Status.PostBattle, PostBattle);
            _fsm.SetInitialState(Status.PreBattle);
        }

        private void Start()
        {
            _fsm.OnFsmStep(FsmStep.Enter);
        }

        public void StartBattle()
        {
            if(_status == Status.PreBattle)
                _fsm.TransitionTo(Status.Battle);
        }

        private void PreBattle(Fsm<Status> fsm, FsmStep step)
        {
            if(step == FsmStep.Enter)
            {
                OnStatusChange?.Invoke(Status.PreBattle);
            }
        }

        private void Battle(Fsm<Status> fsm, FsmStep step)
        {
            if (step == FsmStep.Enter)
            {
                _battleTimer.StartTimer(_battleLength * 60);
                _battleTimer.OnTimerEnd += TransitionToOvertime;
            }
            else if(step == FsmStep.Exit)
            {
                _battleTimer.PauseTimer();
                _battleTimer.OnTimerEnd -= TransitionToOvertime;
            }

            void TransitionToOvertime()
            {
                _fsm.TransitionTo(Status.Overtime);
            }
        }

        private void Overtime(Fsm<Status> fsm, FsmStep step)
        {
            if(step == FsmStep.Enter)
            {
                _battleTimer.StartTimer(_overtimeLength * 60);
                _battleTimer.OnTimerEnd += TransitionToTiebreaker;
            }
            else if (step == FsmStep.Exit)
            {
                _battleTimer.PauseTimer();
                _battleTimer.OnTimerEnd -= TransitionToTiebreaker;
            }

            void TransitionToTiebreaker()
            {
                _fsm.TransitionTo(Status.Tiebreaker);
            }
        }

        private void Tiebreaker(Fsm<Status> fsm, FsmStep step)
        {
            throw new NotImplementedException();
        }

        private void PostBattle(Fsm<Status> fsm, FsmStep step)
        {
            throw new NotImplementedException();
        }
    }
}