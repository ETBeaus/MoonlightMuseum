using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

[Serializable]
public class WordEntry
{
	// note:
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
		f_show			= 0x01,	// Show journal
		f_lock			= 0x02,	// Lock journal view
		f_poem			= 0x04,	// Poem edit mode
		f_dragsticker 	= 0x08,
		f_dragword  	= 0x10	 
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

	public GameObject StickerBgObject;
	public GameObject StickerSpritePrefab;

	private int _hoveredWordEntry;
	//private int[] _keywordNewLineIds = new int[12];
	
	public TextMeshProUGUI KeyWordTM; 
	public TextMeshProUGUI PoemTM;
	
	public GameObject KeywordBox;
	public GameObject StickerBox;
	public GameObject PoemBox;

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
		stickerBG.wrapMode = TextureWrapMode.Clamp;
    }

    void Update()
    {
		// Journal opening:
		// todo: Add inputs with InputSystem, 
		// toggle show flag only if not locked
    	if(!FlagCheck(flag.f_lock)) 
		{
			// If show journal, enable canvas
			_canvas.enabled = (FlagCheck(flag.f_show));
		}

		if(!FlagCheck(flag.f_show)) return;
		Vector2 mousePos = Mouse.current.position.ReadValue();

		_hoveredWordEntry = -1;
		if(CursorAABB(mousePos, KeywordBox.GetComponent<Rect>()))
		{
			TMP_TextInfo textInfo = KeyWordTM.textInfo;
			
			_hoveredWordEntry = TMP_TextUtilities.FindIntersectingLine(
					KeyWordTM,
					mousePos,
					null
			);

			if(_hoveredWordEntry > _wordCollCount) _hoveredWordEntry = -1;
		}
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
		KeyWordTM.text = output;
	}

	public void AddStickerEntry()
	{
		sbyte texId = (sbyte)UnityEngine.Random.Range(0, 11);
		
		bool texIdFree = true;
		for(byte i = 0; i < _stickerCollCount; i++)
		{
			if(stickerEntries[i].texId == texId)
			{
				texIdFree = false;
				break;
			}
		}

		while(!texIdFree)
		{
			texId++;
			if(texId > 11) texId = 0;
			
			bool isFree = true;
			for(byte i = 0; i < _stickerCollCount; i++)
			{
				if(texId == stickerEntries[i].texId)
				{
					isFree = false;
				}
			}

			if(isFree)
			{
				texIdFree = true; 
				break;
			}
		}
		
		Vector2 pos = new Vector2(
			UnityEngine.Random.Range(80, 200 - 80),
			UnityEngine.Random.Range(80, 500 - 80)
		);

		stickerEntries[_stickerCollCount++] = new StickerEntry((byte)texId, pos);
		
		GameObject newSticker = Instantiate(StickerSpritePrefab, StickerBgObject.transform);
		newSticker.transform.localPosition = pos;
		newSticker.transform.localScale = Vector3.one;

		Texture2D tex = stickerTextures[texId];
		
		Sprite newSprite = Sprite.Create(
			tex,
			new Rect(0, 0, tex.width, tex.height),
			Vector2.one * 0.5f, 
			1f
		);

		newSticker.GetComponent<Image>().sprite = newSprite;
	}

	private void KeywordTextFormatApply(int line) 
	{
		
	}

	private void KeywordTextFormatClear()
	{
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

	private bool CursorAABB(Vector2 cursorPos, Rect rect)
	{
		return ( 
			cursorPos.x >= rect.x 				&&
			cursorPos.x <= rect.x + rect.width  &&
			cursorPos.y >= rect.y 				&&
			cursorPos.y <= rect.y + rect.height );
	}
}

