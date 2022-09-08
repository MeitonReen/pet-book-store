namespace BookStore.Base.Implementations.ExtendedConfigurationBinder;

public class CasesSupport
{
    private readonly List<Func<string, string>> _projectorsClassOptionToTargetOption = new();

    public IEnumerable<Func<string, string>> ClassOptionToTargetOptionCaseProjectors =>
        _projectorsClassOptionToTargetOption.ToArray();

    public CasesSupport Add(Func<string, string> projectorClassOptionToTargetOption)
    {
        _projectorsClassOptionToTargetOption.Add(projectorClassOptionToTargetOption);
        return this;
    }
}