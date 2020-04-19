using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace SpaceHawks
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private Texture2D nave;
        private Vector2 posicionNave;
        private float velocNave;

        const int NUM_ENEMIGOS = 11;
        private Texture2D enemigo;
        private Vector2[] posicEnemigo;
        private Vector2 velocEnemigo;
        private bool[] enemigoActivo;

        private Texture2D disparo;
        private Vector2 posicionDisparo;
        private float velocDisparo;
        private bool disparoActivo;

        private Vector2 posicionDisparoEnemigo;
        private float velocDisparoEnemigo;
        private bool disparoActivoEnemigo;
        private int tiempoHastaSiguenteDisparo;

        private SpriteFont fuente;
        private Song musicaDeFondo;
        private SoundEffect sonidoDeDisparo;
        private int puntos;
        private int vidas;
        private int enemigosRestantes;
        private int tiempoMinimoDisparo;
        private int tiempoMaximoDisparo;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 960;
            graphics.PreferredBackBufferHeight = 600;
            graphics.ApplyChanges();

            posicionNave = new Vector2(375, 500);
            velocNave = 340;

            posicEnemigo = new Vector2[NUM_ENEMIGOS];
            enemigoActivo = new bool[NUM_ENEMIGOS];
            //posicionEnemigo1 = new Vector2(100, 150);
            RecolocarEnemigos();
            for (int i = 0; i < NUM_ENEMIGOS; i++)
            {
                enemigoActivo[i] = true;
            }
            velocEnemigo = new Vector2(102, 500);
            velocDisparo = 200;
            disparoActivo = false;
            puntos = 0;
            vidas = 3;

            disparoActivoEnemigo = false;
            velocDisparoEnemigo = 400;
            tiempoHastaSiguenteDisparo = new System.Random().Next(2000, 4000);
            enemigosRestantes = NUM_ENEMIGOS;
            tiempoMinimoDisparo = 500;
            tiempoMaximoDisparo = 2000;
        }

        private void RecolocarEnemigos()
        {
            for (int i = 0; i < NUM_ENEMIGOS; i++)
            {
                int fila = i / 6;
                int columna = i % 6;
                int y = fila * 50 + 100;
                int x = columna * 80 + 50;
                posicEnemigo[i] = new Vector2(x, y);
            }
        }

        protected override void LoadContent()
        {
 
            spriteBatch = new SpriteBatch(GraphicsDevice);

            nave = Content.Load<Texture2D>("nave");

            enemigo = Content.Load<Texture2D>("enemigo1");

            fuente = Content.Load<SpriteFont>("Arial");

            sonidoDeDisparo = Content.Load<SoundEffect>("fire");
            musicaDeFondo = Content.Load<Song>("GameSong");
            disparo = Content.Load<Texture2D>("disparo");
            MediaPlayer.Play(musicaDeFondo);
            MediaPlayer.IsRepeating = true;
        }


        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back
                    == ButtonState.Pressed
                    || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            MoverElementos(gameTime);
            ComprobarColisiones();
            ComprobarEntrada(gameTime);

            base.Update(gameTime);
        }


        protected void MoverElementos(GameTime gameTime)
        {
            bool hayQueDarLaVuelta = false;
            for (int i = 0; i < NUM_ENEMIGOS; i++)
            {
                if (enemigoActivo[i])
                {
                    posicEnemigo[i].X += velocEnemigo.X *
                                    (float)gameTime.
                                    ElapsedGameTime.TotalSeconds;
                    if ((posicEnemigo[i].X > 960 - 20 - enemigo.Width)
                        || (posicEnemigo[i].X < 20))
                    {
                        hayQueDarLaVuelta = true;
                    }
                }
            }
            if (hayQueDarLaVuelta)
            {
                velocEnemigo.X = -velocEnemigo.X;
                for (int i = 0; i < NUM_ENEMIGOS; i++)
                {
                    posicEnemigo[i].Y += velocEnemigo.Y *
                                    (float)gameTime.
                                    ElapsedGameTime.TotalSeconds;
                    if (posicEnemigo[i].Y > 600 - enemigo.Height)
                            posicEnemigo[i].Y = 40;
                }
            }

            if (disparoActivo)
            {
                posicionDisparo.Y -= velocDisparo *
                    (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (posicionDisparo.Y < 0)
                {
                    disparoActivo = false;
                }
            }

            if (disparoActivoEnemigo)
            {
                posicionDisparoEnemigo.Y += velocDisparoEnemigo *
                    (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (posicionDisparoEnemigo.Y > 600)
                {
                    disparoActivoEnemigo = false;
                    tiempoHastaSiguenteDisparo =
                        new System.Random().Next(tiempoMinimoDisparo, tiempoMaximoDisparo);
                }
            }
            else
            {
                tiempoHastaSiguenteDisparo -= gameTime.ElapsedGameTime.Milliseconds;
                if (tiempoHastaSiguenteDisparo <= 0)
                {
                    int numeroEnemigo;
                    do
                    {
                        numeroEnemigo = new System.Random().Next(0, NUM_ENEMIGOS);

                    } while (!enemigoActivo[numeroEnemigo]);
                    float xDE = posicEnemigo[numeroEnemigo].X + 34;
                    float yDE = posicEnemigo[numeroEnemigo].Y + 20;
                    posicionDisparoEnemigo = new Vector2(xDE, yDE);
                    disparoActivoEnemigo = true;
                }
            }
        }

        protected void ComprobarEntrada(GameTime gameTime)
        {
            var estadoTeclado = Keyboard.GetState();
            var estadoGamePad = GamePad.GetState(PlayerIndex.One);
            
            if (estadoTeclado.IsKeyDown(Keys.F11))
            {
                if (!graphics.IsFullScreen)
                {
                    graphics.IsFullScreen = true;
                    graphics.PreferredBackBufferWidth = 1366;
                    graphics.PreferredBackBufferHeight = 768;
                    graphics.ApplyChanges();
                }
                else
                {
                    graphics.IsFullScreen = false;
                    graphics.PreferredBackBufferWidth = 960;
                    graphics.PreferredBackBufferHeight = 600;
                    graphics.ApplyChanges();
                }
                graphics.IsFullScreen = !graphics.IsFullScreen;
                graphics.ApplyChanges();
            }

            if (estadoTeclado.IsKeyDown(Keys.Left)
                || estadoGamePad.DPad.Left > 0
                || estadoGamePad.ThumbSticks.Left.X < 0)
            {
                posicionNave.X -= velocNave * (float)gameTime.
                                   ElapsedGameTime.TotalSeconds;
            }

            if (estadoTeclado.IsKeyDown(Keys.Right)
                || estadoGamePad.DPad.Right > 0
                || estadoGamePad.ThumbSticks.Left.X > 0)
            {
                posicionNave.X += velocNave * (float)gameTime.
                                   ElapsedGameTime.TotalSeconds;
            }

            if (!disparoActivo && 
                (estadoTeclado.IsKeyDown(Keys.Space)
                || estadoGamePad.Buttons.A == ButtonState.Pressed))
            {
                posicionDisparo = new Vector2(
                            posicionNave.X + 25, posicionNave.Y - 20);
                disparoActivo = true;
                sonidoDeDisparo.CreateInstance().Play();
            }
        }

        protected void ComprobarColisiones()
        {
            Rectangle rNave = new Rectangle((int)posicionNave.X,
                                  (int)posicionNave.Y, nave.Width,
                                   nave.Height);
            Rectangle rDisparo = new Rectangle((int)posicionDisparo.X,
                      (int)posicionDisparo.Y, disparo.Width,
                       disparo.Height);
            Rectangle rDisparoEnemigo = new Rectangle((int)posicionDisparoEnemigo.X,
                      (int)posicionDisparoEnemigo.Y, disparo.Width,
                       disparo.Height);
            if (disparoActivoEnemigo && 
                rDisparoEnemigo.Intersects(rNave))
            {
                disparoActivoEnemigo = false;
                vidas--;
                if (vidas <= 0)
                {
                    Exit();
                }
            }
            for (int i = 0; i < NUM_ENEMIGOS; i++)
            {
                if (enemigoActivo[i])
                {
                    Rectangle rEnemigo = new Rectangle((int)posicEnemigo[i].X,
                                        (int)posicEnemigo[i].Y, enemigo.Width,
                                        enemigo.Height);
                    if (disparoActivo && rDisparo.Intersects(rEnemigo))
                    {
                        enemigoActivo[i] = false;
                        enemigosRestantes--;
                        disparoActivo = false;
                        puntos += 10;
                        if (enemigosRestantes == 0)
                        {
                            AvanzarNivel();
                        }
                    }
                    if (rNave.Intersects(rEnemigo))
                    {
                        vidas--;
                        RecolocarEnemigos();
                        if (vidas <= 0)
                        {
                            Exit();
                        }
                    }
                }
            }
        }

        protected void AvanzarNivel()
        {
            RecolocarEnemigos();
            for (int i = 0; i < NUM_ENEMIGOS; i++)
                enemigoActivo[i] = true;
            tiempoMinimoDisparo = (int)(tiempoMinimoDisparo * 0.9);
            tiempoMaximoDisparo = (int)(tiempoMaximoDisparo * 0.9);
            velocDisparoEnemigo = (int)(velocDisparoEnemigo * 1.1);
            velocEnemigo.X = (int)(velocEnemigo.X * 1.1);
            velocEnemigo.Y = (int)(velocEnemigo.Y * 1.1);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            spriteBatch.DrawString(fuente, "Marcador: " + puntos, 
                                    new Vector2(20, 20), 
                                    Color.Brown);
            spriteBatch.DrawString(fuente, "Vidas: " + vidas,
                                    new Vector2(20, 60),
                                    new Color(232, 228, 19));
            spriteBatch.Draw(nave, new Rectangle((int)posicionNave.X,
                                (int)posicionNave.Y, nave.Width, 
                                nave.Height), Color.White);
            
            for (int i = 0; i < NUM_ENEMIGOS; i++)
            {
                if (enemigoActivo[i])
                {
                    spriteBatch.Draw(enemigo, new Rectangle(
                                (int)posicEnemigo[i].X,
                                (int)posicEnemigo[i].Y, enemigo.Width,
                                enemigo.Height), Color.White);
                }
            }

            if (disparoActivo)
            {
                spriteBatch.Draw(disparo, new Rectangle((int)posicionDisparo.X,
                                    (int)posicionDisparo.Y, disparo.Width,
                                    disparo.Height), new Color(240, 12, 39));
            }

            if (disparoActivoEnemigo)
            {
                spriteBatch.Draw(disparo, new Rectangle((int)posicionDisparoEnemigo.X,
                                    (int)posicionDisparoEnemigo.Y, disparo.Width,
                                    disparo.Height), new Color(69, 222, 22));
            }


            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
