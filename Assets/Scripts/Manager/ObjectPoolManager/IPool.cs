using System;

public interface IPool
{
    Type ObjectType { get; }
    int Count { get; }
    void Release();
}