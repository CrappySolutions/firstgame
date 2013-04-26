//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Storage;
//using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.Graphics;
//using WP8GameTest.Entities;

//namespace WP8GameTest.Sprites
//{
//  public class Player : Sprite
//  {
//    const string WIZARD_ASSETNAME = "dude";
//    //{X:634,6628 Y:418,6684}
//    const int START_POSITION_X = 110;
//    const int START_POSITION_Y = 425;
//    const int WIZARD_SPEED = 160;
//    const int MOVE_UP = -1;
//    const int MOVE_DOWN = 1;
//    const int MOVE_LEFT = -1;
//    const int MOVE_RIGHT = 1;

//    List<Fireball> mFireballs = new List<Fireball>();

//    ContentManager mContentManager;

//    enum State
//    {
//      Walking
//    }
//    State mCurrentState = State.Walking;

//    Vector2 mDirection = Vector2.Zero;
//    Vector2 mSpeed = Vector2.Zero;

//    KeyboardState mPreviousKeyboardState;

//    public void LoadContent(ContentManager theContentManager)
//    {
//      mContentManager = theContentManager;

//      foreach (Fireball aFireball in mFireballs)
//      {
//        aFireball.LoadContent(theContentManager);
//      }

//      Position = new Vector2(START_POSITION_X, START_POSITION_Y);
//      base.LoadContent(theContentManager, WIZARD_ASSETNAME);
//      Size = new Rectangle(0, 0, 200, Size.Height);

//      Position = new Vector2(START_POSITION_X, START_POSITION_Y);
//      base.LoadContent(theContentManager, WIZARD_ASSETNAME);
//    }

//    public void Update(GameTime theGameTime, MoveDirection moveDirection)
//    {
//      KeyboardState aCurrentKeyboardState = Keyboard.GetState();

//      UpdateFireball(theGameTime, moveDirection);

//      UpdateMovement(moveDirection);//aCurrentKeyboardState

//      mPreviousKeyboardState = aCurrentKeyboardState;

//      base.Update(theGameTime, mSpeed, mDirection);
//    }

//    private void UpdateFireball(GameTime theGameTime, MoveDirection moveDirection)
//    {
//      foreach (Fireball aFireball in mFireballs)
//      {
//        aFireball.Update(theGameTime);
//      }

//      if (moveDirection == MoveDirection.Right)
//      {
//        ShootFireball();
//      }
//    }

//    private void ShootFireball()
//    {
//      if (mCurrentState == State.Walking)
//      {
//        bool aCreateNew = true;
//        foreach (Fireball aFireball in mFireballs)
//        {
//          if (aFireball.Visible == false)
//          {
//            aCreateNew = false;
//            aFireball.Fire(Position + new Vector2(Size.Width / 2, Size.Height / 2),
//                new Vector2(200, 0), new Vector2(1, 0));
//            break;
//          }
//        }

//        if (aCreateNew == true)
//        {
//          Fireball aFireball = new Fireball();
//          aFireball.LoadContent(mContentManager);
//          aFireball.Fire(Position + new Vector2(Size.Width / 2, Size.Height / 2),
//              new Vector2(200, 200), new Vector2(1, 0));
//          mFireballs.Add(aFireball);
//        }
//      }
//    }

//    private void UpdateMovement(MoveDirection moveDirection) //KeyboardState aCurrentKeyboardState,
//    {
//      if (mCurrentState == State.Walking)
//      {
//        mSpeed = Vector2.Zero;
//        mDirection = Vector2.Zero;

//        switch (moveDirection)
//        {
//          case MoveDirection.Left:
//            mSpeed.X = WIZARD_SPEED;
//          mDirection.X = MOVE_LEFT;
//            break;
//          case MoveDirection.Right:
//             mSpeed.X = WIZARD_SPEED;
//          mDirection.X = MOVE_RIGHT;
//            break;
//          case MoveDirection.Up:
//            mSpeed.Y = WIZARD_SPEED;
//            mDirection.Y = MOVE_UP;
//            break;
//          case MoveDirection.Down:
//            mSpeed.Y = WIZARD_SPEED;
//            mDirection.Y = MOVE_DOWN;
//            break;
//          case MoveDirection.Stop:
//            mSpeed.Y = 0;
//            mSpeed.X = 0;
//            break;
//          default:
//            break;
//        }

//        //if (aCurrentKeyboardState.IsKeyDown(Keys.Left) == true)
//        //{
//        //  mSpeed.X = WIZARD_SPEED;
//        //  mDirection.X = MOVE_LEFT;
//        //}
//        //else if (aCurrentKeyboardState.IsKeyDown(Keys.Right) == true)
//        //{
//        //  mSpeed.X = WIZARD_SPEED;
//        //  mDirection.X = MOVE_RIGHT;
//        //}

//        //if (aCurrentKeyboardState.IsKeyDown(Keys.Up) == true)
//        //{
//        //  mSpeed.Y = WIZARD_SPEED;
//        //  mDirection.Y = MOVE_UP;
//        //}
//        //else if (aCurrentKeyboardState.IsKeyDown(Keys.Down) == true)
//        //{
//        //  mSpeed.Y = WIZARD_SPEED;
//        //  mDirection.Y = MOVE_DOWN;
//        //}
//      }
//    }

//  }
//}
