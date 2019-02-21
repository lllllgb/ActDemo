using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BT
{

	/// <summary>
	/// BTConditionEvaluator is a decorator node.
	/// It allows the child to tick if the conditional check return true, and returns what the child returns.
	/// 
	/// It has two clear options
	/// 	- OnAbortRunning: if Clear is called, it will only clear its child, if it was running.
	/// 	- OnNotRunning: if Clear is called, it will clear its child no matter what.
	/// </summary>
	public class BTConditionEvaluator : BTDecorator
    {

		private List<BTConditional> mConditionals;
        private EBTLogic mLogicOpt;
        private bool mIsConditionsComplete = false;
        
		public BTConditionEvaluator (List<BTConditional> conditionals, EBTLogic logicOpt, BTNode child) : base (child)
        {
			this.mConditionals = conditionals;
			this.mLogicOpt = logicOpt;
		}

        public BTConditionEvaluator(BTConditional conditional, BTNode child) : 
            this(new List<BTConditional>() { conditional}, EBTLogic.And, child)
        {
        }


        public override void Activate (BTDatabase database)
        {
			base.Activate (database);

            for (int i = 0, max = mConditionals.Count; i < max; ++i)
            {
                mConditionals[i].Activate(database);
            }
		}

        public override EBTResult Tick()
        {
            if (!mIsConditionsComplete)
            {
                switch (mLogicOpt)
                {
                    case EBTLogic.And:
                        foreach (BTConditional conditional in mConditionals)
                        {
                            if (!conditional.Check())
                            {
                                return EBTResult.Failed;
                            }
                        }
                        break;

                    case EBTLogic.Or:
                        bool anySuccess = false;

                        foreach (BTConditional conditional in mConditionals)
                        {
                            if (conditional.Check())
                            {
                                anySuccess = true;
                                break;
                            }
                        }

                        if (!anySuccess)
                        {
                            return EBTResult.Failed;
                        }
                        break;
                }
            }

            mIsConditionsComplete = true;
            EBTResult tmpResult = ChildNode.Tick();

            if (EBTResult.Running == tmpResult)
            {
                IsRunning = true;
            }

            return tmpResult;
        }

		public override void Clear()
        {
            base.Clear();

            mIsConditionsComplete = false;
		}
	}

}