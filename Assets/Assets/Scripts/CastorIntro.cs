using UnityEngine;

public class CastorIntro : MonoBehaviour
{
    public ViewManager ViewManager;
    private DialogueManager _DialogueManager;

    void Start()
    {
        _DialogueManager = GetComponent<DialogueManager>();
        _DialogueManager.StartDialogue();
        ViewManager.ToggleActiveButtons();
    }
}
