using Microsoft.Xna.Framework;

public class Unit
{
    public Rectangle Hitbox;
    public int SpeedX;
    public Color Color;
    public int MaxHealth;
    public int Health;
    public int Damage;
    public int Cooldown;
    public bool IsEnemy; 

    public Unit(int x, int y, int size, int speed, int health, int damage, Color color, bool isEnemy)
    {
        Hitbox = new Rectangle(x, y, size, size);
        SpeedX = speed;
        Color = color;
        MaxHealth = health;
        Health = health;
        Damage = damage;
        Cooldown = 0;
        IsEnemy = isEnemy;
    }
}