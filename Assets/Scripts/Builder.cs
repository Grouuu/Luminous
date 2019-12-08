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
	protected List<Slot> selectedSlots;

	void Awake()
	{
		store = FindObjectOfType<Store>();
		grid = FindObjectOfType<Grid>();
	}

	void Start()
	{
		CreateSlots();
		Reset();
	}

	void Update()
	{
		if (selectedSlots.Count >= maxPosition)
			Release();
	}

	public bool SelectSlot(int column, int row)
	{
		if (!store.SelectCube())
			return false;

		Slot slot = GetSlot(column, row);
		slot.SetSelected(true);
		selectedSlots.Add(slot);
		RestrictSlots();
		return true;
	}

	public bool UnselectSlot(int column, int row)
	{
		if (!store.UnselectCube())
			return false;

		Slot slot = GetSlot(column, row);
		slot.SetSelected(false);
		selectedSlots.Remove(slot);
		RestrictSlots();
		return true;
	}

	protected void RestrictSlots()
	{
		AvailableAllSlots();

		if (selectedSlots.Count == 0)
			return;

		int[,] positions = new int[columns, rows];

		foreach (Slot slot in selectedSlots)
		{
			for (int c = Mathf.Max(0, slot.column - 1); c <= Mathf.Min(columns - 1, slot.column + 1); c++)
				for (int r = Mathf.Max(0, slot.row - 1); r <= Mathf.Min(rows - 1, slot.row + 1); r++)
					if (!(c != slot.column && r != slot.row))
						positions[c, r]++;
		}

		foreach (Slot slot in slots)
		{
			if (positions[slot.column, slot.row] == 0)
				slot.SetAvailable(false);
		}
	}

	protected void Release()
	{
		// TODO do not remove from store non added cubes

		int maxCol = 0;
		int minCol = columns - 1;

		foreach (Slot slot in selectedSlots)
		{
			maxCol = Mathf.Max(maxCol, slot.column);
			minCol = Mathf.Min(minCol, slot.column);
		}

		int widthGroup = maxCol - minCol + 1;
		Vector2Int[] positions = new Vector2Int[selectedSlots.Count];

		for (int index = 0; index < selectedSlots.Count; index++)
			positions[index] = new Vector2Int(grid.columns - widthGroup + (selectedSlots[index].column - minCol), selectedSlots[index].row);

		grid.AddGroup(positions, store.RemoveSelectedCubes());
		Reset();
	}

	protected void Reset()
	{
		selectedSlots = new List<Slot>();
		UnselectAllSlots();
		AvailableAllSlots();
		RestrictSlots();
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

	protected void UnselectAllSlots()
	{
		foreach (Slot slot in slots)
			slot.SetSelected(false);
	}

	protected void AvailableAllSlots()
	{
		foreach (Slot slot in slots)
			slot.SetAvailable(true);
	}

	protected Slot GetSlot(int column, int row)
	{
		return slots[column + row * columns];
	}
}
