using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase : MonoBehaviour
{

	[SerializeField] private GameObject[] _enemys;

	public GameObject[] Enemys
	{
		get { return _enemys; }
	}
}
