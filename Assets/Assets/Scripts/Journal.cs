using UnityEngine;

public class Journal : MonoBehaviour
{
	// Bit field enum for state flags
	public enum flag : byte {
		f_show = 0x01,	// Show journal
		f_lock = 0x02	// Lock journal
	}

	// All flags off on start
	public byte flags = (0);

	private Canvas _canvas; 
	private PaintingData _paintingData;

	public bool[] KeywordCollected;
	
    void Start()
    {
       	_canvas = GetComponent<Canvas>(); 
		_paintingData = GameObject.Find("Paintings").GetComponent<PaintingData>();

		KeywordCollected = new bool[_paintingData.dbEntries.Length];	
    }

    void Update()
    {
		// Journal opening:
		// todo: Add inputs with InputSystem, 
		// toggle show flag
		// Only if not locked
    	if(!FlagCheck((byte)flag.f_lock)) 
		{
		}
    }

	// *
	// Flag helper functions:
	// note: type casting being necessary kinda sad /,:
	//
	// Check if flag is on/off 
	public bool FlagCheck(byte flag) 	
	{ return ((flags & (byte)flag) != 0); }

	// Set flag to on
	public void FlagSetOn(byte flag) 	
	{ flags |= (byte)flag; }

	// Set flag to off
	public void FlagSetOff(byte flag) 	
	{ flags &= (byte)~flag; }

	// Toggle flag on/off
	public void FlagToggle(byte flag)	
	{ flags ^= (byte)flag; }
}

