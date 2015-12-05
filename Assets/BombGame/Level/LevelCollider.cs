
using UnityEngine;

class LevelCollider {

	const int RES = 2;
	const int THRESHOLD = 1;

	bool pristine;
	bool dirty;

	GameObject go;
	BoxCollider2D mainCollider;
	BoxCollider2D[,] colliders;
	bool[,] colliderExists;
	float offset;
	int res;

	bool[,] mask;

	public LevelCollider (float x, float y, Transform parent) {
		go = new GameObject("Level Collider " + x + ", " + y);
		go.transform.position = new Vector3(x, y);
		go.transform.parent = parent;
		mainCollider = go.AddComponent<BoxCollider2D>();
		mainCollider.size = new Vector2(1, 1);
		mainCollider.offset = new Vector2(0.5f, 0.5f);
		var size = S.SIZE;
		offset = (float)RES / S.SIZE;
		res = S.SIZE / RES;
		mask = new bool[size, size];
		for (int _x = 0; _x < size; _x++) {
			for (int _y = 0; _y < size; _y++) {
				mask[_x, _y] = true;
			}
		}
		pristine = true;
		dirty = false;
		colliders = new BoxCollider2D[res, res];
		colliderExists = new bool[res, res];
		for (int _x = 0; _x < res; _x++) {
			for (int _y = 0; _y < res; _y++) {
				colliderExists[_x, _y] = false;
			}
		}
	}

	public void Destroy ( ) {
		Object.Destroy(go);
	}

	public void ClearBit (int x, int y) {
		dirty = true;
		mask[x, y] = false;
	}

	public void Update () {
		if (dirty) {
			if (pristine) {
				pristine = false;
				mainCollider.enabled = false;
			}
			for (int x = 0; x < res; x++) {
				for (int y = 0; y < res; y++) {

					int count = 0;
					for (int _x = 0; _x < RES; _x++) {
						for (int _y = 0; _y < RES; _y++) {
							count += mask[x * RES + _x, y * RES + _y] ? 1 : 0;
						}
					}

					var collision = count > THRESHOLD;

					if (collision) {
						if (!colliderExists[x, y]) {
							var c = go.AddComponent<BoxCollider2D>();
							c.size = new Vector2(offset, offset);
							c.offset = new Vector2(offset * x, offset * y);
							colliders[x, y] = c;
							colliderExists[x, y] = true;
						}
					} else {
						if (colliderExists[x, y]) {
							colliders[x, y].enabled = false;
							colliderExists[x, y] = false;
						}
					}

				}
			}
		}
	}

}
