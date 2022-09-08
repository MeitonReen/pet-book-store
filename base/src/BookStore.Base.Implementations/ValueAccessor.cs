namespace BookStore.Base.Implementations
{
    public class ValueAccessor<T>
    {
        public ValueAccessor(T value)
        {
            Value = value;
        }

        public T Value { get; set; }
    }
}