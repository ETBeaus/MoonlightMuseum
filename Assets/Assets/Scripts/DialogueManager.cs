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
    public RawImage BeaverPortraitUI;
    public TMP_Text DialogueTextOutput;
    public TMP_Text NameOutput;
    public GameObject EndDialogueButton;
    public GameObject NextDialogueButton;
    public GameObject DialoguePanel;
    public GameObject ChoicePanel;
    public GameObject ChoiceButton_Prefab;
    public List<SO_DialogueTemplate> SO_DialogueEvents;

    [Header("Bools")]
    public bool IsRepeatableInteraction = false;
    public bool hasQuizQuestion = false;
    public bool CanExitAtAnyTime = true;
    public bool IsForGym = false;
    //public Journal journal;
    #endregion

    #region Private Fields
    [Header("Other")]
    [SerializeField] private bool _questionWasAnswered = false;
    [SerializeField] private int _currentDialogueIndex;
    private List<GameObject> _choiceButtons = new List<GameObject>();
    private Journal _journal;
    private Handler _handler;
    #endregion

    //------------------------------------------------PUBLIC BUTTON METHODS------------------------------------------------------
    #region Public Button Methods

    /// <summary>
    /// Activates all the appropriate boxes/buttons and displays the first line of the sequence.
    /// </summary>
    public void StartDialogue()
    {
        ActivateDialogueCanvas();
        if (CanExitAtAnyTime)
        {
            ActivateEndDialogueButton();
        }
        ResetDialogueIndex();
        DeactivateInitialButton();
        DisplayLine();

        if (!IsForGym)
        {
            _journal = GameObject.Find("HandlerObject").GetComponent<Journal>();
            _handler = GameObject.Find("HandlerObject").GetComponent<Handler>();

            _handler.diagActive = true;
        }
    }

    /// <summary>
    /// Resets the buttons and increments the _currentDialogueIndex.
    /// If need be, reloads the question.
    /// Then, displays the associated line.
    /// </summary>
    public void NextLine()
    {
        if (SO_DialogueEvents[_currentDialogueIndex].IsARepliesChoice)
        {
            ResetChoiceButtonList();
        }

        if (SO_DialogueEvents[_currentDialogueIndex].IsWrongAnswerReaction && hasQuizQuestion)
        {
            ReloadQuestion();
        }
        else if (SO_DialogueEvents[_currentDialogueIndex].IsWrongAnswerReaction && !hasQuizQuestion)
        {
            JumpToAfterQuestionDialogue();
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

    /// <summary>
    /// If you got it right on the first try, cycles the list of dialogue to find the appropriate reaction. Same process if you get it right, but not on the first try. Then, displays the corresponding line.
    /// </summary>
    public void RightAnswer()
    {
        if (!_questionWasAnswered)
        {
            _questionWasAnswered = true;

            for (int i = _currentDialogueIndex; i < SO_DialogueEvents.Count; i++)
            {
                if (SO_DialogueEvents[i].IsFirstTryRightAnswerReaction)
                {
                    _currentDialogueIndex = i;
                    if (!IsForGym)
                    {
                        _journal.AddStickerEntry();
                    }
                    break;
                }
            }
        }
        else
        {
            for (int i = _currentDialogueIndex; i < SO_DialogueEvents.Count; i++)
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

    /// <summary>
    /// Cycles the list of dialogue to find the appropriate reaction. Then, displays the corresponding line.
    /// </summary>
    public void WrongAnswer()
    {
        _questionWasAnswered = true;
        for (int i = _currentDialogueIndex; i < SO_DialogueEvents.Count; i++)
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

    /// <summary>
    /// Deactivates all buttons/boxes and activates or destroys the initial button depending on the situation.
    /// </summary>
    public void EndDialogue()
    {
        DeactivateDialogueCanvas();

        if (!IsForGym)
        {
            _handler.diagActive = false;
        }

        if (SO_DialogueEvents[_currentDialogueIndex].AnswerChoices.Count > 0)
        {
            ResetChoiceButtonList();
        }
        if (_currentDialogueIndex != SO_DialogueEvents.Count - 1 || IsRepeatableInteraction)
        {
            ActivateInitialButton();
        }
        else
        {
            Destroy(this.gameObject);
        }
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

    private void ActivateEndDialogueButton()
    {
        EndDialogueButton.SetActive(true);
        EndDialogueButton.GetComponent<Button>().onClick.RemoveListener(EndDialogue);
        EndDialogueButton.GetComponent<Button>().onClick.AddListener(EndDialogue);
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
        for (int i = _currentDialogueIndex; i < SO_DialogueEvents.Count; i++)
        {
            if (SO_DialogueEvents[i].IsFirstLineAfterQuestion)
            {
                _currentDialogueIndex = i;
                break;
            }
        }
    }

    /// <summary>
    /// Cycles through the methods needed to display the correct name, portrait and line or buttons.
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
        if (!CanExitAtAnyTime && _currentDialogueIndex == SO_DialogueEvents.Count - 1)
        {
            ActivateEndDialogueButton();
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


    #region Toggles
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

    /// <summary>
    /// Toggles the Next Dialogue Button and makes sure the appropriate listener is on.
    /// </summary>
    private void ActivateNextDialogueButton()
    {
        if (SO_DialogueEvents.Count > 1 && _currentDialogueIndex != SO_DialogueEvents.Count - 1 && !SO_DialogueEvents[_currentDialogueIndex].IsARepliesChoice)
        {
            NextDialogueButton.SetActive(true);
            NextDialogueButton.GetComponent<Button>().onClick.RemoveAllListeners();
            NextDialogueButton.GetComponent<Button>().onClick.AddListener(NextLine);
        }
        else
        {
            NextDialogueButton.SetActive(false);
        }
    }



    private void DeactivateInitialButton()
    {
        this.gameObject.SetActive(false);
    }

    private void ActivateInitialButton()
    {
        this.gameObject.SetActive(true);
    }
    #endregion
    
    #endregion
}
