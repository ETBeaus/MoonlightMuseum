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

	public bool DiagActive = false;
	public bool PaintingViewActive;
	public int PrevViewId;
	public int CurrviewId;

	public int PaintingId;

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
		if(DiagActive) 
		{
			if(PaintingViewActive) PaintingTextClose();
			PaintingViewActive = false;
			return;
		}

		PrevViewId = CurrviewId;
		CurrviewId = _viewManager.GetViewId();

		PaintingId = CurrviewId - _paintingsStartId;
		PaintingViewActive = (PaintingId >= 0 && PaintingId < _paintingCount);

		if(PaintingViewActive)		
		{
			if(PrevViewId != CurrviewId)
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

		_wordInspect.CurrPainting = PaintingId; 
		_wordInspect.inspectActive = true;

		_tm.enabled = true;
		_tm.text = _paintingData.dbEntries[PaintingId].Description;

		PaintingViewActive = true;
		
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

		PaintingViewActive = false;
	}
}

