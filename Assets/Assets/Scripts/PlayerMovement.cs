using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonGroup
{
	public List<GameObject> MoveButtons;
	public List<GameObject> TextMeshes;
}

public class PlayerMovement : MonoBehaviour
{
	public byte PrevNodeId = 0;
	public byte CurrNodeId = 0;

	public GameObject MoveButtonPrefab;

	public MapLoader _map;
	private ViewManager _viewManager;

	private List<ButtonGroup> _buttonGroups;

    void Start()
    {
		_viewManager = GetComponentInParent<ViewManager>();
    }

    void Update()
    {
    }

	public void Init()
	{
		// Initialize button groups list
		_buttonGroups = new List<ButtonGroup>();
		
		// Initialize button group GameObject lists 
		for(byte i = 0; i < _map.NodeCount; i++)
		{
			_buttonGroups.Add(new ButtonGroup());
			_buttonGroups[i].MoveButtons = new List<GameObject>();	
			_buttonGroups[i].TextMeshes = new List<GameObject>();
		}

		// Instantiate button objects
		for(byte i = 0; i < _map.NodeCount; i++)
		{
			// Get node data reference
			NodeData _node = _map.nodes[i];
				
			for(byte j = 0; j < _node.EdgeCount; j++)
			{
				// Get edge data reference
				EdgeData _edge = _map.edges[_node.Edges[j]];

				// Check if position should be "edge_point_a" or "edge_point_b" 
				byte posId = (byte)((_edge.Nodes[0] == _node.Id) ? 1 : 0);
				Vector2 point = _edge.Points[posId];

				// Instantiate button object and add to list
				_buttonGroups[i].MoveButtons.Add(
						Instantiate(MoveButtonPrefab, point, transform.rotation, transform)
				);
				
				int newId = _buttonGroups[i].MoveButtons.Count-1;

				// Add text label
				TextMeshProUGUI tm = _buttonGroups[i].MoveButtons[newId].GetComponentInChildren<TextMeshProUGUI>();
				tm.text = "test label";
				tm.color = Color.black;

				// Add click event
				Button btn = _buttonGroups[i].MoveButtons[newId].GetComponent<Button>();
				btn.onClick.AddListener(() => {
						PrevNodeId = CurrNodeId;
						CurrNodeId = _edge.Nodes[posId];
						OnRoomEnter(CurrNodeId);
				});
			}
		}
		
		// Set all buttons to disabled on start 
		for(byte i = 0; i < _buttonGroups.Count; i++)
		{
			ButtonGroup btnGroup = _buttonGroups[i];
			for(byte j = 0; j < btnGroup.MoveButtons.Count; j++)
			{
				btnGroup.MoveButtons[j].GetComponent<Button>().enabled = false;	
				btnGroup.MoveButtons[j].SetActive(false);
			}
		}

		// Enter starting room
		OnRoomEnter(CurrNodeId);
	}

	public void OnRoomEnter(byte id)
	{
		// Disable previous
		for(byte i = 0; i < _buttonGroups[PrevNodeId].MoveButtons.Count; i++)	
		{
			_buttonGroups[PrevNodeId].MoveButtons[i].SetActive(false);
			_buttonGroups[PrevNodeId].MoveButtons[i].GetComponent<Button>().enabled = false;
		}

		// Enable current
		for(byte i = 0; i < _buttonGroups[CurrNodeId].MoveButtons.Count; i++)	
		{
			_buttonGroups[CurrNodeId].MoveButtons[i].SetActive(true);
			_buttonGroups[CurrNodeId].MoveButtons[i].GetComponent<Button>().enabled = true;
		}
		
		_viewManager.SetViewId(id);
	}
}

