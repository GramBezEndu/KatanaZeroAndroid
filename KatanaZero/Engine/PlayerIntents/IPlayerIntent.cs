using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.PlayerIntents
{
    public interface IPlayerIntent : IDrawableComponent
    {
        void IntentFinished();
    }
}
