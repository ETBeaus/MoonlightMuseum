using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_DialogueTemplate", menuName = "Scriptable Objects/SO_DialogueTemplate")]
public class SO_DialogueTemplate : ScriptableObject
{
    [Header("General Info")]
    public string NameToDisplay;
    public Texture2D PortraitTexture;

    [Header("Replies")]
    public bool IsARepliesChoice = false;
    public List<string> ReplyChoices;

    [Header("Line")]
    public string DialogueLine;
}
