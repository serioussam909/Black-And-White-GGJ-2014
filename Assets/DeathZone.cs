using UnityEngine;
using System.Collections;

public class DeathZone : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnCollisionEnter(Collision collision) {
		if(collision.collider.gameObject.layer==LayerMask.NameToLayer("Player"))
		{
			if(collision.collider.gameObject.GetComponent<Player>())
			{
				collision.collider.gameObject.GetComponent<Player>().Die();
			}

			
		}
	}
}
