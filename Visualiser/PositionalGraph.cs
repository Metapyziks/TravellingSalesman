using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;

using OpenTKTools;

using TravellingSalesman;

namespace Visualiser
{
    class PositionalGraph : Graph
    {
        private static float _sScale;

        private static Sprite _sNodeSprite;
        private static Sprite _sConnSprite;

        public static float Scale
        {
            get { return _sScale; }
            set
            {
                _sScale = value;

                if ( _sNodeSprite != null && _sConnSprite != null )
                    _sConnSprite.Scale = _sNodeSprite.Scale = new Vector2( value, value );
            }
        }

        private static void LoadSprites()
        {
            if ( _sNodeSprite != null && _sConnSprite != null )
                return;

            if ( Scale == 0f )
                Scale = 0.125f;

            var nodeTex = new BitmapTexture2D( Properties.Resources.Circle,
                TextureMinFilter.LinearMipmapLinear, TextureMagFilter.Linear );
            var connTex = new BitmapTexture2D( Properties.Resources.Connection,
                TextureMinFilter.LinearMipmapLinear, TextureMagFilter.Linear );

            _sNodeSprite = new Sprite( nodeTex, Scale );
            _sConnSprite = new Sprite( connTex, Scale );
        }

        public static PositionalGraph FromFile( String path )
        {
            LoadSprites();

            return new PositionalGraph( File.ReadAllText( path ) );
        }

        private Vector2[] _positions;

        protected PositionalGraph( String data )
            : base( data )
        {
            _positions = new Vector2[Count];
            double radius = Math.PI * ( Count - 1 ) * 32d;
            for( int i = 0; i < Count; ++ i )
            {
                double ang = i * Math.PI * 2d / Count;
                _positions[i] = new Vector2(
                    (float) ( Math.Cos( ang ) * radius ),
                    (float) ( Math.Sin( ang ) * radius ) );
            }
        }

        public void Render( SpriteShader shader )
        {
            for ( int i = 0; i < Count; ++i )
            {
                _sNodeSprite.Position = _positions[i];
                _sNodeSprite.Render( shader );
            }
        }
    }
}
