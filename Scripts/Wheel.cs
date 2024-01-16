using Godot;
using System;

public partial class Wheel : MeshInstance3D
{

	private Cart cart;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		cart = GetParent<Cart>();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector3 lookAtPos = GlobalPosition + new Vector3(cart.LinearVelocity.X, 0, cart.LinearVelocity.Z);
		if (cart.LinearVelocity.Length() > 0.1 && GlobalPosition != lookAtPos)
		{
			LookAt(lookAtPos);
		}
	}
}
