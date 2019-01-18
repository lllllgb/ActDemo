using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBase
{
    public int ID;
    public int Type;
    public int Slot1;
    public int Slot2;
    public int Slot3;
    public int Slot4;
	
    public int Key() { return ID; }
};

public class SkillBaseManager
{
    protected static readonly SkillBaseManager msInstance = new SkillBaseManager();
    public static SkillBaseManager Instance { get { return msInstance; } }

    public SkillBase GetItem(int id)
    {
        return new SkillBase();
    }
}
