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
	[Export] private MeshInstance3D[] wheels;
	
	[Export] private GpuParticles3D sparkles;
	[Export] private PhysicsMaterial slipperyMaterial, ragdollMaterial;
	[Export] thirdPersonCam camera;
	[Export] int weightMax = 100;
	[Export] private int weight = 0;
	public float driftV = 0;
	//private List<Item> items;
	//driftScore is accumulated through drifting, tippingThreshold is the minimum amount of driftV that causes the cart to tip, minBoost is the minimum driftScore to get a driftBoost, minDrift is the minimum of DriftV for driftScore to build up
	private float driftScore = 0, tippingThreshold, driftBoost = 1, minBoost, fixedTippingThreshold, minDrift = 2;
	public bool isDrifting = false, ragdoll = false, prepUp = false, boostReady = false;
	private Godot.Vector3 lastOrientation = new Godot.Vector3(0,0,0);
	

	public override void _Ready()
	{
		minBoost = thrust * 0.5f;
		fixedTippingThreshold = thrust / 18f;
		tippingThreshold = fixedTippingThreshold;
		Debug.WriteLine(fixedTippingThreshold);
	}

	public override void _Process(double delta)
	{
		float weightPenalty = ((3.5f - (weight / weightMax)) / 3.5f);

		if (!ragdoll) //steering is disabled while ragdolling
		{
			lastOrientation = Rotation;
			if (Input.IsActionPressed("forward"))
			{
				ApplyForce(GlobalTransform.Basis.Z * thrust * weightPenalty * driftBoost * (float)delta * 90);
			}
			
			if (Input.IsActionPressed("brake"))
			{
				ApplyForce(-GlobalTransform.Basis.Z * thrust  * (float)delta * 45);
			}
			if (Input.IsActionPressed("steer_r"))
			{
				ApplyTorque(GlobalTransform.Basis.Y * -angular * (float)delta * 70);
			}
			else if (Input.IsActionPressed("steer_l"))
			{
				ApplyTorque(GlobalTransform.Basis.Y * angular * (float)delta * 70);
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
			if (ragdoll){
				Freeze = true;
				prepUp = true;
			}
		}


		//interpolate tipping threshold and drift boost to normal value
		tippingThreshold = (float)(tippingThreshold + (fixedTippingThreshold - tippingThreshold) * 0.2 * delta);
		driftBoost = (float)(driftBoost + (1 - driftBoost) * 0.7 * delta);

		//check all the shit
		driftV = driftValue();
		Debug.WriteLine(isDrifting + " / " + boostReady + " / " + String.Format("{0:0.00}", driftScore));
		checkDrifting();
		check_tipping();
		addDriftScore(delta, weightPenalty);
		handleSparkles();
	}

	private void handleSparkles(){
		if(Input.IsActionPressed("forward")){
			if (driftBoost > 1.1){
				sparkles.Emitting = true;
			}
			else{
				sparkles.Emitting =false;
			}
		}
		else
			{
				sparkles.Emitting = false;
			}
	}
	private float driftValue()
	{
		return (float)Godot.Mathf.Abs((LinearVelocity.Dot(GlobalTransform.Basis.X.Normalized()) * ((1.2f + (weight / weightMax)) / 2.2f)));
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
			if (driftScore > minBoost){
				boostReady = true;
			}
		}
		else
		{
			if (boostReady){
				var driftExess = driftScore - minBoost;
				driftBoost =  Mathf.Max(driftBoost,Mathf.Min(1 + driftExess / 50, 2));
				tippingThreshold = Mathf.Max(tippingThreshold, Mathf.Min(fixedTippingThreshold + driftExess / 10, fixedTippingThreshold * 1.3f));
			}
			//driftBoost =  Mathf.Max(driftBoost,Mathf.Min(driftScore / 140, 2));
			//tippingThreshold = Mathf.Max(tippingThreshold, Mathf.Min(driftScore / 15, fixedTippingThreshold * 1.3f));
			driftScore = 0;
			boostReady = false;
		}
	}


	private void activateRagdoll() //ragdoll mode
	{
		ragdoll = true;
		PhysicsMaterialOverride = ragdollMaterial;
		camera.setRagdoll(true);
	}

	private void activateNormal()  //normal driving mode
	{
		Freeze = false;
		prepUp = false;
		ragdoll = false;
		camera.setRagdoll(false);
		PhysicsMaterialOverride = slipperyMaterial;
	}

	private void makeUpright(double delta) //raises the cart and changes its rotation to the last rotation before crashing
	{
		Position = Position + (new Godot.Vector3(Position.X, 2, Position.Z)-Position) * (float)(10 * delta);
		Rotation = Rotation + (lastOrientation - Rotation) * (float)(5 * delta);
	}
}
