using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Store : MonoBehaviour
{
	public Vector3 firstPosition;
	public float margin;
	public float speed;
	public float magnitudeMagnet;
	public int maxBuffer;

	protected List<Cube> cubes = new List<Cube>();
	protected int selected = 0;

	void Update()
	{
		for (int position = 0; position < cubes.Count; position++)
		{
			Cube cube = cubes[position];
			Vector3 target = new Vector3(firstPosition.x - (cube.GetComponent<BoxCollider>().bounds.size.x + margin) * position, firstPosition.y, 0);
			Vector3 direction = target - cube.transform.position;

			if (direction.magnitude > magnitudeMagnet)
				cube.transform.position += direction.normalized * speed * Time.deltaTime;
			else
				cube.transform.position = target;
		}
	}

	public bool AddCube(Cube cube)
	{
		if (cubes.Count >= maxBuffer)
			return false;

		// TODO: add cube into the array when the animation ended?

		cubes.Add(cube);
		return true;
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

	public Stack<Cube> RemoveSelectedCubes()
	{
		Stack<Cube> selectedCubes = new Stack<Cube>();

		if (selected > 0)
		{
			for (int index = 0; index < selected; index++)
				selectedCubes.Push(cubes[index]);

			for (int index = 0; index < selected; index++)
				cubes.RemoveAt(0);
		}

		selected = 0;

		return selectedCubes;
	}
}
