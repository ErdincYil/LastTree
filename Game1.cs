using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media; 
using System.Collections.Generic;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Texture2D _pixel, _background;
    private Texture2D _enemySlimeTex, _blackSoldierTex;
    private Texture2D _p1IdleTex, _p1RunTex, _p1AttackTex, _p1HurtTex;
    private Texture2D _p2IdleTex, _p2RunTex, _p2AttackTex, _p2HurtTex, _p2DeathTex;
    private Texture2D _e2IdleTex, _e2RunTex, _e2AttackTex, _e2HurtTex, _e2DeathTex;
    private Texture2D _e3IdleTex, _e3WalkTex, _e3AttackTex, _e3HurtTex, _e3DieTex;
    private Texture2D _btnBaslatTex, _btnRestartTex, _btnExitTex;

    private Song _backgroundMusic;

    private int _gameState = 0; 

    private Rectangle _btnPlay, _btnExit;
    private Rectangle _playerBase, _enemyBase;
    private Rectangle _button1, _button2, _button3;

    private int _pBaseHP, _pBaseMaxHP = 500;
    private int _eBaseHP, _eBaseMaxHP = 500;

    private List<Unit> _armies = new List<Unit>(); 
    private List<Unit> _enemies = new List<Unit>(); 

    private int _myDna = 100, _aiDna = 100;
    private const int MAX_DNA = 250;

    private int _aiTargetUnitType = 0;
    private int _enemySpawnTimer = 0;
    private int _passiveDnaTimer = 0;

    private MouseState _prevMouse;
    private System.Random _rand = new System.Random();

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _btnPlay  = new Rectangle(540, 250, 200, 80);
        _btnExit  = new Rectangle(540, 380, 200, 80);

        _playerBase = new Rectangle(0,    120, 100, 500); 
        _enemyBase  = new Rectangle(1180, 120, 100, 500); 

        _button1 = new Rectangle(200, 620, 80, 80);
        _button2 = new Rectangle(300, 620, 80, 80);
        _button3 = new Rectangle(400, 620, 80, 80);

        _pBaseHP = _pBaseMaxHP;
        _eBaseHP = _eBaseMaxHP;

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });

        _background      = Content.Load<Texture2D>("0");
        _enemySlimeTex   = Content.Load<Texture2D>("eslime");
        _blackSoldierTex = Content.Load<Texture2D>("soldier");

        _p1IdleTex   = Content.Load<Texture2D>("sidle");
        _p1RunTex    = Content.Load<Texture2D>("srun");
        _p1AttackTex = Content.Load<Texture2D>("sattack");
        _p1HurtTex   = Content.Load<Texture2D>("shurt");

        _p2IdleTex   = Content.Load<Texture2D>("widle");
        _p2RunTex    = Content.Load<Texture2D>("wrun");
        _p2AttackTex = Content.Load<Texture2D>("wattack");
        _p2HurtTex   = Content.Load<Texture2D>("whit");
        _p2DeathTex  = Content.Load<Texture2D>("wdeath");

        _e2IdleTex   = Content.Load<Texture2D>("midle");
        _e2RunTex    = Content.Load<Texture2D>("mrun");
        _e2AttackTex = Content.Load<Texture2D>("mattack");
        _e2HurtTex   = Content.Load<Texture2D>("mhit");
        _e2DeathTex  = Content.Load<Texture2D>("mdeath");

        _e3IdleTex   = Content.Load<Texture2D>("Gidle"); 
        _e3WalkTex   = Content.Load<Texture2D>("Grun"); 
        _e3AttackTex = Content.Load<Texture2D>("Gattack"); 
        _e3HurtTex   = Content.Load<Texture2D>("Gtakehit"); 
        _e3DieTex    = Content.Load<Texture2D>("Gdeath"); 

        _btnBaslatTex  = Content.Load<Texture2D>("baslat");
        _btnRestartTex = Content.Load<Texture2D>("restart");
        _btnExitTex    = Content.Load<Texture2D>("exit");

       
        _backgroundMusic = Content.Load<Song>("audio");
        MediaPlayer.IsRepeating = true;
        MediaPlayer.Volume = 0.5f; 
        MediaPlayer.Play(_backgroundMusic); 
    }

    private void StartGame()
    {
        _pBaseHP          = 500;
        _eBaseHP          = 500;
        _myDna            = 100;
        _aiDna            = 100;
        _enemySpawnTimer  = 0;
        _passiveDnaTimer  = 0;
        _aiTargetUnitType = 0;
        _armies.Clear();
        _enemies.Clear();
        _gameState        = 1;
    }

    protected override void Update(GameTime gameTime)
    {
        MouseState currentMouse = Mouse.GetState();
        bool isClicked = currentMouse.LeftButton == ButtonState.Pressed
                      && _prevMouse.LeftButton == ButtonState.Released;

        if (_gameState == 0) 
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            if (isClicked)
            {
                if (_btnPlay.Contains(currentMouse.Position)) StartGame();
                else if (_btnExit.Contains(currentMouse.Position)) Exit();
            }
        }
        else if (_gameState == 1) 
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            UpdatePassiveDna();
            HandlePlayerSpawn(isClicked, currentMouse);
            HandleAiSpawn();
            
            UpdateUnits(_armies, _enemies, _enemyBase, ref _eBaseHP, ref _aiDna);
            UpdateUnits(_enemies, _armies, _playerBase, ref _pBaseHP, ref _myDna);
            
            CleanupDeadUnits();

            if (_pBaseHP <= 0 || _eBaseHP <= 0)
                _gameState = 2;
        }
        else if (_gameState == 2) 
        {
            if (isClicked)
            {
                if (_btnPlay.Contains(currentMouse.Position)) StartGame();
                else if (_btnExit.Contains(currentMouse.Position)) Exit();
            }
        }

        _prevMouse = currentMouse;
        base.Update(gameTime);
    }

    private void UpdatePassiveDna()
    {
        _passiveDnaTimer++;
        if (_passiveDnaTimer >= 60) 
        {
            _myDna = System.Math.Min(_myDna + 5, MAX_DNA);
            _aiDna = System.Math.Min(_aiDna + 5, MAX_DNA);
            _passiveDnaTimer = 0;
        }
    }

    private void HandlePlayerSpawn(bool isSingleClick, MouseState mouse)
    {
        if (!isSingleClick) return;

        if (_button1.Contains(mouse.Position) && _myDna >= 15)
        {
            _myDna -= 15;
            _armies.Add(new Unit(50, 600, 60, 60, 2, 50, 5, Color.White, false, 1));
        }
        else if (_button2.Contains(mouse.Position) && _myDna >= 30)
        {
            _myDna -= 30;
            _armies.Add(new Unit(50, 600, 60, 60, 1, 100, 12, Color.White, false, 2));
        }
        else if (_button3.Contains(mouse.Position) && _myDna >= 50)
        {
            _myDna -= 50;
            _armies.Add(new Unit(50, 600, 70, 70, 1, 200, 20, Color.White, false, 3));
        }
    }

    private void HandleAiSpawn()
    {
        if (_aiTargetUnitType == 0)
        {
            int roll = _rand.Next(1, 100);
            if (roll > 65)      _aiTargetUnitType = 3; 
            else if (roll > 30) _aiTargetUnitType = 2; 
            else                _aiTargetUnitType = 1; 
        }

        _enemySpawnTimer++;
        if (_enemySpawnTimer < 140) return; 

        bool spawned = false;

        if (_aiTargetUnitType == 3 && _aiDna >= 70)
        {
            _enemies.Add(new Unit(1180, 600, 80, 80, -1, 350, 25, Color.White, true, 3));
            _aiDna -= 70;
            spawned = true;
        }
        else if (_aiTargetUnitType == 2 && _aiDna >= 35)
        {
            _enemies.Add(new Unit(1180, 600, 60, 60, -1, 120, 12, Color.White, true, 2));
            _aiDna -= 35;
            spawned = true;
        }
        else if (_aiTargetUnitType == 1 && _aiDna >= 15)
        {
            _enemies.Add(new Unit(1180, 600, 50, 50, -2, 50, 5, Color.White, true, 1));
            _aiDna -= 15;
            spawned = true;
        }

        if (spawned)
        {
            _aiTargetUnitType = 0;
            _enemySpawnTimer  = 0;
        }
    }

    private void UpdateUnits(List<Unit> myUnits, List<Unit> opponentUnits, Rectangle opponentBase, ref int opponentBaseHP, ref int opponentDna)
    {
        bool movingRight = opponentBase.X > 640;

        for (int i = 0; i < myUnits.Count; i++)
        {
            myUnits[i].UpdateAnimation();
            if (myUnits[i].State == 2) continue; 

            Unit target  = null;
            bool fight   = false;
            bool blocked = false;

            foreach (var op in opponentUnits)
            {
                if (op.State != 2 && myUnits[i].Hitbox.Intersects(op.Hitbox))
                {
                    fight  = true;
                    target = op;
                    break;
                }
            }

            if (!fight && myUnits[i].Hitbox.Intersects(opponentBase))
                fight = true;

            if (!fight)
            {
                int checkOffsetX = movingRight ? 0 : -20;
                Rectangle frontBox = new Rectangle(
                    myUnits[i].Hitbox.X + checkOffsetX,
                    myUnits[i].Hitbox.Y,
                    myUnits[i].Hitbox.Width + 20,
                    myUnits[i].Hitbox.Height);

                for (int j = 0; j < myUnits.Count; j++)
                {
                    if (i == j || myUnits[j].State == 2) continue;

                    bool aheadOfMe = movingRight
                        ? myUnits[j].Hitbox.X > myUnits[i].Hitbox.X
                        : myUnits[j].Hitbox.X < myUnits[i].Hitbox.X;

                    if (frontBox.Intersects(myUnits[j].Hitbox) && aheadOfMe)
                    {
                        blocked = true;
                        break;
                    }
                }
            }

            if (fight)
            {
                myUnits[i].State = 1; 
                if (myUnits[i].ReadyToDamage)
                {
                    if (target != null) target.Health -= myUnits[i].Damage;
                    else                opponentBaseHP -= myUnits[i].Damage;
                }
            }
            else if (blocked)
            {
                myUnits[i].State = 4; 
            }
            else
            {
                myUnits[i].State = 0; 
                myUnits[i].Hitbox.X += myUnits[i].SpeedX; 
            }
        }
    }

    private void CleanupDeadUnits()
    {
        for (int i = _enemies.Count - 1; i >= 0; i--)
        {
            if (!_enemies[i].ShouldRemove) continue;
            int reward = _enemies[i].UnitType == 3 ? 55 : _enemies[i].UnitType == 2 ? 30 : 15;
            _myDna = System.Math.Min(_myDna + reward, MAX_DNA);
            _enemies.RemoveAt(i);
        }

        for (int i = _armies.Count - 1; i >= 0; i--)
        {
            if (!_armies[i].ShouldRemove) continue;
            int reward = _armies[i].MaxHealth >= 200 ? 35 : 20;
            _aiDna = System.Math.Min(_aiDna + reward, MAX_DNA);
            _armies.RemoveAt(i);
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        _spriteBatch.Begin();

        _spriteBatch.Draw(_background, new Rectangle(0, 0, 1280, 720), Color.White);

        if (_gameState == 0) 
        {
            DrawMainMenu();
        }
        else if (_gameState == 1) 
        {
            DrawStaticBases(); 
            DrawUnits();
            DrawHealthBars();
            DrawDnaBars();
            DrawSpawnButtons();
        }
        else if (_gameState == 2) 
        {
            DrawStaticBases();
            DrawUnits();
            DrawHealthBars();
            DrawDnaBars();
            DrawGameOver();
        }

        _spriteBatch.End();
        base.Draw(gameTime);
    }

    private void DrawMainMenu()
    {
        int centerX = 640;
        int btnW    = 300;
        int btnH    = 100;
        int btnX    = centerX - btnW / 2;

        _btnPlay = new Rectangle(btnX, 230, btnW, btnH);
        _btnExit = new Rectangle(btnX, 390, btnW, btnH);

        if (_btnBaslatTex != null)
            _spriteBatch.Draw(_btnBaslatTex, _btnPlay, Color.White);

        if (_btnExitTex != null)
            _spriteBatch.Draw(_btnExitTex, _btnExit, Color.White);
    }

    private void DrawGameOver()
    {
        _spriteBatch.Draw(_pixel, new Rectangle(0, 0, 1280, 720), new Color(0, 0, 0, 160));

        int centerX  = 640;
        int btnW     = 300;
        int btnH     = 100;
        int btnX     = centerX - btnW / 2; 

        _btnPlay = new Rectangle(btnX, 230, btnW, btnH);
        _btnExit = new Rectangle(btnX, 390, btnW, btnH);

        if (_btnRestartTex != null)
            _spriteBatch.Draw(_btnRestartTex, _btnPlay, Color.White);

        if (_btnExitTex != null)
            _spriteBatch.Draw(_btnExitTex, _btnExit, Color.White);
    }

    private void DrawStaticBases()
    {
        int floorY = 600; 
        int doorWidth = 50; 
        int doorHeight = 180; 
        int frameThickness = 4; 


        Rectangle leftGateFrame = new Rectangle(0, floorY - doorHeight, doorWidth, doorHeight);
        Rectangle leftGateInterior = new Rectangle(frameThickness, floorY - doorHeight + frameThickness, doorWidth - 2 * frameThickness, doorHeight - frameThickness);
        
        _spriteBatch.Draw(_pixel, leftGateFrame, Color.Gray);
        _spriteBatch.Draw(_pixel, leftGateInterior, new Color(50, 50, 50)); 

        
        int rightGateX = 1280 - doorWidth;
        Rectangle rightGateFrame = new Rectangle(rightGateX, floorY - doorHeight, doorWidth, doorHeight);
        Rectangle rightGateInterior = new Rectangle(rightGateX + frameThickness, floorY - doorHeight + frameThickness, doorWidth - 2 * frameThickness, doorHeight - frameThickness);

        _spriteBatch.Draw(_pixel, rightGateFrame, Color.Gray);
        _spriteBatch.Draw(_pixel, rightGateInterior, new Color(50, 50, 50)); 
    }

    private void DrawUnits()
    {
        foreach (var u in _armies)
        {
            Texture2D tex = _pixel;
            if (u.UnitType == 3) tex = _blackSoldierTex;
            else if (u.UnitType == 1)
            {
                switch (u.State)
                {
                    case 0: tex = _p1RunTex; break;
                    case 1: tex = _p1AttackTex; break;
                    case 4: tex = _p1IdleTex; break;
                    default: tex = _p1HurtTex; break;
                }
            }
            else if (u.UnitType == 2)
            {
                switch (u.State)
                {
                    case 0: tex = _p2RunTex; break;
                    case 1: tex = _p2AttackTex; break;
                    case 2: tex = _p2DeathTex; break;
                    case 4: tex = _p2IdleTex; break;
                    default: tex = _p2HurtTex; break;
                }
            }
            u.Draw(_spriteBatch, tex, _pixel);
        }

        foreach (var e in _enemies)
        {
            Texture2D tex = _pixel;
            if (e.UnitType == 1) tex = _enemySlimeTex;
            else if (e.UnitType == 2)
            {
                switch (e.State)
                {
                    case 0: tex = _e2RunTex; break;
                    case 1: tex = _e2AttackTex; break;
                    case 2: tex = _e2DeathTex; break;
                    case 4: tex = _e2IdleTex; break;
                    default: tex = _e2HurtTex; break;
                }
            }
            else if (e.UnitType == 3)
            {
                switch (e.State)
                {
                    case 0: tex = _e3WalkTex; break;
                    case 1: tex = _e3AttackTex; break;
                    case 2: tex = _e3DieTex; break;
                    case 4: tex = _e3IdleTex; break;
                    default: tex = _e3HurtTex; break;
                }
            }
            e.Draw(_spriteBatch, tex, _pixel);
        }
    }

    private void DrawHealthBars()
    {
        int pW = (_pBaseHP * 150) / _pBaseMaxHP;
        _spriteBatch.Draw(_pixel, new Rectangle(0,    100, 150, 10), Color.Gray);
        _spriteBatch.Draw(_pixel, new Rectangle(0,    100, pW,  10), Color.Lime);

        int eW = (_eBaseHP * 150) / _eBaseMaxHP;
        _spriteBatch.Draw(_pixel, new Rectangle(1130, 100, 150, 10), Color.Gray);
        _spriteBatch.Draw(_pixel, new Rectangle(1130, 100, eW,  10), Color.Red);
    }

    private void DrawDnaBars()
    {
        int myW = (_myDna * 200) / MAX_DNA;
        _spriteBatch.Draw(_pixel, new Rectangle(10,   10, 200, 20), Color.DarkGray);
        _spriteBatch.Draw(_pixel, new Rectangle(10,   10, myW, 20), Color.Cyan);

        int aiW = (_aiDna * 200) / MAX_DNA;
        _spriteBatch.Draw(_pixel, new Rectangle(1070, 10, 200, 20), Color.DarkGray);
        _spriteBatch.Draw(_pixel, new Rectangle(1070, 10, aiW, 20), Color.Magenta);
    }

    private void DrawSpawnButtons()
    {
        Color overlay = new Color(0, 0, 0, 150);
        _spriteBatch.Draw(_pixel, _button1, overlay);
        _spriteBatch.Draw(_pixel, _button2, overlay);
        _spriteBatch.Draw(_pixel, _button3, overlay);

        if (_p1IdleTex != null)
            _spriteBatch.Draw(_p1IdleTex,
                new Rectangle(_button1.X + 10, _button1.Y + 10, 60, 60),
                new Rectangle(0, 0, _p1IdleTex.Height, _p1IdleTex.Height),
                Color.White);

        if (_p2IdleTex != null)
            _spriteBatch.Draw(_p2IdleTex,
                new Rectangle(_button2.X + 10, _button2.Y + 10, 60, 60),
                new Rectangle(0, 0, _p2IdleTex.Width / 6, _p2IdleTex.Height),
                Color.White);

        if (_blackSoldierTex != null)
            _spriteBatch.Draw(_blackSoldierTex,
                new Rectangle(_button3.X + 10, _button3.Y + 10, 60, 60),
                new Rectangle(0, 0, _blackSoldierTex.Width / 3, _blackSoldierTex.Height / 21),
                Color.White);
    }
}