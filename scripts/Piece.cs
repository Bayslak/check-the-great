using Godot;
namespace ChecktheGreat.scripts;

public partial class Piece : Node2D
{
    private Sprite2D _sprite2D;

    [Export] public PieceHouse House;

    public void SetSprite(Texture2D texture2d)
    {
        _sprite2D ??= GetNode<Sprite2D>("Sprite2D");
        _sprite2D.SetTexture(texture2d);
    }

    public void SetHouse(PieceHouse pieceHouse)
    {
        House = pieceHouse;
    }
}