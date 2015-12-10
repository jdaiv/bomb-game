
using System.Collections;
using UnityEngine;

public class GS_PreGame : GameState {

	const float START_TIMER = 2f;

	float[] portrait;
	float[] portraitV;
	int[] flashes;
	bool joinFlip;
	float flipTimer;
	float flashTimer;
	float startTimer;
	bool gameStarting;

	FlameTexture fire;
	Coroutine fireTick;
	Doors doors;

	public GS_PreGame ( ) {
		portrait = new float[] { 0, 0, 0, 0 };
		portraitV = new float[] { 0, 0, 0, 0 };
		flashes = new int[] { 0, 0, 0, 0 };

		fire = new FlameTexture(640, 40);
		doors = new Doors();
		doors.Force(0);
	}

	public override IEnumerator Start ( ) {
		var p = G.I.players;
		p.PlayerJoined += playerJoined;
		p.PlayerLeft += playerLeft;

		fireTick = G.I.StartCoroutine(fire.Tick());

		doors.Goto(400);

		yield return new WaitForEndOfFrame();
	}

	public override IEnumerator End ( ) {
		// ?
		G.I.StopCoroutine(fireTick);

		doors.Activate();

		yield return new WaitForSeconds(4f);

		var p = G.I.players;
		p.PlayerJoined -= playerJoined;
		p.PlayerLeft -= playerLeft;
		yield return new WaitForEndOfFrame();
		G.I.NextGameState(new GS_Game());
	}

	public void playerJoined (P.PlayerData ply) {
		playerFlash(ply.id);
		G.I.PlaySound(24);
	}

	public void playerLeft (P.PlayerData ply) {
		G.I.PlaySound(26);
	}

	public void playerFlash (int index) {
		flashes[index] = 5;
		portrait[index] = 20;
	}

	public override void Update (float dt) {
		if (!gameStarting) { 
			var p = G.I.players;

			var activePlayers = 0;
			var readyPlayers = 0;

			for (int i = 0; i < 4; i++) {
				var ply = p.players[i];
				if (ply.active && ply.device != null) {
					if (ply.device.Action4.WasPressed) {
						p.RemovePlayer(ply.device);
					} else if (ply.device.Action1.WasPressed) {
						ply.ready = !ply.ready;
						flashes[i] = 4;
						portraitV[i] = 100;
						if (ply.ready) {
							G.I.PlaySound(25);
						} else {
							G.I.PlaySound(25);
						}
					}
					if (ply.ready) {
						readyPlayers++;
					}
					activePlayers++;
				}
				portraitV[i] -= 500 * dt;
				portrait[i] += portraitV[i] * dt;
				if (portrait[i] < 0) {
					portrait[i] = 0;
					portraitV[i] *= -0.6f;
				}
				//join[i] = Mathf.Lerp(join[i], -1000, dt * 5);
				//portrait[i] = Mathf.Lerp(portrait[i], 0, dt * 5);
			}

			if (activePlayers > 1 && readyPlayers == activePlayers) {
				startTimer -= dt;
				if (startTimer <= 0) {
					gameStarting = true;
					G.I.StartCoroutine(End());
				}
			} else {
				startTimer = START_TIMER;
			}

			flipTimer += dt;
			flashTimer += dt;
			if (flipTimer >= 0.25f) {
				joinFlip = !joinFlip;
				flipTimer -= 0.25f;
			}
			if (flashTimer >= 0.05f) {
				flashTimer -= 0.05f;
				for (int i = 0; i < 4; i++) {
					if (flashes[i] > 0)
						flashes[i]--;
				}
			}
		}

		doors.Update(dt);
	}

	public override void Render ( ) {

		var middle = 179;

		UI.Rect(0, 0, 640, 360, Color.black);
		UI.Texture(fire.texture, 0, -8, 1, 4);
		UI.Texture(fire.texture, 640, 208, -1, 4, true);
		UI.Image(10, 0, 0);

		for (int i = 0; i < 4; i++) {
			var ply = G.I.players.players[i];
			var x = i * 158 + 19;
			UI.Image(11, x, middle - 64);
			if (ply.active) {
				UI.Text("(A) TO READY", x, middle - 76, Color.white);
				UI.Text("(X) TO LEAVE", x, middle - 88, Color.white);
				if (flashes[i] % 2 == 1) {
					UI.Image(3 + i, x, middle - 64 + portrait[i], Color.black);
				} else {
					UI.Image(3 + i, x, middle - 64 + portrait[i]);
				}
				if (ply.ready)
					UI.Image(7, x + 24, middle - 8 + portrait[i]);
			} else {
				UI.Text("(A) TO JOIN", x, middle - 76, Color.white);
				UI.Image(((i % 2 == 0) ? joinFlip : !joinFlip) ? 9 : 8, x + 32, middle - 32);
			}
		}

		if (startTimer < START_TIMER) {
			UI.Text("GAME START IN:", 320 - 63, 268, Color.white);
			var timer = Mathf.CeilToInt(startTimer);
			UI.Number(320 - 16, 250, timer / 10, Color.white);
			UI.Number(320, 250, timer % 10, Color.white);
		}

		doors.Render();

		UI.Text("PRE-GAME", 0, 0, Color.green);
		UI.Text(startTimer.ToString(), 0, 10, Color.green);

	}

}