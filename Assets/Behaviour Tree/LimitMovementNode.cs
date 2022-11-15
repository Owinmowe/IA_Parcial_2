using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitMovementNode : Node
{
    private GameObject go;
    public LimitMovementNode(List<Node> childrens, GameObject go) : base(childrens)
    {
        this.go = go;
    }

    public override NodeState Evaluate()
    {
        if (go.transform.position.x > 5.0f)
            state = NodeState.FAILURE;
        else
            state = NodeState.SUCCESS;

        return state; ;
    }
}
