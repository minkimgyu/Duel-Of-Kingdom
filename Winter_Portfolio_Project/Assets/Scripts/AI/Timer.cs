using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace WPP.AI.TIMER
{
    public class Timer
    {
        bool _isRunning = false;
        bool _isFinish = false;

        public bool IsFinish { get { return _isFinish == true && _isRunning == true; } }
        public bool IsRunning { get { return _isFinish == false && _isRunning == true; } }

        float _currentTime = 0;
        float _duration = 0;

        public float Ratio { get { return _currentTime / _duration; } }

        public float PassedTime { get { return _currentTime; } }

        public bool CanStart()
        {
            return _isRunning == false;
        }

        public void Start(float duration)
        {
            if (_isRunning == true) return; // 이미 돌고있는 경우

            _duration = duration;
            _isRunning = true;
            _isFinish = false;
            _currentTime = 0;
        }

        /// <summary>
        /// 타이머를 처음으로 초기화해준다.
        /// </summary>
        public void Reset()
        {
            _isRunning = false;
            _isFinish = false;
            _currentTime = 0;
        }

        void Finish()
        {
            _isFinish = true;
            _currentTime = 0;
            // Running이 true고 Finish가 true인 상태
        }

        public void Update()
        {
            if (_isRunning == false || _isFinish == true) return;

            _currentTime += Time.smoothDeltaTime;
            if (_currentTime >= _duration)
            {
                Finish();
            }
        }
    }
}