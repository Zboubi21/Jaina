using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaBeamController : BossAttack
{
    
    void StartLaser(int phaseNbr)
    {

    }

    public override void On_AttackBegin(int phaseNbr)
    {
        base.On_AttackBegin(phaseNbr);
        StartLaser(phaseNbr);
    }

    public override void On_AttackEnd()
    {
        base.On_AttackEnd();
    }

}
