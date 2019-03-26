using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AosHotfixRunTime
{
    public class MathUtility
    {

        public static ACT.ECombatResult Combat(Unit self, Unit target, out int finallyDamage)
        {
            int damage = self.GetAttr(EPA.Attack);
            int defence = target.GetAttr(EPA.Defense);
            finallyDamage = Mathf.Max(damage - defence, 0);

            ACT.ECombatResult result = ACT.ECombatResult.ECR_Normal;
            {
                int critical = self.GetAttr(EPA.Critical);
                int hit = self.GetAttr(EPA.Hit);
                int block = target.GetAttr(EPA.Block);
                int tough = target.GetAttr(EPA.Tough);
                int MAX_RATIO_VALUE = 10000;
                block = Mathf.Clamp(block - hit, 0, MAX_RATIO_VALUE / 2);
                critical = Mathf.Clamp(critical - tough, 0, MAX_RATIO_VALUE / 2);

                
                int rand = UnityEngine.Random.Range(0, MAX_RATIO_VALUE + 1);
                if (rand < block)
                {
                    result = ACT.ECombatResult.ECR_Block;
                    finallyDamage = 0;
                }
                else if (rand < block + critical)
                {
                    result = ACT.ECombatResult.ECR_Critical;
                    finallyDamage = finallyDamage * 150 / 100;
                }
            }

            return result;
        }
    }
}
