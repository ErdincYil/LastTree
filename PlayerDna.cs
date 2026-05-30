using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class PlayerDNA
{
    public int Amount { get; private set; } = 50; 
    public int MaxAmount { get; private set; } = 300; // YENİ: Maksimum DNA sınırı

    public void Add(int amount)
    {
        Amount += amount;
        // YENİ: DNA miktarının sınırı aşmasını engelliyoruz
        if (Amount > MaxAmount) 
        {
            Amount = MaxAmount;
        }
    }

    public bool Spend(int amount)
    {
        if (Amount >= amount)
        {
            Amount -= amount;
            return true;
        }
        return false;
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D pixel)
    {
        // YENİ: Maksimum kapasiteyi gösteren statik gri arka plan barı
        spriteBatch.Draw(pixel, new Rectangle(40, 15, MaxAmount, 10), Color.DimGray); 

        // Mevcut sarı DNA barımız (Gri barın üstüne çizilir)
        spriteBatch.Draw(pixel, new Rectangle(40, 15, Amount, 10), Color.Gold); 
        
        // Sol taraftaki küçük altın ikon kutusu
        spriteBatch.Draw(pixel, new Rectangle(10, 10, 20, 20), Color.Gold); 
    }
}