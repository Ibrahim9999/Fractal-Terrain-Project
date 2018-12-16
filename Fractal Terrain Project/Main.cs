using System;
using System.Linq;

namespace Fractal_Terrain_Project
{
	class Program
	{
		public static void Main(string[] args)
		{
			// TODO: Implement Functionality Here
			
			using (var game = new Window(1000,1000))
			{
				game.Run(30.0);
			}
			
			Console.Write("Press any key to continue . . . ");
			//Console.ReadKey(true);
		}
	}
}