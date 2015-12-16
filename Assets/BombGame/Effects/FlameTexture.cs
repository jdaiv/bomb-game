
using System.Collections;
using UnityEngine;

public class FlameTexture {

	int width, height;
	public Texture2D texture;

	public FlameTexture (int width, int height) {
		this.width = width;
		this.height = height;
		texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
		texture.filterMode = FilterMode.Bilinear;
		texture.wrapMode = TextureWrapMode.Clamp;
		for (int x = 0; x < texture.width; x++) {
			for (int y = texture.height; y >= 0; y--) {
				texture.SetPixel(x, y, Color.clear);
			}
		}
		texture.Apply();
	}

	public IEnumerator Tick ( ) {
		var black = new Color32(180, 0, 0, 255);
		var red = new Color32(255, 0, 0, 255);
		var orange = new Color32(255, 85, 0, 255);
		var yellow = new Color32(255, 170, 0, 255);

		int counter = 0;
		int delay = height / 3;
		var pixels = texture.GetPixels32();
		while (true) {
			for (int y = height - 1; y >= 0; y--) {
				for (int x = 1; x < width - 1; x++) {
					var i = x + y * width;
					if (y == 0) {
						pixels[i] = (U.RandomBool() ? U.RandomBool() ? U.RandomBool() ? orange : yellow : red : black);
					} else {
						var color1 = pixels[i - width];
						var color2 = pixels[i - width - 1];
						var color3 = pixels[i - width + 1];
						var color = new Color32(
							(byte)((color1.r + (color1.r / 1.6f) + color2.r + color3.r) / 4),
							(byte)((color1.g + (color1.g / 1.6f) + color2.g + color3.g) / 4),
							(byte)((color1.b + (color1.b / 1.6f) + color2.b + color3.b) / 4),
							(byte)((color1.a + (color1.a / 1.1f) + color2.a + color3.a) / 4)
                            );
						pixels[i] = color;
					}
				}
				counter++;
				if (counter >= delay) {
					counter = 0;
					yield return new WaitForSeconds(0.01f);
				}
			}
			counter = 0;
			texture.SetPixels32(pixels);
			texture.Apply();
		}
	}

}