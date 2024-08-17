using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrengthenSkill : Buff
{
    public float strengthenValue = 1.2f;
    public override BuffSelector Selector { get; } = BuffSelector.Skill;
    public override void AddBuff(object target)
    {
        if (target is not Skill)
        {
            Debug.LogError("Buff Selector Error");
        }

        var skill = target as Skill;
        skill.ChangeValue(strengthenValue);
    }
}
