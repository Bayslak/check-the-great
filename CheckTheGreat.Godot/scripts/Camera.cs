using Godot;

namespace CheckTheGreat.Godot.scripts;

public partial class Camera : Camera2D
{
    public override void _Ready()
    {
        var viewportSize = GetViewport().GetVisibleRect().Size;
        this.Position = new Vector2(viewportSize.X / 2, viewportSize.Y / 2);
    }
}