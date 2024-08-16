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
    List<Buff> buffs = new();
    static bool isOne;
    private void Awake()
    {
        if (isOne)
        {
            Destroy(this);
        }
        else
        {
            DontDestroyOnLoad(this);
            isOne = true;
        }
    }
    void Start()
    {
        
        foreach (var item in skills)
        {
            item.Value.AddSkill(item.Key);
        }
    }
}
