using Microsoft.Xna.Framework;

public class Army
{
    public Rectangle Hitbox;
    public int SpeedX;
    public Color Color;
    public int MaxHealth;
    public int Health;
    public int Damage;
    public int Cooldown;

    public Army(int x, int y, int size, int speed, int health, int damage, Color color)
    {
        Hitbox = new Rectangle(x, y, size, size);
        SpeedX = speed;
        Color = color;
        MaxHealth = health;
        Health = health;
        Damage = damage;
        Cooldown = 0;
    }
}