using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShipControlManager : MonoBehaviour
{
    [Header("Window")]
    [SerializeField] private Button mobiusButton;
    [SerializeField] private Button crewButton;
    [SerializeField] private GameObject mobiusContainer;
    [SerializeField] private GameObject crewContainer;

    [Header("Mobius")]
    [SerializeField] private GameObject shipTraitsContainer;
    
    [Header("Crew")]
    [SerializeField] private GameObject characterProfilePrefab;
    [SerializeField] private GameObject characterProfileParent;
    [SerializeField] private List<CharacterProfile> characterProfiles;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private GameObject characterTraitsContainer;
    [SerializeField] private Image characterImage;
    
    [Header("Traits")]
    [SerializeField] private GameObject traitPrefab;
    [SerializeField] private GameObject shipTraitParent;
    [SerializeField] private GameObject characterTraitParent;

    public void Initialize()
    {
        // TODO - Instantiate all ship traits
        
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
        mobiusButton.interactable = false;
        crewButton.interactable = true;
        crewContainer.SetActive(false);
        mobiusContainer.SetActive(true);
    }

    public void OpenCrew()
    {
        mobiusButton.interactable = true;
        crewButton.interactable = false;
        mobiusContainer.SetActive(false);
        crewContainer.SetActive(true);
    }
    
    public void DisplayCharacterInfo(CharacterDataScriptable character)
    {
        nameText.text = character.firstName + " " + character.lastName;
        descriptionText.text = character.description;
        characterImage.sprite = character.characterIcon;
        characterImage.preserveAspect = true;
        // TODO - Instantiate all character traits
    }
}
