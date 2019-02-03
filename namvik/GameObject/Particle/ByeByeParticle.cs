using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace namvik.GameObject.Particle
{
    public class ByeByeParticle: BaseGameObject
    {
        private Vector2 _localVector;
        private Vector2 _velocity;
        private Vector2 _initialVelocity;
        private float _accelerationY = -4f;
        private float _xVelocityStopTime = 2;
        private float _livingTime;

        private Vector3[] _vertexes;
        private Vector3 _rotationAxisVector;

        private readonly Effect _byeByeParticleEffect = ContentLoader.GetContent<Effect>(Contents.ByeByeParticleEffect);
        private readonly Effect _basicEffect = ContentLoader.GetContent<Effect>(Contents.BasicEffect);

        private RenderTarget2D _renderTarget;

        private readonly float _rotationPerSecond;
        private readonly float _length;

        private static readonly Random Random = new Random();

        public ByeByeParticle(BaseGameObject parent, Vector2 position, Vector2 center, float rotationPerSecond, float length) : base(parent)
        {
            HasMass = false;

            _localVector = position;
            _rotationPerSecond = rotationPerSecond;
            _length = length;

            InitializeVelocity(center);
            InitializeVertexes();
            InitializeRotationAxis();

            _renderTarget = new RenderTarget2D(
                Game1.Device,
                Game1.Graphics.PreferredBackBufferWidth,
                Game1.Graphics.PreferredBackBufferHeight,
                false,
                SurfaceFormat.Color,
                DepthFormat.None,
                0,
                RenderTargetUsage.DiscardContents);
        }

        private void InitializeRotationAxis()
        {
            var polygonNormalVector = GetNormalVector();
            var moveDirectionVector = new Vector3(Random.NextFloat(-1, 1), -1, 0);
            moveDirectionVector.Normalize();
            _rotationAxisVector = Vector3.Cross(polygonNormalVector, moveDirectionVector);
            _rotationAxisVector.Normalize();
        }

        private void InitializeVertexes()
        {
            _vertexes = new[]
            {
                new Vector2(0, 0),
                new Vector2(0, _length),
                new Vector2(_length, (float)Random.NextDouble() * _length),
            }.Select(vec2 =>
            {
                var angle = (float) (Random.NextDouble() * Math.PI * 2);
                var rotated = vec2.Rotate(angle);
                return new Vector3(rotated, 0);
            }).ToArray();
        }

        private void InitializeVelocity(Vector2 center)
        {
            var dX = _localVector.X - center.X;
            var vX = dX * 0.5f;
            _velocity = new Vector2(vX, 0);
            _initialVelocity = _velocity;
        }

        private Vector3 GetNormalVector()
        {
            var edge1 = _vertexes[1] - _vertexes[0];
            var edge2 = _vertexes[2] - _vertexes[1];

            var normal = Vector3.Cross(edge1, edge2);
            normal.Normalize();
            return normal;
        }

        public IEnumerable<Vector2> GetVertexes()
        {
            return _vertexes.Select(vertex => new Vector2(vertex.X, vertex.Y) + Position + _localVector);
        }

        public override void Update(float dt)
        {
            base.Update(dt);
            _livingTime += dt;

            if (_livingTime <= _xVelocityStopTime)
            {
                var dV = (_initialVelocity.X / _xVelocityStopTime) * dt;
                _velocity.X -= dV;
            }

            _velocity.Y += _accelerationY * dt;

            _localVector += _velocity * dt;

            var rotationAngle = _rotationPerSecond * dt;

            var rotationQuaternion = new Quaternion(_rotationAxisVector * (float)Math.Sin(rotationAngle / 2), (float)Math.Cos(rotationAngle / 2));

            for (var i = 0; i < _vertexes.Length; i += 1)
            {
                var vertex = _vertexes[i];
                _vertexes[i] = Vector3.Transform(vertex, rotationQuaternion);
            }
        }

        public override void PreDraw(float dt, GraphicsDevice device, Camera2D camera)
        {
            base.PreDraw(dt, device, camera);

            device.SetRenderTarget(_renderTarget);
            device.Clear(Color.Transparent);

            var vertexPositionColorTextures = _vertexes.Select(vertex =>
            {
                var position = Position + _localVector + new Vector2(vertex.X, vertex.Y) - camera.Position;
                var uv = Vector2.Zero;
                var vertexPositionColorTexture = new VertexPositionColorTexture(new Vector3(position, 0), Color.White, uv);
                return vertexPositionColorTexture;
            }).ToArray();

            var normalVector = GetNormalVector();
            var vectorToBehindMonitor = new Vector3(0, 0, -1);

            if (Vector3.Dot(vectorToBehindMonitor, normalVector) > 0)
            {
                vertexPositionColorTextures = vertexPositionColorTextures.Reverse().ToArray();
            }

            _basicEffect.CurrentTechnique.Passes[0].Apply();
            Game1.Device.DrawUserPrimitives(
                PrimitiveType.TriangleList,
                vertexPositionColorTextures,
                0,
                vertexPositionColorTextures.Length / 3);


            device.SetRenderTarget(null);
        }

        public override void Draw(float dt, SpriteBatch spriteBatch)
        {
            base.Draw(dt, spriteBatch);

            var projected = _vertexes.Select(vertex => new Vector2(vertex.X, vertex.Y));

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate);
            _byeByeParticleEffect.Parameters["TextureWidth"].SetValue(_renderTarget.Width);
            _byeByeParticleEffect.Parameters["TextureHeight"].SetValue(_renderTarget.Height);

            _byeByeParticleEffect.CurrentTechnique.Passes[0].Apply();
            spriteBatch.Draw(_renderTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            //spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, transformMatrix: Game1.Camera.GetViewMatrix(), samplerState: SamplerState.PointClamp);
            
            //spriteBatch.DrawPolygon(Position + _localVector, projected.ToArray(), Color.Red);

            //spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, transformMatrix: Game1.Camera.GetViewMatrix(), samplerState: SamplerState.PointClamp);
        }
    }
}
