using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
	public Vector3 firstPosition;
	public float margin;
	public int columns;
	public int rows;

	protected Cube[] grid;
	protected List<Cube[]> triominos = new List<Cube[]>();

	protected int deadColumns = 1;

	void Awake()
	{
		grid = new Cube[columns * rows];
	}

	void Update()
	{
		for (int column = 0; column < columns; column++)
		{
			for (int row = 0; row < rows; row++)
			{
				Cube cube = GetCell(column, row);

				if (cube)
				{
					MoveToEmpty(column, row);

					Vector3 target = GetCellPosition(cube, column, row);
					Vector3 direction = target - cube.transform.position;

					if (direction.magnitude > 0.4f)
						cube.transform.position += direction.normalized * 20 * Time.deltaTime;
					else
						cube.transform.position = target;
				}
			}
		}
	}

	public void AddTriomino(Vector2Int[] positions, Stack<Cube> cubes)
	{
		foreach (Vector2Int position in positions)
			AddCube(cubes.Pop(), position.x, position.y);
	}

	protected void AddCube(Cube cube, int column, int row)
	{
		if (!GetCell(column, row))
		{
			SetCell(column, row, cube);
			cube.transform.position = GetCellPosition(cube, column, row);
		}
		else
		{
			Debug.Log("DESTROY");
			Destroy(cube.gameObject);
		}
	}

	protected void MoveToEmpty(int column, int row)
	{
		Cube cube = GetCell(column, row);

		if (!cube)
			return;

		for (int index = deadColumns; index < column; index++)
		{
			if(!GetCell(index, row))
			{
				SetCell(column, row, null);
				SetCell(index, row, cube);
				return;
			}
		}
	}

	protected Vector3 GetCellPosition(Cube cube, int column, int row)
	{
		return new Vector3(
			firstPosition.x + (cube.GetComponent<BoxCollider>().bounds.size.x + margin) * column,
			firstPosition.y - (cube.GetComponent<BoxCollider>().bounds.size.y + margin) * row,
			0
		);
	}

	protected void SetCell(int column, int row, Cube cube)
	{
		grid[column + row * columns] = cube;
	}

	protected Cube GetCell(int column, int row)
	{
		return grid[column + row * columns];
	}
}
