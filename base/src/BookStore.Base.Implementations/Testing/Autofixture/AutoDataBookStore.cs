using AutoFixture;
using AutoFixture.Xunit2;

namespace BookStore.Base.Implementations.Testing.Autofixture;

public class AutoDataBookStore : AutoDataAttribute
{
    public AutoDataBookStore()
        : base(() =>
        {
            var fixture = new Fixture();
            fixture.Customize<DateOnly>(composer => composer
                .FromFactory<DateTime>(DateOnly.FromDateTime));

            return fixture;
        })
    {
    }
}