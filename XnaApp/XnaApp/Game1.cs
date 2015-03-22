using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using XnaApp.Objetos;

namespace XnaApp
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SoundEffect soundEngine;
        SoundEffectInstance soundEngineInstance;
        SoundEffect subMachine;
        SoundEffectInstance subMachineInstance;

        SoundEffect heli;
        SoundEffectInstance heliInstance;
        
        // Imagens utilizadas no jogo
        Texture2D imgNuvem;
        Texture2D imgArcoF;
        Texture2D imgArcoT;
        Texture2D imgNave;
        Texture2D imgMira;
        Texture2D imgTiro;
        Texture2D imgEnemy;
        Texture2D imgExplosao;
        Texture2D imgBoss;
        
        Vector2 origin;
        

        // Objetos utilizados no jogo
        List<Nuvem> nuvens;
        List<Arco> arcosF;
        List<Arco> arcosT;
        List<Explosao> explosoes;
        List<Nave> bossEnemies;
        List<Nave> enemies;
        Nave nave;

        // Limites da nave no eixo vertical
        int YMin = 10;
        int YMax = 710;

        bool desenha = false;
        // Controle do tempo
        float tempo = 0f;
        float tempo2 = 0f;
        float tempo3 = 0f;
        float tempo4 = 0f;
        float tempoDesenha = 300f;
        float tempoJogo = 30f;
        float tempoBoss = 0f;
        float tempoExplosao = 0f;
        float tempoSpawnEnemies = 0f;
        float intervalo = 40f;
        float intervaloTiro = 200f;
        float intervaloSpawnEnemies = 1000f;
        float intervaloExplosao = 40f;
        float intervaloBoss = 20000f;
        float intervaloDesenha = 300f;

        int enemyCount = 10;
        public static readonly Random r = new Random();

        // Controle de vidas e placar
        int vidas = 10;
        int placar = 0;
        
        //posicao e vel mira
        Vector2 posicaoMira = Vector2.Zero;
        const float miraSpeed = 6;
        MouseState prevMouseState; 

        // Fonte
        SpriteFont fonte;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            this.graphics.PreferredBackBufferWidth = 1280;
            this.graphics.PreferredBackBufferHeight = 720;
            this.graphics.IsFullScreen = true;
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
            // TODO: Add your initialization logic here
            // Initialize invoca o método LoadContent para carregar
            // recursos utilizados no jogo
            base.Initialize();

            // Instancia a lista de nuvens
            IniciarNuvens();

            // Instancia a lista de arcos
            IniciarArcos();

            // Instancia nave
            IniciarNave();


            IniciarEnemies();

            IniciaExplosoes();

            IniciaBossEnemies();
        }
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            // Carrega as imagens utilizadas no jogo
            imgNuvem = Content.Load<Texture2D>("Imagens\\Nuvem");
            imgArcoF = Content.Load<Texture2D>("Imagens\\ArcoF");
            imgArcoT = Content.Load<Texture2D>("Imagens\\ArcoT");
            imgNave = Content.Load<Texture2D>("Imagens\\Nave");
            imgMira = Content.Load<Texture2D>("Imagens\\aim");
            imgTiro = Content.Load<Texture2D>("Imagens\\bullet3");
            imgEnemy = Content.Load<Texture2D>("Imagens\\Nave2");
            imgExplosao = Content.Load<Texture2D>("Imagens\\explosao");
            imgBoss = Content.Load<Texture2D>("Imagens\\boss");
            origin.X = imgTiro.Width / 2;
            origin.Y = imgTiro.Height / 2;
            // Carrega fonte
            fonte = Content.Load<SpriteFont>("SpriteFont1");

            soundEngine = Content.Load<SoundEffect>("NewFolder1\\explo");
            soundEngineInstance = soundEngine.CreateInstance();

            subMachine = Content.Load<SoundEffect>("NewFolder1\\submachine_gun");
            subMachineInstance = subMachine.CreateInstance();

            heli = Content.Load<SoundEffect>("NewFolder1\\heli");
            heliInstance = heli.CreateInstance();
            heliInstance.Volume = 0.3f;
            heliInstance.IsLooped = true;
            heliInstance.Play();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            // Libera as imagens utilizadas no jogo
            imgNuvem.Dispose();
            imgArcoT.Dispose();
            imgArcoF.Dispose();
            imgNave.Dispose();
            imgMira.Dispose();
            imgTiro.Dispose();
            imgEnemy.Dispose();
            imgBoss.Dispose();
            imgExplosao.Dispose();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            
            //atualiza posicao da mira em relacao ao mouse
            MouseState mouseState = Mouse.GetState(); 
            if (mouseState.X != prevMouseState.X || mouseState.Y != prevMouseState.Y) 
                posicaoMira = new Vector2(mouseState.X, mouseState.Y); 
            prevMouseState = mouseState;
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                subMachineInstance.Play();
                if (tempo2 > intervaloTiro)
                {
                    tempo2 = 0f;
                    AddTiro(posicaoMira, new Vector2(nave.Posicao.X + 60, nave.Posicao.Y));
                }
            }
           


            //Atualiza tempos
            tempoSpawnEnemies += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            tempo2 += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            tempoExplosao += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            tempoBoss += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            tempoDesenha+= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            tempoJogo -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // Atualiza frame da nave
            AtualizarQuadroNave((float)gameTime.ElapsedGameTime.TotalMilliseconds);
            AtualizarQuadroNaveInimigo((float)gameTime.ElapsedGameTime.TotalMilliseconds);
            AtualizarQuadroNaveBoss((float)gameTime.ElapsedGameTime.TotalMilliseconds);
            //atualiza naves inimigas
            AtualizarEnemies();

            // Verifica o teclado (aceleração da nave)
            VerificarTeclado();

            // Atualiza a posição das nuvens
            AtualizarNuvens();

            // Atualiza a posição dos arcos
            AtualizarArcos();

            // Atualiza a posição da nave
            AtualizarNave();

            // Atualiza os tiros
            AtualizarTiros();

            AtualizaExplosao();

            AtualizarBoss();

            // Controle de colisão
            if (VerificarColisao()) vidas--;
            else
                // Controle de placar
                if (AtualizarPlacar()) 
                {
                    placar++;
                    tempoJogo += 3f;
                    desenha = true;
                }

            if (desenha == true) 
            {
                tempoDesenha = 0;
                desenha = false;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            if (vidas > 0 && tempoJogo > 0.0)
            {
                
                // Desenha nuvens
                foreach (Nuvem n in nuvens)
                    spriteBatch.Draw(n.Imagem, n.RecDst, n.RecSrc, Color.White);

                // Desenha arco de trás
                foreach (Arco c in arcosT)
                    if (c.Visivel) spriteBatch.Draw(c.Imagem, c.Posicao, Color.White);

                // Desenha nave
                spriteBatch.Draw(nave.Imagem, nave.RecDst,
                    nave.RecSrc, Color.White);

                // Desenha arco da frente
                foreach (Arco c in arcosF)
                    if (c.Visivel) spriteBatch.Draw(c.Imagem, c.Posicao, Color.White);

                // Desenha nave inimiga
                foreach (Nave n in enemies)
                    spriteBatch.Draw(n.Imagem, n.RecDst, n.RecSrc, Color.White);

                // Desenha nave boss
                foreach (Nave b in bossEnemies)
                    spriteBatch.Draw(b.Imagem, b.RecDst, b.RecSrc, Color.White);

                foreach (Explosao e in explosoes)
                    spriteBatch.Draw(e.Imagem, e.RecDst, e.RecSrc, Color.White);

                //desenha tiros
                foreach (Tiro t in nave.tiros)
                    spriteBatch.Draw(t.Imagem, t.Posicao, Color.White);

                // Mostra número de vidas
                if (vidas > 1)
                {
                    spriteBatch.DrawString(fonte, "Vidas: " + vidas.ToString(),
                        new Vector2(20, 20), Color.Black);
                }
                else
                {
                    spriteBatch.DrawString(fonte, "Vidas: " + vidas.ToString(),
                        new Vector2(20, 20), Color.Red);
                }



                if (tempoJogo > 10.0)
                {
                    spriteBatch.DrawString(fonte, "Tempo: " + String.Format("{0:0}", tempoJogo), new Vector2(20, 60), Color.Black);
                }
                else
                {
                    spriteBatch.DrawString(fonte, "Tempo: " + String.Format("{0:0}", tempoJogo), new Vector2(20, 60), Color.Red);
                }

                if (tempoDesenha < intervaloDesenha)
                {
                    spriteBatch.DrawString(fonte, "+3s ", new Vector2(150, 60), Color.DarkBlue);
                }
                    // Desenha mira
                spriteBatch.Draw(imgMira, posicaoMira, Color.White); 
            }
            else
            {
                // Mostra fim do jogo no centro da tela
                string fim = "Fim do Jogo";
                float posx = graphics.GraphicsDevice.Viewport.Width / 2 -
                    fonte.MeasureString(fim).X / 2;
                float posy = graphics.GraphicsDevice.Viewport.Height / 2 -
                    fonte.MeasureString(fim).Y / 2;
                Vector2 p = new Vector2(posx, posy);
                spriteBatch.DrawString(fonte, fim, p, Color.Black);
                heliInstance.Stop();
            }

            // Mostra placar
            spriteBatch.DrawString(fonte, "Placar: " + placar.ToString(),
                new Vector2(20, 40), Color.Black);

            spriteBatch.End();

            base.Draw(gameTime);
        }

   

        private void IniciarNuvens()
        {
            // Instancia a lista de nuvens
            nuvens = new List<Nuvem>();
        }

        private void IniciaBossEnemies()
        {
            bossEnemies = new List<Nave>();
        }

        private void IniciaExplosoes()
        {
            explosoes = new List<Explosao>();
        }

        private void IniciarArcos()
        {
            // Instancia as listas de arcos
            arcosF = new List<Arco>();
            arcosT = new List<Arco>();
        }

        private void IniciarNave()
        {
            // Instancia a nave
            nave = new Nave(
                new Vector2(10, YMax - imgNave.Height),
                new Vector2(0, 0));

            // Dados da imagem da nave
            nave.Imagem = imgNave;
            nave.ImagemQtd = 4;
            nave.ImagemNum = 0;
            
            //tiros = new List<Tiro>();
            nave.tiros = new Queue<Tiro>();

            // Velocidade máxima
            nave.VelocidadeMax = -5f;

            // Posição da nave na tela
            nave.RecDst = new Rectangle(
                (int)nave.Posicao.X, (int)nave.Posicao.Y,
                nave.Imagem.Width / nave.ImagemQtd, nave.Imagem.Height);

            // Área de colisão
            nave.AreaColisao.Add(new Rectangle(
                nave.RecDst.X + 30, nave.RecDst.Y,
                nave.RecDst.Width - 70, nave.RecDst.Height - 50));
        }

        private void IniciarEnemies() 
        {
            enemies = new List<Nave>();
        }

        private void AddEnemy()
        {
             Nave enemy = new Nave(new Vector2(graphics.GraphicsDevice.Viewport.Width, 
                 r.Next(YMin, YMax - 100)), new Vector2(0, 0));

            enemy.Imagem = imgEnemy;
            enemy.ImagemQtd = 4;
            enemy.ImagemNum = 0;
            enemy.Life = 3;
            enemy.Aceleracao = r.Next(2, 4);

            enemy.VelocidadeMax = -5f;

            enemy.RecDst = new Rectangle(
                (int)enemy.Posicao.X, (int)enemy.Posicao.Y,
                enemy.Imagem.Width / enemy.ImagemQtd, enemy.Imagem.Height);

            enemy.AreaColisao.Add(new Rectangle(
                enemy.RecDst.X + 25, enemy.RecDst.Y + 10,
                enemy.RecDst.Width - 40, enemy.RecDst.Height - 40));
            enemies.Add(enemy);
        }

        private void AddBossEnemy()
        {
            Nave boss = new Nave(new Vector2(graphics.GraphicsDevice.Viewport.Width,
                r.Next(YMin, YMax - 200)), new Vector2(0, 0));

            boss.Imagem = imgBoss;
            boss.ImagemQtd = 3;
            boss.ImagemNum = 0;
            boss.Life = 20;
            boss.Aceleracao = 2f;

            boss.VelocidadeMax = -5f;

            boss.RecDst = new Rectangle(
                (int)boss.Posicao.X, (int)boss.Posicao.Y,
                boss.Imagem.Width / boss.ImagemQtd, boss.Imagem.Height);

            boss.AreaColisao.Add(new Rectangle(
                boss.RecDst.X + 25, boss.RecDst.Y + 10,
                boss.RecDst.Width - 40, boss.RecDst.Height - 40));
            bossEnemies.Add(boss);
        }

        private void AddTiro(Vector2 mousePosition, Vector2 posicaoNave)
        {
            posicaoNave.Y += 20;
            Tiro tiro = new Tiro(posicaoNave);
            tiro.Imagem = imgTiro;
            mousePosition.X = mousePosition.X + imgMira.Width / 2;
            mousePosition.Y = (mousePosition.Y + imgMira.Height / 2) - 5;
            Vector2 movement = mousePosition - posicaoNave;
            if (movement != Vector2.Zero)
                movement.Normalize();
            tiro.direcao = movement;
            tiro.rotation = (float)Math.Atan2(movement.Y, movement.X);

            // Posição do tiro na tela
            tiro.RecDst = new Rectangle(
                (int)tiro.Posicao.X, (int)tiro.Posicao.Y,
                tiro.Imagem.Width, tiro.Imagem.Height);

            // Área de colisão
            tiro.AreaColisao.Add(new Rectangle(
                tiro.RecDst.X, tiro.RecDst.Y,
                tiro.RecDst.Width, tiro.RecDst.Height));

            nave.tiros.Enqueue(tiro);
        }

        private void AtualizarTiros()
        {
            foreach (Tiro t in nave.tiros.ToList())
            {
                t.Posicao += t.direcao * t.speed;

                // Posição do tiro na tela
                t.RecDst = new Rectangle(
                    (int)t.Posicao.X, (int)t.Posicao.Y,
                    t.Imagem.Width, t.Imagem.Height);

                // Área de colisão
                t.AreaColisao[0] = new Rectangle(
                    t.RecDst.X, t.RecDst.Y,
                    t.RecDst.Width, t.RecDst.Height);

                foreach (Nave e in enemies.ToList())
                {
                    if (t.VerificarColisao(e.AreaColisao[0])) 
                    {
                        nave.tiros.Dequeue();
                        e.Life--;
                    }
                }

                foreach (Nave b in bossEnemies.ToList())
                {
                    if (t.VerificarColisao(b.AreaColisao[0]))
                    {
                        nave.tiros.Dequeue();
                        b.Life--;
                    }
                }

                if (Vector2.Distance(t.Posicao, nave.Posicao) > 1000f) 
                {
                    nave.tiros.Dequeue();
                }
            }
            
               
        }


        private void AtualizarNuvens()
        {
            Random rand = new Random();
            int w = graphics.PreferredBackBufferWidth;
            int h = graphics.PreferredBackBufferHeight;

            // Insere até 12 nuvens na lista de nuvens
            if (nuvens == null || nuvens.Count < 12)
            {
                Nuvem nuvem = new Nuvem(
                    // Posição da nuvem é randômica e de acordo com tamanho da tela
                    new Vector2(
                        rand.Next(w),
                        rand.Next(h - imgNuvem.Height)),
                    // Velocidade da nuvem
                    new Vector2((float)-1f * rand.Next(25, 40) / 100, 0));

                // Seleção da nuvem no arquivo é aleatória
                nuvem.RecSrc = new Rectangle(rand.Next(3) * imgNuvem.Width / 3, 0,
                    imgNuvem.Width / 3, imgNuvem.Height);

                // Imagem da nuvem
                nuvem.Imagem = imgNuvem;

                // Adiciona nuvem na lista
                nuvens.Add(nuvem);
            }
            else
            {
                foreach (Nuvem nuvem in nuvens)
                {
                    // Verifica a posição da nuvem para checar se ainda é visível
                    if (nuvem.Posicao.X <= 0 - imgNuvem.Width / 3)
                    {
                        // Nuvem não é visível, insere em posição randômica
                        nuvem.Posicao = new Vector2(w + rand.Next(w),
                            rand.Next(h - imgNuvem.Height));
                    }
                    else
                    {
                        // Nuvem é visível, altera a posição de acordo com a velocidade
                        nuvem.Posicao = new Vector2(
                            nuvem.Posicao.X + nuvem.Velocidade.X, nuvem.Posicao.Y);
                    }

                    // Posição de desenho da nuvem na tela
                    nuvem.RecDst = new Rectangle(
                        (int)nuvem.Posicao.X, (int)nuvem.Posicao.Y,
                        imgNuvem.Width / 3, imgNuvem.Height);
                }
            }
        }

        private void AtualizarArcos()
        {
            Random rand = new Random();
            int w = graphics.PreferredBackBufferWidth;
            int h = graphics.PreferredBackBufferHeight;

            // Verifica se a lista de arcos está completa
            if (arcosT.Count < 3)
            {
                // Insere novos arcos na lista
                Arco arcoF;
                Arco arcoT;

                // Verifica se é o primeiro arco
                if (arcosT.Count == 0)
                {
                    // Arco de trás - inicia no final da tela
                    arcoT = new Arco(
                        new Vector2(
                            rand.Next(w),
                            rand.Next(h - imgArcoT.Height)),
                        new Vector2(-3f, 0));
                }
                else
                {
                    // Arco de trás - deslocado meia tela em relação ao último arco
                    arcoT = new Arco(
                        new Vector2(
                            arcosT[arcosT.Count - 1].Posicao.X + w / 2,
                            rand.Next(h - imgArcoT.Height)),
                        arcosT[arcosT.Count - 1].Velocidade);
                }

                // Arco da frente
                arcoF = new Arco(arcoT.Posicao, arcoT.Velocidade);

                // Área de colisão superior
                arcoF.AreaColisao.Add(new Rectangle(
                    (int)arcoF.Posicao.X + imgArcoF.Width / 3,
                    (int)arcoF.Posicao.Y,
                    imgArcoF.Width / 3,
                    imgArcoF.Height / 10));

                // Área de colisão inferior
                arcoF.AreaColisao.Add(new Rectangle(
                    (int)arcoF.Posicao.X + imgArcoF.Width / 3,
                    (int)arcoF.Posicao.Y + (int)(9 * imgArcoF.Height / 10),
                    imgArcoF.Width / 3,
                    imgArcoF.Height / 10));

                // Visibilidade do arco
                arcoF.Visivel = true;
                arcoT.Visivel = true;

                // Contagem de pontos
                arcoF.Placar = false;

                // Imagens dos arcos
                arcoF.Imagem = imgArcoF;
                arcoT.Imagem = imgArcoT;

                // Adiciona arcos nas listas
                arcosF.Add(arcoF);
                arcosT.Add(arcoT);
            }
            else
            {
                // Atualiza posições dos arcos na lista

                // Posição do último arco
                Vector2 pos = arcosT[arcosT.Count - 1].Posicao;

                // Percorre lista de arcos
                for (int i = 0; i < arcosT.Count; i++)
                {
                    // Verifica se arco deixou de ser visível
                    if (arcosT[i].Posicao.X <= 0 - imgArcoT.Width)
                    {
                        // Arco não está visível - Desloca meia tela a partir do último
                        // Atualiza posição do arco
                        arcosT[i].Posicao = new Vector2(
                            pos.X + w / 2,
                            rand.Next(h - imgArcoT.Height));
                        arcosF[i].Posicao = arcosT[i].Posicao;
                        // Visibilidade do arco
                        arcosF[i].Visivel = true;
                        arcosT[i].Visivel = true;
                        // Contagem de pontos
                        arcosF[i].Placar = false;
                    }
                    else
                    {
                        // Arco visível - altera a posição de acordo com a velocidade x
                        arcosT[i].Posicao = new Vector2(arcosT[i].Posicao.X + arcosT[i].Velocidade.X,
                            arcosT[i].Posicao.Y);
                        arcosF[i].Posicao = arcosT[i].Posicao;
                    }
                    // Redefine as áreas de colisão
                    arcosF[i].AreaColisao[0] = new Rectangle(
                        (int)arcosF[i].Posicao.X + imgArcoF.Width / 3,
                        (int)arcosF[i].Posicao.Y,
                        imgArcoF.Width / 3,
                        imgArcoF.Height / 10);
                    arcosF[i].AreaColisao[1] = new Rectangle(
                        (int)arcosF[i].Posicao.X + imgArcoF.Width / 3,
                        (int)arcosF[i].Posicao.Y + (int)(9 * imgArcoF.Height / 10),
                        imgArcoF.Width / 3,
                        imgArcoF.Height / 10);
                    // Atualiza posição do último arco
                    pos = arcosT[i].Posicao;
                }
            }
        }
        
        private void AtualizarQuadroNave(float elapsedTime)
        {
            // Atualiza controle de tempo
            tempo += elapsedTime;
            // Verifica se intervalo de atualização foi atingido
            if (tempo > intervalo)
            {
                // Incrementa frame exibido
                nave.ImagemNum++;
                if (nave.ImagemNum > nave.ImagemQtd - 1) nave.ImagemNum = 0;
                // Resseta controle de tempo
                tempo = 0;
            }
            // Define nova posição da imagem
            nave.RecSrc = new Rectangle(
                nave.ImagemNum * imgNave.Width / nave.ImagemQtd, 0,
                imgNave.Width / nave.ImagemQtd, imgNave.Height);
        }

        private void AtualizarQuadroNaveInimigo(float elapsedTime)
        {
            // Atualiza controle de tempo
            tempo3 += elapsedTime;
            // Verifica se intervalo de atualização foi atingido
            if (tempo3 > intervalo)
            {
                foreach (Nave e in enemies.ToList()) { 
                // Incrementa frame exibido
                e.ImagemNum++;
                if (e.ImagemNum > e.ImagemQtd - 1) e.ImagemNum = 0;
                // Resseta controle de tempo
                tempo3 = 0;
                    }
            }
        }


        private void AtualizarQuadroNaveBoss(float elapsedTime)
        {
            // Atualiza controle de tempo
            tempo4 += elapsedTime;
            // Verifica se intervalo de atualização foi atingido
            if (tempo4 > intervalo)
            {
                foreach (Nave b in bossEnemies.ToList())
                {
                    // Incrementa frame exibido
                    b.ImagemNum++;
                    if (b.ImagemNum > b.ImagemQtd - 1) b.ImagemNum = 0;
                    // Resseta controle de tempo
                    tempo4 = 0;
                }
            }
        }


        private void AtualizarEnemies()
        {
            //
            foreach (Nave e in enemies.ToList())
            {

                if (e.Posicao.X > -120)
                {
                    Vector2 nPos1 = new Vector2(e.Posicao.X, e.Posicao.Y);
                    Vector2 nVel1 = new Vector2(e.Velocidade.X, e.Velocidade.Y);

                    nPos1.X -= e.Aceleracao;

                    // Novas posições e velocidade
                    e.Posicao = nPos1;

                    // Posição da nave na tela
                    e.RecDst = new Rectangle(
                        (int)e.Posicao.X, (int)e.Posicao.Y,
                        e.Imagem.Width / e.ImagemQtd, e.Imagem.Height);

                }

                // Define nova posição da imagem
                e.RecSrc = new Rectangle(
                    e.ImagemNum * imgEnemy.Width / e.ImagemQtd, 0,
                    imgEnemy.Width / e.ImagemQtd, imgEnemy.Height);

                e.AreaColisao[0] = new Rectangle(
                e.RecDst.X + 25, e.RecDst.Y + 10,
                e.RecDst.Width - 40, e.RecDst.Height - 40);

                if (e.Posicao.X < -119)
                    enemies.Remove(e);

                if (e.Life <= 0)
                {
                    AddExplosao(e.Posicao);
                    enemies.Remove(e);
                }

                if (e.VerificarColisao(nave.AreaColisao[0]))
                {
                    AddExplosao(e.Posicao);
                    enemies.Remove(e);
                    vidas--;    
                }
                


            }

            if (enemies.ToList().Count < enemyCount && tempoSpawnEnemies > intervaloSpawnEnemies)
            {
                AddEnemy();
                tempoSpawnEnemies = 0;
            }

        }
        //

        private void AtualizarBoss()
        {
            //
            foreach (Nave b in bossEnemies.ToList())
            {

                if (b.Posicao.X > -120)
                {
                    Vector2 nPos1 = new Vector2(b.Posicao.X, b.Posicao.Y);
                    Vector2 nVel1 = new Vector2(b.Velocidade.X, b.Velocidade.Y);

                    nPos1.X -= b.Aceleracao;

                    // Novas posições e velocidade
                    b.Posicao = nPos1;


                    // Posição da nave na tela
                    b.RecDst = new Rectangle(
                        (int)b.Posicao.X, (int)b.Posicao.Y,
                        b.Imagem.Width / b.ImagemQtd, b.Imagem.Height);

                }

                // Define nova posição da imagem
                b.RecSrc = new Rectangle(
                    b.ImagemNum * imgBoss.Width / b.ImagemQtd, 0,
                    imgBoss.Width / b.ImagemQtd, imgBoss.Height);

                b.AreaColisao[0] = new Rectangle(
                b.RecDst.X + 25, b.RecDst.Y + 10,
                b.RecDst.Width - 40, b.RecDst.Height - 40);

                if (b.Posicao.X < -119)
                    bossEnemies.Remove(b);

                if (b.Life <= 0)
                {
                    AddExplosao(b.Posicao);
                    bossEnemies.Remove(b);
                }

                if (b.VerificarColisao(nave.AreaColisao[0]))
                {
                    AddExplosao(b.Posicao);
                    vidas--;
                }



            }

            if (bossEnemies.ToList().Count < 1 && tempoBoss > intervaloBoss)
            {
                AddBossEnemy();
                tempoBoss = 0;
            }

        }

        private void AddExplosao(Vector2 posicao)
        {
            Explosao explosao = new Explosao(posicao);
            explosao.Imagem = imgExplosao;
            explosao.ImagemQtd = 9;
            explosao.ImagemNum = 0;

            // Posição da explosao na tela
            explosao.RecDst = new Rectangle(
                (int)explosao.Posicao.X, (int)explosao.Posicao.Y,
                explosao.Imagem.Width / explosao.ImagemQtd, explosao.Imagem.Height);
            soundEngineInstance.Play();
            explosoes.Add(explosao);
        }

        private void AtualizaExplosao()
        {
            if (tempoExplosao > intervaloExplosao)
            {
                foreach (Explosao e in explosoes.ToList())
                {
                    // Incrementa frame exibido
                    e.ImagemNum++;
                    if (e.ImagemNum > e.ImagemQtd - 1) explosoes.Remove(e);
                    // Resseta controle de tempo
                    tempoExplosao = 0;

                    // Define nova posição da imagem
                    e.RecSrc = new Rectangle(
                    e.ImagemNum * imgExplosao.Width / e.ImagemQtd, 0,
                    imgExplosao.Width / e.ImagemQtd, imgExplosao.Height);
                }
            }
            
                    
                    
            
        }
             
        


        private void AtualizarNave()
        {
            // Posição e velocidade atuais
            Vector2 nPos = new Vector2(nave.Posicao.X, nave.Posicao.Y);
            Vector2 nVel = new Vector2(nave.Velocidade.X, nave.Velocidade.Y);

            // Verifica aceleração e posição vertical
            if (nave.Posicao.Y > YMin && nave.Acelerando)
            {
                // Aceleração da nave
                nave.Aceleracao = -0.3f;
                // Nova posição
                nPos.Y = nave.Posicao.Y + nave.Velocidade.Y + nave.Aceleracao;
                // Nova velocidade
                nVel.Y = nave.Velocidade.Y + nave.Aceleracao;
                if (nVel.Y < nave.VelocidadeMax) nVel.Y = nave.VelocidadeMax;
            }
            else
            {
                if ((nave.Posicao.Y < YMax - nave.Imagem.Height) && !nave.Acelerando)
                {
                    // Nave não está acelarando
                    nave.Aceleracao = 0;
                    // Nova posição
                    nPos.Y = nave.Posicao.Y + nave.Velocidade.Y + 0.1f;
                    // Nova velocidade
                    nVel.Y = nave.Velocidade.Y + 0.1f;
                    if (nVel.Y > -nave.VelocidadeMax) nVel.Y = -nave.VelocidadeMax;
                }
            }

            // Verifica se limite inferior da tela foi atingido
            if (nave.Posicao.Y > YMax - nave.Imagem.Height)
            {
                // Posição e velocidade na base da tela
                nPos.Y = YMax - nave.Imagem.Height;
                nVel.Y = 0;
            }

            // Verifica se o limite superior da tela foi atingido
            if (nave.Posicao.Y < YMin)
            {
                // Posição e velocidade no topo da tela
                nPos.Y = YMin;
                nVel.Y = 0;
            }

            // Novas posições e velocidade
            nave.Posicao = nPos;
            nave.Velocidade = nVel;

            // Posição da nave na tela
            nave.RecDst = new Rectangle(
                (int)nave.Posicao.X, (int)nave.Posicao.Y,
                nave.Imagem.Width / nave.ImagemQtd, nave.Imagem.Height);

            // Área de colisão
            nave.AreaColisao[0] = new Rectangle(
                nave.RecDst.X + 10, nave.RecDst.Y,
                nave.RecDst.Width - 50, nave.RecDst.Height - 50);
        }

        private bool AtualizarPlacar()
        {
            bool result = false;
            // Verifica se os arco já foram criados
            if (arcosF.Count > 0)
            {
                foreach (Arco arco in arcosF)
                    // Verifica se a nave passa dentro do arco
                    if (arco.AreaColisao.Count > 1 && arco.Visivel && !arco.Placar)
                        if (nave.AreaColisao[0].Top > arco.AreaColisao[0].Bottom &&
                            nave.AreaColisao[0].Bottom < arco.AreaColisao[1].Top &&
                            nave.AreaColisao[0].Left > arco.AreaColisao[0].Left + 10 &&
                            nave.AreaColisao[0].Right > arco.AreaColisao[0].Right)
                        {
                            // Arco com pontuação
                            arco.Placar = true;
                            result = true;
                        }
            }
            return result;
        }



        private bool VerificarColisao()
        {
            bool result = false;
            // Verifica a colisão de cada arco com a nave
            for (int i = 0; i < arcosF.Count; i++)
                if (arcosF[i].Visivel && arcosF[i].VerificarColisao(nave.AreaColisao[0]))
                {
                    // Houve colisão, esconde o arco
                    arcosF[i].Visivel = false;
                    arcosT[i].Visivel = false;
                    result = true;
                }
            return result;
        }

        private void VerificarTeclado()
        {
            // Verificação do teclado
            KeyboardState ks = Keyboard.GetState();
            // Teclas pressionadas
            Keys[] keys = ks.GetPressedKeys();
            // Verifica tecla para cima
            nave.Acelerando = ks.IsKeyDown(Keys.W);
            // Verifica fim do jogo
            if (ks.IsKeyDown(Keys.Escape)) Exit();
        }
    }
}
