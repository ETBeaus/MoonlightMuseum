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

	public TextMeshProUGUI textMesh;
	
    void Start()
    {
		_db = GameObject.Find("Paintings").GetComponent<PaintingData>();
		_journal = gameObject.GetComponent<Journal>();
    }

    void Update()
    {
		// Skip update if inspection mode inactive
		if(!inspectActive) return;

		// Get cursor position
		Vector2 mousePos = Mouse.current.position.ReadValue();
		
		// Get index of hovered character in text mesh
		_hoveredId = TMPro.TMP_TextUtilities.FindIntersectingCharacter(
				textMesh,
				mousePos,
				null,
				true
		);

		int kwStart = _db.dbEntries[_currPainting].keyStart;
		int kwEnd   = _db.dbEntries[_currPainting].keyEnd;

		// If hovered character index falls between start and end index of keyword,
		// count keyword hovered as true
		_keywordHovered = (_hoveredId >= kwStart && _hoveredId <= kwEnd);

		// Get click state
		bool click = Mouse.current.leftButton.wasPressedThisFrame;

		// Add keyword to journal if clicked
		if(click && _keywordHovered)
		{
			_journal.AddEntry(_db.dbEntries[_currPainting]);
		}
    }
}

