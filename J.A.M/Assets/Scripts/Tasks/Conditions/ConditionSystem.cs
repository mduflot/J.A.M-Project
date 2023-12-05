using System.Linq;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class ConditionSystem
{
    // Call if Target is Leader or Assistant
    public static bool CheckCharacterCondition(TraitsData.Traits characterTraits, ConditionSO taskCondition)
    {
        if (taskCondition.target == OutcomeData.OutcomeTarget.None) return true;
        
        var ssTraits = GameManager.Instance.SpaceshipManager.SpaceshipTraits;
        var hssTraits = GameManager.Instance.SpaceshipManager.HiddenSpaceshipTraits;
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

        if (!CheckSpaceshipTraits(ssTraits, hssTraits, taskCondition.BaseCondition.SpaceshipTraits,
                taskCondition.BaseCondition.HiddenSpaceshipTraits))
            validateCondition = false;

        if (taskCondition.supplementaryConditions.Count < 1)
            return validateCondition;

        /*Additionnal Condition Check*/

        bool supplementaryCondition = true;
        
        for(int i = 0; i < taskCondition.supplementaryConditions.Count; i++)
        {
            var cond = taskCondition.supplementaryConditions.ElementAt(i);
            if (!CheckJob(characterTraits.GetJob(), cond.Value.Traits.GetJob()))
                supplementaryCondition = false;

            if (!CheckPositiveTraits(characterTraits.GetPositiveTraits(),
                    cond.Value.Traits.GetPositiveTraits()))
                supplementaryCondition = false;

            if (!CheckNegativeTraits(characterTraits.GetNegativeTraits(),
                    cond.Value.Traits.GetNegativeTraits()))
                supplementaryCondition = false;

            if (!CheckSpaceshipTraits(ssTraits, hssTraits,
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

    //Call if target is Crew
    public static bool CheckCrewCondition(ConditionSO condition)
    {
        if (condition.target == OutcomeData.OutcomeTarget.None) return true;
        
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
            
            for(int i2 = 0; i2 < condition.supplementaryConditions.Count; i2++)
            {
                var cond = condition.supplementaryConditions.ElementAt(i2);
                
                bool supplementaryCondition = CheckJob(crew[i].GetJob(), cond.Value.Traits.GetJob());
                
                supplementaryCondition &= CheckPositiveTraits(crew[i].GetPositiveTraits(),
                    cond.Value.Traits.GetPositiveTraits());
                
                supplementaryCondition &= CheckNegativeTraits(crew[i].GetNegativeTraits(),
                    cond.Value.Traits.GetNegativeTraits());
                
                supplementaryCondition &= CheckSpaceshipTraits(ssTraits, hssTraits,
                    cond.Value.SpaceshipTraits,
                    cond.Value.HiddenSpaceshipTraits);
                
                
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
        if (condition.target == OutcomeData.OutcomeTarget.None) return true;
        
        var ssTraits = TraitsData.SpaceshipTraits.None;
        var hssTraits = TraitsData.HiddenSpaceshipTraits.None;

        bool validateCondition = CheckSpaceshipTraits(ssTraits, hssTraits, condition.BaseCondition.SpaceshipTraits,
            condition.BaseCondition.HiddenSpaceshipTraits);

        if (condition.supplementaryConditions.Count < 1) 
            return validateCondition;
        
        
        for (int i = 0; i < condition.supplementaryConditions.Count; i++)
        {
            var cond = condition.supplementaryConditions.ElementAt(i);
            bool supplementaryCondition = CheckSpaceshipTraits(ssTraits, hssTraits,
                cond.Value.SpaceshipTraits,
                cond.Value.HiddenSpaceshipTraits);
                
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
        if (condition.target == OutcomeData.OutcomeTarget.None) return true;
        
        var ssTraits = TraitsData.SpaceshipTraits.None;
        var hssTraits = TraitsData.HiddenSpaceshipTraits.None;

        bool validateCondition = CheckSpaceshipTraits(ssTraits, hssTraits, condition.BaseCondition.SpaceshipTraits,
            condition.BaseCondition.HiddenSpaceshipTraits);

        validateCondition = GaugeCheck.GetGaugeValue(condition.targetGauge) == condition.BaseCondition.GaugeValue;
     
        if (condition.supplementaryConditions.Count < 1) 
            return validateCondition;

        for (int i = 0; i < condition.supplementaryConditions.Count(); i++)
        {
            var cond = condition.supplementaryConditions.ElementAt(i);
            bool supplementaryCondition = GaugeCheck.GetGaugeValue(condition.targetGauge) == cond.Value.GaugeValue;
                
            supplementaryCondition = CheckSpaceshipTraits(ssTraits, hssTraits,
                cond.Value.SpaceshipTraits,
                cond.Value.HiddenSpaceshipTraits);
                
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

    public static bool CheckAdditionnalConditions(ConditionSO condition, int additionnalConditionIndex, CharacterBehaviour potentialTarget = null)
    {
        if(condition.target == OutcomeData.OutcomeTarget.None) return true;
        switch (condition.additionnalConditions[additionnalConditionIndex].target)
        {
            case OutcomeData.OutcomeTarget.Leader:
                return CheckCharacterCondition(potentialTarget.GetTraits(), condition);
                
            case OutcomeData.OutcomeTarget.Assistant:
                return CheckCharacterCondition(potentialTarget.GetTraits(), condition);
                
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
}