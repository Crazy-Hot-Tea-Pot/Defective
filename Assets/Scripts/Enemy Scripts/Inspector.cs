using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inspector : Enemy
{
    public override void Start()
    {
        if (EnemyName == null)
            EnemyName = "Inspector";

        base.Start();
    }
    protected override void PerformIntent()
    {

    }

}