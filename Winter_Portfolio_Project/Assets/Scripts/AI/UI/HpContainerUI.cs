using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace WPP.AI.UI
{
    public class HpContainerUI : BaseUI
    {
        [SerializeField] GameObject _container;
        [SerializeField] Image _content;
        [SerializeField] TMP_Text _levelTxt;
        [SerializeField] TMP_Text _hpTxt;

        Transform _parentTransform;

        public void Initialize(int level, float hp, Transform parentTransform)
        {
            _levelTxt.text = level.ToString();
            _hpTxt.text = hp.ToString();
            OnTxtVisibleRequested(false);

            _parentTransform = parentTransform;
            LookCamera();
        }

        private void Update()
        {
            transform.position = new Vector3(_parentTransform.position.x, yPos, _parentTransform.position.z);
        }

        public void OnVisibleChangeRequested(bool nowShow) => _container.SetActive(nowShow);
        public void OnTxtVisibleRequested(bool nowShow) => _hpTxt.gameObject.SetActive(nowShow);

        public void OnHpChangeRequested(float hp, float maxHp)
        {
            _content.DOFillAmount(hp / maxHp, 0.5f);
            _hpTxt.text = hp.ToString();
        }

        public override void OnDestroyRequested()
        {
            DOTween.Kill(_content);
            base.OnDestroyRequested();
        }
    }
}