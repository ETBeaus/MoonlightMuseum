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
	private bool _clickDown;
	private bool _clickUp;

	private int _clickedKeyword = -1;
	private string[] _poemFinal, _poemTexts;
	
    void Start() 
	{
		_canvas = GameObject.Find("JournalCanvas").GetComponent<Canvas>();
		_paintingData = GameObject.Find("Paintings").GetComponent<PaintingData>();
		_handler = GameObject.Find("HandlerObject").GetComponent<Handler>();

		wordEntries = new WordEntry[_paintingData.dbEntries.Length];	
		stickerEntries = new StickerEntry[_paintingData.dbEntries.Length + 1];

		_button = GameObject.Find("JournalButton").GetComponent<Button>();
		_button.onClick.AddListener(() => {
				FlagToggle(flag.f_show);
		});

		stickerBG = new Texture2D(200, 500);
		stickerBG.wrapMode = TextureWrapMode.Clamp;

		PoemWords = new List<String>();
		_poemFinal = SetPoemFinal();
		_poemTexts = SetPoemStrings();
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
			if(_onPoemBG) 
				AddToPoem(_poemTexts[_clickedKeyword]);
			
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
			UnityEngine.Random.Range(80, 220 - 80),
			UnityEngine.Random.Range(80, 580 - 80));

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
		KeywordTextFormatClear();
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

	private void KeywordTextFormatClear()
	{
		if(_kwTextOriginal == null) return;
		KeyWordTM.text = _kwTextOriginal;
	}

	private void AddToPoem(string word)
	{
		if(PoemWords.Contains(word) || word == String.Empty) return;

		PoemWords.Add(word);
		UpdatePoemText();

		CursorText.text = String.Empty;
	}

	private void UpdatePoemText()
	{
		string text = string.Empty;

		for(int i = 0; i < PoemWords.Count; i++)
			text = text + PoemWords[i] + '\n';

		_poemTextOriginal = text;
		PoemTextFormatClear();
	}

	private void PoemTextFormatClear()
	{
		if(_poemTextOriginal == null) return;
		PoemTM.text = _poemTextOriginal;
	}

	private void PoemTextFormatApply(int wordId)
	{
		PoemTextFormatClear();

		if(_hoveredPoemWordCurr == -1) return; 

		TMP_TextInfo textInfo = PoemTM.textInfo;
		TMP_WordInfo wordInfo = textInfo.wordInfo[wordId];

		string keyword = String.Empty;
		for(int i = 0; i < PoemWords.Count; i++)
		{
			if(PoemWords[i].Contains(wordInfo.GetWord()))
				keyword = PoemWords[i];
		}

		int startId = _poemTextOriginal.IndexOf(keyword);
		int endId = startId + keyword.Length;

		string textLocal = PoemTM.text[startId..(endId+1)];
		string formatted = String.Empty;

		formatted = (
			_poemTextOriginal[0..startId] +
			"<u>" +
			textLocal +
			"</u>" + 
			_poemTextOriginal[(endId+1)..]
		);

		PoemTM.text = formatted;
	}

	private void PoemUpdate()
	{
		if(!_onPoemBG) return;

		TMP_TextInfo textInfo = PoemTM.textInfo; 

		_hoveredPoemWordPrev = _hoveredPoemWordCurr;
		_hoveredPoemWordCurr = TMP_TextUtilities.FindIntersectingWord(PoemTM, _mousePos, null);

		if(_hoveredPoemWordCurr != _hoveredPoemWordPrev) 
			PoemTextFormatApply(_hoveredPoemWordCurr);

		bool _hoverValid = (_hoveredPoemWordCurr > -1 && _hoveredPoemWordCurr < textInfo.wordCount && _onPoemBG);

		if(_hoverValid && _clickDown)
		{
			TMP_WordInfo wordInfo = textInfo.wordInfo[_hoveredPoemWordCurr];
			string word = wordInfo.GetWord();

			for(int i = 0; i < PoemWords.Count; i++)
			{
				if(PoemWords[i].Contains(word))
				{
					_draggedPoemWord = i;
					CursorText.text = PoemWords[i];
					break;
				}
			}
		}
		else if(_draggedPoemWord > -1 && _clickUp && _hoverValid)
		{
			TMP_WordInfo wordInfo = textInfo.wordInfo[_hoveredPoemWordCurr];
			string word = wordInfo.GetWord();

			int swapId = -1;
			for(int i = 0; i < PoemWords.Count; i++) 
			{
				if(PoemWords[i].Contains(word)) 
				{
					swapId = i;
					break;
				}
			}

			if(swapId > -1)
			{
				string swap = PoemWords[swapId];
				PoemWords[swapId] = PoemWords[_draggedPoemWord];
				PoemWords[_draggedPoemWord] = swap;

				UpdatePoemText();
			}

			CursorText.text = string.Empty;
			_draggedPoemWord = -1;
		}
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
		flags ^= (byte)flag;
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
			if(PoemWords[i] != _poemFinal[i])
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
			"Trees of green and blue sky, Where have we been, you and I?\n",
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

	private string[] SetPoemStrings()
	{
		return new string[] {
			// 0, Bluffs and Beach, Turkey Point, Lake Erie
			"You may not realize, but we all hold a spark, We can capsize this future which all seems so dark.\n",
			// 1, Untitled (Whitefish River Looking South to Manitoulin Island)
			"Trees of green and blue sky, Where have we been, you and I?\n",
			// 2, Prairie Fantasy
			"Even we baby beavers can make a change, in this tapestry we weave here, in the digital age.\n",
			// 3, Underpass, Montreal
			"This is our land, but it's always been theirs, too, Outstretch your hand, these repairs are past due.\n", 
			// 4, Manitoba Landscape
			"When rivers are dried, and the needles have fell, Who will cry, and who will say \"\"oh, well.\"\"\n",
			// 5, British Columbia Landscape
			"This is my land, and yours too, and when we stand together, there's nothing we can't do.\n",
			// 6, Logged-Over Hillside
			"When forests are burnt, and species become myth, Those with backs turned with bills in their fist,\n",
			// 7, Strait of Juan de Fuca
			"In saving the rivers, the sea and the beasts, In keeping money from growing on trees.\n",
			// 8, The Ferry Trail, North Saskatchewan River
			"But it doesn't have to be this way, you see, It's not too late for us to succeed.\n",
			// 9, Cove Fields, Quebec
			"\"\"What can we do?\"\" so say we all, Learn, pursue, speak up when you can, for no voice is too small.\n",
			// 10, Fishing Stages, Newfoundland
			"Coast and sea, drifts of snow white, We didn't come between what happened out of sight.\n",
			// 11, The Jack Pine
			"Will stand in the ashes, with the world turned to profit, And the masses all know that this was it.\n"
		};
	}
}

