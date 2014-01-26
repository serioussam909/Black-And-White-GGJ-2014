using UnityEngine;
using System.Collections;

public class LevelExit : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnTriggerEnter(Collider c)
	{
		if(c.gameObject.layer==LayerMask.NameToLayer("Player"))
		{
			Application.LoadLevel(Application.loadedLevel);
		}
	}
}
