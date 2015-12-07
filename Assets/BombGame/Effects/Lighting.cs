
using System.Collections.Generic;
using UnityEngine;

public class PointLight {

	public Vector2 position;
	public Color color;
	public float range;

}

public class Lighting {

	public List<PointLight> lights;

	public Lighting () {
		lights = new List<PointLight>(20);
		Add(new Vector2(2, 2), Color.white, 4);
		Add(new Vector2(17, 2), Color.white, 4);
		Add(new Vector2(17, 17), Color.white, 4);
		Add(new Vector2(2, 17), Color.white, 4);
	}

	public void Add (Vector2 position, Color color, float range) {
		var light = new PointLight();
		light.position = position;
		light.color = color;
		light.range = range;
		lights.Add(light);
	}

}
