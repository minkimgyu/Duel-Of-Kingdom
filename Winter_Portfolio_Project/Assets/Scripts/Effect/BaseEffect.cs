using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.POOL;

namespace WPP.EFFECT
{
    abstract public class BaseEffect : MonoBehaviour
    {
        private void OnDisable()
        {
            ObjectPooler.ReturnToPool(gameObject);
            CancelInvoke();
        }

        public abstract void PlayEffect();

        public abstract void StopEffect();

        protected void DisableObject() => gameObject.SetActive(false);
    }
}
