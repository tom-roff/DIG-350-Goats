using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A game manager using Singleton and Service Locator patterns
/// 1. Add this to a game object to make a singleton
/// 2. Add child objects with managers (e.g. SoundManager) for global referencing
/// </summary>
public class GameManager : NetworkBehaviour
{
	/////////////////////////////////////////////////////
	//////////////////// SINGLETON //////////////////////
	/////////////////////////////////////////////////////

	// *** SINGLETON => make instance accessible outside of class
	public static GameManager Instance { get; private set; }

	// *** SINGLETON => only create once
	public bool singletonCreated = false;

	public OurNetwork OurNetwork { get; set; }
	public MapManager MapManager { get; set; }

	[Tooltip("Turn on debugging")]
	public bool DEBUG = true;

	void CreateSingleton()
	{
		// *** SINGLETON => If instance exists ...
		if (Instance != null && Instance.singletonCreated)
		{
			while (transform.childCount > 0)
			{
				DestroyImmediate(transform.GetChild(0).gameObject);
			}
			// *** SINGLETON => Then delete the object and exit
			DestroyImmediate(this.gameObject);
			return;
		}
		if (Instance != null && Instance != this)
		{
			Destroy(this);
			return;
		}

		// *** SINGLETON => Only reach this point on the first load...
		singletonCreated = true;
		Instance = this;
		DontDestroyOnLoad(this.gameObject);

		// @@@ SERVICE LOCATOR => Store references for global access
		OurNetwork = GetComponentInChildren<OurNetwork>();
		MapManager = GetComponentInChildren<MapManager>();

		if (DEBUG)
			Debug.Log($"*** GameManager (Singleton) created ***");
	}

	private void Awake()
	{
		CreateSingleton();
	}
}