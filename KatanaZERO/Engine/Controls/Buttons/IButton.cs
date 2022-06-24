using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Controls.Buttons
{
    public interface IButton : IDrawableComponent
    {
        EventHandler OnClick { get; set; }
    }
}
