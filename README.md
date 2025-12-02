### Pillsgood.AdventOfCode

Helpers for solving Advent of Code puzzles from unit tests. It handles:

- Fetching your puzzle input
- Converting input into convenient types
- Submitting/validating your answers

Built for use with NUnit but probably works with other test frameworks too.

### Quick start (NUnit)

1) Add NuGet package `Pillsgood.AdventOfCode` to project.
   - Optionally, add `Pillsgood.AdventOfCode.Login` for interactive login.
2) Add a one-time startup/teardown to your test assembly to initialize the library.

    ```csharp
    using NUnit.Framework;
    using Pillsgood.AdventOfCode;
    using Pillsgood.AdventOfCode.Login; // only if you want the login UI
    
    [SetUpFixture]
    public sealed class AdventSetup
    {
        private IDisposable? _app;
    
        [OneTimeSetUp]
        public void Start()
        {
            _app = Aoc.Start(cfg =>
            {
                // Choose ONE of the following:
    
                // 1) Provide a session cookie directly:
                // cfg.WithSession("your_session_cookie_here");
    
                // 2) Or use the interactive login window (from Pillsgood.AdventOfCode.Login):
                // cfg.WithLogin();
    
                // Optional: only needed if your tests run from a helper assembly (probs works)
                // cfg.SetEntryAssembly(typeof(AdventSetup).Assembly);
                
                // Optional: you can add additional puzzle input converters here 
                // cfg.AddInputConverter<SampleType>(() => new SampleTypeInputConverter())
            });
        }
    
        [OneTimeTearDown]
        public void Stop()
        {
            _app?.Dispose();
        }
    }
    ```

3) Create test classes that inherit from `AocFixture`, and follow the naming conventions below. Use `Input` to read puzzle input and `Answer` to
   submit/validate.

    ```csharp
    using NUnit.Framework;
    using Pillsgood.AdventOfCode;
    
    // Class name (or namespace) must include the year and day (see Naming Conventions)
    namespace Y2024
    {
        public class Day01Tests : AocFixture
        {
            [Test]
            public void Part1()
            {
                var lines = Input.Get<IEnumerable<string>>();
    
                var result = SolvePart1(lines);
    
                // Validates against your previously accepted answer (or submits if new)
                Answer.Submit(result);
            }
    
            [Test]
            public void Part2()
            {
                var numbers = Input.Get<int>(System.Globalization.NumberStyles.Integer);
    
                var result = SolvePart2(numbers);
    
                Answer.Submit(result);
            }
    
            private static int SolvePart1(IEnumerable<string> lines)
            {
                // your solution here
                return lines.Count();
            }
    
            private static long SolvePart2(IEnumerable<int> nums)
            {
                // your solution here
                return nums.Select(n => (long)n).Sum();
            }
        }
    }
    ```

---

### Naming conventions (how the library finds date and part)

The library automatically detects which puzzle you’re solving by inspecting the call stack:

- Year: any `20xx` present in the test class' full name (namespace or type name)
- Day: `DayNN` (e.g., `Day1`, `Day01`, `Day25`) present in the test class' full name
- Part: inferred from the test method name `Part1` or `Part2`

Examples that work:

- Namespace `AoC._2023.Day05` and class `Day05Tests`
- Namespace `AdventOfCode.Y2022` and class `Day1_PartTests`

If detection fails, you’ll get an error like “Unable to resolve date/part.” Ensure your class and method names contain the patterns above.

---

### Reading input

`AocFixture` exposes `Input` (an `IPuzzleInputService`) with helpers to parse your input:

```csharp
// Reads raw text as string
var text = Input.Get<string>();

// Reads input as IEnumerable<string> (split by new lines)
var lines = Input.Get<IEnumerable<string>>();

// Read numbers with explicit number style and optional format provider
var ints = Input.Get<int>(System.Globalization.NumberStyles.Integer);
var longs = await Input.GetAsync<long>(System.Globalization.NumberStyles.Integer);

// Asynchronous versions exist for all operations: GetAsync<T>()
```
You can also access the raw stream:

```csharp
await using var stream = await Input.GetInputStreamAsync(new DateOnly(2024, 12, 1));
```

---

### Submitting/validating answers

Use `Answer` from `AocFixture` and call `Submit(...)` or `SubmitAsync(...)` inside your test method. The library:

- Figures out the Day and Part from the method name and type
- Submits the answer to AoC (if not known) and/or asserts against the stored, accepted answer

```csharp
// Numbers:
Answer.Submit(12345);                 // infers Part from method name
await Answer.SubmitAsync(12345L);

// Strings:
Answer.Submit("ABCDEF");
await Answer.SubmitAsync("ABCDEF");

// Defer computation:
Answer.Submit(() => Compute());
await Answer.SubmitAsync(async () => await ComputeAsync());
```

If the answer was already accepted previously, the call will assert equality against the known-good answer instead of re-submitting.

---

### Session setup options

You must authenticate once so the library can fetch inputs and submit answers.

Pick one in `Aoc.Start(cfg => ...)`:

- `cfg.WithSession("<session_cookie>")` — pass your AoC session cookie directly (great for CI/local). The cookie value is the `session` cookie from
  `adventofcode.com`.
- `cfg.WithLogin()` (from `Pillsgood.AdventOfCode.Login`) — shows a low-effort Avalonia login window to acquire and cache the session.

The session is cached, if an invalid/expired session is detected, it will be cleared and you’ll be prompted
again (or you’ll need to provide a valid cookie).

---

### Configuration reference

`Aoc.Start(cfg => { ... })` accepts the following options:

```csharp
var app = Aoc.Start(cfg =>
{
    // 1) Provide a session cookie directly (recommended for CI/local)
    cfg.WithSession("your_session_cookie_here");

    // 2) Or provide a custom session provider
    // cfg.WithSessionProvider(() => new MySessionProvider());

    // Tell the library which assembly contains your tests (optional)
    // cfg.SetEntryAssembly(typeof(SomeTypeInYourTests).Assembly);

    // Register custom input converters (optional)
    // cfg.AddInputConverter<MyType>(() => new MyTypeInputConverter());

    // Set the cache file path (defaults to "store.db")
    // cfg.WithCachePath(".aoc-cache.db");
});
```

Dispose the returned `IDisposable` at the end of the test run to cleanly release services.

---

### Create a custom base class for your tests

If you prefer not to inherit from `AocFixture`, you can create your own base class. Mark it with `AocFixtureAttribute` so the library can resolve the date/part from your call stack.

```csharp
using NUnit.Framework;
using Pillsgood.AdventOfCode;
using Pillsgood.AdventOfCode.Common;

[AocFixture]
public abstract class MyAocBase
{
    protected IPuzzleInputService Input => Locator.GetRequiredService<IPuzzleInputService>();
    protected IAnswerAssertion     Answer => Locator.GetRequiredService<IAnswerAssertion>();
}

public sealed class Day01Tests : MyAocBase
{
    [Test]
    public void Part1()
    {
        var lines = Input.Get<IEnumerable<string>>();
        var result = SolvePart1(lines);
        Answer.Submit(result);
    }
}
```

Notes:

- You still need the global startup/teardown via `Aoc.Start(...)` to initialize DI and caching.
- `AocFixtureAttribute` on your base class (or on the test class) ensures that year/day and part can be resolved by the library.

---

### License

See `LICENSE.md`.
