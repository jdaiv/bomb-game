using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEngine;

public class OgmoLevel {

	public string title;

	public int width;
	public int height;

	public int[] tiles;
	public OgmoEntity[] entities;

	public OgmoLevel (string xmlString) {
		var xml = new XmlDocument();
		xml.LoadXml(xmlString);

		var root = xml.DocumentElement;

		width = int.Parse(root.GetAttribute("width")) / 16;
		height = int.Parse(root.GetAttribute("height")) / 16;

		var tilesString = root["Tiles"].InnerText;
		tilesString = Regex.Replace(tilesString, @"\r\n?|\n|,", "");
		tiles = new int[tilesString.Length];
		
		for (int i = 0; i < tilesString.Length; i++) {
			// flip map vertically
			var x = i % width;
			var y = height - (i / width) - 1;
			var index = y * width + x;
            tiles[index] = int.Parse(tilesString[i].ToString());
		}

		var ents = root["Entities"].ChildNodes;
		entities = new OgmoEntity[ents.Count];

		for (int i = 0; i < ents.Count; i++) {
			var ent = ents[i];
			var id = int.Parse(ent.Attributes["id"].Value);
			var x = int.Parse(ent.Attributes["x"].Value) / 16f;
			// flip vertically
			var y = height - int.Parse(ent.Attributes["y"].Value) / 16f;
			entities[id] = new OgmoEntity(id, ent.Name, new Vector2(x, y));

			foreach (XmlAttribute attr in ent.Attributes) {
				if (attr.Name != "id" && attr.Name != "y" && attr.Name != "y") {
					entities[id].data.Add(attr.Name, attr.Value);
				}
			}
		}
	}

}

public class OgmoEntity {

	public int id;
	public string type;
	public Vector2 position;
	public Dictionary<string, string> data;

	public OgmoEntity (int id, string type, Vector2 position) {
		this.id = id;
		this.type = type;
		this.position = position;
		data = new Dictionary<string, string>();
	}

}

public static class OgmoLoader {

	public static List<OgmoLevel> levels;

	public static void Load ( ) {
		levels = new List<OgmoLevel>();
		var files = Directory.GetFiles("Levels/Maps/");
		foreach (var f in files) {
			Debug.Log("loading level: " + f);
			var data = new OgmoLevel(File.ReadAllText(f));
			data.title = f;
			levels.Add(data);
		}

	}

}