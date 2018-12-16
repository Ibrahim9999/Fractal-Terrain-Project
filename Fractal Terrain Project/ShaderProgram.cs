using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace Fractal_Terrain_Project
{
	/// <summary>
	/// Description of ShaderProgram.
	/// </summary>
	public class ShaderProgram
	{
		public int programID = -1;
		public int vShaderID = -1;
		public int fShaderID = -1;
		public int attributeCount = 0;
		public int uniformCount = 0;
		
		public Dictionary<String, AttributeInfo> attributes = new Dictionary<string, AttributeInfo>();
		public Dictionary<String, UniformInfo> uniforms = new Dictionary<string, UniformInfo>();
		public Dictionary<String, uint> buffers = new Dictionary<string, uint>();
		
		public ShaderProgram(string vShader, string fShader, bool fromFile = false)
		{
			programID = GL.CreateProgram();
			
			if (fromFile)
			{
				LoadShaderFromFile(vShader, ShaderType.VertexShader);
				LoadShaderFromFile(fShader, ShaderType.FragmentShader);
			}
			else
			{
				LoadShaderFromString(vShader, ShaderType.VertexShader);
				LoadShaderFromString(fShader, ShaderType.FragmentShader);
			}
			
			Link();
			GenBuffers();
		}
		public ShaderProgram()
		{
			//programID = GL.CreateProgram();
		}
		
		void LoadShader(String code, ShaderType type, out int address)
		{
			address = GL.CreateShader(type);
			
			GL.ShaderSource(address, code);
			
			Console.WriteLine("Compiling " + type + " shader...");
			GL.CompileShader(address);
			
			GL.AttachShader(programID, address);
			
			Console.WriteLine(GL.GetShaderInfoLog(address));
			Console.WriteLine("Loaded " + type + ".");
		}
		
		// Loading shader from string
		public void LoadShaderFromString(string code, ShaderType type)
		{
			if (type == ShaderType.VertexShader)
			{
				LoadShader(code, type, out vShaderID);
			}
			else if (type == ShaderType.FragmentShader)
			{
				LoadShader(code, type, out fShaderID);
			}
		}
		
		// Loading shader from file
		public void LoadShaderFromFile(string filePath, ShaderType type)
		{
			using (var sr = new StreamReader(filePath))
			{
				LoadShaderFromString(sr.ReadToEnd(), type);
			}
		}
		
		// Linking shaders and assembling list of attributes and uniforms
		public void Link()
		{
			GL.LinkProgram(programID);
			
			Console.WriteLine(GL.GetProgramInfoLog(programID));
			Console.WriteLine("Loaded shaders.");
			
			GL.GetProgram(programID, ProgramParameter.ActiveAttributes, out attributeCount);
			GL.GetProgram(programID, ProgramParameter.ActiveUniforms, out uniformCount);
			
			for (int i = 0; i < attributeCount; i++)
			{
				var info = new AttributeInfo();
				var name = "";
				int length = 0;
				
				GL.GetActiveAttrib(programID, i, 256, out length, out info.size, out info.type, out name);
				
				info.name = name;
				info.address = GL.GetAttribLocation(programID, info.name);
				attributes.Add(name, info);
			}
			for (int i = 0; i < uniformCount; i++)
			{
				var info = new UniformInfo();
				var name = "";
				int length = 0;
				
				GL.GetActiveUniform(programID, i, 256, out length, out info.size, out info.type, out name);
				
				info.name = name;
				uniforms.Add(name, info);
				info.address = GL.GetUniformLocation(programID, info.name);
			}
		}
		
		// Generates buffer objects
		public void GenBuffers()
		{
			for (int i = 0; i < attributes.Count; i++)
			{
				uint buffer = 0;
				GL.GenBuffers(1, out buffer);
				
				buffers.Add(attributes.Values.ElementAt(i).name, buffer);
			}
			for (int i = 0; i < uniforms.Count; i++)
			{
				uint buffer = 0;
				GL.GenBuffers(1, out buffer);
				
				buffers.Add(uniforms.Values.ElementAt(i).name, buffer);
			}
		}
		
		// Enables vertex attribute arrays
		public void EnableVertexAttribArrays()
		{
			for (int i = 0; i < attributes.Count; i++)
			{
				GL.EnableVertexAttribArray(attributes.Values.ElementAt(i).address);
			}
		}
		
		// Disables vertex attribute arrays
		public void DisableVertexAttribArrays()
		{
			for (int i = 0; i < attributes.Count; i++)
			{
				GL.DisableVertexAttribArray(attributes.Values.ElementAt(i).address);
			}
		}
		
		// Retrieve address of attribute based on name
		public int GetAttribute(string name)
		{
			return attributes.ContainsKey(name) ? attributes[name].address : -1;
		}
		
		// Retrieve address of uniform based on name
		public int GetUniform(string name)
		{
			return uniforms.ContainsKey(name) ? uniforms[name].address : -1;
		}
		
		// Retrieve buffer based on name
		public uint GetBuffer(string name)
		{
			return buffers.ContainsKey(name) ? buffers[name] : 0;
		}
		
		
	}
	
	public class AttributeInfo
	{
		public String name = "";
		public int address = -1;
		public int size = 0;
		public ActiveAttribType type;
	}
	
	public class UniformInfo
	{
		public String name = "";
		public int address = -1;
		public int size = 0;
		public ActiveUniformType type;
	}
}
