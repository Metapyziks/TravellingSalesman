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
        public new static PositionalGraph FromFile(String path)
        {
            return new PositionalGraph(File.ReadAllText(path));
        }

        private Sprite _sNodeSprite;
        private Sprite _sConnSprite;

        public float ForceMul { get; set; }

        public float Scale { get; set; }

        public Vector2 CameraPos { get; set; }

        public Route CurrentRoute { get; set; }

        public int SelectedVertex { get; set; }

        private void LoadSprites()
        {
            if (_sNodeSprite != null && _sConnSprite != null)
                return;

            var nodeTex = new BitmapTexture2D(Properties.Resources.Circle,
                TextureMinFilter.LinearMipmapLinear, TextureMagFilter.Linear);
            var connTex = new BitmapTexture2D(Properties.Resources.Connection,
                TextureMinFilter.LinearMipmapLinear, TextureMagFilter.Linear);

            _sNodeSprite = new Sprite(nodeTex, Scale) { UseCentreAsOrigin = true };
            _sConnSprite = new Sprite(connTex) { UseCentreAsOrigin = true };
        }

        private Vector2[] _positions;
        private int[,] _ordered;

        protected PositionalGraph(String data)
            : base(data)
        {
            ForceMul = 32f / Count;

            LoadSprites();

            int[] orderBuffer = new int[Count];
            for (int i = 0; i < Count; ++i)
                orderBuffer[i] = i;

            Random rand = new Random();

            _positions = new Vector2[Count];
            _ordered = new int[Count, Count];

            for (int i = 0; i < Count; ++i) {
                Array.Sort(orderBuffer, Comparer<int>.Create((x, y) => this[i, x] == this[i, y] ? x - y : this[i, x] - this[i, y]));

                for (int j = 0; j < Count; ++j)
                    _ordered[i, j] = orderBuffer[j];

                if (_ordered[i, 0] != i)
                    throw new Exception("uhh");
            }

            DeselectVertex();
            GuessStartPositions();
        }

        public void DeselectVertex()
        {
            SelectedVertex = -1;
        }

        public int GetNearestVertexScreen(Vector2 pos)
        {
            Vector2 centre = new Vector2(400f, 300f);
            return GetNearestVertex(((pos - centre) / Scale) + CameraPos);
        }

        private int GetNearestVertex(Vector2 pos)
        {
            int nearest = -1;
            float nearestDist2 = 0f;

            for (int i = 0; i < Count; ++i) {
                float dist2 = (_positions[i] - pos).LengthSquared;
                if (nearest == -1 || nearestDist2 > dist2) {
                    nearest = i;
                    nearestDist2 = dist2;
                }
            }

            return nearest;
        }

        public void MoveVertexScreen(int vertex, Vector2 pos)
        {
            Vector2 centre = new Vector2(400f, 300f);
            MoveVertex(vertex, ((pos - centre) / Scale) + CameraPos);
        }

        private void MoveVertex(int vertex, Vector2 pos)
        {
            if (vertex == -1) return;

            _positions[vertex] = pos;
        }

        public void GuessStartPositions()
        {
            if (CurrentRoute == null || CurrentRoute.Count != Count) {
                Random rand = new Random();
                for (int i = 0; i < Count; ++i) {
                    _positions[i] = new Vector2(
                        (float) rand.NextDouble() * Count,
                        (float) rand.NextDouble() * Count);
                }
            } else {
                double radius = Math.PI * (Count - 1) * 8f;
                for (int k = 0; k < Count; ++k) {
                    double ang = k * Math.PI * 2d / Count;
                    int i = CurrentRoute[k];
                    _positions[i] = new Vector2(
                        (float) (Math.Cos(ang) * radius),
                        (float) (Math.Sin(ang) * radius));
                }
            }
        }

        public void Stablize()
        {
            Vector2[] moves = new Vector2[Count];

            for (int i = 0; i < Count; ++i) {
                if (i == SelectedVertex) continue;

                for (int j = 0; j < Count; ++j) {
                    int k = _ordered[i, j];

                    if (k == i) continue;

                    Vector2 diff = _positions[k] - _positions[i];

                    float dest = this[i, k] * 16f;
                    float curr = diff.Length;

                    diff.Normalize();

                    float mul = .5f * ForceMul;

                    if (k == SelectedVertex)
                        mul *= 4;
                    //else
                    //    mul *= (Count - j + 1) / (float) Count;

                    moves[i] += diff * (curr - dest) * mul;
                }
            }

            Vector2 min = new Vector2();
            Vector2 max = new Vector2();

            for (int i = 0; i < Count; ++i) {
                _positions[i] += moves[i] * ForceMul;

                if (i == 0 || _positions[i].X < min.X)
                    min.X = _positions[i].X;
                if (i == 0 || _positions[i].X > max.X)
                    max.X = _positions[i].X;

                if (i == 0 || _positions[i].Y < min.Y)
                    min.Y = _positions[i].Y;
                if (i == 0 || _positions[i].Y > max.Y)
                    max.Y = _positions[i].Y;
            }

            CameraPos = (max + min) * .5f;
            Scale = Math.Min(768 / (max.X - min.X), 568 / (max.Y - min.Y));
        }

        public void Render(SpriteShader shader)
        {
            Vector2 centre = new Vector2(400f, 300f);
            _sNodeSprite.Colour = new Color4(127, 127, 127, 191);
            _sNodeSprite.Scale = new Vector2(0.125f * 1.05f);
            for (int i = 0; i < Count; ++i) {
                _sNodeSprite.Position = ((_positions[i] - CameraPos) * Scale) + centre;
                _sNodeSprite.Render(shader);
            }

            if (CurrentRoute != null) {
                _sConnSprite.Height = 8f;
                for (int k = 0; k < CurrentRoute.Count; ++k) {
                    int i = CurrentRoute[k];
                    int j = CurrentRoute[k + 1];

                    Vector2 diff = _positions[j] - _positions[i];
                    Vector2 mid = (_positions[i] + _positions[j]) * 0.5f;
                    float len = diff.Length;
                    float ang = (float) Math.Atan2(diff.Y, diff.X);
                    _sConnSprite.Position = ((mid - CameraPos) * Scale) + centre;
                    Color4 clr = _sConnSprite.Colour;
                    clr.G = 1f - OpenTKTools.Tools.Clamp(Math.Abs(len - this[i, j])
                        / (this[i, j] * 64), 0f, 1f);
                    clr.B = clr.G;
                    _sConnSprite.Colour = clr;
                    _sConnSprite.Width = Scale * len;
                    _sConnSprite.Rotation = ang;
                    _sConnSprite.Render(shader);
                }
            }

            _sNodeSprite.Colour = Color4.White;
            _sNodeSprite.Scale = new Vector2(0.125f);
            for (int i = 0; i < Count; ++i) {
                if (i == SelectedVertex) continue;

                _sNodeSprite.Position = ((_positions[i] - CameraPos) * Scale) + centre;
                _sNodeSprite.Render(shader);
            }

            if (SelectedVertex != -1) {
                _sNodeSprite.Colour = Color4.LimeGreen;

                _sNodeSprite.Position = ((_positions[SelectedVertex] - CameraPos) * Scale) + centre;
                _sNodeSprite.Render(shader);
            }
        }
    }
}
