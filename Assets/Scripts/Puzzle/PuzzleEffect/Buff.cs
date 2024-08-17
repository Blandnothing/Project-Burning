using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Buff : PuzzleEffect
{
    public enum BuffSelector
    {
        Skill,
        Player
    }
    public abstract BuffSelector Selector { get; }
    public abstract void AddBuff(object target);

}
