using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System;

namespace Pong
{
    public class Paddle
    {
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public int Speed { get; set; }

        public Paddle(Texture2D Texture, Vector2 Position)
        {
            this.Texture = Texture;
            this.Position = Position;
        }

        public Paddle() { }

        public void Initialize()
        {
            Speed = 5;
        }

        public void Update(KeyboardState keyboardState, Keys upKey, Keys downKey)
        {
            if (keyboardState.IsKeyDown(upKey))
            {
                Position = new Vector2(Position.X, Position.Y - Speed);
            }
            if (keyboardState.IsKeyDown(downKey))
            {
                Position = new Vector2(Position.X, Position.Y + Speed);
            }

            // Limitar el movimiento de la paleta para que no se salga de la pantalla
            Position = new Vector2(MathHelper.Clamp(Position.X, 0, 800 - Texture.Width), MathHelper.Clamp(Position.Y, 0, 600 - Texture.Height));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }
    }

    public class Ball
    {
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        private int Speed;
        private int x; // Dirección de la pelota
        private int y; // Dirección de la pelota

        public void Update()
        {
            Position = new Vector2(Position.X + x * Speed, Position.Y + y * Speed);

            // Colision con las paredes
            if (Position.Y < 0 || Position.Y > 600 - Texture.Height)
            {
                y = -y;
            }
        }

        public void colideDetection(Paddle player1, Paddle player2)
        {
            // Calculo para detectar el rango de colisión en la paleta
            Rectangle ballRect = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
            Rectangle player1Rect = new Rectangle((int)player1.Position.X, (int)player1.Position.Y, player1.Texture.Width, player1.Texture.Height);
            Rectangle player2Rect = new Rectangle((int)player2.Position.X, (int)player2.Position.Y, player2.Texture.Width, player2.Texture.Height);

            if (ballRect.Intersects(player1Rect) || ballRect.Intersects(player2Rect))
            {
                x = -x;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }

        public void Initialize(int Height, int Width)
        {
            Speed = 5;
            x = 1;
            y = 1;
            Position = new Vector2(Width / 2, Height / 2);
        }
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private bool _gamePaused;
        private KeyboardState _previousKeyboardState;

        private Paddle _player1Paddle;
        private Paddle _player2Paddle;
        private int _player1Score;
        private int _player2Score;
        private Ball _ball;
        public int Height { get; private set; }
        public int Width { get; private set; }
        private enum GameState
        {
            Start,
            Playing,
            Paused,
            GameOver
        }
        private GameState _currentGameState;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
            _gamePaused = false;
            _previousKeyboardState = Keyboard.GetState();
        }

        // Función para detectar si un jugador hace un punto
        public void pointDetection()
        {
            if (_ball.Position.X == 0)
            {
                // Punto para el jugador 2
                _ball.Position = new Vector2(Width / 2, Height / 2);
            }
            if (_ball.Position.X == Width - 100)
            {
                // Punto para el jugador 1
                _ball.Position = new Vector2(Width / 2, Height / 2);
            }
        }

        public void showScore()
        {
            // Mostrar el marcador en la pantalla

        }

        public void showWinner()
        {
            // Mostrar el ganador en la pantalla
        }
        public void resetGame()
        {
            _player1Paddle.Position = new Vector2(Width - _player1Paddle.Texture.Width, 50);
            _player2Paddle.Position = new Vector2(50, 50);
            _ball.Initialize(Height, Width);
        }


        public void madePoint()
        {
            // Aumentar el marcador y mostrar el mensaje de punto, reiniciar la posición de la pelota y paletas

        }

        protected override void Initialize()
        {
            Width = 800;
            Height = 600;
            _graphics.PreferredBackBufferWidth = Width;
            _graphics.PreferredBackBufferHeight = Height;
            _graphics.ApplyChanges();

            // Definición de las posiciones al iniciar el juego
            _player1Paddle = new Paddle();
            _player1Paddle.Initialize();
            _player2Paddle = new Paddle();
            _player2Paddle.Initialize();
            _ball = new Ball();
            _ball.Initialize(Height, Width);

            _currentGameState = GameState.Start;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Cargar texturas
            _player1Paddle.Texture = Content.Load<Texture2D>("paleta");
            _player2Paddle.Texture = Content.Load<Texture2D>("paleta");
            _player1Paddle.Position = new Vector2(Width - _player1Paddle.Texture.Width, 50);
            _ball.Texture = Content.Load<Texture2D>("pelota");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState keyboardState = Keyboard.GetState();

            switch(_currentGameState)
            {
                case GameState.Start:
                    // Mostrar mensaje de inicio
                    if (keyboardState.IsKeyDown(Keys.Enter))
                    {
                        _currentGameState = GameState.Playing;
                    }
                    break;
                case GameState.Playing:
                    // Actualizar la lógica del juego
                    // Actualizar paletas
                    if(keyboardState.IsKeyDown(Keys.P) && !_previousKeyboardState.IsKeyDown(Keys.P))
                    {
                        _currentGameState = GameState.Paused;
                    }
                    if(!_gamePaused)
                    {
                        _player1Paddle.Update(keyboardState, Keys.W, Keys.S);
                        _player2Paddle.Update(keyboardState, Keys.Up, Keys.Down);

                        // Actualizar pelota
                        _ball.colideDetection(_player1Paddle, _player2Paddle);
                        pointDetection();
                        _ball.Update();

                    }    

                    break;
                case GameState.Paused:
                    // Mostrar mensaje de pausa
                    if (keyboardState.IsKeyDown(Keys.P) && !_previousKeyboardState.IsKeyDown(Keys.P))
                    {
                        _currentGameState = GameState.Playing;
                    }
                    break;
                case GameState.GameOver:
                    // Mostrar mensaje de fin de juego
                    if (keyboardState.IsKeyDown(Keys.Enter))
                    {
                        resetGame();
                        _currentGameState = GameState.Start;
                    }
                    break;
            }

            // Alternar el estado de pausa cuando se presiona la tecla P
            if (keyboardState.IsKeyDown(Keys.P) && !_previousKeyboardState.IsKeyDown(Keys.P))
            {
                _gamePaused = !_gamePaused;
            }


            // Si el juego está en pausa, no actualizamos la lógica del juego
            if (_gamePaused)
            {
                return;
            }



            _previousKeyboardState = keyboardState;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            switch (_currentGameState)
            {
                case GameState.Start:
                    DrawInicioScreen();
                    break;
                case GameState.Playing:
                    _player1Paddle.Draw(_spriteBatch);
                    _player2Paddle.Draw(_spriteBatch);
                    _ball.Draw(_spriteBatch);
                    break;
                case GameState.Paused:
                    DrawPauseScreen();
                    break;
                case GameState.GameOver:
                    DrawFinJuegoScreen();
                    break;
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawInicioScreen()
        {
            // Dibujar la pantalla de inicio
            _spriteBatch.DrawString(Content.Load<SpriteFont>("Font"), "Presiona ENTER para comenzar", new Vector2(Width / 2 - 150, Height / 2), Color.White); //"Font" es el nombre del archivo de fuente que se encuentra en la carpeta Content, de tipo .spritefont
        }

        private void DrawPauseScreen()
        {
            // Dibujar la pantalla de pausa
            _spriteBatch.DrawString(Content.Load<SpriteFont>("Font"), "Juego Pausado. Presiona P para continuar", new Vector2(Width / 2 - 150, Height / 2), Color.White);
        }

        private void DrawFinJuegoScreen()
        {
            // Dibujar la pantalla de fin de juego
            _spriteBatch.DrawString(Content.Load<SpriteFont>("Font"), "Fin del juego. Presiona ENTER para reiniciar", new Vector2(Width / 2 - 150, Height / 2), Color.White);
        }

    }
}
