using NUnit.Framework;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool HasCompletedPoem = false;

    public ViewManager ViewManager;

    private bool _startedEndCutscene = false;

    //public Journal journal;

    private void Update()
    {
        //HasCompletedPoem = (journal.PoemWords.Count >= 11);

        if (HasCompletedPoem && !_startedEndCutscene)
        {
            StartEndCutscene();
        }
    }

    //Changes to the view for the end custscene and disables player input.
    private void StartEndCutscene()
    {
        ViewManager.ToggleActiveButtons();
        ViewManager.ChangeBackgroundButton(77);
        ViewManager.EnableNextViewButtons(77);
        _startedEndCutscene = true;
    }
}
