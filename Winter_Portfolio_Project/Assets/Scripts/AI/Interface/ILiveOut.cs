using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPP.AI.BUILDING
{
    // õõ�� ü���� �����
    public interface ILiveOut
    {
        public float LifeTime { get; set; }
        public float PassedTime { get; set; }
        public void DecreaseHp();
    }
}
