using InControl;
using System.Collections.Generic;
using UnityEngine;

public class P {

	const int MAX_PLAYERS = 4;

	public class PlayerData {
		public int id;
		public bool active;
		public Player linkedPlayer;
		public InputDevice device;
		public int hat;
		public int body;
		public bool ready;

		public PlayerData (int id) {
			this.id = id;
			active = false;
			linkedPlayer = null;
			ready = false;
		}
	}

	int activePlayers;
	public PlayerData[] players;

	public P ( ) {
		players = new PlayerData[MAX_PLAYERS];
		for (int i = 0; i < MAX_PLAYERS; i++) {
			players[i] = new PlayerData(i);
		}

		activePlayers = 0;

		InputManager.OnDeviceDetached += ControllerDisconnected;
	}

	void ControllerDisconnected (InputDevice device) {
		RemovePlayer(device);
	}

	public void CheckPlayers ( ) {
		if (activePlayers < MAX_PLAYERS) {
			var inputDevice = InputManager.ActiveDevice;
			if (inputDevice.Action1.WasPressed) {
				if (FindPlayer(inputDevice) == null) {
					AddPlayer(inputDevice);
				}
			} else if (InputManager.AnyKeyIsPressed) {
				if (FindPlayer(null) == null) {
					AddPlayer(null);
				}
			}
		}
	}

	public PlayerData FindPlayer (InputDevice device) {
		foreach (var ply in players) {
			if (ply.active) {
				if (ply.device == device) {
					return ply;
				}
			}
		}
		return null;
	}

	public void AddPlayer (InputDevice device) {
		foreach (var ply in players) {
			if (!ply.active) {
				ply.active = true;
				ply.device = device;
				break;
			}
		}
		activePlayers++;
	}

	public void RemovePlayer (InputDevice device) {
		foreach (var ply in players) {
			if (ply.active) {
				if (ply.device == device) {
					ply.active = false;
					break;
				}
			}
		}
		activePlayers--;
	}

}


