using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
	public Cube cubePrefabs;
	public float baseDelay;
	public float baseRandom;
	public float speed;
	public Rect playgroundBounds;
	public bool debug;

	void Start()
	{
		StartSpawn();
	}

	void Update()
	{
		if (debug)
		{
			Debug.DrawLine(new Vector3(playgroundBounds.x, playgroundBounds.y, 0), new Vector3(playgroundBounds.x + playgroundBounds.width, playgroundBounds.y, 0), Color.red);
			Debug.DrawLine(new Vector3(playgroundBounds.x + playgroundBounds.width, playgroundBounds.y, 0), new Vector3(playgroundBounds.x + playgroundBounds.width, playgroundBounds.y + playgroundBounds.height, 0), Color.red);
			Debug.DrawLine(new Vector3(playgroundBounds.x, playgroundBounds.y, 0), new Vector3(playgroundBounds.x, playgroundBounds.y + playgroundBounds.height), Color.red);
			Debug.DrawLine(new Vector3(playgroundBounds.x, playgroundBounds.y + playgroundBounds.height, 0), new Vector3(playgroundBounds.x + playgroundBounds.width, playgroundBounds.y + playgroundBounds.height, 0), Color.red);
		}
	}

	public void StartSpawn()
	{
		SpawnCube();
		StartCoroutine("SpawnTimer");
	}

	protected IEnumerator SpawnTimer()
	{
		yield return new WaitForSeconds(baseDelay + Random.Range(-baseRandom, baseRandom));
		SpawnCube();
		StartCoroutine("SpawnTimer");
	}

	protected void SpawnCube()
	{
		Cube cube = Instantiate<Cube>(cubePrefabs);
		cube.SetInteractive(true);
		cube.speed = speed;
		cube.transform.position = new Vector3(
			Random.Range(playgroundBounds.x, playgroundBounds.x + playgroundBounds.width),
			playgroundBounds.y - cube.GetComponent<MeshRenderer>().bounds.size.y,
			0
		);
	}
}
