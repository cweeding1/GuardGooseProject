using Godot;
using System;

public partial class lock_scene : Node2D
{
    private Node2D lockRoot;
    private Sprite2D keySprite;
    private float angle;
    private float length = 200;
    private float width = 25;
    private Vector2[] points;

    public override void _Ready()
    {
        lockRoot = GetNode<Node2D>("LockRoot");
        keySprite = lockRoot.GetNode<Sprite2D>("KeySprite");
        keySprite.LookAt(GetGlobalMousePosition());

        int rnd = new Random().Next(0,360);
        angle = Mathf.DegToRad(rnd);

        points = _GeneratePickArea(width, length);
        
        keySprite.LookAt(GetGlobalMousePosition());
        
        lockRoot = GetNode<Node2D>("LockRoot");
        GD.Print(lockRoot.Position);
    }

    public override void _Draw()
    {
        DrawLine(points[0], points[1], new Color(1, 0, 0), 2);  // Red line
        DrawLine(points[1], points[2], new Color(1, 0, 0), 2);  // Red line
        DrawLine(points[2], points[3], new Color(1, 0, 0), 2);  // Red line
        DrawLine(points[3], points[0], new Color(1, 0, 0), 2);  // Red line
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
            if (Geometry2D.IsPointInPolygon(mousePosition, points))
            {
                GD.Print("IN");
                _GenerateNewSmallerPickArea();
            }
            else
            {
                GD.Print("OUT");
            }
        }
    }

    private Vector2[] _GeneratePickArea(float width, float length)
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