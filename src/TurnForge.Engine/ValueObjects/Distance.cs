namespace TurnForge.Engine.ValueObjects;

public readonly record struct Distance(int Um)
{
    public static Distance Zero => new(0);

    public static Distance operator +(
        Distance a, Distance b)
        => new(a.Um + b.Um);

    public bool IsGreaterThan(Distance other)
        => Um > other.Um;
}