using System;
using System.Collections.Generic;
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

	private int _hoveredWordEntryPrev = -1, _hoveredWordEntryCurr = -1;
	private int _hoveredPoemWordPrev = -1, _hoveredPoemWordCurr = -1;
	
	public TextMeshProUGUI KeyWordTM, PoemTM;
	
	public GameObject KeywordBox, StickerBox, PoemBox;

	private string _kwTextOriginal, _kwTextFormatted;
	private string _poemTextOriginal, _poemTextFormatted;
	
	public TextMeshProUGUI CursorText;
	public List<string> PoemWords;

	private int _draggedPoemWord = -1;
	private bool _onPoemBG = false;

	private Vector2 _mousePos; 
	private bool _clickDown, _clickUp;

	private int _clickedKeyword = -1;
	private string[] _poemFinal, _poemTexts;

	private int[] blankWordIds = new int[12];
	private int _blankHoverCurr = -1, _blankHoverPrev = -1;

	private string _outline; 

	private string[] _correctKeywords = {
		"Green and Blue",
		"Sea, Drifts",
		"Repairs are past",
		"Who will say",
		"become myth",
		"the ashes",
		"not too late",
		"the rivers, the sea",
		"a spark",
		"this tapestry",
		"pursue",
		"stand together"
	};
	
    void Start() 
	{
		_canvas = GameObject.Find("JournalCanvas").GetComponent<Canvas>();
		_paintingData = GameObject.Find("Paintings").GetComponent<PaintingData>();
		_handler = GameObject.Find("HandlerObject").GetComponent<Handler>();

		wordEntries = new WordEntry[_paintingData.dbEntries.Length];	
		stickerEntries = new StickerEntry[_paintingData.dbEntries.Length + 1];

		_button = GameObject.Find("JournalButton").GetComponent<Button>();
		_button.onClick.AddListener(() => { FlagToggle(flag.f_show); });

		PoemWords = new List<String>();
		_poemFinal = SetPoemFinal();

		_outline = PoemOutline();
		_poemTextOriginal = _outline; 
		PoemTM.text = _poemTextOriginal;		
		FindBlank();

		//PoemFillCorrect();
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
		_mousePos = Mouse.current.position.ReadValue();

		_hoveredWordEntryPrev = _hoveredWordEntryCurr;
		_hoveredWordEntryCurr = -1;

		int _hoveredWordEntryChar = -1;
		_hoveredWordEntryChar = TMP_TextUtilities.FindIntersectingCharacter(KeyWordTM, _mousePos, null, true);

		if(_hoveredWordEntryChar > -1)	
			_hoveredWordEntryCurr = TMP_TextUtilities.FindIntersectingLine(KeyWordTM, _mousePos, null);

		if(_hoveredWordEntryCurr != _hoveredWordEntryPrev)
			KeywordTextFormatApply(_hoveredWordEntryCurr);

		bool hoverWordValid = (_hoveredWordEntryCurr > -1 && _hoveredWordEntryCurr < _wordCollCount);

		_clickDown = Mouse.current.leftButton.wasPressedThisFrame;
		_clickUp   = Mouse.current.leftButton.wasReleasedThisFrame;

		CursorText.transform.position = _mousePos;

		_onPoemBG = RectTransformUtility.RectangleContainsScreenPoint(PoemBox.GetComponent<Image>().rectTransform, _mousePos, null);
		PoemUpdate();		

		if(hoverWordValid && _clickDown)
		{
			FlagSetOn(flag.f_dragword);

			if(CursorText.text != wordEntries[_hoveredWordEntryCurr].painting.keyWord)
			{
				CursorText.text = wordEntries[_hoveredWordEntryCurr].painting.keyWord;
				_clickedKeyword = _hoveredWordEntryCurr;
			}

		} 
		else if(FlagCheck(flag.f_dragword) && _clickUp)
		{
			if(_blankHoverCurr != -1)
			{
				if(_onPoemBG && CursorText.text == _correctKeywords[_blankHoverCurr]) 
					AddToPoem(CursorText.text, -1);
			}
			
			CursorText.text = string.Empty;
			FlagSetOff(flag.f_dragword);
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
		_kwTextOriginal = output;
		KeyWordTM.text = _kwTextOriginal;
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
			UnityEngine.Random.Range(80, 500 - 80));

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

	private void TextFormatClear(string original, TextMeshProUGUI tm)	
	{
		if(original == null) return;
		tm.text = original;
	}

	private void KeywordTextFormatApply(int line) 
	{
		TextFormatClear(_kwTextOriginal, KeyWordTM);
		if(line < 0 || line > _wordCollCount - 1 || _kwTextOriginal == null) return;
		
		string[] lines = _kwTextOriginal.Split('\n');
		int lineEnd = lines[line].Length;

		lines[line] = $"<u>{lines[line]}</u>";

		string formatted = string.Empty;
		
		for(int i = 0; i < _wordCollCount; i++)
			formatted = formatted + $"{lines[i]}\n";

		_kwTextFormatted = formatted;
		KeyWordTM.text = _kwTextFormatted;
	}
	
	// *
	// Flag helper functions:
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
	{ 
		// Invert flag value
		flags ^= (byte)flag;

		// Check win condition when "show" flag gets set to off
		if(flag == flag.f_show && !FlagCheck(flag.f_show)) TestEndScene();
	}

	private void TestEndScene()
	{
		Debug.Log("testing end...");
		if(PoemWords.Count < _poemFinal.Length) 
		{
			Debug.Log("not enough words");
			return;
		}

		for(int i = 0; i < PoemWords.Count; i++)  
		{
			string inputWord = PoemWords[i].ToLower().Trim();
			string correctWord = _correctKeywords[i].ToLower().Trim();
			
			if(inputWord != correctWord)
			{
				Debug.Log("Out of order");
				return;
			}
		}

		var gm = GameObject.Find("GameManager").GetComponent<GameManager>();
		gm.HasCompletedPoem = true;
	}

	private string[] SetPoemFinal()
	{
		return new string[] {
			"Trees of Green and Blue sky, Where have we been, you and I?\n",
			"Coast and sea, drifts of snow white, We didn't come between what happened out of sight.\n",
			"This is our land, but it's always been theirs, too, Outstretch your hand, these repairs are past due.\n",
			"When rivers are dried, and the needles have fell, Who will cry, and who will say \"\"oh, well.\"\"\n",
			"When forests are burnt, and species become myth, Those with backs turned with bills in their fist,\n",
			"Will stand in the ashes, with the world turned to profit, And the masses all know that this was it.\n",
			"But it doesn't have to be this way, you see, It's not too late for us to succeed.\n",
			"In saving the rivers, the sea and the beasts, In keeping money from growing on trees.\n",
			"You may not realize, but we all hold a spark, We can capsize this future which all seems so dark.\n",
			"Even we baby beavers can make a change, in this tapestry we weave here, in the digital age.\n",
			"\"\"What can we do?\"\" so say we all, Learn, pursue, speak up when you can, for no voice is too small.\n",
			"This is my land, and yours too, and when we stand together, there's nothing we can't do.\n"
		};	
	}

	private String PoemOutline()
	{
		return (
				"Trees of ----- sky, Where have we been, you and I?\n" + 
				"Coast and ----- of snow white, We didn't come between what happened out of sight.\n" +
				"This is our land, but it's always been theirs, too, Outstretch your hand, these ----- due.\n" + 
				"When rivers are dried, and the needles have fell, Who will cry, and ----- \"\"oh, well.\"\"\n" + 
				"When forests are burnt, and species -----, Those with backs turned with bills in their fist,\n" + 
				"Will stand in -----, with the world turned to profit, And the masses all know that this was it.\n" + 
				"But it doesn't have to be this way, you see, It's ----- for us to succeed." +
				"In saving the ----- and the beasts, In keeping money from growing on trees." +
				"You may not realize, but we all hold -----, We can capsize this future which all seems so dark." + 
				"Even we baby beavers can make a change, in ----- we weave here, in the digital age." + 
				"\"\"What can we do?\"\" so say we all, Learn, -----, speak up when you can, for no voice is too small." + 
				"This is my land, and yours too, and when we -----, there's nothing we can't do."  
	   	);
	}

	private void FindBlank()
	{
		string poem = PoemOutline(), blank = "-----";
		int count = 0, id = 0;

		while((id = poem.IndexOf(blank, id)) != -1)	
		{
			blankWordIds[count++] = id;
			id += blank.Length;
		}

		/*
		char[] chars = _outline.ToCharArray();
		for(int i = 0; i < count; i++)
		{
			chars[blankWordIds[i]] = '*'; 
			chars[blankWordIds[i]+blank.Length] = '*';
		}

		_outline = chars.ArrayToString();
		_poemTextOriginal = _outline;
		PoemTM.text = _poemTextOriginal;
		*/
	}
	
	private void PoemUpdate()
	{
		int len = "-----".Length;

		_blankHoverPrev = _blankHoverCurr; 
		_blankHoverCurr = -1;

		for(int i = 0; i < blankWordIds.Length; i++)
		{
			int start = blankWordIds[i]; 
			int end = start + len;

			//int charHovId = TMP_TextUtilities.FindIntersectingCharacter(PoemTM, _mousePos, Camera.main, true);
			int charHovId = TMP_TextUtilities.FindIntersectingCharacter(PoemTM, _mousePos, null, true);
			if(!_onPoemBG) charHovId = -1;
			
			if(charHovId >= start && charHovId < end && charHovId != -1)
			{
				_blankHoverCurr = i;
				break;
			}
		}

		if(_blankHoverCurr != _blankHoverPrev)
		{
			if(_onPoemBG && _blankHoverCurr > -1 && _blankHoverCurr < blankWordIds.Length)
			{
				int open = blankWordIds[_blankHoverCurr]; 
				int close = Math.Min(blankWordIds[_blankHoverCurr] + len, _outline.Length);
				PoemTextFormatApply(open, close, "<color=#48cae4>", "</color>");
			}
			else
				TextFormatClear(_poemTextOriginal, PoemTM);
		}

		if(_onPoemBG && _blankHoverCurr != -1 )
		{
			if(CursorText.text == _correctKeywords[_blankHoverCurr])
				CursorText.color = Color.green;
		}
		else 
		{
			CursorText.color = Color.red;
		}
	}

	void PoemTextFormatApply(int startId, int endId, string tagOpen, string tagClose)
	{
		if(_poemTextOriginal == null || startId < 0 || endId >= _poemTextOriginal.Length) return;
		
		string textLocal = _poemTextOriginal[startId..(endId+1)];
		string formatted = String.Empty;

		formatted = (
			_poemTextOriginal[0..startId] +
			tagOpen +
			textLocal +
			tagClose + 
			_poemTextOriginal[(endId+1)..]
		);

		PoemTM.text = formatted;
	}

	void AddToPoem(string word, int autoId)
	{
		if(autoId == -1 && (_blankHoverCurr < 0 || _blankHoverCurr >= blankWordIds.Length)) return;
		if(PoemWords.Contains(word) || word == String.Empty) return;
		PoemWords.Add(word);

		//int id = blankWordIds[_blankHoverCurr];
		int id = blankWordIds[(autoId == -1) ? _blankHoverCurr : autoId];
		int len = "-----".Length;

		_poemTextOriginal = _poemTextOriginal.Remove(id, len).Insert(id, word);
		PoemTM.text = _poemTextOriginal;

		int offset = word.Length - len;
		for(int i = _blankHoverCurr + 1; i < blankWordIds.Length; i++)
			blankWordIds[i] += offset;

		_blankHoverCurr = -1;
		_outline = _poemTextOriginal;
	}
	
	void PoemFillCorrect()	
	{
		for(int i = 0; i < _correctKeywords.Length; i++)
			AddToPoem(_correctKeywords[i], i);
	}
}

