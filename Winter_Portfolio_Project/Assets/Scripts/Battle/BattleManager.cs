using System;
using UnityEngine;
using WPP.AI;
using WPP.AI.GRID;
using WPP.AI.SPAWNER;
using WPP.Battle.Fsm;
using WPP.CAMERA;
using WPP.ClientInfo;
using WPP.DeckManagement;

namespace WPP.Battle
{
    [DefaultExecutionOrder(-100)]
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

        [Header("Deck & Elixir")]
        [SerializeField] private DeckSystem _deckSystem;
        [SerializeField] private ElixirSystem _elixirSystem;
        public DeckSystem DeckSystem => _deckSystem;
        public ElixirSystem ElixirSystem => _elixirSystem;

        [Header("Player")]
        [SerializeField] private BattlePlayer _player;
        [SerializeField] private BattlePlayer _opponent;
        public BattlePlayer Player => _player;
        public BattlePlayer Opponent => _opponent;
        public BattlePlayer GetPlayerOfEntity(Entity entity)
        {
            int playerID = ClientData.Instance().player_id_in_game;

            return (entity.OwnershipId == playerID) ? _player : _opponent;
        }

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

        public static BattleManager Instance() => _instance;

        public event Action<Status> OnStatusChange;

        private Fsm<Status> _fsm;
        public Status CurrentStatus => _fsm.CurrentState;

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
            if(_fsm.CurrentState == Status.PreBattle)
                _fsm.TransitionTo(Status.Battle);
        }

        public void StartOverTime()
        {
            if(_fsm.CurrentState == Status.Battle)
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
                OnStatusChange?.Invoke(fsm.CurrentState);
                _deckSystem.Init(DeckManager.CurrentDeck);

                _player.Init();
                _opponent.Init();

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
                //_battleTimer.OnTimerEnd += TransitionToOvertime;
                _elixirSystem.SetElixirRegenTime(_battleRegenRate);
                _elixirSystem.StartRegen();

                _player.CrownSystem.OnCrownCountMax += TransitionToPostBattle;
                _opponent.CrownSystem.OnCrownCountMax += TransitionToPostBattle;
            }
            else if(step == FsmStep.Exit)
            {
                _battleTimer.PauseTimer();
                //_battleTimer.OnTimerEnd -= TransitionToOvertime;

                _player.CrownSystem.OnCrownCountMax -= TransitionToPostBattle;
                _opponent.CrownSystem.OnCrownCountMax -= TransitionToPostBattle;
            }
        }
        private void TransitionToPostBattle()
        {
            if(_fsm.CurrentState == Status.Battle)
                _fsm.TransitionTo(Status.PostBattle);
        }
        /*
        private void TransitionToOvertime()
        {
            _fsm.TransitionTo(Status.Overtime);
        }
        */

        private bool _isOvertimeSecondHalf = false;

        private void Overtime(Fsm<Status> fsm, FsmStep step)
        {
            if(step == FsmStep.Enter)
            {
                _battleTimer.StartTimer(_overtimeLength * 60);
                _battleTimer.OnTimerEnd += TransitionToTiebreaker;

                _elixirSystem.SetElixirRegenTime(_overtimeRegenRate1);
            }
            else if(step == FsmStep.Update)
            {
                if (!_isOvertimeSecondHalf)
                {
                    if (_battleTimer.TimeLeft <= _overtimeLength * 60f * 0.5f)
                    {
                        _elixirSystem.SetElixirRegenTime(_overtimeRegenRate2);
                        _isOvertimeSecondHalf = true;
                    }
                }
            }
            else if (step == FsmStep.Exit)
            {
                _battleTimer.PauseTimer();
                _battleTimer.OnTimerEnd -= TransitionToTiebreaker;

                _elixirSystem.StopRegen();
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
            if(step == FsmStep.Enter)
            {
                if(_player.CrownSystem.CrownCount > _opponent.CrownSystem.CrownCount)
                {
                    Debug.Log("Player Win");
                }
                else if(_player.CrownSystem.CrownCount < _opponent.CrownSystem.CrownCount)
                {
                    Debug.Log("Opponent Win");
                }
                else
                {
                    Debug.Log("Tie");
                }
            }
        }
    }
}