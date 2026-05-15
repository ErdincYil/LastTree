using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class PlayerDna
{
    public int Current;
    public int Max;
    public Rectangle BarBg;

    public PlayerDna()
    {
        Current = 50;
        Max = 300;
        BarBg = new Rectangle(10, 10, 300, 30);
    }

    public void Add(int amount)
    {
        Current = Current + amount;
        if (Current > Max) Current = Max;
    }

    public bool Spend(int amount)
    {
        if (Current >= amount)
        {
            Current = Current - amount;
            return true;
        }
        return false;
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D pixel)
    {
        spriteBatch.Draw(pixel, BarBg, Color.DimGray);
        int maviUzunluk = (Current * 300) / Max;
        Rectangle BarFill = new Rectangle(10, 10, maviUzunluk, 30);
        spriteBatch.Draw(pixel, BarFill, Color.RoyalBlue);
    }
}