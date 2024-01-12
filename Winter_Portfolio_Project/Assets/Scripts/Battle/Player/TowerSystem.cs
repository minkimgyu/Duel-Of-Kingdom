using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPP.Battle
{
    public class TowerSystem : MonoBehaviour
    {
        [SerializeField] private List<Tower> _crownTowers;
        [SerializeField] private Tower _kingTower;

        public event System.Action<Tower> OnCrownTowerDestroyed;
        public event System.Action<Tower> OnKingTowerDestroyed;


        private void OnEnable()
        {
            _kingTower.OnDestroyed += OnKingTowerDestroyHandler;
        }
        private void OnDisable()
        {
            _kingTower.OnDestroyed -= OnKingTowerDestroyHandler;
        }

        private void OnKingTowerDestroyHandler(Tower tower)
        {
            DestoyAllCrownTower();
        }

        private void DestoyAllCrownTower()
        {
            foreach (var tower in _crownTowers)
            {
                tower.DestroyTower();
            }
        }
    }
}