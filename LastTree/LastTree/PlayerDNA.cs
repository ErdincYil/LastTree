using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class PlayerDna
{
    public int Current;
    public int Max;
    public Rectangle BarBg;
    public Rectangle BarFill;

    public PlayerDna(int startAmount, int maxAmount)
    {
        Current = startAmount;
        Max = maxAmount;
        BarBg = new Rectangle(10, 10, 300, 30);
    }

    public void Add(int amount)
    {
        Current += amount;
        if (Current > Max) Current = Max;
    }

    public bool Spend(int amount)
    {
        if (Current >= amount)
        {
            Current -= amount;
            return true;
        }
        return false;
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D pixel)
    {
        spriteBatch.Draw(pixel, BarBg, Color.DimGray);
        
        float ratio = (float)Current / Max;
        BarFill = new Rectangle(BarBg.X, BarBg.Y, (int)(BarBg.Width * ratio), BarBg.Height);
        spriteBatch.Draw(pixel, BarFill, Color.RoyalBlue);
    }
}