using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class WordInspect : MonoBehaviour 
{
	private PaintingData _db;
	private DialogueManager _diagManager;

	private byte _currPainting;

	public bool inspectActive = false;

	private bool _keywordHovered; 
	private int _hoveredId;
	
    void Start()
    {
		_db = GameObject.Find("PaintingStuff").GetComponent<PaintingData>();
		_diagManager = GameObject.Find("DialogueStarter").GetComponent<DialogueManager>();
    }

    void Update()
    {
		if(!inspectActive) return;
		
		/*	
		_hoveredId = TMPro.TMP_TextUtilities.FindIntersectingCharacter(
				_diagManager.DialogueTextOutput,
				Input.mousePosition,
				null,
				true
		);
		*/

		int kwStart = _db.dbEntries[_currPainting].keyStart;
		int kwEnd   = _db.dbEntries[_currPainting].keyEnd;

		_keywordHovered = (_hoveredId >= kwStart &&  _hoveredId <= kwEnd);
    }
}

