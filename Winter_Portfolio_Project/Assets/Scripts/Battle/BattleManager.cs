using System;
using UnityEngine;
using WPP.AI;
using WPP.AI.GRID;
using WPP.AI.SPAWNER;
using WPP.Battle.Fsm;
using WPP.CAMERA;
using WPP.ClientInfo;
using WPP.DeckManagement;
using WPP.SOUND;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WPP.Battle
{
    public enum BattleResult
    {
        Win,
        Lose,
        Tie
    }

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
        public BattleTimer BattleTimer => _battleTimer;

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
        public float BattleRegenRate => _battleRegenRate;
        public float OvertimeRegenRate1 => _overtimeRegenRate1;
        public float OvertimeRegenRate2 => _overtimeRegenRate2;

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
        public event Action<BattleResult> OnGameOver; 

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
                SoundManager.PlayBGM("BGM4", true);

                OnStatusChange?.Invoke(fsm.CurrentState);
                _deckSystem.Init(DeckManager.CurrentDeck);

                _player.Init(true);
                _opponent.Init(false);

                LandFormation landFormation = ClientData.Instance().LandFormation;

                _gridController.Initialize(landFormation);
                _cameraController.Rotate(landFormation);
            }
            else if(step == FsmStep.Update)
            {
                // TODO : Remove this
                _fsm.TransitionTo(Status.Battle);
            }
        }

        private bool _isBattleLastMinute = false;
        private void Battle(Fsm<Status> fsm, FsmStep step)
        {
            if (step == FsmStep.Enter)
            {
                print("Battle Start");

                _battleTimer.StartTimer(_battleLength * 60);
                _battleTimer.OnTimerEnd += StopTime;
                _elixirSystem.SetElixirRegenTime(_battleRegenRate);
                _elixirSystem.StartRegen();

                _player.CrownSystem.OnCrownCountMax += OnBattleStatusEndCondition;
                _opponent.CrownSystem.OnCrownCountMax += OnBattleStatusEndCondition;
            }
            else if(step == FsmStep.Update)
            {
                if (!_isBattleLastMinute)
                {
                    if (_battleTimer.TimeLeft <= 60f)
                    {
                        _elixirSystem.SetElixirRegenTime(_overtimeRegenRate1);
                        _isBattleLastMinute = true;
                    }
                }
            }
            else if(step == FsmStep.Exit)
            {
                _battleTimer.PauseTimer();
                _battleTimer.OnTimerEnd -= StopTime;

                _player.CrownSystem.OnCrownCountMax -= OnBattleStatusEndCondition;
                _opponent.CrownSystem.OnCrownCountMax -= OnBattleStatusEndCondition;
            }
        }
        private void StopTime()
        {
            Time.timeScale = 0;
        }
        private void OnBattleStatusEndCondition()
        {
            if(_fsm.CurrentState == Status.Battle)
                _fsm.TransitionTo(Status.PostBattle);
        }

        private bool _isOvertimeSecondHalf = false;

        private void Overtime(Fsm<Status> fsm, FsmStep step)
        {
            if(step == FsmStep.Enter)
            {
                print("Overtime Start");

                if (CheckBattleEndCondition())
                {
                    _fsm.TransitionTo(Status.PostBattle);
                    return;
                }
                Time.timeScale = 1;
                _elixirSystem.SetElixirRegenTime(_overtimeRegenRate1);

                _battleTimer.StartTimer(_overtimeLength * 60);
                _battleTimer.OnTimerEnd += OnOverTimeStatusEndCondition;

                _player.CrownSystem.OnCrownCountChange += OnOverTimeStatusEndCondition;
                _opponent.CrownSystem.OnCrownCountChange += OnOverTimeStatusEndCondition;
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
                _battleTimer.OnTimerEnd -= OnOverTimeStatusEndCondition;

                _elixirSystem.StopRegen();

                _player.CrownSystem.OnCrownCountChange -= OnOverTimeStatusEndCondition;
                _opponent.CrownSystem.OnCrownCountChange -= OnOverTimeStatusEndCondition;
            }
        }
        private void OnOverTimeStatusEndCondition()
        {
            if (_fsm.CurrentState == Status.Overtime)
                _fsm.TransitionTo(Status.PostBattle);
        }
        private void OnOverTimeStatusEndCondition(int _) => OnOverTimeStatusEndCondition();

        private void Tiebreaker(Fsm<Status> fsm, FsmStep step)
        {
            throw new NotImplementedException();
        }

        private void PostBattle(Fsm<Status> fsm, FsmStep step)
        {
            if(step == FsmStep.Enter)
            {
                Time.timeScale = 0;

                BattleResult result;

                SoundManager.StopBGM();
                SoundManager.PlaySFX("Fanfare");

                if(_player.CrownSystem.CrownCount > _opponent.CrownSystem.CrownCount)
                {
                    Debug.Log("Player Win");
                    result = BattleResult.Win;
                }
                else if(_player.CrownSystem.CrownCount < _opponent.CrownSystem.CrownCount)
                {
                    Debug.Log("Opponent Win");
                    result = BattleResult.Lose;
                }
                else
                {
                    Debug.Log("Tie");
                    result = BattleResult.Tie;
                }

                OnGameOver?.Invoke(result);
            }
        }
    
        private bool CheckBattleEndCondition()
        {
            if (_player.CrownSystem.CrownCount != _opponent.CrownSystem.CrownCount)
            {
                return true;
            }

            return false;
        
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(BattleManager))]
    public class BattleManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!Application.isPlaying) return;
            var battleManager = (BattleManager)target;

            EditorGUILayout.Space();

            if (GUILayout.Button("Start Overtime"))
            {
                battleManager.StartOverTime();
            }

            EditorGUILayout.LabelField("Player Towers", EditorStyles.boldLabel);

            if (GUILayout.Button("Destroy Left Princess Tower"))
            {
                battleManager.Player.TowerSystem.DestroyLeftPrincessTower();
            }
            if(GUILayout.Button("Destroy Right Princess Tower"))
            {
                battleManager.Player.TowerSystem.DestroyRightPrincessTower();
            }
            if (GUILayout.Button("Destroy King Tower"))
            {
                battleManager.Player.TowerSystem.DestroyKingTower();
            }

            EditorGUILayout.LabelField("Opponent Towers", EditorStyles.boldLabel);

            if (GUILayout.Button("Destroy Left Princess Tower"))
            {
                battleManager.Opponent.TowerSystem.DestroyLeftPrincessTower();
            }
            if (GUILayout.Button("Destroy Right Princess Tower"))
            {
                battleManager.Opponent.TowerSystem.DestroyRightPrincessTower();
            }
            if (GUILayout.Button("Destroy King Tower"))
            {
                battleManager.Opponent.TowerSystem.DestroyKingTower();
            }
        }
    }
#endif
}