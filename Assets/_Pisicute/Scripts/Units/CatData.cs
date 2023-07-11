using System;

[Serializable]
public class CatData
{
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
}
