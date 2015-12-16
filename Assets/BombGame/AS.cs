using System;
using UnityEngine;

public class AS : S {

	public const int FRAME_DURATION = 3;

	public Sprite[] frames;
	
	public int frame;
	public FrameTimer timer;
	public bool loop;
	public int returnTo;
	public int end;

	public AS (Sprite[] frames) {
		this.frames = frames;
		returnTo = frames.Length - 1;
		timer = new FrameTimer(FRAME_DURATION, true);
	}

	public override void Update ( ) {

		timer.Tick();
		if (timer.done) {
			frame++;
			if (frame >= end) {
				if (loop) {
					frame = 0;
				} else {
					frame = returnTo;
					timer.Stop();
				}
			}
			UpdateFrame();
		}

		base.Update();

	}

	public void UpdateFrame () {
		renderer.sprite = frames[frame];
	}

	public void Play (int from = 0, int to = -1) {
		timer.Reset();
		timer.Start();

		frame = from;

		if (to >= 0) {
			end = to + 1;
		} else {
			end = frames.Length;
		}
		UpdateFrame();
	}

	public void GoTo (int frame) {
		this.frame = frame;
		timer.Reset();
		UpdateFrame();
	}

	public void Stop ( ) {
		timer.Stop();
		frame = 0;
		UpdateFrame();
	}

	public void Pause ( ) {
		timer.Pause();
	}

	public void Resume ( ) {
		timer.Start();
	}

}