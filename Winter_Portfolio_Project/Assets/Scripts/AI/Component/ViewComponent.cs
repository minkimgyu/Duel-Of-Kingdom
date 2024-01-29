using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewComponent : MonoBehaviour
{
    [SerializeField] float _viewSpeed = 3f;

    Vector3 ReturnDirection(Vector3 targetPos)
    {
        return (targetPos - transform.position).normalized;
    }

    public void View(Vector3 targetPos)
    {
        Vector3 dir = ReturnDirection(targetPos);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * _viewSpeed);
    }

    public void View(Transform arm, Vector3 targetPos)
    {
        Vector3 dir = ReturnDirection(targetPos);
        arm.rotation = Quaternion.Lerp(arm.rotation, Quaternion.LookRotation(dir), Time.deltaTime * _viewSpeed);
    }
}
