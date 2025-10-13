using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndCutscene : MonoBehaviour
{
    private DialogueManager _dialogueManager;

    private void OnEnable()
    {
        //Force starts the end cutscene dialogue.
        _dialogueManager = GetComponent<DialogueManager>();
        _dialogueManager.StartDialogue();
        _dialogueManager.EndDialogueButton.GetComponent<Button>().onClick.AddListener(BackToMainMenu);
        _dialogueManager.EndDialogueButton.SetActive(false);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
