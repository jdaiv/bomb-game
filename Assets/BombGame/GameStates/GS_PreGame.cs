
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
	float door1Pos;
	float door2Pos;
	float door1Vel;
	float door2Vel;

	Texture2D bg;

	Coroutine fire;

	public GS_PreGame ( ) {
		portrait = new float[] { 0, 0, 0, 0 };
		portraitV = new float[] { 0, 0, 0, 0 };
		flashes = new int[] { 0, 0, 0, 0 };

		door1Pos = 400;
		door2Pos = 400;
		door1Vel = 0;
		door2Vel = 0;

		bg = new Texture2D(640, 40, TextureFormat.RGB24, false);
		bg.filterMode = FilterMode.Bilinear;
		bg.wrapMode = TextureWrapMode.Clamp;
		for (int x = 0; x < bg.width; x++) {
			for (int y = bg.height; y >= 0; y--) {
				bg.SetPixel(x, y, Color.clear);
			}
		}
		bg.Apply();
	}

	public override IEnumerator Start ( ) {
		var p = G.I.players;
		p.PlayerJoined += playerJoined;

		fire = G.I.StartCoroutine(FireTick());

		yield return new WaitForEndOfFrame();
	}

	public override IEnumerator End ( ) {
		// ?
		G.I.StopCoroutine(fire);

		yield return new WaitForSeconds(4f);

		var p = G.I.players;
		p.PlayerJoined -= playerJoined;
		yield return new WaitForEndOfFrame();
		G.I.NextGameState(new GS_Game());
	}

	public void playerJoined (P.PlayerData ply) {
		playerFlash(ply.id);
	}

	public void playerLeft (P.PlayerData ply) {

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

			if (activePlayers > 0 && readyPlayers == activePlayers) {
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
		} else {

			door1Vel -= 500 * dt;
			door1Pos += door1Vel * dt;
			if (door1Pos < 0) {
				door1Pos = 0;
				door1Vel *= Random.Range(-0.1f, -0.3f);
			}

			door2Vel -= 500 * dt;
			door2Pos += door2Vel * dt;
			if (door2Pos < 0) {
				door2Pos = 0;
				door2Vel *= Random.Range(-0.1f, -0.3f);
			}

		}
	}

	public IEnumerator FireTick ( ) {
		var black = new Color32(180, 0, 0, 255);
		var red = new Color32(255, 0, 0, 255);
		var orange = new Color32(255, 85, 0, 255);
		var yellow = new Color32(255, 170, 0, 255);

		int counter = 0;
		int delay = bg.height / 3;
		while (true) {
			var pixels = bg.GetPixels32();
			for (int y = bg.height - 1; y >= 0; y--) {
				for (int x = 1; x < bg.width - 1; x++) {
					var i = x + y * bg.width;
					//for (int y = 0; y < bg.height; y++) {
					if (y == 0) {
						pixels[i] = (U.RandomBool() ? U.RandomBool() ? U.RandomBool() ? orange : yellow : red : black);
					} else {
						//var color1 = bg.GetPixel(x, y - 1);
						//var color2 = bg.GetPixel(x + 1, y - 1);
						//var color3 = bg.GetPixel(x + 2, y - 1);
						//var color4 = bg.GetPixel(x - 1, y - 1);
						//var color5 = bg.GetPixel(x - 2, y - 1);
						//var color = color1 * 0.70f + color2 * 0.15f /*+ color3 * 0.05f*/ + color4 * 0.15f /*+ color5 * 0.05f*/;
						var color1 = pixels[i - bg.width];
						var color2 = pixels[i - bg.width - 1];
						var color3 = pixels[i - bg.width + 1];
						var color = new Color32(
							(byte)((color1.r + (color1.r / 1.6f) + color2.r + color3.r) / 4),
							(byte)((color1.g + (color1.g / 1.6f) + color2.g + color3.g) / 4),
							(byte)((color1.b + (color1.b / 1.6f) + color2.b + color3.b) / 4),
							255
							);
						pixels[i] = color;
					}
				}
				counter++;
				if (counter >= delay) {
					counter = 0;
					yield return new WaitForSeconds(0.01f);
				}
			}
			counter = 0;
			bg.SetPixels32(pixels);
			bg.Apply();
		}
	}

	public override void Render ( ) {

		var middle = 179;

		UI.Rect(0, 0, 640, 360, Color.black);
		UI.Texture(bg, 0, -8, 1, 4);
		UI.Texture(bg, 640, 208, -1, 4, true);
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

		if (gameStarting) {
			var tex = G.I.uiSprites[12];
			UI.Texture(tex, 0 -door1Pos, 0);
			UI.Texture(tex, 640 + door2Pos, 0, -1, 1);
		}

		UI.Text("PRE-GAME", 0, 0, Color.green);
		UI.Text(startTimer.ToString(), 0, 10, Color.green);

	}

}