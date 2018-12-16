using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Fractal_Terrain_Project
{
	/// <summary>
	/// Description of Window.
	/// </summary>
	public class Window : GameWindow
	{
		List<Plot> objects = new List<Plot>();
		
		Camera camera = new Camera();
		Key axisRotation = Key.Unknown;
		KeyboardState lastKeyState;
		Vector2 lastMousePos;
		float lastScrollPos;
		bool onScreen = true;
		bool fullScreen;
		bool firstPerson;
		bool axisOn = true;
		bool targetOn;
		
		const int pixelBuffer = 80;
		const double valueShift = .02;
		const double keyboardConstant = 5;
		const double zoomConstant = .1;
		const double moveConstant = 5;
		const double scrollConstant = 5;
		
		Light activeLight = new Light(new Vector3(), new Vector3(.9f, .8f, .8f));
		
		Dictionary<string, Key> keyMap = new Dictionary<string, Key>();
		
		float time;
		float alpha = 1;
		float objectSize = 1;
		double var4D;
		
		// Width, Height, #-bit color depth, # bits in the depth buffer, Stencil Buffer, Multisampling
		public Window(int width, int height) : base(width, height, new GraphicsMode(32,24,0,1))
		{
			// For when DPI is NOT 1:1
			Bounds = new Rectangle(900, 450, width, height);
		}
		
		// Returns ID for new program object 
		void InitProgram()
		{
			ResetCamera();
			
			/*
			//var equation = new StandardEquation("y = a * sin(b * x + c) + d", true, new Bounds (-10,10), new Bounds (-10,10), 30, 30, var4D);
			var equation = new StandardEquation("z = a * sin(b * x + w + c) + d * sin(e * y + w + f)", true, new Bounds (-5,5), new Bounds (-5,5), 10, 10, var4D);
			//var equation = new StandardEquation("z = cos(sqrt(x*x+y*y)-5*w)", true, new Bounds (-10,10), new Bounds (-10,10), 30, 30, var4D);
			//var equation = new StandardEquation("theta = r + s", true, new Bounds (-Math.PI,Math.PI), new Bounds (0,2*Math.PI), 50, 50, var4D);
			//var equation = new StandardEquation("r = 4*pow(cos(phi*s),2)", true, new Bounds (-.1,Math.PI), new Bounds (-Math.PI,Math.PI), 50, 50, var4D);
			equation.currentDisplay.Add(DisplayType.QUADS);
			equation.a = 1;
			equation.b = 1;
			equation.d = 1;
			equation.e = 1;
			
			objects.Add(equation);*/
			
			/*
			//var pickover = new PickoverAttractor(new Point(0,0,0), true, 100, 100000, 0, 0, 0, 0);
			var pickover = new PickoverAttractor(new Point(0,0,0), true, 100, 10000, .574, 4.71, 1.21, .75);
			pickover.currentDisplay.Add(DisplayType.POINTS);
            pickover.singleColor = true;
            pickover.e = 1;
			
			objects.Add(pickover);*/
			
			
			/*
			//var cliffordAttractor = new CliffordAttractor(new Point(.0001,0,0), false, 100, 100000, 0, 0, 0, 0);
			var cliffordAttractor = new CliffordAttractor(new Point(.0001,0,0), false, 100, 100000, 2.879879, -0.966918, .765145, .744728);
			//var cliffordAttractor = new CliffordAttractor(new Point(.1,0,0), false, 100, 100000, -1.7, -1.3, -.1, -1.2);
			cliffordAttractor.currentDisplay.Add(DisplayType.POINTS);
            cliffordAttractor.singleColor = true;
            cliffordAttractor.e = 1;
			
			objects.Add(cliffordAttractor);
			*/
			
			/*
			//var rossler = new RosslerAttractor(new Point(.0001,0,0), false, 100, 100000, 0, 0, 0, 0);
			var rossler = new RosslerAttractor(new Point(.0001,0,0), true, 100, 10000, .015, .2, .2, 5.7);
			rossler.currentDisplay.Add(DisplayType.POINTS);
            rossler.singleColor = true;
            rossler.is3D = true;
            rossler.e = 1;
			
			objects.Add(rossler);
			*/
			
			/* */
			//var lorenz = new LorenzAttractor(new Point(.1,.1,0), true, 1000, 100000, .001, 0, 0, 0, 0);
			var lorenz = new LorenzAttractor(new Point(.0001,0,0), true, 1500, 100000, .001, 10, 28, 8d/3);
			//var lorenz = new LorenzAttractor(new Point(.0001,0,0), true, 1000, 100000, .001, 52.308, 24.088, 8d/3);
			lorenz.currentDisplay.Add(DisplayType.POINTS);
			
			objects.Add(lorenz);
			
		}
		
		// Initializes camera values
		void ResetCamera()
		{
			camera.position = new Vector3(0,0,20);
			
			camera.horizontalAxis = new Vector3(1, 0, 0);
			camera.verticalAxis = new Vector3(0, -1, 0);
			camera.depthAxis = new Vector3(0, 0, -1);
			camera.direction = camera.depthAxis;
			
			camera.fovAngle = 57;
			camera.moveSpeed = .03f;
			camera.zoomSpeed = 2f;
			camera.turnAngle = .2f;
			
			camera.UpdateTargetDistance();
		}
		
		// Maps all keys
		void MapKeys()
		{
			// Fullscreen mode
			keyMap.Add("fullScreen", Key.F11);
			
			// Display info
			keyMap.Add("displayInfo", Key.Slash);
			
			// Rotation Type change
			keyMap.Add("cameraMode", Key.B);
			
			// Axis toggle
			keyMap.Add("toggleAxis", Key.Semicolon);
			
			// Toggle Point toggle
			keyMap.Add("toggleTarget", Key.Quote);
			
			// Target 3D object
			keyMap.Add("toggle3D", Key.M);
			
			// Toggle Lighting
			keyMap.Add("toggleLighting", Key.L);
			
			// Object Size change
			keyMap.Add("increaseObjectSize", Key.BracketRight);
			keyMap.Add("decreaseObjectSize", Key.BracketLeft);
			
			// Display object types
			keyMap.Add("togglePointDisplay", Key.U);
			keyMap.Add("toggleLineDisplay", Key.I);
			keyMap.Add("toggleTriangleDisplay", Key.O);
			keyMap.Add("toggleQuadDisplay", Key.P);
			
			// Maximum Iteration change
			keyMap.Add("increaseMaxIteration", Key.C);
			keyMap.Add("decreaseMaxIteration", Key.V);
			
			// Movement
			keyMap.Add("moveUp", Key.W);
			keyMap.Add("moveLeft", Key.A);
			keyMap.Add("moveDown", Key.S);
			keyMap.Add("moveRight", Key.D);
			
			// Zooming
			keyMap.Add("zoomIn", Key.R);
			keyMap.Add("zoomOut", Key.F);
			
			keyMap.Add("zoomObject", Key.J);
			
			// Rotating
			keyMap.Add("pitchUp", Key.Up);
			keyMap.Add("yawLeft", Key.Left);
			keyMap.Add("pitchDown", Key.Down);
			keyMap.Add("yawRight", Key.Right);
			keyMap.Add("rollLeft", Key.Q);
			keyMap.Add("rollRight", Key.E);
			
			keyMap.Add("rotateAroundX", Key.X);
			keyMap.Add("rotateAroundY", Key.Y);
			keyMap.Add("rotateAroundZ", Key.Z);
			
			// Transparency change
			keyMap.Add("increaseTransparency", Key.H);
			keyMap.Add("decreaseTransparency", Key.G);
			
			// var4D change
			keyMap.Add("increaseVar4D", Key.Period);
			keyMap.Add("decreaseVar4D", Key.Comma);
			
			// Strange Attractor parameter change
			keyMap.Add("parameterT", Key.Number0);
			keyMap.Add("parameterA", Key.Number1);
			keyMap.Add("parameterB", Key.Number2);
			keyMap.Add("parameterC", Key.Number3);
			keyMap.Add("parameterD", Key.Number4);
			keyMap.Add("parameterE", Key.Number5);
			keyMap.Add("parameterF", Key.Number6);
		}
        
		bool IsPlotChanged()
		{
			KeyboardState keyState = Keyboard.GetState();
			
			return keyState.IsKeyDown(keyMap["toggle3D"]) ||
				   keyState.IsKeyDown(keyMap["parameterT"]) ||
				   keyState.IsKeyDown(keyMap["parameterA"]) ||
				   keyState.IsKeyDown(keyMap["parameterB"]) ||
				   keyState.IsKeyDown(keyMap["parameterC"]) ||
				   keyState.IsKeyDown(keyMap["parameterD"]) ||
				   keyState.IsKeyDown(keyMap["parameterE"]) ||
				   keyState.IsKeyDown(keyMap["parameterF"]) ||
				   keyState.IsKeyDown(keyMap["increaseVar4D"]) ||
				   keyState.IsKeyDown(keyMap["decreaseVar4D"]) ||
				   keyState.IsKeyDown(keyMap["increaseMaxIteration"]) ||
				   keyState.IsKeyDown(keyMap["decreaseMaxIteration"]);
		}
		
		// Centers mouse cursor in window so it doesn't leave window
		void ResetCursor()
		{/*
			MouseState mouseState = Mouse.GetCursorState();
			int mouseX = mouseState.X;
			int mouseY = mouseState.Y;
			
			if (mouseX < Bounds.Left + pixelBuffer)
			{
				mouseX = Bounds.Right - pixelBuffer;
			}
			else if (mouseX > Bounds.Right - pixelBuffer)
			{
				mouseX = Bounds.Left + pixelBuffer;
			}
			if (mouseY < Bounds.Top + pixelBuffer)
			{
				mouseY = Bounds.Bottom - pixelBuffer;
			}
			else if (mouseY > Bounds.Bottom - pixelBuffer)
			{
				mouseY = Bounds.Top + pixelBuffer;
			}
			
			OpenTK.Input.Mouse.SetPosition(mouseX, mouseY);
			*/
			// Sets mouse position to center of screen
			OpenTK.Input.Mouse.SetPosition(Bounds.Left + Bounds.Width / 2, Bounds.Top + Bounds.Height / 2);
		}
		
		// Keyboard Controller
		void KeyboardController()
		{
			var keyState = Keyboard.GetState();
			bool shift = keyState.IsKeyDown(Key.ShiftLeft) || keyState.IsKeyDown(Key.ShiftRight);
			bool ctrl = keyState.IsKeyDown(Key.ControlLeft) || keyState.IsKeyDown(Key.ControlRight);
			bool alt = keyState.IsKeyDown(Key.AltLeft) || keyState.IsKeyDown(Key.AltRight);
			bool tab = keyState.IsKeyDown(Key.Tab);
			
			double shiftFactor = (shift ? (alt ? 100 : 10) : 1) * (ctrl ? (alt ? .01 : .1) : 1) * (tab ? -1 : 1);
			double moveFactor = (shift ? (alt ? 100 : 10) : 1) * (ctrl ? (alt ? .01 : .1) : 1);
			
			if (keyState.IsKeyDown(Key.Escape))
				Exit();
			
			if (keyState.IsKeyDown(Key.N))
				((StrangeAttractor) objects[0]).startPoint.X+=shiftFactor;
			if (keyState.IsKeyDown(Key.M))
				((StrangeAttractor) objects[0]).startPoint.Y+=shiftFactor;
			if (keyState.IsKeyDown(Key.Comma))
				((StrangeAttractor) objects[0]).startPoint.Z+=shiftFactor;
							
			// Centers camera to view object
			if (keyState.IsKeyDown(keyMap["zoomObject"]))
				if (shift)
					ResetCamera();
				else
					foreach (Plot p in objects)
					{
						p.ZoomObject(ref camera);
						
						camera.position = Camera.MoveAlongAxis(camera.target, camera.direction, Math.Min(p.minX-p.maxX, Math.Min(p.minY-p.maxY, p.minZ-p.maxZ)));
					}
			
			// Movement
			if (keyState.IsKeyDown(keyMap["moveUp"]))
				camera.MoveAlongAxis(camera.verticalAxis, (float) (moveFactor * keyboardConstant));
			if (keyState.IsKeyDown(keyMap["moveLeft"]))
				camera.MoveAlongAxis(camera.horizontalAxis, (float) (moveFactor * keyboardConstant));
			if (keyState.IsKeyDown(keyMap["moveDown"]))
				camera.MoveAlongAxis(-camera.verticalAxis, (float) (moveFactor * keyboardConstant));
			if (keyState.IsKeyDown(keyMap["moveRight"]))
				camera.MoveAlongAxis(-camera.horizontalAxis, (float) (moveFactor * keyboardConstant));
			if (keyState.IsKeyDown(keyMap["zoomIn"]))
				camera.Zoom((float) (moveFactor * keyboardConstant * zoomConstant));
			if (keyState.IsKeyDown(keyMap["zoomOut"]))
				camera.Zoom((float) -(moveFactor * keyboardConstant * zoomConstant));
			
			// Rotation
			if (keyState.IsKeyDown(keyMap["pitchUp"]))
			{
				if (axisRotation == Key.Unknown)
				{
					if (firstPerson)
						camera.Pitch((float) -(moveFactor * keyboardConstant));
					else
						camera.ArcBallPitch((float) -(moveFactor * keyboardConstant));
				}
				else
				{
					switch (axisRotation)
					{
						case Key.X:
							camera.RotateX(camera.turnAngle * (float) shiftFactor);
							break;
						case Key.Y:
							camera.RotateY(camera.turnAngle * (float) shiftFactor);
							break;
						default:
							camera.RotateZ(camera.turnAngle * (float) shiftFactor);
							break;
					}
				}
			}
			if (keyState.IsKeyDown(keyMap["pitchDown"]))
			{
				if (axisRotation == Key.Unknown)
				{
					if (firstPerson)
						camera.Pitch((float) (moveFactor * keyboardConstant));
					else
						camera.ArcBallPitch((float) (moveFactor * keyboardConstant));
				}
				else
				{
					switch (axisRotation)
					{
						case Key.X:
							camera.RotateX(-camera.turnAngle * (float) shiftFactor);
							break;
						case Key.Y:
							camera.RotateY(-camera.turnAngle * (float) shiftFactor);
							break;
						default:
							camera.RotateZ(-camera.turnAngle * (float) shiftFactor);
							break;
					}
				}
			}
			if (keyState.IsKeyDown(keyMap["yawRight"]))
			{
				if (axisRotation == Key.Unknown)
				{
					if (firstPerson)
						camera.Yaw((float) -(moveFactor * keyboardConstant));
					else
						camera.ArcBallYaw((float) -(moveFactor * keyboardConstant));
				}
				else
				{
					switch (axisRotation)
					{
						case Key.X:
							camera.RotateX(-camera.turnAngle * (float) shiftFactor);
							break;
						case Key.Y:
							camera.RotateY(-camera.turnAngle * (float) shiftFactor);
							break;
						default:
							camera.RotateZ(-camera.turnAngle * (float) shiftFactor);
							break;
					}
				}
			}
			if (keyState.IsKeyDown(keyMap["yawLeft"]))
			{
				if (axisRotation == Key.Unknown)
				{
					if (firstPerson)
						camera.Yaw((float) (moveFactor * keyboardConstant));
					else
						camera.ArcBallYaw((float) (moveFactor * keyboardConstant));
				}
				else
				{
					switch (axisRotation)
					{
						case Key.X:
							camera.RotateX(camera.turnAngle * (float) shiftFactor);
							break;
						case Key.Y:
							camera.RotateY(camera.turnAngle * (float) shiftFactor);
							break;
						default:
							camera.RotateZ(camera.turnAngle * (float) shiftFactor);
							break;
					}
				}
			}
			if (keyState.IsKeyDown(keyMap["rollLeft"]))
				camera.Roll((float) (moveFactor * keyboardConstant));
			if (keyState.IsKeyDown(keyMap["rollRight"]))
				camera.Roll((float) -(moveFactor * keyboardConstant));
			
			// Fractal Maximum Iteration change
			if (keyState.IsKeyDown(keyMap["increaseMaxIteration"]))
				foreach (Plot p in objects)
					if (p.plotType == PlotType.STRANGE_ATTRACTOR)
						((StrangeAttractor)p).maxIteration += (int) (Math.Ceiling(valueShift) * shiftFactor);
			if (keyState.IsKeyDown(keyMap["decreaseMaxIteration"]))
				foreach (Plot p in objects)
					if (p.plotType == PlotType.STRANGE_ATTRACTOR)
						((StrangeAttractor)p).maxIteration -= (int) (Math.Ceiling(valueShift) * shiftFactor);
			
			foreach (Plot p in objects)
				if (p.plotType == PlotType.STRANGE_ATTRACTOR)
					((StrangeAttractor)p).maxIteration = Math.Max(1, ((StrangeAttractor)p).maxIteration);
			
			// FourthVar
			if (keyState.IsKeyDown(keyMap["increaseVar4D"]))
				var4D += valueShift * shiftFactor;
			if (keyState.IsKeyDown(keyMap["decreaseVar4D"]))
			    var4D -= valueShift * shiftFactor;
			
			// A,B,C,D,E -> 1,2,3,4,5
			if (keyState.IsKeyDown(keyMap["parameterT"]))
				foreach (Plot p in objects)
					p.t += valueShift * shiftFactor / 1000;
			if (keyState.IsKeyDown(keyMap["parameterA"]))
				foreach (Plot p in objects)
					p.a += valueShift * shiftFactor;
			if (keyState.IsKeyDown(keyMap["parameterB"]))
				foreach (Plot p in objects)
					p.b += valueShift * shiftFactor;
			if (keyState.IsKeyDown(keyMap["parameterC"]))
				foreach (Plot p in objects)
					p.c += valueShift * shiftFactor;
			if (keyState.IsKeyDown(keyMap["parameterD"]))
				foreach (Plot p in objects)
					p.d += valueShift * shiftFactor;
			if (keyState.IsKeyDown(keyMap["parameterE"]))
				foreach (Plot p in objects)
					p.e += valueShift * shiftFactor;
			if (keyState.IsKeyDown(keyMap["parameterF"]))
				foreach (Plot p in objects)
					p.f += valueShift * shiftFactor;
			
			// Transparency
			if (keyState.IsKeyDown(keyMap["increaseTransparency"]) && alpha <= 1.0)
				alpha += .01f;
			if (keyState.IsKeyDown(keyMap["decreaseTransparency"]) && alpha >= 0)
				alpha -= .01f;
			
			lastKeyState = keyState;
		}
		
		// Mouse Controller
		void MouseController()
		{
			var keyState = Keyboard.GetState();
			bool shift = keyState.IsKeyDown(Key.ShiftLeft) || keyState.IsKeyDown(Key.ShiftRight);
			bool ctrl = keyState.IsKeyDown(Key.ControlLeft) || keyState.IsKeyDown(Key.ControlRight);
			bool alt = keyState.IsKeyDown(Key.AltLeft) || keyState.IsKeyDown(Key.AltRight);
			
			double shiftFactor = (shift ? (alt ? 100 : 10) : 1) * (ctrl ? (alt ? .01 : .1) : 1);
			
			var mousePos = new Vector2(OpenTK.Input.Mouse.GetState().X, OpenTK.Input.Mouse.GetState().Y);
			var scrollPos = Mouse.GetCursorState().WheelPrecise;
			
			if (Mouse.GetCursorState().IsButtonDown(MouseButton.Right) && onScreen)
			{
				var delta = lastMousePos - mousePos;
				
				if (shift)
				{
					camera.MoveAlongAxis(camera.horizontalAxis, delta.X);
					camera.MoveAlongAxis(camera.verticalAxis, delta.Y);
				}
				else
				{
					if (axisRotation == Key.Unknown)
					{
						if (firstPerson)
						{
							camera.Yaw(delta.X);
							camera.Pitch(-delta.Y);
						}
						else
						{
							camera.ArcBallYaw(delta.X);
							camera.ArcBallPitch(-delta.Y);
						}
					}
					else
					{
						switch (axisRotation)
						{
							case Key.X:
								camera.RotateX((delta.X - delta.Y));
								break;
							case Key.Y:
								camera.RotateY((delta.X - delta.Y));
								break;
							default:
								camera.RotateZ((delta.X - delta.Y));
								break;
						}
					}
					
					ResetCursor();
				}
			}
			
			if (onScreen)
			{
				//camera.ZoomTarget(camera.target, (float) shiftFactor * (scrollPos - lastScrollPos));
				/*
				camera.Zoom(scrollPos - lastScrollPos);
				
				camera.UpdateTargetDistance();
				float targetDistance = camera.targetDistance;
				
				camera.Zoom(-(scrollPos - lastScrollPos));
				camera.ZoomTarget(GetWorldRay(Mouse.X, Mouse.Y), scrollPos - lastScrollPos, false);
				
				camera.target = Camera.MoveAlongAxis(camera.position, camera.direction, targetDistance);
				*/
				
				camera.ZoomTarget(GetWorldRay(Mouse.GetCursorState().X, Mouse.GetCursorState().Y), scrollPos - lastScrollPos, false);
			}
			
			lastMousePos = mousePos;
			lastScrollPos = scrollPos;
			
			CursorVisible = !onScreen || !Mouse.GetCursorState().IsButtonDown(MouseButton.Right);
		}
		
		// Draws objects to screen
		void Draw()
		{
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			
			Matrix4 modelview = camera.GetViewMatrix();
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadMatrix(ref modelview);
			
			camera.DisplayTarget(5, 50 * time, targetOn);
			Plot.ShowAxis(axisRotation == Key.X, axisRotation == Key.Y, axisRotation == Key.Z, 5, axisOn);
			
			foreach (Plot p in objects)
			{
				p.Generate();
				
				p.Show(objectSize, alpha);
			}
			
			SwapBuffers();
			GL.Finish();
		}
		
		// Returns world ray at window coordinate
		public Vector3 GetWorldRay(int x, int y)
        {
            var t = new float[1];
            var viewport = new int[4];
            
            Matrix4 modelviewMatrix, projectionMatrix;
            GL.GetFloat(GetPName.ModelviewMatrix, out modelviewMatrix);
            GL.GetFloat(GetPName.ProjectionMatrix, out projectionMatrix);
            GL.GetInteger(GetPName.Viewport, viewport);
            
            GL.ReadPixels(x, Height - y, 1, 1, PixelFormat.DepthComponent, PixelType.Float, t);

            return UnProject(new Vector3(x, viewport[3] - y, t[0]), modelviewMatrix, projectionMatrix, viewport);
        }
        static Vector3 UnProject(Vector3 screen, Matrix4 view, Matrix4 projection, IList<int> viewPort)
        {
            var pos = new Vector4();

            pos.X = (screen.X - viewPort[0]) / viewPort[2] * 2.0f - 1.0f;
            pos.Y = (screen.Y - viewPort[1]) / viewPort[3] * 2.0f - 1.0f;
            pos.Z = screen.Z * 2.0f - 1.0f;
            pos.W = 1.0f;

            var pos2 = Vector4.Transform(pos, Matrix4.Invert(Matrix4.Mult(view, projection)));
            var posOut = new Vector3(pos2.X, pos2.Y, pos2.Z);

            return posOut / pos2.W;
        }
		
        // Finds the intersection point between a vector and a plane
        public static Vector3 IntersectVectorWithPlane(Vector3 vectorPos, Vector3 vectorDir, Vector3 planePos, Vector3 planeNorm)
        {
        	float t = (planeNorm.X*(planePos.X-vectorPos.X) + planeNorm.Y*(planePos.Y-vectorPos.Y) + planeNorm.Z*(planePos.Z-vectorPos.Z)) / (planeNorm.X*vectorDir.X + planeNorm.Y*vectorDir.Y + planeNorm.Z*vectorDir.Z);
        	
        	return vectorPos + vectorDir*t;
        }
        
		// 1
		// Called after an OpenGL context has been established,
		// but before entering the main loop
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			
			// Initializes program and loads shaders
			InitProgram();
			MapKeys();
			
			// Changing title of window
			Title = "PlotEquation";
			
			// Makes initial window maximized
			//WindowState = WindowState.Maximized;
			
			// Resets the color of the window to a single color
			GL.ClearColor(Color.Black);
			
			// Specifies the diameter of rastered points
			GL.PointSize(1f);
			
			// Enable Depth
			GL.Enable(EnableCap.DepthTest);
			
			// Enable point smoothing
			GL.Enable(EnableCap.PointSmooth);
			
			// Enable Lighting
			GL.Enable(EnableCap.Lighting);
			GL.Enable(EnableCap.ColorMaterial);
			
			// Enable Transparency
			GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
			GL.Enable(EnableCap.Blend);
			
			// Lighting
			float[] lightPos = {1,1,1};
			float[] lightDiffuse = {1.0f, 1.0f, 1.0f};
			float[] lightAmbient = {0.0f, 0.0f, 0.0f};
			GL.Light(LightName.Light0, LightParameter.Position, lightPos);
			GL.Light(LightName.Light0, LightParameter.Diffuse, lightDiffuse);
			GL.Light(LightName.Light0, LightParameter.Ambient, lightAmbient);
			
			GL.Enable(EnableCap.Light0);
		}
		
		// 2
		// Called when the window is resized
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			
			// Default: 0, 0, 980, 1280
			GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
			//Console.WriteLine(camera.targetDistance);
			Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float) (camera.fovAngle * Math.PI / 180), Width / (float) Height, 0.0001f, camera.targetDistance * 10);
			
			GL.MatrixMode(MatrixMode.Projection);
			
			GL.LoadMatrix(ref projection);
			
			Draw();
		}
		
		// 3
		// Called when frame is updated
		protected override void OnUpdateFrame(FrameEventArgs e)
		{
			base.OnUpdateFrame(e);
			
			if (fullScreen)
				WindowState = WindowState.Fullscreen;
			
			time += (float) valueShift;
			
			foreach (Plot p in objects)
			{
				p.color = ColorPalette.ColorFromHSV(5*time,.8,1);
				
				if (p.plotType == PlotType.STANDARD_EQUATION)
					((Equation)p).UpdateFourthVar(var4D);
			}
			
			if (Focused)
			{
				KeyboardController();
				MouseController();
			}
		}
		
		// 4
		// Called when the frame is rendered
		protected override void OnRenderFrame(FrameEventArgs e)
		{
			base.OnRenderFrame(e);
			//Exit();
			Draw();
		}
		
		// Occurs whenever a keyboard key is pressed
		protected override void OnKeyDown(KeyboardKeyEventArgs e)
		{
			base.OnKeyDown(e);
			
			if (e.Key == keyMap["fullScreen"])
			{
				fullScreen = !fullScreen;
			}
			if (e.Key == keyMap["cameraMode"])
			{
				firstPerson = !firstPerson;
			}
			if (e.Key == keyMap["toggleAxis"])
			{
				axisOn = !axisOn;
			}
			if (e.Key == keyMap["toggleTarget"])
			{
				targetOn = !targetOn;
			}
			if (e.Key == keyMap["displayInfo"])
			{
				Console.WriteLine(camera);
				
				foreach (Plot p in objects)
				{
					Console.WriteLine(p);
				}
			}
			if (e.Key == keyMap["toggle3D"])
			{
				foreach (Plot p in objects)
				{
					p.is3D = !p.is3D;
				}
			}
			if (e.Key == keyMap["toggleLighting"])
			{
				if (GL.IsEnabled(EnableCap.Lighting))
					GL.Disable(EnableCap.Lighting);
				else
					GL.Enable(EnableCap.Lighting);
			}
			if (e.Key == keyMap["increaseObjectSize"])
			{
				objectSize += .5f;
				
				if (objectSize >= 20)
					objectSize = 20f;
			}
			if (e.Key == keyMap["decreaseObjectSize"])
			{
				objectSize -= .5f;
				
				if (objectSize <= 1)
					objectSize = 1f;
			}
			if (e.Key == keyMap["rotateAroundX"])
			{
				axisRotation = axisRotation == Key.X ? Key.Unknown : Key.X;
			}
			if (e.Key == keyMap["rotateAroundY"])
			{
				axisRotation = axisRotation == Key.Y ? Key.Unknown : Key.Y;
			}
			if (e.Key == keyMap["rotateAroundZ"])
			{
				axisRotation = axisRotation == Key.Z ? Key.Unknown : Key.Z;
			}
			if (e.Key == keyMap["togglePointDisplay"])
			{
				foreach (Plot p in objects)
					if (!p.currentDisplay.Contains(DisplayType.POINTS))
						p.currentDisplay.Add(DisplayType.POINTS);
					else
						p.currentDisplay.Remove(DisplayType.POINTS);
			}
			if (e.Key == keyMap["toggleLineDisplay"])
			{
				foreach (Plot p in objects)
					if (!p.currentDisplay.Contains(DisplayType.LINES))
						p.currentDisplay.Add(DisplayType.LINES);
					else
						p.currentDisplay.Remove(DisplayType.LINES);
			}
			if (e.Key == keyMap["toggleTriangleDisplay"])
			{
				foreach (Plot p in objects)
					if (!p.currentDisplay.Contains(DisplayType.TRIANGLES))
						p.currentDisplay.Add(DisplayType.TRIANGLES);
					else
						p.currentDisplay.Remove(DisplayType.TRIANGLES);
			}
			if (e.Key == keyMap["toggleQuadDisplay"])
			{
				foreach (Plot p in objects)
					if (!p.currentDisplay.Contains(DisplayType.QUADS))
						p.currentDisplay.Add(DisplayType.QUADS);
					else
						p.currentDisplay.Remove(DisplayType.QUADS);
			}
		}
		
		// Called whenever the mouse cursor leaves the window bounds
		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			
			onScreen = false;
		}
		
		// Called whenever the mouse cursor reenters the window bounds
		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter(e);
			
			onScreen = true;
		}
	}
}
