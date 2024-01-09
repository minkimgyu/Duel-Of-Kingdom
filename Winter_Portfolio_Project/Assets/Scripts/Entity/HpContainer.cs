using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HpContainer : MonoBehaviour
{
    [SerializeField] Image content;

    public void OnHpChangeRequested(float ratio)
    {
        content.DOFillAmount(ratio, 0.5f);
    }
}
