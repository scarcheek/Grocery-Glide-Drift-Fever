using System;
using System.Diagnostics;
using System.Numerics;
using Godot;
using Godot.NativeInterop;
using System.Collections.Generic;


public partial class Cart : RigidBody3D
{
	private int thrust = 100; //thrust force
	private int angular = 20; //turning force
	[Export] public MeshInstance3D[] wheels;
	[Export] private int minDrift = 2, fixedTippingThreshold; //minDrift is the minimum of DriftV for driftScore to build up
	[Export] int weightMax = 100;
	public float driftV = 0;
	[Export] private int weight = 0;
	//private List<Item> items;
	private float driftScore = 0, tippingThreshold, driftBoost = 1, minBoost; //driftScore is accumulated through drifting, tippingThreshold is the minimum amount of DriftV that causes the cart to tip, minBoost is the minimum driftScore to get a driftBoost
	public bool isDrifting = false, ragdoll = false, prepUp = false;
	private Godot.Vector3 lastOrientation = new Godot.Vector3(0,0,0);
	[Export] PhysicsMaterial slipperyMaterial, ragdollMaterial;
	[Export] thirdPersonCam camera;

	public override void _Ready()
	{
		minBoost = thrust * 0.75f;
		fixedTippingThreshold = thrust / 11;
		tippingThreshold = fixedTippingThreshold;
	}

	public override void _Process(double delta)
	{
		float weightPenalty = ((3 - (weight / weightMax)) / 3f);

		if (!ragdoll) //steering is disabled while ragdolling
		{
			lastOrientation = Rotation;
			if (Input.IsActionPressed("forward"))
			{
				ApplyForce(GlobalTransform.Basis.Z * thrust * weightPenalty * driftBoost * (float)delta * 120);
			}
			if (Input.IsActionPressed("brake"))
			{
				Debug.WriteLine("brake");
				ApplyForce(-GlobalTransform.Basis.Z * thrust * weightPenalty * driftBoost * (float)delta * 75);
			}
			if (Input.IsActionPressed("steer_r"))
			{
				ApplyTorque(GlobalTransform.Basis.Y * -angular * weightPenalty * (float)delta * 75);
			}
			else if (Input.IsActionPressed("steer_l"))
			{
				ApplyTorque(GlobalTransform.Basis.Y * angular * weightPenalty * (float)delta * 75);
			}

		}
		else
		{
			if (prepUp){
				makeUpright(delta);
			}
			if (LinearVelocity.Length() < 0.02f && GlobalTransform.Basis.Y.Normalized().Dot(new Godot.Vector3(0, 1, 0)) > 0.999f)
			{  //detect if landed upright and reset 
				Debug.WriteLine("is upright");
				activateNormal();
			}
		}

		if (Input.IsActionJustPressed("ui_cancel"))
		{
			GetTree().Quit();
		}

		if (Input.IsKeyLabelPressed(Key.R)){
			Freeze = true;
			prepUp = true;
		}


		//interpolate tipping threshold and drift boost to normal value
		tippingThreshold = (float)(tippingThreshold + (fixedTippingThreshold - tippingThreshold) * 0.4 * delta);
		driftBoost = (float)(driftBoost + (1 - driftBoost) * 0.4 * delta);

		//check all the shit
		driftV = driftValue();
		checkDrifting();
		check_tipping();
		addDriftScore(delta, weightPenalty);
	}

	private float driftValue()
	{
		return (float)Godot.Mathf.Abs((LinearVelocity.Dot(GlobalTransform.Basis.X.Normalized()) * ((1 + (weight / weightMax)) / 2f)));
	}

	private void check_tipping()
	{
		if (driftV > tippingThreshold)
		{
			activateRagdoll();
		}
	}

	private void checkDrifting()
	{
		if (!ragdoll && driftV > minDrift)
		{
			isDrifting = true;
		}
		else
		{
			isDrifting = false;
		}
	}

	private void addDriftScore(double delta, float weightPenalty)
	{
		if (isDrifting)
		{
			driftScore += (float)(driftV * delta * 10 * weightPenalty);
		}
		else
		{
			if (driftScore > minBoost)
			{
				Debug.WriteLine("boosted");
				driftBoost += Mathf.Min(driftScore / 150, 1);
				tippingThreshold += Mathf.Min(driftScore / 250, 5);
			}
			driftScore = 0;
		}
	}


	private void activateRagdoll()
	{
		ragdoll = true;
		PhysicsMaterialOverride = ragdollMaterial;
		camera.setRagdoll(true);
	}

	private void activateNormal()
	{
		Freeze = false;
		prepUp = false;
		ragdoll = false;
		camera.setRagdoll(false);
		PhysicsMaterialOverride = slipperyMaterial;
	}

	private void makeUpright(double delta)
	{
		Position = Position + (new Godot.Vector3(Position.X, 2, Position.Z)-Position) * (float)(10 * delta);
		Rotation = Rotation + (lastOrientation - Rotation) * (float)(5 * delta);
	}
}
