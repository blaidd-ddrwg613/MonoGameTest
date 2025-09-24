using System;
using System.Collections.Generic;

namespace MonoGameLibrary.Graphics;

public class Animation
{
    /// <summary>
    /// The TextureRegions that make up the frames of this animation, the order in the collection is the order they
    /// they should display in.
    /// </summary>
    public List<TextureRegion> Frames { get; set; }
    
    /// <summary>
    /// The amount of time to delay between each frame displaying.
    /// </summary>
    public TimeSpan Delay { get; set; }

    /// <summary>
    /// Creates a new animation.
    /// </summary>
    public Animation()
    {
        Frames = new List<TextureRegion>();
        Delay = TimeSpan.FromMilliseconds(100);
    }

    /// <summary>
    /// Creates a new animation with the specified frames and delay.
    /// </summary>
    /// <param name="frames"> An ordered collection of the frames for this animation. </param>
    /// <param name="delay"> The amount of time to delay for each frame of the animation. </param>
    public Animation(List<TextureRegion> frames, TimeSpan delay)
    {
        Frames = frames;
        Delay = delay;
    }
}