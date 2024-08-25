using Godot;
using System;

public partial class lock_scene : Node2D
{
	private Node2D lockRoot;
	private Sprite2D keySprite;
	private Label keyLabel;
   
    private float angle;
    private float length = 200;
    private float width = 25;
	private Vector2[] rectanglePoints;


	private int keys = 3;


        int rnd = new Random().Next(0,360);
        angle = Mathf.DegToRad(rnd);

		//Setup

		int rnd = new Random().Next(0,360);
		angle = Mathf.DegToRad(rnd);

		rectanglePoints = _GeneratePickArea(width, length);
		
		keySprite.LookAt(GetGlobalMousePosition());

		keyLabel = GetNode<Label>("KeyLabel");
		_SetKeyText();
	}

    public override void _Draw()
    {
        DrawLine(rectanglePoints[0], rectanglePoints[1], new Color(1, 0, 0), 2);  // Red line
        DrawLine(rectanglePoints[1], rectanglePoints[2], new Color(1, 0, 0), 2);  // Red line
        DrawLine(rectanglePoints[2], rectanglePoints[3], new Color(1, 0, 0), 2);  // Red line
        DrawLine(rectanglePoints[3], rectanglePoints[0], new Color(1, 0, 0), 2);  // Red line
    }

    public override void _Process(double delta)
    {
        Vector2 mousePosition = GetGlobalMousePosition();
        Vector2 direction = mousePosition - GlobalPosition;
        float mouseAngle = Mathf.Atan2(direction.Y, direction.X);
        keySprite.Rotation = mouseAngle;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            Vector2 mousePosition = GetGlobalMousePosition();
			if (Geometry2D.IsPointInPolygon(mousePosition, rectanglePoints))
            {
                GD.Print("IN");
                _GenerateNewSmallerPickArea();
            }
            else
            {
                GD.Print("OUT");
				if(keys > 0)
				{
					keys -= 1;
				}
				_SetKeyText();
            }
        }
    }

	private void _SetKeyText()
	{
		keyLabel.Text = $"Keys: {keys}";
	}

	private Vector2[] _GeneratePickArea( float width, float length )
	{
		Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).Normalized();
		Vector2 perpendicular = new Vector2(-direction.Y, direction.X).Normalized() * (width / 2);

        Vector2 corner1 = perpendicular;
        Vector2 corner2 = -perpendicular;
        Vector2 corner3 = direction * length + perpendicular;
        Vector2 corner4 = direction * length - perpendicular;
        
        return new Vector2[] { corner1, corner3, corner4, corner2 };
    }

    private void _GenerateNewSmallerPickArea()
    {
        // Reduce the size of the lock picking area
        width *= 0.8f;
        length *= 0.8f;

        // Generate new points for the smaller area
        points = _GeneratePickArea(width, length);

        // Redraw the scene to reflect the new area
        GD.Print("Redrawing");
        
    }
}