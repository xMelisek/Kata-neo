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
        public Atlas atlas;
        public Animator animator;

        public ControlType controlType;
        private MapManager mapManager;
        public Attack attack;

        private PlayerState playerState;
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
        /// <summary>
        /// The velocity to which the player goes back to
        /// </summary>
        private readonly Vector2 baseVelocity = new Vector2(0, -19.62f);
        public float moveSpeed = 3f;
        private Vector2 wallJumpAngle = new Vector2(0.5f, 1);
        private float wallJumpForce = 15f;
        private bool flipped = false;
        bool grounded = false;
        string wallcling;
        bool crouching = false;

        //Attack vars
        private Vector2 attackOffset = new Vector2(50, 50);
        float attackCooldown = 0.3f;
        float attackDelay = 0.075f;
        float attackForce = 15f;
        bool attacking = false;
        bool canAttack = true;

        public Player(ControlType controlType, MapManager mapManager, Atlas atlas, Vector2 position)
        {
            this.position = position;
            this.controlType = controlType;
            this.mapManager = mapManager;
            this.atlas = atlas;
            animator = new Animator(atlas.GetAnim("Idle"), UpdateTex);
        }

        #region Game Loop Updates
        /// <summary>
        /// Update input for a player using a keyboard
        /// </summary>
        public void KeyboardUpdate(GameTime gameTime)
        {
            input = new Vector2(Input.GetAxis(Input.AxisType.HorizontalKeyboard), Input.GetAxis(Input.AxisType.VerticalKeyboard));
            //Horizontal movement, don't add when player is too fast horizontally
            if (Math.Abs(velocity.X) < 6) velocity.X += input.X * moveSpeed;
            //Jumping and walljumping
            if (Input.GetKeyDown(Keys.Space))
            {
                if(grounded)
                {
                    velocity.Y = 20;
                    grounded = false;
                }
                else if (wallcling != null)
                {
                    velocity += new Vector2(wallcling == "right" ? wallJumpAngle.X : -wallJumpAngle.X, wallJumpAngle.Y) * wallJumpForce;
                    wallcling = null;
                }
            }
            //Attacking
            if (Input.GetKeyDown(Keys.K) && canAttack)
            {
                animator.ChangeAnim(atlas.GetAnim("Swing"));
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
                Timer.AddTimer(attackDelay, EndSwing);
                Timer.AddTimer(attackCooldown, RenewAttackCD);
                canAttack = false;
                attacking = true;
            }
            //Crouching
            crouching = Input.GetKey(Keys.S) && grounded && Math.Floor(Math.Abs(velocity.X)) == 0 && !attacking;
        }

        /// <summary>
        /// Update input for a player using a gamepad
        /// </summary>
        public void GamepadUpdate(GameTime gameTime)
        {
            input = new Vector2(MathF.Ceiling(Input.GetAxis(Input.AxisType.GamePadLeftHorizontal, controlType)), MathF.Ceiling(Input.GetAxis(Input.AxisType.GamePadLeftVertical, controlType)));
            //Horizontal movement, don't add when player is too fast horizontally
            if (Math.Abs(velocity.X) < 6) velocity.X += input.X * moveSpeed;
            //Jumping
            if (Input.GetButtonDown(controlType, Buttons.A) && grounded)
            {
                velocity.Y = 20;
                grounded = false;
            }
            //Attacking
            if (Input.GetButtonDown(controlType, Buttons.X) && canAttack)
            {
                animator.ChangeAnim(atlas.GetAnim("Swing"));
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
                Timer.AddTimer(attackDelay, EndSwing);
                Timer.AddTimer(attackCooldown, RenewAttackCD);
                canAttack = false;
                attacking = true;
            }
            //Crouching
            crouching = input.Y < 0 && grounded && Math.Floor(Math.Abs(velocity.X)) == 0 && !attacking;
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
            animator.Update(gameTime);
            if (input.X > 0) flipped = false;
            else if (input.X < 0) flipped = true;

            //Should probably make it being changed in proper places, but i'm too dumb and lazy
            if (grounded)
                playerState = PlayerState.Grounded;
            else if (attacking)
                playerState = PlayerState.Attacking;
            else if (wallcling != null)
                playerState = PlayerState.Walljumped;
            else
                playerState = PlayerState.Airborne;

            //Decrease velocity and check collision
            switch(playerState)
            {
                case PlayerState.Grounded:
                    velocity = Mathf.Lerp(velocity, Vector2.Zero, 0.05f);
                    break;
                case PlayerState.Airborne:
                    velocity = Mathf.Lerp(velocity, baseVelocity, 0.05f);
                    break;
                case PlayerState.Walljumped:
                    velocity = Mathf.Lerp(velocity, baseVelocity, velocity.Y < 0 ? 0.01f : 0.04f);
                    break;
                case PlayerState.Attacking:
                    velocity = Mathf.Lerp(velocity, baseVelocity, 0.025f);
                    break;
            }
            
            CheckCollision();

            //Check if the player is on the bottom of the screen so he can jump
            //Fix the player being on the wallcling animation when on the bottom border and hugging a wall
            if (position.Y > 1080 - sprite.Height * scale.Y / 2 - 1)
            {
                grounded = true;
                wallcling = null;
            }
            //Put change anim checks after collision checking otherwise crouch is buggy on objects
            //And put it after the groundcheck on bottom border so wallcling won't be buggy
            if (!attacking)
            {
                if (crouching) animator.ChangeAnim(atlas.GetAnim("Crouch"));
                else if (MathF.Abs(velocity.X) <= 0.5 && grounded)
                {
                    animator.ChangeAnim(atlas.GetAnim("Idle"));
                }
                else if (wallcling != null && !grounded)
                    animator.ChangeAnim(atlas.GetAnim("Wallcling"));
                else
                {
                    animator.ChangeAnim(atlas.GetAnim("Run"));
                }
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
            if(wallcling != null && !grounded)
                spriteBatch.Draw(sprite, position, null, Color.White, 0f,
                    new Vector2(sprite.Width / 2, sprite.Height / 2), scale, wallcling == "right" ? SpriteEffects.None: SpriteEffects.FlipHorizontally, 0f);
            else
                spriteBatch.Draw(sprite, position, null, Color.White, 0f,
                    new Vector2(sprite.Width / 2, sprite.Height / 2), scale, flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
        }
        #endregion

        //Callback for the animator to update the texture
        void UpdateTex(Texture2D tex) => sprite = tex;

        //Attack timer callbacks
        void EndSwing()
        {
            attack = null;
            attacking = false;
            animator.ChangeAnim(atlas.GetAnim("Idle"));
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
            wallcling = null;
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
                        //Confine the player on the right side if he is on it
                        if (Math.Abs(distVec.X) < Math.Abs(distVec.Y))
                        {
                            wallcling = "right";
                            position.X = tile.Rect.Right + sprite.Width * scale.X / 2 - 3;
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
                            wallcling = null;
                        }
                    }
                    else
                    {
                        //Confine the player on the left side if he is on it
                        if (Math.Abs(distVec.X) < Math.Abs(distVec.Y))
                        {
                            wallcling = "left";
                            position.X = tile.Rect.Left - sprite.Width * scale.X / 2 + 3;
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
                            wallcling = null;
                        }
                    }
                }
            }
        }

        enum PlayerState
        {
            Grounded,
            Airborne,
            Walljumped,
            Attacking
        }
    }
}
