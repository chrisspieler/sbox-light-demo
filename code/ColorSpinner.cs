using System;

namespace LightDemo;

public sealed class ColorSpinner : Component
{
	[Property] public float Radius { get; set; } = 20f;
	[Property] public float RPM { get; set; } = 60f;
	[Property] public float LerpSpeed { get; set; } = 3f;
	[Property] public int LightCount 
	{
		get => _lightCount;
		set
		{
			_lightCount = Math.Max( value, 0 );
			if ( Active && Game.IsPlaying )
			{
				CreateLights();
			}
		}
	}
	private int _lightCount = 3;
	[Property] public bool ShadowsEnabled { get; set; } = false;
	[Property] public float ConeOuter { get; set; } = 10f;
	[Property] public float MaxDistance { get; set; } = 500f;
	[Property] public float Attenuation { get; set; } = 1.0f;

	private List<SceneSpotLight> _lights = new();

	protected override void OnEnabled()
	{
		CreateLights();
	}

	protected override void OnDisabled()
	{
		DestroyLights();
	}

	private void DestroyLights()
	{
		foreach( var light in _lights )
		{
			light.Delete();
		}
		_lights.Clear();
	}

	private void CreateLights()
	{
		DestroyLights();
		for (int i = 0; i < LightCount; i++)
		{
			_lights.Add( new SceneSpotLight( Scene.SceneWorld, Transform.Position, Color.White ) );
		}
	}

	protected override void OnUpdate()
	{
		if ( !_lights.Any() )
			return;

		var worldPos = Transform.Position;
		for ( int i = 0; i < LightCount; i++ )
		{
			var angle = 360f / _lights.Count * i;
			var light = _lights[i];
			UpdateLightPosition( light, angle );
			light.LightColor = new ColorHsv( angle, 1f, 1f );
			light.ShadowsEnabled = ShadowsEnabled;
			light.ConeOuter = ConeOuter;
			light.ConeInner = ConeOuter * 0.8f;
			light.Radius = MaxDistance;
			light.QuadraticAttenuation = Attenuation;
		}
	}

	private void UpdateLightPosition( SceneSpotLight light, float angle )
	{
		angle += RPM * Time.Now;
		angle %= 360f;
		var currentPos = light.Position;
		var targetY = Radius * MathF.Cos( MathF.Tau * angle / 360f );
		var targetZ = Radius * MathF.Sin( MathF.Tau * angle / 360f );
		var targetPos = Transform.World.PointToWorld( new Vector3( 0f, targetY, targetZ ) );
		light.Position = currentPos.LerpTo( targetPos, Time.Delta * LerpSpeed );
		light.Rotation = Transform.Rotation;
	}

	protected override void DrawGizmos()
	{
		foreach( var light in _lights )
		{
			Gizmo.Draw.Color = light.LightColor;
			Gizmo.Draw.LineSphere( Transform.World.PointToLocal( light.Position ), 2f );
		}
	}
}
