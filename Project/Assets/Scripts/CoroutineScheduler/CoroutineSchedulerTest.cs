using UnityEngine;
using System.Collections;


/// <summary>
/// CoroutineSchedulerTest.cs
/// 
/// Port of the Javascript version from 
/// http://www.unifycommunity.com/wiki/index.php?title=CoroutineScheduler
/// 
/// Linked list node type used by coroutine scheduler to track scheduling of coroutines.
///  
/// BMBF Researchproject http://playfm.htw-berlin.de
/// PlayFM - Serious Games für den IT-gestützten Wissenstransfer im Facility Management 
///	Gefördert durch das bmb+f - Programm Forschung an Fachhochschulen profUntFH
///	
///	<author>Frank.Otto@htw-berlin.de</author>
///
/// </summary>

public class CoroutineSchedulerTest : MonoBehaviour
{
	
	CoroutineScheduler scheduler;
	// Use this for initialization
	void Start ()
	{
		scheduler = new CoroutineScheduler ();
		scheduler.StartCoroutine (MyCoroutine ());
	}
	
	
	IEnumerator MyCoroutine ()
	{
		Debug.Log ("MyCoroutine: Begin");
		yield return 0;
		// wait for next update
		Debug.Log ("MyCoroutine: next update;" + Time.time);
		yield return 2;
		// wait for 2 updates, same as yield; yield;
		Debug.Log ("MyCoroutine: After yield 2;" + Time.time);
		yield return 3.5f;
		// wait for 3.5 seconds
		Debug.Log ("MyCoroutine: After 3.5 seconds;" + Time.time);
		// you can also yield for a coroutine running on a completely different scheduler instance
		yield return scheduler.StartCoroutine (WaitForMe ());
		Debug.Log ("MyCoroutine: After WaitForMe() finished;" + Time.time);
	}
	
	IEnumerator WaitForMe ()
	{
		yield return 7.8f;
		// wait for 7.8 seconds before finishing
	}
	
	// Update is called once per 	
	void Update ()
	{
		
		scheduler.UpdateAllCoroutines (Time.frameCount, Time.time);
	}
	
	
}