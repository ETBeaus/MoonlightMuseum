using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[Serializable]
public class WordEntry
{
	// jnote:
	// Currently, class only stores painting entry for showing keyword text...
	// More could be added later, different colors, pictures, etc.
	public DB_Entry painting;
	public WordEntry(DB_Entry _painting) { painting = _painting; }
}

[Serializable]
public class StickerEntry
{
}

public class Journal : MonoBehaviour
{
	// Bit field enum for state flags
	public enum flag : byte {
		f_show = 0x01,	// Show journal
		f_lock = 0x02,	// Lock journal view
		f_poem = 0x04	// Poem edit mode
	}

	// All flags off on start
	public byte flags = (0);

	private Canvas _canvas; 
	private PaintingData _paintingData;
	private Handler _handler;

	public WordEntry[] entries;

	private int _collectCount = 0;

	private Button _button;  
	
    void Start()
    {
		_canvas = GameObject.Find("JournalCanvas").GetComponent<Canvas>();
		_paintingData = GameObject.Find("Paintings").GetComponent<PaintingData>();
		_handler = GameObject.Find("HandlerObject").GetComponent<Handler>();

		entries = new WordEntry[_paintingData.dbEntries.Length];	

		_button = GameObject.Find("JournalButton").GetComponent<Button>();
		_button.onClick.AddListener(() => {
				FlagToggle(flag.f_show);
		});
    }

    void Update()
    {
		// Journal opening:
		// todo: Add inputs with InputSystem, 
		// toggle show flag
		// Only if not locked
    	if(!FlagCheck(flag.f_lock)) 
		{
		}

		// If show journal, enable canvas
		_canvas.enabled = (FlagCheck(flag.f_show));
    }

	public void AddKeywordEntry(DB_Entry painting) 
	{
		// Check if already in journal
		for(byte i = 0; i < _collectCount; i++)
		{
			// Don't add entry if it exists already
			bool skipAdd = (entries[i].painting.keyWord == painting.keyWord);	
			if(skipAdd) return;
		}

		// Add entry, increment collected count
		entries[_collectCount++] = new WordEntry(painting);

		// Update text mesh
		UpdateKeywordText();
		
		// Close painting description	
		_handler.PaintingTextClose();
	}

	public void UpdateKeywordText()
	{
		// Init output string
		string output =	string.Empty;

		// For each journal entry, print keyword + newline char
		for(byte i = 0; i < _collectCount; i++)
			output += " - " + entries[i].painting.keyWord + "\n";

		// Overwrite tm text with output string
		_canvas.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = output;	
	}

	// *
	// Flag helper functions:
	// *note: type casting being necessary kinda sad /,:
	//
	// Check if flag is on/off 
	public bool FlagCheck(Journal.flag flag) 	
	{ return ((flags & (byte)flag) != 0); }

	// Set flag to on
	public void FlagSetOn(Journal.flag flag) 	
	{ flags |= (byte)flag; }

	// Set flag to off
	public void FlagSetOff(Journal.flag flag) 	
	{ flags &= (byte)~flag; }

	// Toggle flag on/off
	public void FlagToggle(Journal.flag flag)	
	{ flags ^= (byte)flag; }
}

