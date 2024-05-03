using Sandbox;

public sealed class LightWander : Component
{
	[Property] public BBox Bounds { get; set; } = BBox.FromPositionAndSize( Vector3.Zero, 300f );

	private Dictionary<SpotLight, Vector3> _targetPositions = new();
	private Dictionary<SpotLight, float> _targetCone = new();

	protected override void OnUpdate()
	{
		var spotLights = Components.GetAll<SpotLight>( FindMode.InChildren );
		foreach( var spotLight in spotLights )
		{
			UpdateLight( spotLight );
		}
	}

	private void UpdateLight( SpotLight light )
	{
		if ( !_targetPositions.ContainsKey( light ) )
		{
			ResetLight( light );
		}

		var targetPos = _targetPositions[light];
		light.Transform.Position = light.Transform.Position.LerpTo( targetPos, Time.Delta );
		light.ConeOuter = light.ConeOuter.LerpTo( _targetCone[light], Time.Delta * 3f );

		if ( light.Transform.Position.Distance( targetPos ) < 10f )
		{
			ResetLight( light );
		}
	}

	private void ResetLight( SpotLight light )
	{
		_targetCone[light] = Game.Random.Float( 1.5f, 9f );
		_targetPositions[light] = Transform.World.PointToWorld( Bounds.RandomPointInside );
	}

	protected override void DrawGizmos()
	{
		Gizmo.Draw.LineBBox( Bounds );
	}
}
