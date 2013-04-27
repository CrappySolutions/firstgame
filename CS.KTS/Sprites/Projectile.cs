using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.KTS.Sprites
{
  public class Projectile : AnimatedSprite
  {
    private int MAX_DISTANCE = 700;

    private Vector2 StartPosition;
    public Projectile(Texture2D texture, int rows, int columns)
      : base(texture, rows, columns)
    {
      Scale = 0.3f;
    }

    public override void Update(Microsoft.Xna.Framework.GameTime theGameTime, Movement movement)
    {
      DoRemove = (Vector2.Distance(StartPosition, Position) > MAX_DISTANCE);
      if (!DoRemove)
        Position += mDirection * mSpeed * (float)theGameTime.ElapsedGameTime.TotalSeconds;
    }

    public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
    {
      spriteBatch.Draw(SpriteTexture, Position,
      new Rectangle(0, 0, SpriteTexture.Width, SpriteTexture.Height),
      Color.White, 0.0f, Vector2.Zero, Scale, SpriteEffects.None, 0);
    }

    public void Fire(Vector2 theStartPosition, Vector2 theSpeed, Vector2 theDirection)
    {
      Position = theStartPosition;
      StartPosition = theStartPosition;
      mSpeed = theSpeed;
      mDirection = theDirection;
    }
  }
}
