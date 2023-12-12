using System;

[Serializable]
public class CatData
{
    [Serializable]
    public struct Stat<T>
    {
        public T baseValue;
        public T value;

        public static bool operator ==(Stat<T> a, Stat<T>b)
        {
            return a.baseValue.Equals(b.baseValue) && a.value.Equals(b.value);
        }

        public static bool operator !=(Stat<T> a, Stat<T> b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return this == (Stat<T>)obj;
        }
    }
    public CatTypes type;
    public Factions factions;
    public int health;
    public int shield;
    public Stat<int> maxHealth;
    public Stat<int> power;
    public Stat<int> speed;

    public CatData(CatData data)
    {
        health = data.health;
        shield = data.shield;
        maxHealth = data.maxHealth;
        power = data.power;
        speed = data.speed;
    }

    public override bool Equals(object obj)
    {
        CatData other = obj as CatData;
        return (type == other.type) && (health == other.health) && (shield == other.shield) && (maxHealth == other.maxHealth) && (power == other.power) && (speed == other.speed);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(type, health, shield, maxHealth, power, speed);
    }
}
