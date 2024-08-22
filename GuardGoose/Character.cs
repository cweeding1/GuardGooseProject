using Godot;
using System;
using System.Threading;

public partial class Character : CharacterBody3D
{
	[Export] public float moveSpeed = 5.0f;
	[Export] public float jumpForce = 10.0f;
	[Export] public float gravity = -9.8f;
	[Export] public float inputBufferTime = 0.2f;
	private float jumpBufferTimeRemaining = 0.0f;
	private Godot.Camera3D camera;
	private Node3D cameraController;
	private Node3D cameraTarget;

	public override void _Ready()
	{
		cameraController = GetNode<Node3D>("CharacterCameraNode");
		cameraTarget = cameraController.GetNode<Node3D>("CharacterCameraTarget");
		camera = cameraTarget.GetNode<Godot.Camera3D>("CharacterCamera");
	}

	private void HandleInputBuffer(float delta)
	{
		if (jumpBufferTimeRemaining > 0)
		{
			jumpBufferTimeRemaining -= delta;
		}
		if (Input.IsActionJustPressed("ui_accept"))
		{
			jumpBufferTimeRemaining = inputBufferTime;
		}
	}

	private void HandleMovement(float delta)
	{
		Vector3 velocity = Velocity;

		Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

		Vector3 moveDirection = new Vector3(inputDir.X, 0, inputDir.Y).Normalized();

		velocity.X = moveDirection.X * moveSpeed;
		velocity.Z = moveDirection.Z * moveSpeed;

		// Apply gravity
		if (!IsOnFloor())
		{
			velocity.Y += gravity * delta * 3;
		}
		else
		{
			velocity.Y = 0; 

			if (jumpBufferTimeRemaining > 0)
			{
				velocity.Y = jumpForce;
				jumpBufferTimeRemaining = 0;
			}
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	public override void _PhysicsProcess(double delta)
	{
		HandleInputBuffer((float)delta);
		HandleMovement((float)delta);

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
