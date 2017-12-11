using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Threading;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using CrackerChase;
using Fizzyo_Library;

namespace GameFile
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        /// <summary>
        /// All the variables and structs and lists required to run the main part of the game excluding class specific variables which are called when required
        /// </summary>
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteBatch backgroundtexturelist;
        Player background;
        SoundEffect BurpSound;
        PlayerMover Player;
        List<Player> gameSprites = new List<Player>();
        List<Player> Backgrounds = new List<Player>();
        List<Enemies> e = new List<Enemies>();
        Random random = new Random();
        Stopwatch stopwatch = new Stopwatch();
        SpriteFont messageFont;
        GameStates CurrentState;

        

        enum GameStates
        {
            StartState,
            PlayingState
        };

        private Viewport viewport;
        public Texture2D Boat;
        public Texture2D AirShip;
        public Texture2D backgroundTexture;
        public Texture2D EnemyBoat;
        public Texture2D SubMarine;
        public Texture2D Shark;
        public Texture2D EnemyPlane;
        
        string messageString = "Press SPACE to start playing. "  /*Press 'S' to configure the breath recogniser or Press 'R' to reset the game"*/;
        public string fileName = @"C:\Users\harry\Desktop\GAMESAVE.txt";
        public int score;
        int screenWidth = 800;
        int screenHeight = 480;
        public int lives;
        public int[] HighScores = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };



        FizzyoDevice fizzyoDevice;
        InputState inputState;
        BreathRecogniser breathRecogniser;


        public float maxPressure = 0.4f;
        public float maxBreathLength = 3f;
        private bool GoodBreath = false;

        public string recordedDataPath = @"C:\Users\harry\Desktop\Working Versions\#5 Installation of Fizzyo Logic\CrackerChase\Data\FizzyoData_3min.fiz";


        /// <summary>
        /// Initially called to set the player position in the centre so when resuming the game after closing the program without a game over
        /// so the games resume is consistent and the player wont immediatly die
        /// </summary>
        void startPlayingGame()
        {
            foreach (Player s in gameSprites)
            {
                s.Reset();
            }
            messageString = "Harry's Game";
        }


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

           
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            inputState = new InputState(this);
            Services.AddService(typeof(InputState), inputState);

            fizzyoDevice = new FizzyoDevice(this);
            fizzyoDevice.useRecordedData = true;
            Services.AddService(typeof(FizzyoDevice), fizzyoDevice);

            breathRecogniser = new BreathRecogniser(maxPressure, maxBreathLength);

            LoadfizzyoRecordedData();


            base.Initialize();
        }

        /// <summary>
        /// Mostly loads textures for the player and enemies and sets screen size furthermore creates the first instances of the player and creates the background
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            CurrentState = GameStates.StartState;
            
            spriteBatch = new SpriteBatch(GraphicsDevice);
            messageFont = Content.Load<SpriteFont>("MessageFont");
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 480;
            screenWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            screenHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;

            Boat = Content.Load<Texture2D>("cheese");
            AirShip = Content.Load<Texture2D>("AirShip");
            backgroundTexture = Content.Load<Texture2D>("Ocean");
            EnemyBoat = Content.Load<Texture2D>("cracker");
            SubMarine = Content.Load<Texture2D>("SubMarine");
            EnemyPlane = Content.Load<Texture2D>("EnemyPlane");
            Shark = Content.Load<Texture2D>("Shark");
            BurpSound = Content.Load<SoundEffect>("Burp");
            int PlayerWidth = screenWidth / 10;
            Player = new PlayerMover(screenWidth, screenHeight, Boat, PlayerWidth, screenWidth / 4, screenHeight / 2, 500, 500);
            gameSprites.Add(Player);
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
        }

        float spawn = 0;

        /// <summary>
        /// Contains the game states: the initial paused main menu in which the player can config the breath recogniser reads the game save and resets lives and score if need be
        /// and the playing states which shows the score on screen calls the function to load enemies 
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Update(GameTime gameTime)
        {
            

            switch (CurrentState)
            {
                case GameStates.StartState:
                    updateStartScreen();
                    ResetEnemies();
                    ReadFile(score);
                    if (lives==0)
                    {
                        lives = 3;
                    }
                    else
                    {
                        string fileName = @"C:\Users\harry\Desktop\GAMESAVE.txt";
                        StreamReader sr = new StreamReader(fileName);
                        sr.ReadLine();
                        score = Convert.ToInt32(sr.ReadLine());
                        sr.Close();
                    }
                    break;

                case GameStates.PlayingState:
                    var fizzyoDevice = (FizzyoDevice)this.Services.GetService(typeof(FizzyoDevice));

                    

                    foreach (Enemies enemy in e)
                    {
                        messageString = (" Score: " + score + "  " + "Lives: " + GetLives() + "Breath: " + breathRecogniser.isLastBreathGood); /* + " " + screenWidth + " " + screenHeight*//*+ " PlayerPosition: " + Player.xPosition + " " + Player.yPosition + "Enemy Position: " + enemy.RoundPositionX() + " " + enemy.RoundPositionY()*/
                    }
                    updateGamePlay(gameTime);
                    spawn += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    loadEnemies();
                    break;
            }
            base.Update(gameTime);

        }

        /// <summary>
        /// Initiates the game should the user press space or initiates the config should the user press the button to open the form with the config
        /// </summary>
        public void updateStartScreen()
        {
            KeyboardState keys = Keyboard.GetState();
            if (keys.IsKeyDown(Keys.Space))
            {
                startPlayingGame();
                CurrentState = GameStates.PlayingState;
            }
            if (keys.IsKeyDown(Keys.S))
            {
                BreathConfig f = new BreathConfig();
                f.Show();
            }
            if (keys.IsKeyDown(Keys.R))
            {
                lives = 0;
                score = 0;
                WriteFile();
            }
        }
        /// <summary>
        /// Game over ends the game and saves the current score in the text file SHOULD it be higher than any of the other scores
        /// Then returns to the main menu;
        /// </summary>
        public void GameOver()
        {
            lives = 0;
            LoadfizzyoRecordedData();
            messageString = " Game Over : Press Space to exit   Score: " + score.ToString();
            WriteFile();
            score = 0;
            WriteFile();
            
            CurrentState = GameStates.StartState;
        }

        public void ResetEnemies()
        {
            foreach (Enemies enemy in e)
            {
                enemy.isVisible = false;
            }
        }
        public int getScore()
        {
            return score;
        }
        public int EnemyYAlternator()
        {
            int randY = screenHeight / 2;

            int posY = random.Next(0, 4);

            if (posY == 2)
                return (screenHeight / 4);

            else if (posY == 3)
                return screenHeight - (screenHeight / 4);

            else
                return screenHeight / 2;
            }
        public void loadEnemies()
        {
            if (spawn >= 0.5) // Changes speed at which the enemies spawns (inversly)
            {
                spawn = 0;

                if (e.Count < 5)
                {
                    e.Add(new Enemies(Content.Load<Texture2D>("cracker"), new Vector2(screenWidth, EnemyYAlternator())));
                }
            }

            for (int i = 0; i < e.Count; i++)
            {
                if (!e[i].isVisible)
                {
                    e.RemoveAt(i);
                    i--;
                }
            }
        }

        public void updateGamePlay(GameTime gameTime)
        {

            
            fizzyoDevice.Update(gameTime);
            inputState.Update(gameTime);


            score += 1;
            KeyboardState keys = Keyboard.GetState();
            if (keys.IsKeyDown(Keys.Up))
            {
                Player.StartMovingUp();
            }
            else
            {
                Player.StopMovingUp();
            }

            if (keys.IsKeyDown(Keys.Down))
            {
                Player.StartMovingDown();
            }
            else
            {
                Player.StopMovingDown();
            }
            float startRightMovement = Player.xPosition;
            if (/*keys.IsKeyDown(Keys.G)*/breathRecogniser.isLastBreathGood)
            {
                Player.GoodBreath(startRightMovement);
            }
            if (keys.IsKeyDown(Keys.B))
            {
                Player.BadBreath(startRightMovement);
            }
            if (Player.xPosition < screenWidth / 1000)
            {
                GameOver();
            }

            PlayerTexture();
            EnemyTextureSelector();
            foreach (Enemies enemy in e)
            {
                double dx = (Player.xPosition - enemy.position.X);
                double dy = (Player.yPosition - enemy.position.Y);
                if ((Math.Sqrt((dx * dx) + (dy * dy))) < (enemy.texture.Width))
                {
                    long TimeBetweenLastCollison = stopwatch.ElapsedMilliseconds;
                    if (TimeBetweenLastCollison == 0 || TimeBetweenLastCollison > 3000)
                    {
                        stopwatch.Reset();
                        stopwatch.Start();
                        ReduceOrEnd();
                    }
                }
            }
            foreach (Enemies enemy in e)
            {
            enemy.Update(graphics.GraphicsDevice);
            }

            foreach (Player s in gameSprites)
            {
                s.Update(1.0f / 60.0f);
            }
            ///
            /// FIZZYO IN GAME LOOP
            ///

            InputState inputstate = new InputState(this);

            breathRecogniser.AddSample(gameTime.ElapsedGameTime.Milliseconds, fizzyoDevice.Pressure());

            
        }
        public void ReduceOrEnd()
        {
            if (GetLives() == 1)
            {
                GameOver();
            }
            else
            {
                ReduceLives();
            }
        }

        public void PlayerTexture()
        {
            if (Player.ChangeTexture() == 2)
            {
                Player.texture = AirShip;
            }
            else if (Player.ChangeTexture() == 0)
            {
                Player.texture = Boat;
            }
            else
            {
                Player.texture = SubMarine;
            }
        }

        protected override void Draw(GameTime gameTime)

        {
            drawGamePlay();
            base.Draw(gameTime);
        }

        private void drawGamePlay()
        {
            spriteBatch.Begin();
            Rectangle screenRectangle = new Rectangle(0, 0, screenWidth, screenHeight);
            spriteBatch.Draw(backgroundTexture, screenRectangle, Color.White);
            foreach (Player s in gameSprites)
            {
                s.Draw(spriteBatch);
            }
            float xPos = (screenWidth - messageFont.MeasureString(messageString).X) / 2;

            Vector2 statusPos = new Vector2(xPos, screenHeight/20);

            spriteBatch.DrawString(messageFont, messageString, statusPos, Color.Red);

            foreach (Enemies enemy in e)
            {
                enemy.Draw(spriteBatch);
            }
            spriteBatch.End();
        }
        private void drawStartScreen()
        {
            spriteBatch.Begin();
            Rectangle screenRectangle = new Rectangle(0, 0, screenWidth, screenHeight);
            spriteBatch.Draw(backgroundTexture, screenRectangle, Color.White);
            foreach (Player b in Backgrounds)
            {
                b.Draw(spriteBatch);
            }

            foreach (Player s in gameSprites)
            {
                s.Draw(spriteBatch);
            }
            float xPos = (screenWidth - messageFont.MeasureString(messageString).X) / 2;

            Vector2 statusPos = new Vector2(xPos, screenHeight / 2);

            spriteBatch.DrawString(messageFont, messageString, statusPos, Color.Red);

            spriteBatch.End();
        }
        public void EnemyTextureSelector()
        {
            foreach (Enemies enemy in e)
            {
                if (enemy.RoundPositionY() == screenHeight/2)
                {
                    enemy.texture = EnemyBoat;
                }
                else if (enemy.RoundPositionY() == screenHeight/4)
                {
                    enemy.texture = EnemyPlane;
                }
                else
                {
                    enemy.texture = Shark;
                }
            }
        }
        public void ReadFile(int score)
        {
           

            string fileName = @"C:\Users\harry\Desktop\GAMESAVE.txt";
            int FileLives;
            int ReadScore;
            if (File.Exists(fileName))
            {
                StreamReader sr = new StreamReader(fileName);
                FileLives = Convert.ToInt32(sr.ReadLine());
                ReadScore = Convert.ToInt32(sr.ReadLine());
                for (int i = 0; i <= HighScores.Length-1; i++)
                {
                    HighScores[i] = Convert.ToInt32(sr.ReadLine());
                }
                sr.Close();
                if (LoadLives(FileLives) == true)
                {
                    score = ReadScore;
                }
 
            }
            else
            {
                StreamWriter sw = new StreamWriter(fileName);
                sw.Close();
            }
        }
        public void WriteFile()
        {
            string fileName = @"C:\Users\harry\Desktop\GAMESAVE.txt";
            StreamWriter sw = new StreamWriter(fileName);
            sw.WriteLine(Convert.ToString(lives));
            sw.WriteLine(Convert.ToString(score));
            Array.Sort(HighScores);
            if (HighScores[0] < score)
            {
                HighScores[0] = score;
            }
            Array.Sort(HighScores);
            for (int i = 0; i <= HighScores.Length-1; i++)
            {
                sw.WriteLine(Convert.ToString(HighScores[i]));
            }
            sw.Close();
        }
        public int GetLives()
        {
            return (lives);
        }
        public void ResetLives()
        {
            lives = 3;
        }
        public void ReduceLives()
        {
            lives -= 1;
        }
        public bool LoadLives(int FileLives)
        {
            if (FileLives > 0)
            {
                lives = FileLives;
                return true;
            }
            else
            {
                ResetLives();
                return false;
            }
        }
        protected override void OnExiting(Object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
            string FileName = @"C:\Users\harry\Desktop\GAMESAVE.txt";
            if (File.Exists(FileName))
            {
                WriteFile();
            }
        }
        public void BubbleSortScores(int i)
        {
            if (score > HighScores[i])
            {
                int temp = 0;
                temp = HighScores[i];
                HighScores[i] = score;
                score = temp;
                i += 1;
            }
        }







        



        private void LoadfizzyoRecordedData()
        {
            try
            {
                using (FileStream fs = new FileStream(recordedDataPath, FileMode.Open))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        List<String> inputArray = new List<string>();
                        while (sr.Peek() >= 0)
                        {
                            inputArray.Add(sr.ReadLine());
                        }
                        fizzyoDevice.LoadRecordedData(inputArray.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("could not load file " + recordedDataPath + " " + ex.ToString());
            }
            finally
            {
                Debug.WriteLine("file loaded " + recordedDataPath);
            }
        }
        
    }
}


