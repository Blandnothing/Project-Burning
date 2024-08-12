using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PuzzleEffect 
{
    public enum PuzzleType    //ƴͼЧ��������
    {
        Skill,
        Buff
    }
    public PuzzleType type;
    public KeyCode keyCode;  //���ܰ󶨵İ���
    public abstract void Effect();//����ִ�еĺ���
}
