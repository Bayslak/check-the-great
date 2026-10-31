using Godot;

namespace CheckTheGreat.Godot.scripts;

public partial class Cell : Node2D
{
    private Board _board;
    
    [Export] private Sprite2D _sprite, _highlightSprite, _startingTileSprite;
    [Export] private Texture2D _lightTexture, _darkTexture;
    [Export] private Texture2D _highlightLightTexture, _highlightDarkTexture;

    [Export] private Area2D _area2D;
    public bool IsHighlighted;
    public bool IsStartingTile;
    
    public override void _Ready()
    {
        _sprite = GetNode<Sprite2D>("sprite");
        _highlightSprite = GetNode<Sprite2D>("highlight_sprite");
        _startingTileSprite = GetNode<Sprite2D>("starting_tile_sprite");
        
        _area2D = GetNode<Area2D>("Area2D");
        _area2D.MouseEntered += HandleMouseEntered;
        _area2D.MouseExited += HandleMouseExited;
    }

    public Vector2 GetTextureSize()
    {
        return _sprite.Texture.GetSize();
    }
    
    public void InitializeCell(bool lightCell, Board board)
    {
        _board =  board;
        
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
        DeHighlight();
        SetCellHovered(null);
    }

    private void HandleMouseEntered()
    {
        Highlight();
        SetCellHovered(this);
    }

    private void SetCellHovered(Cell? cell)
    {
        _board.SetCellHovered(cell);
    }
    
    public void Highlight()
    {
        _highlightSprite.Visible = true;
        IsHighlighted = true;
    }

    public void DeHighlight()
    {
        _highlightSprite.Visible = false;
        IsHighlighted = false;
    }

    public void SetStartingTile(bool value)
    {
        _startingTileSprite ??= GetNode<Sprite2D>("starting_tile_sprite");
        
        _startingTileSprite.Visible = value;
        IsStartingTile = value;
    }
}