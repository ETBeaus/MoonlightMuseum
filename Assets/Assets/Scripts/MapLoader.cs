using UnityEngine;
using System;
using System.IO;

public struct NodeData
{
	// Node index
	public byte id;

	// Background texture index
	public byte background;

	// Number of connected edges
	public byte EdgeCount;

	// Edge reference indices
	public byte[] edges;

	// Node name
	public string label;
}

public struct EdgeData
{
	// Edge index
	public byte id;	

	// Connected node references
	public byte[] nodes; 

	// UI Button positions, relative to room size(800x600)
	public Vector2[] points;
}

public class MapLoader : MonoBehaviour
{
	public byte NodeCount;
	public byte EdgeCount;

	public NodeData[] nodes = new NodeData[byte.MaxValue];
	public EdgeData[] edges = new EdgeData[byte.MaxValue];

	private delegate void ParseLineFn(string key, string val, ref byte currId);
	private ParseLineFn[] parseFns;

    void Start()
    {
		parseFns = new ParseLineFn[] { ParseLineNodes, ParseLineEdges };
    }

	private void LoadMap(string filePath) 
	{
		// Open file stream
		StreamReader reader = new StreamReader(filePath);
		if(reader == null) 
		{
			Debug.LogError($"ERROR: file: {filePath} does not exist");
			return;
		}

		sbyte sector = -1;
		byte currId = 0;

		// Read map data
		string line;
		while((line = reader.ReadLine()) != null) 
		{
			ParseLine(line, ref sector, ref currId);
			Debug.Log(line);
		}

		// Close file stream
		reader.Dispose();
	}

	private void ParseLine(string line, ref sbyte sector, ref byte currId) 
	{
		if(line[0] == '[') 
		{
			sector++;
			return;
		}

		int eq = line.IndexOf('=');
		if(eq == -1) return;

		string key = line.Substring(0, eq);
		string val = line.Substring(eq, + 1);
		
		parseFns[sector](key, val, ref currId);
	}

	private void ParseLineNodes(string key, string val, ref byte currId)	
	{
		switch(key) 
		{
			case "count": 
				NodeCount = byte.Parse(val);
				break;

			case "id":
				currId = byte.Parse(val);
				break;
			
			case "label":
				nodes[currId].label = val;
				break;

			case "backgound_id":
				nodes[currId].background = byte.Parse(val);
				break;

			case "e":
				nodes[currId].edges[nodes[currId].EdgeCount++] = byte.Parse(val);
				break;
		}
	}

	private void ParseLineEdges(string key, string val, ref byte currId)
	{
		switch(key) 
		{
			case "count": 
				EdgeCount = byte.Parse(val);
				break;

			case "id":
				currId = byte.Parse(val);
				break;

			case "node_id_a":
				edges[currId].nodes[0] = byte.Parse(val);
				break;

			case "node_id_b":
				edges[currId].nodes[1] = byte.Parse(val);
				break;

			case "pos_a":
				edges[currId].points[0] = ParseVec2(val);
				break;
				
			case "pos_b":
				edges[currId].points[1] = ParseVec2(val);
				break;
		}
	}

	private Vector2 ParseVec2(string str)
	{
		int braceId = str.IndexOf('{');
		int commaId = str.IndexOf(',');	

		string strFloatX = str.Substring(braceId, commaId);
		string strFloatY = str.Substring(commaId + 2, str.Length - 1);

		return new Vector2(float.Parse(strFloatX), float.Parse(strFloatY));
	}
}

