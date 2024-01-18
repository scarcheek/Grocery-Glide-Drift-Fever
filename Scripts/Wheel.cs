using Godot;
using System;
using System.Diagnostics;

public partial class Wheel : MeshInstance3D
{

	private Cart cart;
	private Vector3 velocityDir;

	public float driftAngleThresh = 0.7f;
	public float driftVelocityThresh = 7f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		cart = GetParent<Cart>();
		velocityDir = GlobalPosition + new Vector3(cart.LinearVelocity.X, 0, cart.LinearVelocity.Z);
		Debug.WriteLine(GetSurfaceOverrideMaterial(0)._Get("Albedo"));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		velocityDir = GlobalPosition + new Vector3(cart.LinearVelocity.X, 0, cart.LinearVelocity.Z);
		LookInVelocityDirection();
		CheckForDrift();
	}

	private void LookInVelocityDirection()
	{
		if (cart.LinearVelocity.Length() > 0.1 && GlobalPosition != velocityDir)
		{
			LookAt(velocityDir);
		}
	}

	private void CheckForDrift()
	{
		float angleToCart = GlobalTransform.Basis.Z.AngleTo(cart.GlobalTransform.Basis.Z);
		if (cart.LinearVelocity.Dot(cart.GlobalTransform.Basis.X.Normalized()) > driftVelocityThresh && angleToCart < Math.PI - driftAngleThresh && angleToCart > driftAngleThresh) {
			// Debug.WriteLine("Drift engaged");
		}

	}

}
