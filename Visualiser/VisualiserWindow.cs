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
        private BitmapTexture2D _circle;
        private BitmapTexture2D _connection;
        private Sprite[] _sprites;

        public VisualiserWindow( int width, int height )
            : base( width, height )
        {
            Title = "Travelling Salesman Visualiser";
        }

        protected override void OnLoad( EventArgs e )
        {
            GL.ClearColor( Color4.CornflowerBlue );

            _spriteShader = new SpriteShader( Width, Height );

            _sprites = new Sprite[]
            {
                new Sprite( _circle, 0.25f )
                {
                    UseCentreAsOrigin = true,
                    Position = new Vector2( 400f - 32f, 300f )
                },
                new Sprite( _circle, 0.25f )
                {
                    UseCentreAsOrigin = true,
                    Position = new Vector2( 400f + 32f, 300f )
                },
                new Sprite( _connection, 0.25f )
                {
                    UseCentreAsOrigin = true,
                    Position = new Vector2( 400f, 300f )
                }
            };
        }

        protected override void OnRenderFrame( FrameEventArgs e )
        {
            GL.Clear( ClearBufferMask.ColorBufferBit );

            _spriteShader.Begin();
            foreach( Sprite sprite in _sprites )
                sprite.Render( _spriteShader );
            _spriteShader.End();

            SwapBuffers();
        }
    }
}
