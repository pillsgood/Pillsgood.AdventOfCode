using Pillsgood.AdventOfCode.Common;

namespace Pillsgood.AdventOfCode;

[AocFixture]
public abstract class AocFixture
{
    protected static IAnswerAssertion Answer => field ??= Locator.GetRequiredService<IAnswerAssertion>();

    protected static IPuzzleInputService Input => field ??= Locator.GetRequiredService<IPuzzleInputService>();
}