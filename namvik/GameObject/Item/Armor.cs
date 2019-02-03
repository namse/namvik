using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using namvik.FiniteStateMachine;
using namvik.GameObject.Particle;
using namvik.Tile;
using Color = Microsoft.Xna.Framework.Color;
using Math = System.Math;

namespace namvik.GameObject.Item
{
    public static class RandomExtension
    {
        public static float NextFloat(this Random random)
        {
            return (float)random.NextDouble();
        }
    }

    public class Armor : BaseItem
    {
        enum ArmorState
        {
            BeforeLoot,
            Wearing,
            Destroyed,
        }

        public struct Dot
        {
            public float X; // 0 <= X <= 1
            public float Value; // 0 <= X <= 1
        }
        private static readonly Random Random = new Random();

        private readonly FiniteStateMachine<ArmorState> _armorStateFSM;
        private const int DestroyDelay = 500;
        private const int SparklingDurationOnWearing = 200;
        private bool _isSkippingDraw;
        private const int SparklingBetweenDelay = 100;
        private readonly Effect _byeByeEffect = ContentLoader.GetContent<Effect>(Contents.ByeByeEffect);
        private readonly Texture2D _zigZagTexture;
        private readonly Texture2D _mosaicTexture;

        private int frames;
        private List<Dot> Dots = GenerateDots(10);

        private ByeByeParticleSystem _byeByeParticleSystem;
        private ByeByeParticle _byeByeParticle;
        private BloomEffect _bloomEffect;
        public Armor(BaseGameObject parent) : base(parent)
        {
            _zigZagTexture = new Texture2D(Game1.Device, 1, Texture.Height);
            _mosaicTexture = new Texture2D(Game1.Device, Texture.Width, Texture.Height);
            _byeByeParticleSystem = new ByeByeParticleSystem(this);

            _byeByeParticleSystem.MakeParticlesWithTexture(Texture);
            var center = new Vector2(0, -10);
            //_byeByeParticle = new ByeByeParticle(this, center, center);
            //_bloomEffect = new BloomEffect(this, new List<ByeByeParticle>{ _byeByeParticle });

            categoryBits = CategoryBits.Armor;
            maskBits = MaskBits.Armor;

            CreateBody();
            HasMass = false;

            _armorStateFSM = new FiniteStateMachine<ArmorState>(
                ArmorState.BeforeLoot,
                new Dictionary<(ArmorState, ArmorState), Action>
                {
                    { (ArmorState.BeforeLoot, ArmorState.Wearing), () => { Sparkle(SparklingDurationOnWearing); } },
                    { (ArmorState.Wearing, ArmorState.Destroyed), async () =>
                    {
                        HasMass = true;
                        Sparkle(DestroyDelay);
                        await Task.Delay(DestroyDelay);

                        IsDead = true;
                    } },
                });
        }

        private void CreateBody()
        {
            var bodyDef = new BodyDef
            {
                Position = Position.ToVec2(),
                FixedRotation = true,
            };

            Body = Map.World.CreateBody(bodyDef);

            var polygonDef = new PolygonDef();


            var originY = 35f.ToMeter();
            var width = 65f.ToMeter();
            var height = 55f.ToMeter();

            var center = new Vec2(width / 2f, originY + height / 2f);
            var hx = width / 2f;
            var hy = height / 2f;

            polygonDef.SetAsBox(hx, hy, center, angle: 0);

            polygonDef.Density = 0.001f;
            polygonDef.Friction = 0f;
            polygonDef.Restitution = 0f;
            polygonDef.Filter.CategoryBits = (ushort)categoryBits;
            polygonDef.Filter.MaskBits = (ushort)maskBits;

            MainBodyShape = Body.CreateShape(polygonDef);
            PolygonDefs.Add(polygonDef);

            Body.SetMassFromShapes();

            Body.SetUserData(this);
        }

        public override void OnLooted()
        {
            base.OnLooted();
            _armorStateFSM.ChangeState(ArmorState.Wearing);
        }

        public override void OnDestroyed()
        {
            _armorStateFSM.ChangeState(ArmorState.Destroyed);
        }

        public async Task Sparkle(int durationMilliseconds)
        {
            var introDelay = SparklingBetweenDelay;

            await Task.Delay(introDelay);

            var sparklingTimes = (durationMilliseconds - introDelay) / SparklingBetweenDelay;
            for (var i = 0; i < sparklingTimes; i += 1)
            {
                _isSkippingDraw = true;
                await Task.Delay(SparklingBetweenDelay);
                _isSkippingDraw = false;
                await Task.Delay(SparklingBetweenDelay);
            }
        }

        private static List<Dot> GenerateDots(int numberOfDots)
        {
            var dots = new List<Dot>();

            var firstDot = new Dot { X = 0, Value = Random.NextFloat() };
            dots.Add(firstDot);

            for (var i = 0; i < numberOfDots - 2; i += 1)
            {
                var dot = new Dot { X = Random.NextFloat(), Value = Random.NextFloat() };
                dots.Add(dot);
            }

            var lastDot = new Dot { X = 1, Value = Random.NextFloat() };
            dots.Add(lastDot);

            dots.Sort((dot1, dot2) => dot1.X.CompareTo(dot2.X));

            return dots;
        }

        private float CalculateDotValue(float x, List<Dot> dots)
        {
            var dotN = dots.Where(dot => dot.X <= x).OrderByDescending(dot => dot.X).First();
            var indexN = dots.FindIndex(dot => dot.Equals(dotN));
            if (indexN + 1 == dots.Count)
            {
                return dotN.Value;
            }
            var dotN1 = dots[indexN + 1];

            var t = (x - dotN.X) * dotN1.Value + (dotN1.X - x) * dotN.Value;
            var v = t / (dotN1.X - dotN.X);

            return v;
        }

        private void MakeMosaic(Texture2D texture)
        {
            var destX = Random.Next(0, texture.Width);
            var destY = Random.Next(0, texture.Height);
            var destWidth = Random.Next(1, Math.Min(texture.Width / 2, texture.Width - destX));
            var destHeight = Random.Next(1, Math.Min(texture.Height / 10, texture.Height - destY));

            var destRectangle = new Rectangle(destX, destY, destWidth, destHeight);


            var colors = new Color[destWidth * destHeight];
            for (var i = 0; i < colors.Length; i += 1)
            {
                colors[i] = new Color(0, 0, 0, 0.5f);
            }
            texture.SetData(0, destRectangle, colors, 0, colors.Length);
        }

        private void ResetTexture(Texture2D texture)
        {
            var colors = new Color[texture.Width * texture.Height];
            for (var i = 0; i < colors.Length; i += 1)
            {
                colors[i] = Color.Transparent;
            }
            texture.SetData(colors);
        }

        public override void Draw(float dt, SpriteBatch spriteBatch)
        {
            if (_isSkippingDraw)
            {
                return;
            }

            var originY = 35;
            var height = 55;

            frames += 1;
            if (frames > 6)
            {
                frames = 0;
                Dots = GenerateDots(10);
                ResetTexture(_mosaicTexture);
                MakeMosaic(_mosaicTexture);
                MakeMosaic(_mosaicTexture);
                MakeMosaic(_mosaicTexture);
                MakeMosaic(_mosaicTexture);
                MakeMosaic(_mosaicTexture);
            }

            var data = new Color[_zigZagTexture.Width * _zigZagTexture.Height];
            for (var index = 0; index < data.Count(); index++)
            {
                int r;
                if (index < originY || index > originY + height)
                {
                    r = 0;
                }
                else
                {
                    var x = (index - originY) / (float)height;
                    var value = CalculateDotValue(x, Dots);

                    r = (int)Math.Floor(255 * value);
                }

                data[index] = new Color(r, 0, 0, 255);
            }
            _zigZagTexture.SetData(data);


            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, transformMatrix: Game1.Camera.GetViewMatrix(), samplerState: SamplerState.PointClamp);

            _byeByeEffect.Parameters["ZigZagTexture"].SetValue(_zigZagTexture);
            //_byeByeEffect.CurrentTechnique.Passes[0].Apply();
            base.Draw(dt, spriteBatch);
            DrawTexture(dt, spriteBatch, _mosaicTexture);

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, transformMatrix: Game1.Camera.GetViewMatrix(), samplerState: SamplerState.PointClamp);
        }
    }
}
