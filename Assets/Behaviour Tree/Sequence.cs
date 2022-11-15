using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : Node
{
    public Sequence()
    {
    }

    public Sequence(List<Node> childrens) : base(childrens)
    {
    }

    public override NodeState Evaluate()
    {
        bool anyRunningNode = false;
        foreach (Node node in childrens)
        {
            switch (node.Evaluate())
            {
                case NodeState.RUNNING:
                    anyRunningNode = true;
                    break;
                case NodeState.SUCCESS:
                    break;
                case NodeState.FAILURE:
                    state = NodeState.FAILURE;
                    return state;
                default:
                    break;
            }
        }

        state = anyRunningNode ? NodeState.RUNNING : NodeState.SUCCESS;
        return state;
    }
}
