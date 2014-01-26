using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	public float ttl=2.0f;
	float timer=0;

	void Update () {
		if(rigidbody.velocity.magnitude<4)
		{
			Destroy(this.gameObject);
		}
		timer+=Time.deltaTime;
		if(timer>ttl)
		{
			Destroy(this.gameObject);
		}
	}
}
