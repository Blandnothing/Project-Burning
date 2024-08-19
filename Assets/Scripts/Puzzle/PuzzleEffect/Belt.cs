using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Belt Skill", menuName = "Puzzle/Puzzle Effect/Skill/Belt")]
public class Belt : Skill
{
    public float Atk { get => attackPower * multiplier; }
    public float attackPower = 100;
    public float atkBack = 5;
    float multiplier = 1f;
    public override void AddSkill(KeyCode key)
    {
        PlayerScript.Instance.dicSkill[key]=new PlayerScript.SkillInfo(PlayerScript.SkillInfo.SkillType.atk,"¼¼ÄÜ",Atk,atkBack,vfx);
    }
    public override void ChangeValue(float v1)
    {
        multiplier += v1;
    }
}
