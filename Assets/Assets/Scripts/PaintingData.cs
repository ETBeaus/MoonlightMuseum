using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class DB_Entry
{
	// Start and end indices of keyword in description string
	// Used for word inspection
    public int KeyStart;
    public int KeyEnd;

    public string Title;
    public string Link;
    public string Description;
    public string KeyWord;
    public string DataPoint;
    public string PainterName;

	public DB_Entry(string _title, string _link, string _description, string _keyword, string _datapoint, string _painterName)
	{
		Title = _title;
		Link = _link;
		Description = _description;
		KeyWord = _keyword;
		DataPoint = _datapoint;
		PainterName = _painterName;
	}
}

public class PaintingData : MonoBehaviour
{
    const byte _entryCount = 12;
    public DB_Entry[] dbEntries = new DB_Entry[_entryCount];

    void Start()
    {
		// *note:
		// SreamingAssets path needs to be fixed,
		// also no file io for web builds...
        //DB_Read("db_corrected.txt");
		DB_HardCodeEntries();
    }

    private void DB_Read(string filePath)
    {
		// Open file stream
        StreamReader reader = new StreamReader($"Assets/StreamingAssets/{filePath}");
        if(reader == null)
        {
            Debug.LogError($"ERROR: file: {filePath} does not exist");
            return;
        }

		// Read file, track current entry id
        string line;
        sbyte id = -1;
        while((line = reader.ReadLine()) != null)
        {
            DB_ParseLine(line, ref id);
        }
		
		// Close file stream
		reader.Close();

		// Set keyword start + end indices for all entries
		SetKeyIds();
    }

    private void DB_ParseLine(string line, ref sbyte currId)
    {
		// Don't parse empty lines
        if(line.Length == 0) return;

		// Get '=' index in line string
		// Split string later into key and value
        int eq = -1;
        eq = line.IndexOf('=');

        if(eq == -1)
        {
			// Change current entry 
            if(line[0] == '[')
            {
				// Get entry title
                int braceCloseId = line.IndexOf(']');
				string title = line[1..braceCloseId];

				// Increment current entry, set title 
                dbEntries[++currId].Title = title;
            }

            return;
        }

        DB_Entry entry = dbEntries[currId];

		// Split string at eq
        string key = line.Substring(0, eq).Trim();
        string val = line.Substring(eq + 1).Trim();

		// Set appopriate value
        switch(key)
        {
            case "link":			entry.Link = val;				break;
            case "desc":			entry.Description = val;		break;
            case "key_word":		entry.KeyWord = val;			break;
            case "data_point": 		entry.DataPoint = val;			break;
            case "painter_name": 	entry.PainterName = val;		break;
        }
    }

    private void SetKeyIds()
    {
		// Iterate through all entries
        for(byte i = 0; i < _entryCount; i++)
        {
            DB_Entry entry = dbEntries[i];

			// Iterate through description string
			for(int j = 0; j < entry.Description.Length; j++) {
				// Early out if remaining chars too small
				if(j + entry.KeyWord.Length > entry.Description.Length) break;	

				// Get string portion 
				string str = entry.Description[j..(j+entry.KeyWord.Length)].ToLower();

				// If portion + keyword are equal, set key indices
				if(str.Equals(entry.KeyWord.ToLower()))
				{
					entry.KeyStart = j;
					entry.KeyEnd = j + entry.KeyWord.Length;
				}
			}
        }
    }

	private void DB_HardCodeEntries()
	{
		dbEntries[0] = new DB_Entry(
			"Bluffs and Beach, Turkey Point, Lake Erie",
			"https://www.gallery.ca/collection/artwork/bluffs-and-beach-turkey-point-lake-erie",
			"Along this beach, there is, brilliantly, a spark of colour. When you look at this painting, can you smell the flowers? Feel the breeze and the sand under your feet? Hear the swell of Lake Erie's waves against the shore?",
			"a spark",
			"Five-lined skink (Great Lakes/St. Lawrence population)",
			"Eva Brook Donly"
		);

		dbEntries[1] = new DB_Entry(
			"Untitled (Whitefish River Looking South to Manitoulin Island)",
			"https://www.gallery.ca/collection/artwork/untitled-whitefish-river-looking-south-to-manitoulin-island",
			"This piece is Untitled, but it depicts Manitoulin Island, which is found in lake Huron. Its beautiful green and blue hues give the viewer a sense of freedom afforded only by vastly open spaces.",
			"Green and Blue",
			"Northern leopard frog (Rocky Mountain population)",
			"Franklin Carmichael"
		);

		dbEntries[2] = new DB_Entry(
			"Prairie Fantasy",
			"https://www.gallery.ca/collection/artwork/prairie-fantasy",
			"What kind of species might live in this tapestry? The prairies, so unassuming at first, host more life than you could imagine!",
			"this tapestry",
			"Burrowing owl",
			"L.L. FitzGerald"
		);

		dbEntries[3] = new DB_Entry(
			"Underpass, Montreal",
			"https://www.gallery.ca/collection/artwork/underpass-montreal",
			"Despite being grey, our cities are home to many species; you just have to keep your eyes peeled. Though this underpass looks like repairs are past needed, you probably wouldn't guess that it's the perfect home for a surprising species.",
			"Repairs are past",
			"Common nighthawk",
			"Ghitta Caiserman"
		);

		dbEntries[4] = new DB_Entry(
			"Manitoba Landscape",
			"https://www.gallery.ca/collection/artwork/manitoba-landscape",
			"Paintings in this style are called \"pointilist,\" and you might think, \"Who will say that this is a landscape?\" Believe us, sometimes it's hard to see! But look closely, let the colours blend in to one another, and see what your mind comes up with.",
			"Who will say",
			"Dusky dune moth",
			"L.L. FitzGerald"
		);

		dbEntries[5] = new DB_Entry(
			"British Columbia Landscape",
			"https://www.gallery.ca/collection/artwork/british-columbia-landscape-0",
			"Once upon a time, many years ago, Canada's forests were dense and dark, with trees that stand together like little tin soldiers, shoulder to shoulder. When you look at this piece, what do you feel? What do you see? Is it as simple as tree upon tree?",
			"stand together",
			"Taylor's checkerspot",
			"Emily Carr"
		);

		dbEntries[6] = new DB_Entry(
			"Logged-Over Hillside",
			"https://www.gallery.ca/collection/artwork/logged-over-hillside",
			"The forests of the Canadian past have become myth; in the past, they had treelines so dense that it was like looking into another world. This piece shows that loss, that which we have for so long now been missing.",
			"become myth",
			"Mountain beaver",
			"Emily Carr"
		);

		dbEntries[7] = new DB_Entry(
			"Strait of Juan de Fuca",
			"https://www.gallery.ca/collection/artwork/strait-of-juan-de-fuca",
			"Bright, bold brush strokes evoke a feeling of movement in the rivers, the sea, and make it seem like the waves might spill right out of the frame and wash the viewer away!",
			"the rivers, the sea",
			"North Atlantic right whale AND North Pacific right whale",
			"Emily Carr"
		);

		dbEntries[8] = new DB_Entry(
			"The Ferry Trail, North Saskatchewan River",
			"https://www.gallery.ca/collection/artwork/the-ferry-trail-north-saskatchewan-river",
			"This sleepy landscape reminds us: it's not too late - it's never too late! Take it slow and take it easy, sit back and let the clouds crawl across the sky.",
			"not too late",
			"Black-Tailed Prairie Dog",
			"Gus Kenderdine"
		);

		dbEntries[9] = new DB_Entry(
			"Cove Fields, Quebec",
			"https://www.gallery.ca/collection/artwork/cove-fields-quebec",
			"The beams of light through the clouds pursue the curve of the cove, in a painting that evokes the thoughts of chilly coastal Autumn days and the smell of wet grass and ocean spray.",
			"pursue",
			"Caribou (Atlantic-Gaspésie population)",
			"Edmund Morris"
		);

		dbEntries[10] = new DB_Entry(
			"Fishing Stages, Newfoundland",
			"https://www.gallery.ca/collection/artwork/fishing-stages-newfoundland",
			"It's hard to believe, but people lived and worked in these shacks. Under them, the sea drifts, full of fish and other bounties.",
			"Sea, Drifts",
			"Leatherback sea turtle (Atlantic population)",
			"Maurice Cullen"
		);

		dbEntries[11] = new DB_Entry(
			"The Jack Pine",
			"https://www.gallery.ca/collection/artwork/the-jack-pine",
			"Here, a beautiful pine grows from the ashes of the ground, superimposed on a colourful sunrise. The painter has said that this scene is meant to represent the soul of Northern Ontario.",
			"the ashes",
			"Eastern hog-nosed snake",
			"Tom Thomson"
		);

		SetKeyIds();
	}
}

