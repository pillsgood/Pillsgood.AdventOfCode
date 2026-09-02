using System.Globalization;
using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using Pillsgood.AdventOfCode.Common;
using Pillsgood.AdventOfCode.Common.InputConverters;

namespace Pillsgood.AdventOfCode.Tests;

public class InputReaderTests
{
    private ServiceProvider _serviceProvider;

    [OneTimeSetUp]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddInputConverters();
        _serviceProvider = services.BuildServiceProvider();
    }

    [OneTimeTearDown]
    public void Dispose()
    {
        _serviceProvider.Dispose();
    }

    [Test]
    public void TestParseNumber()
    {
        var inputReader = _serviceProvider.GetRequiredService<IPuzzleInputReader>();
        using var stream = new StringReader("123123");
        var ret = inputReader.Read<long>(stream);

        ret.Should().Be(123123L);
    }

    [Test]
    public void TestParseNumberList()
    {
        var inputReader = _serviceProvider.GetRequiredService<IPuzzleInputReader>();
        using var stream = new StringReader("1\n2");
        var ret = inputReader.Read<List<int>>(stream);

        ret.Should().BeEquivalentTo([1, 2]).And.BeOfType<List<int>>();
    }

    [Test]
    public void TestParseNumberArray()
    {
        var inputReader = _serviceProvider.GetRequiredService<IPuzzleInputReader>();
        using var stream = new StringReader("1\n2");
        var ret = inputReader.Read<int[]>(stream);

        ret.Should().BeEquivalentTo([1, 2]).And.BeOfType<int[]>();
    }

    [Test]
    public void TestNumberStyle()
    {
        var inputReader = _serviceProvider.GetRequiredService<IPuzzleInputReader>();
        using var stream = new StringReader("11011011");
        inputReader.Read<int>(stream, [NumberStyles.BinaryNumber]).Should().Be(0b11011011);
    }
}