using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace namvik.GameObject.Item
{
    public abstract class BaseItem: BaseGameObject
    {
        protected BaseItem(BaseGameObject parent): base(parent)
        {

        }
    }
}
