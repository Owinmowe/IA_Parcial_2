using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Root : Node
{
    public Root()
    {
    }

    public Root(List<Node> childrens) : base(childrens)
    {
    }

    public override NodeState Evaluate()
    {
        foreach (Node node in childrens)
        {
            switch (node.Evaluate())
            {
                case NodeState.RUNNING:
                    state = NodeState.RUNNING;
                    return state;
                case NodeState.SUCCESS:
                    state = NodeState.SUCCESS;
                    return state;
                case NodeState.FAILURE:
                    break;
                default:
                    break;
            }

        }
        state = NodeState.FAILURE;
        return state;
    }
}
