using Godot;
using System;

public partial class BaseCart : VehicleBody3D
{
	private int acc = 10;
	private int deceleration = 10;
	private float steerAngle = 0.3f;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Input.IsActionPressed("forward")){
			EngineForce = acc;
		}
		else if (Input.IsActionPressed("brake")){
			Brake = deceleration;
		}

		if (Input.IsActionPressed("steer_r")){
			Steering = -steerAngle;
		}
		else if (Input.IsActionPressed("steer_l")){
			Steering = steerAngle;
		}
		else{
			Steering = 0;
		}
	}
}
