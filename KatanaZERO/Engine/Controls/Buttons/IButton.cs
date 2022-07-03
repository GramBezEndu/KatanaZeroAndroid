namespace Engine.Controls.Buttons
{
    using System;

    public interface IButton : IDrawableComponent
    {
        EventHandler OnClick { get; set; }
    }
}
