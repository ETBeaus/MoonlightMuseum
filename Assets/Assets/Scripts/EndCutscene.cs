using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndCutscene : MonoBehaviour
{
    private DialogueManager _DialogueManager;

    private void OnEnable()
    {
        _DialogueManager = GetComponent<DialogueManager>();
        _DialogueManager.StartDialogue();
        _DialogueManager.EndDialogueButton.GetComponent<Button>().onClick.AddListener(BackToMainMenu);
        _DialogueManager.EndDialogueButton.SetActive(false);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
