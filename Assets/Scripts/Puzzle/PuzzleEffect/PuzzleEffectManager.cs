using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleEffectManager : MonoBehaviour
{
    static PuzzleEffectManager instance;
    public static PuzzleEffectManager Instance
    {
        get
        {
            if (instance==null)
            {
                Debug.LogError("PuzzleEffectManager Lost");
            }
            return instance;
        }
    }
    public Dictionary<KeyCode, Skill> skills = new();
    Dictionary<KeyCode,Buff> buffs = new();
    static bool isOne;
    private void Awake()
    {
        if (isOne)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(this);
            isOne = true;
        }
    }
    void Start()
    {
        foreach (var item in buffs)
        {
            switch (item.Value.Selector)
            {
                case Buff.BuffSelector.Skill:
                    if(skills.ContainsKey(item.Key))
                        item.Value.AddBuff(skills[item.Key]);
                    break;
                case Buff.BuffSelector.Player:
                    item.Value.AddBuff(PlayerScript.Instance);
                    break;
                default:
                    break;
            }
        }
        
        foreach (var item in skills)
        {
            item.Value.AddSkill(item.Key);
        }
    }
}
