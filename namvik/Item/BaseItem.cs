using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace namvik.Item
{
    public abstract class BaseItem: GameObject
    {
        protected BaseItem(GameObject parent): base(parent)
        {

        }
    }
}
