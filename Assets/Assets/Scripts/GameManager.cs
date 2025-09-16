using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Just trying out something here, maybe storing broadly used variables in a GameManager
    //and having other classes derive from it could help?
    // But it would make everything more coupled?

    public int currentViewIndex;

    public List<string> genericDialogueList;

    public List<string> specificDialogueList;

    public List<bool> answeredTriviaList;

    public TMP_Text dialogueBox;

    [SerializeField] private bool _hasAnsweredTriviaRoom1 = false;


    void Start()
    {
        PopulateAnsweredTriviaList();
    }


    void Update()
    {

    }

    private void PopulateAnsweredTriviaList()
    {
        answeredTriviaList.Add(_hasAnsweredTriviaRoom1);
    }
}
