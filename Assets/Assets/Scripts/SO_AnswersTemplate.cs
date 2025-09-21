using UnityEngine;

[CreateAssetMenu(fileName = "SO_AnswersTemplate", menuName = "Scriptable Objects/SO_AnswersTemplate")]
public class SO_AnswersTemplate : ScriptableObject
{
    public string Answer;
    public bool IsRightAnswer;
    public bool IsWrongAnswer;
}
