using UnityEngine;
using System.Collections;

public enum PowerUpType
{
	HEALTH,
	POWER
}

public class PowerUp : MonoBehaviour {

	public PowerUpType type=PowerUpType.HEALTH;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void FixedUpdate()
	{
		this.transform.Rotate(new Vector3(0,100*Time.deltaTime,0));
	}
}
