using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace WPP.AI.UI
{
    abstract public class BaseUI : MonoBehaviour
    {
        protected float yPos = 5f;

        protected void LookCamera()
        {
            Transform _cameraTransform = Camera.main.transform;
            transform.LookAt(transform.position + _cameraTransform.rotation * Vector3.forward, _cameraTransform.rotation * Vector3.up);
        }

        public virtual void OnDestroyRequested()
        {
            if (gameObject == null) return;
            Destroy(gameObject);
        }
    }
}
