using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PuzzleEffect 
{
    public enum PuzzleType    //拼图效果的种类
    {
        Skill,
        Buff
    }
    public PuzzleType type;
    public KeyCode keyCode;  //技能绑定的按键
    public abstract void Effect();//技能执行的函数
}
