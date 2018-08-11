using System;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjectVariables
{
    public class ScriptableObjectVariable<T> : ScriptableObject
    {
        [Multiline]
        public string DeveloperDescription = "";

        public T Value { get { return value; } set { SetValue(value); } }

        [SerializeField]
        private T value;
        public bool useAsConstant = true; // If true it is only possible to change the variable via the Inspector

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

        public bool SetValue(T value)
        {
            if (useAsConstant) return false;

            if (!equalityComparer.Equals(value, this.value))
            {
                this.value = value;
                if (Changed != null) { Changed(value); }
            }

            return true;
        }

        public bool SetValue(ScriptableObjectVariable<T> variable)
        {
            return SetValue(variable.Value);
        }

        public virtual bool ApplyChange(T amount)
        {
            if (useAsConstant) return false;
            return true;
        }
        public virtual bool ApplyChange(ScriptableObjectVariable<T> amount)
        {
            if (useAsConstant) return false;
            return true;
        }
    }
}