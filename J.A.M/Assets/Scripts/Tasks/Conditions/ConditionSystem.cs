using System.Linq;
using CharacterSystem;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ConditionSystem
{
    // Call if Target is Leader or Assistant
    public static bool CheckCharacterCondition(CharacterBehaviour leaderCharacter, ConditionSO condition)
    {
        if (condition.BaseCondition.target == OutcomeData.OutcomeTarget.None) return true;
        if (leaderCharacter == null) return false;

        var ssTraits = GameManager.Instance.SpaceshipManager.SpaceshipTraits;
        var hssTraits = GameManager.Instance.SpaceshipManager.HiddenSpaceshipTraits;
        
        bool validateCondition;

        /*Base Condition Check*/

        var characterTraits = leaderCharacter.GetTraits();

        if (condition.targetStat != OutcomeData.OutcomeTargetStat.None)
        {
            validateCondition = leaderCharacter.CheckStat(condition.statThreshold, condition.targetStat,
                condition.statComparison);

            if (!validateCondition) return false;
        }

        validateCondition = CheckJob(characterTraits.GetJob(), condition.BaseCondition.Traits.GetJob())
                            && CheckPositiveTraits(characterTraits.GetPositiveTraits(),
                                condition.BaseCondition.Traits.GetPositiveTraits())
                            && CheckNegativeTraits(characterTraits.GetNegativeTraits(),
                                condition.BaseCondition.Traits.GetNegativeTraits())
                            && CheckSpaceshipTraits(ssTraits, hssTraits, condition.BaseCondition.SpaceshipTraits,
                                condition.BaseCondition.HiddenSpaceshipTraits);

        if (condition.supplementaryConditions.Count < 1)
            return validateCondition;

        return CheckSupplementaryCondition(condition, leaderCharacter, validateCondition);
    }

    //Call if target is Crew
    public static bool CheckCrewCondition(ConditionSO condition)
    {
        if (condition.BaseCondition.target == OutcomeData.OutcomeTarget.None) return true;

        var ssTraits = GameManager.Instance.SpaceshipManager.SpaceshipTraits;
        var hssTraits = GameManager.Instance.SpaceshipManager.HiddenSpaceshipTraits;

        if (!CheckSpaceshipTraits(ssTraits, hssTraits,
                condition.BaseCondition.SpaceshipTraits,
                condition.BaseCondition.HiddenSpaceshipTraits))
            return false;

        var crew = GameManager.Instance.SpaceshipManager.characters;
        
        for (int i = 0; i < crew.Length; i++)
        {
            //If conditions are false, skip to next character
            if (!CheckJob(crew[i].GetJob(), condition.BaseCondition.Traits.GetJob())) continue;

            if (!CheckPositiveTraits(crew[i].GetPositiveTraits(),
                    condition.BaseCondition.Traits.GetPositiveTraits())) continue;

            if (!CheckNegativeTraits(crew[i].GetNegativeTraits(),
                    condition.BaseCondition.Traits.GetNegativeTraits())) continue;

            if (condition.supplementaryConditions.Count < 1)
                return true;

            if (CheckSupplementaryCondition(condition, crew[i], true))
                return true;
        }
        return false;
    }

    //Call if target is Spaceship
    public static bool CheckSpaceshipCondition(ConditionSO condition)
    {
        if (condition.BaseCondition.target == OutcomeData.OutcomeTarget.None) return true;

        var ssTraits = GameManager.Instance.SpaceshipManager.SpaceshipTraits;
        var hssTraits = GameManager.Instance.SpaceshipManager.HiddenSpaceshipTraits;

        bool validateCondition = CheckSpaceshipTraits(ssTraits, hssTraits, condition.BaseCondition.SpaceshipTraits,
            condition.BaseCondition.HiddenSpaceshipTraits);

        if (condition.supplementaryConditions.Count < 1)
            return validateCondition;

        return CheckSupplementaryCondition(condition, null, validateCondition);
    }

    //Call if target is Gauge
    public static bool CheckGaugeCondition(ConditionSO condition)
    {
        if (condition.BaseCondition.target == OutcomeData.OutcomeTarget.None) return true;

        var ssTraits = GameManager.Instance.SpaceshipManager.SpaceshipTraits;
        var hssTraits = GameManager.Instance.SpaceshipManager.HiddenSpaceshipTraits;

        bool validateCondition = CheckSpaceshipTraits(ssTraits, hssTraits, condition.BaseCondition.SpaceshipTraits,
            condition.BaseCondition.HiddenSpaceshipTraits);

        validateCondition = GaugeCheck.GetGaugeValue(condition.targetGauge) == condition.BaseCondition.GaugeValue;

        if (condition.supplementaryConditions.Count < 1)
            return validateCondition;

        return CheckSupplementaryCondition(condition, null, validateCondition);
    }

    public static bool CheckGaugeValueCondition(ConditionSO condition)
    {
        if (condition.BaseCondition.target == OutcomeData.OutcomeTarget.None) return true;

        var ssTraits = TraitsData.SpaceshipTraits.None;
        var hssTraits = TraitsData.HiddenSpaceshipTraits.None;

        bool validateCondition = CheckSpaceshipTraits(ssTraits, hssTraits, condition.BaseCondition.SpaceshipTraits,
            condition.BaseCondition.HiddenSpaceshipTraits);

        validateCondition =
            GaugeCheck.CheckGaugeValue(condition.BaseCondition.GaugeValueToCheck, condition.targetGauge);

        if (condition.supplementaryConditions.Count < 1)
            return validateCondition;

        return CheckSupplementaryCondition(condition, null, validateCondition);
    }

    private static bool CheckSupplementaryCondition(ConditionSO taskCondition,CharacterBehaviour leaderCharacter, bool validateCondition)
    {
        bool supplementaryCondition = true;

        for (int i = 0; i < taskCondition.supplementaryConditions.Count; i++)
        {
            var cond = taskCondition.supplementaryConditions.ElementAt(i);
            switch (cond.Value.BaseCondition.target)
            {
                case OutcomeData.OutcomeTarget.Leader:
                    supplementaryCondition = CheckCharacterCondition(leaderCharacter, cond.Value);
                    break;

                case OutcomeData.OutcomeTarget.Assistant:
                    supplementaryCondition = CheckCharacterCondition(leaderCharacter, cond.Value);
                    break;

                case OutcomeData.OutcomeTarget.Crew:
                    supplementaryCondition = CheckCrewCondition(cond.Value);
                    break;

                case OutcomeData.OutcomeTarget.Ship:
                    supplementaryCondition = CheckSpaceshipCondition(cond.Value);
                    break;

                case OutcomeData.OutcomeTarget.Gauge:
                    supplementaryCondition = CheckGaugeCondition(cond.Value);
                    break;

                case OutcomeData.OutcomeTarget.GaugeValue:
                    supplementaryCondition = CheckGaugeValueCondition(cond.Value);
                    break;

                case OutcomeData.OutcomeTarget.None:
                    break;
            }

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
    
    public static bool RouteCondition(OutcomeData.OutcomeTarget target, UI.Notification notification)
    {
        bool validateCondition = false;
        switch (target)
        {
            case OutcomeData.OutcomeTarget.Leader:
                validateCondition =
                    CheckCharacterCondition(notification.LeaderCharacters[0], notification.taskCondition);
                break;
            case OutcomeData.OutcomeTarget.Assistant:
                if (notification.AssistantCharacters.Count >= 1)
                    validateCondition =
                        CheckCharacterCondition(notification.AssistantCharacters[0], notification.taskCondition);
                break;
            case OutcomeData.OutcomeTarget.Gauge:
                validateCondition = CheckGaugeCondition(notification.taskCondition);
                break;
            case OutcomeData.OutcomeTarget.GaugeValue:
                validateCondition = CheckGaugeValueCondition(notification.taskCondition);
                break;
            case OutcomeData.OutcomeTarget.Crew:
                validateCondition = CheckCrewCondition(notification.taskCondition);
                break;
            case OutcomeData.OutcomeTarget.Ship:
                validateCondition = CheckSpaceshipCondition(notification.taskCondition);
                break;
            case OutcomeData.OutcomeTarget.None:
                validateCondition = true;
                break;
        }

        return validateCondition;
    }

    private static bool CheckJob(TraitsData.Job job, TraitsData.Job conditionJob)
    {
        return job.HasFlag(conditionJob);
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

    public enum ComparisonOperator
    {
        Less,
        Equal,
        Higher,
        LessOrEqual,
        HigherOrEqual,
    }
}