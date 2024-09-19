using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterClass : MonoBehaviour
{
    public bool isUnlocked;
    public GameObject lockPanel;
    public GameObject framePanel;
    public Image characterImage;
    public Button thisButton;
    void Start()
    {
        isUnlocked = false;

        framePanel = transform.Find("Frame").gameObject;
        framePanel.SetActive(false);
        
        lockPanel = transform.Find("Lock Panel").gameObject;
        lockPanel.SetActive(true);
        
        thisButton = GetComponent<Button>();
        thisButton.interactable = false;
    }

    public void FirstUnlocked()
    {
        if (isUnlocked)
        {
            lockPanel.SetActive(false);
            framePanel.SetActive(true);
            thisButton.interactable = true;
        }
    }

    public void ChoosenAlreadyUnlocked()
    {
        if (isUnlocked)
        {
            Debug.Log("Card already unlocked");
        }
    }


}
