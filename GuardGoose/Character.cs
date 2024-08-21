using Godot;
using System;
using System.Threading;

public partial class Character : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	private Godot.Camera3D camera;
	private Node3D cameraController;
	private Node3D cameraTarget;

    public override void _Ready()
    {
        cameraController = GetNode<Node3D>("Camera_Controller");
        cameraTarget = cameraController.GetNode<Node3D>("Camera_Target");
        camera = cameraTarget.GetNode<Godot.Camera3D>("Camera3D");
    }

    public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.Y = JumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();

		cameraController.Position = LerpXaxisAndZaxis(cameraController.Position, Position, 0.06f);
	}

	private Vector3 LerpXaxisAndZaxis(Vector3 from, Vector3 to, float weight)
	{
		var returnTo = new Vector3()
		{
			X = from.X + (to.X - from.X) * weight,
			Z = from.Z + (to.Z - from.Z) * weight,
		};

		return returnTo;
	}

	
}
