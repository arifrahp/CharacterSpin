using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinManager : MonoBehaviour
{
    public List<Spinner> characterToSpin;

    [System.Serializable]
    public class Spinner
    {
        public int rarity; // 1 = common, 2 = rare, 3 = epic, 4 = legendary
        public float weight;
        public float minRange;
        public float maxRange;
    }

    public List<CharacterClass> characterClassesCommon = new List<CharacterClass>();
    public List<CharacterClass> characterClassesRare = new List<CharacterClass>();
    public List<CharacterClass> characterClassesEpic = new List<CharacterClass>();
    public List<CharacterClass> characterClassesLegendary = new List<CharacterClass>();

    // This function can be triggered by a button click
    public void TriggerSpin()
    {
        Spinner chosenSpinner = ChooseSpinnerByWeight();
        if (chosenSpinner != null)
        {
            // Depending on the rarity, call the appropriate function to select a character
            switch (chosenSpinner.rarity)
            {
                case 1:
                    SelectRandomCharacter(characterClassesCommon);
                    break;
                case 2:
                    SelectRandomCharacter(characterClassesRare);
                    break;
                case 3:
                    SelectRandomCharacter(characterClassesEpic);
                    break;
                case 4:
                    SelectRandomCharacter(characterClassesLegendary);
                    break;
                default:
                    Debug.LogError("Invalid rarity");
                    break;
            }
        }
    }

    // Function to choose a Spinner based on weight
    Spinner ChooseSpinnerByWeight()
    {
        float totalWeight = 0f;
        // Calculate the total weight
        foreach (Spinner spinner in characterToSpin)
        {
            totalWeight += spinner.weight;
        }

        // Get a random value between 0 and total weight
        float randomValue = Random.Range(0, totalWeight);

        // Iterate over spinners and check which one falls in the range
        float cumulativeWeight = 0f;
        foreach (Spinner spinner in characterToSpin)
        {
            cumulativeWeight += spinner.weight;
            if (randomValue < cumulativeWeight)
            {
                return spinner; // Return the spinner whose weight range matches the random value
            }
        }

        return null; // Fallback in case something goes wrong
    }

    // Function to select a random character from a list and unlock it
    void SelectRandomCharacter(List<CharacterClass> characterList)
    {
        // Filter out characters that are already unlocked
        List<CharacterClass> lockedCharacters = GetLockedCharacters(characterList);

        if (lockedCharacters.Count > 0)
        {
            int randomIndex = Random.Range(0, lockedCharacters.Count);
            CharacterClass selectedCharacter = lockedCharacters[randomIndex];

            // Unlock the chosen character
            selectedCharacter.isUnlocked = true;

            Debug.Log("Selected character: " + selectedCharacter.name + " is now unlocked.");
        }
        else
        {
            Debug.LogError("No locked characters available for this rarity.");
        }
    }

    // Function to get only the characters that are still locked (isUnlocked == false)
    List<CharacterClass> GetLockedCharacters(List<CharacterClass> characterList)
    {
        List<CharacterClass> lockedCharacters = new List<CharacterClass>();

        foreach (CharacterClass character in characterList)
        {
            if (!character.isUnlocked)
            {
                lockedCharacters.Add(character);
            }
        }

        return lockedCharacters;
    }
}
