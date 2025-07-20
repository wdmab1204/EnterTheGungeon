using System;

namespace GameEngine.Observable
{
    public class ReactiveProperty<T>
    {
        private T _value;

        public T Value
        {
            get => _value;
            set
            {
                if (!Equals(_value, value))
                {
                    _value = value;
                    OnValueChanged?.Invoke(_value);
                }
            }
        }

        public event Action<T> OnValueChanged;

        public ReactiveProperty(T initialValue = default)
        {
            _value = initialValue;
        }

        public void ForceNotify()
        {
            OnValueChanged?.Invoke(_value);
        }
    }
}
