using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[Serializable]
public class JournalEntry
{
	// jnote:
	// Currently, class only stores painting entry for showing keyword text...
	// More could be added later, different colors, pictures, etc.
	public DB_Entry painting;
	public JournalEntry(DB_Entry _painting) { painting = _painting; }
}

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

	public JournalEntry[] entries;

	private int _collectCount = 0;

	private Button _button;  
	
    void Start()
    {
		_canvas = GameObject.Find("JournalCanvas").GetComponent<Canvas>();
		_paintingData = GameObject.Find("Paintings").GetComponent<PaintingData>();

		entries = new JournalEntry[_paintingData.dbEntries.Length];	

		_button = GameObject.Find("JournalButton").GetComponent<Button>();
		_button.onClick.AddListener(() => {
				FlagToggle((byte)flag.f_show);
		});
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

		// If show journal, enable canvas
		_canvas.enabled = (FlagCheck((byte)flag.f_show));
    }

	public void AddEntry(DB_Entry painting) 
	{
		// Check if already in journal
		for(byte i = 0; i < _collectCount; i++)
		{
			// Don't add entry if it exists already
			bool skipAdd = (entries[i].painting.keyWord == painting.keyWord);	
			if(skipAdd) return;
		}

		// Add entry, increment collected count
		entries[_collectCount++] = new JournalEntry(painting);
		UpdateText();
	}

	public void UpdateText()
	{
		// Init output string
		string output =	string.Empty;

		// For each journal entry, print keyword + newline char
		for(byte i = 0; i < _collectCount; i++)
			output += entries[i].painting.keyWord + "\n";

		// Overwrite tm text with output string
		_canvas.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = output;	
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

