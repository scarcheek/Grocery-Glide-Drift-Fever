using Godot;
using System;

public partial class thirdPersonCam : Camera3D
{
	[Export] public RigidBody3D Cart;
	[Export] private float dist, height;
	private bool ragdoll = false;

	public override void _Ready()
	{
	}

	public void setRagdoll(bool ragdoll){
		this.ragdoll = ragdoll;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if (!ragdoll){
		Godot.Vector3 CartDir = -Cart.GlobalTransform.Basis.Z;
		Godot.Vector3 globalCamreaPos = Cart.Position + new Godot.Vector3(CartDir.X*dist,height,CartDir.Z*dist);
		Godot.Vector3 cameraMovement= globalCamreaPos - Position;
		Position = Position + (cameraMovement.Normalized()*(float)(cameraMovement.Length()*10*delta));
		//Position = globalCamreaPos;
		LookAt(Cart.Position + new Godot.Vector3(0,0.3f,0));
		}
		else{
			Vector3 direction = Cart.Position - Position;
			if(MathF.Abs(direction.Length()) > 2){
				Position = Position + direction.Normalized()*(float)(direction.Length()*delta);
				
			}
			LookAt(Cart.Position + new Godot.Vector3(0,0.5f,0));
		}
	}
}
