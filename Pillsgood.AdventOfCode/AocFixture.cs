using Pillsgood.AdventOfCode.Common;
using Splat;

namespace Pillsgood.AdventOfCode;

[AocFixture]
public abstract class AocFixture
{
    private readonly Lazy<IPuzzleInputService> _inputService =
        new(static () => Locator.Current.GetRequiredService<IPuzzleInputService>());

    private readonly Lazy<IAnswerAssertion> _assertion =
        new(static () => Locator.Current.GetRequiredService<IAnswerAssertion>());

    protected IPuzzleInputService Input => _inputService.Value;

    protected IAnswerAssertion Answer => _assertion.Value;
}