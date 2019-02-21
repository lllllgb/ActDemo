using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffBase
{
    public int ID;
    public int Type;
    public int Time;
    public int SpecialEffect;
    public string Parameter;
    public string AttribEffect;
    public int Multipy;
    public int Add;
    public int SummonUnit;
    public string Effect;
    public string KeepEffect;
    public string EndEffect;
	
	public int Key () { return ID; }
};

public class BuffBaseManager
{
    protected static readonly BuffBaseManager msInstance = new BuffBaseManager();
    public static BuffBaseManager Instance { get { return msInstance; } }

    protected BuffBaseManager()
    {
    }

    public BuffBase GetItem(int id)
    {
        return new BuffBase();
    }
}

