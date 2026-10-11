using Godot;

namespace ChecktheGreat.scripts;

public partial class Cell : Node2D
{
    [Export] private Sprite2D _sprite, _highlightSprite;
    [Export] private Texture2D _lightTexture, _darkTexture;
    [Export] private Texture2D _highlightLightTexture, _highlightDarkTexture;

    [Export] private Area2D _area2D;

    public override void _Ready()
    {
        _sprite = GetNode<Sprite2D>("sprite");
        _highlightSprite = GetNode<Sprite2D>("highlight_sprite");
        
        _area2D = GetNode<Area2D>("Area2D");
        _area2D.MouseEntered += HandleMouseEntered;
        _area2D.MouseExited += HandleMouseExited;
    }

    public Vector2 GetTextureSize()
    {
        return _sprite.Texture.GetSize();
    }
    
    public void InitializeCell(bool lightCell)
    {
        _sprite ??= GetNode<Sprite2D>("sprite");
        _highlightSprite ??= GetNode<Sprite2D>("highlight_sprite");

        switch (lightCell)
        {
            case true:
                _sprite.SetTexture(_lightTexture);
                _highlightSprite.SetTexture(_highlightLightTexture);
                break;
            case false:
                _sprite.SetTexture(_darkTexture);
                _highlightSprite.SetTexture(_highlightDarkTexture);
                break;
        }
    }
    
    private void HandleMouseExited()
    {
        _highlightSprite.Visible = false;
    }

    private void HandleMouseEntered()
    {
        _highlightSprite.Visible = true;
    }
}