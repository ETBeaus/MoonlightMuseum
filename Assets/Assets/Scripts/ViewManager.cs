using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ViewManager : MonoBehaviour
{
    public List<Texture2D> Views;
    public List<GameObject> ButtonGroups;
    public RawImage BackgroundImage;
    public CanvasGroup FadePanel;
    

    public float FadeSpeed = 3f;

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
	
	// NOTE: **
	// For use in test scene
	public void SetViewId(int id) { 
		_currentViewIndex = id;
		BackgroundImage.texture = Views[id];
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
        ButtonGroups[_currentViewButtonsIndex].SetActive(false);
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
        if (FadePanel.alpha == 0 && _timer == 0)
        {
            _isFadingOut = true;
        }
        _timer += Time.deltaTime;
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
            ButtonGroups[_nextViewButtonsIndex].SetActive(true); //Activates the UI buttons for next room
            _isFadingIn = false;
            _timer = 0;
            _isChangingBackground = false;
        }
    }
}
