using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : PuzzleEffect
{
    public GameObject vfx;
    abstract public void AddSkill(KeyCode key);
    virtual public void ChangeValue(float v1)
    {

    }
    virtual public void ChangeValue(float v1,float v2) 
    {

    }
}
