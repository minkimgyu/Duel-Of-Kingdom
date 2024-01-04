using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public enum NodeState
    {
        RUNNING,
        SUCCESS,
        FAILURE
    }

    public class Tree
    {
        private Node _rootNode = null;

        public void SetUp(Node rootNode)
        {
            _rootNode = rootNode;
        }

        public void OnUpdate()
        {
            if (_rootNode != null)
                _rootNode.Evaluate();
        }
    }

    public class Sequence : Node
    {
        public Sequence() : base() { }
        public Sequence(List<Node> childNodes) : base(childNodes) { }

        public override NodeState Evaluate()
        {
            for (int i = 0; i < _childNodes.Count; i++)
            {
                NodeState nowState = _childNodes[i].Evaluate();

                if (nowState == NodeState.FAILURE)
                {
                    return NodeState.FAILURE;
                }
                else if (nowState == NodeState.SUCCESS)
                {
                    continue;
                }
                else if (nowState == NodeState.RUNNING)
                {
                    return NodeState.RUNNING;
                }
                else
                {
                    continue;
                }
            }

            return NodeState.SUCCESS;
            // 만약 모든 노드가 SUCCESS인 경우 마지막에 SUCCESS 반환
        }
    }

    public class Selector : Node
    {
        public Selector() : base() { }
        public Selector(List<Node> childNodes) : base(childNodes) { }

        public override NodeState Evaluate()
        {
            for (int i = 0; i < _childNodes.Count; i++)
            {
                NodeState nowState = _childNodes[i].Evaluate();

                if (nowState == NodeState.FAILURE)
                {
                    continue;
                }
                else if (nowState == NodeState.SUCCESS)
                {
                    return NodeState.SUCCESS;
                }
                else if (nowState == NodeState.RUNNING)
                {
                    return NodeState.RUNNING;
                }
                else
                {
                    continue;
                }
            }

            return NodeState.FAILURE;
        }
    }

    abstract public class IFNode : Node
    {
        public IFNode(Node childNode) : base(childNode)
        {
        }

        protected abstract bool CheckCondition();

        public override NodeState Evaluate()
        {
            bool result = CheckCondition();

            if (_childNodes.Count == 0 || _childNodes[0] == null) return NodeState.FAILURE;

            if (result == true)
            {
                return _childNodes[0].Evaluate(); // Ã¹¹øÂ° ³ëµå¸¦ Æò°¡ÇÔ
            }
            else
            {
                return NodeState.FAILURE;
            }
        }
    }

    abstract public class Node
    {
        Node _parentNode;
        Node ParentNode { set { _parentNode = value; } }

        protected List<Node> _childNodes = new List<Node>();

        public Node()
        {
            _parentNode = null;
        }

        public Node(Node node)
        {
            AddNodeToChildList(node);
        }

        public Node(List<Node> childrenNodes)
        {
            for (int i = 0; i < childrenNodes.Count; i++)
            {
                AddNodeToChildList(childrenNodes[i]);
            }
        }

        void AddNodeToChildList(Node node)
        {
            node.ParentNode = this;
            _childNodes.Add(node);
        }

        public virtual NodeState Evaluate() { return default; }
    }
}