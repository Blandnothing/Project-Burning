using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Belt : Skill
{
    public float atk = 100;
    public float atkBack = 5;
    public override void AddSkill(KeyCode key)
    {
        PlayerScript.Instance.dicSkill[key]=new PlayerScript.SkillInfo(PlayerScript.SkillInfo.SkillType.atk,"¼¼ÄÜ",atk,atkBack);
    }
    public override void ChangeValue(float v1)
    {
        atk *= v1;
    }
}
