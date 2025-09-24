using System;
using System.IO;
using UnityEngine;

[Serializable]
public class DB_Entry
{
	// Start and end indices of keyword in description string
	// Used for word inspection
    public int keyStart;
    public int keyEnd;

    public string title;
    public string link;
    public string description;
    public string keyWord;
    public string dataPoint;
    public string painterName;
}

public class PaintingData : MonoBehaviour
{
    const byte _entryCount = 12;
    public DB_Entry[] dbEntries = new DB_Entry[_entryCount];

    void Start()
    {
        DB_Read("paintings_db.txt");
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
                dbEntries[++currId].title = title;
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
            case "link":			entry.link = val;				break;
            case "desc":			entry.description = val;		break;
            case "key_word":		entry.keyWord = val;			break;
            case "data_point": 		entry.dataPoint = val;			break;
            case "painter_name": 	entry.painterName = val;		break;
        }
    }

    private void SetKeyIds()
    {
		// Iterate through all entries
        for(byte i = 0; i < _entryCount; i++)
        {
            DB_Entry entry = dbEntries[i];

			// Iterate through description string
			for(int j = 0; j < entry.description.Length; j++) {
				// Early out if remaining chars too small
				if(j + entry.keyWord.Length > entry.description.Length) break;	

				// Get string portion 
				string str = entry.description[j..(j+entry.keyWord.Length)].ToLower();

				// If portion + keyword are equal, set key indices
				if(str.Equals(entry.keyWord.ToLower()))
				{
					entry.keyStart = j;
					entry.keyEnd = j + entry.keyWord.Length;
				}
			}
        }
    }
}
