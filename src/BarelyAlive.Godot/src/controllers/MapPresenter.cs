using Godot;

namespace BarelyAlive.Godot.controllers;

public partial class MapPresenter:Node
{
    [Export] private Sprite2D _mapSprite;
    [Export] public bool HasMap { get; private set; } = false;
    public override void _Ready()
    {
        var gameContext = GameContext.Instance;
        if(gameContext.HasMissionContext) 
            OnMissionContextChanged();
        gameContext.MissionContextChanged += OnMissionContextChanged;
        _mapSprite = GetNode<Sprite2D>("MapTexture");
    }

    private void OnMissionContextChanged()
    {
        if(_mapSprite == null)
            _mapSprite = GetNode<Sprite2D>("MapTexture");
        Texture2D mapTexture = GameContext.Instance.MapTexture;
        _mapSprite.Texture = mapTexture;
        HasMap= true;
    }
    
    
    public void FitMapToArea(Vector2 availableSize)
    {
        if(_mapSprite == null)
            _mapSprite = GetNode<Sprite2D>("MapTexture");
        var texture = _mapSprite.Texture;
        if (texture == null)
            return;
        Vector2 textureSize = texture.GetSize();
        float scaleX = availableSize.X / textureSize.X;
        float scaleY = availableSize.Y / textureSize.Y;

        // Mantener proporción
        Vector2 scale =new Vector2(Mathf.Min(scaleX, scaleY),Mathf.Min(scaleX, scaleY));

        _mapSprite.Scale = scale;
        GameContext.Instance.SetRealTextureSize(scale*textureSize);
    }
}