using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : PuzzleEffect
{
    abstract public void AddSkill(KeyCode key);
    virtual public void ChangeValue(float v1)
    {

    }
    virtual public void ChangeValue(float v1,float v2)
    {

    }
}
