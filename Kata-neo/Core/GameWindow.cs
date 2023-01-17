using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace KataNeo
{
    public class GameWindow : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public EntityManager entityManager;
        public MapManager mapManager;
        private List<ControlType> _usedControls;

#if DEBUG
        private Texture2D debugOutline;
#endif

        public GameWindow()
        {
            _graphics = new GraphicsDeviceManager(this);
            Window.IsBorderless = true;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        static void Main()
        {
            using (var game = new GameWindow())
                game.Run();
        }

        protected override void Initialize()
        {
            _usedControls = new List<ControlType>();
            entityManager = new EntityManager(this);
            mapManager = new MapManager();

            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.ApplyChanges();

            //Init states so errors wont be thrown at the first frame
            MonoHelp.curState = Keyboard.GetState();
            List<GamePadState> padStates = new List<GamePadState>();
            foreach (PlayerIndex playerIndex in (PlayerIndex[])Enum.GetValues(typeof(PlayerIndex)))
            {
                padStates.Add(GamePad.GetState(playerIndex));
            }
            MonoHelp.gamePadStates = padStates.ToArray();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            mapManager.LoadMap("Maps/Map1.smap", this);
            //Comment for now as first player defaulting to keyboard may not be the wanted result
            //entityManager.AddPlayer(ControlType.Keyboard, mapManager);
#if DEBUG
            debugOutline = Content.Load<Texture2D>("DebugSprites/OutlineCollider");
#endif
        }

        protected override void Update(GameTime gameTime)
        {
            MonoHelp.prevState = MonoHelp.curState;
            MonoHelp.curState = Keyboard.GetState();
            MonoHelp.prevGamePadStates = MonoHelp.gamePadStates;
            List<GamePadState> padStates = new List<GamePadState>();
            foreach (PlayerIndex playerIndex in (PlayerIndex[])Enum.GetValues(typeof(PlayerIndex)))
            {
                padStates.Add(GamePad.GetState(playerIndex));
            }
            MonoHelp.gamePadStates = padStates.ToArray();

            if (MonoHelp.GetKeyDown(Keys.F1)) Window.IsBorderless = true;
            if (MonoHelp.GetKeyDown(Keys.F2)) Window.IsBorderless = false;
            //Join players
            if (MonoHelp.GetKeyDown(Keys.Enter) && !_usedControls.Contains(ControlType.Keyboard))
            {
                _usedControls.Add(ControlType.Keyboard);
                entityManager.AddPlayer(ControlType.Keyboard, mapManager);
            }
            if (MonoHelp.GetButtonDown(PlayerIndex.One, Buttons.A) && !_usedControls.Contains(ControlType.Gamepad1))
            {
                _usedControls.Add(ControlType.Gamepad1);
                entityManager.AddPlayer(ControlType.Gamepad1, mapManager);
            }
            if (MonoHelp.GetButtonDown(PlayerIndex.Two, Buttons.A) && !_usedControls.Contains(ControlType.Gamepad2))
            {
                _usedControls.Add(ControlType.Gamepad2);
                entityManager.AddPlayer(ControlType.Gamepad2, mapManager);
            }
            if (MonoHelp.GetButtonDown(PlayerIndex.Three, Buttons.A) && !_usedControls.Contains(ControlType.Gamepad3))
            {
                _usedControls.Add(ControlType.Gamepad3);
                entityManager.AddPlayer(ControlType.Gamepad3, mapManager);
            }
            if (MonoHelp.GetButtonDown(PlayerIndex.Four, Buttons.A) && !_usedControls.Contains(ControlType.Gamepad4))
            {
                _usedControls.Add(ControlType.Gamepad4);
                entityManager.AddPlayer(ControlType.Gamepad4, mapManager);
            }
            //Update entities
            entityManager.Update(gameTime,
                new InputState(Keyboard.GetState(), new GamePadState[4]
                { GamePad.GetState(PlayerIndex.One), GamePad.GetState(PlayerIndex.Two), GamePad.GetState(PlayerIndex.Three), GamePad.GetState(PlayerIndex.Four) }));

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DimGray);

            _spriteBatch.Begin(samplerState: SamplerState.PointWrap);
            //Draw tiles
            mapManager.Draw(gameTime, _spriteBatch);
            //Draw entities
            entityManager.Draw(gameTime, _spriteBatch);
#if DEBUG
            //Debug drawing
            foreach (var tile in mapManager.tiles)
            {
                _spriteBatch.Draw(debugOutline, tile.Rect, Color.White);
            }
            foreach (var player in entityManager._players)
            {
                _spriteBatch.Draw(debugOutline, player.Rect, Color.White);
            }
#endif
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}