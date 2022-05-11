using Godot;
using System;
using System.Collections.Generic;

public class Cell : Spatial
{
    //hue represents as fraction of degrees
	float maxHueValue_green = 113f/360f;    //0.36f
	float minHueValue_red = 0f;

	public int Size { get; set; } = 1;
	public Color Color { get; private set; }

    private List<Product> products;
    private Dictionary<Product, float> demandRate;

	private RandomNumberGenerator random = new RandomNumberGenerator();
	private Timer timer;

	public override void _Ready()
	{
		timer = new Timer();
		AddChild(timer);

		timer.Connect("timeout", this, "SetColor");
        timer.Start(1.0f);
	}

	public override void _Process(float delta)
	{
		//set color to mesh instance
		//SetColorToMeshInstance();
	}

    private float d = 0.01f;
	private void SetColor()
	{
		MeshInstance mesh = (MeshInstance)GetNode(@"MeshInstance");
		var material = (SpatialMaterial)mesh.GetSurfaceMaterial(0);
		var color = material.AlbedoColor;
		float h, s, v;
		color.ToHsv(out h, out s, out v);
        random.Randomize();   
        h += GetValueBasedOfDemand(h);  
		h = Mathf.Clamp(h, minHueValue_red, maxHueValue_green);

		var newColor = Color.FromHsv(h, s, v);
		material.AlbedoColor = newColor;

        GD.Print(this.ToString() + ", color RGB: " + newColor);
        GD.Print("color HSV: " + h + "; " + s + "; " + v);
        GD.Print();
	}

	private float GetValueBasedOfDemand(float h)
	{
        //get new (or addition to original?) hue value based on changed product's price
		return -0.02f;
	}
}
