
using System.Collections;
using UnityEngine;

public class GS_Game : GameState {

	const float GAME_TIME = 60;
	public const int SCORE = 10;

	// GAME DATA

	float timer;

	// GRAPHICS

	bool ready;

	FlameTexture fire;
	Coroutine fireTick;
	Doors doors;

	public GS_Game ( ) {

		ready = false;

		fire = new FlameTexture(1280, 19);
		doors = new Doors();
		doors.Force(0);

	}

	public override IEnumerator Start ( ) {
		ready = false;
		updateSprites = true;
		fireTick = G.I.StartCoroutine(fire.Tick());
		updateEntities = true;
		NewRound();
		yield return new WaitForFixedUpdate();
		updateEntities = false;
		yield return new WaitForSeconds(1);
		doors.Goto(400);
		yield return new WaitForSeconds(1);
		updateEntities = true;
		ready = true;
	}

	public override IEnumerator End ( ) {
		ready = false;
		updateEntities = false;
		G.I.StopCoroutine(fireTick);
		yield return new WaitForSeconds(0.15f);
		updateSprites = false;
		yield return new WaitForSeconds(1);
		yield return new WaitForFixedUpdate();
		doors.Activate();
		yield return new WaitForSeconds(3);
		G.I.StartCoroutine(Start());
	}

	public IEnumerator EndGame ( ) {
		ready = false;
		updateEntities = false;
		G.I.StopCoroutine(fireTick);
		yield return new WaitForSeconds(0.15f);
		updateSprites = false;
		yield return new WaitForSeconds(1);
		yield return new WaitForFixedUpdate();
		doors.Activate();
		yield return new WaitForSeconds(3);
		G.I.NextGameState(new GS_PostGame());
	}

	public void NewRound ( ) {
		var g = G.I;
		g.Clear();
		g.level.Generate();
		g.SpawnPlayers();
		timer = GAME_TIME;
	}

	public override void Update (float dt) {
		if (Input.GetKeyDown(KeyCode.R)) {
			NewRound();
		}

		doors.Update(dt);

		var g = G.I;
		var p = g.players;

		if (ready) {
			var allDead = true;
			int alive = 0;
			foreach (var ply in p.players) {
				if (ply.linkedPlayer != null) {
					if (ply.linkedPlayer.alive) {
						allDead = false;
						alive++;
					}
					if (ply.score >= SCORE) {
						G.I.StartCoroutine(EndGame());
					}
				}
			}

			if (allDead || alive <= 1) {
				g.StartCoroutine(End());
			} else {
				if (timer > 0) {
					timer -= dt;
					if (timer <= 0) {
						foreach (var ply in p.players) {
							if (ply.linkedPlayer != null)
								ply.linkedPlayer.Kill(null);
						}
						g.StartCoroutine(End());
					}
				} else {
					timer = 0;
				}
			}

		}
	}

	public override void Render ( ) {

		UI.Rect(0, 360 - 38, 640, 38, Color.black);
		UI.Texture(fire.texture, 0, 341, 0.5f, 1, false);
		UI.Texture(fire.texture, 0, 322, 0.5f, 1, true);

		var p = G.I.players;

		var offset = 0;
		if (p.players[0].active) {
			UI.Image(13, offset, 360 - 64);
			var score = p.players[0].score / 10;
			var score_2 = p.players[0].score % 10;
			UI.Number(offset + 128 - 32, 360 - 32, score, Color.white);
			UI.Number(offset + 128 - 16, 360 - 32, score_2, Color.white);
		}

		offset += 128;

		if (p.players[1].active) {
			UI.Image(14, offset, 360 - 64);
			var score = p.players[1].score / 10;
			var score_2 = p.players[1].score % 10;
			UI.Number(offset + 128 - 32, 360 - 32, score, Color.white);
			UI.Number(offset + 128 - 16, 360 - 32, score_2, Color.white);
		}

		offset += 128;

		// Logo
		//UI.Image(17, offset, 360 - 64);

		UI.TextOutline("TIME LEFT:", offset + 19, 360 - 19, Color.white, Color.black, 1);
		var t = Mathf.CeilToInt(timer) / 10;
		var t_2 = Mathf.CeilToInt(timer) % 10;
		UI.Number(offset + 48, 360 - 35, t, Color.white);
		UI.Number(offset + 64, 360 - 35, t_2, Color.white);

		offset += 128;

		if (p.players[2].active) {
			UI.Image(15, offset, 360 - 64);
			var score = p.players[2].score / 10;
			var score_2 = p.players[2].score % 10;
			UI.Number(offset, 360 - 32, score, Color.white);
			UI.Number(offset + 16, 360 - 32, score_2, Color.white);
		}

		offset += 128;

		if (p.players[3].active) {
			UI.Image(16, offset, 360 - 64);
			var score = p.players[3].score / 10;
			var score_2 = p.players[3].score % 10;
			UI.Number(offset, 360 - 32, score, Color.white);
			UI.Number(offset + 16, 360 - 32, score_2, Color.white);
		}

		doors.Render();

		//UI.Text(Mathf.FloorToInt(1 / Time.deltaTime).ToString(), 0, 10, Color.black);
		//UI.Text("IN-GAME", 0, 0, Color.black);

	}

}