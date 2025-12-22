using BarelyAlive.Godot.Model;
using Godot;

namespace BarelyAlive.Godot.controllers;

public partial class MapPresenter : Node
{
    [Export] private Sprite2D _mapSprite;
    [Export] public bool HasMap { get; private set; } = false;
    MapContext _context;
    public override void _Ready()
    {
        GD.Print("[MapPresenter] _Ready called.");
        _context = GameSession.Instance.MapContext;
        
        if (_context.IsMapReady)
        {
            GD.Print("[MapPresenter] MapContext is ALREADY ready. Calling OnMapContextChanged manually.");
            OnMapContextChanged();
        }
        else
        {
            GD.Print("[MapPresenter] MapContext is NOT ready waiting for signal...");
        }
        
        _context.MapContextChanged += OnMapContextChanged;
        _mapSprite = GetNode<Sprite2D>("MapTexture");
    }

    private void OnMapContextChanged()
    {
        GD.Print("[MapPresenter] OnMapContextChanged signal received!");
        if (_mapSprite == null)
            _mapSprite = GetNode<Sprite2D>("MapTexture");
        
        Texture2D mapTexture = _context.MapTexture;
        GD.Print($"[MapPresenter] Setting texture: {mapTexture?.ResourcePath ?? "NULL"}");
        _mapSprite.Texture = mapTexture;
        HasMap = true;
    }


    public void FitMapToArea(Vector2 availableSize)
    {
        if (_mapSprite == null)
            _mapSprite = GetNode<Sprite2D>("MapTexture");
        var texture = _mapSprite.Texture;
        if (texture == null)
            return;
        Vector2 textureSize = texture.GetSize();
        float scaleX = availableSize.X / textureSize.X;
        float scaleY = availableSize.Y / textureSize.Y;

        // Mantener proporción
        Vector2 scale = new Vector2(Mathf.Min(scaleX, scaleY), Mathf.Min(scaleX, scaleY));

        _mapSprite.Scale = scale;
        GameSession.Instance.MapContext.SetRealTextureSize(scale * textureSize);
    }
}