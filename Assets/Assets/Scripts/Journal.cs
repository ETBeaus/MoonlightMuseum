using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.LowLevel;
using UnityEngine.Rendering;
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
	public byte texId;
	public Vector2 position;

	public StickerEntry(byte _texId, Vector2 _pos) { texId = _texId; position = _pos; }
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

	public WordEntry[] wordEntries;
	public StickerEntry[] stickerEntries;

	private byte _wordCollCount = 0;
	private byte _stickerCollCount = 0;

	private Button _button;  

	public Texture2D[] stickerTextures = new Texture2D[12];
	public Texture2D stickerBG;
	public Image Img_stickerBG;
	
    void Start()
    {
		_canvas = GameObject.Find("JournalCanvas").GetComponent<Canvas>();
		_paintingData = GameObject.Find("Paintings").GetComponent<PaintingData>();
		_handler = GameObject.Find("HandlerObject").GetComponent<Handler>();

		wordEntries = new WordEntry[_paintingData.dbEntries.Length];	
		stickerEntries = new StickerEntry[_paintingData.dbEntries.Length];

		_button = GameObject.Find("JournalButton").GetComponent<Button>();
		_button.onClick.AddListener(() => {
				FlagToggle(flag.f_show);
		});

		stickerBG = new Texture2D(200, 500);
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
		for(byte i = 0; i < _wordCollCount; i++)
		{
			// Don't add entry if it exists already
			bool skipAdd = (wordEntries[i].painting.keyWord == painting.keyWord);	
			if(skipAdd) return;
		}

		// Add entry, increment collected count
		wordEntries[_wordCollCount++] = new WordEntry(painting);

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
		for(byte i = 0; i < _wordCollCount; i++)
			output += " - " + wordEntries[i].painting.keyWord + "\n";

		// Overwrite tm text with output string
		_canvas.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = output;	
	}

	public void AddStickerEntry()
	{
		sbyte texId = (sbyte)UnityEngine.Random.Range(0, 11);
		
		Vector2 pos = new Vector2(
			UnityEngine.Random.Range(80, 200 - 80),
			UnityEngine.Random.Range(80, 500 - 80)
		);

		stickerEntries[_stickerCollCount++] = new StickerEntry((byte)texId, pos);
		StickerTexUpdate();
	}

	public void StickerTexUpdate()
	{
		StickerEntry newSticker = stickerEntries[_stickerCollCount-1];
		Texture2D tex = stickerTextures[newSticker.texId];
		
		// Get pixels
		Color[] bgPX = stickerBG.GetPixels();
		Color[] stickerPX = tex.GetPixels();

		// Copy sticker pixels to background
		for(UInt16 y = 0; y < (UInt16)(tex.height); y++) {
			for(UInt16 x = 0; x < (UInt16)(tex.width); x++) {
				// Ignore transparent pixels
				if(stickerPX[x + y * tex.width].a == 0) continue;	

				bgPX[(UInt16)((x + newSticker.position.x) + (y + newSticker.position.y) * stickerBG.width)]
					= stickerPX[x + y * tex.width];
			}
		}

		// Set new pixels, apply
		stickerBG.SetPixels(bgPX);	
		stickerBG.Apply();
		Img_stickerBG.sprite = Sprite.Create(stickerBG, new Rect(0, 0, stickerBG.width, stickerBG.height), Vector2.one * 0.5f);
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

