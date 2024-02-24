using Godot;
using System;
using System.Diagnostics;

public partial class Wheel : MeshInstance3D
{

	private Cart cart;
	private Vector3 velocityDir, velocity2D;
	private GpuParticles3D smoke,sparks;

	public float driftAngleThresh = 0.7f;
	public float driftVelocityThresh = 7f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		cart = GetParent<Cart>();
		smoke = GetChild<GpuParticles3D>(0);
		sparks = GetChild<GpuParticles3D>(1);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		velocity2D = new Vector3(cart.LinearVelocity.X, 0, cart.LinearVelocity.Z);
		velocityDir = GlobalPosition + velocity2D;
		LookInVelocityDirection();

		
		if (cart.isDrifting){
			smoke.Emitting = true;
			sparks.Emitting =true;
		}else{
			smoke.Emitting = false;
			sparks.Emitting =false;
		}
		
	}

	private void LookInVelocityDirection()
	{
		if (velocity2D.Length() > 0.1)
		{
				LookAt(velocityDir);
				Rotation = new Vector3(0,Rotation.Y,0);
		}
	}
}
	/*
		float angleToCart = GlobalTransform.Basis.Z.AngleTo(cart.GlobalTransform.Basis.Z);
		if (cart.LinearVelocity.Dot(cart.GlobalTransform.Basis.X.Normalized()) > driftVelocityThresh && angleToCart < Math.PI - driftAngleThresh && angleToCart > driftAngleThresh) {
	*/

