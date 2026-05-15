using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _pixel;

    private int _gameState = 0; 
    
    private Rectangle _btnPlay;
    private Rectangle _btnExit;

    private Rectangle _ground;
    private Rectangle _tree;
    private int _treeMaxHealth = 1000;
    private int _treeHealth = 1000;

    private List<Unit> _armies = new List<Unit>();
    private List<Unit> _enemies = new List<Unit>();

    private PlayerDna _myDna;
    
    private Rectangle _button1;
    private Rectangle _button2;
    private Rectangle _button3;
    
    private MouseState _prevMouse;

    private int _currentLevel = 1;
    private int _enemySpawnTimer = 0;
    
    private int _enemiesSpawned = 0;
    private int _enemiesKilled = 0;
    private int _enemiesToKillThisLevel = 15; 

    private int _aiDna = 50;
    private System.Random _rand = new System.Random();

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _btnPlay = new Rectangle(540, 200, 200, 100); 
        _btnExit = new Rectangle(540, 400, 200, 100); 

        _ground = new Rectangle(0, 620, 1280, 100);
        _tree = new Rectangle(0, 120, 150, 500);

        _myDna = new PlayerDna();

        _button1 = new Rectangle(200, 630, 80, 80); 
        _button2 = new Rectangle(300, 630, 80, 80); 
        _button3 = new Rectangle(400, 630, 80, 80); 

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });
    }

    private void StartNextLevel()
    {
        _enemiesSpawned = 0;
        _enemiesKilled = 0;
        _aiDna = 50;
        _armies.Clear();
        _enemies.Clear();
        
        if (_currentLevel == 2) { _enemiesToKillThisLevel = 25; }
        if (_currentLevel == 3) { _enemiesToKillThisLevel = 40; }
        
        _gameState = 1; 
    }

    protected override void Update(GameTime gameTime)
    {
        MouseState currentMouse = Mouse.GetState();
        Point mousePos = new Point(currentMouse.X, currentMouse.Y);
        bool isClicked = (currentMouse.LeftButton == ButtonState.Pressed && _prevMouse.LeftButton == ButtonState.Released);

        if (_gameState == 0) 
        {
            if (isClicked && _btnPlay.Contains(mousePos)) { _gameState = 1; }
            if (isClicked && _btnExit.Contains(mousePos)) { Exit(); }
        }
        else if (_gameState == 2) 
        {
            if (isClicked) { StartNextLevel(); }
        }
        else if (_gameState == 3) 
        {
            if (isClicked) { Exit(); }
        }
        else if (_gameState == 1) 
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            if (isClicked)
            {
                if (_button1.Contains(mousePos) && _myDna.Spend(15))
                    _armies.Add(new Unit(150, 570, 50, 2, 50, 10, Color.LightBlue, false));
                else if (_button2.Contains(mousePos) && _myDna.Spend(30))
                    _armies.Add(new Unit(150, 545, 75, 1, 100, 25, Color.Blue, false));
                else if (_button3.Contains(mousePos) && _myDna.Spend(50))
                    _armies.Add(new Unit(150, 520, 100, 1, 200, 50, Color.DarkBlue, false));
            }

            _enemySpawnTimer++;
            
            if (_enemySpawnTimer >= 60)
            {
                if (_currentLevel == 1) _aiDna += 10;
                if (_currentLevel == 2) _aiDna += 15;
                if (_currentLevel == 3) _aiDna += 20;

                if (_enemiesSpawned < _enemiesToKillThisLevel)
                {
                    int tip = _rand.Next(1, 100);
                    bool spawned = false;

                    if (_currentLevel == 1)
                    {
                        if (tip > 85 && _aiDna >= 30) { _enemies.Add(new Unit(1280, 545, 75, -1, 100, 25, Color.Red, true)); _aiDna -= 30; spawned = true; }
                        else if (_aiDna >= 15) { _enemies.Add(new Unit(1280, 570, 50, -2, 50, 10, Color.Orange, true)); _aiDna -= 15; spawned = true; }
                    }
                    else if (_currentLevel == 2)
                    {
                        if (tip > 90 && _aiDna >= 50) { _enemies.Add(new Unit(1280, 520, 100, -1, 200, 50, Color.DarkRed, true)); _aiDna -= 50; spawned = true; }
                        else if (tip > 50 && _aiDna >= 30) { _enemies.Add(new Unit(1280, 545, 75, -1, 100, 25, Color.Red, true)); _aiDna -= 30; spawned = true; }
                        else if (_aiDna >= 15) { _enemies.Add(new Unit(1280, 570, 50, -2, 50, 10, Color.Orange, true)); _aiDna -= 15; spawned = true; }
                    }
                    else if (_currentLevel == 3)
                    {
                        if (tip > 70 && _aiDna >= 50) { _enemies.Add(new Unit(1280, 520, 100, -1, 200, 50, Color.DarkRed, true)); _aiDna -= 50; spawned = true; }
                        else if (tip > 30 && _aiDna >= 30) { _enemies.Add(new Unit(1280, 545, 75, -1, 100, 25, Color.Red, true)); _aiDna -= 30; spawned = true; }
                        else if (_aiDna >= 15) { _enemies.Add(new Unit(1280, 570, 50, -2, 50, 10, Color.Orange, true)); _aiDna -= 15; spawned = true; }
                    }

                    if (spawned) _enemiesSpawned++;
                }
                _enemySpawnTimer = 0;
            }

            for (int i = 0; i < _armies.Count; i++)
            {
                bool isFighting = false;
                bool isBlockedByFriend = false; 

                for (int j = 0; j < _enemies.Count; j++)
                {
                    if (_armies[i].Hitbox.Intersects(_enemies[j].Hitbox))
                    {
                        isFighting = true;
                        if (_armies[i].Cooldown == 0) 
                        { 
                            _enemies[j].Health = _enemies[j].Health - _armies[i].Damage; 
                            _armies[i].Cooldown = 60; 
                        }
                    }
                }

                if (isFighting == false)
                {
                    for (int j = 0; j < _armies.Count; j++)
                    {
                        if (i != j) 
                        {
                            if (_armies[i].Hitbox.Intersects(_armies[j].Hitbox))
                            {
                                if (_armies[j].Hitbox.X > _armies[i].Hitbox.X)
                                {
                                    isBlockedByFriend = true;
                                }
                            }
                        }
                    }
                }

                if (isFighting == false && isBlockedByFriend == false) { _armies[i].Hitbox.X += _armies[i].SpeedX; }
                if (_armies[i].Cooldown > 0) { _armies[i].Cooldown--; }
            }

            for (int i = 0; i < _enemies.Count; i++)
            {
                bool isFighting = false;
                bool isBlockedByFriend = false;

                for (int j = 0; j < _armies.Count; j++)
                {
                    if (_enemies[i].Hitbox.Intersects(_armies[j].Hitbox))
                    {
                        isFighting = true;
                        if (_enemies[i].Cooldown == 0) 
                        { 
                            _armies[j].Health = _armies[j].Health - _enemies[i].Damage; 
                            _enemies[i].Cooldown = 60; 
                        }
                    }
                }
                
                if (isFighting == false && _enemies[i].Hitbox.Intersects(_tree))
                {
                    isFighting = true;
                    if (_enemies[i].Cooldown == 0) 
                    { 
                        _treeHealth -= _enemies[i].Damage; 
                        _enemies[i].Cooldown = 60; 
                    }
                }

                if (isFighting == false)
                {
                    for (int j = 0; j < _enemies.Count; j++)
                    {
                        if (i != j && _enemies[i].Hitbox.Intersects(_enemies[j].Hitbox))
                        {
                            if (_enemies[j].Hitbox.X < _enemies[i].Hitbox.X)
                            {
                                isBlockedByFriend = true;
                            }
                        }
                    }
                }

                if (isFighting == false && isBlockedByFriend == false) { _enemies[i].Hitbox.X += _enemies[i].SpeedX; }
                if (_enemies[i].Cooldown > 0) { _enemies[i].Cooldown--; }
            }

            for (int i = _enemies.Count - 1; i >= 0; i--)
            {
                if (_enemies[i].Health <= 0) 
                { 
                    int reward = 15;
                    if (_enemies[i].MaxHealth == 100) reward = 30;
                    if (_enemies[i].MaxHealth == 200) reward = 50;

                    _myDna.Add(reward); 
                    _enemiesKilled++;
                    _enemies.RemoveAt(i); 
                }
            }

            for (int i = _armies.Count - 1; i >= 0; i--)
            {
                if (_armies[i].Health <= 0) { _armies.RemoveAt(i); }
            }

            if (_treeHealth <= 0) 
            {
                _gameState = 3; 
            }
            
            if (_enemiesKilled >= _enemiesToKillThisLevel)
            {
                if (_currentLevel == 3)
                {
                    _gameState = 3; 
                }
                else
                {
                    _currentLevel++;
                    _gameState = 2; 
                }
            }
        }
        
        _prevMouse = currentMouse;
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.SkyBlue);
        _spriteBatch.Begin();

        if (_gameState == 0) 
        {
            _spriteBatch.Draw(_pixel, _btnPlay, Color.LimeGreen); 
            _spriteBatch.Draw(_pixel, _btnExit, Color.Red);       
        }
        else if (_gameState == 2) 
        {
            _spriteBatch.Draw(_pixel, new Rectangle(440, 260, 400, 200), Color.DarkBlue);
        }
        else if (_gameState == 3) 
        {
            if (_treeHealth <= 0)
                _spriteBatch.Draw(_pixel, new Rectangle(440, 260, 400, 200), Color.DarkRed); 
            else
                _spriteBatch.Draw(_pixel, new Rectangle(440, 260, 400, 200), Color.Gold); 
        }
        else if (_gameState == 1) 
        {
            _spriteBatch.Draw(_pixel, _ground, Color.ForestGreen);
            _spriteBatch.Draw(_pixel, _tree, Color.SaddleBrown);

            int treeHpWidth = (_treeHealth * 150) / _treeMaxHealth;
            _spriteBatch.Draw(_pixel, new Rectangle(_tree.X, _tree.Y - 20, 150, 10), Color.Gray);
            _spriteBatch.Draw(_pixel, new Rectangle(_tree.X, _tree.Y - 20, treeHpWidth, 10), Color.Lime);

            _myDna.Draw(_spriteBatch, _pixel);

            _spriteBatch.Draw(_pixel, _button1, Color.Gray);
            _spriteBatch.Draw(_pixel, new Rectangle(_button1.X, _button1.Y + 85, 16, 10), Color.Yellow); 

            _spriteBatch.Draw(_pixel, _button2, Color.Gray);
            _spriteBatch.Draw(_pixel, new Rectangle(_button2.X, _button2.Y + 85, 32, 10), Color.Yellow); 

            _spriteBatch.Draw(_pixel, _button3, Color.Gray);
            _spriteBatch.Draw(_pixel, new Rectangle(_button3.X, _button3.Y + 85, 56, 10), Color.Yellow); 

            int progressWidth = (_enemiesKilled * 200) / _enemiesToKillThisLevel;
            _spriteBatch.Draw(_pixel, new Rectangle(1000, 20, 200, 20), Color.DarkSlateGray);
            _spriteBatch.Draw(_pixel, new Rectangle(1000, 20, progressWidth, 20), Color.Magenta); 

            for (int i = 0; i < _armies.Count; i++)
            {
                _spriteBatch.Draw(_pixel, _armies[i].Hitbox, _armies[i].Color);
                int hpWidth = (_armies[i].Health * _armies[i].Hitbox.Width) / _armies[i].MaxHealth;
                _spriteBatch.Draw(_pixel, new Rectangle(_armies[i].Hitbox.X, _armies[i].Hitbox.Y - 15, _armies[i].Hitbox.Width, 8), Color.Red);
                _spriteBatch.Draw(_pixel, new Rectangle(_armies[i].Hitbox.X, _armies[i].Hitbox.Y - 15, hpWidth, 8), Color.Lime);
            }

            for (int i = 0; i < _enemies.Count; i++)
            {
                _spriteBatch.Draw(_pixel, _enemies[i].Hitbox, _enemies[i].Color);
                int hpWidth = (_enemies[i].Health * _enemies[i].Hitbox.Width) / _enemies[i].MaxHealth;
                _spriteBatch.Draw(_pixel, new Rectangle(_enemies[i].Hitbox.X, _enemies[i].Hitbox.Y - 15, _enemies[i].Hitbox.Width, 8), Color.Red);
                _spriteBatch.Draw(_pixel, new Rectangle(_enemies[i].Hitbox.X, _enemies[i].Hitbox.Y - 15, hpWidth, 8), Color.Lime);
            }
        }

        _spriteBatch.End();
        base.Draw(gameTime);
    }
}