namespace WATIGA.Common.Metaballs;

public struct Metaball()
{
	public Vector2 Position = Vector2.Zero;
	public Vector2 Velocity = Vector2.Zero;
	public float Rotation = 0f;
	public float Scale = 1f;
	public Color Color = Color.White;
	public bool Active = false;
	public object CustomData = null;
}
