
using UnityEngine;

public class GS_PreGame : GameState {


	float[] join;
	float[] ready;
	float[] portrait;
	bool joinFlip;
	float flipTimer;

	public GS_PreGame ( ) {
		join = new float[] { 0, 0, 0, 0 };
		ready = new float[] { -1000, -1000, -1000, -1000 };
		portrait = new float[] { 1000, 1000, 1000, 1000 };
	}

	public override void Start ( ) {

	}

	public override void End ( ) {

	}

	public override void Update (float dt) {
		for (int i = 0; i < 4; i++) {
			var ply = G.I.players.players[i];
			if (ply.active) {
				join[i] = Mathf.Lerp(join[i], -1000, dt * 5);
				portrait[i] = Mathf.Lerp(portrait[i], 0, dt * 5);
			}
		}
		flipTimer += dt;
		if (flipTimer >= 0.25f) {
			joinFlip = !joinFlip;
			flipTimer -= 0.25f;
		}
	}

	public override void Render ( ) {

		var middle = 179;

		UI.Rect(0, 0, 1000, 1000, Color.black);

		for (int i = 0; i < 4; i++) {
			var x = i * 158 + 19;
			UI.Rect(x, 0, 128, 1000, Color.black);
			UI.Image(3 + i, x, middle - 64 + portrait[i]);
			UI.Image(joinFlip ? 9 : 8, x + 32, middle - 32 + join[i]);
		}

		UI.Rect(0, 0, 1000, 120, Color.white);
		UI.Rect(0, 240, 1000, 120, Color.white);

		UI.Text("PRE-GAME", 0, 0, Color.green);

	}

}