
public abstract class GameState {

	public bool updateEntities;

	public abstract void Start ( );

	public abstract void Update (float dt);

	public abstract void End ( );

	public abstract void Render ( );

}
