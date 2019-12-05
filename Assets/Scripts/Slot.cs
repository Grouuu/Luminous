using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
	[HideInInspector] public int row;
	[HideInInspector] public int column;

	protected Builder builder;

	protected bool empty = true;
	protected bool available = true;

	protected Transform bg_empty;
	protected Transform bg_full;
	protected Transform bg_disable;

	void Awake()
	{
		builder = FindObjectOfType<Builder>();

		bg_empty = gameObject.transform.Find("center_empty");
		bg_full = gameObject.transform.Find("center_full");
		bg_disable = gameObject.transform.Find("center_disable");
	}

	void Start()
	{
		SetEmpty(true);
		SetAvailable(true);
	}

	void OnMouseDown()
	{
		if (!available)
			return;

		if (empty)
			SetEmpty(!builder.AddCube(column, row));
		else
			SetEmpty(builder.RemoveCube(column, row));
	}

	public void SetAvailable(bool value)
	{
		available = value;
		UpdateState();
	}

	public void SetEmpty(bool value)
	{
		empty = value;
		UpdateState();
	}

	protected void UpdateState()
	{
		if (!available)
		{
			// disable
			bg_empty.gameObject.SetActive(false);
			bg_full.gameObject.SetActive(false);
			bg_disable.gameObject.SetActive(true);
		}
		else if (!empty)
		{
			// full
			bg_empty.gameObject.SetActive(false);
			bg_full.gameObject.SetActive(true);
			bg_disable.gameObject.SetActive(false);
		}
		else
		{
			// empty
			bg_empty.gameObject.SetActive(true);
			bg_full.gameObject.SetActive(false);
			bg_disable.gameObject.SetActive(false);
		}
	}
}
