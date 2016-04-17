using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	Dimension dimension=Dimension.WHITE;
	public static Dimension Dimension=Dimension.WHITE;
	// Use this for initialization

	private GameObject[] grounds;
	private GameObject[] terrains;
	private GameObject player;
	private DimensionVisibility[] flippable;
	public Color groundLight;
	public Color groundDark;
	public Color playerLight;
	public Color playerDark;
	public Color bgLight;
	public Color bgDark;
	public Color fogLight;
	public Color fogDark;
	public Color enemyLight;
	public Color enemyDark;
	private GameObject[] waters;
	private GameObject[] lavas;
	private GameObject[] enemies;
	private GameObject gameOver;
	void Start () {
		grounds=GameObject.FindGameObjectsWithTag("Ground");
		player=GameObject.FindGameObjectWithTag("Player");
		gameOver=GameObject.FindGameObjectWithTag("GameOver");
		flippable=FindObjectsOfType<DimensionVisibility>();
		terrains=GameObject.FindGameObjectsWithTag("bgterrain");
		waters=GameObject.FindGameObjectsWithTag("Water");
		lavas=GameObject.FindGameObjectsWithTag("Lava");
		enemies=GameObject.FindGameObjectsWithTag("Enemy");

		isFlipping=true;
		FlipDimensions();
	}
	private bool isFlipping=false;

	public void FlipDimensions()
	{
		if(!isFlipping)
		{
			return;
		}
		if(dimension==Dimension.WHITE)
		{
			RenderSettings.fogColor=fogLight;
			Camera.main.backgroundColor=bgLight;
			player.GetComponent<Renderer>().sharedMaterial.color=playerLight;
			foreach(GameObject en in enemies)
			{
				if(en!=null)
				{
					en.GetComponent<Renderer>().sharedMaterial.color=enemyLight;
				}
			}

			foreach(GameObject gnd in grounds)
			{
				gnd.GetComponent<Renderer>().material.color=groundLight;
			}
			foreach(GameObject water in waters)
			{
				water.SetActive(true);
			}
			foreach(GameObject lava in lavas)
			{
				lava.SetActive(false);
			}
			foreach(GameObject trn in terrains)
			{
				trn.GetComponent<Terrain>().materialTemplate.color=groundLight;
			}
			foreach(DimensionVisibility dv in flippable)
			{
				if(dv.dimension==Dimension.WHITE)
				{
					dv.gameObject.SetActive(true);
				}
				if(dv.dimension==Dimension.BLACK)
				{
					dv.gameObject.SetActive(false);
				}
			}
		}
		if(dimension==Dimension.BLACK)
		{
			RenderSettings.fogColor=fogDark;
			Camera.main.backgroundColor=bgDark;
			player.GetComponent<Renderer>().sharedMaterial.color=playerDark;
			foreach(GameObject en in enemies)
			{
				if(en!=null)
				{
					en.GetComponent<Renderer>().sharedMaterial.color=enemyDark;
				}
			}
			foreach(GameObject gnd in grounds)
			{
				gnd.GetComponent<Renderer>().material.color=groundDark;
			}
			foreach(GameObject water in waters)
			{
				water.SetActive(false);
			}
			foreach(GameObject lava in lavas)
			{
				lava.SetActive(true);
			}
			foreach(GameObject trn in terrains)
			{
				trn.GetComponent<Terrain>().materialTemplate.color=groundDark;
			}
			foreach(DimensionVisibility dv in flippable)
			{
				if(dv.dimension==Dimension.BLACK)
				{
					dv.gameObject.SetActive(true);
				}
				if(dv.dimension==Dimension.WHITE)
				{
					dv.gameObject.SetActive(false);
				}
			}
		}
		isFlipping=false;
	}
	
	// Update is called once per frame
	void Update () {
		if(!player.GetComponent<Player>().Alive)
		{
			gameOver.SetActive(true);
			if(Input.anyKeyDown)
			{
				Application.LoadLevel(Application.loadedLevel);
			}
			return;
		}
		gameOver.SetActive(false);
		if(!isFlipping)
		{
			if(Input.GetButtonDown("FlipDimension"))
			{
				isFlipping=true;
				if(dimension==Dimension.WHITE)
				{
					dimension=Dimension.BLACK;
				}
				else if(dimension==Dimension.BLACK)
				{
					dimension=Dimension.WHITE;
				}

			}
		}
		GameController.Dimension=dimension;
		FlipDimensions();
	}
}
