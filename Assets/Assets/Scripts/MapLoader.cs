using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class NodeData
{
	// Node index
	public byte Id;

	// Background texture index
	public sbyte Background;

	// Number of connected edges
	public byte EdgeCount;

	// Edge reference indices
	public byte[] Edges;

	// Node name
	public string Label;
}

[Serializable]
public class EdgeData
{
	// Edge index
	public byte Id;	

	// Connected node references
	public byte[] Nodes; 

	// UI Button positions, relative to room size(800x600)
	public Vector2[] Points;
}

public class MapLoader : MonoBehaviour
{
	public byte NodeCount;
	public byte EdgeCount;

	public NodeData[] nodes; 
	public EdgeData[] edges; 

	private delegate void _parseLineFn(string key, string val, ref byte currId);
	private _parseLineFn[] _parseFns;

	private ViewManager _viewManager;
	
	private PlayerMovement _movement;
	private PlayerMovement _pMovement;

    void Start()
    {
		nodes = new NodeData[byte.MaxValue];
		edges = new EdgeData[byte.MaxValue];

		for(byte i = 0; i < byte.MaxValue; i++) 
		{
			// Allocate edge reference array on nodes
			nodes[i] = new NodeData(); 
			nodes[i].Edges = new byte[8];

			// Allocate node references and point arrays on edges
			edges[i] = new EdgeData();
			edges[i].Nodes = new byte[2];
			edges[i].Points = new Vector2[2];
		}

		// Set function pointers/delegates
		_parseFns = new _parseLineFn[] { ParseLineNodes, ParseLineEdges };
		
		_viewManager = GameObject.Find("Background/Fade").GetComponent<ViewManager>();
		_viewManager._mapLoader = this;

		_pMovement = GameObject.Find("PlayerObject").GetComponent<PlayerMovement>();
		_pMovement._map = this;

		LoadMap("test.txt");
		_pMovement.Init();
    }

	private void LoadMap(string filePath) 
	{
		// Open file stream
		StreamReader reader = new StreamReader($"Assets/StreamingAssets/{filePath}");
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
			//Debug.Log(line);
		}

		// Close file stream
		reader.Dispose();

	    //for(byte i = 0; i < NodeCount; i++)
	    //	Debug.Log($"node[{i}] bg: {nodes[i].Background}");

		PrintNodeData();

		// Set start view
		_viewManager.SetViewId(nodes[0].Background);
	}

	private void ParseLine(string line, ref sbyte sector, ref byte currId) 
	{
		if(line.Length == 0 || line[0] == '#') return;

		if(line[0] == '[') 
		{
			sector++;
			return;
		}

		int eq = -1;
		eq = line.IndexOf('=');
		if(eq == -1) return;

		string key = line.Substring(0, eq).Trim();
		string val = line.Substring(eq + 1).Trim();
		
		_parseFns[sector](key, val, ref currId);
	}

	private void ParseLineNodes(string key, string val, ref byte currId)	
	{
		Debug.Log($"key: {key}");
		Debug.Log($"val: {val}");
		
		switch(key) 
		{
			case "count": 
				NodeCount = byte.Parse(val);
				break;

			case "id":
				currId = byte.Parse(val);
				nodes[currId].Id = currId;
				Debug.Log($"currId: {currId}");
				break;
			
			case "label":
				nodes[currId].Label = val;
				break;

			case "background_id":
				nodes[currId].Background = sbyte.Parse(val);
				break;

			case "e":
				nodes[currId].Edges[nodes[currId].EdgeCount++] = byte.Parse(val);
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
				edges[currId].Nodes[0] = byte.Parse(val);
				break;

			case "node_id_b":
				edges[currId].Nodes[1] = byte.Parse(val);
				break;

			case "pos_a":
				edges[currId].Points[0] = ParseVec2(val);
				break;
				
			case "pos_b":
				edges[currId].Points[1] = ParseVec2(val);
				break;
		}
	}

	private Vector2 ParseVec2(string str)
	{

		int braceOpenId = str.IndexOf('{');
		int BraceCloseId = str.IndexOf('}');
		int commaId = str.IndexOf(',');	

		string strFloatX = str[(braceOpenId+1)..commaId];
		string strFloatY = str[(commaId+2)..BraceCloseId];
		
	    Vector2 scale = new Vector2(800/440, 600/330);
		//Vector2 scale = Vector2.one;
		Vector2 raw = new Vector2(float.Parse(strFloatX), float.Parse(strFloatY));

		return new Vector2(raw.x * scale.x, raw.y * scale.y);
	}

	private void PrintNodeData()
	{
		Debug.Log("----- NODE DATA -----");

		for(byte i = 0; i < NodeCount; i++)
		{
			Debug.Log($"raw index: {i}");
			Debug.Log($"id: {nodes[i].Id}");
			Debug.Log($"label: {nodes[i].Label}");
			Debug.Log($"bg: {nodes[i].Background}");
		}

		Debug.Log("---------------------");
	}
}

