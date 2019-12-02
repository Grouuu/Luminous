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
	protected Slot[] slots;
	protected bool[,] positions;

	protected int added = 0;

	void Awake()
	{
		store = FindObjectOfType<Store>();
		positions = new bool[columns, rows];
	}

	void Start()
	{
		EmptyPositions();
		CreateSlots();
		EmptySlots();
	}

	public bool AddCube(int column, int row)
	{
		// TODO: check if valid position OR invalidate unvalid slots

		if (!store.SelectCube())
			return false;

		added++;
		positions[column, row] = true;

		if (added >= maxPosition)
			Release();

		return true;
	}

	public bool RemoveCube(int column, int row)
	{
		if (!store.UnselectCube())
			return false;

		if (added > 0)
			added--;

		positions[column, row] = false;

		return true;
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
		added = 0;
		EmptyPositions();
		EmptySlots();
		store.RemoveSelectedCubes();
	}

	protected void EmptyPositions()
	{
		for (int column = 0; column < columns; column++)
			for (int row = 0; row < rows; row++)
				positions[column, row] = false;
	}

	protected void EmptySlots()
	{
		foreach (Slot slot in slots)
			slot.SetAvailable(true);
	}

	protected Slot GetSlot(int column, int row)
	{
		return slots[column + row * columns];
	}
}
