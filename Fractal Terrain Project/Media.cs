using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Fractal_Terrain_Project
{
	public class ColorPalette
	{
		public List<Color> colors;
		int currentIndex;
		
		public ColorPalette(List<Color> colors)
		{
			this.colors = colors;
		}
		
		public void SetCurrentIndex(int index)
		{
			currentIndex = index;
		}
		
		public Color GetColor()
		{
			return colors[currentIndex];
		}
		public Color GetColor(int index, bool setIndex = false)
		{
			index %= colors.Count;
			
			if (setIndex)
				currentIndex = index;
			
			return colors[index];
		}
		
		public void NextColor(int skip = 1)
		{
			currentIndex += skip;
			
			if (currentIndex >= colors.Count)
				currentIndex %= colors.Count;
		}
		
		public static void ColorToHSV(Color color, out double hue, out double saturation, out double value)
		{
		    int max = Math.Max(color.R, Math.Max(color.G, color.B));
		    int min = Math.Min(color.R, Math.Min(color.G, color.B));
		
		    hue = color.GetHue();
		    saturation = (max == 0) ? 0 : 1d - (1d * min / max);
		    value = max / 255d;
		}
		
		public static Color ColorFromHSV(double hue, double saturation, double value)
		{
			int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
			double f = hue / 60 - Math.Floor(hue / 60);
		
			value = value * 255;
			int v = Convert.ToInt32(value);
			int p = Convert.ToInt32(value * (1 - saturation));
			int q = Convert.ToInt32(value * (1 - f * saturation));
			int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));
			
//		    Output.ToRhinoConsole("v", v);Output.ToRhinoConsole("t", t);Output.ToRhinoConsole("p", p);
		    
			if (hi == 0)
				return Color.FromArgb(v, t, p);
			if (hi == 1)
				return Color.FromArgb(q, v, p);
			if (hi == 2)
				return Color.FromArgb(p, v, t);
			if (hi == 3)
				return Color.FromArgb(p, q, v);
			if (hi == 4)
				return Color.FromArgb(t, p, v);
			
			return Color.FromArgb(v, p, q);
		}
		
		public static Color GetRandomColor()
		{
			return Color.FromArgb(Calculate.RandInt(0, 255), Calculate.RandInt(0, 255), Calculate.RandInt(0, 255));
		}
		
		public static Bitmap GrayscaleToFalseColor(Bitmap pic)
		{
			for (int x = 0; x < pic.Width; x++)
				for (int y = 0; y < pic.Height; y++)
					pic.SetPixel(x,y, ColorFromHSV(pic.GetPixel(x,y).GetBrightness() * 360 / 1, 1, 1));
			
			return pic;
		}
		public static Bitmap GrayscaleToFalseColor(Bitmap pic, IList<Color> colors)
		{
			for (int x = 0; x < pic.Width; x++)
				for (int y = 0; y < pic.Height; y++)
					pic.SetPixel(x,y, colors[(int) (pic.GetPixel(x,y).GetBrightness() * colors.Count)]);
			
			return pic;
		}
		
		public static Bitmap BlackAndWhite(Bitmap pic)
		{
			for (int x = 0; x < pic.Width; x++)
				for (int y = 0; y < pic.Height; y++)
					pic.SetPixel(x,y, pic.GetPixel(x,y) == Color.Black ? Color.Black : Color.White);
			
			return pic;
		}
		
		public static Color Blend(Color color1, Color color2, double amount)
		{
		    return Color.FromArgb((int) ((color2.R * amount) + color1.R * (1 - amount)), (int) ((color2.G * amount) + color1.G * (1 - amount)), (int) ((color2.B * amount) + color1.B * (1 - amount)));
		}
		
		internal static List<Color> OneColor(Color color)
		{
			var colors = new List<Color>();
			colors.Add(color);
			
			return colors;
		}
		
		internal static List<Color> Custom(List<Color> colors, bool chopped = false)
		{
			return Custom(colors, 0, chopped);
		}
		internal static List<Color> Custom(List<Color> colors, double skipRatio, bool chopped)
		{
			int colorPallete = 256 * colors.Count;
			
			var newColors = new List<Color>();
			int colorValueR = colors[0].R;
			int colorValueG = colors[0].G;
			int colorValueB = colors[0].B;
			
			if (skipRatio > 1 || skipRatio < 0)
				skipRatio = Math.Abs(skipRatio) % 1;
			
			int skip = (int) skipRatio * colorPallete;
			
			if (chopped || colors.Count == 1)
				return colors;
			
			for(int i = 0; i < colors.Count - 1; i++)
			{
				for (double a = 0; a < 1; a += 0.00390625)
					newColors.Add(Blend(colors[i], colors[i+1], a));
				
				if (i + 1 == colors.Count)
					break;
				
//			    Output.Here(skip);Output.ToRhinoConsole("R", colorValueR);Output.ToRhinoConsole("G", colorValueG);Output.ToRhinoConsole("B", colorValueB);
			    
//			    skip = skip < colorPallete - 1 ? skip + 1 : 0;
			}
			
			return newColors;
		}
		
		// RYB - COMPLETE
        internal static List<Color> RYB(bool chopped)
        {
        	return RYB(0, chopped);
        }
        internal static List<Color> RYB(double skipRatio = 0, bool chopped = false)
		{
		    // Should be 767, which is the count of this list of colors
			const int colorPallete = 767;
			
		    var colors = new List<Color>();
		    int colorValueR = 255;
			int colorValueG = 0;
			int colorValueB = 0;
			
			if (skipRatio > 1 || skipRatio < 0)
				skipRatio = Math.Abs(skipRatio) % 1;
		    
			int skip = (int) (skipRatio * colorPallete);
		    
		    /*
		     * Red: 255,0,0
		     * Green: 0,255,0
		     * Blue: 0,0,255
		     */
		    
		    for(int i = 0; i < colorPallete; i++)
		    {
			    if (skip >= 512)
			    {// Blue to Red (512-767)
			    	colorValueR = chopped ? 0 : skip - 512;
			        colorValueG = 0;
			    	colorValueB = chopped ? 255 : 767 - skip;
			    }
			    else if (skip >= 256)
			    {// Green to Blue 256-511)
			    	colorValueR = 0;
			    	colorValueB = chopped ? 0 : skip - 256;
			    	colorValueG = chopped ? 255 : 511 - skip;
			    }
			    else
			    {// Red to Green (0-255)
			        colorValueR = chopped ? 255 : 255 - skip;
			        colorValueG = chopped ? 0 : skip;
			        colorValueB = 0;
			    }
//			   Output.Here(skip);Output.ToRhinoConsole("R", colorValueR);Output.ToRhinoConsole("G", colorValueG);Output.ToRhinoConsole("B", colorValueB);
			    colors.Add(Color.FromArgb(colorValueR, colorValueG, colorValueB));
			    
			    skip = skip < colorPallete - 1 ? skip + 1 : 0;
			}
		    
		    return colors;
		}
        
		// ROYGBV
		internal static List<Color> Rainbow(bool chopped)
		{
			return Rainbow(0, chopped);
		}
        internal static List<Color> Rainbow(double skipRatio = 0, bool chopped = false)
		{
			// Should be 1536, which is the count of this list of colors
			const int colorPallete = 1536;
			
		    var colors = new List<Color>();
		    int colorValueR = 255;
			int colorValueG = 0;
			int colorValueB = 0;
			
			if (skipRatio > 1 || skipRatio < 0)
				skipRatio = Math.Abs(skipRatio) % 1;
		    
			int skip = (int) (skipRatio * colorPallete);
		    /*
		     * Red: 255,0,0
		     * Orange: 255,127,0
		     * Yellow: 255,255,0
		     * Green: 0,255,0
		     * Blue: 0,0,255
		     * Purple: 127,0,255
		     */
		    
		    for(int i = 0; i < colorPallete; i++)
		    {
			    if (skip >= 1024)
			    {// Yellow to Orange to Red (1024-1535)
			    	colorValueR = 255;
			    	colorValueG = chopped ? (skip > 1280 ? 127 : 255) : 767 - skip / 2;
			    	colorValueB = 0;
			    }
			    else if (skip >= 768)
			    {// Green to Yellow (768-1023)
			    	colorValueR = chopped ? 0 : skip - 768;
			    	colorValueG = 255;
			    	colorValueB = 0;
			    }
			    else if (skip >= 512)
			    {// Blue to Green (512-767)
			    	colorValueR = 0;
			    	colorValueB = chopped ? 255 : 767 - skip;
			    	colorValueG = chopped ? 0 : skip - 512;
			    }
			    else if (skip >= 256)
			    {// Purple to Blue (256-511)
			        colorValueR = chopped ? 255 : 255 - skip / 2;
			        colorValueG = 0;
			    	colorValueB = 255;
			    }
			    else
			    {// Red to Purple (0-255)
			        colorValueR = chopped ? 255 : 255 - skip / 2;
			        colorValueG = 0;
			        colorValueB = chopped ? 0 : skip;
			    }
//			    Output.Here(skip);Output.ToRhinoConsole("R", colorValueR);Output.ToRhinoConsole("G", colorValueG);Output.ToRhinoConsole("B", colorValueB);
			    colors.Add(Color.FromArgb(colorValueR, colorValueG, colorValueB));
			    
			    skip = skip < colorPallete - 1 ? skip + 1 : 0;
			}
		    
		    return colors;
		}
        
		// RGB
        internal static List<Color> RGB(bool chopped)
        {
        	return RGB(0, chopped);
        }
        internal static List<Color> RGB(double skipRatio = 0, bool chopped = false)
		{
		    // Should be 767, which is the count of this list of colors
			const int colorPallete = 767;
			
		    List<Color> colors = new List<Color>();
		    int colorValueR = 255;
			int colorValueG = 0;
			int colorValueB = 0;
			
			if (skipRatio > 1 || skipRatio < 0)
				skipRatio = Math.Abs(skipRatio) % 1;
		    
			int skip = (int) (skipRatio * colorPallete);
		    
		    /*
		     * Red: 255,0,0
		     * Green: 0,255,0
		     * Blue: 0,0,255
		     */
		    
		    for(int i = 0; i < colorPallete; i++)
		    {
			    if (skip >= 512)
			    {// Blue to Red (512-767)
			    	colorValueR = chopped ? 0 : skip - 512;
			        colorValueG = 0;
			    	colorValueB = chopped ? 255 : 767 - skip;
			    }
			    else if (skip >= 256)
			    {// Green to Blue 256-511)
			    	colorValueR = 0;
			    	colorValueB = chopped ? 0 : skip - 256;
			    	colorValueG = chopped ? 255 : 511 - skip;
			    }
			    else
			    {// Red to Green (0-255)
			        colorValueR = chopped ? 255 : 255 - skip;
			        colorValueG = chopped ? 0 : skip;
			        colorValueB = 0;
			    }
//			   Output.Here(skip);Output.ToRhinoConsole("R", colorValueR);Output.ToRhinoConsole("G", colorValueG);Output.ToRhinoConsole("B", colorValueB);
			    colors.Add(Color.FromArgb(colorValueR, colorValueG, colorValueB));
			    
			    skip = skip < colorPallete - 1 ? skip + 1 : 0;
			}
		    
		    return colors;
		}
        
		// ROYGBV - COMPLETE
		internal static List<Color> Standard(bool chopped)
		{
			return Standard(0, chopped);
		}
        internal static List<Color> Standard(double skipRatio = 0, bool chopped = false)
		{
			// Should be 1536, which is the count of this list of colors
			const int colorPallete = 1536;
			
		    var colors = new List<Color>();
		    int colorValueR = 255;
			int colorValueG = 0;
			int colorValueB = 0;
			
			if (skipRatio > 1 || skipRatio < 0)
				skipRatio = Math.Abs(skipRatio) % 1;
		    
			int skip = (int) (skipRatio * colorPallete);
		    /*
		     * Red: 255,0,0
		     * Orange: 255,127,0
		     * Yellow: 255,255,0
		     * Green: 0,255,0
		     * Blue: 0,0,255
		     * Purple: 127,0,255
		     */
		    
		    for(int i = 0; i < colorPallete; i++)
		    {
			    if (skip >= 1024)
			    {// Yellow to Orange to Red (1024-1535)
			    	colorValueR = 255;
			    	colorValueG = chopped ? (skip > 1280 ? 127 : 255) : 767 - skip / 2;
			    	colorValueB = 0;
			    }
			    else if (skip >= 768)
			    {// Green to Yellow (768-1023)
			    	colorValueR = chopped ? 0 : skip - 768;
			    	colorValueG = 255;
			    	colorValueB = 0;
			    }
			    else if (skip >= 512)
			    {// Blue to Green (512-767)
			    	colorValueR = 0;
			    	colorValueB = chopped ? 255 : 767 - skip;
			    	colorValueG = chopped ? 0 : skip - 512;
			    }
			    else if (skip >= 256)
			    {// Purple to Blue (256-511)
			        colorValueR = chopped ? 255 : 255 - skip / 2;
			        colorValueG = 0;
			    	colorValueB = 255;
			    }
			    else
			    {// Red to Purple (0-255)
			        colorValueR = chopped ? 255 : 255 - skip / 2;
			        colorValueG = 0;
			        colorValueB = chopped ? 0 : skip;
			    }
//			    Output.Here(skip);Output.ToRhinoConsole("R", colorValueR);Output.ToRhinoConsole("G", colorValueG);Output.ToRhinoConsole("B", colorValueB);
			    colors.Add(Color.FromArgb(colorValueR, colorValueG, colorValueB));
			    
			    skip = skip < colorPallete - 1 ? skip + 1 : 0;
			}
		    
		    return colors;
		}
        
		// Black and White
        internal static List<Color> Monochrome(bool chopped)
        {
        	return Monochrome(0, chopped);
        }
        internal static List<Color> Monochrome(double skipRatio = 0, bool chopped = false)
		{
		    const int colorPallete = 1536;
			
		    List<Color> colors = new List<Color>();
		    int colorValueR = 0;
			int colorValueG = 0;
			int colorValueB = 0;
			
			if (skipRatio > 1 || skipRatio < 0)
				skipRatio = Math.Abs(skipRatio) % 1;
		    
			int skip = (int) (skipRatio * colorPallete);
		    
		    /*
		     * Black: 0,0,0
		     * White: 255,255,255
		     */
		    
		    for(int i = 0; i < colorPallete; i++)
		    {
			    if (skip >= 768)
			    {// White to Black (1024-1535)
			        colorValueR = chopped ? 255 : 384 - skip / 4;
			        colorValueG = chopped ? 255 : 384 - skip / 4;
			        colorValueB = chopped ? 255 : 384 - skip / 4;
			    }
			    else
			    {// Black to White (0-767)
			    	colorValueR = chopped ? 0 : skip / 4;
			    	colorValueB = chopped ? 0 : skip / 4;
			    	colorValueG = chopped ? 0 : skip / 4;
			    }
//			    Output.Here(skip);Output.ToRhinoConsole("R", colorValueR);Output.ToRhinoConsole("G", colorValueG);Output.ToRhinoConsole("B", colorValueB);
			    colors.Add(Color.FromArgb(colorValueR, colorValueG, colorValueB));
			    
			    skip = skip < colorPallete - 1 ? skip + 1 : 0;
			}
		    
		    return colors;
		}
	}
}