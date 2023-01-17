using KataNeo.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

namespace KataNeo.Entitites
{
    public class Player : Entity
    {
        public ControlType controlType;

        //Position and movement related vars
        public Vector2 position = new Vector2(1920 / 2, 1080 / 2);
        public Rectangle Rect
        {
            get => new Rectangle((int)position.X - sprite.Width / 2, (int)position.Y - sprite.Height / 2,
            sprite.Width, sprite.Height);
        }
        public Vector2 velocity;
        private readonly Vector2 baseVelocity = new Vector2(0, -19.62f);
        public float moveSpeed = 5f;

        public Texture2D sprite;

        public AnimData animData;
        public Animator animator;

        private MapManager mapManager;

        float attackTime;
        float attackDelay = 0.2f;
        bool grounded = false;
        bool isAlive = true;
        bool jumped = false;
        bool attacking = false;
        bool attackHeld = false;

        //Temporarily give a sprite
        public Player(ControlType controlType, MapManager mapManager, AnimData animData)
        {
            this.controlType = controlType;
            this.mapManager = mapManager;
            this.animData = animData;
            animator = new Animator(ref sprite, animData.GetAnim("Idle_L"));
        }

        #region Game Loop Updates
        /// <summary>
        /// Update input for a player using a keyboard
        /// </summary>
        /// <param name="keyboard">Keyboard state</param>
        public void InputUpdate(KeyboardState keyboard, GameTime gameTime)
        {
            //Horizontal movement
            if (keyboard.IsKeyDown(Keys.D) && !keyboard.IsKeyDown(Keys.A)) velocity.X = moveSpeed;
            else if (keyboard.IsKeyDown(Keys.A) && !keyboard.IsKeyDown(Keys.D)) velocity.X = -moveSpeed;
            //Jumping
            if (keyboard.IsKeyDown(Keys.Space) && !jumped && grounded)
            {
                velocity.Y = 20;
                grounded = false;
                jumped = true;
            }
            else if (keyboard.IsKeyUp(Keys.Space) && jumped) jumped = false;
            //Attacking
            if (keyboard.IsKeyDown(Keys.F) && !attackHeld && !attacking)
            {
                //Attack
                Debug.WriteLine("Attacking!");
                //Set attack cooldown
                attackTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
                attackHeld = true;
                attacking = true;
            }
            else if (keyboard.IsKeyUp(Keys.F)) attackHeld = false;
        }

        /// <summary>
        /// Update input for a player using a gamepad
        /// </summary>
        /// <param name="gamePad">State of gamepad player is currently using</param>
        public void InputUpdate(GamePadState gamePad, GameTime gameTime)
        {
            //Horizontal movement
            velocity.X = gamePad.ThumbSticks.Left.X * moveSpeed;
            //Jumping
            if (gamePad.Buttons.A == ButtonState.Pressed && !jumped && grounded)
            {
                velocity.Y = 20;
                grounded = false;
                jumped = true;
            }
            else if (gamePad.Buttons.A == ButtonState.Released && jumped) jumped = false;
            //Attacking
            if (gamePad.Buttons.X == ButtonState.Pressed && !attackHeld && !attacking)
            {
                //Attack
                Debug.WriteLine("Attacking!");
                //Set attack cooldown
                attackTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
                attackHeld = true;
                attacking = true;
            }
            else if (gamePad.Buttons.X == ButtonState.Released) attackHeld = false;
        }

        public override void Update(GameTime gameTime)
        {
            sprite = animator.Update(gameTime);
#if DEBUG
            //Debug.WriteLine($"Player {(int)controlType} position: {position}");
#endif
            if (gameTime.TotalGameTime.TotalSeconds >= attackTime + attackDelay)
                attacking = false;

            //Apply the velocity to the player
            position += new Vector2(velocity.X, -velocity.Y);
            if (velocity.X > 0 && animator.curAnim != animData.GetAnim("Idle_R")) animator.ChangeAnim(animData.GetAnim("Idle_R"));
            else if (velocity.X < 0 && animator.curAnim != animData.GetAnim("Idle_L")) animator.ChangeAnim(animData.GetAnim("Idle_L"));
            velocity = Mathf.Lerp(velocity, baseVelocity, 0.05f);
            CheckCollision();

            //Check if the player is on the bootom of the screen so he can jump
            if (position.Y > 1080 - sprite.Height / 2)
            {
                grounded = true;
            }
            //Confine the player within the game window
            if (isAlive)
            {
                position = new Vector2(Mathf.Clamp(position.X, 0 + sprite.Width / 2, 1920 - sprite.Width / 2),
                    Mathf.Clamp(position.Y, 0 + sprite.Height / 2, 1080 - sprite.Height / 2));
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, position, null, Color.White, 0f,
                new Vector2(sprite.Width / 2, sprite.Height / 2), 1f, SpriteEffects.None, 0f);
        }
        #endregion

        public void CheckCollision()
        {
            grounded = false;
            foreach (var tile in mapManager.tiles)
            {
                if (Rect.Intersects(tile.Rect))
                {
                    Vector2 distVec = position - tile.position;
                    bool left = false;
                    bool up = false;

                    //Check which side the player could collide
                    if (distVec.X < 0)
                        distVec.X += sprite.Width / 2;
                    else
                    {
                        left = true;
                        distVec.X -= sprite.Width / 2;
                    }
                    if (distVec.Y < 0)
                        distVec.Y += sprite.Height / 2;
                    else
                    {
                        up = true;
                        distVec.Y -= sprite.Height / 2;
                    }

                    //Move the player to either side of the tile depending on which one is higher
                    if (Math.Abs(distVec.X) >= Math.Abs(distVec.Y))
                    {
                        if (left)
                        {
                            velocity.X = 0;
                            position.X = tile.position.X + tile.sprite.Width / 2 + sprite.Width / 2;
                        }
                        else
                        {
                            velocity.X = 0;
                            position.X = tile.position.X - tile.sprite.Width / 2 - sprite.Width / 2;
                        }
                    }
                    else
                    {
                        if (up)
                        {
                            velocity.Y = 0;
                            position.Y = tile.position.Y + tile.sprite.Height / 2 + sprite.Height / 2;
                        }
                        else
                        {
                            grounded = true;
                            velocity.Y = 0;
                            position.Y = tile.position.Y - tile.sprite.Height / 2 - sprite.Height / 2;
                        }
                    }
                }
            }
        }
    }
}
