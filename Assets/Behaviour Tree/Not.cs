using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Not : Node
{
    public Not()
    {
    }

    public Not(List<Node> childrens) : base(childrens)
    {
    }

    public override NodeState Evaluate()
    {
        foreach (Node node in childrens)
        {
            switch (node.Evaluate())
            {
                case NodeState.RUNNING:
                    break;
                case NodeState.SUCCESS:
                    state = NodeState.FAILURE;
                    return state;
                case NodeState.FAILURE:
                    state = NodeState.SUCCESS;
                    return state;
                default:
                    break;
            }
        }

        state = NodeState.RUNNING;
        return state;
    }
}
