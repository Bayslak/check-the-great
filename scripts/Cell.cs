using Godot;

namespace ChecktheGreat.scripts;

public partial class Cell : Node2D
{
    [Export] private Sprite2D _sprite;
    [Export] private Texture2D _lightTexture, _darkTexture;

    public override void _Ready()
    {
        _sprite =  GetNode<Sprite2D>("Sprite2D");
    }

    public Vector2 GetTextureSize()
    {
        return _sprite.Texture.GetSize();
    }
    
    public void InitializeCell(bool lightCell)
    {
        _sprite ??= GetNode<Sprite2D>("Sprite2D");

        switch (lightCell)
        {
            case true:
                _sprite.SetTexture(_lightTexture);
                break;
            case false:
                _sprite.SetTexture(_darkTexture);
                break;
        }
    }
}