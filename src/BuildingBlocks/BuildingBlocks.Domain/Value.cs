namespace BuildingBlocks.Domain;

public abstract record Value<T> where T : Value<T>
{
}