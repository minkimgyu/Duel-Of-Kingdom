using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WPP.DeckManagement;
using WPP.DeckManagement.UI;
using WPP.AI.GRID;
using WPP.AI.SPAWNER;
using WPP.Battle.Fsm;

namespace WPP.Battle.UI
{
    public class BattleDeckUIController : MonoBehaviour
    {
        [Header("DeckSystem")]
        [SerializeField] private DeckSystem _deckSystem;
        [SerializeField] private ElixirSystem _elixirSystem;
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

        // Test Purpose
        private void Start()
        {
            Deck deck = new Deck();
            for (int i = 0; i < 8; i++)
            {
                deck.SetCard(i, "card_" + i.ToString());
            }
            _deckSystem.Init(deck);
            _elixirSystem.StartRegen();

            _fsm.OnFsmStep(FsmStep.Enter);
        }
        private void Update()
        {
            _fsm.OnFsmStep(FsmStep.Update);

            if (_deckSystem.LeftCooldown > 0f)
                _cooldown.text = _deckSystem.LeftCooldown.ToString("F1");
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

        bool _placedCard = false;
        private void OnPlacing(Fsm<State> fsm, FsmStep step)
        {
            if (step == FsmStep.Enter)
            {
                _cards[_selectedCardIndex].gameObject.SetActive(false);
                _deckSystem.OnCardUsed += OnCardUsed;

                OffsetRect offsetRect1 = new OffsetRect(0, 0, 0, 0);
                _gridController.FSM.OnSelect(offsetRect1);

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

                if (Input.GetMouseButtonDown(0))
                {
                    if (_deckSystem.UseCard(_selectedCardIndex))
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

                _deckSystem.OnCardUsed -= OnCardUsed;

                _gridController.FSM.OnCancelSelect();
            }

            void OnCardUsed(Card card)
            {
                int id = 1;
                float duration = 3f;

                _placedCard = true;
                _gridController.FSM.OnPlant(id, 2, 2, duration);

                fsm.TransitionTo(State.Idle);
            }
        }
        private void OnEnable()
        {
            _deckSystem.OnHandChange += OnCardDrawn;
            _elixirSystem.OnElixirCountChange += SetElixirBar;
        }

        private void OnCardDrawn()
        {
            var hand = _deckSystem.Hand;

            for (int i = 0; i < _cards.Length; i++)
            {
                if (hand[i].id == null)
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

                    _cards[i].SetCard(hand[i].id, hand[i].cost);
                }
            }

            _next.text = _deckSystem.Next.id;
            _nextElixir.text = _deckSystem.Next.cost.ToString();
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
