using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveButtonData
{
	public Vector2 position;
	public Vector2 size;

	public GameObject button;
}

public class RoomData
{
	public NodeData nodeData;
	
	public List<GameObject> moveButtons;
}

public class Movement : MonoBehaviour
{
	private byte _currNodeId = 0;
	private byte _prevNodeId = 0;

	private NodeData[] _nodes;
	
	public MapLoader _mapLoader;
	private ViewManager _viewManager;

	public GameObject movementButtonPrefab;
	public List<GameObject> movementButtons;

	public List<RoomData> rooms = new List<RoomData>();

    void Start()
    {
		_viewManager = GameObject.Find("Background/Fade").GetComponent<ViewManager>();
		_viewManager.SetViewId(0);
    }

    void Update()
    {
    }

	public void Init()
	{
		Debug.Log("movement init()");
		SpawnMovementButtons();
	}

	void SpawnMovementButtons()
	{
		for(byte i = 0; i < _mapLoader.NodeCount; i++)				
		{
			RoomData rd = new RoomData();
			rd.nodeData = _mapLoader.nodes[i];
			rd.moveButtons = new List<GameObject>();
			rooms.Add(rd);

			for(byte j = 0; j < _mapLoader.nodes[i].EdgeCount; j++)
			{
				NodeData node = _mapLoader.nodes[i];
				EdgeData edge = _mapLoader.edges[node.Edges[j]];

				byte id = node.Edges[j];
				MakeMoveButton(_mapLoader.nodes[i], _mapLoader.edges[id]);
			}
		}

		OnRoomEnter(rooms[0]);
	}

	void MakeMoveButton(NodeData node, EdgeData edge)
	{
		Vector2 position = Vector2.zero;	

		for(byte i = 0; i < 2; i++) {
			if(_mapLoader.edges[node.Edges[i]].Nodes[i] == node.Id)
			{
				GameObject buttonObj = Instantiate(movementButtonPrefab, transform);
				buttonObj.transform.position = edge.Points[i];
				buttonObj.name = "button" + node.Id;
				
				var rect = buttonObj.GetComponent<RectTransform>();
				rect.anchoredPosition = edge.Points[i];

				buttonObj.SetActive(false);

				rooms[node.Id].moveButtons.Add(buttonObj);
			}
		}
	}

	void OnRoomEnter(RoomData rd)
	{
		for(byte i = 0; i < rooms[_prevNodeId].moveButtons.Count; i++)
		{
			rooms[_prevNodeId].moveButtons[i].SetActive(false);
		}

		for(byte i = 0; i < rd.moveButtons.Count; i++)
		{
			rooms[_currNodeId].moveButtons[i].SetActive(true);
		}

		//_viewManager.SetViewId(rd.nodeData.Background);
		//_viewManager.SetViewId(0);

		//Debug.Log($"{rd.nodeData.Id}");
		Debug.Log(rd.nodeData.Label);
	}

	void OnDirectionClick(byte id)
	{
		OnRoomEnter(rooms[id]);
	}
}

