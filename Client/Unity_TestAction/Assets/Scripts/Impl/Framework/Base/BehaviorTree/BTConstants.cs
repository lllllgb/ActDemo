using UnityEngine;
using System.Collections;
using System;

namespace BT
{

	public enum EBTResult
    {
		Success,
		Failed,
		Running,
	}

	public enum EBTAbortOpt
    {
		None,
		Self,
		LowerPriority,
		Both,
	}

    [Flags]
	public enum EBTClearOpt
    {
		Default = 1 << 0,
		Selected = 1 << 1,
		DefaultAndSelected,
		All,
	}

	public enum EBTLogic
    {
		And,
		Or,
	}

    [Flags]
	public enum EBTExecuteOpt
    {
		OnTick = 1 << 0,
		OnClear = 1 << 1,
		Both = OnTick | OnClear,
	}

	public enum BTDataReadOpt
    {
		ReadAtBeginning,
		ReadEveryTick,
	}

	public enum BTNumCompareOpt
    {
		Greater,
		GreaterEquals,
		Equals,
		LessEquals,
		Less,
	}

	public enum BTSpecialNum
    {
		AlliesNum,
	}
}