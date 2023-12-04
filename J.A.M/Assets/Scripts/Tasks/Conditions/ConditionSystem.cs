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
        foreach (var cond in taskCondition.supplementaryConditions)
        {
            if (!CheckJob(characterTraits.GetJob(), taskCondition.BaseCondition.Traits.GetJob()))
                supplementaryCondition = false;

            if (!CheckPositiveTraits(characterTraits.GetPositiveTraits(),
                    taskCondition.BaseCondition.Traits.GetPositiveTraits()))
                supplementaryCondition = false;

            if (!CheckNegativeTraits(characterTraits.GetNegativeTraits(),
                    taskCondition.BaseCondition.Traits.GetNegativeTraits()))
                supplementaryCondition = false;

            if (!CheckSpaceshipTraits(spaceshipTraits, hiddenSpaceshipTraits,
                    taskCondition.BaseCondition.SpaceshipTraits,
                    taskCondition.BaseCondition.HiddenSpaceshipTraits))
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
                    validateCondition = (validateCondition != supplementaryCondition);
                    break;
            }
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