using UnityEngine;
using TMPro;

public class Handler : MonoBehaviour
{
	private ViewManager _viewManager;
	private PaintingData _paintingData;
	private WordInspect _wordInspect;

	private int _paintingCount = 12;
	private int _paintingsStartId = 42; 

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

		// note: 
		// Set in editor, this is broken?
		//canvas = GetComponentInChildren<Canvas>();
    }

    void Update()
    {
		paintingId = -1;

		prevViewId = currViewId;
		currViewId = _viewManager.GetViewId();

		paintingViewActive = (currViewId >= _paintingsStartId && currViewId <= _paintingsStartId + _paintingCount);

		if(paintingViewActive)		
		{
			paintingId = (currViewId - _paintingsStartId);

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

	private void PaintingTextInit()
	{
		//gameObject.GetComponentInChildren<Canvas>().enabled = true;
		canvas.enabled = true;

		_wordInspect._currPainting = paintingId; 
		_wordInspect.inspectActive = true;

		_tm.enabled = true;
		_tm.text = _paintingData.dbEntries[paintingId].description;
	}

	private void PaintingTextClose()
	{
		//gameObject.GetComponentInChildren<Canvas>().enabled = false;
		canvas.enabled = false;
		_wordInspect.inspectActive = false;
		_tm.enabled = false;
	}
}

