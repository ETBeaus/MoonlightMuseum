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

	public Button btn_Prev;
	public Button btn_Next;
	
    void Start()
    {
		_viewManager = GameObject.Find("ViewManager").GetComponent<ViewManager>();
		_paintingData = GameObject.Find("Paintings").GetComponent<PaintingData>(); 
		_wordInspect = GetComponent<WordInspect>();

		_tm = GetComponentInChildren<TextMeshProUGUI>();
		_wordInspect.textMesh = _tm;

		// *note: 
		// Set in editor, this is broken?
		//canvas = GetComponentInChildren<Canvas>();
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
				Debug.Log($"painting id: {paintingId}");
			}
		}
		else if(_wordInspect.inspectActive)
		{
			PaintingTextClose();
		}
    }

	public void PaintingTextInit()
	{
		canvas.enabled = true;

		_wordInspect._currPainting = paintingId; 
		_wordInspect.inspectActive = true;

		_tm.enabled = true;
		_tm.text = _paintingData.dbEntries[paintingId].description;

		paintingViewActive = true;
		//_tm.pageToDisplay = 0;
		
		_wordInspect.OnShow();
	}

	public void PaintingTextClose()
	{
		canvas.enabled = false;
		_wordInspect.inspectActive = false;
		_tm.enabled = false;

		paintingViewActive = false;
	}

	public void CyclePage(sbyte dir) 	
	{
		//if(!(_tm.pageToDisplay + dir > -1 && _tm.pageToDisplay < _tm.GetTextInfo(_tm.text).pageCount - 1)) return;
		//_tm.pageToDisplay += dir;
	}
}

