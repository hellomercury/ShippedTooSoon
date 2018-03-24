﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GameManager : MonoBehaviour {
	private GameMap gameMap = null;
	private PlayerMovement player = null;
	private CameraController roomCamera = null;
	private GameTimeManager timeManager = new GameTimeManager();

	// Room management
	private Room currentRoom = null;
	private Room previousRoom = null;
	private bool hasSwitchedRoom = false;


	// -------------------------------------------------------------------------
	// Unity Methods
	// -------------------------------------------------------------------------
	void Start () {
		Debug.Log("GameManager::Start()");

		GameObject gameMapObject 	= GameObject.Find("GameMap");
		GameObject cameraObject 	= GameObject.Find("Main Camera");
		GameObject playerObject 	= GameObject.FindGameObjectWithTag("Player");

		Assert.IsNotNull(gameMapObject, "Unable to find GameMap object in scene");
		Assert.IsNotNull(cameraObject, "Unable to find Main Camera GameObject");
		Assert.IsNotNull(playerObject, "Unable to recover the Player GameObject");

		this.gameMap = gameMapObject.GetComponent<GameMap>();
		this.player = playerObject.GetComponent<PlayerMovement>();
		this.roomCamera = cameraObject.GetComponent<CameraController>();

		Assert.IsNotNull(this.gameMap, "Unable to recover GameMap script from GameMap Object");
		Assert.IsNotNull(this.roomCamera, "Unable to recover CameraController script");
		Assert.IsNotNull(this.player, "Unable to recover the player script");

		// Init setup
		this.currentRoom = this.gameMap.getRoomUnderWorldPos(this.player.transform.position);
		this.previousRoom = this.currentRoom;
	}
	
	// Update is called once per frame
	void Update () {
		this.updateCurrentRoom();
		this.updateCameraPosition();

		if(this.hasSwitchedRoom) {
			this.previousRoom.onRoomExit();
			this.currentRoom.onRoomEnter();
			this.previousRoom = this.currentRoom;
		}
	}


	// -------------------------------------------------------------------------
	// Core Methods
	// -------------------------------------------------------------------------
	private void updateCurrentRoom() {
		this.currentRoom = this.gameMap.getRoomUnderWorldPos(this.player.transform.position);
		Assert.IsNotNull(this.currentRoom, "Player is not in a room (But should be)");

		if(this.currentRoom != null) {
			this.hasSwitchedRoom = false;
			if(this.currentRoom.getId() != this.previousRoom.getId()) {
				this.hasSwitchedRoom = true;
			}
		}

	}

	private void updateCameraPosition() {
		if(this.currentRoom != null) {
			Vector3 center = this.gameMap.getCellCenterWorldFromId(this.currentRoom.getId());
			this.roomCamera.targetPosition = center;
		}
	}


	// -------------------------------------------------------------------------
	// Time Methods
	// -------------------------------------------------------------------------
	GameTimeManager GetTimeManager() {
		return this.timeManager;
	}
}
