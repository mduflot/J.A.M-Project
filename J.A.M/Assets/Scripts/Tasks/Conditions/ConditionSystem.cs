using UnityEngine;

public class ConditionSystem
{
    public static bool CheckCondition(TraitsData.Traits characterTraits, TraitsData.SpaceshipTraits spaceshipTraits,
        TraitsData.HiddenSpaceshipTraits hiddenSpaceshipTraits, ConditionSO taskCondition)
    {
        bool validateCondition = true;

        /*Base Condition Check*/

        if (!CheckJob(characterTraits.GetJob(), taskCondition.BaseCondition.Traits.GetJob()))
            validateCondition = false;

        if (!CheckPositiveTraits(characterTraits.GetPositiveTraits(),
                taskCondition.BaseCondition.Traits.GetPositiveTraits()))
            validateCondition = false;

        if (!CheckNegativeTraits(characterTraits.GetNegativeTraits(),
                taskCondition.BaseCondition.Traits.GetNegativeTraits()))
            validateCondition = false;

        if (!CheckSpaceshipTraits(spaceshipTraits, hiddenSpaceshipTraits, taskCondition.BaseCondition.SpaceshipTraits,
                taskCondition.BaseCondition.HiddenSpaceshipTraits))
            validateCondition = false;

        if (taskCondition.supplementaryConditions.Count < 1)
            return validateCondition;

        /*Additionnal Condition Check*/

        bool supplementaryCondition = true;
        //FIX : turn into for loop
        foreach (var cond in taskCondition.supplementaryConditions)
        {
            if (!CheckJob(characterTraits.GetJob(), cond.Value.Traits.GetJob()))
                supplementaryCondition = false;

            if (!CheckPositiveTraits(characterTraits.GetPositiveTraits(),
                    cond.Value.Traits.GetPositiveTraits()))
                supplementaryCondition = false;

            if (!CheckNegativeTraits(characterTraits.GetNegativeTraits(),
                    cond.Value.Traits.GetNegativeTraits()))
                supplementaryCondition = false;

            if (!CheckSpaceshipTraits(spaceshipTraits, hiddenSpaceshipTraits,
                    cond.Value.SpaceshipTraits,
                    cond.Value.HiddenSpaceshipTraits))
                supplementaryCondition = false;

            //Apply logical operator
            switch (cond.Key)
            {
                case TraitsData.TraitOperator.OR:
                    validateCondition |= supplementaryCondition;
                    break;
                case TraitsData.TraitOperator.AND:
                    validateCondition &= supplementaryCondition;
                    break;
                case TraitsData.TraitOperator.XOR:
                    validateCondition ^= supplementaryCondition;
                    break;
                case TraitsData.TraitOperator.NAND:
                    validateCondition = !(validateCondition && supplementaryCondition);
                    break;
                case TraitsData.TraitOperator.NOT:
                    validateCondition = (validateCondition & !supplementaryCondition);
                    break;
            }
        }

        return validateCondition;
    }

    public static bool CheckEnvironmentCondition(CharacterBehaviour[] crew, TraitsData.SpaceshipTraits spaceshipTraits, TraitsData.HiddenSpaceshipTraits hiddenSpaceshipTraits, ConditionSO condition)
    {
        bool validateCondition = true;
        int validatingCrew = -1;
        
        validateCondition = CheckSpaceshipTraits(spaceshipTraits, hiddenSpaceshipTraits,
            condition.BaseCondition.SpaceshipTraits,
            condition.BaseCondition.HiddenSpaceshipTraits);

        if (!validateCondition)
            return false;
        
        for (int i = 0; i < crew.Length; i++)
        {
            //In theory you could check CheckJob return value, skipping an assignation each time, but if(!CheckJob) continue; seems to create unreachable code
            validateCondition = CheckJob(crew[i].GetJob(), condition.BaseCondition.Traits.GetJob());
            if (!validateCondition) continue;
            
            validateCondition = CheckPositiveTraits(crew[i].GetPositiveTraits(), condition.BaseCondition.Traits.GetPositiveTraits());
            if (!validateCondition) continue;

            validateCondition = CheckNegativeTraits(crew[i].GetNegativeTraits(), condition.BaseCondition.Traits.GetNegativeTraits());
            if (!validateCondition) continue;

            if (condition.supplementaryConditions.Count < 1)
            {
                if (!validateCondition)
                    continue;
                else
                    return validateCondition;
            }
            
            bool supplementaryCondition = true;
            foreach (var cond in condition.supplementaryConditions)
            {
                supplementaryCondition = CheckJob(crew[i].GetJob(), cond.Value.Traits.GetJob());
                if (!supplementaryCondition)
                    break;
                supplementaryCondition = CheckPositiveTraits(crew[i].GetPositiveTraits(),
                    cond.Value.Traits.GetPositiveTraits());
                if (!supplementaryCondition)
                    break;
                supplementaryCondition = CheckNegativeTraits(crew[i].GetNegativeTraits(),
                    cond.Value.Traits.GetNegativeTraits());
                if (!supplementaryCondition)
                    break;
                supplementaryCondition = CheckSpaceshipTraits(spaceshipTraits, hiddenSpaceshipTraits,
                    cond.Value.SpaceshipTraits,
                    cond.Value.HiddenSpaceshipTraits);
                if (!supplementaryCondition)
                    break;
                
                //Apply logical operator
                switch (cond.Key)
                {
                    case TraitsData.TraitOperator.OR:
                        validateCondition |= supplementaryCondition;
                        break;
                    case TraitsData.TraitOperator.AND:
                        validateCondition &= supplementaryCondition;
                        break;
                    case TraitsData.TraitOperator.XOR:
                        validateCondition ^= supplementaryCondition;
                        break;
                    case TraitsData.TraitOperator.NAND:
                        validateCondition = !(validateCondition && supplementaryCondition);
                        break;
                    case TraitsData.TraitOperator.NOT:
                        validateCondition = (validateCondition & !supplementaryCondition);
                        break;
                }
            }

            if (validateCondition)
                break;
        }

        return validateCondition;
    }
    
    private static bool CheckJob(TraitsData.Job job, TraitsData.Job conditionJob)
    {
        return (job & conditionJob) == conditionJob;
    }

    private static bool CheckPositiveTraits(TraitsData.PositiveTraits pTraits,
        TraitsData.PositiveTraits conditionPTraits)
    {
        return (pTraits & conditionPTraits) == conditionPTraits;
    }

    private static bool CheckNegativeTraits(TraitsData.NegativeTraits negativeTraits,
        TraitsData.NegativeTraits conditionNtTraits)
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