using NUnit.Framework;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool HasCompletedPoem = false;

    public ViewManager ViewManager;

    private bool _startedEndCutscene = false;

    void Update()
    {
        if (HasCompletedPoem && !_startedEndCutscene)
        {
            ViewManager.ToggleActiveButtons();
            ViewManager.ChangeBackgroundButton(77);
            ViewManager.EnableNextViewButtons(77);
            _startedEndCutscene = true;
        }
    }
}
