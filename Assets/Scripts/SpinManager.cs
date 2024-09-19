using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpinManager : MonoBehaviour
{
    public int gold;
    public TMP_Text goldText;
    public int buyPrice;
    public TMP_Text buyText;
    public Image spinWheel;
    public TMP_Text resultText;
    public GameObject resultPanel;
    public Image resultImage;
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

    private float spinDuration = 3f; // Duration of the spin animation
    private float decelerationDuration = 1f; // Duration of the deceleration phase

    private void Start()
    {
        buyText.text = "BUY " + buyPrice + " GOLD/SPIN"; 
    }
    private void Update()
    {
        goldText.text = gold.ToString();
    }

    // This function can be triggered by a button click
    public void TriggerSpin()
    {
        if(gold > buyPrice)
        {
            gold -= buyPrice;
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

                // Calculate the stopping point on the wheel based on rarity
                float stopAngle = GetRandomStopAngleForRarity(chosenSpinner);
                StartCoroutine(SpinWheel(stopAngle));
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
        if (characterList.Count > 0)
        {
            int randomIndex = Random.Range(0, characterList.Count);
            CharacterClass selectedCharacter = characterList[randomIndex];

            if (selectedCharacter.isUnlocked)
            {
                resultImage.sprite = selectedCharacter.characterImage.sprite;
                selectedCharacter.ChoosenAlreadyUnlocked();
                resultText.text = "Unlucky, you get character you've already unlocked, try again!";
            }
            else if (!selectedCharacter.isUnlocked)
            {
                resultImage.sprite = selectedCharacter.characterImage.sprite;
                selectedCharacter.isUnlocked = true;
                selectedCharacter.FirstUnlocked();
                resultText.text = "Selected character: " + selectedCharacter.name + " is now unlocked.";
            }
        }
        else
        {
            Debug.LogError("Character list is empty for this rarity.");
        }
    }

    // Function to get a random stopping angle for the wheel based on rarity
    float GetRandomStopAngleForRarity(Spinner spinner)
    {
        return Random.Range(spinner.minRange, spinner.maxRange);
    }

    // Coroutine to spin the wheel and stop at the calculated angle
    IEnumerator SpinWheel(float targetAngle)
    {
        float startAngle = spinWheel.transform.rotation.eulerAngles.z;
        float totalRotation = 360f * 3; // Three full rotations

        // Ensure that we always rotate clockwise
        float finalAngle = GetClockwiseAngle(startAngle, targetAngle + totalRotation);

        float elapsed = 0f;

        // Spin the wheel with the total rotation
        while (elapsed < spinDuration)
        {
            float angle = Mathf.Lerp(startAngle, finalAngle, elapsed / spinDuration);
            spinWheel.transform.rotation = Quaternion.Euler(0, 0, angle);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Finalize the spinning animation and start deceleration
        elapsed = 0f;
        while (elapsed < decelerationDuration)
        {
            float decelerationAngle = Mathf.Lerp(finalAngle, targetAngle, elapsed / decelerationDuration);
            spinWheel.transform.rotation = Quaternion.Euler(0, 0, decelerationAngle);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the final angle is set precisely
        spinWheel.transform.rotation = Quaternion.Euler(0, 0, targetAngle);
        Debug.Log("Wheel stopped at angle: " + targetAngle);

        Invoke("ActivatingResultPanel", 0.5f);
        Invoke("DeactivatingResultPanel", 6f);
    }

    // Function to calculate the clockwise angle
    float GetClockwiseAngle(float startAngle, float targetAngle)
    {
        float difference = targetAngle - startAngle;

        if (difference < 0)
        {
            difference += 360f; // Always rotate clockwise
        }

        return startAngle + difference;
    }

    public void ActivatingResultPanel()
    {
        resultPanel.SetActive(true);
    }

    public void DeactivatingResultPanel()
    {
        if (resultPanel.active)
        {
            resultPanel.SetActive(false);
        }
    }
}
