#version 400

in vec3 vPos;
in vec3 vColor;

out vec4 color;

uniform mat4 modelView;

void main()
{
	gl_Position = modelView * vec4(vPos, 1.0);
	
	color = vec4(vColor, .9);
}