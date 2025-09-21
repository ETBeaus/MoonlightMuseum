using System;
using System.IO;
using UnityEngine;

[Serializable]
public class DB_Entry 
{
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
		StreamReader reader = new StreamReader($"Assets/StreamingAssets/{filePath}");
		if(reader == null) 
		{
			Debug.LogError($"ERROR: file: {filePath} does not exist");
			return;
		}

		string line;
		sbyte id = -1;
		while((line = reader.ReadLine()) != null)
		{
			DB_ParseLine(line, ref id);
		}
	}

	private void DB_ParseLine(string line, ref sbyte currId)
	{
		if(line.Length == 0) return;

		int eq = -1;
		eq = line.IndexOf('=');
		if(eq == -1)
		{
			if(line[0] == '[')
			{
				int braceCloseId = line.IndexOf(']');
				string title = line.Substring(1, braceCloseId).Trim();

				dbEntries[++currId].title = title;
			}

			return;
		}

		DB_Entry entry = dbEntries[currId];

		string key = line.Substring(0, eq).Trim();
		string val = line.Substring(eq + 1).Trim();

		switch(key)
		{
			case "link":			entry.link = val; 			break;
			case "desc":			entry.description = val; 	break;
			case "key_word":		entry.keyWord = val; 		break;
			case "data_point": 		entry.dataPoint = val; 		break;
			case "painter_name": 	entry.painterName = val; 	break;
		}
	}
}

