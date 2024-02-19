using UnityEngine;

namespace WPP.Battle
{
    public class Tower : MonoBehaviour
    {
        [SerializeField] private TowerData _data;

        public event System.Action<Tower> OnDestroyed;
        public event System.Action<int, int> OnDamaged; // currentHp, maxHp

        private bool _isDestroyed;
        private int _maxHp;
        private int _currentHp;

        private void Awake()
        {
            _isDestroyed = false;
            _maxHp = _data.hp;
            _currentHp = _maxHp;
        }

        public void Damage(int damage)
        {
            if (_isDestroyed) return;

            _currentHp = Mathf.Max(0, _currentHp - damage);
            OnDamaged?.Invoke(_currentHp, _maxHp);

            if (_currentHp <= 0)
            {
                DestroyTower();
            }
        }

        public void DestroyTower()
        {
            if(_isDestroyed) return;
            _isDestroyed = true;

            _currentHp = 0;
            OnDestroyed?.Invoke(this);
        }
    }
}
