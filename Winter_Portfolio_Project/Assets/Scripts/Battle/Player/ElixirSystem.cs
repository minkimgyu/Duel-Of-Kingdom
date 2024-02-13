using UnityEngine;

namespace WPP.Battle
{
    public class ElixirSystem : MonoBehaviour
    {
        [SerializeField] private int _startElixir = 5;
        [SerializeField] private int _maxElixir = 10;
        [Space]
        [SerializeField] private float _startElixirRegenTime = 2.8f;

        public event System.Action<int> OnElixirCountChange;
        public int ElixirCount => _elixirCount;
        public float MaxElixirCount => _maxElixir;
        public float ElixirRegenTime => _elixirRegenTime;

        private float _elixirRegenTime = 2.8f;
        private int _elixirCount = 0;
        private bool _regenStarted = false;

        private void Awake()
        {
            _elixirCount = _startElixir;
            _elixirRegenTime = _startElixirRegenTime;
        }

        private void Start()
        {
            OnElixirCountChange?.Invoke(_elixirCount);
        }

        public void SpendElixir(int amount)
        {
            if (_elixirCount < amount) return;

            _elixirCount = Mathf.Clamp(_elixirCount - amount, 0, _maxElixir);

            OnElixirCountChange?.Invoke(_elixirCount);
        }

        public void AddElixir()
        {
            if(_elixirCount >= _maxElixir) return;

            _elixirCount = Mathf.Clamp(_elixirCount + 1, 0, _maxElixir);

            OnElixirCountChange?.Invoke(_elixirCount);
        }

        public void SetElixirRegenTime(float time)
        {
            _elixirRegenTime = time;
        }
    
        public void StartRegen()
        {
            _regenStarted = true;
            OnElixirCountChange?.Invoke(_elixirCount);
        }
        public void StopRegen()
        {
            _regenStarted = false;
        }

        private float _regenTimer = 0f;
        private void Update()
        {
            if (!_regenStarted) return;

            _regenTimer += Time.deltaTime;
            if (_regenTimer >= _elixirRegenTime)
            {
                _regenTimer = 0f;
                AddElixir();
            }
        }
    }
}
