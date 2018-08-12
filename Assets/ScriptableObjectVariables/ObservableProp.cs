using System;
using System.Collections.Generic;
using UnityEngine;

public class ObservableProp<T> : IWithChangedAction<T>, IWithValue<T>
{
    [SerializeField]
    private T value;

    readonly IEqualityComparer<T> equalityComparer = EqualityComparer<T>.Default;

    private Action<T> changed;
    public Action<T> Changed
    {
        get { return changed; }
        set
        {
            changed = value;
            if (value != null)
            {
                value(this.value);
            }
        }
    }

    public T Value
    {
        get
        {
            return value;
        }
        set
        {
            if (!equalityComparer.Equals(value, this.value))
            {
                this.value = value;
                if (changed != null) { changed(value); }
            }
        }
    }

    public ObservableProp(T value)
    {
        this.value = value;
    }
}

[Serializable]
public class ObservableIntProp : ObservableProp<int> { public ObservableIntProp(int value) : base(value) { } }
[Serializable]
public class ObservableFloatProp : ObservableProp<float> { public ObservableFloatProp(float value) : base(value) { } }
[Serializable]
public class ObservableBoolProp : ObservableProp<bool> { public ObservableBoolProp(bool value) : base(value) { } }
[Serializable]
public class ObservableVector2Prop : ObservableProp<Vector2> { public ObservableVector2Prop(Vector2 value) : base(value) { } }
[Serializable]
public class ObservableVector3Prop : ObservableProp<Vector3> { public ObservableVector3Prop(Vector3 value) : base(value) { } }