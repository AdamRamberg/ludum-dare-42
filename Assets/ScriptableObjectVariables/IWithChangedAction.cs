using System;

public interface IWithChangedAction<T>
{
    Action<T> Changed { get; set; }
}