using UnityEngine;

public class Journal : MonoBehaviour
{
	public enum FLAGS : byte {
		f_show = 0x01,	// Show journal
		f_lock = 0x02	// Lock journal
	}

	public byte flags = (0);

	private Canvas _canvas; 
	
    void Start()
    {
       	_canvas = GetComponent<Canvas>(); 
    }

    void Update()
    {
    	if((flags & (byte)FLAGS.f_lock) == 0) 
		{
			if(Input.GetKey(KeyCode.J))
			{
				flags ^= (byte)FLAGS.f_show;
			}
		} 
    }
}
