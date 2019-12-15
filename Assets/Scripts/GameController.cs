using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
	protected Spawner spawer;
	protected TimeBar timeBar;
	protected Grid grid;
	protected Store store;

	void Awake()
	{
		spawer = GetComponent<Spawner>();
		timeBar = GetComponent<TimeBar>();
		grid = GetComponent<Grid>();
		store = GetComponent<Store>();
	}

	void Start()
	{
		StartGame();
	}

	public void StartGame()
	{
		spawer.StartSpawn();
		timeBar.StartTimer();
	}

	public void StopGame()
	{
		spawer.StopSpawn();
		timeBar.PauseTimer();
		grid.Stop();
		store.Stop();

		Cube[] cubes = FindObjectsOfType<Cube>();

		foreach (Cube cube in cubes)
			cube.Stop();
	}

	public void GameOver()
	{
		StopGame();
	}
}
