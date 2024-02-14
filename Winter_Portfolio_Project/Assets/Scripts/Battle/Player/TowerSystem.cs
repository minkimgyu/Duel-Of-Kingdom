using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPP.Battle
{
    public class TowerSystem : MonoBehaviour
    {
        [SerializeField] private CrownSystem _crownSystem;

        private bool _isKingTowerDestroyed;
        private bool _isLeftPrincessTowerDestroyed;
        private bool _isRightPrincessTowerDestroyed;

        public event Action OnKingTowerDestroyed;
        public event Action OnLeftPrincessTowerDestroyed;
        public event Action OnRightPrincessTowerDestroyed;

        public void DestroyKingTower()
        {
            if (_isKingTowerDestroyed) return;

            _isKingTowerDestroyed = true;

            DestroyLeftPrincessTower();
            DestroyRightPrincessTower();

            _crownSystem.AddCrown();
            OnKingTowerDestroyed?.Invoke();
        }

        public void DestroyLeftPrincessTower()
        {
            if (_isLeftPrincessTowerDestroyed) return;

            _isLeftPrincessTowerDestroyed = true;

            _crownSystem.AddCrown();
            OnLeftPrincessTowerDestroyed?.Invoke();
        }

        public void DestroyRightPrincessTower()
        {
            if (_isRightPrincessTowerDestroyed) return;

            _isRightPrincessTowerDestroyed = true;

            _crownSystem.AddCrown();
            OnRightPrincessTowerDestroyed?.Invoke();
        }
    }
}