using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_DialogueTemplate", menuName = "Scriptable Objects/SO_DialogueTemplate")]
public class SO_DialogueTemplate : ScriptableObject
{
    [Header("General Info")]
    public string NameToDisplay;
    public Texture2D PortraitTexture;

    [Header("Line")]
    public string DialogueLine;

    [Header("Questions")]
    public bool IsAQuestion = false;

    [Header("Answers")]
    public bool IsARepliesChoice = false;
    public List<SO_AnswersTemplate> AnswerChoices;

    [Header("Following Dialogue")]
    public bool IsFirstLineAfterQuestion;

    [Header("AnswerReactions")]
    public bool IsWrongAnswerReaction = false;
    public bool IsSecondTryRightAnswerReaction = false;
    public bool IsFirstTryRightAnswerReaction = false;
}
