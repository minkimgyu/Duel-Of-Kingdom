using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace WPP.AI.UI
{
    public class ClockUI : BaseUI
    {
        [SerializeField] Image _content;
        float _duration;

        public void Initialize(Vector3 pos, float duration)
        {
            _duration = duration;
            transform.position = new Vector3(pos.x, yPos, pos.z);

            LookCamera();
            _content.DOFillAmount(1, _duration).onComplete = OnDestroyRequested;
        }

        public override void OnDestroyRequested()
        {
            DOTween.Kill(_content);
            base.OnDestroyRequested();
        }
    }
}
