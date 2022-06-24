﻿using Engine.Physics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Sprites
{
    public interface IInteractable : ISprite
    {
        EventHandler OnInteract { get; set; }
    }
}
