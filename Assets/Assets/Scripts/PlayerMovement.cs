using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
	public byte CurrNodeId = 0;

	private MapLoader _map;

    void Start()
    {
		_map = GameObject.Find("HandlerObject").GetComponent<MapLoader>();
    }

    void Update()
    {
        
    }

	void OnGUI()
	{
		//Vector2 scale = new Vector2(Screen.width / 440, Screen.height / 330);
		//Debug.Log($"{Screen.width}, {Screen.height}");
		//GUI.BeginGroup(new Rect(0, 0, 440, 330));

		NodeData node = _map.nodes[CurrNodeId];
		for(byte i = 0; i < node.EdgeCount; i++)
		{
			EdgeData edge = _map.edges[node.Edges[i]];

			byte posId = (edge.Nodes[0] == node.Id) ? (byte)(1) : (byte)(0);
			Vector2 point = edge.Points[posId];

			Rect rect = new Rect(point.x, point.y, 100, 100);
			string label = _map.nodes[edge.Nodes[posId]].Label;

			if(GUI.Button(rect, label))
			{
				CurrNodeId = edge.Nodes[posId];
			}
		}

		//GUI.EndGroup();
	}

	public void Init()
	{
	}
}
