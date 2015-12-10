
using System.Collections;

public abstract class GameState {

	public bool updateEntities;
	public bool updateSprites;

	public abstract IEnumerator Start ( );

	public abstract IEnumerator End ( );

	public abstract void Update (float dt);

	public abstract void Render ( );

}
