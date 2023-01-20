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
        public Vector2 position;
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
        bool attacking = false;

        //Temporarily give a sprite
        public Player(ControlType controlType, MapManager mapManager, AnimData animData, Vector2 position)
        {
            this.position = position;
            this.controlType = controlType;
            this.mapManager = mapManager;
            this.animData = animData;
            animator = new Animator(ref sprite, animData.GetAnim("Idle_L"));
        }

        #region Game Loop Updates
        /// <summary>
        /// Update input for a player using a keyboard
        /// </summary>
        public void KeyboardUpdate(GameTime gameTime)
        {
            //Horizontal movement
            velocity.X = MonoHelp.GetAxis(AxisType.HorizontalKeyboard) * moveSpeed;
            //Jumping
            if (MonoHelp.GetKeyDown(Keys.Space) /*&& grounded*/)
            {
                velocity.Y = 20;
                grounded = false;
            }
            //Attacking
            if (MonoHelp.GetKeyDown(Keys.F) && !attacking)
            {
                //Attack
                Debug.WriteLine("Attacking!");
                //Set attack cooldown
                attackTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
                attacking = true;
            }
        }

        /// <summary>
        /// Update input for a player using a gamepad
        /// </summary>
        public void GamepadUpdate(GameTime gameTime)
        {
            //Horizontal movement
            velocity.X = MonoHelp.GetAxis(AxisType.GamePadLeftHorizontal, controlType) * moveSpeed;
            //Jumping
            if (MonoHelp.GetButtonDown(controlType, Buttons.A) && grounded)
            {
                velocity.Y = 20;
                grounded = false;
            }
            //Attacking
            if (MonoHelp.GetButtonDown(controlType, Buttons.A) && !attacking)
            {
                //Attack
                Debug.WriteLine("Attacking!");
                //Set attack cooldown
                attackTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
                attacking = true;
            }
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

            //Check if the player is on the bottom of the screen so he can jump
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

        //I love making physics I love making physics I love making physics I love making physics I love making physics
        public void CheckCollision()
        {
            grounded = false;
            foreach (var tile in mapManager.tiles)
            {
                if (Rect.Intersects(tile.Rect))
                {
                    Vector2 distVec;
                    bool right = false;
                    bool top = false;

                    if (Math.Abs(Rect.Right - tile.Rect.Left) < Math.Abs(Rect.Left - tile.Rect.Right))
                        distVec.X = Rect.Right - tile.Rect.Left;
                    else
                    {
                        distVec.X = Rect.Left - tile.Rect.Right;
                        right = true;
                    }
                    if (Math.Abs(Rect.Bottom - tile.Rect.Top) < Math.Abs(Rect.Top - tile.Rect.Bottom))
                    {
                        distVec.Y = Rect.Bottom - tile.Rect.Top;
                        top = true;
                    }
                    else
                        distVec.Y = Rect.Top - tile.Rect.Bottom;
                    if (right)
                    {
                        if (Math.Abs(distVec.X) < Math.Abs(distVec.Y))
                        {
                            position.X = tile.Rect.Right + sprite.Width / 2;
                            velocity.X = 0;
                        }
                        else
                        {
                            if (top)
                            {
                                //Decrease slightly to constantly collide and not fk up the grounded flag
                                position.Y = tile.Rect.Top - sprite.Height / 2;
                                velocity.Y = 0;
                                grounded = true;
                            }
                            else
                            {
                                position.Y = tile.Rect.Bottom + sprite.Height / 2;
                                velocity.Y = 0;
                            }
                        }
                    }
                    else
                    {
                        if (Math.Abs(distVec.X) < Math.Abs(distVec.Y))
                        {
                            position.X = tile.Rect.Left - sprite.Width / 2;
                            velocity.X = 0;
                        }
                        else
                        {
                            if (top)
                            {
                                //Decrease slightly to constantly collide and not fk up the grounded flag
                                position.Y = tile.Rect.Top - sprite.Height / 2;
                                velocity.Y = 0;
                                grounded = true;
                            }
                            else
                            {
                                position.Y = tile.Rect.Bottom + sprite.Height / 2;
                                velocity.Y = 0;
                            }
                        }
                    }
                }
            }
        }
    }
}
