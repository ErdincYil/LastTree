using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _pixel;

    private Rectangle _ground;
    private Rectangle _tree;
    private int _treeMaxHealth = 1000;
    private int _treeHealth = 1000;

    private List<Army> _armies = new List<Army>();
    private List<Enemy> _enemies = new List<Enemy>();

    private PlayerDna _myDna;
    
    private Rectangle[] _spawnButtons = new Rectangle[3];
    private int[] _unitCosts = { 20, 40, 70 };
    private MouseState _prevMouse;

    private float _enemySpawnTimer = 0;
    private int _enemyDNA = 30;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _ground = new Rectangle(0, 620, 1280, 100);
        _tree = new Rectangle(0, 120, 150, 500);

        _myDna = new PlayerDna(50, 100);

        for (int i = 0; i < 3; i++)
            _spawnButtons[i] = new Rectangle(200 + (i * 100), 630, 80, 80);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });
    }

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

        MouseState currentMouse = Mouse.GetState();
        Point mousePos = new Point(currentMouse.X, currentMouse.Y);

        if (currentMouse.LeftButton == ButtonState.Pressed && _prevMouse.LeftButton == ButtonState.Released)
        {
            if (_spawnButtons[0].Contains(mousePos) && _myDna.Spend(_unitCosts[0]))
            {
                _armies.Add(new Army(150, 570, 50, 2, 50, 10, Color.LightBlue));
            }
            else if (_spawnButtons[1].Contains(mousePos) && _myDna.Spend(_unitCosts[1]))
            {
                _armies.Add(new Army(150, 545, 75, 1, 100, 25, Color.Blue));
            }
            else if (_spawnButtons[2].Contains(mousePos) && _myDna.Spend(_unitCosts[2]))
            {
                _armies.Add(new Army(150, 520, 100, 1, 200, 50, Color.DarkBlue));
            }
        }
        _prevMouse = currentMouse;

        _enemySpawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (_enemySpawnTimer > 3.0f)
        {
            _enemyDNA += 25;
            if (_enemyDNA >= 70) { _enemies.Add(new Enemy(1280, 520, 100, -1, 200, 50, Color.DarkRed)); _enemyDNA -= 70; }
            else if (_enemyDNA >= 40) { _enemies.Add(new Enemy(1280, 545, 75, -1, 100, 25, Color.Red)); _enemyDNA -= 40; }
            else if (_enemyDNA >= 20) { _enemies.Add(new Enemy(1280, 570, 50, -2, 50, 10, Color.Orange)); _enemyDNA -= 20; }
            _enemySpawnTimer = 0;
        }

        foreach (var a in _armies)
        {
            bool fight = false;
            foreach (var e in _enemies)
            {
                if (a.Hitbox.Intersects(e.Hitbox))
                {
                    fight = true;
                    if (a.Cooldown == 0) { e.Health -= a.Damage; a.Cooldown = 40; }
                    break;
                }
            }
            if (!fight) a.Hitbox.X += a.SpeedX;
            if (a.Cooldown > 0) a.Cooldown--;
        }

        foreach (var e in _enemies)
        {
            bool fight = false;
            foreach (var a in _armies)
            {
                if (e.Hitbox.Intersects(a.Hitbox))
                {
                    fight = true;
                    if (e.Cooldown == 0) { a.Health -= e.Damage; e.Cooldown = 40; }
                    break;
                }
            }
            if (!fight && e.Hitbox.Intersects(_tree))
            {
                fight = true;
                if (e.Cooldown == 0) { _treeHealth -= e.Damage; e.Cooldown = 40; }
            }
            if (!fight) e.Hitbox.X += e.SpeedX;
            if (e.Cooldown > 0) e.Cooldown--;
        }

        if (_treeHealth < 0) _treeHealth = 0;

        for (int i = _enemies.Count - 1; i >= 0; i--)
        {
            if (_enemies[i].Health <= 0) 
            { 
                _myDna.Add(15); 
                _enemies.RemoveAt(i); 
            }
        }

        for (int i = _armies.Count - 1; i >= 0; i--)
            if (_armies[i].Health <= 0) _armies.RemoveAt(i);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.SkyBlue);
        _spriteBatch.Begin();

        _spriteBatch.Draw(_pixel, _ground, Color.ForestGreen);
        _spriteBatch.Draw(_pixel, _tree, Color.SaddleBrown);

        int treeHpWidth = (int)((_treeHealth / (float)_treeMaxHealth) * 150);
        _spriteBatch.Draw(_pixel, new Rectangle(_tree.X, _tree.Y - 20, 150, 10), Color.Gray);
        _spriteBatch.Draw(_pixel, new Rectangle(_tree.X, _tree.Y - 20, treeHpWidth, 10), Color.Lime);

        _myDna.Draw(_spriteBatch, _pixel);

        for (int i = 0; i < 3; i++)
        {
            _spriteBatch.Draw(_pixel, _spawnButtons[i], Color.Gray);
            _spriteBatch.Draw(_pixel, new Rectangle(_spawnButtons[i].X, _spawnButtons[i].Y + 85, (int)(80 * (_unitCosts[i]/100f)), 10), Color.Yellow);
        }

        foreach (var a in _armies)
        {
            _spriteBatch.Draw(_pixel, a.Hitbox, a.Color);
            _spriteBatch.Draw(_pixel, new Rectangle(a.Hitbox.X, a.Hitbox.Y - 15, (int)(a.Hitbox.Width * (a.Health / (float)a.MaxHealth)), 8), Color.Lime);
        }

        foreach (var e in _enemies)
        {
            _spriteBatch.Draw(_pixel, e.Hitbox, e.Color);
            _spriteBatch.Draw(_pixel, new Rectangle(e.Hitbox.X, e.Hitbox.Y - 15, (int)(e.Hitbox.Width * (e.Health / (float)e.MaxHealth)), 8), Color.Red);
        }

        _spriteBatch.End();
        base.Draw(gameTime);
    }
}