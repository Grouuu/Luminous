using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeBar : MonoBehaviour
{
	public GameObject bar;
	public float duration;
	public float incrementSpeed;

	protected Grid grid;

	protected float currentIncrement = 1;
	protected bool available = false;

	protected float time;

	void Awake()
	{
		grid = FindObjectOfType<Grid>();
	}

	void Start()
	{
		ResetTimer();
	}

	private void Update()
	{
		if (!available)
			return;

		time += currentIncrement * Time.deltaTime;
		bar.transform.localScale = new Vector3(time / duration, 1, 1);

		if (time >= duration)
			TimerComplete();
	}

	public void StartTimer()
	{
		available = true;
	}

	public void PauseTimer()
	{
		available = false;
	}

	public void StopTimer()
	{
		available = false;
		ResetTimer();
	}

	protected void TimerComplete()
	{
		ResetTimer();
		currentIncrement += incrementSpeed;
		grid.RemoveLastLine();
	}

	protected void ResetTimer()
	{
		time = 0;
	}
}
