using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using WPP.AI.TARGET;
using Tree = BehaviorTree.Tree;
using WPP.AI.ATTACK;
using WPP.AI.BTUtility;
using WPP.AI.CAPTURE;
using WPP.AI.GRID;
using WPP.AI.FSM;
using WPP.AI.ACTION.STATE;
using WPP.POOL;
using WPP.SOUND;

namespace WPP.AI.UNIT
{
    abstract public class Unit : LifeAI
    {
        public enum Type
        {
            AirUnit,
            GroundUnit,
        }

        // �⺻���� ��ġ�� ����� ���⼭ ����
        // �̵����� ����

        // Ready --> Active State�� ��ȯ��

        /// <summary>
        /// ���� �����Ұ��� Ȯ��.
        /// ���� ������ ��� ����
        /// </summary>
        protected bool IgnoreWall { get { return CompareTag(Type.AirUnit.ToString()); } }

        protected MoveComponent _moveComponent;
        protected ViewComponent _viewComponent;
        protected CaptureComponent _captureComponent; // ���� ����� ��ž�̶� ���� Ŭ������ ������ ����

        protected CapsuleCollider _capsuleCollider;

        protected List<Vector3> _posListForDrawingGizmo = new List<Vector3>(); // �̰Ŵ� BT���� �������ֱ�

        protected Vector3 _synchronizedPosition;
        public Vector3 SynchronizedPosition { get { return _synchronizedPosition; } }

        public override float ReturnHpContainerScale() { return 0.5f; }

        protected override void InitializeComponent()
        {
            _moveComponent = GetComponent<MoveComponent>();
            _viewComponent = GetComponent<ViewComponent>();

            _captureComponent = GetComponentInChildren<CaptureComponent>();

            _capsuleCollider = GetComponent<CapsuleCollider>();

            base.InitializeComponent();
        }

        public override void SynchronizePosition(Vector3 pos) { _synchronizedPosition = pos; }

        protected void ResetPosListForDrawingGizmo(List<Vector3> posList) => _posListForDrawingGizmo = posList;

        protected bool IsPosListEmpty() { return _posListForDrawingGizmo.Count == 0; }

        protected override void DrawGizmo() // �̰͵� �̺�Ʈ�� �Ѱ�����
        {
            base.DrawGizmo();

            // ���� �ֱ� ������ ���� ������
            if (_posListForDrawingGizmo.Count == 0) return;

            Gizmos.color = Color.blue;

            for (int i = 0; i < _posListForDrawingGizmo.Count - 1; i++)
                Gizmos.DrawLine(
                    new Vector3(_posListForDrawingGizmo[i].x, 1.5f, _posListForDrawingGizmo[i].z),
                    new Vector3(_posListForDrawingGizmo[i + 1].x, 1.5f, _posListForDrawingGizmo[i + 1].z));

        }

        public override float ReturnColliderSize()
        {
            return _capsuleCollider.radius * (transform.localScale.x + transform.localScale.z) / 2; // �̰Ŵ� ĸ���ݶ��̴��� ����� + scale�� ��� ���� ������Ѿ���
        }

        public override void Die()
        {
            SoundManager.PlaySFX("UnitDie");
            ObjectPooler.SpawnFromPool("DieEffect", transform.position);
            base.Die();
        }
    }

    // �⺻���� ���� ����� ������ ����
    // Ground �Ǵ� Air ���������� ���� Ŭ�������� ���� �Ҵ�
    abstract public class AttackUnit : Unit
    {
        // �⺻ �ɷ�ġ
        protected float _damage; // ������
        protected float _hitSpeed; // ���� �ӵ�
        protected float _range; // ����

        protected float _offsetDistance = 0.45f; // ���� �Ÿ� ������
        protected float _directFollowOffset = 2f; // ���� ���� ��� Ȱ��ȭ �Ÿ�
        //

        protected float _delayDuration = 0; // ���� �� ���� �ð� ������

        AttackComponent _attackComponent;

        public override void ResetDelayAfterSpawn(float delayDuration) { _delayDuration = delayDuration; }

        protected override void InitializeComponent()
        {
            _attackComponent = GetComponent<AttackComponent>();
            _attackComponent.Initialize(_damage, _hitSpeed, _ownershipId);
            base.InitializeComponent();
        }

        public override void Initialize(int level, string name, float hp, CaptureTag[] targetTag, float damage, float hitSpeed, float range, float captureRange)
        {
            // BT�� ���� ������ ���⼭ �ʱ�ȭ�ؾ���

            //_id = id;
            _level = level;
            _name = name;

            _maxHp = hp; // �ִ� ü�� ����
            HP = hp;
            _damage = damage;
            _hitSpeed = hitSpeed;
            _range = range;

            InitializeComponent(); 
            // ���⼭ ������Ʈ�� �����ͼ� �ʱ�ȭ���ش�.
            // �߰��� BT�� �ʱ�ȭ���ش�.
           
            _captureComponent.Initialize(targetTag, OwnershipId, captureRange); // �̷� ������ ���� ������ �Ҵ����ش�.

            Dictionary<ActionState, BaseState> attackStates = new Dictionary<ActionState, BaseState>()
            {
                {ActionState.Ready, new ReadyState(this, _delayDuration)},
                {ActionState.Active, new ActiveState(this)}
            };

            InitializeFSM(attackStates, ActionState.Ready);
        }

        protected override void InitializeBT()
        {
            GameObject go = GameObject.FindWithTag("Grid");
            if (go == null) return;

            PathFinder pathFinder = go.GetComponent<PathFinder>();
            if (pathFinder == null) return;

            List<Node> _childNodes;

            _childNodes = new List<Node>()
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
                                new Selector
                                (
                                    new List<Node>
                                    {
                                        new Selector
                                        (
                                            new List<Node>
                                            {
                                                new Sequence
                                                (
                                                    new List<Node>
                                                    {
                                                        // Ÿ���� �ٲ�� ���, ���� �ð� ������ �־��ֱ�
                                                        new CheckIsNearAndCancelAttackWhenExit(_captureComponent, _range, _offsetDistance, _attackComponent, true, ReturnColliderSize()), // DelayForAttack�� �־��ֱ�
                                                        new LookAtTarget(_captureComponent, _viewComponent),
                                                        new Stop(_moveComponent),

                                                        new Attack(_attackComponent, _captureComponent)
                                                        // ���� ����
                                                        // ���� ������ ���� ���� ��� �Ÿ��� �־����� ��� ����
                                                    }
                                                ),
                                                new Sequence
                                                (
                                                    new List<Node>
                                                    {
                                                        new CheckIsNear(_captureComponent, _range + _directFollowOffset),
                                                        new GoDirectToPoint(_captureComponent, _moveComponent, _viewComponent, ResetPosListForDrawingGizmo, IsPosListEmpty)
                                                    }
                                                )
                                            }
                                        ),
                                        new FollowPath(_moveComponent, _viewComponent, _captureComponent, pathFinder, ResetPosListForDrawingGizmo, IsMyEntity, IgnoreWall)
                                    }
                                )
                            }
                        ),
                        new Stop(_moveComponent)
                    }
                )
            };

            //if (IsMyEntity)
            //{
            //    _childNodes = new List<Node>()
            //    {
            //        new Selector
            //        (
            //            new List<Node>()
            //            {
            //                new Sequence
            //                (
            //                    new List<Node>
            //                    {
            //                        new CanFindTarget(_captureComponent), // ���� Ÿ���� ���ٸ� Ÿ���� Ÿ������ �������ش�.
            //                        new Selector
            //                        (
            //                            new List<Node>
            //                            {
            //                                new Selector
            //                                (
            //                                    new List<Node>
            //                                    {
            //                                        new Sequence
            //                                        (
            //                                            new List<Node>
            //                                            {
            //                                                // Ÿ���� �ٲ�� ���, ���� �ð� ������ �־��ֱ�
            //                                                new CheckIsNearAndCancelAttackWhenExit(_captureComponent, _range, _offsetDistance, _attackComponent, true, ReturnColliderSize()), // DelayForAttack�� �־��ֱ�
            //                                                new LookAtTarget(_captureComponent, _viewComponent),
            //                                                new Stop(_moveComponent),

            //                                                new Attack(_attackComponent, _captureComponent)
            //                                                // ���� ����
            //                                                // ���� ������ ���� ���� ��� �Ÿ��� �־����� ��� ����
            //                                            }
            //                                        ),
            //                                        new Sequence
            //                                        (
            //                                            new List<Node>
            //                                            {
            //                                                new CheckIsNear(_captureComponent, _range + _directFollowOffset),
            //                                                new GoDirectToPoint(_captureComponent, _moveComponent, _viewComponent, ResetPosListForDrawingGizmo, IsPosListEmpty)
            //                                            }
            //                                        )
            //                                    }
            //                                ),
            //                                new FollowPath(_moveComponent, _viewComponent, _captureComponent, pathFinder, ResetPosListForDrawingGizmo, IsMyEntity, IgnoreWall)
            //                            }
            //                        )
            //                    }
            //                ),
            //                new Stop(_moveComponent)
            //            }
            //        )
            //    };
            //}
            //else
            //{
            //    _childNodes = new List<Node>()
            //    {
            //        new Selector
            //        (
            //            new List<Node>()
            //            {
            //                new SyncPosition(this, 3, _moveComponent.MoveSpeed)
            //            }
            //        )
            //    };
            //}

            

            Node rootNode = new Selector(_childNodes);
            _bt.SetUp(rootNode);
        }
    }
}