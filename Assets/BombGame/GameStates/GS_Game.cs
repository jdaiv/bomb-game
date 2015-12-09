
using System.Collections;
using UnityEngine;

public class GS_Game : GameState {
	
	float door1Pos;
	float door2Pos;
	bool ready;

	public GS_Game ( ) {

		door1Pos = 0;
		door2Pos = 0;
		ready = false;

	}

	public override IEnumerator Start ( ) {
		G.I.NewRound();
		yield return new WaitForEndOfFrame();
		ready = true;
	}

	public override IEnumerator End ( ) {
		yield return new WaitForEndOfFrame();
	}

	public override void Update (float dt) {
		if (ready) {
			door1Pos = Mathf.Lerp(door1Pos, 400, dt * 5);
			door2Pos = Mathf.Lerp(door2Pos, 400, dt * 5);
		}
	}

	public override void Render ( ) {
		
		var tex = G.I.uiSprites[12];
		UI.Texture(tex, 0 - door1Pos, 0);
		UI.Texture(tex, 640 + door2Pos, 0, -1, 1);

		UI.Text(Mathf.FloorToInt(1 / Time.deltaTime).ToString(), 0, 10, Color.black);
		UI.Text("IN-GAME", 0, 0, Color.black);

	}

}