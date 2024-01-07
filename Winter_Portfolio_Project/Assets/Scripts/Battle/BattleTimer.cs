using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPP.Battle
{
    public class BattleTimer : MonoBehaviour
    {
        // float : timer length
        public event System.Action<float> OnTimerStart;
        // elapsed time, timer length
        public event System.Action<float, float> OnTimerUpdate;
        public event System.Action OnTimerEnd;

        public void StartTimer(float length)
        {
            _timerLength = length;
            _elapsedTime = 0f;

            OnTimerStart?.Invoke(_timerLength);
            OnTimerUpdate?.Invoke(0, _timerLength);

            _isPaused = false;
        }

        public void ResumeTimer()
        {
            throw new System.NotImplementedException();
        }

        public void PauseTimer()
        {
            _isPaused = true;
        }

        public void SetCurrentTime(int time)
        {
            throw new System.NotImplementedException();
        }

        private bool _isPaused;
        private float _timerLength;
        private float _elapsedTime;

        private void Awake()
        {
            _isPaused = true;
            _elapsedTime = 0f;
        }

        private void Update()
        {
            if (_isPaused)
            {
                return;
            }

            _elapsedTime += Time.deltaTime;
            OnTimerUpdate?.Invoke(Mathf.Min(_elapsedTime, _timerLength), _timerLength);

            if (_elapsedTime >= _timerLength)
            {
                _isPaused = true;
                OnTimerEnd?.Invoke();
            }
        }
    }
}