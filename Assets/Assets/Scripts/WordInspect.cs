using UnityEngine;
using TMPro; 
using UnityEngine.InputSystem;

public class WordInspect : MonoBehaviour 
{
	private PaintingData _db;

	public int _currPainting;

	public bool inspectActive = false;

	public bool _keywordHovered; 
	public int _hoveredId;

	private GameObject _eventSystem; 

	public TextMeshProUGUI textMesh;
	
    void Start()
    {
		_db = GameObject.Find("Paintings").GetComponent<PaintingData>();
		_eventSystem = GameObject.Find("EventSystem");
    }

    void Update()
    {
		if(!inspectActive) return;

		Vector2 mousePos = Mouse.current.position.ReadValue();
		
		_hoveredId = TMPro.TMP_TextUtilities.FindIntersectingCharacter(
				textMesh,
				mousePos,
				null,
				true
		);

		int kwStart = _db.dbEntries[_currPainting].keyStart;
		int kwEnd   = _db.dbEntries[_currPainting].keyEnd;

		_keywordHovered = (_hoveredId >= kwStart &&  _hoveredId <= kwEnd);
    }
}
