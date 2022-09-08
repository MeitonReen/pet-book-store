using System.Text.RegularExpressions;
using BookStore.Base.Abstractions.MinimalDnf;
using BookStore.Base.Implementations.MinimalDnf.Quine_McCluskey;
using DynamicExpresso;

namespace BookStore.Base.Implementations.MinimalDnf;

public class AbbreviatedMinimalDnfGenerator : IMinimalDnfGenerator
{
    private const string AndServiceMarker = "&&";
    private const string OrServiceMarker = "||";

    private const string AndProxyPdnfExpressionMarker = "*";
    private const string OrProxyPdnfExpressionMarker = "+";

    private readonly (string From, string To)[]
        _proxyPdnfExpressionToRequestedLogicalMarkers;

    //@"[a-z0-9\-]+\.[crud]{1,4}"
    private readonly string _regexValueSelector;

    private readonly (string From, string To)[]
        _requestedToServiceLogicalMarkers;

    private readonly (string From, string To)[]
        _serviceToProxyPdnfExpressionLogicalMarkers =
        {
            (AndServiceMarker, AndProxyPdnfExpressionMarker),
            (OrServiceMarker, OrProxyPdnfExpressionMarker)
        };

    private readonly Dictionary<string, string> _valueVariableDict = new();


    private readonly IEnumerator<string> _variableNamesEnumerator = Enumerable
        .Range(97, 26)
        .Select(el => Convert.ToChar(el).ToString())
        .GetEnumerator();

    public AbbreviatedMinimalDnfGenerator(string andMarker, string orMarker,
        string regexValueSelector)
    {
        _requestedToServiceLogicalMarkers = new[]
        {
            (andMarker, AndServiceMarker),
            (orMarker, OrServiceMarker)
        };
        _proxyPdnfExpressionToRequestedLogicalMarkers = new[]
        {
            (AndProxyPdnfExpressionMarker, andMarker),
            (OrProxyPdnfExpressionMarker, orMarker)
        };

        _regexValueSelector = regexValueSelector;
    }

    public async Task<string> Generate(string logicalExpression)
    {
        if (string.IsNullOrEmpty(logicalExpression))
        {
            throw new ArgumentException(default, nameof(logicalExpression));
        }

        var preparedLogicalExpression = PrepareLogicalExpression(logicalExpression);

        var binaryTableDependVariables = BinaryTableDependVariablesGenerate();

        var logicalFunctionAsLambda =
            new Interpreter().Parse(preparedLogicalExpression,
                _valueVariableDict.Values.Select(variableName =>
                    new Parameter(variableName, typeof(bool))).ToArray());

        var perfectDnf =
            PdnfGenerate(binaryTableDependVariables, logicalFunctionAsLambda);

        var minimalDnf = (await new PdnfExpression(LogicalMarkersMap(perfectDnf,
                    _serviceToProxyPdnfExpressionLogicalMarkers))
                .Simplify())
            .ToString();

        minimalDnf = LoadValuesToVariables(minimalDnf);

        minimalDnf = LogicalMarkersMap(minimalDnf,
            _proxyPdnfExpressionToRequestedLogicalMarkers);

        return minimalDnf;
    }

    private string PrepareLogicalExpression(string logicalExpression)
    {
        var preparedLogicalExpression = LogicalMarkersMap(logicalExpression,
            _requestedToServiceLogicalMarkers);
        preparedLogicalExpression = SaveValuesToVariables(preparedLogicalExpression);
        return preparedLogicalExpression;
    }

    private static string LogicalMarkersMap(string logicalExpression,
        IEnumerable<(string From, string To)> map) =>
        map.Aggregate(logicalExpression, (logicalExpressionInner, el) =>
        {
            var (from, to) = el;
            logicalExpressionInner = logicalExpressionInner.Replace(from, to);
            return logicalExpressionInner;
        });

    private string SaveValuesToVariables(string logicalExpression)
    {
        return Regex.Replace(logicalExpression, _regexValueSelector, match =>
            {
                if (_valueVariableDict.TryGetValue(match.Value, out var variable))
                {
                    return variable;
                }

                _variableNamesEnumerator.MoveNext();
                _valueVariableDict.Add(match.Value, _variableNamesEnumerator.Current);
                return _variableNamesEnumerator.Current;
            }
        );
    }

    private IEnumerable<IEnumerable<(bool BinaryValue, string VariableName)>>
        BinaryTableDependVariablesGenerate() => Enumerable
        .Range(0, (int) Math.Pow(2, _valueVariableDict.Count))
        .Select(el =>
        {
            var elAsBinarySs = Convert.ToString(el, 2);
            return Enumerable
                .Repeat('0', _valueVariableDict.Count - elAsBinarySs.Length)
                .Concat(elAsBinarySs)
                .Select((binaryEl, i) => (
                    BinaryValue: binaryEl == '1',
                    VariableName: _valueVariableDict.Values.ElementAt(i)));
        });

    private string PdnfGenerate(
        IEnumerable<IEnumerable<(bool BinaryValue, string VariableName)>>
            binaryTableDependVariables,
        Lambda logicalFunction) => binaryTableDependVariables
        .Select(binaryDependVariablesLine =>
        {
            var (parameters, preparedTermComponents) = binaryDependVariablesLine
                .Aggregate((Parameters: new List<Parameter>(),
                        PreparedTermComponents: new List<string>()),
                    (aggregator, binaryEl) =>
                    {
                        aggregator.Parameters.Add(
                            new Parameter(binaryEl.VariableName,
                                binaryEl.BinaryValue));
                        aggregator.PreparedTermComponents.Add(
                            binaryEl.BinaryValue
                                ? binaryEl.VariableName
                                : $"!{binaryEl.VariableName}");

                        return aggregator;
                    });

            var targetLogicalFunctionValueIsTrue =
                (bool) logicalFunction.Invoke(parameters);

            if (!targetLogicalFunctionValueIsTrue) return "";

            return preparedTermComponents
                .Aggregate((leftTermComponent, rightTermComponent) =>
                    $"{leftTermComponent}*{rightTermComponent}");
        })
        .Where(term => !string.IsNullOrEmpty(term))
        .Aggregate((leftTerm, rightTerm) => $"{leftTerm}+{rightTerm}");

    private string LoadValuesToVariables(string logicalExpression) =>
        _valueVariableDict
            .Aggregate(logicalExpression, (logicalExpressionInner, el) =>
            {
                var (value, variable) = el;

                logicalExpressionInner = Regex.Replace(
                    logicalExpressionInner, $@"(^|\s){variable}(\s|$)", value);
                ;
                return logicalExpressionInner;
            });
}