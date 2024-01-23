using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using WPP.AI;
using WPP.AI.ATTACK;
using WPP.AI.BTUtility;

namespace WPP.AI.TOWER
{
    // �⺻ ����� InitializeBT ���⿡�� �����ϰ�
    // �߰��� ������ ����� ������ ���� Ŭ������ ���� �߰� �����ؼ� �����Ű��

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
                                new CanFindTarget(_captureComponent), // ���� Ÿ���� ���ٸ� Ÿ���� Ÿ������ �������ش�.
                                new Sequence
                                (
                                    new List<Node>
                                    {
                                        new CheckIsNearAndCancelAttackWhenExit(_captureComponent, 2.5f, 0.3f, _projectileAttackComponent), // DelayForAttack�� �־��ֱ�
                                        new LookAtTarget(_captureComponent, _viewComponent, _arm),

                                        new Attack(_projectileAttackComponent, _captureComponent)
                                        // ���� ����
                                        // ���� ������ ���� ���� ��� �Ÿ��� �־����� ��� ����
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

            InitializeBT(); // ���⼭ �ʱ�ȭ ����
        }

        public override float ReturnColliderLength()
        {
            return _boxCollider.size.x / 2;
        }
    }
}
