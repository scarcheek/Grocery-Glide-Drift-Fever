using Godot;
using System;

public partial class thirdPersonCam : Camera3D
{
	// Called when the node enters the scene tree for the first time.
	[Export] public RigidBody3D Cart;
	[Export] private float dist, height;

	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Godot.Vector3 CartDir = -Cart.GlobalTransform.Basis.Z;
		Godot.Vector3 globalCamreaPos = Cart.Position + new Godot.Vector3(CartDir.X*dist,height,CartDir.Z*dist);
		Godot.Vector3 cameraMovement= globalCamreaPos - Position;
		Position = Position + (cameraMovement.Normalized()*(float)(cameraMovement.Length()*10*delta));
		//Position = globalCamreaPos;
		LookAt(Cart.Position + new Godot.Vector3(0,0.5f,0));
	}
}
