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

        private bool transition;
        private Texture2D transitionTex;
        private Vector2 transitionPos = new Vector2(-2560, 0);

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

            TargetElapsedTime = TimeSpan.FromSeconds(1d / 60d);
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();

            //Window.AllowUserResizing = true;
            //Window.ClientSizeChanged += OnResize;

            MonoHelp.GameWindow = this;
            //Init states so errors wont be thrown at the first frame
            Input.curState = Keyboard.GetState();
            List<GamePadState> padStates = new List<GamePadState>();
            foreach (PlayerIndex playerIndex in (PlayerIndex[])Enum.GetValues(typeof(PlayerIndex)))
            {
                padStates.Add(GamePad.GetState(playerIndex));
            }
            Input.gamePadStates = padStates.ToArray();

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

        private void OnResize(object sender, EventArgs e)
        {
            Point bounds = CalcResolution();
            InitializeResolutionIndependence(bounds.X, bounds.Y);
        }

        private Point CalcResolution()
        {
            // Get window width multiplier
            float multiplier = Window.ClientBounds.Width / 16f;
            //Check if width multiplier will fit into window height
            while (9 * multiplier > Window.ClientBounds.Height)
                multiplier--;
            return new Point((int)(16 * multiplier), (int)(9 * multiplier));
        }

        protected override void LoadContent()
        {
            MonoHelp.Content = Content;
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            mapManager = new MapManager();
            transitionTex = Content.Load<Texture2D>("Transition");
            //mapManager.ExportMap("Maps/PlaygroundMap");
            KDebug.debugOutline = Content.Load<Texture2D>("DebugSprites/OutlineCollider");
        }

        protected override void Update(GameTime gameTime)
        {
            MonoHelp.Content = Content;
            Timer.Update(gameTime);
            //Update input states for input handling
            Input.prevState = Input.curState;
            Input.curState = Keyboard.GetState();
            Input.prevGamePadStates = Input.gamePadStates;
            List<GamePadState> padStates = new List<GamePadState>();
            foreach (PlayerIndex playerIndex in (PlayerIndex[])Enum.GetValues(typeof(PlayerIndex)))
            {
                padStates.Add(GamePad.GetState(playerIndex));
            }
            Input.gamePadStates = padStates.ToArray();

            if (Input.GetKeyDown(Keys.F1)) Window.IsBorderless = true;
            if (Input.GetKeyDown(Keys.F2)) Window.IsBorderless = false;
            //Join players
            if (Input.GetKeyDown(Keys.Enter) && !_usedControls.Contains(ControlType.Keyboard))
            {
                entityManager.AddPlayer(ControlType.Keyboard, mapManager, mapManager.spawnPoses[_usedControls.Count]);
                _usedControls.Add(ControlType.Keyboard);
            }
            if (Input.GetButtonDown(PlayerIndex.One, Buttons.A) && !_usedControls.Contains(ControlType.Gamepad1))
            {
                entityManager.AddPlayer(ControlType.Gamepad1, mapManager, mapManager.spawnPoses[_usedControls.Count]);
                _usedControls.Add(ControlType.Gamepad1);
            }
            if (Input.GetButtonDown(PlayerIndex.Two, Buttons.A) && !_usedControls.Contains(ControlType.Gamepad2))
            {
                entityManager.AddPlayer(ControlType.Gamepad2, mapManager, mapManager.spawnPoses[_usedControls.Count]);
                _usedControls.Add(ControlType.Gamepad2);
            }
            if (Input.GetButtonDown(PlayerIndex.Three, Buttons.A) && !_usedControls.Contains(ControlType.Gamepad3))
            {
                entityManager.AddPlayer(ControlType.Gamepad3, mapManager, mapManager.spawnPoses[_usedControls.Count]);
                _usedControls.Add(ControlType.Gamepad3);
            }
            if (Input.GetButtonDown(PlayerIndex.Four, Buttons.A) && !_usedControls.Contains(ControlType.Gamepad4))
            {
                entityManager.AddPlayer(ControlType.Gamepad4, mapManager, mapManager.spawnPoses[_usedControls.Count]);
                _usedControls.Add(ControlType.Gamepad4);
            }
            //Update entities
            entityManager.Update(gameTime);
            mapManager.Update(gameTime);

            if (transition) transitionPos = Mathf.Lerp(transitionPos, new Vector2(-320, 0), 0.05f);
            else transitionPos = Mathf.Lerp(transitionPos, new Vector2(-2560, 0), 0.05f);
            if (MathF.Floor(transitionPos.X) == -320 && transition)
            {
                mapManager.LoadMap(UnTransition);
            }

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
            mapManager.DrawFg(gameTime, _spriteBatch);
            KDebug.Draw(mapManager, entityManager, _spriteBatch);
            _spriteBatch.Draw(transitionTex, transitionPos, Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        //Should maybe put it later into its own thing
        public void Transition()
        {
            transitionPos = new Vector2(2560, 0);
            transition = true;
        }

        public void UnTransition()
        {
            entityManager.players.Clear();
            foreach (var control in _usedControls)
            {
                entityManager.AddPlayer(control, mapManager, mapManager.spawnPoses[_usedControls.IndexOf(control)]);
            }
            transition = false;
        }
    }
}