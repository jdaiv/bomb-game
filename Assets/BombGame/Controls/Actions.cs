using System;
using InControl;
using UnityEngine;

public class Actions : PlayerActionSet {
	public PlayerAction Fire;
	public PlayerAction Throw;
	public PlayerAction Start;
	public PlayerAction Left;
	public PlayerAction Right;
	public PlayerAction Up;
	public PlayerAction Down;
	public PlayerTwoAxisAction Move;


	public Actions ( ) {
		Fire = CreatePlayerAction("Fire");
		Throw = CreatePlayerAction("Throw");
		Start = CreatePlayerAction("Start Game");
		Left = CreatePlayerAction("Move Left");
		Right = CreatePlayerAction("Move Right");
		Up = CreatePlayerAction("Move Up");
		Down = CreatePlayerAction("Move Down");
		Move = CreateTwoAxisPlayerAction(Left, Right, Down, Up);
	}

	public static Actions CreateWithDefaultBindings (bool keyboard = false) {
		var actions = new Actions();

		if (keyboard) {

			actions.Fire.AddDefaultBinding(Key.Z);
			actions.Throw.AddDefaultBinding(Key.X);
			actions.Start.AddDefaultBinding(Key.Return);

			actions.Left.AddDefaultBinding(Key.LeftArrow);
			actions.Right.AddDefaultBinding(Key.RightArrow);
			actions.Up.AddDefaultBinding(Key.UpArrow);
			actions.Down.AddDefaultBinding(Key.DownArrow);

		} else {

			actions.Fire.AddDefaultBinding(InputControlType.Action1);
			actions.Fire.AddDefaultBinding(InputControlType.Action2);
			actions.Throw.AddDefaultBinding(InputControlType.Action3);
			actions.Throw.AddDefaultBinding(InputControlType.Action4);
			actions.Start.AddDefaultBinding(InputControlType.Start);

			actions.Left.AddDefaultBinding(InputControlType.LeftStickLeft);
			actions.Right.AddDefaultBinding(InputControlType.LeftStickRight);
			actions.Up.AddDefaultBinding(InputControlType.LeftStickUp);
			actions.Down.AddDefaultBinding(InputControlType.LeftStickDown);
			actions.Up.AddDefaultBinding(InputControlType.Analog4);
			actions.Down.AddDefaultBinding(InputControlType.Analog4);

			actions.Left.AddDefaultBinding(InputControlType.DPadLeft);
			actions.Right.AddDefaultBinding(InputControlType.DPadRight);
			actions.Up.AddDefaultBinding(InputControlType.DPadUp);
			actions.Down.AddDefaultBinding(InputControlType.DPadDown);

		}

		return actions;
	}
}