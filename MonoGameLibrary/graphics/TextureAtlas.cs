using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.Graphics;

public class TextureAtlas
{
    private Dictionary<string, TextureRegion> _regions;
    private Dictionary<string, Animation> _animations;

    /// <summary>
    /// Gets or Sets the source texture represented by this texture atlas.
    /// </summary>
    Texture2D Texture { get; set; }

    /// <summary>
    /// Create a new texture Atlas.
    /// </summary>
    public TextureAtlas()
    {
        _regions = new Dictionary<string, TextureRegion>();
        _animations = new Dictionary<string, Animation>();
    }

    /// <summary>
    /// Creates a new texture atlas instance using the provided Texture.
    /// </summary>
    /// <param name="texture"> The source texture represented by the texture atlas. </param>
    public TextureAtlas(Texture2D texture)
    {
        Texture = texture;
        _regions = new Dictionary<string, TextureRegion>();
        _animations = new Dictionary<string, Animation>();
    }

    /// <summary>
    /// Creates a new region and add it to this texture atlas.
    /// </summary>
    /// <param name="name"> The name givin to the texture region. </param>
    /// <param name="x"> The top-left x-coordinate position of the region boundary relative to the top-left corner of the source texture boundary </param>
    /// <param name="y"> The top-left y-coordinate position of the region boundary relative to the top-left corner of the source texture boundary. </param>
    /// <param name="width"> The width in pixels of the specified region. </param>
    /// <param name="height"> The height in pixels of the specified region. </param>
    public void AddRegion(string name, int x, int y, int width, int height)
    {
        TextureRegion region = new TextureRegion(Texture, x, y, width, height);
        _regions.Add(name, region);
    }

    /// <summary>
    /// Gets the region of this texture atlas with the givin name.
    /// </summary>
    /// <param name="name"> The name of the region to retrive. </param>
    /// <returns> The textureRegion with the specified name. </returns>
    public TextureRegion GetRegion(string name)
    {
        return _regions[name];
    }

    /// <summary>
    /// Removes region from this texture atlas with the givin name.
    /// </summary>
    /// <param name="name"> The name of the region to remove. </param>
    public void RemoveRegion(string name)
    {
        _regions.Remove(name);
    }
    
    /// <summary>
    /// Adds given animation to this texture atlas.
    /// </summary>
    /// <param name="name"> Name of the animation.</param>
    /// <param name="animation"> Animation to add. </param>
    public void AddAnimations(string name, Animation animation)
    {
        _animations.Add(name, animation);
    }
    
    /// <summary>
    /// Gets the specified animation from this atlas.
    /// </summary>
    /// <param name="name"> Name of the animation to retrieve =. </param>
    /// <returns></returns>
    public Animation GetAnimation(string name)
    {
        return _animations[name];
    }

    /// <summary>
    /// Removes the specified animation from this atlas.
    /// </summary>
    /// <param name="name"> The name of the animation to remove. </param>
    public void RemoveAnimation(string name)
    {
        _animations.Remove(name);    
    }
    
    /// <summary>
    /// Remove all regions from this texture atlas.
    /// </summary>
    public void ClearRegions()
    {
        _regions.Clear();
    }

    public static TextureAtlas FromFile(ContentManager content, string fileName)
    {
        TextureAtlas atlas = new TextureAtlas();

        string filePath = Path.Combine(content.RootDirectory, fileName);

        using (Stream stream = TitleContainer.OpenStream(filePath))
        {
            using (XmlReader reader = XmlReader.Create(stream))
            {
                XDocument doc = XDocument.Load(reader);
                XElement root = doc.Root;

                // The <Texture> element contains the content path for the Texture2D to load.
                // So we will retrieve that value then use the content manager to load the texture.
                string texturePath = root.Element("Texture").Value;
                atlas.Texture = content.Load<Texture2D>(texturePath);

                // The <Regions> element contains individual <Region> elements, each one describing
                // a different texture region within the atlas.  
                //
                // Example:
                // <Regions>
                //      <Region name="spriteOne" x="0" y="0" width="32" height="32" />
                //      <Region name="spriteTwo" x="32" y="0" width="32" height="32" />
                // </Regions>
                //
                // So we retrieve all of the <Region> elements then loop through each one
                // and generate a new TextureRegion instance from it and add it to this atlas.
                var regions = root.Element("Regions")?.Elements("Region");

                if (regions != null)
                {
                    foreach (var region in regions)
                    {
                        string name = region.Attribute("name")?.Value;
                        int x = int.Parse(region.Attribute("x")?.Value ?? "0");
                        int y = int.Parse(region.Attribute("y")?.Value ?? "0");
                        int width = int.Parse(region.Attribute("width")?.Value ?? "0");
                        int height = int.Parse(region.Attribute("height")?.Value ?? "0");

                        if (!string.IsNullOrEmpty(name))
                        {
                            atlas.AddRegion(name, x, y, width, height);
                        }
                    }
                }

                // The <Animations> element contains individual <Animation> elements, each one describing
                // a different animation within the atlas.
                //
                // Example:
                // <Animations>
                //      <Animation name="animation" delay="100">
                //          <Frame region="spriteOne" />
                //          <Frame region="spriteTwo" />
                //      </Animation>
                // </Animations>
                //
                // So we retrieve all of the <Animation> elements then loop through each one
                // and generate a new Animation instance from it and add it to this atlas.
                var animationElements = root.Element("Animations").Elements("Animation");

                if (animationElements != null)
                {
                    foreach (var animationElement in animationElements)
                    {
                        string name = animationElement.Attribute("name")?.Value;
                        float delayInMilliseconds = float.Parse(animationElement.Attribute("delay")?.Value ?? "0");
                        TimeSpan delay = TimeSpan.FromMilliseconds(delayInMilliseconds);

                        List<TextureRegion> frames = new List<TextureRegion>();

                        var frameElements = animationElement.Elements("Frame");

                        if (frameElements != null)
                        {
                            foreach (var frameElement in frameElements)
                            {
                                string regionName = frameElement.Attribute("region").Value;
                                TextureRegion region = atlas.GetRegion(regionName);
                                frames.Add(region);
                            }
                        }

                        Animation animation = new Animation(frames, delay);
                        atlas.AddAnimations(name, animation);
                    }
                }
                return atlas;
            }
        }
    }

    /// <summary>
    /// Creates a new sprite using the region from this texture atlas with the specified name.
    /// </summary>
    /// <param name="regionName">The name of the region to create the sprite with.</param>
    /// <returns>A new Sprite using the texture region with the specified name.</returns>
    public Sprite CreateSprite(string regionName)
    {
        TextureRegion region = GetRegion(regionName);
        return new Sprite(region);
    }
    
    /// <summary>
    /// Creates a new animated sprite using the animation from this texture atlas with the specified name.
    /// </summary>
    /// <param name="animationName">The name of the animation to use.</param>
    /// <returns>A new AnimatedSprite using the animation with the specified name.</returns>
    public AnimatedSprite CreateAnimatedSprite(string animationName)
    {
        Animation animation = GetAnimation(animationName);
        return new AnimatedSprite(animation);
    }


}