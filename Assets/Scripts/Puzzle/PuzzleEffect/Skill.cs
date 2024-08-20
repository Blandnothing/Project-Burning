using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[JsonObject(MemberSerialization.OptOut)]
public abstract class Skill : PuzzleEffect
{
    [JsonIgnore]
    public GameObject vfx;
    abstract public void AddSkill(KeyCode key);
    virtual public void ChangeValue(float v1)
    {

    }
    virtual public void ChangeValue(float v1,float v2) 
    {

    }
}
