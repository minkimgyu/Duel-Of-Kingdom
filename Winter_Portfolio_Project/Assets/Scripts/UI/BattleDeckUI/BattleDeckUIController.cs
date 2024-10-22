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
using WPP.SOUND;

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

        //[SerializeField] private TextMeshProUGUI _next;
        //[SerializeField] private TextMeshProUGUI _nextElixir;
        [SerializeField] private CardUI _nextCard;
        [SerializeField] private TextMeshProUGUI _cooldown;
        [Space]
        [SerializeField] private float _selectedCardOffset = 50f;
        [SerializeField] private float _selectedCardScale = 1.2f;
        [Header("Elixir")]
        [SerializeField] private RectTransform _elixirFillArea;
        [SerializeField] private Slider _elixirSlider;
        [SerializeField] private Slider _usingElixir;
        [SerializeField] private Slider _warningElixir;
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

        private void OnEnable() {
            _battleManager.OnStatusChange += OnStatusChange;

            _battleManager.DeckSystem.OnHandChange += OnCardDrawn;
            _battleManager.ElixirSystem.OnElixirCountChange += SetElixirBar;
        }

        private void OnDisable()
        {
            _battleManager.OnStatusChange -= OnStatusChange;
            Time.timeScale = 1f;
        }

        private void OnStatusChange(BattleManager.Status status)
        {
            if(status == BattleManager.Status.PreBattle)
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

            if (_battleManager.DeckSystem.LeftCooldown > 0f)
            {
                _nextCard.SetInteractable(false);
                _cooldown.text = _battleManager.DeckSystem.LeftCooldown.ToString("F1");
            }
            else
            {
                _nextCard.SetInteractable(true);
                _cooldown.text = "";
            }
        }

        private void OnIdle(Fsm<State> fsm, FsmStep step)
        {
            if(step == FsmStep.Enter)
            {
                SoundManager.PlaySFX("CardDrop");

                _selectedCardIndex = -1;
                UpdateCardTransform();
                HideUsingElixirBar();
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
                SoundManager.PlaySFX("CardDraw");

                UpdateCardTransform();
                ShowUsingElixirBar(_battleManager.DeckSystem.Hand[_selectedCardIndex]);

            }
            else if (step == FsmStep.Update)
            {
                Vector2 localMousePosition = _cardSection.InverseTransformPoint(Input.mousePosition);
                if (!_cardSection.rect.Contains(localMousePosition))
                {
                    SoundManager.PlaySFX("CardUp");

                    fsm.TransitionTo(State.Placing);
                    return;
                }
            }
            else if (step == FsmStep.Exit)
            {
                HideUsingElixirBar();
            }
        }

        private bool _placedCard = false;
        //private CardData _selectedCardData;
        private void OnPlacing(Fsm<State> fsm, FsmStep step)
        {
            if (step == FsmStep.Enter)
            {
                _cards[_selectedCardIndex].gameObject.SetActive(false);
                _battleManager.DeckSystem.OnCardUsed += OnCardUsed;

                Card card = _battleManager.DeckSystem.Hand[_selectedCardIndex];
                int level = _battleManager.DeckSystem.GetCardLevel(_selectedCardIndex);
                Debug.Log("Placing Card name : " + name + ", lv : " + level);
                CardData selectedCardData = CardCollection.Instance().FindCard(card.id, level);

                if (selectedCardData.radius == 0) // OffsetRect 사용
                {
                    OffsetRect offsetRect
                   = new OffsetRect(selectedCardData.gridSize.top, selectedCardData.gridSize.down,
                   selectedCardData.gridSize.left, selectedCardData.gridSize.right);
                    _gridController.OnSelect(offsetRect);
                }
                else // 원형 범위 사용
                {
                    _gridController.OnSelect(selectedCardData.radius);
                }

                ShowUsingElixirBar(card);

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
                    if (_battleManager.DeckSystem.UseCard(_selectedCardIndex))
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

                _battleManager.DeckSystem.OnCardUsed -= OnCardUsed;

                _gridController.FSM.OnCancelSelect();
                HideUsingElixirBar();
            }
        }

        void OnCardUsed(Card card, int level)
        {
            _placedCard = true;
            //Debug.Log("Plant id : " + _selectedCardData.id);

            SoundManager.PlaySFX("CardDrop");

            // temp id
            //int id = UnityEngine.Random.Range(0, 8);
            //_gridController.FSM.OnPlant(card);
            _gridController.OnPlant(card, level);

            _fsm.TransitionTo(State.Idle);
        }

        private void OnCardDrawn()
        {
            var hand = _battleManager.DeckSystem.Hand;

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

            //_next.text = _battleManager.DeckSystem.Next.id;
            //_nextElixir.text = _battleManager.DeckSystem.Next.cost.ToString();
            _nextCard.SetCard(_battleManager.DeckSystem.Next);
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

            UpdateUsingElixirPos();
        }

        bool _isUsingElixir = false;
        private void ShowUsingElixirBar(Card card)
        {
            _isUsingElixir = true;

            _usingElixir.value = card.cost;

            UpdateUsingElixirPos();
        }

        private void HideUsingElixirBar()
        {
            _isUsingElixir = false;

            _usingElixir.gameObject.SetActive(false);
            _warningElixir.gameObject.SetActive(false);
        }

        private void UpdateUsingElixirPos()
        {
            if (!_isUsingElixir)
            {
                _usingElixir.gameObject.SetActive(false);
                _warningElixir.gameObject.SetActive(false);
            }
            else if (_battleManager.DeckSystem.Hand[_selectedCardIndex].cost > _battleManager.ElixirSystem.ElixirCount)
            {
                _warningElixir.gameObject.SetActive(true);
                _usingElixir.gameObject.SetActive(false);

                _warningElixir.value = _battleManager.DeckSystem.Hand[_selectedCardIndex].cost;
            }
            else
            {
                _warningElixir.gameObject.SetActive(false);
                _usingElixir.gameObject.SetActive(true);

                var fillAreaWidth = _elixirFillArea.rect.width;
                float offsetX = fillAreaWidth * (1 - (_battleManager.ElixirSystem.ElixirCount - 0.5f) / _battleManager.ElixirSystem.MaxElixirCount);

                var usingRect = _usingElixir.GetComponent<RectTransform>();

                var y = usingRect.anchoredPosition.y;
                usingRect.anchoredPosition = new Vector2(_elixirFillArea.anchoredPosition.x - offsetX, y);
            }
        }
    }
}
