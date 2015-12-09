

using UnityEngine;

public class Doors {

	float door1Pos;
	float door2Pos;
	float door1Vel;
	float door2Vel;
	float gotoPos;
	bool gravity;

	public Doors ( ) {
		door1Pos = 400;
		door2Pos = 400;
		door1Vel = 0;
		door2Vel = 0;
		gotoPos = 0;
		gravity = true;
	}

	public void Update (float dt) {
		if (gravity) {
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
		} else {
			door1Pos = Mathf.Lerp(door1Pos, gotoPos, dt * 5);
			door2Pos = Mathf.Lerp(door2Pos, gotoPos, dt * 5);
		}
	}

	public void Activate ( ) {
		gravity = true;
	}

	public void Goto (float pos) {
		gravity = false;
		door1Vel = 0;
		door2Vel = 0;
		gotoPos = pos;
	}

	public void Force (float pos) {
		gravity = false;
		gotoPos = pos;
		door1Pos = pos;
		door2Pos = pos;
		door1Vel = 0;
		door2Vel = 0;
	}

	public void Render ( ) {
		var tex = G.I.uiSprites[12];
		UI.Texture(tex, 0 - door1Pos, 0);
		UI.Texture(tex, 640 + door2Pos, 0, -1, 1);
	}

}