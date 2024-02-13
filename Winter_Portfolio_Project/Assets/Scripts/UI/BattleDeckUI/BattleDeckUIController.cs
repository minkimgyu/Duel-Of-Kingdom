using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WPP.DeckManagement;
using WPP.DeckManagement.UI;
using WPP.AI.GRID;
using WPP.AI.SPAWNER;
using WPP.Battle.Fsm;
using WPP.Collection;
using WPP.ClientInfo.Card;
using System;

namespace WPP.Battle.UI
{
    public class BattleDeckUIController : MonoBehaviour
    {
        [SerializeField] private BattleManager _battleManager;
        [Header("SpawnSystem")]
        [SerializeField] private GridController _gridController;
        [SerializeField] private Spawner _spawner;
        [Space]
        [SerializeField] private RectTransform _cardSection;

        [Header("Card")]
        [SerializeField] private CardUI[] _cards;

        [SerializeField] private TextMeshProUGUI _next;
        [SerializeField] private TextMeshProUGUI _nextElixir;
        [SerializeField] private TextMeshProUGUI _cooldown;
        [Space]
        [SerializeField] private float _selectedCardOffset = 50f;
        [SerializeField] private float _selectedCardScale = 1.2f;
        [Header("Elixir")]
        [SerializeField] private Slider _elixirSlider;
        [SerializeField] private TextMeshProUGUI _elixirText;

        private BattlePlayer _player;
        private enum State
        {
            Idle,
            Selecting,
            Placing
        }
        private Fsm<State> _fsm;

        private void Awake()
        {
            _fsm = new Fsm<State>();
            _fsm.Add(State.Idle, OnIdle);
            _fsm.Add(State.Selecting, OnSelecting);
            _fsm.Add(State.Placing, OnPlacing);

            _fsm.SetInitialState(State.Idle);
        }

        private void OnEnable() {
            _player = _battleManager.Player;

            _battleManager.OnStatusChange += OnStatusChange;

            _player.Deck.OnHandChange += OnCardDrawn;
            _player.Elixir.OnElixirCountChange += SetElixirBar;
        }

        private void OnDisable()
        {
            _battleManager.OnStatusChange -= OnStatusChange;
        }

        private void OnStatusChange(BattleManager.Status status)
        {
            Debug.Log("BattleDeckUIController OnStatusChange : " + status);
            if(status == BattleManager.Status.Loading)
            {
                Init();
            }
        }

        public void Init()
        {
            _fsm.OnFsmStep(FsmStep.Enter);
            Debug.Log("BattleDeckUIController Init");
        }
        private void Update()
        {
            _fsm.OnFsmStep(FsmStep.Update);

            if (_player.Deck.LeftCooldown > 0f)
                _cooldown.text = _player.Deck.LeftCooldown.ToString("F1");
            else _cooldown.text = "";
        }

        private void OnIdle(Fsm<State> fsm, FsmStep step)
        {
            if(step == FsmStep.Enter)
            {
                _selectedCardIndex = -1;
                UpdateCardTransform();
            }
            else if(step == FsmStep.Update)
            {
            }
            else if (step == FsmStep.Exit)
            {
            }
        }
        private int _selectedCardIndex = -1;
        public void SelectCard(int index)
        {
            if(_fsm.CurrentState == State.Idle || _fsm.CurrentState == State.Selecting)
            {
                if (_selectedCardIndex != index)
                {
                    _selectedCardIndex = index;
                    _fsm.TransitionTo(State.Selecting);
                    return;
                }
                else
                {
                    _fsm.TransitionTo(State.Idle);
                    return;
                }
            }
        }

        private void OnSelecting(Fsm<State> fsm, FsmStep step)
        {
            if (step == FsmStep.Enter)
            {
                UpdateCardTransform();
            }
            else if (step == FsmStep.Update)
            {
                Vector2 localMousePosition = _cardSection.InverseTransformPoint(Input.mousePosition);
                if (!_cardSection.rect.Contains(localMousePosition))
                {
                    fsm.TransitionTo(State.Placing);
                    return;
                }
            }
            else if (step == FsmStep.Exit)
            {
            }
        }

        private bool _placedCard = false;
        //private CardData _selectedCardData;
        private void OnPlacing(Fsm<State> fsm, FsmStep step)
        {
            if (step == FsmStep.Enter)
            {
                _cards[_selectedCardIndex].gameObject.SetActive(false);
                _player.Deck.OnCardUsed += OnCardUsed;

                Card card = _player.Deck.Hand[_selectedCardIndex];
                int level = _player.Deck.GetCardLevel(_selectedCardIndex);
                Debug.Log("Placing Card name : " + name + ", lv : " + level);
                CardData selectedCardData = CardCollection.Instance().FindCard(card.id, level);

                OffsetRect offsetRect 
                    = new OffsetRect(selectedCardData.gridSize.top, selectedCardData.gridSize.down, 
                    selectedCardData.gridSize.left, selectedCardData.gridSize.right);
                _gridController.FSM.OnSelect(offsetRect);

                _placedCard = false;
            }
            else if (step == FsmStep.Update)
            {
                Vector2 localMousePosition = _cardSection.InverseTransformPoint(Input.mousePosition);
                if (_cardSection.rect.Contains(localMousePosition))
                {
                    fsm.TransitionTo(State.Selecting);
                    return;
                }
                if(Input.GetMouseButtonDown(1)) 
                { 
                    fsm.TransitionTo(State.Idle);
                    return;
                }

                if (Input.GetMouseButtonDown(0))
                {
                    if (_player.Deck.UseCard(_selectedCardIndex))
                    {
                        return;
                    }
                }
            }
            else if (step == FsmStep.Exit)
            {
                if(_placedCard)
                {
                    _cards[_selectedCardIndex].gameObject.SetActive(false);
                }
                else
                {
                    _cards[_selectedCardIndex].gameObject.SetActive(true);
                }

                _player.Deck.OnCardUsed -= OnCardUsed;

                _gridController.FSM.OnCancelSelect();
            }
        }

        void OnCardUsed(Card card, int level)
        {
            _placedCard = true;
            //Debug.Log("Plant id : " + _selectedCardData.id);

            // temp id
            //int id = UnityEngine.Random.Range(0, 8);
            //_gridController.FSM.OnPlant(card);
            _gridController.OnPlant(card, level);

            _fsm.TransitionTo(State.Idle);
        }

        private void OnCardDrawn()
        {
            var hand = _player.Deck.Hand;

            for (int i = 0; i < _cards.Length; i++)
            {
                if (hand[i].IsEmpty())
                {
                    _cards[i].gameObject.SetActive(false);
                }
                else
                {
                    if (_selectedCardIndex == i && _fsm.CurrentState == State.Placing) {}
                    else
                    {
                        _cards[i].gameObject.SetActive(true);
                    }

                    _cards[i].SetCard(hand[i]);
                }
            }

            _next.text = _player.Deck.Next.id;
            _nextElixir.text = _player.Deck.Next.cost.ToString();
        }

        private void UpdateCardTransform()
        {
            for(int i = 0; i < _cards.Length; ++i)
            {
                var transform = _cards[i].GetComponent<RectTransform>();
                if (i == _selectedCardIndex)
                {
                    transform.anchoredPosition = Vector2.up * _selectedCardOffset;
                    transform.localScale = Vector3.one * _selectedCardScale;
                }
                else
                {
                    transform.anchoredPosition = Vector2.zero;
                    transform.localScale = Vector3.one;
                }
            }
        }
        
        private void SetElixirBar(int count)
        {
            _elixirSlider.value = count;
            _elixirText.text = count.ToString();
        }


    }
}
