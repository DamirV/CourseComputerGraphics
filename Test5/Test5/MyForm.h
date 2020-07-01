#pragma once
#include <math.h>
#define PI 3.1415926535897932384626433832795

namespace Test5 {

	using namespace System;
	using namespace System::ComponentModel;
	using namespace System::Collections;
	using namespace System::Windows::Forms;
	using namespace System::Data;
	using namespace System::Drawing;
	using namespace OpenTK::Platform::Windows;
	using namespace OpenTK::Graphics;
	using namespace OpenTK;

	struct TPoint
	{
		float x, y, z;
	};

	public ref class MyForm : public System::Windows::Forms::Form
	{
	private:
		bool glload = false;
		float distance = 10;
		bool mouseDown;
		int mouseDownX;
		int mouseDownY;
		int mouseX;
		int mouseY;
		int angleX, angleY;
		int angleDownX, angleDownY;
		const float grad = (2 * PI ) / 360;
		String^ glVersion;
		String^ glsVersion;

		int BasicProgramID;
		int BasicVertexShader;
		int BasicFragmentShader;
		void TAngle(float &x, float &y, int u ) {
			float dx;
			float dy;
			float du = grad*(float)u;
			dx = x * cos(du) - y * sin(du);
			dy = x * sin(du) + y * cos(du);
			x = dx;
			y = -dy;
		}
	public:
		MyForm(void)
		{
			InitializeComponent();
			//
			//TODO: Add the constructor code here
			//
		}
	protected:
		~MyForm()
		{
			if (components)
			{
				delete components;
			}
		}
	private: OpenTK::GLControl^  glControl1;
	private: System::Windows::Forms::Timer^  timer1;
	private: System::ComponentModel::IContainer^  components;


#pragma region Windows Form Designer generated code
			 /// <summary>
			 /// Required method for Designer support - do not modify
			 /// the contents of this method with the code editor.
			 /// </summary>
			 void InitializeComponent(void)
			 {
				 this->components = (gcnew System::ComponentModel::Container());
				 this->glControl1 = (gcnew OpenTK::GLControl());
				 this->timer1 = (gcnew System::Windows::Forms::Timer(this->components));
				 this->SuspendLayout();
				 // 
				 // glControl1
				 // 
				 this->glControl1->BackColor = System::Drawing::Color::Black;
				 this->glControl1->Location = System::Drawing::Point(0, 0);
				 this->glControl1->Name = L"glControl1";
				 this->glControl1->Size = System::Drawing::Size(500, 500);
				 this->glControl1->TabIndex = 0;
				 this->glControl1->VSync = false;
				 this->glControl1->Load += gcnew System::EventHandler(this, &MyForm::glControl1_Load);
				 this->glControl1->MouseDown += gcnew System::Windows::Forms::MouseEventHandler(this, &MyForm::glControl1_MouseDown);
				 this->glControl1->MouseMove += gcnew System::Windows::Forms::MouseEventHandler(this, &MyForm::glControl1_MouseMove);
				 this->glControl1->MouseUp += gcnew System::Windows::Forms::MouseEventHandler(this, &MyForm::glControl1_MouseUp);
				 this->glControl1->MouseWheel += gcnew System::Windows::Forms::MouseEventHandler(this, &MyForm::glControl1_MouseWheel);
				 // 
				 // timer1
				 // 
				 this->timer1->Enabled = true;
				 this->timer1->Interval = 16;
				 this->timer1->Tick += gcnew System::EventHandler(this, &MyForm::timer1_Tick);
				 // 
				 // MyForm
				 // 
				 this->AutoScaleDimensions = System::Drawing::SizeF(6, 13);
				 this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
				 this->ClientSize = System::Drawing::Size(501, 501);
				 this->Controls->Add(this->glControl1);
				 this->Name = L"MyForm";
				 this->Text = L"MyForm";
				 this->ResumeLayout(false);

			 }
#pragma endregion
	private: System::Void glControl1_Load(System::Object^  sender, System::EventArgs^  e) {
		
		glload = true;
		glVersion = GL::GetString(StringName::Version);
		glsVersion = GL::GetString(StringName::ShadingLanguageVersion);
	}

	private: System::Void glControl1_MouseMove(System::Object^  sender, System::Windows::Forms::MouseEventArgs^  e) {
		mouseX = e->X;
		mouseY = e->Y;
		Text = " X:" + (mouseX)+" Y:" + (mouseY)+" " + (angleX)+" " + (angleY);

		if (mouseDown) {
			angleX = angleDownX + (mouseX - mouseDownX);
			angleY = angleDownY + (mouseY - mouseDownY);
		}
	}

	private: System::Void glControl1_MouseWheel(System::Object^  sender, System::Windows::Forms::MouseEventArgs^  e) {
		Text = " Wheel:" + (distance);
		if (e->Delta < 0)
		{
			distance--;
		}
		if (e->Delta > 0)
		{
			distance++;
		}
	}
	
			 private: bool Load(const String^ vertexShaderFile, const String^ fragmentShaderFile) {
		
				 return false;
			 }
	private: void Render() {
		if (glload)
		{
			GL::MatrixMode(MatrixMode::Projection);
			GL::Ortho(-1, 1, -1, 1, -1, 1); //система координат
			GL::Viewport(0, 0, glControl1->Width, glControl1->Height);
			GL::Clear(ClearBufferMask::ColorBufferBit | ClearBufferMask::DepthBufferBit);
			GL::LoadIdentity();

			Matrix4 perspective = Matrix4::CreatePerspectiveFieldOfView(0.5f, glControl1->Width / glControl1->Height, 1, 100);
			GL::LoadMatrix(perspective); //загрузка настройки проекции
			GL::ClearColor(Color::Black);
			TPoint point[24];

			
			point[0].x = -0.5f; point[0].y = -0.5f; point[0].z = 0.5f; 
			point[1].x = 0.5f; point[1].y = -0.5f; point[1].z = 0.5f;
			point[2].x = 0.5f; point[2].y = 0.5f; point[2].z = 0.5f;
			point[3].x = -0.5f; point[3].y = 0.5f; point[3].z = 0.5f;

			point[4].x = -0.5f; point[4].y = -0.5f; point[4].z = -0.5f;
			point[5].x = -0.5f; point[5].y = 0.5f; point[5].z = -0.5f;
			point[6].x = 0.5f; point[6].y = 0.5f; point[6].z = -0.5f;
			point[7].x = 0.5f; point[7].y = -0.5f; point[7].z = -0.5f;

			point[8].x = -0.5f; point[8].y = -0.5f; point[8].z = -0.5f;
			point[9].x = -0.5f; point[9].y = -0.5f; point[9].z = 0.5f;
			point[10].x = -0.5f; point[10].y = 0.5f; point[10].z = 0.5f;
			point[11].x = -0.5f; point[11].y = 0.5f; point[11].z = -0.5f;
			

			point[12].x = 0.5f; point[12].y = -0.5f; point[12].z = -0.5f;
			point[13].x = 0.5f; point[13].y = 0.5f; point[13].z = -0.5f;
			point[14].x = 0.5f; point[14].y = 0.5f; point[14].z = 0.5f;
			point[15].x = 0.5f; point[15].y = -0.5f; point[15].z = 0.5f;

			point[16].x = 0.5f; point[16].y = 0.5f; point[16].z = -0.5f;
			point[17].x = -0.5f; point[17].y = 0.5f; point[17].z = -0.5f;
			point[18].x = -0.5f; point[18].y = 0.5f; point[18].z = 0.5f;
			point[19].x = 0.5f; point[19].y = 0.5f; point[19].z = 0.5f;

			point[20].x = -0.5f; point[20].y = -0.5f; point[20].z = -0.5f;
			point[21].x = 0.5f; point[21].y = -0.5f; point[21].z = -0.5f;
			point[22].x = 0.5f; point[22].y = -0.5f; point[22].z = 0.5f;
			point[23].x = -0.5f; point[23].y = -0.5f; point[23].z = 0.5f;

			
			for (int i = 0; i < 24; i++)
			{
					//point[i].z -= distance;
				TAngle(point[i].y, point[i].z, angleY);
				TAngle(point[i].x, point[i].z, angleX);
				point[i].z -= distance;
			}

			
			GL::Begin(BeginMode::Polygon);
			GL::Color3(Color::Yellow); 
			for (int i = 0; i < 4; i++)
			{
				GL::Vertex3(point[i].x, point[i].y, point[i].z);
			}
			GL::End();

			
			GL::Begin(BeginMode::Polygon);
			GL::Color3(Color::Blue); 
			for (int i = 4; i < 8; i++)
			{
				GL::Vertex3(point[i].x, point[i].y, point[i].z);
			}
			GL::End();
			

			
			GL::Begin(BeginMode::Polygon);
			GL::Color3(Color::White); 
			for (int i = 8; i < 12; i++)
			{
				GL::Vertex3(point[i].x, point[i].y, point[i].z);
			}
			GL::End();

			
			GL::Begin(BeginMode::Polygon);
			GL::Color3(Color::Green); 
			for (int i = 12; i < 16; i++)
			{
				GL::Vertex3(point[i].x, point[i].y, point[i].z);
			}
			GL::End();

			
			GL::Begin(BeginMode::Polygon);
			GL::Color3(Color::Red); 
			for (int i = 16; i < 20; i++)
			{
				GL::Vertex3(point[i].x, point[i].y, point[i].z);
			}
			GL::End();

			
			GL::Begin(BeginMode::Polygon);
			GL::Color3(Color::Orange); 
			for (int i = 20; i < 24; i++)
			{
				GL::Vertex3(point[i].x, point[i].y, point[i].z);
			}
			GL::End();

			GL::Enable(EnableCap::CullFace);
			//GL::Flush();
			glControl1->SwapBuffers();
			glControl1->Invalidate();
			

		}
	}

	private: System::Void timer1_Tick(System::Object^  sender, System::EventArgs^  e) {
		Render();
	}
private: System::Void glControl1_MouseDown(System::Object^  sender, System::Windows::Forms::MouseEventArgs^  e) {
	mouseDown = true;
	mouseDownX = e->X;
	mouseDownY = e->Y;
	angleDownX = angleX;
	angleDownY = angleY;
}
private: System::Void glControl1_MouseUp(System::Object^  sender, System::Windows::Forms::MouseEventArgs^  e) {
	mouseDown = false;
}

};

}
