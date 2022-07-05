using System;

namespace BuildingBlocks.Domain;

public abstract class Entity<TId> 
    where TId : Value<TId>
{

    public TId Id { get; protected set; }

    protected Entity(){}

}