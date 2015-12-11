
using System.Collections;
using UnityEngine;

public class GS_PostGame : GameState {

	const float END_TIMER = 10f;

	FlameTexture fire;
	Coroutine fireTick;
	Doors doors;

	float timer = 1;
	int winner;

	public GS_PostGame ( ) {
		fire = new FlameTexture(640, 40);
		doors = new Doors();
		doors.Force(0);
	}

	public override IEnumerator Start ( ) {

		foreach (var ply in G.I.players.players) {
			if (ply.score >= GS_Game.SCORE) {
				winner = ply.id;
				break;
			}
		}

		G.I.players.ClearPlayers();
		fireTick = G.I.StartCoroutine(fire.Tick());
		timer = END_TIMER;
		yield return new WaitForSeconds(1);
		doors.Goto(400);
		yield return new WaitForEndOfFrame();
	}

	public override IEnumerator End ( ) {
		// ?
		G.I.StopCoroutine(fireTick);
		doors.Activate();

		yield return new WaitForSeconds(4f);
		G.I.NextGameState(new GS_PreGame());
	}

	public override void Update (float dt) {
		doors.Update(dt);
		if (timer > 0) {
			timer -= dt;
			if (timer <= 0) {
				timer = 0;
				G.I.StartCoroutine(End());
			}
		}
	}

	public override void Render ( ) {

		UI.Rect(0, 0, 640, 360, Color.black);
		UI.Texture(fire.texture, 0, -8, 1, 4);
		UI.Texture(fire.texture, 640, 208, -1, 4, true);
		UI.Image(10, 0, 0);
		UI.Image(11, 256, 116);
		UI.TextOutline("!WINNER!", 320 - 36, 246, Color.white, Color.black, 1);
		UI.Image(3 + winner, 256, 116);

		UI.TextOutline("NEXT ROUND IN:", 320 - 63, 80, Color.white, Color.black, 1);
		var t = Mathf.CeilToInt(timer) / 10;
		var t_2 = Mathf.CeilToInt(timer) % 10;
		UI.Number(320 - 16, 64, t, Color.white);
		UI.Number(320, 64, t_2, Color.white);

		doors.Render();

		//UI.Text("POST-GAME", 0, 0, Color.green);

	}

}