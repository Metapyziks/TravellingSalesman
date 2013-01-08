using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using OpenTKTools;

namespace Visualiser
{
    class VisualiserWindow : GameWindow
    {
        private SpriteShader _spriteShader;

        public PositionalGraph Graph { get; set; }

        public VisualiserWindow(int width, int height)
            : base(width, height)
        {
            Title = "Travelling Salesman Visualiser";
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(Color4.CornflowerBlue);

            _spriteShader = new SpriteShader(Width, Height);

            Mouse.ButtonDown += (sender, me) => {
                if (Graph != null) {
                    Graph.SelectedVertex = Graph.GetNearestVertexScreen(new Vector2(me.X, me.Y));
                }
            };

            Mouse.ButtonUp += (sender, me) => {
                if (Graph != null) {
                    Graph.DeselectVertex();
                }
            };

            Mouse.Move += (sender, me) => {
                if (Graph != null && Graph.SelectedVertex != -1) {
                    Graph.MoveVertexScreen(Graph.SelectedVertex, new Vector2(me.X, me.Y));
                }
            };
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (Graph != null) {
                Graph.Stablize();
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            _spriteShader.Begin();
            if (Graph != null)
                Graph.Render(_spriteShader);
            _spriteShader.End();

            SwapBuffers();
        }
    }
}
