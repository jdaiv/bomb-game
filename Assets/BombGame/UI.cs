using UnityEngine;

public static class UI {

	public static void Image (int image, float x, float y) {
		var tex = G.I.uiSprites[image];
		var _x = Mathf.RoundToInt(x);
		var _y = Mathf.RoundToInt(y + tex.height);
		Graphics.DrawTexture(new Rect(_x, _y, tex.width, -tex.height), tex);
	}

	public static void Image (int image, float x, float y, Color color) {
		var tex = G.I.uiSprites[image];
		var _x = Mathf.RoundToInt(x);
		var _y = Mathf.RoundToInt(y + tex.height);
		Graphics.DrawTexture(new Rect(_x, _y, tex.width, -tex.height), tex,
			new Rect(0, 0, 1, 1), 0, 0, 0, 0, color);
	}

	public static void Texture (Texture2D tex, float x, float y, float scaleX = 1, float scaleY = 1, bool flipV = false, bool flipH = false) {
		var _x = Mathf.RoundToInt(x);
		if (flipV) {
			var _y = Mathf.RoundToInt(y);
			Graphics.DrawTexture(new Rect(_x, _y, tex.width * scaleX, tex.height * scaleY), tex);
		} else {
			var _y = Mathf.RoundToInt(y + tex.height * scaleY);
			Graphics.DrawTexture(new Rect(_x, _y, tex.width * scaleX, -tex.height * scaleY), tex);
		}
	}

	public static void Rect (float x, float y, int width, int height, Color color) {
		var tex = G.I.uiSprites[0];
		var _x = Mathf.RoundToInt(x);
		var _y = Mathf.RoundToInt(y + height);
		Graphics.DrawTexture(new Rect(_x, _y, width, -height), tex,
			new Rect(0, 0, 4, 4), 0, 0, 0, 0, color);
	}

	public static void Number (float x, float y, int number, Color color) {
		var tex = G.I.uiSprites[1];
		var _x = Mathf.RoundToInt(x);
		var _y = Mathf.RoundToInt(y + tex.height);
		Graphics.DrawTexture(new Rect(_x, _y, S.SIZE, -S.SIZE), tex,
			new Rect(0.1f * number, 0, 0.1f, 1), 0, 0, 0, 0, color);
	}

	public static void Text (string str, float x, float y, Color color) {
		var tex = G.I.uiSprites[2];
		var count = 0;
		var _x = Mathf.RoundToInt(x);
		var _y = Mathf.RoundToInt(y + 9);
		foreach (var chr in str) {
			if (chr <= 0x7E && chr >= 0x21) {
				var ascii = (byte)chr - 0x21;
				Graphics.DrawTexture(new Rect(_x + 9 * count, _y, 9, -9), tex,
					new Rect((ascii / 95f), 0, (1 / 95f), 1), 0, 0, 0, 0, color);
			}
			count++;
		}
	}

	public static void TextOutline (string str, float x, float y, Color color, Color outlineColor, int outlineWidth = 1) {
		Text(str, x + outlineWidth, y, outlineColor);
		Text(str, x, y + outlineWidth, outlineColor);
		Text(str, x - outlineWidth, y, outlineColor);
		Text(str, x, y - outlineWidth, outlineColor);
		Text(str, x - outlineWidth, y + outlineWidth, outlineColor);
		Text(str, x + outlineWidth, y - outlineWidth, outlineColor);
		Text(str, x + outlineWidth, y + outlineWidth, outlineColor);
		Text(str, x - outlineWidth, y - outlineWidth, outlineColor);
		Text(str, x, y, color);
	}

}
