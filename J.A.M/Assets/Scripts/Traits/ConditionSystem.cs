using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionSystem
{
    public static bool CheckCondition(TraitsData.Traits characterTraits, TraitsData.SpaceshipTraits spaceshipTraits,
        TraitsData.HiddenSpaceshipTraits hiddenSpaceshipTraits, ConditionSO taskCondition)
    {
        bool validateCondition = true;
        //If at anypoint match becomes false, return
        
        /*Base Condition Check*/
        
        //If match Job
        //continue
        
        if(!CheckJob(characterTraits.GetJob(), taskCondition.BaseCondition.Traits.GetJob())) 
            return false;
        
        //If match Ptraits
        //continue

        if (!CheckPositiveTraits(characterTraits.GetPositiveTraits(),
                taskCondition.BaseCondition.Traits.GetPositiveTraits()))
            return false;
        
        //If match Ntraits
        //continue

        if (!CheckNegativeTraits(characterTraits.GetNegativeTraits(),
                taskCondition.BaseCondition.Traits.GetNegativeTraits()))
            return false;
        
        //If match STraits
        //continue

        //If match HSTraits
        //continue

        if (!CheckSpaceshipTraits(spaceshipTraits, hiddenSpaceshipTraits, taskCondition.BaseCondition.SpaceshipTraits,
                taskCondition.BaseCondition.HiddenSpaceshipTraits)) 
            return false;

        if (taskCondition.additionnalConditions.Count < 1)
            return true;
        
        /*Additionnal Condition Check*/

        bool additionnalCondition = true;
        foreach (var cond in taskCondition.additionnalConditions)
        {
            if(!CheckJob(characterTraits.GetJob(), taskCondition.BaseCondition.Traits.GetJob())) 
                additionnalCondition = false;
        
            if (!CheckPositiveTraits(characterTraits.GetPositiveTraits(),
                    taskCondition.BaseCondition.Traits.GetPositiveTraits()))
                additionnalCondition = false;
        
            if (!CheckNegativeTraits(characterTraits.GetNegativeTraits(),
                    taskCondition.BaseCondition.Traits.GetNegativeTraits()))
                additionnalCondition = false;
        
            if (!CheckSpaceshipTraits(spaceshipTraits, hiddenSpaceshipTraits, taskCondition.BaseCondition.SpaceshipTraits,
                    taskCondition.BaseCondition.HiddenSpaceshipTraits)) 
                additionnalCondition = false;


            //Apply logical operator
            switch (cond.Key)
            {
                case TraitsData.TraitOperator.OR:
                    validateCondition |= additionnalCondition;
                    break;
                case TraitsData.TraitOperator.AND:
                    validateCondition &= additionnalCondition;
                    break;
                case TraitsData.TraitOperator.XOR:
                    validateCondition ^= additionnalCondition;
                    break;
                case TraitsData.TraitOperator.NAND:
                    validateCondition = !(validateCondition && additionnalCondition);
                    break;
                case TraitsData.TraitOperator.NOT:
                    validateCondition = (validateCondition != additionnalCondition);
                    break;
                
                default:
                    break;
            }
        }
        
        
        return validateCondition;
    }

    private static bool CheckJob(TraitsData.Job job, TraitsData.Job conditionJob)
    {
        return (job & conditionJob) == conditionJob;
    }
    
    private static bool CheckPositiveTraits(TraitsData.PositiveTraits pTraits, TraitsData.PositiveTraits conditionPTraits)
    {
        return (pTraits & conditionPTraits) == conditionPTraits;
    }
    
    private static bool CheckNegativeTraits(TraitsData.NegativeTraits negativeTraits, TraitsData.NegativeTraits conditionNtTraits)
    {
        return (negativeTraits & conditionNtTraits) == conditionNtTraits;
    }

    private static bool CheckSpaceshipTraits(TraitsData.SpaceshipTraits spaceshipTraits,
        TraitsData.HiddenSpaceshipTraits hiddenSpaceshipTraits, TraitsData.SpaceshipTraits conditionSTraits,
        TraitsData.HiddenSpaceshipTraits conditionHSTraits)
    {
        return ((spaceshipTraits & conditionSTraits) == conditionSTraits) &&
               ((hiddenSpaceshipTraits & conditionHSTraits) == conditionHSTraits);
    }
}
