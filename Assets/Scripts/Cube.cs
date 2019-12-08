using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
	[HideInInspector] public float speed = 0;

	protected Rigidbody rb;
	protected Store store;

	protected bool interactive = true;
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

	void Start()
	{
		UpdateState();
	}

	void Update()
	{
		rb.position += new Vector3(0, speed, 0) * Time.deltaTime;
	}

	void OnMouseDown()
	{
		if (!interactive)
			return;

		if (!store.AddCube(this))
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

	public void SetInteractive(bool value)
	{
		interactive = value;
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
