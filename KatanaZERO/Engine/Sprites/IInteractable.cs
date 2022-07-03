namespace Engine.Sprites
{
    using System;

    public interface IInteractable : ISprite
    {
        EventHandler OnInteract { get; set; }
    }
}
