namespace BookStore.Base.Implementations.Result
{
    public class EnvironmentAccessor : ValueAccessor<string>
    {
        public EnvironmentAccessor(string value) : base(value)
        {
        }
    }
}