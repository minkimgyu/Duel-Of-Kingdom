using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPP.Battle
{
    public class CrownSystem : MonoBehaviour
    {
        public event System.Action<int> OnCrownCountChange;
        public event System.Action OnCrownCountMax;

        private int _maxCrownCount = 3;
        private int _crownCount = 0;

        public void AddCrown()
        {
            if (_crownCount >= _maxCrownCount) return;

            _crownCount++;
            _crownCount = Mathf.Clamp(_crownCount, 0, _maxCrownCount);

            OnCrownCountChange?.Invoke(_crownCount);

            if (_crownCount == _maxCrownCount)
            {
                OnCrownCountMax?.Invoke();
            }
        }


    }
}
