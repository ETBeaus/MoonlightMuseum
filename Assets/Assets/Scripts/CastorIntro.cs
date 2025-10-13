using UnityEngine;

public class CastorIntro : MonoBehaviour
{
    public ViewManager ViewManager;


    private DialogueManager _dialogueManager;

    void Start()
    {
        //Force starts the Intro dialogue and disables player input
        _dialogueManager = GetComponent<DialogueManager>();
        _dialogueManager.StartDialogue();
        ViewManager.ToggleActiveButtons();
    }
}
