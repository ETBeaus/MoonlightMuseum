using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class DialogueManager : MonoBehaviour
{
    //References to set in inspector
    public GameObject DialogueCanvas;
    public GameObject BeaverButton;
    public RawImage BeaverPortraitUI;
    public TMP_Text DialogueTextOutput;
    public TMP_Text NameOutput;
    public GameObject NextDialogueButton;
    public GameObject DialoguePanel;
    public GameObject ChoicePanel;
    public GameObject ChoiceButton_NoImpact_Prefab;

    public List<SO_DialogueTemplate> SO_DialogueEvents;

    //Private variables
    private int _currentDialogueIndex;
    private List<GameObject> _choiceButtons = new List<GameObject>();

    //Public button methods
    public void StartDialogue()
    {
        ActivateDialogueCanvas();
        ResetDialogueIndex();
        DeactivateBeaverButton();
        SetPortraitImage(_currentDialogueIndex);
        SetName(_currentDialogueIndex);
        if (SO_DialogueEvents[_currentDialogueIndex].IsARepliesChoice)
        {
            DeactivateDialoguePanel();
            ActivateChoicePanel();
            SetupChoiceButtons();
        }
        else
        {
            DeactivateChoicePanel();
            ActivateDialoguePanel();
            SetLine(_currentDialogueIndex);
        }
        ActivateNextDialogueButton();
    }

    public void NextLine()
    {
        if (SO_DialogueEvents[_currentDialogueIndex].IsARepliesChoice)
        {
            ResetChoiceButtonList();
        }
        _currentDialogueIndex++;
        SetPortraitImage(_currentDialogueIndex);
        SetName(_currentDialogueIndex);
        if (SO_DialogueEvents[_currentDialogueIndex].IsARepliesChoice)
        {
            DeactivateDialoguePanel();
            ActivateChoicePanel();
            SetupChoiceButtons();
        }
        else
        {
            DeactivateChoicePanel();
            ActivateDialoguePanel();
            SetLine(_currentDialogueIndex);
        }
        ActivateNextDialogueButton();
    }

    public void EndDialogue()
    {
        if (SO_DialogueEvents[_currentDialogueIndex].IsARepliesChoice)
        {
            ResetChoiceButtonList();
        }
        if (_currentDialogueIndex != SO_DialogueEvents.Count - 1)
            {
                ActivateBeaverButton();
            }
        DeactivateDialogueCanvas();
    }

    //Private methods
    private void ResetDialogueIndex()
    {
        _currentDialogueIndex = 0;
    }

    private void ResetChoiceButtonList()
    {
        for (int i = 0; i < _choiceButtons.Count; i++)
        {
            Destroy(_choiceButtons[i]);
        }
        _choiceButtons.Clear();
    }

    private void SetPortraitImage(int dialogueIndex)
    {
        BeaverPortraitUI.texture = SO_DialogueEvents[dialogueIndex].PortraitTexture;
    }

    private void SetName(int dialogueIndex)
    {
        NameOutput.text = SO_DialogueEvents[dialogueIndex].NameToDisplay;
    }

    private void SetLine(int dialogueIndex)
    {
        DialogueTextOutput.text = SO_DialogueEvents[dialogueIndex].DialogueLine;
    }

    private void ActivateDialogueCanvas()
    {
        DialogueCanvas.SetActive(true);
    }

    private void DeactivateDialogueCanvas()
    {
        DialogueCanvas.SetActive(false);
    }

    private void ActivateDialoguePanel()
    {
        DialoguePanel.SetActive(true);
    }

    private void DeactivateDialoguePanel()
    {
        DialoguePanel.SetActive(false);
    }

    private void ActivateChoicePanel()
    {
        ChoicePanel.SetActive(true);
    }

    private void DeactivateChoicePanel()
    {
        ChoicePanel.SetActive(false);
    }

    private void ActivateNextDialogueButton()
    {
        if (SO_DialogueEvents.Count > 1 && _currentDialogueIndex != SO_DialogueEvents.Count - 1 && !SO_DialogueEvents[_currentDialogueIndex].IsARepliesChoice)
        {
            NextDialogueButton.SetActive(true);
        }
        else
        {
            NextDialogueButton.SetActive(false);
        }
    }

    private void SetupChoiceButtons()
    {
        for (int i = 0; i < SO_DialogueEvents[_currentDialogueIndex].ReplyChoices.Count; i++)
        {
            _choiceButtons.Add(Instantiate(ChoiceButton_NoImpact_Prefab, ChoicePanel.transform));
        }

        for (int i = 0; i < _choiceButtons.Count; i++)
        {
            _choiceButtons[i].GetComponentInChildren<TMP_Text>().text = SO_DialogueEvents[_currentDialogueIndex].ReplyChoices[i];
            _choiceButtons[i].GetComponent<Button>().onClick.AddListener(NextLine);
        }
    }

    private void DeactivateBeaverButton()
    {
        BeaverButton.SetActive(false);
    }

    private void ActivateBeaverButton()
    {
        BeaverButton.SetActive(true);
    }

}
