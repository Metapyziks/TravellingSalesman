using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using OpenTKTools;

using TravellingSalesman;

namespace Visualiser
{
    class PositionalGraph : Graph
    {
        public new static PositionalGraph FromFile( String path )
        {
            return new PositionalGraph( File.ReadAllText( path ) );
        }

        private Sprite _sNodeSprite;
        private Sprite _sConnSprite;

        public float Friction { get; set; }
        public float ForceMul { get; set; }

        public float Scale { get; set; }

        public Vector2 CameraPos { get; set; }

        private void LoadSprites()
        {
            if ( _sNodeSprite != null && _sConnSprite != null )
                return;

            var nodeTex = new BitmapTexture2D( Properties.Resources.Circle,
                TextureMinFilter.LinearMipmapLinear, TextureMagFilter.Linear );
            var connTex = new BitmapTexture2D( Properties.Resources.Connection,
                TextureMinFilter.LinearMipmapLinear, TextureMagFilter.Linear );

            _sNodeSprite = new Sprite( nodeTex, Scale ) { UseCentreAsOrigin = true };
            _sConnSprite = new Sprite( connTex ) { UseCentreAsOrigin = true };
        }

        private Vector2[] _positions;
        private int[,] _ordered;

        protected PositionalGraph( String data )
            : base( data )
        {
            Friction = .0000005f;
            ForceMul = 1f;

            LoadSprites();

            int[] orderBuffer = new int[ Count];
            for( int i = 0; i < Count; ++ i )
                orderBuffer[i] = i;

            _positions = new Vector2[Count];
            _ordered = new int[Count, Count];
            double radius = Math.PI * ( Count - 1 ) * 8f;
            for( int i = 0; i < Count; ++ i )
            {
                double ang = i * Math.PI * 2d / Count;
                _positions[i] = new Vector2(
                    (float) ( Math.Cos( ang ) * radius ),
                    (float) ( Math.Sin( ang ) * radius ) );

                Array.Sort( orderBuffer, Comparer<int>.Create( (x,y) => this[i,x] - this[i,y] ) );

                for ( int j = 0; j < Count; ++j )
                    _ordered[i, j] = orderBuffer[j];
            }
        }

        public void Stablize()
        {
            Vector2[] moves = new Vector2[Count];

            for ( int i = 0; i < Count; ++i )
            {
                for ( int j = 0; j < Count && j < 4; ++j )
                {
                    int k = _ordered[i, j];

                    if ( k == i )
                        continue;

                    Vector2 diff = _positions[k] - _positions[i];

                    float dest = this[i, k] * 16f;
                    float curr = diff.Length;

                    diff.Normalize();

                    moves[i] += diff * ( curr - dest ) * .5f / j;
                }
            }

            Vector2 min = new Vector2();
            Vector2 max = new Vector2();

            for ( int i = 0; i < Count; ++i )
            {
                _positions[i] += moves[i] * Friction;

                if ( _positions[i].X < min.X )
                    min.X = _positions[i].X;
                else if ( _positions[i].X > max.X )
                    max.X = _positions[i].X;

                if ( _positions[i].Y < min.Y )
                    min.Y = _positions[i].Y;
                else if ( _positions[i].Y > max.Y )
                    max.Y = _positions[i].Y;
            }

            CameraPos = ( max + min ) * .5f;
            Scale = Math.Min( 800 / ( max.X - min.X ), 600 / ( max.Y - min.Y ) );
        }

        public void Render( SpriteShader shader )
        {
            Vector2 centre = new Vector2( 400f, 300f );
            _sNodeSprite.Colour = new Color4( 127, 127, 127, 191 );
            _sNodeSprite.Scale = new Vector2( Scale * 4f * 1.05f );
            for ( int i = 0; i < Count; ++i )
            {
                _sNodeSprite.Position = ( ( _positions[i] - CameraPos ) * Scale ) + centre;
                _sNodeSprite.Render( shader );
            }

            _sNodeSprite.Colour = Color4.White;
            _sNodeSprite.Scale = new Vector2( Scale * 4f );
            for ( int i = 0; i < Count; ++i )
            {
                /*
                for ( int j = i + 1; j < Count; ++j )
                {
                    Vector2 diff = _positions[j] - _positions[i];
                    Vector2 mid = ( _positions[i] + _positions[j] ) * 0.5f;
                    float ang = (float) Math.Atan2( diff.Y, diff.X );
                    _sConnSprite.Position = ( ( mid - CameraPos ) * Scale ) + centre;
                    _sConnSprite.Width = Scale * diff.Length;
                    _sConnSprite.Height = 512f / this[i, j];
                    _sConnSprite.Rotation = ang;
                    _sConnSprite.Render( shader );
                }
                */

                _sNodeSprite.Position = ( ( _positions[i] - CameraPos ) * Scale ) + centre;
                _sNodeSprite.Render( shader );
            }
        }
    }
}
