using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAttrib
{
    public int ID;
    public string Desc;
    public int Level;
    public string Action1;
    public string Action2;
    public string Action3;
    public string Icon2;
    public string Icon3;
    public int CD;
    public int LevelRequest;
    public int SP;
    public int Gold;
    public int Gem;
    public int Energy1;
    public int Energy2;
    public int Energy3;
    public int DamageCoff1;
    public int DamageBase1;
    public int DamageCoff2;
    public int DamageBase2;
    public int DamageCoff3;
    public int DamageBase3;
    public int Mode;
    public int Chance;
    public int Target;
    public int BuffID;
	
    public static int MakeKey(int id, int level) 
	{ 
		return (id << 16) + level; 
	}
    public int Key() { return MakeKey(ID, Level); }
};

public class SkillAttribManager
{
    protected static readonly SkillAttribManager msInstance = new SkillAttribManager();
    public static SkillAttribManager Instance { get { return msInstance; } }
    

    public SkillAttrib GetItem(int id, int level)
    {
        return new SkillAttrib();
    }
}
