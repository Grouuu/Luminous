using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
	[HideInInspector] public Store store;

	void Awake()
	{
		store = GetComponent<Store>();
	}
}
