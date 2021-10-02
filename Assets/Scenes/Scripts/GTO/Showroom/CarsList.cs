using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Car List", menuName = "Car List", order = 51)]
public class CarsList : ScriptableObject
{

	public GameObject[] vehicles;

	#region singleton
	private static CarsList instance;
	public static CarsList Instance { get { if (instance == null) instance = Resources.Load("Resourses") as CarsList; return instance; } }
	#endregion
}
