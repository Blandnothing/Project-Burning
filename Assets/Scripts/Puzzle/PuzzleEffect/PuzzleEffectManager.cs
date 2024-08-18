using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    Dictionary<KeyCode,List<Buff>> buffs = new();
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
        SceneManager.sceneLoaded += (s, e) =>EnableEffects();
    }
    void Start()
    {
        EnableEffects();
    }
    public void EnableEffects()
    {
        if (!GameObject.FindGameObjectWithTag("Player")) return;

        foreach (var bs in buffs)
        {
            foreach (var item in bs.Value)
            {
                switch (item.Selector)
                {
                    case Buff.BuffSelector.Skill:
                        if (skills.ContainsKey(bs.Key))
                            item.AddBuff(skills[bs.Key]);
                        break;
                    case Buff.BuffSelector.Player:
                        item.AddBuff(PlayerScript.Instance);
                        break;
                    default:
                        break;
                }
            }
        }

        foreach (var item in skills)
        {
            item.Value.AddSkill(item.Key);
        }
    }
    public void AddPuzzle(PuzzleEffect effect,KeyCode key)
    {
        if(effect is Buff)
        {
            if (!buffs.ContainsKey(key))
            {
                buffs[key] = new();
            }
            buffs[key].Add(effect as Buff);
        }
        else if (effect is Skill)
        {
            skills[key]= effect as Skill;
        }
    }
}
