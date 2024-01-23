using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using WPP.AI.UNIT;
using WPP.AI.ATTACK;
using WPP.AI.BTUtility;

namespace WPP.AI
{
    public class Barbarian : GroundUnit
    {
        MeleeAttackComponent _meleeAttackComponent;

        protected override void Start()
        {
            _meleeAttackComponent = GetComponent<MeleeAttackComponent>();
            base.Start();
        }

        protected override void InitializeBT()
        {
            GameObject go = GameObject.FindWithTag("GridManager");
            if (go == null) return;

            PathFinder pathFinder = go.GetComponent<PathFinder>();
            if (pathFinder == null) return;

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
                                                        new CheckIsNearAndCancelAttackWhenExit(_captureComponent, 1.5f, 0.3f, _meleeAttackComponent), // DelayForAttack�� �־��ֱ�
                                                        new LookAtTarget(_captureComponent, _viewComponent),
                                                        new Stop(_moveComponent),

                                                        new Attack(_meleeAttackComponent, _captureComponent)
                                                        // ���� ����
                                                        // ���� ������ ���� ���� ��� �Ÿ��� �־����� ��� ����
                                                    }
                                                ),
                                                new Sequence
                                                (
                                                    new List<Node>
                                                    {
                                                        new CheckIsNear(_captureComponent, 2.5f),
                                                        new GoDirectToPoint(_captureComponent, _moveComponent, _viewComponent, ResetPosListForDrawingGizmo, IsPosListEmpty)
                                                    }
                                                )
                                            }
                                        ),
                                        new FollowPath(_moveComponent, _viewComponent, _captureComponent, pathFinder, ResetPosListForDrawingGizmo)
                                    }
                                )
                            }
                        ),
                        new Stop(_moveComponent)
                    }
                )
            };

            Node rootNode = new Selector(_childNodes);
            _bt.SetUp(rootNode);
        }
    }
}