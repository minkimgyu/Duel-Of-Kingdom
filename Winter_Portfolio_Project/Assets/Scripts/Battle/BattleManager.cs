using System;
using UnityEngine;
using WPP.AI.GRID;
using WPP.AI.SPAWNER;
using WPP.Battle.Fsm;
using WPP.CAMERA;
using WPP.ClientInfo;

namespace WPP.Battle
{
    public class BattleManager : MonoBehaviour
    {
        [SerializeField] bool _isTest = false;
        [Header("AI")]
        [SerializeField] Spawner _spawner;
        [SerializeField] GridController _gridController;
        [SerializeField] CameraController _cameraController;

        [Header("Battle Timer")]
        [SerializeField] private BattleTimer _battleTimer;
        [SerializeField] private float _battleLength = 3f;
        [SerializeField] private float _overtimeLength = 2f;

        [Header("Player")]
        [SerializeField] private BattlePlayer _player;
        public BattlePlayer Player => _player;

        [Header("CrownSystem")]
        [SerializeField] private CrownSystem _playerCrown;
        [SerializeField] private CrownSystem _opponentCrown;
        public CrownSystem PlayerCrown => _playerCrown;
        public CrownSystem OpponentCrown => _opponentCrown;


        [Header("Elixir System")]
        [SerializeField] private float _battleRegenRate = 2.8f;
        [SerializeField] private float _overtimeRegenRate1 = 1.4f;
        [SerializeField] private float _overtimeRegenRate2 = 0.9f;

        public enum Status
        {
            PreBattle,
            Battle,
            Overtime,
            Tiebreaker,
            PostBattle
        }

        private static BattleManager _instance;

        public static BattleManager Instance()
        {
            if(_instance == null )
            {
                return null;
            }
            return _instance;
        }

        public event Action<Status> OnStatusChange;

        private Status _status;
        public Status CurrentStatus => _status;

        private Fsm<Status> _fsm;

        private void Awake()
        {
            _instance = this;
            _fsm = new Fsm<Status>();
            _fsm.OnStateTransition += (status) => { OnStatusChange?.Invoke(status); };

            _fsm.Add(Status.PreBattle, PreBattle);
            _fsm.Add(Status.Battle, Battle);
            _fsm.Add(Status.Overtime, Overtime);
            _fsm.Add(Status.Tiebreaker, Tiebreaker);
            _fsm.Add(Status.PostBattle, PostBattle);
            _fsm.SetInitialState(Status.PreBattle);
        }

        // TODO : use this
        private void Start()
        {
            if(!_isTest) _fsm.OnFsmStep(FsmStep.Enter);
        }
        
        // for test purpose
        public void StartLoad()
        {
            if(_isTest) _fsm.OnFsmStep(FsmStep.Enter);
        }

        private void Update()
        {
            _fsm.OnFsmStep(FsmStep.Update);
        }

        public void StartBattle()
        {
            if(_status == Status.PreBattle)
                _fsm.TransitionTo(Status.Battle);
        }

        public void StartOverTime()
        {
             _fsm.TransitionTo(Status.Overtime);
        }

        public void StartTiebreak()
        {
            _fsm.TransitionTo(Status.Tiebreaker);
        }

        private void PreBattle(Fsm<Status> fsm, FsmStep step)
        {
            if(step == FsmStep.Enter)
            {
                // Ready to Battle

                OnStatusChange?.Invoke(fsm.CurrentState);
                _player.Init();

                LandFormation landFormation = ClientData.Instance().LandFormation;

                Debug.Log("LandFormation : " + landFormation);
                _gridController.Initialize(landFormation);
                _cameraController.Rotate(landFormation);

            }
            else if(step == FsmStep.Update)
            {
                // TODO : Remove this
                _fsm.TransitionTo(Status.Battle);
            }
        }

        private void Battle(Fsm<Status> fsm, FsmStep step)
        {
            if (step == FsmStep.Enter)
            {
                _battleTimer.StartTimer(_battleLength * 60);
                _battleTimer.OnTimerEnd += TransitionToOvertime;

                _player.Elixir.SetElixirRegenTime(_battleRegenRate);
                _player.Elixir.StartRegen();
            }
            else if(step == FsmStep.Exit)
            {
                _battleTimer.PauseTimer();
                _battleTimer.OnTimerEnd -= TransitionToOvertime;
            }
        }
        private void TransitionToOvertime()
        {
            _fsm.TransitionTo(Status.Overtime);
        }


        private bool _isOvertimeSecondHalf = false;

        private void Overtime(Fsm<Status> fsm, FsmStep step)
        {
            if(step == FsmStep.Enter)
            {
                _battleTimer.StartTimer(_overtimeLength * 60);
                _battleTimer.OnTimerEnd += TransitionToTiebreaker;

                _player.Elixir.SetElixirRegenTime(_overtimeRegenRate1);
            }
            else if(step == FsmStep.Update)
            {
                if (!_isOvertimeSecondHalf)
                {
                    if (_battleTimer.TimeLeft <= _overtimeLength * 60f * 0.5f)
                    {
                        _player.Elixir.SetElixirRegenTime(_overtimeRegenRate2);
                        _isOvertimeSecondHalf = true;
                    }
                }
            }
            else if (step == FsmStep.Exit)
            {
                _battleTimer.PauseTimer();
                _battleTimer.OnTimerEnd -= TransitionToTiebreaker;

                _player.Elixir.StopRegen();
            }
        }
        void TransitionToTiebreaker()
        {
            _fsm.TransitionTo(Status.Tiebreaker);
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