using System;
using InControl;
using UnityEngine;

public class Actions : PlayerActionSet {
	public PlayerAction FireUp;
	public PlayerAction FireDown;
	public PlayerAction FireLeft;
	public PlayerAction FireRight;
	public PlayerAction Throw;
	public PlayerAction Start;
	public PlayerAction Left;
	public PlayerAction Right;
	public PlayerAction Up;
	public PlayerAction Down;
	public PlayerTwoAxisAction Fire;
	public PlayerTwoAxisAction Move;


	public Actions ( ) {
		FireUp = CreatePlayerAction("Fire Up");
		FireDown = CreatePlayerAction("Fire Down");
		FireLeft = CreatePlayerAction("Fire Left");
		FireRight = CreatePlayerAction("Fire Right");
		Throw = CreatePlayerAction("Throw");
		Start = CreatePlayerAction("Start Game");
		Left = CreatePlayerAction("Move Left");
		Right = CreatePlayerAction("Move Right");
		Up = CreatePlayerAction("Move Up");
		Down = CreatePlayerAction("Move Down");
		Move = CreateTwoAxisPlayerAction(Left, Right, Down, Up);
		Fire = CreateTwoAxisPlayerAction(FireLeft, FireRight, FireDown, FireUp);
	}

	public static Actions CreateWithDefaultBindings (bool keyboard = false) {
		var actions = new Actions();

		if (keyboard) {

			actions.FireLeft.AddDefaultBinding(Key.LeftArrow);
			actions.FireRight.AddDefaultBinding(Key.RightArrow);
			actions.FireUp.AddDefaultBinding(Key.UpArrow);
			actions.FireDown.AddDefaultBinding(Key.DownArrow);
			actions.Throw.AddDefaultBinding(Key.X);
			actions.Start.AddDefaultBinding(Key.Return);

			actions.Left.AddDefaultBinding(Key.A);
			actions.Right.AddDefaultBinding(Key.D);
			actions.Up.AddDefaultBinding(Key.W);
			actions.Down.AddDefaultBinding(Key.S);

		} else {

			actions.FireUp.AddDefaultBinding(InputControlType.Action4);
			actions.FireDown.AddDefaultBinding(InputControlType.Action2);
			actions.FireLeft.AddDefaultBinding(InputControlType.Action3);
			actions.FireRight.AddDefaultBinding(InputControlType.Action1);
			actions.Throw.AddDefaultBinding(InputControlType.LeftBumper);
			actions.Throw.AddDefaultBinding(InputControlType.RightBumper);
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