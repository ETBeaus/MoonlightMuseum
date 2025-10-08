using NUnit.Framework;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool HasCompletedPoem = false;

    public ViewManager ViewManager;

    private bool _startedEndCutscene = false;

	//public Journal journal;

    void Update()
    {
		//HasCompletedPoem = (journal.PoemWords.Count >= 11);

        if (HasCompletedPoem && !_startedEndCutscene)
        {
            ViewManager.ToggleActiveButtons();
            ViewManager.ChangeBackgroundButton(77);
            ViewManager.EnableNextViewButtons(77);
            _startedEndCutscene = true;
        }
    }
}
