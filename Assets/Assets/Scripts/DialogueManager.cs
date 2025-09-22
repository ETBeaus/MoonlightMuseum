using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class DialogueManager : MonoBehaviour
{
    #region Public Fields
    [Header("References")]
    public GameObject DialogueCanvas;
    public GameObject InitialButton;
    public RawImage BeaverPortraitUI;
    public TMP_Text DialogueTextOutput;
    public TMP_Text NameOutput;
    public GameObject NextDialogueButton;
    public GameObject DialoguePanel;
    public GameObject ChoicePanel;
    public GameObject ChoiceButton_Prefab;
    public List<SO_DialogueTemplate> SO_DialogueEvents;
    public bool IsRepeatableInteraction = false;
    #endregion

    #region Private Fields
    [Header("Other")]
    [SerializeField] private bool _questionWasAnswered = false;
    [SerializeField] private int _currentDialogueIndex;
    private List<GameObject> _choiceButtons = new List<GameObject>();
    #endregion

    //------------------------------------------------PUBLIC BUTTON METHODS------------------------------------------------------
    #region Public Button Methods
    public void StartDialogue()
    {
        ActivateDialogueCanvas();
        ResetDialogueIndex();
        DeactivateInitialButton();
        DisplayLine();
    }

    /// <summary>
    /// Resets the buttons and increments the _currentDialogueIndex.
    /// If need be, reloads the question.
    /// Then, display the associated line.
    /// </summary>
    public void NextLine()
    {
        if (SO_DialogueEvents[_currentDialogueIndex].IsARepliesChoice)
        {
            ResetChoiceButtonList();
        }

        if (SO_DialogueEvents[_currentDialogueIndex].IsWrongAnswerReaction)
        {
            ReloadQuestion();
        }
        else if (SO_DialogueEvents[_currentDialogueIndex].IsSecondTryRightAnswerReaction || SO_DialogueEvents[_currentDialogueIndex].IsFirstTryRightAnswerReaction)
        {
            JumpToAfterQuestionDialogue();
        }
        else
        {
            _currentDialogueIndex++;
        }

        DisplayLine();
    }

    public void RightAnswer()
    {
        if (!_questionWasAnswered)
        {
            //StickerLogic

            _questionWasAnswered = true;

            for (int i = 0; i < SO_DialogueEvents.Count; i++)
            {
                if (SO_DialogueEvents[i].IsFirstTryRightAnswerReaction)
                {
                    _currentDialogueIndex = i;
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < SO_DialogueEvents.Count; i++)
            {
                if (SO_DialogueEvents[i].IsSecondTryRightAnswerReaction)
                {
                    _currentDialogueIndex = i;
                    break;
                }
            }
        }
        ResetChoiceButtonList();
        DisplayLine();
    }

    public void WrongAnswer()
    {
        _questionWasAnswered = true;
        for (int i = 0; i < SO_DialogueEvents.Count; i++)
        {
            if (SO_DialogueEvents[i].IsWrongAnswerReaction)
            {
                _currentDialogueIndex = i;
                break;
            }
        }
        ResetChoiceButtonList();
        DisplayLine();
    }

    public void EndDialogue()
    {
        if (SO_DialogueEvents[_currentDialogueIndex].AnswerChoices.Count > 0)
        {
            ResetChoiceButtonList();
        }
        if (_currentDialogueIndex != SO_DialogueEvents.Count - 1 || IsRepeatableInteraction)
        {
            ActivateInitialButton();
        }
        DeactivateDialogueCanvas();
    }
    #endregion

    //------------------------------------------------PRIVATE METHODS------------------------------------------------------
    #region Private Methods
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

    private void ReloadQuestion()
    {
        for (int i = 0; i < SO_DialogueEvents.Count; i++)
        {
            if (SO_DialogueEvents[i].IsAQuestion)
            {
                _currentDialogueIndex = i;
                break;
            }
        }
    }

    private void JumpToAfterQuestionDialogue()
    {
        for (int i = 0; i < SO_DialogueEvents.Count; i++)
        {
            if (SO_DialogueEvents[i].IsFirstLineAfterQuestion)
            {
                _currentDialogueIndex = i;
                break;
            }
        }
    }

    /// <summary>
    /// Cycles through the methods needed to display the correct name, portrait and line or buttons
    /// </summary>
    private void DisplayLine()
    {
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


    /// <summary>
    /// Instantiates button prefabs by cycling through the Answer List.
    /// Populates them with the corresponding answer.
    /// Adds a OnClick() method depending on the answer type.
    /// </summary>
    private void SetupChoiceButtons()
    {
        for (int i = 0; i < SO_DialogueEvents[_currentDialogueIndex].AnswerChoices.Count; i++)
        {
            _choiceButtons.Add(Instantiate(ChoiceButton_Prefab, ChoicePanel.transform));
        }

        for (int i = 0; i < _choiceButtons.Count; i++)
        {
            if (SO_DialogueEvents[_currentDialogueIndex].AnswerChoices[i].IsRightAnswer)
            {
                _choiceButtons[i].GetComponentInChildren<TMP_Text>().text = SO_DialogueEvents[_currentDialogueIndex].AnswerChoices[i].Answer;
                _choiceButtons[i].GetComponent<Button>().onClick.AddListener(RightAnswer);
            }
            else if (SO_DialogueEvents[_currentDialogueIndex].AnswerChoices[i].IsWrongAnswer)
            {
                _choiceButtons[i].GetComponentInChildren<TMP_Text>().text = SO_DialogueEvents[_currentDialogueIndex].AnswerChoices[i].Answer;
                _choiceButtons[i].GetComponent<Button>().onClick.AddListener(WrongAnswer);
            }
            else
            {
                _choiceButtons[i].GetComponentInChildren<TMP_Text>().text = SO_DialogueEvents[_currentDialogueIndex].AnswerChoices[i].Answer;
                _choiceButtons[i].GetComponent<Button>().onClick.AddListener(NextLine);
            }
        }
    }

    private void DeactivateInitialButton()
    {
        InitialButton.SetActive(false);
    }

    private void ActivateInitialButton()
    {
        InitialButton.SetActive(true);
    }
    #endregion
}
