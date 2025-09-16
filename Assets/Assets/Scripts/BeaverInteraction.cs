using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BeaverInteraction : GameManager
{
    //Trying to really only have Dialogue and Beaver Interaction here

    public int roomNumber;
    public GameObject dialogueUI;
    public RawImage beaverPortraitUI;
    public Texture2D beaverPortraitTexture;

    public void Dialogue()
    {
        SetPortraitImage();
        ActivateDialogueBox();
        if (answeredTriviaList[roomNumber - 1] == true)
        {
            ShowSpecificDialogue(roomNumber);
        }
        else
        {
            ShowGenericDialogue();
        }
    }

    public void EndDialogue()
    {
        dialogueUI.SetActive(false);
    }

    private void SetPortraitImage()
    {
        beaverPortraitUI.texture = beaverPortraitTexture;
    }

    private void ShowGenericDialogue()
    {
        int _randomGenericDialogue = Random.Range(0, genericDialogueList.Count);
        dialogueBox.text = genericDialogueList[_randomGenericDialogue];
    }

    private void ShowSpecificDialogue(int roomNumber)
    {
        dialogueBox.text = specificDialogueList[roomNumber - 1];
    }

    private void ActivateDialogueBox()
    {
        dialogueUI.SetActive(true);
    }
}
