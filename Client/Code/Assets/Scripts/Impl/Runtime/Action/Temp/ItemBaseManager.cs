using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase
{
    public int ID;
    public string Name;
    public string Desc;
    public byte MainType;
    public byte SubType;
    public int Role;
    public byte Level;
    public byte Quality;
    public int Score;
    public string Model;
    public string Icon;
    public int SellPrice;
	
	public int Key () { return ID; }
};

public class ItemBaseManager
{
    protected static readonly ItemBaseManager msInstance = new ItemBaseManager();
    public static ItemBaseManager Instance { get { return msInstance; } }

    public ItemBase GetItem(int id)
    {
        return new ItemBase();
    }
}

