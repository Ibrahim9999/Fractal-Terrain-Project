using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using NCalc;

namespace Fractal_Terrain_Project
{
	/// <summary>
	/// Description of Plot.
	/// </summary>
	/// 
	public enum PlotType
	{
		NONE = 0, STANDARD_EQUATION, PARAMETRIC_EQUATION, FRACTAL, STRANGE_ATTRACTOR, TERRAIN
	}
	
	public enum EquationType
	{
		NONE = 0, CARTESIAN, POLAR, CYLINDRICAL, CONICAL, CARTESIAN_4D, SPHERICAL_4D, CYLINDRICAL_4D, CONICAL_4D,
	}
	
	public enum VariablesUsed
	{
		NONE = 0, ONE, TWO, PARAMETRIC_CURVE, IMPLICIT_CURVE, ONE_TWO, TWO_THREE, ONE_THREE, PARAMETRIC_SURFACE, IMPLICIT_SURFACE,
	}
	
	public enum Comparison
	{
		NONE = 0, EQUALITY, LESS_THAN, GREATER_THAN, LESS_EQUALS, GREATER_EQUALS,
	}
	
	public enum FractalType
	{
		NONE = 0, MANDELBROT, SIERPINSKI, LINDENMAYER,
	}
	
	public enum StrangeAttractorType
	{
		NONE = 0, LORENZ, PICKOVER, CLIFFORD, ROSSLER
	}
	
	public enum NoiseType
	{
		NONE = 0, RANDOM, PERLIN, SIMPLEX
	}
	
	public enum DisplayType
	{
		NONE = 0, POINTS, LINES, TRIANGLES, QUADS
	}
	
	public abstract class Plot
	{
		public PlotType plotType;
		public Comparison comparison;
		public FractalType fractalType;
		public StrangeAttractorType strangeAttractorType;
		
		public bool is3D;
		
		public PointList pointList;
		public PointGrid pointGrid;
		public Polyline polyline;
		public Wireframe wireframe;
		public TriangleMesh triangleMesh;
		public QuadMesh quadMesh;
		
		public List<DisplayType> currentDisplay = new List<DisplayType>();
		
		public bool singleColor;
		public Color color;
		public List<Color> colors;
		public ColorPalette colorPalette = new ColorPalette(ColorPalette.Rainbow());
		
		public float minX;
		public float maxX;
		public float minY;
		public float maxY;
		public float minZ;
		public float maxZ;
		
		List<double> sliders = new List<double>(6);
		public double t;
		
		
		public bool IsEquation()
		{
			return plotType == PlotType.STANDARD_EQUATION || plotType == PlotType.PARAMETRIC_EQUATION;
		}
		
		public bool IsStrangeAttractor()
		{
			return plotType == PlotType.STRANGE_ATTRACTOR;
		}
		
		public bool IsTerrain()
		{
			return plotType == PlotType.TERRAIN;
		}
		
		public void ZoomObject(ref Camera camera)
		{
			var center = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, (minZ + maxZ) / 2);
			
			camera.target = center;
			camera.position = Camera.MoveAlongAxis(camera.target, camera.direction, -camera.targetDistance);
		}
		
		public static void ShowAxis(bool x, bool y, bool z, float width, bool display = true)
		{
			if (display)
			{
				GL.LineWidth(width);
				
				GL.Begin(PrimitiveType.Lines);
				{
					if (x)
						GL.Color3(1f, 1f, 1f);
					else
						GL.Color3(1f, 0, 0);
					GL.Vertex3(-10,0,0);
					GL.Vertex3( 10,0,0);
					
					if (y)
						GL.Color3(1f, 1f, 1f);
					else
						GL.Color3(0, 1f, 0);
					GL.Vertex3(0,-10,0);
					GL.Vertex3(0, 10,0);
					
					if (z)
						GL.Color3(1f, 1f, 1f);
					else
						GL.Color3(0, 0, 1f);
					GL.Vertex3(0,0,-10);
					GL.Vertex3(0,0, 10);
				}
				GL.End();
				
				GL.LineWidth(1);
			}
		}
		
		public void Show(float size, float alpha = 1)
		{
			if (!is3D)
			{
				currentDisplay.Remove(DisplayType.TRIANGLES);
				currentDisplay.Remove(DisplayType.QUADS);
				
				if (!currentDisplay.Contains(DisplayType.POINTS) && !currentDisplay.Contains(DisplayType.LINES))
					currentDisplay.Add(DisplayType.LINES);
			}
			
			if (currentDisplay.Contains(DisplayType.POINTS) && (IsEquation() || IsTerrain()))
			{
				GL.PointSize(size);
				
				GL.Begin(PrimitiveType.Points);
				{
					GL.Color4(color.R / 255f, color.G / 255f, color.B / 255f, alpha);
					
					foreach (PointList pointlist in pointGrid.points)
						foreach (Point point in pointlist.points)
							GL.Vertex3((float) point.X, (float) point.Y, (float) point.Z);
				}
				GL.End();
				GL.PointSize(1);
			}
			else if (currentDisplay.Contains(DisplayType.POINTS) && IsStrangeAttractor())
			{
				GL.PointSize(size);
				
				GL.Begin(PrimitiveType.Points);
				{
					if (singleColor)
						GL.Color4(color.R / 255f, color.G / 255f, color.B / 255f, alpha);
					
					for (int i = 0; i < pointList.Count; i++)
					{
						if (!singleColor)
							GL.Color4(colors[i].R / 255f, colors[i].G / 255f, colors[i].B / 255f, alpha);
						
						GL.Vertex3((float) pointList[i].X, (float) pointList[i].Y, (float) pointList[i].Z);
					}
				}
				GL.End();
				GL.PointSize(1);
			}
			
			if (currentDisplay.Contains(DisplayType.LINES) && (IsEquation() || IsTerrain()))
			{
				var lines = wireframe.ToLines();
				GL.LineWidth(size);
				
				GL.Begin(PrimitiveType.Lines);
				{
					GL.Color4(color.R / 255f, color.G / 255f, color.B / 255f, alpha);
					
					if (is3D)
						foreach (Line line in lines)
						{
							GL.Vertex3((float) line.start.X, (float) line.start.Y, (float) line.start.Z);
							GL.Vertex3((float) line.end.X, (float) line.end.Y, (float) line.end.Z);
						}
					else
						foreach (Line line in wireframe.uCurves[0].ToLines())
						{
							GL.Vertex3((float) line.start.X, (float) line.start.Y, (float) line.start.Z);
							GL.Vertex3((float) line.end.X, (float) line.end.Y, (float) line.end.Z);
						}
				}
				GL.End();
				GL.LineWidth(1);
			}
			else if (currentDisplay.Contains(DisplayType.LINES) && IsStrangeAttractor())
			{
				var lines = polyline.ToLines();
				GL.LineWidth(size);
				
				GL.Begin(PrimitiveType.Lines);
				{
					if (singleColor)
						GL.Color4(color.R / 255f, color.G / 255f, color.B / 255f, alpha);
					
					for (int i = 0; i < lines.Count; i++)
					{
						if (!singleColor)
							GL.Color4(colors[i].R / 255f, colors[i].G / 255f, colors[i].B / 255f, alpha);
						
						GL.Vertex3((float) lines[i].start.X, (float) lines[i].start.Y, (float) lines[i].start.Z);
						GL.Vertex3((float) lines[i].end.X, (float) lines[i].end.Y, (float) lines[i].end.Z);
					}
				}
				GL.End();
				GL.LineWidth(1);
			}
			
			if (currentDisplay.Contains(DisplayType.TRIANGLES))
			{
				GL.Begin(PrimitiveType.Triangles);
				{
					GL.Color4(color.R / 255f, color.G / 255f, color.B / 255f, alpha);
					
					foreach (Triangle t in triangleMesh.triangles)
					{
						var side1 = new Vector3((float) t[0].X, (float) t[0].Y, (float) t[0].Z);
						var side2 = new Vector3((float) t[1].X, (float) t[1].Y, (float) t[1].Z);
						var side3 = new Vector3((float) t[2].X, (float) t[2].Y, (float) t[2].Z);
						
						GL.Normal3(-Vector3.Cross(side3 - side1, side2 - side1));
						GL.Vertex3(side1);
						
						GL.Normal3(-Vector3.Cross(side1 - side2, side3 - side2));
						GL.Vertex3(side2);
						
						GL.Normal3(-Vector3.Cross(side2 - side3, side1 - side3));
						GL.Vertex3(side3);
					}
				}
				GL.End();
			}
			
			if (currentDisplay.Contains(DisplayType.QUADS))
			{
				GL.Begin(PrimitiveType.Quads);
				{
					GL.Color4(color.R / 255f, color.G / 255f, color.B / 255f, alpha);
					
					foreach (Quad q in quadMesh.quads)
					{
						var side1 = new Vector3((float) q[0].X, (float) q[0].Y, (float) q[0].Z);
						var side2 = new Vector3((float) q[1].X, (float) q[1].Y, (float) q[1].Z);
						var side3 = new Vector3((float) q[2].X, (float) q[2].Y, (float) q[2].Z);
						var side4 = new Vector3((float) q[3].X, (float) q[3].Y, (float) q[3].Z);
						
						GL.Normal3(-Vector3.Cross(side4 - side1, side2 - side1));
						GL.Vertex3(side1);
						
						GL.Normal3(-Vector3.Cross(side1 - side2, side3 - side2));
						GL.Vertex3(side2);
						
						GL.Normal3(-Vector3.Cross(side2 - side3, side4 - side3));
						GL.Vertex3(side3);
						
						GL.Normal3(-Vector3.Cross(side3 - side4, side1 - side4));
						GL.Vertex3(side4);
					}
				}
				GL.End();
			}
			
		}
		
		public void Hide()
		{
			currentDisplay = new List<DisplayType>();
		}
		
		public int GetSliderCount()
		{
			return sliders.Count;
		}
		
		public void AddSlider(double value)
		{
			sliders.Add(value);
		}
		public double GetSlider(int index)
		{
			return sliders[index];
		}
		public void SetSlider(int index, double value)
		{
			sliders[index] = value;
		}
		public void AdjustSlider(int index, double value)
		{
			sliders[index] += value;
		}
		
		public abstract override string ToString();
		public abstract void Generate();
	}
	
	public abstract class StrangeAttractor : Plot
	{
		public Point startPoint;
		public Point currentPoint;
		
		public int initialIterations;
		public int maxIteration;
		public int currentIteration;
		
		public bool dynamicDraw;
		
		public void Update()
		{
			NextPoint();
			AddPoint();
		}
		
		public void FixPoint()
		{
			const double maxValue = 1e99;
			
			if (Double.IsPositiveInfinity(currentPoint.X) || currentPoint.X > maxValue)
				currentPoint.X = maxValue;
			else if (Double.IsNegativeInfinity(currentPoint.X) || currentPoint.X < -maxValue)
				currentPoint.X = -maxValue;
			if (Double.IsPositiveInfinity(currentPoint.Y) || currentPoint.Y > maxValue)
				currentPoint.Y = maxValue;
			else if (Double.IsNegativeInfinity(currentPoint.Y) || currentPoint.Y < -maxValue)
				currentPoint.Y = -maxValue;
			if (Double.IsPositiveInfinity(currentPoint.Z) || currentPoint.Z > maxValue)
				currentPoint.Z = maxValue;
			else if (Double.IsNegativeInfinity(currentPoint.Z) || currentPoint.Z < -maxValue)
				currentPoint.Z = -maxValue;
		}
		
		public void AddPoint()
		{
			pointList.points.Add(currentPoint);
			
			colors.Add(colorPalette.GetColor((int) (currentIteration * (double) (colorPalette.colors.Count - 1) / maxIteration)));
			//colors.Add(colorPalette.GetColor());
			colorPalette.NextColor();
			
			minX = (float) Math.Min(currentPoint.X, minX);
			maxX = (float) Math.Max(currentPoint.X, maxX);
			minY = (float) Math.Min(currentPoint.Y, minY);
			maxY = (float) Math.Max(currentPoint.Y, maxY);
			minZ = (float) Math.Min(currentPoint.Z, minZ);
			maxZ = (float) Math.Max(currentPoint.Z, maxZ);
			
			if (dynamicDraw)
				polyline = pointList.ToPolyline();
		}
		
		public override string ToString()
		{
			return  (is3D ? "3D " : "") + strangeAttractorType + " Attractor:\n\tMaxIteration: " + maxIteration + "\n\tdT: " + t + "\n\tA: " + GetSlider(0) + "\n\tB: " + GetSlider(1) + "\n\tC: " + GetSlider(2)/* + "\n\tD: " + GetSlider(3)*/ + "\n\tX Bounds: " + minX + ", " + maxX + "\n\tY Bounds: " + minY + ", " + maxY + "\n\tZ Bounds: " + minZ + ", " + maxZ + '\n';
		}
		
		public override void Generate()
		{
			colors = new List<Color>();
			colorPalette.SetCurrentIndex(0);
			currentIteration = 0;
			pointList = new PointList(new List<Point>());
			polyline = new Polyline(new List<Point>());
			currentPoint = startPoint;
			
			minX = 0;
			maxX = 0;
			minY = 0;
			maxY = 0;
			minZ = 0;
			maxZ = 0;
			
			for (int i = 0; i < initialIterations; i++)
				NextPoint();
			
			AddPoint();
			
			for (int i = 0; i < maxIteration && !dynamicDraw; i++)
			{
				NextPoint();
				AddPoint();
			}
			
			if (!dynamicDraw)
				polyline = pointList.ToPolyline();
		}
		
		public abstract void NextPoint();
	}
	
	public abstract class Equation : Plot
	{
		public int pointsPerCurve;
		public int curvesPerSurface;
		public double var4D;
		
		public Bounds firstVar;
		public Bounds secondVar;
		public Bounds thirdVar;
		
		public EquationType equationType;
		public VariablesUsed variablesUsed;
		public List<string> vars = new List<string>();
		
		public static void RemoveMathFunctions(ref string eq)
		{
			eq = eq.Replace("ieeeremainder(", "");
			eq = eq.Replace("remainder(", "");
			eq = eq.Replace("truncate(", "");
			eq = eq.Replace("randdec(", "");
			eq = eq.Replace("randint(", "");
			eq = eq.Replace("ceiling(", "");
			eq = eq.Replace("random(", "");
			eq = eq.Replace("round(", "");
			eq = eq.Replace("floor(", "");
			eq = eq.Replace("log10(", "");
			eq = eq.Replace("asin(", "");
			eq = eq.Replace("acos(", "");
			eq = eq.Replace("atan(", "");
			eq = eq.Replace("sinh(", "");
			eq = eq.Replace("cosh(", "");
			eq = eq.Replace("tanh(", "");
			eq = eq.Replace("csch(", "");
			eq = eq.Replace("sech(", "");
			eq = eq.Replace("coth(", "");
			eq = eq.Replace("sinc(", "");
			eq = eq.Replace("sign(", "");
			eq = eq.Replace("sqrt(", "");
			eq = eq.Replace("rand", "");
			eq = eq.Replace("abs(", "");
			eq = eq.Replace("pow(", "");
			eq = eq.Replace("min(", "");
			eq = eq.Replace("max(", "");
			eq = eq.Replace("exp(", "");
			eq = eq.Replace("log(", "");
			eq = eq.Replace("sin(", "");
			eq = eq.Replace("cos(", "");
			eq = eq.Replace("tan(", "");
			eq = eq.Replace("csc(", "");
			eq = eq.Replace("sec(", "");
			eq = eq.Replace("cot(", "");
			eq = eq.Replace("ln(", "");
			eq = eq.Replace("pi", "");
		}
		
		public static bool IsValid(Expression e)
		{
			if (e.HasErrors())
			{
				Console.WriteLine(e.Error);
				return false;
			}
			
			return true;
		}
		
		public static bool CheckForBadVar(char[] eq)
		{
			foreach (char c in eq)
				if ((int) c != 101 && ((int) c >= 97 && (int) c <= 122))
			    	return true;
			
			return false;
		}
		
		public static bool HasRightVariables(string equation, string a, string b = "", string c = "", string d = "", string e = "", string f = "", string g = "", string h = "", string i = "")
		{
			if (equation.Length == 0)
				return false;
			
			equation = equation.Substring(equation.IndexOf('=') + 1);
			
			RemoveMathFunctions(ref equation);
			
			if (equation.IndexOf(a, StringComparison.Ordinal) != -1 && a != "")
				equation = equation.Replace(a, "");
			if (equation.IndexOf(b, StringComparison.Ordinal) != -1 && b != "")
				equation = equation.Replace(b, "");
			if (equation.IndexOf(c, StringComparison.Ordinal) != -1 && c != "")
				equation = equation.Replace(c, "");
			if (equation.IndexOf(d, StringComparison.Ordinal) != -1 && d != "")
				equation = equation.Replace(d, "");
			if (equation.IndexOf(e, StringComparison.Ordinal) != -1 && e != "")
				equation = equation.Replace(e, "");
			if (equation.IndexOf(f, StringComparison.Ordinal) != -1 && f != "")
				equation = equation.Replace(f, "");
			if (equation.IndexOf(g, StringComparison.Ordinal) != -1 && g != "")
				equation = equation.Replace(g, "");
			if (equation.IndexOf(h, StringComparison.Ordinal) != -1 && h != "")
				equation = equation.Replace(h, "");
			if (equation.IndexOf(i, StringComparison.Ordinal) != -1 && i != "")
				equation = equation.Replace(i, "");
			if (equation.Length != 0)
				return !CheckForBadVar(equation.ToCharArray());
			
			return true;
		}
		
		public static void SetParameters(ref Expression expression)
		{
			expression.Parameters["pi"] = Math.PI;
			expression.Parameters["e"] = Math.E;
			
			expression.EvaluateFunction += delegate (string name, FunctionArgs args)
			{
				double d = Double.NaN,
					e = Double.NaN,
					f = Double.NaN;
				
				if (args.Parameters.Length > 0)
					d = Convert.ToDouble(args.Parameters[0].Evaluate());
				if (args.Parameters.Length > 1)
					e = Convert.ToDouble(args.Parameters[1].Evaluate());
				if (args.Parameters.Length > 2)
					f = Convert.ToDouble(args.Parameters[2].Evaluate());
				
				switch (name)
				{
					case "abs":
						args.Result = Math.Abs(d);
						break;
					case "pow":
						args.Result = Math.Pow(d, e);
						break;
					case "sqrt":
						args.Result = Math.Sqrt(d);
						break;
					case "round":
						args.Result = Math.Round(d, Convert.ToInt32(e));
						break;
					case "sign":
						if (d > 0)
							args.Result = 1;
						else if (d < 0)
							args.Result = -1;
						else
							args.Result = 0;
						break;
					case "min":
						args.Result = Math.Min(d, e);
						break;
					case "max":
						args.Result = Math.Max(d, e);
						break;
					case "ceiling":
						args.Result = Math.Ceiling(d);
						break;
					case "truncate":
						args.Result = Math.Truncate(d);
						break;
					case "exp":
						args.Result = Math.Exp(d);
						break;
					case "floor":
						args.Result = Math.Floor(d);
						break;
					case "remainder":
					case "ieeeremainder":
						args.Result = Math.IEEERemainder(d, e);
						break;
					case "ln":
						args.Result = Math.Log(d);
						break;
					case "log":
						if (Double.IsNaN(e))
							args.Result = Math.Log10(d);
						else
							args.Result = Math.Log10(e) / Math.Log10(d);
						break;
					case "log10":
						args.Result = Math.Log10(d);
						break;
					case "sin":
						args.Result = Math.Sin(d % (2 * Math.PI));
						break;
					case "cos":
						args.Result = Math.Cos(d % (2 * Math.PI));
						break;
					case "tan":
						args.Result = Math.Tan(d % (2 * Math.PI));
						break;
					case "csc":
						args.Result = 1 / Math.Sin(d % (2 * Math.PI));
						break;
					case "sec":
						args.Result = 1 / Math.Cos(d % (2 * Math.PI));
						break;
					case "cot":
						args.Result = 1 / Math.Tan(d % (2 * Math.PI));
						break;
					case "asin":
						args.Result = Math.Asin(d % (2 * Math.PI));
						break;
					case "acos":
						args.Result = Math.Acos(d % (2 * Math.PI));
						break;
					case "atan":
						args.Result = Math.Atan(d % (2 * Math.PI));
						break;
					case "sinh":
						args.Result = Math.Sinh(d);
						break;
					case "cosh":
						args.Result = Math.Cosh(d);
						break;
					case "tanh":
						args.Result = Math.Tanh(d);
						break;
					case "csch":
						args.Result = 1 / Math.Sinh(d);
						break;
					case "sech":
						args.Result = 1 / Math.Cosh(d);
						break;
					case "coth":
						args.Result = 1 / Math.Tanh(d);
						break;
					case "sinc":
						if (Equals(d, 0))
							args.Result = 1;
						else
							args.Result = Math.Sin(d % (2 * Math.PI)) / d;
						break;
					case "random":
						var random = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
						if (Double.IsNaN(d))
							d = 1;
						args.Result = random.NextDouble() * d;
						break;
					case "randint":
						random = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
						if (d < e)
							args.Result = (int)d + (int)((e - d + 1) * random.NextDouble());
						else
							args.Result = (int)e + (int)((d - e + 1) * random.NextDouble());
						break;
					case "randdec":
						random = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
						if (d < e)
							args.Result = d + (e - d + 1) * random.NextDouble();
						else
							args.Result = e + (d - e + 1) * random.NextDouble();
						break;
				}
			};
		}
		
		public void UpdateFourthVar(double var4)
		{
			var4D = var4;
			
			Generate();
		}
		
		public abstract void DetermineEquationType();
	}
	
	public sealed class StandardEquation : Equation
	{
		public string expression;
		
		public StandardEquation(string expression, bool is3D, Bounds firstVar, Bounds secondVar, int pointsPerCurve, int curvesPerSurface, double var4D, Comparison comparison = Comparison.EQUALITY)
		{
			plotType = PlotType.STANDARD_EQUATION;
			fractalType = FractalType.NONE;
			strangeAttractorType = StrangeAttractorType.NONE;
			this.is3D = is3D;
			
			this.var4D = var4D;
			
			this.expression = expression.ToLower().Replace(" ", "");
			this.firstVar = firstVar;
			this.secondVar = secondVar;
			this.pointsPerCurve = pointsPerCurve;
			this.curvesPerSurface = curvesPerSurface;
			this.comparison = comparison;
			
			DetermineEquationType();
			
			Generate();
		}
		
		public override string ToString()
		{
			return equationType + " Equation:\n\t\"" + expression + "\"\n";
		}
		
		public override void DetermineEquationType()
		{
			if (this.expression.IndexOf("x", StringComparison.Ordinal) != -1 || this.expression.IndexOf("y", StringComparison.Ordinal) != -1 || (this.expression.IndexOf("z", StringComparison.Ordinal) != -1 && this.expression.IndexOf("theta", StringComparison.Ordinal) == -1 && this.expression.IndexOf("phi", StringComparison.Ordinal) == -1 && this.expression.IndexOf("r", StringComparison.Ordinal) == -1))
				equationType = EquationType.CARTESIAN;
			else if ((this.expression.IndexOf("theta", StringComparison.Ordinal) != -1 || this.expression.IndexOf("phi", StringComparison.Ordinal) != -1) && (this.expression.IndexOf("r", StringComparison.Ordinal) != -1 || this.expression.IndexOf("z", StringComparison.Ordinal) == -1))
				equationType = EquationType.POLAR;
			else
				equationType = EquationType.CYLINDRICAL;
			
		    string one = "";
		    string two = "";
		    string three = "";
		    string four = "";
		    string simplifiedEq = this.expression.Substring(this.expression.IndexOf('=') + 1);
			bool oneExists = false;
			bool twoExists = false;
			bool threeExists = false;
			bool fourExists = false;
			bool equalsExists = false;
			
		    if (IsValid(new Expression(this.expression)))
		    {
				switch (equationType)
				{
					case EquationType.POLAR:
						one = "theta";
						two = "phi";
						three = "r";
						four = "s";
						break;
					case EquationType.CYLINDRICAL:
						one = "theta";
						two = "z";
						three = "r";
						four = "s";
						break;
					default:
						one = "x";
						two = "y";
						three = "z";
						four = "w";
						break;
				}
		    }
		    
			oneExists |= simplifiedEq.IndexOf(one, StringComparison.Ordinal) != -1;
			twoExists |= simplifiedEq.IndexOf(two, StringComparison.Ordinal) != -1;
			threeExists |= simplifiedEq.IndexOf(three, StringComparison.Ordinal) != -1;
			fourExists |= simplifiedEq.IndexOf(four, StringComparison.Ordinal) != -1;
			equalsExists |= this.expression.IndexOf('=') != -1;
			
			simplifiedEq += "+0*a*b*c*d*e*f*" + four;
			
			if (((oneExists || twoExists || fourExists || HasRightVariables(this.expression, one, two, four, "a", "b", "c", "d", "e", "f")) && (this.expression.StartsWith(three + "=", StringComparison.Ordinal) || this.expression.StartsWith("f(" + one + "," + two + ")=", StringComparison.Ordinal) || !equalsExists)) && HasRightVariables(this.expression, one, two, four, "a", "b", "c", "d", "e", "f"))
			{
				simplifiedEq += "*" + one + "*" + two;
				variablesUsed = VariablesUsed.ONE_TWO;
				vars.Add(one);
				vars.Add(two);
			}
			else if (((twoExists || threeExists || fourExists || HasRightVariables(this.expression, two, three, four, "a", "b", "c", "d", "e", "f")) && (this.expression.StartsWith(one + "=", StringComparison.Ordinal) || this.expression.StartsWith("f(" + two + "," + three + ")=", StringComparison.Ordinal) || !equalsExists)) && HasRightVariables(this.expression, two, three, four, "a", "b", "c", "d", "e", "f"))
			{
				simplifiedEq += "*" + two + "*" + three;
				variablesUsed = VariablesUsed.TWO_THREE;
				vars.Add(two);
				vars.Add(three);
			}
			else if (((oneExists || threeExists || fourExists || HasRightVariables(this.expression, one, three, four, "a", "b", "c", "d", "e", "f")) && (this.expression.StartsWith(two + "=", StringComparison.Ordinal) || this.expression.StartsWith("f(" + one + "," + three + ")=", StringComparison.Ordinal) || !equalsExists)) && HasRightVariables(this.expression, one, three, four, "a", "b", "c", "d", "e", "f"))
			{
				simplifiedEq += "*" + one + "*" + three;
				variablesUsed = VariablesUsed.ONE_THREE;
				vars.Add(one);
				vars.Add(three);
			}
			
			vars.Add(four);
			this.expression = simplifiedEq;
		}
		
		public override void Generate()
		{
			var eq = new Expression(expression);
      		SetParameters(ref eq);
			
			double curveIteration = (secondVar.max - secondVar.min) / curvesPerSurface;
			double pointIteration = (firstVar.max - firstVar.min) / pointsPerCurve;
			
			int curveDecimalCount = Calculate.DecimalCount(curveIteration);
			int pointDecimalCount = Calculate.DecimalCount(pointIteration);
			
			wireframe = new Wireframe(new List<Polyline>(), new List<Polyline>());
			triangleMesh = new TriangleMesh(new List<Triangle>());
			quadMesh = new QuadMesh(new List<Quad>());
			//Console.WriteLine(firstVar + "\n" + secondVar + "\n");
			Bounds curveBounds = firstVar;
			Bounds pointBounds = secondVar;
			
			string one = vars[0];
			string two = vars[1];
			
			if (!is3D)
			{
				one = vars[1];
				two = vars[0];
				
				pointBounds = curveBounds;
				curveBounds = new Bounds(0,0);
				
				curveIteration = 2;
			}
			
			eq.Parameters[vars[2]] = var4D;
			eq.Parameters["a"] = GetSlider(0);
			eq.Parameters["b"] = GetSlider(1);
			eq.Parameters["c"] = GetSlider(2);
			eq.Parameters["d"] = GetSlider(3);
			eq.Parameters["e"] = GetSlider(4);
			eq.Parameters["f"] = GetSlider(5);
			
			double result = 0;
			
			for (double varOne = curveBounds.min; varOne <= curveBounds.max; varOne += curveIteration)
			{
				eq.Parameters[one] = Math.Round(varOne, curveDecimalCount);
				var curve = new Polyline(new List<Point>());
				
				for (double varTwo = pointBounds.min; varTwo <= pointBounds.max; varTwo += pointIteration)
				{
					eq.Parameters[two] = Math.Round(varTwo, pointDecimalCount);
					
					result = Convert.ToDouble(eq.Evaluate());
					
					if (Double.IsPositiveInfinity(result))
						result = Double.MaxValue;
					else if (Double.IsNegativeInfinity(result))
						result = Double.MinValue;
					else if (Double.IsNaN(result))
						result = 0;
					
					Point functionResult = Calculate.FunctionResult(equationType, variablesUsed, is3D ? varOne : varTwo, is3D ? varTwo : varOne, result);
					
					minX = (float) Math.Min(functionResult.X, minX);
					maxX = (float) Math.Max(functionResult.X, minX);
					minY = (float) Math.Min(functionResult.Y, minY);
					maxY = (float) Math.Max(functionResult.Y, minY);
					minZ = (float) Math.Min(functionResult.Z, minZ);
					maxZ = (float) Math.Max(functionResult.Z, minZ);
					
					curve.Add(functionResult);
				}
				
//				if (closeV)
//					curve.Add(curve[0]);
				
				wireframe.uCurves.Add(curve);
			}
			
//			if (closeU)
//				surface.Add(surface[0]);
			
			pointGrid.FromWireframe(wireframe);
			wireframe.MakeVFromU();
			triangleMesh.MakeFromWireframe(wireframe);
			quadMesh.MakeFromWireframe(wireframe);
		}
	}
	
	public sealed class LorenzAttractor : StrangeAttractor
	{
		public LorenzAttractor(Point startPoint, bool is3D, int initialIterations, int maxIteration, double t, double a, double b, double c, bool dynamicDraw = false)
		{
			plotType = PlotType.STRANGE_ATTRACTOR;
			comparison = Comparison.NONE;
			fractalType = FractalType.NONE;
			strangeAttractorType = StrangeAttractorType.LORENZ;
			
			triangleMesh = new TriangleMesh(new List<Triangle>());
			quadMesh = new QuadMesh(new List<Quad>());
			
			this.startPoint = startPoint;
			this.initialIterations = initialIterations;
			this.maxIteration = maxIteration;
			this.t = t;
			AddSlider(a);
			AddSlider(b);
			AddSlider(c);
			this.dynamicDraw = dynamicDraw;
			this.is3D = is3D;
			
			Generate();
		}
		
		public override void NextPoint()
		{
			currentPoint +=  t * new Point(GetSlider(0) * (currentPoint.Y - currentPoint.X),
			                               -currentPoint.X * (currentPoint.Z - GetSlider(1)) - currentPoint.Y,
										   is3D ? currentPoint.X * currentPoint.Y - GetSlider(2) * currentPoint.Z : 0);
			
			FixPoint();
			
			currentIteration++;
		}
	}
	
	public sealed class PickoverAttractor : StrangeAttractor
	{
		public PickoverAttractor(Point startPoint, bool is3D, int initialIterations, int maxIteration, double a, double b, double c, double d, bool dynamicDraw = false)
		{
			plotType = PlotType.STRANGE_ATTRACTOR;
			comparison = Comparison.NONE;
			fractalType = FractalType.NONE;
			strangeAttractorType = StrangeAttractorType.PICKOVER;
			
			triangleMesh = new TriangleMesh(new List<Triangle>());
			quadMesh = new QuadMesh(new List<Quad>());
			
			this.startPoint = startPoint;
			this.initialIterations = initialIterations;
			this.maxIteration = maxIteration;
			AddSlider(a);
			AddSlider(b);
			AddSlider(c);
			AddSlider(d);
			this.dynamicDraw = dynamicDraw;
			this.is3D = is3D;
			
			Generate();
		}
		
		public override void NextPoint()
		{
			currentPoint =  new Point(Math.Sin(GetSlider(0) * currentPoint.Y) - currentPoint.Z * Math.Cos(GetSlider(1) * currentPoint.X),
			                 		  currentPoint.Z * Math.Sin(GetSlider(2) * currentPoint.Y) - Math.Cos(GetSlider(3) * currentPoint.X),
			                 		  is3D ? GetSlider(4) * Math.Sin(currentPoint.X): 0);
			
			FixPoint();
			
			currentIteration++;
		}
	}
	
	public sealed class CliffordAttractor : StrangeAttractor
	{
		public CliffordAttractor(Point startPoint, bool is3D, int initialIterations, int maxIteration, double a, double b, double c, double d, bool dynamicDraw = false)
		{
			plotType = PlotType.STRANGE_ATTRACTOR;
			comparison = Comparison.NONE;
			fractalType = FractalType.NONE;
			strangeAttractorType = StrangeAttractorType.CLIFFORD;
			
			triangleMesh = new TriangleMesh(new List<Triangle>());
			quadMesh = new QuadMesh(new List<Quad>());
			
			this.startPoint = startPoint;
			this.initialIterations = initialIterations;
			this.maxIteration = maxIteration;
			SetSlider(0, a);
			SetSlider(1, b);
			SetSlider(2, c);
			SetSlider(3, d);
			this.dynamicDraw = dynamicDraw;
			this.is3D = is3D;
			
			Generate();
		}
		
		public override void NextPoint()
		{
			currentPoint =  new Point(Math.Sin(GetSlider(0) * currentPoint.Y) + GetSlider(2) * Math.Sin(GetSlider(0) * currentPoint.X),
			                 		  Math.Sin(GetSlider(1) * currentPoint.X) + GetSlider(3) * Math.Sin(GetSlider(1) * currentPoint.Y),
			                 		  is3D ? GetSlider(4) * Math.Sin(currentPoint.X): 0);
			
			FixPoint();
			
			currentIteration++;
//			currentPoint.X *= 10;
//			currentPoint.Y *= 10;
//			currentPoint.Z *= 10;
//			Console.WriteLine(currentPoint);
		}
	}
	
	
	public sealed class RosslerAttractor : StrangeAttractor
	{
		public RosslerAttractor(Point startPoint, bool is3D, int initialIterations, int maxIteration, double a, double b, double c, double d, bool dynamicDraw = false)
		{
			plotType = PlotType.STRANGE_ATTRACTOR;
			comparison = Comparison.NONE;
			fractalType = FractalType.NONE;
			strangeAttractorType = StrangeAttractorType.ROSSLER;
			
			triangleMesh = new TriangleMesh(new List<Triangle>());
			quadMesh = new QuadMesh(new List<Quad>());
			
			this.startPoint = startPoint;
			this.initialIterations = initialIterations;
			this.maxIteration = maxIteration;
			SetSlider(0, a);
			SetSlider(1, b);
			SetSlider(2, c);
			SetSlider(3, d);
			this.dynamicDraw = dynamicDraw;
			this.is3D = is3D;
			
			Generate();
		}
		
		public override void NextPoint()
		{
			currentPoint =  new Point(-currentPoint.Y - currentPoint.Z,
			                 		  currentPoint.X + GetSlider(0) * currentPoint.Y,
			                 		  is3D ? GetSlider(1) + currentPoint.Z * (currentPoint.X - GetSlider(1)) : 0);
			
			FixPoint();
			
			currentIteration++;
		}
	}
	
	public sealed class TerrainMap : Plot
	{
		Bitmap heightMap;
		NoiseType noiseType;
		int xRes = 100;
		int yRes = 100;
		
		double maxTerrainHeight = 10;
		double minTerrainHeight = -10;
		
		public TerrainMap(double minTerrainHeight = -10, double maxTerrainHeight = 10)
		{
			is3D = true;
			
			triangleMesh = new TriangleMesh(new List<Triangle>());
			quadMesh = new QuadMesh(new List<Quad>());
			
			noiseType = NoiseType.NONE;
			
			this.minTerrainHeight = minTerrainHeight;
			this.maxTerrainHeight = maxTerrainHeight;
			
			heightMap = new Bitmap(xRes, yRes);
		}
		public TerrainMap(NoiseType noiseType, double minTerrainHeight = -10, double maxTerrainHeight = 10)
		{
			is3D = true;
			
			triangleMesh = new TriangleMesh(new List<Triangle>());
			quadMesh = new QuadMesh(new List<Quad>());
			
			this.noiseType = noiseType;
			
			this.minTerrainHeight = minTerrainHeight;
			this.maxTerrainHeight = maxTerrainHeight;
			
			heightMap = new Bitmap(xRes, yRes);
		}
		public TerrainMap(int xRes, int yRes, double minTerrainHeight = -10, double maxTerrainHeight = 10)
		{
			is3D = true;
			
			triangleMesh = new TriangleMesh(new List<Triangle>());
			quadMesh = new QuadMesh(new List<Quad>());
			
			noiseType = NoiseType.NONE;
			
			this.xRes = xRes;
			this.yRes = yRes;
			
			this.minTerrainHeight = minTerrainHeight;
			this.maxTerrainHeight = maxTerrainHeight;
			
			heightMap = new Bitmap(xRes, yRes);
		}
		public TerrainMap(int xRes, int yRes, NoiseType noiseType, double minTerrainHeight = -10, double maxTerrainHeight = 10)
		{
			is3D = true;
			
			triangleMesh = new TriangleMesh(new List<Triangle>());
			quadMesh = new QuadMesh(new List<Quad>());
			
			this.noiseType = noiseType;
			
			this.xRes = xRes;
			this.yRes = yRes;
			
			this.minTerrainHeight = minTerrainHeight;
			this.maxTerrainHeight = maxTerrainHeight;
			Console.WriteLine(noiseType);
			heightMap = new Bitmap(xRes, yRes);
		}
		
		public override string ToString()
		{
			throw new NotImplementedException();
		}
		
		public Bitmap GetHeightmap()
		{
			return heightMap;
		}
		public NoiseType Noise
		{
			get { return noiseType; }
		}
		public int Width
		{
			get { return xRes; }
		}
		public int Height
		{
			get { return yRes; }
		}
		public double MinTerrainHeight
		{
			get { return minTerrainHeight; }
		}
		public double MaxTerrainHeight
		{
			get { return maxTerrainHeight; }
		}
		
		public void GenerateHeightmap()
		{
			for (int y = 0; y < Height; y++)
				for (int x = 0; x < Width; x++)
					switch (noiseType)
					{
						case NoiseType.RANDOM:
							heightMap.SetPixel(x, y, ColorPalette.GetRandomColor());
							
							break;
						case NoiseType.PERLIN:
							heightMap.SetPixel(x, y, PerlinNoise());
							
							break;
						case NoiseType.SIMPLEX:
							heightMap.SetPixel(x, y, SimplexNoise());
							
							break;
						default:
							heightMap.SetPixel(x, y, Color.Black);
							
							break;
					}
		}
		
		public override void Generate()
		{
			triangleMesh = new TriangleMesh(new List<Triangle>());
			quadMesh = new QuadMesh(new List<Quad>());
			
			GenerateHeightmap();
			
			Point nwPoint = new Point(),
				  nePoint = new Point(),
				  sePoint = new Point(),
				  swPoint = new Point();
			
			
			var grid = new Point[xRes,yRes];
			
			// Create points
			for (int i = 0; i < xRes; i++)
				for (int j = 0; j < yRes; j++)
					grid[i,j] = new Point(((double)i/(xRes-1)) * 20 - 10, ((double)j/(yRes-1)) * 20 - 10, heightMap.GetPixel(i, j).GetHue()/360 * 20 -10);
			
			for (int i = 0; i < xRes - 1; i++)
				for (int j = 0; j < yRes - 1; j++)
				{
				//Create point values
				{
					nwPoint = grid[i,j];
					nePoint = grid[i + 1,j];
					sePoint = grid[i + 1,j + 1];
					swPoint = grid[i,j + 1];
					
					//Create triangles
					triangleMesh.Add(new Triangle(nwPoint, nePoint, sePoint));
					triangleMesh.Add(new Triangle(nwPoint, sePoint, swPoint));
		
					// Create quad
					quadMesh.Add(new Quad(nwPoint, nePoint, sePoint, swPoint));}
				}
		}
		
		public static Color PerlinNoise()
		{Console.WriteLine(ColorPalette.GetRandomColor());
			return ColorPalette.GetRandomColor();
		}
		
		public static Color SimplexNoise()
		{
			return ColorPalette.GetRandomColor();
		}
	}
}
