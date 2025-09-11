using UnityEngine;
using System.IO;

public class NodeData
{
	// Node index
	public ushort id;

	// Background texture index
	public ushort background;

	// Number of connected edges
	public ushort edgeCount;

	// Edge reference indices
	public ushort[] edges;

	// Node name
	public string label;
}

public class EdgeData
{
	// Edge index
	public ushort id;	

	// Connected node references
	public ushort[] nodes; 

	// UI Button positions, relative to room size(800x600)
	public Vector2[] points;
}

public class MapLoader : MonoBehaviour
{
    void Start()
    {
        
    }

	private void LoadMap(string filePath) 
	{
		using (StreamReader reader = new StreamReader(filePath))
		{
			string line;
			while((line = reader.ReadLine()) != null)
			{
			}
		}
	}

	private void ParseLine(string line) 
	{
	}
}

