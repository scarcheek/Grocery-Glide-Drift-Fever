using Godot;
using System;
using System.Diagnostics;

public partial class Wheel : MeshInstance3D
{

	private Cart cart;
	private Vector3 velocityDir;
	private GpuParticles3D smoke;

	public float driftAngleThresh = 0.7f;
	public float driftVelocityThresh = 7f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		cart = GetParent<Cart>();
		smoke = GetChild<GpuParticles3D>(0);
		velocityDir = GlobalPosition + new Vector3(cart.LinearVelocity.X, 0, cart.LinearVelocity.Z);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		velocityDir = GlobalPosition + new Vector3(cart.LinearVelocity.X, 0, cart.LinearVelocity.Z);
		LookInVelocityDirection();

		
		if (cart.isDrifting){
			smoke.Emitting = true;
		}else{
			smoke.Emitting = false;
		}
		
	}

	private void LookInVelocityDirection()
	{
		if (cart.LinearVelocity.Length() > 0.1 && GlobalPosition != velocityDir)
		{
			LookAt(velocityDir);
		}
	}
}
	/*
		float angleToCart = GlobalTransform.Basis.Z.AngleTo(cart.GlobalTransform.Basis.Z);
		if (cart.LinearVelocity.Dot(cart.GlobalTransform.Basis.X.Normalized()) > driftVelocityThresh && angleToCart < Math.PI - driftAngleThresh && angleToCart > driftAngleThresh) {
	*/

