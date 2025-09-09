using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ViewManager : MonoBehaviour
{
    public List<Texture2D> views;
    public RawImage backgroundImage;
    public CanvasGroup fadePanel;

    public float fadeSpeed = 1f;

    bool _isChangingBackground = false;
    bool _hasChangedUI = false;
    bool _isFadingOut = false;
    bool _isFadingIn = false;
    float _timer = 0;

    int _currentViewIndex = 0;
    int _nextViewIndex;

    void Update()
    {
        if (_isChangingBackground)
        {
            if (!_hasChangedUI)
            {
                ChangeUI();
                _hasChangedUI = true;
            }
            ChangeBackground(_nextViewIndex);
        }
    }

    public void ChangeBackgroundButton(int nextViewIndex)
    {
        _nextViewIndex = nextViewIndex;
        _currentViewIndex = nextViewIndex;
        _isChangingBackground = true;
    }

    public void DisableActiveButtons(GameObject activeButtons)
    {
        activeButtons.SetActive(false);
    }

    public void MiscInteraction(GameObject textToShow)
    {
        textToShow.SetActive(true);
    }

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
            _isFadingIn = false;
            _timer = 0;
            _isChangingBackground = false;
            _hasChangedUI = false;
        }
    }

    private void ChangeUI()
    {
        //Here, we'll add the logic to change UI from view to view
    }
}
