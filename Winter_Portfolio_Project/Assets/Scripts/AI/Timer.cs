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
            if (_isRunning == true) return; // �̹� �����ִ� ���

            _duration = duration;
            _isRunning = true;
            _isFinish = false;
            _currentTime = 0;
        }

        /// <summary>
        /// Ÿ�̸Ӹ� ó������ �ʱ�ȭ���ش�.
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
            // Running�� true�� Finish�� true�� ����
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