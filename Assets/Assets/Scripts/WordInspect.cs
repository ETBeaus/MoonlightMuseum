using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ResponseGroup
{
    public string[] str;
    public ResponseGroup(string[] _str) { str = _str; }
}

public class WordInspect : MonoBehaviour 
{
    static string[] _responseCorrect = {
        "Yes!",
        "Correct!",
        "That's right!",
        "You got it!"
    };

    static string[] _responseWrong = {
        "Nope..",
        "Incorrect.",
        "That's not right, sorry...",
        "Keep trying..."
    };

    ResponseGroup[] _respGroups = {
        new ResponseGroup(_responseCorrect),
        new ResponseGroup(_responseWrong)
    };

	private DialogueManager _diagManager;

    void Start()
    {

    }

    void Update()
    {
    }

    public void WordInspectResponse(bool success)
    {
        byte _responseId = (byte)Random.Range(0, 4);
    }
}

