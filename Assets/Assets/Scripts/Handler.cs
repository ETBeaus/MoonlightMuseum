using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Handler : MonoBehaviour
{
	private ViewManager _viewManager;
	private PaintingData _paintingData;
	private WordInspect _wordInspect;

	private int _paintingCount = 12;
	private int _paintingsStartId = 65; 

	public bool diagActive = false;
	public bool paintingViewActive;
	public int prevViewId;
	public int currViewId;

	public int paintingId;

	public TextMeshProUGUI _tm;
	public Canvas canvas;
	
    void Start()
    {
		_viewManager = GameObject.Find("ViewManager").GetComponent<ViewManager>();
		_paintingData = GameObject.Find("Paintings").GetComponent<PaintingData>(); 
		_wordInspect = GetComponent<WordInspect>();

		_tm = GetComponentInChildren<TextMeshProUGUI>();
		_wordInspect.textMesh = _tm;
    }

    void Update()
    {
		if(diagActive) 
		{
			if(paintingViewActive) PaintingTextClose();
			paintingViewActive = false;
			return;
		}

		prevViewId = currViewId;
		currViewId = _viewManager.GetViewId();

		paintingId = currViewId - _paintingsStartId;
		paintingViewActive = (paintingId >= 0 && paintingId < _paintingCount);

		if(paintingViewActive)		
		{
			if(prevViewId != currViewId)
			{
				PaintingTextInit();
			}
		}
		else if(_wordInspect.inspectActive)
		{
			PaintingTextClose();
		}
    }

	/// <summary>
	/// Open painting description
	/// </summary>
	public void PaintingTextInit()
	{
		canvas.enabled = true;

		_wordInspect._currPainting = paintingId; 
		_wordInspect.inspectActive = true;

		_tm.enabled = true;
		_tm.text = _paintingData.dbEntries[paintingId].description;

		paintingViewActive = true;
		
		_wordInspect.OnShow();
	}

	/// <summary>	
	/// Close painting description
	/// </summary>
	public void PaintingTextClose()
	{
		canvas.enabled = false;
		_wordInspect.inspectActive = false;
		_tm.enabled = false;

		paintingViewActive = false;
	}
}

