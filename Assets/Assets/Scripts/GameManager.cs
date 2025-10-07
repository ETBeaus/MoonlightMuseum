using NUnit.Framework;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool HasCompletedPoem = false;

    public ViewManager ViewManager;

    void Update()
    {
        if (HasCompletedPoem)
        {
            ViewManager.ToggleActiveButtons();
            ViewManager.ChangeBackgroundButton(77);
            ViewManager.EnableNextViewButtons(77);
            HasCompletedPoem = false;
        }
    }
}
