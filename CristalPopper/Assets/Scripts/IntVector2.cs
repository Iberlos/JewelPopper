
using System;
using UnityEngine.Internal;

public struct IntVector2
{
    public int x;
    public int y;

    public IntVector2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    //Methods
    public int this[int index]
    {
        get
        {
            if (index == 0)
            {
                return this.x;
            }
            if (index != 1)
            {
                throw new IndexOutOfRangeException("Invalid IntVector2 index!");
            }
            return this.y;
        }
        set
        {
            if (index != 0)
            {
                if (index != 1)
                {
                    throw new IndexOutOfRangeException("Invalid IntVector2 index!");
                }
                this.y = value;
            }
            else
            {
                this.x = value;
            }
        }
    }

    public override bool Equals(object other)
    {
        if (!(other is IntVector2))
        {
            return false;
        }
        IntVector2 vector = (IntVector2)other;
        return this.x.Equals(vector.x) && this.y.Equals(vector.y);
    }

    public override int GetHashCode()
    {
        return this.x.GetHashCode() ^ this.y.GetHashCode() << 2;
    }

    public void Scale(IntVector2 scale)
    {
        this.x *= scale.x;
        this.y *= scale.y;
    }

    public void Set(int new_x, int new_y)
    {
        this.x = new_x;
        this.y = new_y;
    }

    public float SqrMagnitude()
    {
        return this.x * this.x + this.y * this.y;
    }

    public override string ToString()
    {
        return string.Format("({0:F1}, {1:F1})", new object[]
                                    {
            this.x,
            this.y
        });
    }

    public string ToString(string format)
    {
        return string.Format("({0}, {1})", new object[]
                                    {
            this.x.ToString (format),
            this.y.ToString (format)
        });
    }

    // Operators
    public static IntVector2 operator +(IntVector2 a, IntVector2 b)
    {
        return new IntVector2(a.x + b.x, a.y + b.y);
    }

    public static IntVector2 operator /(IntVector2 a, int d)
    {
        return new IntVector2(a.x / d, a.y / d);
    }

    public static bool operator ==(IntVector2 lhs, IntVector2 rhs)
    {
        return lhs.x == rhs.x && lhs.y == rhs.y;
    }

    public static bool operator !=(IntVector2 lhs, IntVector2 rhs)
    {
        return lhs.x != rhs.x || lhs.y != rhs.y;
    }

    public static IntVector2 operator *(IntVector2 a, int d)
    {
        return new IntVector2(a.x * d, a.y * d);
    }

    public static IntVector2 operator *(int d, IntVector2 a)
    {
        return new IntVector2(a.x * d, a.y * d);
    }

    public static IntVector2 operator -(IntVector2 a, IntVector2 b)
    {
        return new IntVector2(a.x - b.x, a.y - b.y);
    }

    public static IntVector2 operator -(IntVector2 a)
    {
        return new IntVector2(-a.x, -a.y);
    }
}

