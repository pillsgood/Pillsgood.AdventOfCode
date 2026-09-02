namespace Pillsgood.AdventOfCode.Common.InputConverters;

public class IdentityConverter : IInputConverter<string, string>
{
    public int GetAffinity(string input, object[]? arguments) => 1;

    public string Convert(string input, object[]? arguments) => input;
}