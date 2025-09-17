using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BeaverInteraction : GameManager
{
    //Trying to really only have Dialogue and Beaver Interaction here

    public int roomNumber;
    public string nameOfPatron;

    //References to set in inspector
    public GameObject dialogueUI;
    public GameObject beaverButton;
    public RawImage beaverPortraitUI;
    public Texture2D beaverPortraitTexture;
    public GameObject nextDialogueButton;
    public TMP_Text dialogueBox;
    public TMP_Text nameBox;

    //Lists - dialogue goes there (in inspector)!
    public List<string> genericDialogueList;
    public List<string> specificDialogueList;

    //Private variables
    private int _currentDialogueIndex;


    //Public button methods
    public void StartDialogue()
    {
        DeactivateBeaverButton();
        SetPortraitImage();
        ActivateDialogueBox();
        SetName();
        if (answeredTriviaList[roomNumber - 1] == true)
        {
            _currentDialogueIndex = 0;
            ShowSpecificDialogue(_currentDialogueIndex);
            ActivateNextDialogueButton();
        }
        else
        {
            ShowGenericDialogue();
        }
    }

    public void NextLine()
    {
        _currentDialogueIndex++;
        ShowSpecificDialogue(_currentDialogueIndex);
        ActivateNextDialogueButton();
    }

    public void EndDialogue()
    {
        ActivateBeaverButton();
        dialogueUI.SetActive(false);
    }


    //Private methods
    private void SetPortraitImage()
    {
        beaverPortraitUI.texture = beaverPortraitTexture;
    }

    private void SetName()
    {
        nameBox.text = nameOfPatron;
    }

    private void ShowGenericDialogue()
    {
        int _randomGenericDialogue = Random.Range(0, genericDialogueList.Count);
        dialogueBox.text = genericDialogueList[_randomGenericDialogue];
    }

    private void ShowSpecificDialogue(int dialogueIndex)
    {
        dialogueBox.text = specificDialogueList[dialogueIndex];
    }

    private void ActivateDialogueBox()
    {
        dialogueUI.SetActive(true);
    }

    private void ActivateNextDialogueButton()
    {
        if (specificDialogueList.Count > 1 && _currentDialogueIndex != specificDialogueList.Count - 1)
        {
            nextDialogueButton.SetActive(true);
        }
        else
        {
            nextDialogueButton.SetActive(false);
        }
    }

    private void DeactivateBeaverButton()
    {
        beaverButton.SetActive(false);
    }

    private void ActivateBeaverButton()
    {
        beaverButton.SetActive(true);
    }
}
