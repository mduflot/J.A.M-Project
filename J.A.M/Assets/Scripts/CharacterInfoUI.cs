using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfoUI : MonoBehaviour
{
    [SerializeField] private Image icon;

    [SerializeField] private TextMeshProUGUI name;

    [SerializeField] private Transform traitsParent;

    [SerializeField] private TraitHoverable traitPrefab;
    private List<TraitHoverable> characterTraits = new();


    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void SetupCharacterInfo(CharacterDataScriptable c)
    {
        icon.sprite = c.characterIcon;
        name.text = c.firstName + " " + c.lastName;
        foreach (var characterTrait in characterTraits)
        {
            Destroy(characterTrait.gameObject);
        }
        characterTraits.Clear();
        SetupTraits(c);
        gameObject.SetActive(true);
    }

    public void ClearCharacterInfo()
    {
        icon.sprite = null;
        name.text = null;
        gameObject.SetActive(false);
    }

    private void SetupTraits(CharacterDataScriptable c)
    {
        foreach (TraitsData.Job trait in Enum.GetValues(typeof(TraitsData.Job)))
        {
            if (trait == TraitsData.Job.None) continue;
            if (trait == TraitsData.Job.Lammy) continue;
            if (trait == TraitsData.Job.Elrenda) continue;
            if (trait == TraitsData.Job.Leisin) continue;
            if (trait == TraitsData.Job.Malda) continue;
            if (trait == TraitsData.Job.Seltis) continue;
            if (trait == TraitsData.Job.Varus) continue;
            if (c.traits.GetJob().HasFlag(trait))
            {
                var traitHoverable = Instantiate(traitPrefab, traitsParent.transform);
                traitHoverable.Initialize(trait.ToString());
                characterTraits.Add(traitHoverable);
            }
        }
        
        foreach (TraitsData.PositiveTraits trait in Enum.GetValues(typeof(TraitsData.PositiveTraits)))
        {
            if (trait == TraitsData.PositiveTraits.None) continue;
            if (c.traits.GetPositiveTraits().HasFlag(trait))
            {
                var traitHoverable = Instantiate(traitPrefab, traitsParent.transform);
                traitHoverable.Initialize(trait.ToString());
                characterTraits.Add(traitHoverable);
            }
        }
        
        foreach (TraitsData.NegativeTraits trait in Enum.GetValues(typeof(TraitsData.NegativeTraits)))
        {
            if (trait == TraitsData.NegativeTraits.None) continue;
            if (c.traits.GetNegativeTraits().HasFlag(trait))
            {
                var traitHoverable = Instantiate(traitPrefab, traitsParent.transform);
                traitHoverable.Initialize(trait.ToString());
                characterTraits.Add(traitHoverable);
            }
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(traitsParent.GetComponent<RectTransform>());
        
    }
}
