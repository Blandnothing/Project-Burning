using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "DoubleJump Buff", menuName = "Puzzle/Puzzle Effect/Buff/DoubleJump")]

public class DoubleJump : Buff
{
    public override BuffSelector Selector { get; } = BuffSelector.Player;
    public override void AddBuff(object target)
    {
        if (target is not PlayerScript)
        {
            Debug.LogError("Buff Selector Error");
        }

        var player = target as PlayerScript;
        player.maxJumpCount++;
    }
}
