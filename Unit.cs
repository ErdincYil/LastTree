using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Unit
{
    public Rectangle Hitbox;
    public int Health;
    public int MaxHealth;
    public int SpeedX;
    public int Damage;
    public int UnitType;
    public bool IsEnemy;
    public bool ShouldRemove;
    public bool ReadyToDamage;
    public Color Color;

    public int Frame;
    public int Timer;

    private int state;
    private int prevHealth;
    private int flashTimer;
    private int frameDelay;

    public int State
    {
        get
        {
            return state;
        }
        set
        {
            if (state != value)
            {
                state = value;
                Frame = 0;
                Timer = 0;
            }
        }
    }

    public Unit(int x, int y, int width, int height, int speedX, int health, int damage, Color color, bool isEnemy, int unitType)
    {
        Hitbox = new Rectangle(x, y, width, height);
        SpeedX = speedX;
        Health = health;
        MaxHealth = health;
        Damage = damage;
        Color = color;
        IsEnemy = isEnemy;
        UnitType = unitType;
        prevHealth = health;
        state = 0;
        Frame = 0;
        frameDelay = 5;
    }

    public int GetMaxFrames()
    {
        if (IsEnemy == false)
        {
            if (UnitType == 1)
            {
                if (State == 0) return 8;
                if (State == 1) return 6;
                if (State == 2) return 4;
                if (State == 3) return 4;
                if (State == 4) return 10;
            }
            if (UnitType == 2)
            {
                if (State == 0) return 8;
                if (State == 1) return 8;
                if (State == 2) return 7;
                if (State == 3) return 4;
                if (State == 4) return 6;
            }
            if (UnitType == 3)
            {
                if (State == 4) return 1;
                return 3;
            }
        }
        
        if (IsEnemy == true)
        {
            if (UnitType == 1)
            {
                if (State == 0) return 8;
                if (State == 1) return 5;
                if (State == 2) return 5;
                if (State == 3) return 4;
                if (State == 4) return 1;
            }
            if (UnitType == 2)
            {
                if (State == 0) return 8;
                if (State == 1) return 8;
                if (State == 2) return 4;
                if (State == 3) return 3;
                if (State == 4) return 4;
            }
            if (UnitType == 3)
            {
                if (State == 0) return 8;
                if (State == 1) return 8;
                if (State == 2) return 4;
                if (State == 3) return 4;
                if (State == 4) return 4;
            }
        }
        return 1;
    }

    public int GetCols()
    {
        if (IsEnemy == false)
        {
            if (UnitType == 3) return 3;
        }
        if (IsEnemy == true)
        {
            if (UnitType == 1) return 10;
        }

        if (IsEnemy == false)
        {
            if (UnitType == 1)
            {
                if (State == 0) return 16;
                if (State == 1) return 6;
            }
        }

        return GetMaxFrames();
    }

    public int GetRows()
    {
        if (IsEnemy == false)
        {
            if (UnitType == 3) return 21;
        }
        if (IsEnemy == true)
        {
            if (UnitType == 1) return 8;
        }
        return 1;
    }

    public int GetRowIndex()
    {
        if (IsEnemy == false)
        {
            if (UnitType == 3)
            {
                if (State == 0) return 5;
                if (State == 1) return 3;
                if (State == 2) return 12;
                if (State == 3) return 11;
                return 0;
            }
        }

        if (IsEnemy == true)
        {
            if (UnitType == 1)
            {
                if (State == 0) return 1;
                if (State == 1) return 5;
                if (State == 2) return 4;
                if (State == 3) return 3;
                return 1;
            }
        }

        return 0;
    }

    public void UpdateAnimation()
    {
        ReadyToDamage = false;
        
        if (flashTimer > 0)
        {
            flashTimer = flashTimer - 1;
        }

        if (Health <= 0)
        {
            if (State != 2)
            {
                State = 2;
            }
        }
        
        if (Health < prevHealth)
        {
            if (State != 2)
            {
                flashTimer = 10;
                if (State != 3)
                {
                    if (State != 1)
                    {
                        State = 3;
                    }
                }
                prevHealth = Health;
            }
        }

        int maxFrames = GetMaxFrames();

        bool isIdleUnit = false;
        if (IsEnemy == true)
        {
            if (UnitType == 1) isIdleUnit = true;
            if (UnitType == 2) isIdleUnit = true;
        }
        if (IsEnemy == false)
        {
            if (UnitType == 3) isIdleUnit = true;
        }

        if (State == 4)
        {
            if (isIdleUnit == true)
            {
                Frame = 0;
                return;
            }
        }

        Timer = Timer + 1;
        if (Timer < frameDelay)
        {
            return;
        }

        if (State == 1)
        {
            int hitFrame = maxFrames / 2;
            if (IsEnemy == false)
            {
                if (UnitType == 3) hitFrame = 1;
                if (UnitType == 1) hitFrame = 3;
                if (UnitType == 2) hitFrame = 5;
            }
            if (IsEnemy == true)
            {
                if (UnitType == 2) hitFrame = 5;
                if (UnitType == 3) hitFrame = 5;
            }

            if (Frame == hitFrame)
            {
                ReadyToDamage = true;
            }
        }

        Timer = 0;
        Frame = Frame + 1;

        if (Frame >= maxFrames)
        {
            if (State == 2)
            {
                Frame = maxFrames - 1;
                ShouldRemove = true;
            }
            if (State == 3)
            {
                State = 0;
            }
            if (State != 2)
            {
                if (State != 3)
                {
                    Frame = 0;
                }
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D texture, Texture2D pixelTexture)
    {
        if (texture == null)
        {
            if (flashTimer > 0)
            {
                spriteBatch.Draw(pixelTexture, Hitbox, Color.Red);
            }
            if (flashTimer <= 0)
            {
                spriteBatch.Draw(pixelTexture, Hitbox, Color);
            }
            return;
        }
        
        if (texture.Width == 1)
        {
            if (flashTimer > 0)
            {
                spriteBatch.Draw(pixelTexture, Hitbox, Color.Red);
            }
            if (flashTimer <= 0)
            {
                spriteBatch.Draw(pixelTexture, Hitbox, Color);
            }
            return;
        }

        int cols = GetCols();
        int rows = GetRows();
        int rowIndex = GetRowIndex();

        if (Frame >= cols)
        {
            Frame = 0;
        }

        int cellWidth = texture.Width / cols;
        int cellHeight = texture.Height / rows;

        float scale = 1.0f;
        int offsetX = 0;
        int offsetY = 0;

        if (IsEnemy == false)
        {
            if (UnitType == 1)
            {
                scale = 1.6f;
                offsetX = 0;
                offsetY = 0;
            }
            if (UnitType == 2)
            {
                scale = 1.0f;
                offsetY = 15;
            }
            if (UnitType == 3)
            {
                scale = 2.4f;
                offsetY = -5;
            }
        }
        
        if (IsEnemy == true)
        {
            if (UnitType == 1)
            {
                scale = 1.4f;
                offsetY = 10;
            }
            if (UnitType == 2)
            {
                scale = 1.3f;
                offsetY = 5;
            }
            if (UnitType == 3)
            {
                scale = 2.0f;
                offsetX = 0;
                offsetY = 40;
            }
        }

        int drawWidth = (int)(cellWidth * scale);
        int drawHeight = (int)(cellHeight * scale);

        int floorY = 600;
        int drawX = Hitbox.X + (Hitbox.Width / 2) - (drawWidth / 2) + offsetX;
        int drawY = floorY - drawHeight + offsetY;

        Rectangle drawRect = new Rectangle(drawX, drawY, drawWidth, drawHeight);
        Rectangle sourceRect = new Rectangle(Frame * cellWidth, rowIndex * cellHeight, cellWidth, cellHeight);

        SpriteEffects effects = SpriteEffects.None;
        if (IsEnemy == true)
        {
            effects = SpriteEffects.FlipHorizontally;
        }

        Color drawColor = Color.White;
        if (flashTimer > 0)
        {
            drawColor = Color.Red;
        }

        spriteBatch.Draw(texture, drawRect, sourceRect, drawColor, 0f, Vector2.Zero, effects, 0f);

        if (State != 2)
        {
            int barWidth = (int)(drawWidth * 0.6f);
            
            if (barWidth < 20)
            {
                barWidth = 20;
            }
            if (barWidth > 60)
            {
                barWidth = 60;
            }

            int filled = (Health * barWidth) / MaxHealth;
            int barX = Hitbox.X + (Hitbox.Width / 2) - (barWidth / 2);
            int barY = drawY - 10;

            spriteBatch.Draw(pixelTexture, new Rectangle(barX, barY, barWidth, 4), Color.Red);
            spriteBatch.Draw(pixelTexture, new Rectangle(barX, barY, filled, 4), Color.Lime);
        }
    }
}