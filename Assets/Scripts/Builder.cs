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
	}

	void Update()
	{
		if (selectedSlots.Count >= maxPosition)
			Release();
	}

	public bool AddCube(int column, int row)
	{
		if (!CheckSlotValid(column, row))
			return false;

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

	protected bool CheckSlotValid(int column, int row)
	{
		if (selectedSlots.Count == 0)
			return true;

		for (int c = Mathf.Max(0, column - 1); c <= Mathf.Min(columns - 1, column + 1); c++)
			for (int r = Mathf.Max(0, row - 1); r <= Mathf.Min(rows - 1, row + 1); r++)
				if (!(c == column && r == row) && !(c != column && r != row) && selectedSlots.Contains(GetSlot(c, r)))
					return true;

		return false;
	}

	protected void RestrictSlots()
	{
		int[,] positions = new int[columns, rows];

		foreach (Slot slot in selectedSlots)
		{
			for (int column = 0; column < columns; column++)
			{
				for (int row = -1; row <= 1; row++)
				{
					int targetRow = slot.row + row;

					if (targetRow >= 0 && targetRow < rows)
						positions[column, targetRow]++;
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
		Vector2Int[] positions = new Vector2Int[selectedSlots.Count];

		for (int index = 0; index < selectedSlots.Count; index++)
			positions[index] = new Vector2Int(grid.columns - columns + selectedSlots[index].column, selectedSlots[index].row);

		grid.AddTriomino(positions, store.RemoveSelectedCubes());

		added = 0;
		selectedSlots = new List<Slot>();
		EmptySlots();
		RestrictSlots();
	}

	protected void EmptySlots()
	{
		foreach (Slot slot in slots)
			slot.SetEmpty(true);
	}

	protected Slot GetSlot(int column, int row)
	{
		return slots[column + row * columns];
	}
}
