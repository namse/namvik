using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using namvik.FiniteStateMachine;

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

        public Armor(BaseGameObject parent) : base(parent)
        {
            _armorStateFSM = new FiniteStateMachine<ArmorState>(
                ArmorState.BeforeLoot,
                new Dictionary<(ArmorState, ArmorState), Action>
                {
                    { (ArmorState.BeforeLoot, ArmorState.Wearing), () => { Sparkle(); } }
                });
        }

        public override void OnLooted()
        {
            _armorStateFSM.ChangeState(ArmorState.Wearing);
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
