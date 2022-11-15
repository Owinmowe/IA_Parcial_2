using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinerBT : Tree
{

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
    protected override Node Setup()
    {
        Node root = new Root(new List<Node>{
            new Sequence(new List<Node>{
                new Not(new List<Node>()
                {
                    new Sequence(new List<Node>{
                        new LimitMovementNode(new List<Node>(), gameObject),
                        new MovementNode(new List<Node>(), gameObject)
                    }),
                }),
                new RotationNode(new List<Node>(), gameObject)
            })
        });
        
        return root;
    }
}
