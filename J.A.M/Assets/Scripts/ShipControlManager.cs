using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShipControlManager : MonoBehaviour
{
    [Header("Window")]
    [SerializeField] private Image mobiusButton;
    [SerializeField] private Image crewButton;
    [SerializeField] private GameObject mobiusContainer;
    [SerializeField] private GameObject crewContainer;

    [Header("Mobius")]
    [SerializeField] private GameObject shipTraitsContainer;
    
    [Header("Crew")]
    [SerializeField] private GameObject characterProfilePrefab;
    [SerializeField] private GameObject characterProfileParent;
    [SerializeField] private List<CharacterProfile> characterProfiles = new();
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private GameObject characterTraitsContainer;
    [SerializeField] private Image characterImage;
    
    [Header("Traits")]
    [SerializeField] private TraitHoverable traitPrefab;
    [SerializeField] private GameObject shipTraitParent;
    [SerializeField] private GameObject characterTraitParent;
    private List<TraitHoverable> characterTraits = new();
    [SerializeField] private TraitsHoverMenu traitsHoverMenu;
    
    [Header("Layout Elements")]
    [SerializeField] private GameObject descriptionParent;

    public void Initialize()
    {
        foreach (TraitsData.SpaceshipTraits trait in Enum.GetValues(typeof(TraitsData.SpaceshipTraits)))
        {
            if (trait == TraitsData.SpaceshipTraits.None) continue;
            if (GameManager.Instance.SpaceshipManager.SpaceshipTraits.HasFlag(trait))
            {
                var traitHoverable = Instantiate(traitPrefab, shipTraitParent.transform);
                traitHoverable.Initialize(trait.ToString());
                traitHoverable.hoverMenu = traitsHoverMenu;
                traitHoverable.data = new HoverMenuData
                {
                    text1 = trait.ToString(),
                    text2 = GameManager.Instance.SpaceshipManager.spaceshipTraitsDescription.ContainsKey(trait) ? GameManager.Instance.SpaceshipManager.spaceshipTraitsDescription[trait] : "",
                    baseParent = transform.parent,
                    parent = transform
                };
            }
        }
        
        for (int index = 0; index < GameManager.Instance.SpaceshipManager.characters.Length; index++)
        {
            var character = GameManager.Instance.SpaceshipManager.characters[index];
            if (index == 0) DisplayCharacterInfo(character.GetCharacterData());
            var profile = Instantiate(characterProfilePrefab, characterProfileParent.transform);
            profile.GetComponent<CharacterProfile>().Initialize(character.GetCharacterData(), character.GetCharacterData().characterIcon, character.GetCharacterData().firstName, this);
            characterProfiles.Add(profile.GetComponent<CharacterProfile>());
        }
        
        OpenMobius();
    }
    
    public void OpenMobius()
    {
        mobiusButton.color = Color.white;
        crewButton.color = Color.black;
        crewContainer.SetActive(false);
        mobiusContainer.SetActive(true);
    }

    public void OpenCrew()
    {
        mobiusButton.color = Color.black;
        crewButton.color = Color.white;
        mobiusContainer.SetActive(false);
        crewContainer.SetActive(true);
    }
    
    public void DisplayCharacterInfo(CharacterDataScriptable character)
    {
        foreach (var trait in characterTraits)
        {
            Destroy(trait.gameObject);
        }
        characterTraits.Clear();
        nameText.text = character.firstName + " " + character.lastName;
        descriptionText.text = character.description;
        characterImage.sprite = character.characterBody;
        characterImage.preserveAspect = true;
        
        foreach (TraitsData.Job trait in Enum.GetValues(typeof(TraitsData.Job)))
        {
            if (trait == TraitsData.Job.None) continue;
            if (character.traits.GetJob().HasFlag(trait))
            {
                var traitHoverable = Instantiate(traitPrefab, characterTraitParent.transform);
                traitHoverable.Initialize(trait.ToString());
                traitHoverable.hoverMenu = traitsHoverMenu;
                traitHoverable.data = new HoverMenuData
                {
                    text1 = trait.ToString(),
                    text2 = GameManager.Instance.SpaceshipManager.jobDescription.ContainsKey(trait) ? GameManager.Instance.SpaceshipManager.jobDescription[trait] : "",
                    baseParent = transform.parent,
                    parent = transform
                };
                characterTraits.Add(traitHoverable);
            }
        }
        
        foreach (TraitsData.PositiveTraits trait in Enum.GetValues(typeof(TraitsData.PositiveTraits)))
        {
            if (trait == TraitsData.PositiveTraits.None) continue;
            if (character.traits.GetPositiveTraits().HasFlag(trait))
            {
                var traitHoverable = Instantiate(traitPrefab, characterTraitParent.transform);
                traitHoverable.Initialize(trait.ToString());
                traitHoverable.hoverMenu = traitsHoverMenu;
                traitHoverable.data = new HoverMenuData
                {
                    text1 = trait.ToString(),
                    text2 = GameManager.Instance.SpaceshipManager.positiveTraitsDescription.ContainsKey(trait) ? GameManager.Instance.SpaceshipManager.positiveTraitsDescription[trait] : "",
                    baseParent = transform.parent,
                    parent = transform
                };
                characterTraits.Add(traitHoverable);
            }
        }
        
        foreach (TraitsData.NegativeTraits trait in Enum.GetValues(typeof(TraitsData.NegativeTraits)))
        {
            if (trait == TraitsData.NegativeTraits.None) continue;
            if (character.traits.GetNegativeTraits().HasFlag(trait))
            {
                var traitHoverable = Instantiate(traitPrefab, characterTraitParent.transform);
                traitHoverable.Initialize(trait.ToString());
                traitHoverable.hoverMenu = traitsHoverMenu;
                traitHoverable.data = new HoverMenuData
                {
                    text1 = trait.ToString(),
                    text2 = GameManager.Instance.SpaceshipManager.negativeTraitsDescription.ContainsKey(trait) ? GameManager.Instance.SpaceshipManager.negativeTraitsDescription[trait] : "",
                    baseParent = transform.parent,
                    parent = transform
                };
                characterTraits.Add(traitHoverable);
            }
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(descriptionParent.GetComponent<RectTransform>());
    }
}
