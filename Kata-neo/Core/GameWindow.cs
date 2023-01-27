using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace KataNeo
{
    public class GameWindow : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private ResolutionIndependentRenderer _resolutionIndependence;
        private Camera2D _camera;

        private List<ControlType> _usedControls;
        public EntityManager entityManager;
        public MapManager mapManager;

#if DEBUG
        private Texture2D debugOutline;
#endif

        public GameWindow()
        {
            _graphics = new GraphicsDeviceManager(this);
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
            _resolutionIndependence = new ResolutionIndependentRenderer(this);
            _camera = new Camera2D(_resolutionIndependence);
            InitializeResolutionIndependence(1280, 720);
            _camera.Zoom = 1f;
            _camera.Position = new Vector2(_resolutionIndependence.VirtualWidth / 2, _resolutionIndependence.VirtualHeight / 2);
            _usedControls = new List<ControlType>();
            entityManager = new EntityManager(this);
            mapManager = new MapManager();

            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
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

        //Initialize Independent resolution and camera
        private void InitializeResolutionIndependence(int realScreenWidth, int realScreenHeight)
        {
            _resolutionIndependence.VirtualWidth = 1920;
            _resolutionIndependence.VirtualHeight = 1080;
            _resolutionIndependence.ScreenWidth = realScreenWidth;
            _resolutionIndependence.ScreenHeight = realScreenHeight;
            _resolutionIndependence.Initialize();

            _camera.RecalculateTransformationMatrices();
        }

        protected override void LoadContent()
        {
            MonoHelp.Content = Content;
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            //mapManager.ExportMap("Maps/PlaygroundMap");
            mapManager.LoadMap("Maps/Map1", this);
            //Comment for now as first player defaulting to keyboard may not be the wanted result
            //entityManager.AddPlayer(ControlType.Keyboard, mapManager);
#if DEBUG
            debugOutline = Content.Load<Texture2D>("DebugSprites/OutlineCollider");
#endif
        }

        protected override void Update(GameTime gameTime)
        {
            MonoHelp.Content = Content;
            //Update input states for input handling
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
                entityManager.AddPlayer(ControlType.Keyboard, mapManager, mapManager.spawnPoses[_usedControls.Count]);
                _usedControls.Add(ControlType.Keyboard);
            }
            if (MonoHelp.GetButtonDown(PlayerIndex.One, Buttons.A) && !_usedControls.Contains(ControlType.Gamepad1))
            {
                entityManager.AddPlayer(ControlType.Gamepad1, mapManager, mapManager.spawnPoses[_usedControls.Count]);
                _usedControls.Add(ControlType.Gamepad1);
            }
            if (MonoHelp.GetButtonDown(PlayerIndex.Two, Buttons.A) && !_usedControls.Contains(ControlType.Gamepad2))
            {
                entityManager.AddPlayer(ControlType.Gamepad2, mapManager, mapManager.spawnPoses[_usedControls.Count]);
                _usedControls.Add(ControlType.Gamepad2);
            }
            if (MonoHelp.GetButtonDown(PlayerIndex.Three, Buttons.A) && !_usedControls.Contains(ControlType.Gamepad3))
            {
                entityManager.AddPlayer(ControlType.Gamepad3, mapManager, mapManager.spawnPoses[_usedControls.Count]);
                _usedControls.Add(ControlType.Gamepad3);
            }
            if (MonoHelp.GetButtonDown(PlayerIndex.Four, Buttons.A) && !_usedControls.Contains(ControlType.Gamepad4))
            {
                entityManager.AddPlayer(ControlType.Gamepad4, mapManager, mapManager.spawnPoses[_usedControls.Count]);
                _usedControls.Add(ControlType.Gamepad4);
            }
            //Update entities
            entityManager.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DimGray);

            _resolutionIndependence.BeginDraw();
            _spriteBatch.Begin(samplerState: SamplerState.PointWrap, transformMatrix: _camera.GetViewTransformationMatrix());
            //Draw tiles
            mapManager.Draw(gameTime, _spriteBatch);
            //Draw entities
            entityManager.Draw(gameTime, _spriteBatch);
#if DEBUG
            //Debug drawing
            foreach (var tile in mapManager.tiles)
            {
                _spriteBatch.Draw(debugOutline, tile.Rect, Color.Green);
            }
            foreach (var player in entityManager.players)
            {
                _spriteBatch.Draw(debugOutline, player.Rect, Color.Green);
                if(player.attack != null)
                    _spriteBatch.Draw(debugOutline, player.attack.Rect, Color.Red);
            }
#endif
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}