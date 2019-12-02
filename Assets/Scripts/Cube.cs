using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
	[HideInInspector] public float speed;

	protected Rigidbody rb;
	protected GameController game;
	protected bool free = true;
	protected bool available = true;

	protected Transform bg_enable;
	protected Transform bg_disable;

	void Awake()
	{
		rb = GetComponent<Rigidbody>();
		game = FindObjectOfType<GameController>();

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
		bool added = game.store.AddCube(this);

		if (!added)
			Destroy(gameObject);
	}

	void OnBecameInvisible()
	{
		Destroy(gameObject);
	}

	public void UpdateState()
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

	public void SetAvailable(bool value)
	{
		available = value;
		UpdateState();
	}
}
