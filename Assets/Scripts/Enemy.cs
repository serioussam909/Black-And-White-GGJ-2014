using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
	public float moveSpeed = 2f;		// The speed the enemy moves at.
	public int Lives = 1;					// How many times the enemy can be hit before it dies.
	public AudioClip[] deathClips;		// An array of audioclips that can play when the enemy dies.
	//public GameObject hundredPointsUI;	// A prefab of 100 that appears when the enemy dies.
	public int startDir=1;
	Fragmentum frag;

	private SpriteRenderer ren;			// Reference to the sprite renderer.
	private Transform frontCheck;		// Reference to the position of the gameobject used for checking if something is in front.
	public bool dead = false;			// Whether or not the enemy is dead.
	private Score score;				// Reference to the Score script.
	public bool jump = false;				// Condition for whether the player should jump.
	public float jumpForce = 1000f;
	Transform muzzle;
	// The fastest the player can travel in the x axis.
	private bool grounded = false;	
	private Transform groundCheck;			// A position marking where to check if the player is grounded.
	GameObject player;
	public AudioClip jumpClip;
	public AudioClip hurtClip;

	public AudioClip deathClip;
	AudioSource audio;
	void Awake()
	{
		audio=GetComponent<AudioSource>();
		player=GameObject.FindGameObjectWithTag("Player");
		int rnd=Random.Range(0,10);
		if(rnd>4)
		{
			startDir=-1;
		}
		Vector3 enemyScale = transform.localScale;
		enemyScale.x = startDir;
			transform.localScale = enemyScale;
		// Setting up the references.
		groundCheck = transform.Find("groundCheck");
		frontCheck = transform.Find("frontCheck").transform;
	//	score = GameObject.Find("Score").GetComponent<Score>();
		frag=GetComponent<Fragmentum>();
	}

	void FixedUpdate ()
	{

		if(dead)
		{
			return;
		}

		if((transform.position-player.transform.position).sqrMagnitude>300)
		{
			return;
		}

		 
		// If the jump button is pressed and the player is grounded then the player should jump.
		if(GameController.Dimension==Dimension.BLACK )
		{
			grounded = Physics.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground")); 

			if(grounded)
			{

			// Play a random jump audio clip.
			//	int i = Random.Range(0, jumpClips.Length);
			//	AudioSource.PlayClipAtPoint(jumpClips[i], transform.position);
			
			// Add a vertical force to the player.
				rigidbody.AddForce(new Vector2(0f, jumpForce));
				audio.PlayOneShot(jumpClip);
			}
			

		}
		// Set the enemy's velocity to moveSpeed in the x direction.
		rigidbody.velocity = new Vector3(transform.localScale.x *moveSpeed , rigidbody.velocity.y,0);	


			// ... set the sprite renderer's sprite to be the damagedEnemy sprite.
		
			
		// If the enemy has zero or fewer hit points and isn't dead yet...
		if(Lives <= 0 && !dead)
			// ... call the death function.
			Die ();
	}
	float blinkTimer=0;
	float timeToBlink=1f;
	public void Hurt()
	{
		if(blinking)
		{
			return;
		}
		audio.PlayOneShot(hurtClip);
		// Reduce the number of hit points by one.
		Lives--;
		blinking=true;
		if(Lives<=0)
		{
			Die ();
		}
	}
	float deathTimer=0;
	public void Update()
	{
		if(blinking&&blinkTimer<timeToBlink)
		{
			blinkTimer+=Time.deltaTime;
			frag.sphereObjectRadius=Mathf.Lerp(0.4f,0.9f,(Mathf.Cos(Time.timeSinceLevelLoad*10)+1)/2);
		}
		if(blinking&&blinkTimer>=timeToBlink)
		{
			blinkTimer=0;
			blinking=false;
			frag.sphereObjectRadius=0.4f;
		}
		if(dead&&deathTimer<1f)
		{
			deathTimer+=Time.deltaTime;
		
				frag.sphereObjectRadius=Mathf.Lerp(0.4f,0.9f,(deathTimer/1));
			return;

		}
		if(deathTimer>=1f)
		{
			Destroy(this.gameObject);
		}
		if(!dead)
		{
			
			if((transform.position-player.transform.position).sqrMagnitude>300)
			{
				return;
			}
			// Create an array of all the colliders in front of the enemy.
			Collider[] frontHits = Physics.OverlapSphere(frontCheck.position, .25f);
			
			// Check each of the colliders.
			foreach(Collider c in frontHits)
			{
				// If any of the colliders is an Obstacle...
				if(c.gameObject.layer==LayerMask.NameToLayer("Ground")
				   ||c.gameObject.layer==LayerMask.NameToLayer("Obstacles")
				   ||c.gameObject.layer==LayerMask.NameToLayer("Enemies")
				   )
				{
					// ... Flip the enemy and stop checking the other colliders.
					Flip ();
					break;
				}
			}
		}

	}

	void OnCollisionEnter(Collision collision) {
		if(collision.collider.gameObject.layer==LayerMask.NameToLayer("Player")&&!dead)
		{
			if(collision.collider.gameObject.GetComponent<Player>())
			{
				collision.collider.gameObject.GetComponent<Player>().Hurt();
			}
		}
		if(collision.collider.gameObject.layer==LayerMask.NameToLayer("Bullets")&&!dead)
		{
			Hurt ();
			Destroy (collision.collider.gameObject);
		}
	}

	bool blinking=false;
		
	public void Die()
	{
		if(dead)
		{
			return;
		}
		audio.PlayOneShot(deathClip);
		
		// Find all of the sprite renderers on this object and it's children.
	
		rigidbody.isKinematic=true;

		// Re-enable the main sprite renderer and set it's sprite to the deadEnemy sprite.
		//ren.enabled = true;

		// Increase the score by 100 points
	//	score.score += 100;

		// Set dead to true.
		dead = true;

		// Allow the enemy to rotate and spin it by adding a torque.
	
		Vector3 enemyScale = transform.localScale;
		enemyScale.x = 1;
		transform.localScale = enemyScale;

		// Find all of the colliders on the gameobject and set them all to be triggers.
		Collider[] cols = GetComponents<Collider>();
		foreach(Collider c in cols)
		{
			c.isTrigger = true;
		}

		// Play a random audioclip from the deathClips array.
		//int i = Random.Range(0, deathClips.Length);
		//AudioSource.PlayClipAtPoint(deathClips[i], transform.position);

		// Create a vector that is just above the enemy.
		Vector3 scorePos;
		scorePos = transform.position;
		scorePos.y += 1.5f;
		frag.sphereObjectRadius=0.2f;

		// Instantiate the 100 points prefab at this point.
		//Instantiate(hundredPointsUI, scorePos, Quaternion.identity);
	}


	public void Flip()
	{
		// Multiply the x component of localScale by -1.
		Vector3 enemyScale = transform.localScale;
		enemyScale.x *= -1;
		transform.localScale = enemyScale;
	}
}
