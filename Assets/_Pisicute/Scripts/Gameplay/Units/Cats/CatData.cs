using System;

[Serializable]
public class CatData
{
    public CatTypes type;
    public Factions factions;
    public int health;
    public int maxHealth;
    public int power;
    public int speed;

    public CatData(CatData data)
    {
        health = data.health;
        maxHealth = data.maxHealth;
        power = data.power;
        speed = data.speed;
    }

    public override bool Equals(object obj)
    {
        CatData other = obj as CatData;
        return (type == other.type) && (health == other.health) && (maxHealth == other.maxHealth) && (power == other.power) && (speed == other.speed);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(type, health, maxHealth, power, speed);
    }
}
