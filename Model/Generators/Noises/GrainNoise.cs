using Godot;
using System;

namespace Trains.Model.Generators.Noises
{
	public class GrainNoise : OpenSimplexNoise
	{
		public GrainNoise()
		{
			Period = 8f;	//distance to next value
			Octaves = 4;	//layers
			Persistence = 0.9f;	//the effect that layers have
			Lacunarity = 1.5f;	//detail per layer
		}

		public float GetMyNoise(float x, float y, float max) => GetNoise2d(x, y) * max / 2 + max / 2;
	}
}