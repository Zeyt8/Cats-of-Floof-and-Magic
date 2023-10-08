using System;

[Serializable]
public class Resources
{
    public int food;
    public int wood;
    public int stone;
    public int steel;
    public int sulfur;
    public int gems;

    public enum Resource
    {
        Food,
        Wood,
        Stone,
        Steel,
        Sulfur,
        Gems
    }

    public Resources() { }

    public Resources(int food, int wood, int stone, int steel, int sulfur, int gems)
    {
        this.food = food;
        this.wood = wood;
        this.stone = stone;
        this.steel = steel;
        this.sulfur = sulfur;
        this.gems = gems;
    }

    public Resources(Resource resource)
    {
        switch (resource)
        {
            case Resource.Food:
                food = 1;
                break;
            case Resource.Wood:
                wood = 1;
                break;
            case Resource.Stone:
                stone = 1;
                break;
            case Resource.Steel:
                steel = 1;
                break;
            case Resource.Sulfur:
                sulfur = 1;
                break;
            case Resource.Gems:
                gems = 1;
                break;
        }
    }

    public int this[Resource r]
    {
        get => this[(int)r];
        set => this[(int)r] = value;
    }

    public int this[int i]
    {
        get
        {
            return i switch
            {
                0 => food,
                1 => wood,
                2 => stone,
                3 => steel,
                4 => sulfur,
                5 => gems,
                _ => 0,
            };
        }
        set
        {
            switch (i)
            {
                case 0:
                    food = value;
                    break;
                case 1:
                    wood = value;
                    break;
                case 2:
                    stone = value;
                    break;
                case 3:
                    steel = value;
                    break;
                case 4:
                    sulfur = value;
                    break;
                case 5:
                    gems = value;
                    break;
                default:
                    break;
            }
        }
    }

    public static Resources operator +(Resources a, Resources b)
    {
        a.food = b.food;
        a.wood += b.wood;
        a.stone += b.stone;
        a.steel += b.steel;
        a.sulfur += b.sulfur;
        a.gems += b.gems;
        return a;
    }

    public static Resources operator -(Resources a, Resources b)
    {
        a.food = -b.food;
        a.wood = -b.wood;
        a.stone = -b.stone;
        a.steel = -b.steel;
        a.sulfur -= b.sulfur;
        a.gems -= b.gems;
        return a;
    }

    public static Resources operator *(Resources a, int b)
    {
        a.food *= b;
        a.wood *= b;
        a.stone *= b;
        a.steel *= b;
        a.sulfur *= b;
        a.gems *= b;
        return a;
    }

    public static bool operator >(Resources a, Resources b)
    {
        if (a.food > b.food && a.wood > b.wood && a.stone > b.stone && a.steel > b.steel && a.sulfur > b.sulfur && a.gems > b.gems)
            return true;
        else
            return false;
    }

    public static bool operator <(Resources a, Resources b)
    {
        if (a.food < b.food && a.wood < b.wood && a.stone < b.stone && a.steel < b.steel && a.sulfur < b.sulfur && a.gems < b.gems)
            return true;
        else
            return false;
    }

    public static bool operator ==(Resources a, Resources b)
    {
        if (a is null || b is null) return false;
        if (a.food == b.food && a.wood == b.wood && a.stone == b.stone && a.steel == b.steel && a.sulfur == b.sulfur && a.gems == b.gems)
            return true;
        else
            return false;
    }

    public static bool operator !=(Resources a, Resources b)
    {
        return !(a == b);
    }

    public static bool operator >=(Resources a, Resources b)
    {
        if (a.food >= b.food && a.wood >= b.wood && a.stone >= b.stone && a.steel >= b.steel && a.sulfur >= b.sulfur && a.gems >= b.gems)
            return true;
        else
            return false;
    }

    public static bool operator <=(Resources a, Resources b)
    {
        if (a.food <= b.food && a.wood <= b.wood && a.stone <= b.stone && a.steel <= b.steel && a.sulfur <= b.sulfur && a.gems <= b.gems)
            return true;
        else
            return false;
    }

    public override bool Equals(object obj)
    {
        return this == (Resources)obj;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return "(" + food + wood + " " + stone + " " + steel + " " + sulfur + gems + ")";
    }
}
