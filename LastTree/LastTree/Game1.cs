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

    private List<Army> _armies;
    private List<Enemy> _enemies;

    private int _playerDNA = 0;
    private int _maxDNA = 500;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
    }

    protected override void Initialize()
    {
        _ground = new Rectangle(0, 620, 1280, 100);
        _tree = new Rectangle(0, 120, 150, 500);

        _armies = new List<Army>();
        _enemies = new List<Enemy>();

        _armies.Add(new Army(200, 570, 50, 2, 50, 10, Color.DarkBlue));
        _armies.Add(new Army(350, 520, 100, 1, 150, 30, Color.Navy));
        
        _enemies.Add(new Enemy(1000, 570, 50, -2, 50, 10, Color.Orange));
        _enemies.Add(new Enemy(1150, 520, 100, -1, 150, 30, Color.DarkOrange));

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

        foreach (var a in _armies)
        {
            bool isFighting = false;
            foreach (var e in _enemies)
            {
                if (a.Hitbox.Intersects(e.Hitbox))
                {
                    isFighting = true;
                    if (a.Cooldown == 0)
                    {
                        e.Health -= a.Damage;
                        a.Cooldown = 30;
                    }
                    break;
                }
            }

            if (!isFighting) a.Hitbox.X += a.SpeedX;
            if (a.Cooldown > 0) a.Cooldown--;
        }

        foreach (var e in _enemies)
        {
            bool isFighting = false;
            foreach (var a in _armies)
            {
                if (e.Hitbox.Intersects(a.Hitbox))
                {
                    isFighting = true;
                    if (e.Cooldown == 0)
                    {
                        a.Health -= e.Damage;
                        e.Cooldown = 30;
                    }
                    break;
                }
            }

            if (!isFighting && e.Hitbox.Intersects(_tree))
            {
                isFighting = true;
                if (e.Cooldown == 0)
                {
                    _treeHealth -= e.Damage;
                    e.Cooldown = 30;
                }
            }

            if (!isFighting) e.Hitbox.X += e.SpeedX;
            if (e.Cooldown > 0) e.Cooldown--;
        }

        for (int i = _enemies.Count - 1; i >= 0; i--)
        {
            if (_enemies[i].Health <= 0)
            {
                _playerDNA += 50;
                if (_playerDNA > _maxDNA) _playerDNA = _maxDNA;
                _enemies.RemoveAt(i);
            }
        }

        for (int i = _armies.Count - 1; i >= 0; i--)
        {
            if (_armies[i].Health <= 0)
            {
                _armies.RemoveAt(i);
            }
        }

        if (_treeHealth < 0) _treeHealth = 0;

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

        int dnaBarWidth = (int)((_playerDNA / (float)_maxDNA) * 300);
        _spriteBatch.Draw(_pixel, new Rectangle(10, 10, 300, 20), Color.Gray);
        _spriteBatch.Draw(_pixel, new Rectangle(10, 10, dnaBarWidth, 20), Color.Blue);

        foreach (var a in _armies)
        {
            _spriteBatch.Draw(_pixel, a.Hitbox, a.Color);
            int hpWidth = (int)((a.Health / (float)a.MaxHealth) * a.Hitbox.Width);
            _spriteBatch.Draw(_pixel, new Rectangle(a.Hitbox.X, a.Hitbox.Y - 10, a.Hitbox.Width, 5), Color.Red);
            _spriteBatch.Draw(_pixel, new Rectangle(a.Hitbox.X, a.Hitbox.Y - 10, hpWidth, 5), Color.Lime);
        }

        foreach (var e in _enemies)
        {
            _spriteBatch.Draw(_pixel, e.Hitbox, e.Color);
            int hpWidth = (int)((e.Health / (float)e.MaxHealth) * e.Hitbox.Width);
            _spriteBatch.Draw(_pixel, new Rectangle(e.Hitbox.X, e.Hitbox.Y - 10, e.Hitbox.Width, 5), Color.Red);
            _spriteBatch.Draw(_pixel, new Rectangle(e.Hitbox.X, e.Hitbox.Y - 10, hpWidth, 5), Color.Lime);
        }

        _spriteBatch.End();
        base.Draw(gameTime);
    }
}