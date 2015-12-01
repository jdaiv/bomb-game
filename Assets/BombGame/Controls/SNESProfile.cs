using System;

namespace InControl {
	[AutoDiscover]
	public class SNESProfile : UnityInputDeviceProfile {
		public SNESProfile ( ) {
			Name = "SNES Controller";
			Meta = "Some junk-ass controller off ebay.";

			SupportedPlatforms = new[] {
				"Windows"
			};

			JoystickRegex = new[] {
				"USB Gamepad",
				"USB Gamepad ",
			};

			MinUnityVersion = new VersionInfo(5, 0, 0, 0);

			ButtonMappings = new[] {
				new InputControlMapping {
					Handle = "X",
					Target = InputControlType.Action1,
					Source = Button0
				},
				new InputControlMapping {
					Handle = "A",
					Target = InputControlType.Action2,
					Source = Button1
				},
				new InputControlMapping {
					Handle = "B",
					Target = InputControlType.Action3,
					Source = Button2
				},
				new InputControlMapping {
					Handle = "Y",
					Target = InputControlType.Action4,
					Source = Button3
				},
				new InputControlMapping {
					Handle = "Left Bumper",
					Target = InputControlType.LeftBumper,
					Source = Button4
				},
				new InputControlMapping {
					Handle = "Right Bumper",
					Target = InputControlType.RightBumper,
					Source = Button5
				},
				new InputControlMapping {
					Handle = "Back",
					Target = InputControlType.Back,
					Source = Button8
				},
				new InputControlMapping {
					Handle = "Start",
					Target = InputControlType.Start,
					Source = Button9
				},
			};

			AnalogMappings = new[] {
				DPadLeftMapping( Analog0 ),
				DPadRightMapping( Analog0 ),
				DPadUpMapping( Analog4 ),
				DPadDownMapping( Analog4 ),
			};
		}
	}

}

