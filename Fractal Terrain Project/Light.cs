using System;
using OpenTK;

namespace Fractal_Terrain_Project
{
	/// <summary>
	/// Description of Light.
	/// </summary>
	public class Light
	{
		public Vector3 position;
		public Vector3 color;
		public float diffuseIntensity;
		public float ambientIntensity;
		
		public Light(Vector3 pos, Vector3 col, float diffIntensity = 1, float ambIntensity = 1)
		{
			position = pos;
			color = col;
			diffuseIntensity = diffIntensity;
			ambientIntensity = ambIntensity;
		}
	}
}
