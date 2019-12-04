using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
	public Vector3 firstPosition;
	public float margin;
	public int columns;
	public int rows;

	protected Cube[,] grid;

	void Awake()
	{
		grid = new Cube[columns, rows];
	}

	void Update()
	{
		for (int column = 0; column < columns; column++)
		{
			for (int row = 0; row < rows; row++)
			{
				Cube cube = grid[column, row];

				if (cube)
				{
					MoveToEmpty(column, row);

					Vector3 target = GetPosition(cube, column, row);
					Vector3 direction = target - cube.transform.position;

					if (direction.magnitude > 0.4f)
						cube.transform.position += direction.normalized * 20 * Time.deltaTime;
					else
						cube.transform.position = target;
				}
			}
		}
	}

	public void AddCube(Cube cube, int column, int row)
	{
		if (!grid[column, row])
		{
			grid[column, row] = cube;
			cube.transform.position = GetPosition(cube, column, row);
		}
		else
		{
			Destroy(cube.gameObject);
		}
	}

	protected void MoveToEmpty(int column, int row)
	{
		Cube cube = grid[column, row];

		if (!cube)
			return;

		for (int index = 0; index < column; index++)
		{
			if(!grid[index, row])
			{
				grid[column, row] = null;
				grid[index, row] = cube;
				return;
			}
		}
	}

	protected Vector3 GetPosition(Cube cube, int column, int row)
	{
		return new Vector3(
			firstPosition.x + (cube.GetComponent<BoxCollider>().bounds.size.x + margin) * column,
			firstPosition.y - (cube.GetComponent<BoxCollider>().bounds.size.y + margin) * row,
			0
		);
	}
}
