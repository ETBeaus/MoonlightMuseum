using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ViewManager : MonoBehaviour
{
    #region Public Fields
    //References
    public List<Texture2D> Views;
    public List<GameObject> ButtonGroups;
    public RawImage BackgroundImage;
    public CanvasGroup FadePanel;

    public float FadeSpeed = 3f;
    #endregion

    #region Private Fields
    private bool _isChangingBackground = false;
    private bool _isFadingOut = false;
    private bool _isFadingIn = false;
    private float _timer = 0;

    private int _previousViewIndex;
    private int _currentViewIndex;
    private int _nextViewIndex;

    private int _previousViewButtonsIndex;
    private int _currentViewButtonsIndex = 0;
    private int _nextViewButtonsIndex;

    public MapLoader _mapLoader;
    #endregion

    // NOTE: **
    // For use in test scene
    public void SetViewId(int id)
    {
        _currentViewIndex = id;
        BackgroundImage.texture = Views[id];
    }

    public int GetViewId()
    {
        return _currentViewIndex;
    }

    private void Update()
    {
        if (_isChangingBackground)
        {
            ChangeBackground(_nextViewIndex);
        }
    }

    //-------------------------UI OnClick() METHODS--------------------------------
    #region Public Button Methods
    /// <summary>
    /// Stores the current view as the previous view and toggles the background change for the next view.
    /// </summary>
    /// <param name="nextViewIndex"></param>
    public void ChangeBackgroundButton(int nextViewIndex)
    {
        _previousViewIndex = _currentViewIndex;
        _nextViewIndex = nextViewIndex;
        _isChangingBackground = true;
    }

    /// <summary>
    /// Flips the switch on the buttons linked to this view. If active, deactivates them. If inactive, activates them.
    /// </summary>
    public void ToggleActiveButtons()
    {
        if (ButtonGroups[_currentViewButtonsIndex].activeSelf)
        {
            ButtonGroups[_currentViewButtonsIndex].SetActive(false);
        }
        else
        {
            ButtonGroups[_currentViewButtonsIndex].SetActive(true);
        }
    }

    /// <summary>
    /// Stores the current buttons index as the previous buttons index and toggles the next buttons.
    /// </summary>
    /// <param name="nextViewButtonsIndex"></param>
    public void EnableNextViewButtons(int nextViewButtonsIndex)
    {
        _previousViewButtonsIndex = _currentViewButtonsIndex;
        _nextViewButtonsIndex = nextViewButtonsIndex;
        _currentViewButtonsIndex = _nextViewButtonsIndex;
    }

    //Was only used in early, early testing.
    public void MiscInteraction(GameObject textToShow)
    {
        textToShow.SetActive(true);
    }

    /// <summary>
    /// Reverts back to previous views and previous buttons.
    /// </summary>
    public void WalkBack()
    {
        ToggleActiveButtons();
        ChangeBackgroundButton(_previousViewIndex);
        EnableNextViewButtons(_previousViewButtonsIndex);
    }
    #endregion

    //------------------------PRIVATE METHODS---------------------------------
    #region Private Methods
    /// <summary>
    /// Fades out, changes background, then fades back in and activates the next buttons.
    /// </summary>
    /// <param name="nextViewIndex"></param>
    private void ChangeBackground(int nextViewIndex)
    {
        if (FadePanel.alpha == 0 && _timer == 0)
        {
            _isFadingOut = true;
        }
        _timer += Time.deltaTime;
        //Actually, more of a fade in, since it's the black background that fades in an out... will name that differently next time!
        if (_isFadingOut == true)
        {
            if (FadePanel.alpha < 1)
            {
                FadePanel.alpha += Time.deltaTime * FadeSpeed;
            }
        }

        if (FadePanel.alpha == 1 && _isFadingOut)
        {
            BackgroundImage.texture = Views[nextViewIndex];
            _currentViewIndex = nextViewIndex;
            _isFadingOut = false;
            _isFadingIn = true;
        }

        if (_isFadingIn == true)
        {
            if (FadePanel.alpha > 0)
            {
                FadePanel.alpha -= Time.deltaTime * FadeSpeed;
            }
        }

        if (FadePanel.alpha == 0 && _isFadingIn)
        {
            //Activates the UI buttons for next room
            ButtonGroups[_nextViewButtonsIndex].SetActive(true);
            _isFadingIn = false;
            _timer = 0;
            _isChangingBackground = false;
        }
        Debug.Log("Entering view" + _currentViewIndex);
    }
    #endregion
}
