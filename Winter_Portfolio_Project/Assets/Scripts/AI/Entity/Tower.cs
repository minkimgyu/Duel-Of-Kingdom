using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using WPP.AI;
using WPP.AI.ATTACK;
using WPP.AI.BTUtility;

namespace WPP.AI.TOWER
{
    // 기본 기능은 InitializeBT 여기에서 구현하고
    // 추가로 구현할 기능이 있으면 하위 클레스를 따로 추가 제작해서 적용시키기

    abstract public class AttackTower : Tower
    {
        [SerializeField] Transform _arm;
        protected CaptureComponent _captureComponent;
        protected ViewComponent _viewComponent;
        protected ProjectileAttackComponent _projectileAttackComponent;

        protected override void Start()
        {
            _captureComponent = GetComponentInChildren<CaptureComponent>();
            _viewComponent = GetComponentInChildren<ViewComponent>();
            _projectileAttackComponent = GetComponent<ProjectileAttackComponent>();
            base.Start();
        }

        protected override void InitializeBT()
        {
            List<Node> _childNodes = new List<Node>()
            {
                new Selector
                (
                    new List<Node>()
                    {
                        new Sequence
                        (
                            new List<Node>
                            {
                                new CanFindTarget(_captureComponent), // 만약 타겟이 없다면 타워를 타겟으로 지정해준다.
                                new Sequence
                                (
                                    new List<Node>
                                    {
                                        new CheckIsNearAndCancelAttackWhenExit(_captureComponent, 2.5f, 0.3f, _projectileAttackComponent), // DelayForAttack도 넣어주기
                                        new LookAtTarget(_captureComponent, _viewComponent, _arm),

                                        new Attack(_projectileAttackComponent, _captureComponent)
                                        // 공격 진행
                                        // 만약 공격이 진행 중인 경우 거리가 멀어져도 계속 진행
                                    }
                                )
                            }
                        ),
                    }
                )
            };

            Node rootNode = new Selector(_childNodes);
            _bt.SetUp(rootNode);
        }
    }   

    abstract public class SpawnTower : Tower
    {
        protected override void InitializeBT()
        {

        }
    }

    abstract public class Tower : EntityAI
    {
        protected BoxCollider _boxCollider;

        protected override void Start()
        {
            base.Start();
            _boxCollider = GetComponent<BoxCollider>();

            InitializeBT(); // 여기서 초기화 진행
        }

        public override float ReturnColliderLength()
        {
            return _boxCollider.size.x / 2;
        }
    }
}
