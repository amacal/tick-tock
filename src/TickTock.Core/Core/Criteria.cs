using System;

namespace TickTock.Core.Core
{
    public class Criteria<T>
    {
        public static readonly Criteria<T> Nothing = new Criteria<T>();

        private readonly T value;
        private readonly bool valued;

        private Criteria()
        {
        }

        private Criteria(T value)
        {
            this.value = value;
            this.valued = true;
        }

        public bool Is(T value)
        {
            return valued == false || Object.Equals(this.value, value);
        }

        public static implicit operator Criteria<T>(T value)
        {
            return new Criteria<T>(value);
        }
    }
}