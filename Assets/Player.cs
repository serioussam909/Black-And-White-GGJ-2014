using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Player : MonoBehaviour {

	// Use this for initialization
	[HideInInspector]
	public bool facingRight = true;			// For determining which way the player is currently facing.
	[HideInInspector]
	public bool jump = false;				// Condition for whether the player should jump.
	public float jumpForce = 1000f;
	Transform muzzle;
	Transform foot;
	GameObject hudLife;
	public float moveForce = 365f;			// Amount of force added to move the player left and right.
	public float maxSpeed = 5f;				// The fastest the player can travel in the x axis.
	private bool grounded = false;	
	private Transform groundCheck;			// A position marking where to check if the player is grounded.
	public int Lives=1;
	public bool Alive=true;
	public GameObject bullet;
	Fragmentum frag;
	public AudioClip jumpClip;
	public AudioClip stompClip;
	public AudioClip deathClip;
	public AudioClip hurtClip;
	public AudioClip powerUpClip;
	public AudioClip shootClip;
	AudioSource audio;

	public float bulletDelay=0.3f;
	float bulletTimer=0;
	List<GameObject> HudLives=new List<GameObject>();
	void Awake()
	{
		audio=GetComponent<AudioSource>();
		// Setting up references.
		groundCheck = transform.Find("groundCheck");
		muzzle = transform.Find("muzzle");
		foot = transform.Find("foot");
		frag=GetComponent<Fragmentum>();
		hudLife=GameObject.FindGameObjectWithTag("HUDLife");
		HudLives.Add(hudLife);
		GameObject hudLifeInst=hudLife;
		for(int i=0;i<Lives-1;i++)
		{
			hudLifeInst=(GameObject)GameObject.Instantiate(hudLife,new Vector3( hudLifeInst.transform.position.x+1.5f,hudLifeInst.transform.position.y,hudLifeInst.transform.position.z),hudLifeInst.transform.rotation);
			HudLives.Add(hudLifeInst);
		}
	}

	void Start () {
		
	}
	bool shoot=false;
	float deathTimer=0;

	float blinkTimer=0;
	float timeToBlink=1f;
	public void Hurt()
	{
		if(blinking)
		{
			return;
		}
		audio.PlayOneShot(hurtClip);
		blinking=true;
		HudLives[Lives-1].renderer.enabled=false;
			Lives--;
			if(Lives<=0)
			{
				Die();
			}
			

	}
	bool blinking=false;
	void Update()
	{
		if(!Alive)
		{
			muzzle.renderer.enabled=false;
			foot.renderer.enabled=false;
			if(deathTimer<4f)
			{
				deathTimer+=Time.deltaTime;
				
				frag.sphereObjectRadius=Mathf.Lerp(0.4f,2.8f,(deathTimer/4));
				
			}
			if(deathTimer>=1f)
			{

			}
			return;
		}

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

		if(GameController.Dimension==Dimension.BLACK)
		{
			muzzle.renderer.enabled=true;
			foot.renderer.enabled=false;
		}
		if(GameController.Dimension==Dimension.WHITE)
		{
			muzzle.renderer.enabled=false;
			foot.renderer.enabled=true;
		}
		// The player is grounded if a linecast to the groundcheck position hits anything on the ground layer.
		grounded = Physics.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));  
		// If the jump button is pressed and the player is grounded then the player should jump.
		if(GameController.Dimension==Dimension.WHITE)
		{
			RaycastHit hitinfo;
			bool hit = Physics.Linecast(transform.position, groundCheck.position, out hitinfo, 1 << LayerMask.NameToLayer("Enemies")); 
			if(hit)
			{
				GameObject enemy=hitinfo.collider.gameObject;
				if(enemy.GetComponent<Enemy>())
				{
					enemy.GetComponent<Enemy>().Die();
					audio.PlayOneShot(stompClip);
				}
			}
		}

		if(Input.GetButtonDown("Jump") && grounded)
			jump = true;
		if(Input.GetButtonDown("Fire1"))
		{
			shoot=true;
		}
		if(bulletTimer>bulletDelay&&shoot)
		{
			if(GameController.Dimension==Dimension.BLACK)
			{

				GameObject currBullet=(GameObject)GameObject.Instantiate(bullet,muzzle.position,Quaternion.identity);
				float xForce=1;
				if(!facingRight)
				{
					xForce=-1;
				}
				currBullet.rigidbody.AddForce(new Vector3(xForce*200f,-200f,0));
				bulletTimer=0;
				audio.PlayOneShot(shootClip);
				shoot=false;
			}
		}
		bulletTimer+=Time.deltaTime;


		if(Lives<=0)
		{
			Die();
		}
	}

	void FixedUpdate ()
	{
		if(!Alive)
		{
			return;
		}
		// Cache the horizontal input.
		float h = Input.GetAxis("Horizontal");
		

		// If the player is changing direction (h has a different sign to velocity.x) or hasn't reached maxSpeed yet...
		if(h * rigidbody.velocity.x < maxSpeed)
			// ... add a force to the player.
			rigidbody.AddForce(Vector3.right * h * moveForce);
		
		// If the player's horizontal velocity is greater than the maxSpeed...
		if(Mathf.Abs(rigidbody.velocity.x) > maxSpeed)
			// ... set the player's velocity to the maxSpeed in the x axis.
			rigidbody.velocity = new Vector3(Mathf.Sign(rigidbody.velocity.x) * maxSpeed, rigidbody.velocity.y,0);
		
		// If the input is moving the player right and the player is facing left...
		if(h > 0 && !facingRight)
			// ... flip the player.
			Flip();
		// Otherwise if the input is moving the player left and the player is facing right...
		else if(h < 0 && facingRight)
			// ... flip the player.
			Flip();
		
		// If the player should jump...
		if(jump)
		{
			// Set the Jump animator trigger parameter.
		//	anim.SetTrigger("Jump");
			
			// Play a random jump audio clip.
		//	int i = Random.Range(0, jumpClips.Length);
		//	AudioSource.PlayClipAtPoint(jumpClips[i], transform.position);
			
			// Add a vertical force to the player.
			rigidbody.AddForce(new Vector2(0f, jumpForce));
			
			// Make sure the player can't jump again until the jump conditions from Update are satisfied.
			jump = false;
			audio.PlayOneShot(jumpClip);
		}
	}

	public void Die()
	{
		Alive=false;
		// Switch the way the player is labelled as facing.
		facingRight = true;
		
		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x =1;
		transform.localScale = theScale;
		rigidbody.isKinematic=true;
		collider.isTrigger=true;
		audio.PlayOneShot(deathClip);
	
	}
	void OnCollisionEnter(Collision collision) {
		if(collision.collider.gameObject.layer==LayerMask.NameToLayer("Powerups")&&Alive)
		{
			if(collision.collider.gameObject.GetComponent<PowerUp>())
			{
				if(collision.collider.gameObject.GetComponent<PowerUp>().type==PowerUpType.HEALTH)
				{
					Destroy (collision.collider.gameObject);
					Lives++;
					audio.PlayOneShot(powerUpClip);
					if(HudLives.Count<Lives)
					{   
						GameObject hudLifeInst=HudLives[HudLives.Count-1];
						hudLifeInst=(GameObject)GameObject.Instantiate(hudLife,new Vector3( hudLifeInst.transform.position.x+1.5f,hudLifeInst.transform.position.y,hudLifeInst.transform.position.z),hudLifeInst.transform.rotation);
						HudLives.Add(hudLifeInst);
					}
					else
					{
						HudLives[Lives-1].renderer.enabled=true;
					}
				}
			}
		}
		if(collision.collider.gameObject.layer==LayerMask.NameToLayer("EnemyBullets")&&Alive)
		{
			Hurt ();

			Destroy (collision.collider.gameObject);
		}
	}
	
	void Flip ()
	{
		// Switch the way the player is labelled as facing.
		facingRight = !facingRight;
		
		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}



}
