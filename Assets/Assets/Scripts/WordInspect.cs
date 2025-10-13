using System;
using UnityEngine;
using TMPro; 
using UnityEngine.InputSystem;

public class WordInspect : MonoBehaviour 
{
	private PaintingData _db;
	private Journal _journal;

	public int _currPainting;

	public bool inspectActive = false;

	public bool _keywordHovered; 
	public int _hoveredId;

	private bool _keywordHoveredPrevFrame;

	public TextMeshProUGUI textMesh;

	private int _wordStart;
	private int _wordEnd;

	private int _wordHoverIdCurr;
	private int _wordHoverIdPrev;

	public string textOriginal;
	public string textFormatted;
	
    void Start()
    {
		_db = GameObject.Find("Paintings").GetComponent<PaintingData>();
		_journal = gameObject.GetComponent<Journal>();
    }

    void Update()
    {
		// Skip update if inspection mode inactive
		if(!inspectActive) return;

		_wordHoverIdPrev = _wordHoverIdCurr;
		_keywordHoveredPrevFrame = _keywordHovered;

		bool updateFormat = false;

		// Get cursor position
		Vector2 mousePos = Mouse.current.position.ReadValue();
		
		// Get index of hovered character in text mesh
		_hoveredId = TMPro.TMP_TextUtilities.FindIntersectingCharacter(
				textMesh,
				mousePos,
				null,
				true
		);
		
		_wordHoverIdCurr = TMPro.TMP_TextUtilities.FindIntersectingWord(textMesh, mousePos, null);

		updateFormat = (
				_wordHoverIdPrev != _wordHoverIdCurr && 
				_wordHoverIdCurr > -1 &&
				_wordHoverIdCurr < textMesh.textInfo.wordCount
		);

		int kwStart = _db.dbEntries[_currPainting].keyStart;
		int kwEnd   = _db.dbEntries[_currPainting].keyEnd;

		// If hovered character index falls between start and end index of keyword,
		// count keyword hovered as true
		_keywordHovered = (_hoveredId >= kwStart && _hoveredId <= kwEnd);

		// Get click state
		bool click = Mouse.current.leftButton.wasPressedThisFrame;

		// Add keyword to journal if clicked
		if(_keywordHovered && click)
			_journal.AddKeywordEntry(_db.dbEntries[_currPainting]);

		if(updateFormat || (_keywordHovered && !_keywordHoveredPrevFrame))
		{
			FormatReset();

			if(_keywordHovered)
			{
				string textColor = "#48cae4";
				string defaultColor = "#000000";
				FormatApply(kwStart, kwEnd, $"<color={textColor}>", $"<color={defaultColor}>");
				return;
			}

			var wordInfo = textMesh.textInfo.wordInfo[_wordHoverIdCurr];
			FormatApply(wordInfo.firstCharacterIndex, wordInfo.lastCharacterIndex, "<u>", "</u>");
		}
    }

	/// <summary>
	/// Apply rich text formatting 
	/// from char index startId to endId
	/// with tags tagOpen and tagClose
	/// </summary>
	void FormatApply(int startId, int endId, string tagOpen, string tagClose)
	{
		if(textOriginal == null || startId < 0 || endId >= textOriginal.Length) return;
		
		string textLocal = textOriginal[startId..(endId+1)];
		string formatted = String.Empty;

		formatted = (
			textOriginal[0..startId] +
			tagOpen +
			textLocal +
			tagClose + 
			textOriginal[(endId+1)..]
		);

		textMesh.text = formatted;
	}

	/// <summary>
	/// Clear text formatting
	/// </summary>
	void FormatReset()
	{
		if(textOriginal == null) return; 
		textMesh.text = textOriginal;
	}

	public void OnShow() { textOriginal = _db.dbEntries[_currPainting].description; }
}

