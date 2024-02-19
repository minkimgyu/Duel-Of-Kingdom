using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.AI.TARGET;
using System;

namespace WPP.AI.PROJECTILE
{
    abstract public class BaseProjectile : MonoBehaviour
    {
        [SerializeField] float _speed = 3;
        protected ITarget _target;
        Vector3 _storedPosition;
        protected float _damage;

        protected int _ownershipId;

        public virtual void Initialize(ITarget target, float damage, int ownershipId)
        {
            _target = target;
            _damage = damage;
            _ownershipId = ownershipId;
        }

        private void OnTriggerEnter(Collider other)
        {
            ITarget target = other.GetComponent<ITarget>();
            if (target == null || _target != target) return;

            DoDamageTask();
            Destroy(gameObject);
        }

        protected abstract void DoDamageTask();

        void Move(Vector3 targetPos)
        {
            if (_target.Equals(null) == false)
            {
                Vector3 dir = (targetPos - transform.position).normalized;
                transform.rotation = Quaternion.LookRotation(dir);
            }

            transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * _speed);
            if (_target.Equals(null) == true && Vector3.Distance(transform.position, _storedPosition) < 0.1f)
            {
                Destroy(gameObject);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (_target.Equals(null) == false) _storedPosition = _target.ReturnPosition();
            Move(_storedPosition);
        }
    }
}