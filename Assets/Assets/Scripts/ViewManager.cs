using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ViewManager : MonoBehaviour
{
    public List<Texture2D> views;
    public List<GameObject> buttonGroups;
    public RawImage backgroundImage;
    public CanvasGroup fadePanel;
    

    public float fadeSpeed = 1f;

    bool _isChangingBackground = false;
    bool _isFadingOut = false;
    bool _isFadingIn = false;
    float _timer = 0;

    int _previousViewIndex;
    int _currentViewIndex;
    int _nextViewIndex;

    int _previousViewButtonsIndex;
    int _currentViewButtonsIndex = 0;
    int _nextViewButtonsIndex;

	public MapLoader _mapLoader;
	
	// NOTE: **
	// For use in test scene
	public void SetViewId(int id) { 
		_currentViewIndex = id;
		backgroundImage.texture = views[id];
	}

    void Update()
    {
        if (_isChangingBackground)
        {
            ChangeBackground(_nextViewIndex);
        }
    }

    //-------------------------UI OnClick() METHODS--------------------------------

    public void ChangeBackgroundButton(int nextViewIndex)
    {
        _previousViewIndex = _currentViewIndex;
        _nextViewIndex = nextViewIndex;
        _isChangingBackground = true;
    }

    public void DisableActiveButtons()
    {
        buttonGroups[_currentViewButtonsIndex].SetActive(false);
    }

    public void EnableNextViewButtons(int nextViewButtonsIndex)
    {
        _previousViewButtonsIndex = _currentViewButtonsIndex;
        _nextViewButtonsIndex = nextViewButtonsIndex;
        _currentViewButtonsIndex = _nextViewButtonsIndex;
    }

    public void MiscInteraction(GameObject textToShow)
    {
        textToShow.SetActive(true);
    }

    public void WalkBack()
    {
        DisableActiveButtons();
        ChangeBackgroundButton(_previousViewIndex);
        EnableNextViewButtons(_previousViewButtonsIndex);
    }


    //------------------------PRIVATE METHODS---------------------------------

    private void ChangeBackground(int nextViewIndex)
    {
        if (fadePanel.alpha == 0 && _timer == 0)
        {
            _isFadingOut = true;
        }
        _timer += Time.deltaTime;
        if (_isFadingOut == true)
        {
            if (fadePanel.alpha < 1)
            {
                fadePanel.alpha += Time.deltaTime * fadeSpeed;
            }
        }

        if (fadePanel.alpha == 1 && _isFadingOut)
        {
            backgroundImage.texture = views[nextViewIndex];
            _currentViewIndex = nextViewIndex;
            _isFadingOut = false;
            _isFadingIn = true;
        }

        if (_isFadingIn == true)
        {
            if (fadePanel.alpha > 0)
            {
                fadePanel.alpha -= Time.deltaTime * fadeSpeed;
            }
        }

        if (fadePanel.alpha == 0 && _isFadingIn)
        {
            buttonGroups[_nextViewButtonsIndex].SetActive(true); //Activates the UI buttons for next room
            _isFadingIn = false;
            _timer = 0;
            _isChangingBackground = false;
        }
    }
}
