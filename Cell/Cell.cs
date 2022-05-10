using Godot;
using System;

public class Cell : Spatial
{
	int maxHueValue_green = 113;
	int minHueValue_red = 0;

    public int Size {get; set;} = 1;
    public int X { get; set; } 
    public int Y { get; set; }
    public Color Color { get; private set; }
    private Random random = new Random();

    public override void _Process(float delta)
	{
        //set color to mesh instance
		MeshInstance mesh = (MeshInstance)GetNode(@"MeshInstance");
        var material = (SpatialMaterial)mesh.GetSurfaceMaterial(0);
        var color = material.AlbedoColor;
        float h, s, v; 
        color.ToHsv(out h, out s, out v);
        h += (float)random.NextDouble() * 0.2f;
        h = Mathf.Clamp(h, minHueValue_red, maxHueValue_green);

        var newColor = Color.FromHsv(h, s, v);
        material.AlbedoColor = newColor;
	}	
}
