using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
	[HideInInspector] public float speed;

	protected Rigidbody rb;
	protected Store store;
	protected bool free = true;
	protected bool available = true;

	protected Transform bg_enable;
	protected Transform bg_disable;

	void Awake()
	{
		rb = GetComponent<Rigidbody>();
		store = FindObjectOfType<Store>();

		bg_enable = gameObject.transform.Find("cube_on");
		bg_disable = gameObject.transform.Find("cube_off");

		UpdateState();
	}

	void Update()
	{
		if (free)
			rb.position += new Vector3(0, speed, 0) * Time.deltaTime;
	}

	void OnMouseDown()
	{
		if (!free)
			return;

		free = false;
		bool added = store.AddCube(this);

		if (!added)
			Destroy(gameObject);
	}

	void OnBecameInvisible()
	{
		Destroy(gameObject);
	}

	public void SetAvailable(bool value)
	{
		available = value;
		UpdateState();
	}

	public void SetFree(bool value)
	{
		free = value;
	}

	protected void UpdateState()
	{
		if (available)
		{
			bg_enable.gameObject.SetActive(true);
			bg_disable.gameObject.SetActive(false);
		}
		else
		{
			bg_enable.gameObject.SetActive(false);
			bg_disable.gameObject.SetActive(true);
		}
	}
}
