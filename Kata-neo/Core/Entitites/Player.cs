using KataNeo.Animation;
using KataNeo.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

namespace KataNeo.Entitites
{
    public class Player : Entity
    {
        //General vars
        public Texture2D sprite;
        public AnimData animData;
        public Animator animator;

        public ControlType controlType;
        private MapManager mapManager;
        public Attack attack;

        public int Health { get; private set; } = 100;
        //Unused for now, maybe will be used for a lying body after death
        bool alive = true;

        //Movement vars
        public Vector2 position;
        public Vector2 scale = new Vector2(4, 4);
        public Rectangle Rect
        {
            get => new Rectangle((int)(position.X - sprite.Width * scale.X / 2f), (int)(position.Y - sprite.Height * scale.Y / 2f),
            (int)(sprite.Width * scale.X), (int)(sprite.Height * scale.Y));
        }
        private Vector2 input;
        public Vector2 velocity;
        private readonly Vector2 baseVelocity = new Vector2(0, -19.62f);
        public float moveSpeed = 3f;
        private bool flipped = false;
        bool grounded = false;

        //Attack vars
        private Vector2 attackOffset = new Vector2(50, 50);
        float attackCooldown = 0.3f;
        float attackDelay = 0.075f;
        float attackForce = 15f;
        bool attacking = false;
        bool canAttack = true;

        public Player(ControlType controlType, MapManager mapManager, AnimData animData, Vector2 position)
        {
            this.position = position;
            this.controlType = controlType;
            this.mapManager = mapManager;
            this.animData = animData;
            animator = new Animator(ref sprite, animData.GetAnim("Idle_R"), UpdateTex);
        }

        #region Game Loop Updates
        /// <summary>
        /// Update input for a player using a keyboard
        /// </summary>
        public void KeyboardUpdate(GameTime gameTime)
        {
            input = new Vector2(MonoHelp.GetAxis(AxisType.HorizontalKeyboard), MonoHelp.GetAxis(AxisType.VerticalKeyboard));
            //Horizontal movement, don't add when player is too fast horizontally
            if (Math.Abs(velocity.X) < 6) velocity.X += input.X * moveSpeed;
            //Jumping
            if (MonoHelp.GetKeyDown(Keys.Space) && grounded)
            {
                velocity.Y = 20;
                grounded = false;
            }
            //Attacking
            if (MonoHelp.GetKeyDown(Keys.K) && canAttack)
            {
                animator.ChangeAnim(animData.GetAnim(flipped ? "Swing_L" : "Swing_R"));
                if (!grounded)
                {
                    velocity += new Vector2(attackForce * input.X, attackForce * input.Y);
                }
                //Attack
                if (input.X == 0 && input.Y == 0)
                    attack = new Attack(new Vector2(attackOffset.X * (flipped ? -1 : 1), attackOffset.Y * input.Y), this);
                else
                    attack = new Attack(new Vector2(attackOffset.X * input.X, attackOffset.Y * input.Y), this);
                //Set attack cooldown
                MonoHelp.AddTimer(attackDelay, EndSwing);
                MonoHelp.AddTimer(attackCooldown, RenewAttackCD);
                canAttack = false;
                attacking = true;
            }
        }

        /// <summary>
        /// Update input for a player using a gamepad
        /// </summary>
        public void GamepadUpdate(GameTime gameTime)
        {
            input = new Vector2(MonoHelp.GetAxis(AxisType.GamePadLeftHorizontal, controlType), MonoHelp.GetAxis(AxisType.GamePadLeftVertical, controlType));
            //Horizontal movement, don't add when player is too fast horizontally
            if (Math.Abs(velocity.X) < 6) velocity.X = input.X * moveSpeed;
            //Jumping
            if (MonoHelp.GetButtonDown(controlType, Buttons.A) && grounded)
            {
                velocity.Y = 20;
                grounded = false;
            }
            //Attacking
            if (MonoHelp.GetButtonDown(controlType, Buttons.X) && canAttack)
            {
                if (!grounded)
                {
                    velocity += new Vector2(attackForce * input.X, attackForce * input.Y);
                }
                //Attack
                if (input.X == 0 && input.Y == 0)
                    attack = new Attack(new Vector2(attackOffset.X * (flipped ? -1 : 1), attackOffset.Y * input.Y), this);
                else
                    attack = new Attack(new Vector2(attackOffset.X * input.X, attackOffset.Y * input.Y), this);
                //Set attack cooldown
                canAttack = false;
                attacking = true;
            }
        }

        public override void Update(GameTime gameTime)
        {
            //Debug.WriteLine($"Player {(int)controlType} velocity: {velocity}");
            if (attacking)
                attack.Update(gameTime, position);

            //Apply the velocity to the player and dampen it if grounded and not moving
            if (grounded && input.X == 0) velocity.X *= 0.95f;
            position += new Vector2(velocity.X, -velocity.Y);

            //Set idle animation if not performing an action to the right direction
            flipped = input.X < 0;
            if (!attacking)
            {
                if (MathF.Abs(velocity.X) <= 0.5)
                {
                    animator.ChangeAnim(animData.GetAnim(flipped ? "Idle_L" : "Idle_R"));
                }
                else
                {
                    animator.ChangeAnim(animData.GetAnim(flipped ? "Run_L" : "Run_R"));
                }
            }

            //Decrease velocity and check collision
            velocity = Mathf.Lerp(velocity, baseVelocity, 0.05f);
            CheckCollision();

            //Check if the player is on the bottom of the screen so he can jump
            if (position.Y > 1080 - sprite.Height * scale.Y / 2)
            {
                grounded = true;
            }
            //Confine the player within the game window
            if (alive)
            {
                position = new Vector2(Mathf.Clamp(position.X, 0 + sprite.Width * scale.X / 2, 1920 - sprite.Width * scale.X / 2),
                    Mathf.Clamp(position.Y, 0 + sprite.Height * scale.Y / 2, 1080 - sprite.Height * scale.Y / 2));
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (attacking) attack.Draw(gameTime, spriteBatch);
            spriteBatch.Draw(sprite, position, null, Color.White, 0f,
                new Vector2(sprite.Width / 2, sprite.Height / 2), scale, SpriteEffects.None, 0f);
        }
        #endregion

        void UpdateTex(Texture2D tex) => sprite = tex;

        void EndSwing()
        {
            attack = null;
            attacking = false;
            animator.ChangeAnim(animData.GetAnim(flipped ? "Idle_L" : "Idle_R"));
        }

        void RenewAttackCD() => canAttack = true;

        /// <summary>
        /// Take damage
        /// </summary>
        /// <param name="val">Damage to be taken</param>
        /// <returns>If the player died or not</returns>
        public bool TakeDamage(int val)
        {
            Health -= val;
            if (Health <= 0) return true;
            return false;
        }

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
                            position.X = tile.Rect.Right + sprite.Width * scale.X / 2;
                            velocity.X = 0;
                        }
                        else
                        {
                            if (top)
                            {
                                //Decrease slightly to constantly collide and not fk up the grounded flag
                                position.Y = tile.Rect.Top - sprite.Height * scale.Y / 2 + 1;
                                velocity.Y = 0;
                                grounded = true;
                            }
                            else
                            {
                                position.Y = tile.Rect.Bottom + sprite.Height * scale.Y / 2;
                                velocity.Y = 0;
                            }
                        }
                    }
                    else
                    {
                        if (Math.Abs(distVec.X) < Math.Abs(distVec.Y))
                        {
                            position.X = tile.Rect.Left - sprite.Width * scale.X / 2;
                            velocity.X = 0;
                        }
                        else
                        {
                            if (top)
                            {
                                //Decrease slightly to constantly collide and not fk up the grounded flag
                                position.Y = tile.Rect.Top - sprite.Height * scale.Y / 2 + 1;
                                velocity.Y = 0;
                                grounded = true;
                            }
                            else
                            {
                                position.Y = tile.Rect.Bottom + sprite.Height * scale.Y / 2;
                                velocity.Y = 0;
                            }
                        }
                    }
                }
            }
        }
    }
}
