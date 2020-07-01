#version 450

layout(location = 0) in vec3 vertexPos;
layout(location = 1) in vec3 vertexColor;

out vec3 fragmentColor;

void main() {
  fragmentColor = vertexColor;
  gl_Position = vec4(vertexPos, 1);
}