using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Grid : MonoBehaviour
{
	public Vector3 firstPosition;
	public float margin;
	public int columns;
	public int rows;
	public float speed;
	public float magnitudeMagnet;

	protected Cube[] grid;
	protected List<List<Cube>> groups = new List<List<Cube>>();

	void Awake()
	{
		grid = new Cube[columns * rows];
	}

	void Update()
	{
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
	}

	protected void UpdateCubePosition(Cube cube)
	{
		// TODO
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
				foreach (Cube cube in group)
				{
					Vector2Int position = GetCubePosition(cube);
					// BUG : return -1/-1 sometimes
					Debug.Log("FROM " + position.x + "/" + position.y);
					SetCellCube(position.x, position.y, null);
					position.x -= minEmptyColumns;
					Debug.Log("TO " + position.x + "/" + position.y);
					SetCellCube(position.x, position.y, cube);
				}
			}
		}
	}

	public void AddGroup(Vector2Int[] positions, Stack<Cube> cubes)
	{
		List<Cube> group = new List<Cube>();

		foreach (Vector2Int position in positions)
		{
			Cube cube = cubes.Pop();
			if (AddCube(cube, position.x, position.y))
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
	}

	protected bool AddCube(Cube cube, int column, int row)
	{
		// TODO prevents to add cube at the left of an already added cube

		// BUG

		if (!GetCellCube(column, row))
		{
			SetCellCube(column, row, cube);
			cube.transform.position = GetCellPosition(cube, column, row);
			return true;
		}
		else
		{
			Debug.LogWarning("DESTROY");
			Destroy(cube.gameObject);
		}
		return false;
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
			firstPosition.x + (templateCube.GetComponent<BoxCollider>().bounds.size.x + margin) * column,
			firstPosition.y - (templateCube.GetComponent<BoxCollider>().bounds.size.y + margin) * row,
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
