using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WP8GameTest
{
  public class Sprite
  {
    //The current position of the Sprite
    public Vector2 Position = new Vector2(0, 0);

    //The texture object used when drawing the sprite
    public Texture2D SpriteTexture;

    //The asset name for the Sprite's Texture
    public string AssetName;

    //The Size of the Sprite (with scale applied)
    public Rectangle Size;

    public bool DoRemove;

    public Rectangle BoundingBox 
    {
      get { return new Rectangle((int)Position.X, (int)Position.Y, SpriteTexture.Width, SpriteTexture.Height); } 
    }

    //The amount to increase/decrease the size of the original sprite. When
    //modified throught he property, the Size of the sprite is recalculated
    //with the new scale applied.
    private float mScale = 1.0f;
    public float Scale
    {
      get { return mScale; }
      set
      {
        mScale = value;
        //Recalculate the Size of the Sprite with the new scale
        Size = new Rectangle(0, 0, (int)(SpriteTexture.Width * Scale), (int)(SpriteTexture.Height * Scale));
      }
    }

    

    //Load the texture for the sprite using the Content Pipeline
    public virtual void LoadContent(ContentManager theContentManager, string theAssetName)
    {
      SpriteTexture = theContentManager.Load<Texture2D>(theAssetName);
      AssetName = theAssetName;
      Size = new Rectangle(0, 0, (int)(SpriteTexture.Width * Scale), (int)(SpriteTexture.Height * Scale));
    }

    //Update the Sprite and change it's position based on the passed in speed, direction and elapsed time.
    public void Update(GameTime theGameTime, Vector2 theSpeed, Vector2 theDirection)
    {
      Position += theDirection * theSpeed * (float)theGameTime.ElapsedGameTime.TotalSeconds;
    }

    //Draw the sprite to the screen
    public virtual void Draw(SpriteBatch theSpriteBatch)
    {
      theSpriteBatch.Draw(SpriteTexture, Position,
          new Rectangle(0, 0, SpriteTexture.Width, SpriteTexture.Height),
          Color.White, 0.0f, Vector2.Zero, Scale, SpriteEffects.None, 0);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="otherObject"></param>
    /// <returns></returns>
    public bool IsColliding(Sprite otherObject)
    {
      return this.BoundingBox.Intersects(otherObject.BoundingBox);
    }

  }
}
