using System.Linq;
using CharacterSystem;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class ConditionSystem
{
    // Call if Target is Leader or Assistant
    public static bool CheckCharacterCondition(CharacterBehaviour leaderCharacter, CharacterBehaviour[] assistantCharacters, ConditionSO taskCondition)
    {
        if (taskCondition.BaseCondition.target == OutcomeData.OutcomeTarget.None) return true;
        
        var ssTraits = GameManager.Instance.SpaceshipManager.SpaceshipTraits;
        var hssTraits = GameManager.Instance.SpaceshipManager.HiddenSpaceshipTraits;
        bool validateCondition = true;

        if (!CheckSpaceshipTraits(ssTraits, hssTraits, taskCondition.BaseCondition.SpaceshipTraits,
                taskCondition.BaseCondition.HiddenSpaceshipTraits))
            return false;
        
        /*Base Condition Check*/

        var characterTraits = leaderCharacter.GetTraits();
        
        if (taskCondition.targetStat != OutcomeData.OutcomeTargetStat.None)
        {
            validateCondition = leaderCharacter.CheckStat(taskCondition.statThreshold, taskCondition.targetStat,
                taskCondition.statComparison);
        }
        
        if (!CheckJob(characterTraits.GetJob(), taskCondition.BaseCondition.Traits.GetJob()))
            validateCondition = false;

        if (!CheckPositiveTraits(characterTraits.GetPositiveTraits(),
                taskCondition.BaseCondition.Traits.GetPositiveTraits()))
            validateCondition = false;

        if (!CheckNegativeTraits(characterTraits.GetNegativeTraits(),
                taskCondition.BaseCondition.Traits.GetNegativeTraits()))
            validateCondition = false;

        if (taskCondition.supplementaryConditions.Count < 1)
            return validateCondition;

        /*Additionnal Condition Check*/

        bool supplementaryCondition = true;
        
        for(int i = 0; i < taskCondition.supplementaryConditions.Count; i++)
        {
            var cond = taskCondition.supplementaryConditions.ElementAt(i);
            switch (cond.Value.BaseCondition.target)
            {
                case OutcomeData.OutcomeTarget.Leader:
                    supplementaryCondition = CheckCharacterCondition(leaderCharacter, assistantCharacters, cond.Value);
                    break;
                
                case OutcomeData.OutcomeTarget.Assistant:
                    supplementaryCondition = CheckCharacterCondition(leaderCharacter, assistantCharacters, cond.Value);
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

    //Call if target is Crew
    public static bool CheckCrewCondition(ConditionSO condition)
    {
        if (condition.BaseCondition.target == OutcomeData.OutcomeTarget.None) return true;
        
        var crew = GameManager.Instance.SpaceshipManager.characters;
        var ssTraits = GameManager.Instance.SpaceshipManager.SpaceshipTraits;
        var hssTraits = GameManager.Instance.SpaceshipManager.HiddenSpaceshipTraits;
        
        bool validateCondition = CheckSpaceshipTraits(ssTraits, hssTraits,
            condition.BaseCondition.SpaceshipTraits,
            condition.BaseCondition.HiddenSpaceshipTraits);

        if (!validateCondition)
            return false;
        
        //Assume next conditions will be false
        validateCondition = false;
        
        for (int i = 0; i < crew.Length; i++)
        {
            //If conditions are false, skip to next character
            if (!CheckJob(crew[i].GetJob(), condition.BaseCondition.Traits.GetJob())) continue;
            
            if (!CheckPositiveTraits(crew[i].GetPositiveTraits(), condition.BaseCondition.Traits.GetPositiveTraits())) continue;

            if (!CheckNegativeTraits(crew[i].GetNegativeTraits(), condition.BaseCondition.Traits.GetNegativeTraits())) continue;
            
            if (condition.supplementaryConditions.Count < 1)
                return true;

            validateCondition = true;
            
            bool supplementaryCondition = true;
            
            for(int i2 = 0; i2 < condition.supplementaryConditions.Count; i2++)
            {
                var cond = condition.supplementaryConditions.ElementAt(i2);
                switch (cond.Value.BaseCondition.target)
                {
                    case OutcomeData.OutcomeTarget.Leader:
                        //supplementaryCondition = CheckCharacterCondition()
                        break;
                
                    case OutcomeData.OutcomeTarget.Assistant:
                        //supplementaryCondition = CheckCharacterCondition()
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

            if (validateCondition)
                return true;
        }

        return false;
    }

    //Call if target is Spaceship
    public static bool CheckSpaceshipCondition(ConditionSO condition)
    {
        if (condition.BaseCondition.target == OutcomeData.OutcomeTarget.None) return true;
        
        var ssTraits = TraitsData.SpaceshipTraits.None;
        var hssTraits = TraitsData.HiddenSpaceshipTraits.None;

        bool validateCondition = CheckSpaceshipTraits(ssTraits, hssTraits, condition.BaseCondition.SpaceshipTraits,
            condition.BaseCondition.HiddenSpaceshipTraits);

        if (condition.supplementaryConditions.Count < 1) 
            return validateCondition;

        bool supplementaryCondition = true;
        
        for (int i = 0; i < condition.supplementaryConditions.Count; i++)
        {
            var cond = condition.supplementaryConditions.ElementAt(i);
            
            switch (cond.Value.BaseCondition.target)
            {
                case OutcomeData.OutcomeTarget.Leader:
                    //supplementaryCondition = CheckCharacterCondition()
                    break;
                
                case OutcomeData.OutcomeTarget.Assistant:
                    //supplementaryCondition = CheckCharacterCondition()
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
    
    //Call if target is Gauge
    public static bool CheckGaugeCondition(ConditionSO condition)
    {
        if (condition.BaseCondition.target == OutcomeData.OutcomeTarget.None) return true;
        
        var ssTraits = TraitsData.SpaceshipTraits.None;
        var hssTraits = TraitsData.HiddenSpaceshipTraits.None;

        bool validateCondition = CheckSpaceshipTraits(ssTraits, hssTraits, condition.BaseCondition.SpaceshipTraits,
            condition.BaseCondition.HiddenSpaceshipTraits);

        validateCondition = GaugeCheck.GetGaugeValue(condition.targetGauge) == condition.BaseCondition.GaugeValue;
     
        if (condition.supplementaryConditions.Count < 1) 
            return validateCondition;

        bool supplementaryCondition = true;
        
        for (int i = 0; i < condition.supplementaryConditions.Count(); i++)
        {
            var cond = condition.supplementaryConditions.ElementAt(i);
            switch (cond.Value.BaseCondition.target)
            {
                case OutcomeData.OutcomeTarget.Leader:
                    //supplementaryCondition = CheckCharacterCondition()
                    break;
                
                case OutcomeData.OutcomeTarget.Assistant:
                    //supplementaryCondition = CheckCharacterCondition()
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

    public static bool CheckAdditionnalConditions(CharacterBehaviour leaderCharacter, CharacterBehaviour[] assistantCharacters, ConditionSO condition, int additionnalConditionIndex)
    {
        if(condition.BaseCondition.target == OutcomeData.OutcomeTarget.None) return true;
        switch (condition.additionnalConditions[additionnalConditionIndex].BaseCondition.target)
        {
            case OutcomeData.OutcomeTarget.Leader:
                return CheckCharacterCondition(leaderCharacter, assistantCharacters, condition);
                
            case OutcomeData.OutcomeTarget.Assistant:
                return CheckCharacterCondition(leaderCharacter, assistantCharacters, condition);
                
            case OutcomeData.OutcomeTarget.Crew:
                return CheckCrewCondition(condition);
                
            case OutcomeData.OutcomeTarget.Ship:
                return CheckSpaceshipCondition(condition);
                
            case OutcomeData.OutcomeTarget.Gauge:
                return CheckGaugeCondition(condition);
            
            default:
                Debug.LogError("Target mismatch ! Condition cannot be verified.");
                return false;
        }
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
    
    public enum ComparisonOperator
    {
        Less,
        Equal,
        Higher,
        LessOrEqual,
        HigherOrEqual,
    }
}