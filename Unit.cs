using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Unit
{
    public Rectangle Hitbox;
    public int Health, MaxHealth, SpeedX, Damage, UnitType;
    public bool IsEnemy, ShouldRemove, ReadyToDamage;
    public Color Color;

    public int Frame, Timer;

    private int _state;
    private int _prevHealth;
    private int _flashTimer;
    private const int FrameDelay = 5;

    public int State
    {
        get => _state;
        set
        {
            if (_state == value) return;
            _state = value;
            Frame = 0;
            Timer = 0;
        }
    }

    public Unit(int x, int y, int width, int height, int speedX, int health, int damage, Color color, bool isEnemy, int unitType)
    {
        Hitbox     = new Rectangle(x, y, width, height);
        SpeedX     = speedX;
        Health     = health;
        MaxHealth  = health;
        Damage     = damage;
        Color      = color;
        IsEnemy    = isEnemy;
        UnitType   = unitType;
        _prevHealth = health;
        _state     = 0;
        Frame      = 0;
    }

    public int GetMaxFrames()
    {
        if (!IsEnemy)
        {            if (UnitType == 1) return State == 0 ? 8 : (State == 1 ? 6 : (State == 2 ? 4 : (State == 3 ? 4 : 10)));
            if (UnitType == 2) return State == 0 ? 8 : (State == 1 ? 8 : (State == 2 ? 7 : (State == 3 ? 4 : 6)));
            if (UnitType == 3) return State == 4 ? 1 : 3;
        }
        else
        {
            if (UnitType == 1) return State == 0 ? 8 : (State == 1 ? 5 : (State == 2 ? 5 : (State == 3 ? 4 : 1)));
            if (UnitType == 2) return State == 0 ? 8 : (State == 1 ? 8 : (State == 2 ? 4 : (State == 3 ? 3 : 4)));
            if (UnitType == 3) return State == 0 ? 8 : (State == 1 ? 8 : (State == 2 ? 4 : (State == 3 ? 4 : 4)));
        }
        return 1;
    }

    public int GetCols()
{
    if (!IsEnemy && UnitType == 3) return 3;
    if (IsEnemy  && UnitType == 1) return 10;

    if (!IsEnemy && UnitType == 1)
    {
        if (State == 0) return 16;
        if (State == 1) return 6;
    }

    return GetMaxFrames();
}
    public int GetRows()
    {
        if (!IsEnemy && UnitType == 3) return 21;
        if (IsEnemy  && UnitType == 1) return 8;
        return 1;
    }

    public int GetRowIndex()
    {
        if (!IsEnemy && UnitType == 3)
            return State == 0 ? 5 : (State == 1 ? 3 : (State == 2 ? 12 : (State == 3 ? 11 : 0)));

        if (IsEnemy && UnitType == 1)
            return State == 0 ? 1 : (State == 1 ? 5 : (State == 2 ? 4 : (State == 3 ? 3 : 1)));

        return 0;
    }

    public void UpdateAnimation()
    {
        ReadyToDamage = false;
        if (_flashTimer > 0) _flashTimer--;

        if (Health <= 0 && State != 2)
        {
            State = 2; 
        }
        else if (Health < _prevHealth && State != 2)
        {
            _flashTimer = 10;
            if (State != 3 && State != 1) State = 3; 
            _prevHealth = Health;
        }

        int maxFrames = GetMaxFrames();
        bool isIdleUnit = (IsEnemy && UnitType == 1)
                       || (IsEnemy && UnitType == 2)
                       || (!IsEnemy && UnitType == 3);

        if (State == 4 && isIdleUnit)
        {
            Frame = 0;
            return;
        }

        Timer++;
        if (Timer < FrameDelay) return;

        if (State == 1)
        {
            int hitFrame = maxFrames / 2;
            if (!IsEnemy && UnitType == 3) hitFrame = 1;
            if (!IsEnemy && UnitType == 1) hitFrame = 3;
            if (!IsEnemy && UnitType == 2) hitFrame = 5;
            if ( IsEnemy && UnitType == 2) hitFrame = 5;
            if ( IsEnemy && UnitType == 3) hitFrame = 5; 

            if (Frame == hitFrame) ReadyToDamage = true;
        }

        Timer = 0;
        Frame++;

        if (Frame >= maxFrames)
        {
            if (State == 2) { Frame = maxFrames - 1; ShouldRemove = true; } 
            else if (State == 3) State = 0; 
            else Frame = 0; 
        }
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D texture, Texture2D pixelTexture)
    {
        if (texture == null || texture.Width == 1)
        {
            spriteBatch.Draw(pixelTexture, Hitbox, _flashTimer > 0 ? Color.Red : Color);
            return;
        }

        int cols     = GetCols();
        int rows     = GetRows();
        int rowIndex = GetRowIndex();

        if (Frame >= cols) Frame = 0;

        int cellWidth  = texture.Width  / cols;
        int cellHeight = texture.Height / rows;

        float scale   = 1.0f;
        int offsetX   = 0;
        int offsetY   = 0;

        if (!IsEnemy)
        {
            if (UnitType == 1) { scale = 1.6f; offsetX = 0; offsetY = 0; } 
            else if (UnitType == 2) { scale = 1.0f; offsetY = 15; } 
            else if (UnitType == 3) { scale = 2.4f; offsetY = -5; } 
        }
        else
        {
            if (UnitType == 1) { scale = 1.4f; offsetY = 10; } 
            else if (UnitType == 2) { scale = 1.3f; offsetY = 5; } 
            else if (UnitType == 3) 
            { 
                scale = 2.0f; 
                if (State == 0)      { offsetX = 0; offsetY = 40; } 
                else if (State == 1) { offsetX = 0; offsetY = 40; } 
                else if (State == 2) { offsetX = 0; offsetY = 40; } 
                else if (State == 3) { offsetX = 0; offsetY = 40; } 
                else if (State == 4) { offsetX = 0; offsetY = 40; } 
            }
        }

        int drawWidth  = (int)(cellWidth  * scale);
        int drawHeight = (int)(cellHeight * scale);

        int floorY = 600;
        int drawX  = Hitbox.X + Hitbox.Width / 2 - drawWidth  / 2 + offsetX;
        int drawY  = floorY - drawHeight + offsetY;

        Rectangle drawRect   = new Rectangle(drawX, drawY, drawWidth, drawHeight);
        Rectangle sourceRect = new Rectangle(Frame * cellWidth, rowIndex * cellHeight, cellWidth, cellHeight);

        SpriteEffects effects = IsEnemy ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        Color drawColor       = _flashTimer > 0 ? Color.Red : Color.White;

        spriteBatch.Draw(texture, drawRect, sourceRect, drawColor, 0f, Vector2.Zero, effects, 0f);

        DrawHealthBar(spriteBatch, pixelTexture, drawX, drawY, drawWidth);
    }

    private void DrawHealthBar(SpriteBatch spriteBatch, Texture2D pixelTexture, int drawX, int drawY, int drawWidth)
    {
        if (State == 2) return;

        int barWidth = (int)(drawWidth * 0.6f);
        barWidth = System.Math.Clamp(barWidth, 20, 60);

        int filled = (Health * barWidth) / MaxHealth;
        int barX   = Hitbox.X + Hitbox.Width / 2 - barWidth / 2;
        int barY   = drawY - 10;

        spriteBatch.Draw(pixelTexture, new Rectangle(barX, barY, barWidth, 4), Color.Red);
        spriteBatch.Draw(pixelTexture, new Rectangle(barX, barY, filled,   4), Color.Lime);
    }
}