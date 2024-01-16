using System;
using System.Diagnostics;
using System.Numerics;
using Godot;


public partial class Cart : RigidBody3D
{
	[Export] public MeshInstance3D[] wheels;
	private int thrust = 1000;
	private int angular = 200;
	private int weight = 0;
	public override void _Ready()
	{
		AxisLockAngularX = true;
		AxisLockAngularZ = true;
	}

	// Called every frame. 'delta' is the elapsed time Math.Since the previous frame.
	public override void _Process(double delta)
	{
		float weightPenalty = 1-weight/100;
		if(Input.IsActionPressed("forward")){
			ApplyForce(GlobalTransform.Basis.Z*thrust*weightPenalty, CenterOfMass);
			Debug.WriteLine("thrust");
		}
		if (Input.IsActionPressed("steer_r")){
			ApplyTorque(GlobalTransform.Basis.Y*-angular*weightPenalty);
		}
		else if (Input.IsActionPressed("steer_l")){
			ApplyTorque(GlobalTransform.Basis.Y*angular*weightPenalty);
		}


		foreach (MeshInstance3D wheel in wheels){
			if (LinearVelocity.Length() > 0.1){
				wheel.LookAt(wheel.GlobalPosition + new Godot.Vector3(LinearVelocity.X, 0, LinearVelocity.Z));
			}
		}
	}
}
