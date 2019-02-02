using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using namvik.FiniteStateMachine;
using namvik.Tile;
using Color = Microsoft.Xna.Framework.Color;

namespace namvik.GameObject.Item
{
    public class Armor : BaseItem
    {
        enum ArmorState
        {
            BeforeLoot,
            Wearing,
            Destroyed,
        }

        private readonly FiniteStateMachine<ArmorState> _armorStateFSM;

        private const int DestroyDelay = 500;

        public Armor(BaseGameObject parent) : base(parent)
        {
            categoryBits = CategoryBits.Armor;
            maskBits = MaskBits.Armor;

            CreateBody();
            HasMass = false;

            _armorStateFSM = new FiniteStateMachine<ArmorState>(
                ArmorState.BeforeLoot,
                new Dictionary<(ArmorState, ArmorState), Action>
                {
                    { (ArmorState.BeforeLoot, ArmorState.Wearing), () => { Sparkle(); } },
                    { (ArmorState.Wearing, ArmorState.Destroyed), async () =>
                    {
                        HasMass = true;

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

        public async Task Sparkle()
        {
            const int delayBetween = 200;
            for (var i = 0; i < 3; i += 1)
            {
                _color = Color.Yellow;
                await Task.Delay(delayBetween);
                _color = Color.White;
                await Task.Delay(delayBetween);
            }
        }
    }
}
