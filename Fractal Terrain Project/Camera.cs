using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Fractal_Terrain_Project
{
	/// <summary>
	/// Description of Camera.
	/// </summary>
	public class Camera
	{
		public Vector3 position = Vector3.Zero;
		public Vector3 target = Vector3.Zero;
		public Vector3 direction = new Vector3(0, 0, 1);
		public Vector3 horizontalAxis = new Vector3(1, 0, 0);
		public Vector3 verticalAxis = new Vector3(0, 1, 0);
		public Vector3 depthAxis = new Vector3(0, 0, 1);
		public float fovAngle = 57;
		public float moveSpeed = .2f;
		public float zoomSpeed = .2f;
		public float turnAngle = 1f;
		
		public float targetDistance;
		
		public Camera(Vector3 pos, Vector3 dir, Vector3 hAxis, Vector3 vAxis, Vector3 dAxis)
		{
			position = pos;
			direction = dir;
			horizontalAxis = hAxis;
			verticalAxis = vAxis;
			depthAxis = dAxis;
			
			UpdateTargetDistance();
		}
		public Camera(Vector3 pos, Vector3 dir)
		{
			position = pos;
			direction = dir;
			
			UpdateTargetDistance();
		}
		public Camera() {UpdateTargetDistance();}
		
		public override string ToString()
		{
			return "Camera:\n\tPosition: " + position + "\n\tDirection: " + direction + "\n\tRotation target: " + target + "\n\tHorizontal Axis: " + horizontalAxis + "\n\tVertical Axis: " + verticalAxis + "\n\tDepth Axis: " + depthAxis + "\n";
		}
		
		public void DisplayTarget(float width, float rotateAngle, bool display = true)
		{
			if (display)
			{
				float length = targetDistance / 20f;
				var rotateAxis = new Vector3(1,1,1).Normalized();
				
				var hAxis = Rotate(new Vector3(length, 0, 0), rotateAxis, rotateAngle);
				var vAxis = Rotate(new Vector3(0, length, 0), rotateAxis, rotateAngle);
				var dAxis = Rotate(new Vector3(0, 0, length), rotateAxis, rotateAngle);
				
				GL.LineWidth(width);
				
				GL.Begin(PrimitiveType.Lines);
				{
					GL.Color3(Color.White);
					/*
					GL.Vertex3(MoveAlongAxis(target, horizontalAxis, -length));
					GL.Vertex3(MoveAlongAxis(target, horizontalAxis, length));
					
					GL.Vertex3(MoveAlongAxis(target, verticalAxis, -length));
					GL.Vertex3(MoveAlongAxis(target, verticalAxis, length));
					
					GL.Vertex3(MoveAlongAxis(target, horizontalAxis, -length));
					GL.Vertex3(MoveAlongAxis(target, horizontalAxis, length));
					*/
					GL.Vertex3(target + hAxis);
					GL.Vertex3(target - hAxis);
					
					GL.Vertex3(target + vAxis);
					GL.Vertex3(target - vAxis);
					
					GL.Vertex3(target + dAxis);
					GL.Vertex3(target - dAxis);
				}
				GL.End();
			}
		}
		
		public Matrix4 GetViewMatrix()
		{
		    return Matrix4.LookAt(position, position + direction, verticalAxis);
		}
		
		public void MoveAlongAxis(Vector3 axis, bool changeTarget = true)
		{
			MoveAlongAxis(axis, 1, changeTarget);
		}
		public void MoveAlongAxis(Vector3 axis, float distance, bool changeTarget = true)
		{
			UpdateTargetDistance();
			position += axis * distance * moveSpeed;
			
			if (changeTarget)
				ChangeTarget();
		}
		public static Vector3 MoveAlongAxis(Vector3 position, Vector3 axis, float distance)
		{
			return position + axis * distance;
		}
		
		public float GetRadius()
		{
			return (float) Math.Sqrt(position.X*position.X + position.Y*position.Y + position.Z*position.Z);
		}
		
		public void MoveForward(bool changeTarget = true)
		{
			MoveForward(1, changeTarget);
		}
		public void MoveForward(float distance, bool changeTarget = true)
		{
			UpdateTargetDistance();
			position += direction * distance * moveSpeed;
			
			if (changeTarget)
				ChangeTarget();
		}
		
		public void Zoom(float distance = 1)
		{
			MoveForward(zoomSpeed * distance * targetDistance, false);
		}
		
		public void ZoomAlongAxis(Vector3 axis, float distance)
		{
			MoveAlongAxis(axis, zoomSpeed * distance * targetDistance, false);
		}
		
		public void ZoomTarget(Vector3 target, float distance, bool changeTarget = false)
		{
			UpdateTargetDistance();
			MoveAlongAxis((target - position).Normalized(), zoomSpeed * distance * targetDistance, false);
			
			if (changeTarget)
				ChangeTarget();
		}
		
		public void RotateX(double angle, bool changeTarget = true)
		{
			UpdateTargetDistance();
			ArcBall(new Vector3(1,0,0), angle);
			
			if (changeTarget)
				ChangeTarget();
		}
		
		public void RotateY(double angle, bool changeTarget = true)
		{
			UpdateTargetDistance();
			ArcBall(new Vector3(0,1,0), angle);
			
			if (changeTarget)
				ChangeTarget();
		}
		
		public void RotateZ(double angle, bool changeTarget = true)
		{
			UpdateTargetDistance();
			ArcBall(new Vector3(0,0,1), angle);
			
			if (changeTarget)
				ChangeTarget();
		}
		
		public void Yaw(double angle, bool changeTarget = true)
		{
			UpdateTargetDistance();
			RotateAxes(Quaternion.FromAxisAngle(turnAngle * angle * Math.PI / 360, new Vector3D(verticalAxis)));
			
			if (changeTarget)
				ChangeTarget();
		}
		
		public void Pitch(double angle, bool changeTarget = true)
		{
			UpdateTargetDistance();
			RotateAxes(Quaternion.FromAxisAngle(turnAngle * angle * Math.PI / 360, new Vector3D(horizontalAxis)));
			
			if (changeTarget)
				ChangeTarget();
		}
		
		public void Roll(double angle, bool changeTarget = true)
		{
			UpdateTargetDistance();
			RotateAxes(Quaternion.FromAxisAngle(turnAngle * angle * Math.PI / 360, new Vector3D(depthAxis)));
			
			if (changeTarget)
				ChangeTarget();
		}
		
		public void ArcBallYaw(double angle, bool aroundOrigin = false)
		{
			position -= (aroundOrigin ? new Vector3(0) : target);
			
			RotateAxes(Quaternion.FromAxisAngle(turnAngle * angle * Math.PI / 360, new Vector3D(verticalAxis)));
			
			position = (depthAxis * -GetRadius()) + (aroundOrigin ? new Vector3(0) : target);
		}
		
		public void ArcBallPitch(double angle, bool aroundOrigin = false)
		{
			position -= (aroundOrigin ? new Vector3(0) : target);
			
			RotateAxes(Quaternion.FromAxisAngle(turnAngle * angle * Math.PI / 360, new Vector3D(horizontalAxis)));
			
			position = (depthAxis * -GetRadius()) + (aroundOrigin ? new Vector3(0) : target);
		}
		
		public void ArcBall(Vector3 axis, double angle, bool aroundOrigin = true)
		{
			RotateAxes(Quaternion.FromAxisAngle(turnAngle * angle * Math.PI / 360, new Vector3D(axis)));
			
			position = (depthAxis * -GetRadius()) + (aroundOrigin ? new Vector3(0) : target);
		}
		
		public static Vector3 Rotate(Vector3 position, Vector3 axis, double angle)
		{
			Quaternion localRotation = Quaternion.FromAxisAngle(angle * Math.PI / 360, new Vector3D(axis));
			
			Vector3D v = localRotation.Inverse() * (localRotation * new Vector3D(position));
			
			position.X = (float) v.X;
			position.Y = (float) v.Y;
			position.Z = (float) v.Z;
			
			return position;
		}
		
		public void UpdateTargetDistance()
		{
			targetDistance = (float) Vector3D.DistanceBetween(new Vector3D(position), new Vector3D(target));
		}
		public void UpdateTargetDistance(Vector3 target)
		{
			targetDistance = (float) Vector3D.DistanceBetween(new Vector3D(position), new Vector3D(target));
		}
		
		void ChangeTarget()
		{
			target = MoveAlongAxis(position, direction, targetDistance);
		}
		
		void RotateAxes(Quaternion localRotation)
		{
			var hAxis = new Vector3D(horizontalAxis);
			var vAxis = new Vector3D(verticalAxis);
			var dir = new Vector3D(direction);
			
			ApplyRotationToVector(localRotation, ref hAxis);
			ApplyRotationToVector(localRotation, ref vAxis);
			ApplyRotationToVector(localRotation, ref dir);
			
			horizontalAxis.X = (float) hAxis.X;
			horizontalAxis.Y = (float) hAxis.Y;
			horizontalAxis.Z = (float) hAxis.Z;
			
			verticalAxis.X = (float) vAxis.X;
			verticalAxis.Y = (float) vAxis.Y;
			verticalAxis.Z = (float) vAxis.Z;
			
			direction.X = (float) dir.X;
			direction.Y = (float) dir.Y;
			direction.Z = (float) dir.Z;
			
			depthAxis = direction;
		}
		
		void ApplyRotationToVector(Quaternion rotation, ref Vector3D v)
		{
			v = rotation.Inverse() * (rotation * v);
		}
	}
}
