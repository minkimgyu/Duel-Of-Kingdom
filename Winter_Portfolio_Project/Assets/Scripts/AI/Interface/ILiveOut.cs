using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPP.AI.BUILDING
{
    // ÃµÃµÈ÷ Ã¼·ÂÀ» ±ð¾ÆÁÜ
    public interface ILiveOut
    {
        public float LifeTime { get; set; }
        public float PassedTime { get; set; }
        public void DecreaseHp();
    }
}
