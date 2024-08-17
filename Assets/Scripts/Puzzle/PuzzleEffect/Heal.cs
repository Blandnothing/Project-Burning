using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : Skill
{
    public float healValue=50;

    public override void AddSkill(KeyCode key)
    {
        PlayerScript.Instance.dicSkill[key]=new PlayerScript.SkillInfo(PlayerScript.SkillInfo.SkillType.heal,"",healValue);
    }
    public override void ChangeValue(float v1)
    {
        healValue *= v1;
    }
}
