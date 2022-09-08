namespace BookStore.Base.Abstractions.MinimalDnf;

public interface IMinimalDnfGenerator
{
    Task<string> Generate(string logicalExpression);
}