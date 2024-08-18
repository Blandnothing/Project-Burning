using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "StrengthenSkill Buff", menuName = "Puzzle/Puzzle Effect/Buff/StrengthenSkill")]

public class StrengthenSkill : Buff
{
    public float strengthenValue = 0.2f;
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
