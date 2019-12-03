using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour
{
	public Slot slotPrefab;
	public Vector3 firstPosition;
	public float margin;
	public int columns;
	public int rows;
	public int maxPosition;

	protected Store store;
	protected Grid grid;
	protected Slot[] slots;
	protected List<Slot> selectedSlots = new List<Slot>();

	protected int added = 0;

	void Awake()
	{
		store = FindObjectOfType<Store>();
		grid = FindObjectOfType<Grid>();
	}

	void Start()
	{
		CreateSlots();
		EmptySlots();
	}

	void Update()
	{
		if (selectedSlots.Count >= maxPosition)
			Release();
	}

	public bool AddCube(int column, int row)
	{
		if (!store.SelectCube())
			return false;

		added++;
		selectedSlots.Add(GetSlot(column, row));
		RestrictSlots();

		return true;
	}

	public bool RemoveCube(int column, int row)
	{
		if (!store.UnselectCube())
			return false;

		if (added > 0)
			added--;

		selectedSlots.Remove(GetSlot(column, row));
		RestrictSlots();

		return true;
	}

	protected void RestrictSlots()
	{
		int[,] positions = new int[columns, rows];

		foreach (Slot slot in selectedSlots)
		{
			for (int column = -1; column <= 1; column++)
			{
				for (int row = -1; row <= 1; row++)
				{
					int targetCol = slot.column + column;
					int targetRow = slot.row + row;

					if (targetCol >= 0 && targetCol < columns && targetRow >= 0 && targetRow < rows)
						positions[targetCol, targetRow]++;
				}
			}
		}

		foreach (Slot slot in slots)
			slot.SetAvailable(positions[slot.column, slot.row] == selectedSlots.Count);
	}

	protected void CreateSlots()
	{
		slots = new Slot[columns * rows];

		for (int column = 0; column < columns; column++)
		{
			for (int row = 0; row < rows; row++)
			{
				Slot slot = Instantiate(slotPrefab);
				slot.transform.position = new Vector3(
					firstPosition.x + (slot.GetComponent<BoxCollider>().bounds.size.x + margin) * column,
					firstPosition.y - (slot.GetComponent<BoxCollider>().bounds.size.y + margin) * row,
					0
				);
				slot.column = column;
				slot.row = row;
				slots[column + row * columns] = slot;
			}
		}		
	}

	protected void Release()
	{
		Stack<Cube> cubes = store.RemoveSelectedCubes();

		foreach(Slot slot in selectedSlots)
			grid.AddCube(cubes.Pop(), grid.columns - (columns - slot.column), slot.row);

		added = 0;
		selectedSlots = new List<Slot>();
		EmptySlots();
		RestrictSlots();
	}

	protected void EmptySlots()
	{
		foreach (Slot slot in slots)
			slot.SetEmpty();
	}

	protected Slot GetSlot(int column, int row)
	{
		return slots[column + row * columns];
	}
}
