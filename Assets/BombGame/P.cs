using InControl;
using System.Collections.Generic;
using UnityEngine;

public class P {

	public delegate void PlayerEvent (PlayerData ply);

	const int MAX_PLAYERS = 4;

	public class PlayerData {
		public int id;
		public bool active;
		public Player linkedPlayer;
		public InputDevice device;
		public int hat;
		public int body;
		public int score;
		public bool ready;

		public PlayerData (int id) {
			this.id = id;
			active = false;
			linkedPlayer = null;
			ready = false;
		}
	}

	public int activePlayers;
	public PlayerData[] players;
	public PlayerEvent PlayerJoined;
	public PlayerEvent PlayerLeft;

	public P ( ) {
		players = new PlayerData[MAX_PLAYERS];
		for (int i = 0; i < MAX_PLAYERS; i++) {
			players[i] = new PlayerData(i);
		}

		activePlayers = 0;

		InputManager.OnDeviceDetached += ControllerDisconnected;
	}

	public void AddScore (int id, int amt) {
		if (id >= 0 && id < 4) {
			players[id].score += amt;
			if (players[id].score < 0) {
				players[id].score = 0;
			}
		}
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
			}
			//else if (InputManager.AnyKeyIsPressed) {
			//	if (FindPlayer(null) == null) {
			//		AddPlayer(null);
			//	}
			//}
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
		PlayerData p = null;
		bool success = false;
		foreach (var ply in players) {
			if (!ply.active) {
				ply.active = true;
				ply.ready = false;
				ply.device = device;
				p = ply;
				success = true;
				break;
			}
		}
		if (success) {
			if (PlayerJoined != null) {
				PlayerJoined(p);
			}
			activePlayers++;
		}
	}

	public void ClearPlayers ( ) {
		foreach (var ply in players) {
			ply.active = false;
			ply.ready = false;
			ply.score = 0;
		}
		activePlayers = 0;
	}

	public void RemovePlayer (InputDevice device) {
		PlayerData p = null;
		bool success = false;
		foreach (var ply in players) {
			if (ply.active) {
				if (ply.device == device) {
					ply.active = false;
					success = true;
					break;
				}
			}
		}
		if (success) {
			if (PlayerLeft != null) {
				PlayerLeft(p);
			}
			activePlayers--;
		}
	}

}


