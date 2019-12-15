using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Grid : MonoBehaviour
{
	public Cube cubePrefab;
	public GameObject groupPrefab;
	public Vector3 firstPosition;
	public float margin;
	public int columns;
	public int rows;
	public float speed;
	public float magnitudeMagnet;

	[Header("Debug")]

	public bool debug;

	protected GameController controller;

	protected Cube[] grid;
	protected List<List<Cube>> groups = new List<List<Cube>>();

	protected bool paused = false;

	void Awake()
	{
		controller = FindObjectOfType<GameController>();

		grid = new Cube[columns * rows];
	}

	void Update()
	{
		if (paused)
			return;

		foreach (Cube cube in grid)
		{
			if (!cube)
				continue;

			Vector2Int gridPosition = GetCubePosition(cube);
			Vector3 targetPosition = GetCellPosition(cube, gridPosition.x, gridPosition.y);

			if (cube.transform.position == targetPosition)
				continue;

			Vector3 direction = targetPosition - cube.transform.position;

			if (direction.magnitude > magnitudeMagnet)
				cube.transform.position += direction.normalized * speed * Time.deltaTime;
			else
				cube.transform.position = targetPosition;
		}

		// DEBUG
		if (debug && Input.GetKeyDown("space"))
			RemoveLastLine();
		//
	}

	public void Stop()
	{
		paused = true;
	}

	protected void UpdateGroupsTargetPositions()
	{
		foreach (List<Cube> group in groups)
		{
			int minEmptyColumns = int.MaxValue;

			foreach (Cube cube in group)
			{
				Vector2Int position = GetCubePosition(cube);
				int col = position.x;
				int counter = 0;

				while (--col > -1)
				{
					Cube c = GetCellCube(col, position.y);
					if (!c || group.Contains(c))
						counter++;
					else
						break;
				}

				minEmptyColumns = Mathf.Min(minEmptyColumns, counter);
			}

			if (minEmptyColumns > 0)
			{
				for (int column = 0; column < columns; column++)
					for (int row = 0; row < rows; row++)
					{
						Cube cube = GetCellCube(column, row);
						if (cube && group.Contains(cube))
						{
							Vector2Int position = GetCubePosition(cube);
							SetCellCube(position.x, position.y, null);
							position.x -= minEmptyColumns;
							SetCellCube(position.x, position.y, cube);
						}
					}
			}
		}
	}

	public int AddGroup(Vector2Int[] positions)
	{
		List<Cube> group = new List<Cube>();

		foreach (Vector2Int position in positions)
		{
			bool available = true;

			for (int index = position.x; index < columns; index++)
			{
				Cube c = GetCellCube(index, position.y);
				if (c && !group.Contains(c))
					available = false;
			}

			if (!available)
				continue;

			Cube cube = AddCube(position.x, position.y);
			if (cube)
				group.Add(cube);
		}

		if (group.Count > 0)
		{
			groups.Add(group);

			try
			{
				UpdateGroupsTargetPositions();
			}
			catch (Exception e)
			{
				Debug.LogError(e);
			}
		}

		return group.Count;
	}

	public void RemoveLastLine()
	{
		for (int column = 0; column < columns; column++)
			for (int row = 0; row < rows; row++)
			{
				Cube cube = GetCellCube(column, row);

				// TODO if column == 0 && !c → game over

				if (!cube)
				{
					if (column == 0)
						controller.GameOver();

					continue;
				}

				if (column == 0)
					RemoveCube(cube);
				else
				{
					SetCellCube(column, row, null);
					SetCellCube(column - 1, row, cube);
				}
			}
	}

	protected Cube AddCube(int column, int row)
	{
		Cube cube = Instantiate<Cube>(cubePrefab);
		SetCellCube(column, row, cube);
		cube.transform.position = GetCellPosition(cube, column, row);
		return cube;
	}

	protected void RemoveCube(Cube cube)
	{
		List<Cube> group = GetGroup(cube);
		group.Remove(cube);

		if (group.Count == 0)
			groups.Remove(group);

		Destroy(cube.gameObject);
	}

	protected List<Cube> GetGroup(Cube cube)
	{
		foreach (List<Cube> group in groups)
			foreach (Cube c in group)
				if (c == cube)
					return group;
		return null;
	}

	protected Vector3 GetCellPosition(Cube templateCube, int column, int row)
	{
		return new Vector3(
			firstPosition.x + (templateCube.GetComponent<MeshRenderer>().bounds.size.x + margin) * column,
			firstPosition.y - (templateCube.GetComponent<MeshRenderer>().bounds.size.y + margin) * row,
			0
		);
	}

	protected Vector2Int GetCubePosition(Cube cube)
	{
		for (int column = 0; column < columns; column++)
			for (int row = 0; row < rows; row++)
				if (GetCellCube(column, row) == cube)
					return new Vector2Int(column, row);

		return new Vector2Int(-1, -1);
	}

	protected void SetCellCube(int column, int row, Cube cube)
	{
		grid[column + row * columns] = cube;
	}

	protected Cube GetCellCube(int column, int row)
	{
		return grid[column + row * columns];
	}
}
