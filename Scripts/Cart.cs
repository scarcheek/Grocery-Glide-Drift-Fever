using System;
using System.Diagnostics;
using System.Numerics;
using Godot;


public partial class Cart : RigidBody3D
{
	[Export] public MeshInstance3D[] wheels;
	[Export] int tippingThreshold = 11;

	[Export] PhysicsMaterial slipperyMaterial, ragdollMaterial;

	[Export] int weightMax = 100;
	private int thrust = 200;
	private int angular = 40;
	private int weight = 100;
	private bool ragdoll = false;
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time Math.Since the previous frame.
	public override void _Process(double delta)
	{
		float weightPenalty = ((2 - (weight / weightMax)) / 2f);

		if (!ragdoll)
		{
			if (Input.IsActionPressed("forward"))
			{
				ApplyForce(GlobalTransform.Basis.Z * thrust * weightPenalty * (float)delta*100);
			}
			if (Input.IsActionPressed("steer_r"))
			{
				ApplyTorque(GlobalTransform.Basis.Y * -angular * weightPenalty * (float)delta*100);
			}
			else if (Input.IsActionPressed("steer_l"))
			{
				ApplyTorque(GlobalTransform.Basis.Y * angular * weightPenalty * (float)delta*100);
			}
		}

		if (checkTipping())
		{
			activateRagdoll();
		}
	}

	private bool checkTipping()
	{
		Debug.WriteLine(LinearVelocity.Dot(GlobalTransform.Basis.X.Normalized()) * ((1 + (weight / weightMax)) / 2f));

		if (Godot.Mathf.Abs(LinearVelocity.Dot(GlobalTransform.Basis.X.Normalized()) * ((1 + (weight / weightMax)) / 2f)) > tippingThreshold)
		{
			return true;
		}
		return false;
	}

	private void activateRagdoll()
	{
		AxisLockAngularX = false;
		AxisLockAngularZ = false;
		PhysicsMaterialOverride = ragdollMaterial;
	}

	private void activateNormal()
	{
		//readjust position
		AxisLockAngularX = true;
		AxisLockAngularZ = true;
		PhysicsMaterialOverride = slipperyMaterial;
	}
}
