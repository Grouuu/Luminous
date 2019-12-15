using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Store : MonoBehaviour
{
	public Cube cubePrefabs;
	public Vector3 firstPosition;
	public float margin;
	public float speed;
	public float magnitudeMagnet;
	public int maxBuffer;

	[Header("Debug")]

	public int startCubes;
	public bool infiniteCubes;

	protected List<Cube> cubes = new List<Cube>();
	protected int selected = 0;

	protected bool paused = false;

	void Start()
	{
		// Debug
		for (int i = 0; i < startCubes; i++)
			AddCube();
		//
	}

	void Update()
	{
		if (paused)
			return;

		for (int index = 0; index < cubes.Count; index++)
		{
			Cube cube = cubes[index];
			Vector3 target = GetIndexPosition(index, cube);
			Vector3 direction = target - cube.transform.position;

			if (direction.magnitude > magnitudeMagnet)
				cube.transform.position += direction.normalized * speed * Time.deltaTime;
			else
				cube.transform.position = target;
		}
	}

	public void Stop()
	{
		paused = true;
	}

	public Vector3 AddCube()
	{
		if (cubes.Count >= maxBuffer)
			return Vector3.positiveInfinity;

		int index = maxBuffer + 1;
		Cube cube = Instantiate<Cube>(cubePrefabs);
		cube.transform.position = GetIndexPosition(index, cube);
		cubes.Add(cube);
		return GetIndexPosition(index, cube);
	}

	public Vector3 RemoveCube()
	{
		if (cubes.Count < 1)
			return Vector3.positiveInfinity;

		int index = 0;
		Cube cube = cubes[0];
		cubes.Remove(cube);
		Vector3 position = GetIndexPosition(index, cube);
		Destroy(cube.gameObject);

		if (selected > 0)
			selected--;

		// Debug
		if (infiniteCubes)
			AddCube();
		//

		return position;
	}

	public void RemoveCubes(int nb)
	{
		for (int i = 0; i < nb; i++)
			RemoveCube();

		while (UnselectCube()) {}
	}

	public bool SelectCube()
	{
		if (cubes.Count > selected)
		{
			selected++;
			cubes[selected - 1].SetAvailable(false);
			return true;
		}

		return false;
	}

	public bool UnselectCube()
	{
		if (selected > 0)
		{
			cubes[selected - 1].SetAvailable(true);
			selected--;
			return true;
		}

		return false;
	}

	protected Vector3 GetIndexPosition(int index, Cube templateCube)
	{
		return new Vector3(firstPosition.x - (templateCube.GetComponent<MeshRenderer>().bounds.size.x + margin) * index, firstPosition.y, 0);
	}
}
