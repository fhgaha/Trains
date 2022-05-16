using Godot;
using System;

public class Sprite3DScript : Sprite3D
{
    //workaround for error
    public override void _Ready()
    {
        Texture = GetNode<Viewport>("Viewport").GetTexture();
    }

}
