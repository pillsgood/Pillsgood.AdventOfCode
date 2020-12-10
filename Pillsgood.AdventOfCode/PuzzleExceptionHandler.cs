using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Pillsgood.AdventOfCode.Abstractions;
using Pillsgood.AdventOfCode.Core;

namespace Pillsgood.AdventOfCode
{
    internal class PuzzleExceptionHandler
    {
        private class PuzzlePartComparer : IEqualityComparer<(IPuzzleMetadata metadata, int part)>
        {
            public bool Equals((IPuzzleMetadata metadata, int part) x, (IPuzzleMetadata metadata, int part) y) =>
                x.metadata?.Equals(y.metadata) == true && x.part == y.part;

            public int GetHashCode((IPuzzleMetadata metadata, int part) obj) =>
                HashCode.Combine(obj.metadata?.Year, obj.metadata?.Day, obj.part);
        }

        private readonly Dictionary<(IPuzzleMetadata, int), Action> _logExceptions =
            new Dictionary<(IPuzzleMetadata, int), Action>(new PuzzlePartComparer());


        private readonly IAocConsole _console;

        public PuzzleExceptionHandler(PuzzleRunner runner, IAocConsole console = null)
        {
            _console = console;
            runner.PuzzleExceptionThrown += (key, exception) =>
                _logExceptions.Add(key, () => OnPuzzleExceptionThrown(key, exception));
        }

        public void For(IPuzzleMetadata metadata, int part)
        {
            if (_logExceptions.ContainsKey((metadata, part)))
            {
                _logExceptions[(metadata, part)]?.Invoke();
            }
        }

        private void OnPuzzleExceptionThrown(object key, Exception exception)
        {
            switch (exception)
            {
                case PuzzleNullOrEmptyException _:
                    _console?.WriteAnswerIsNull();
                    break;
                case TargetInvocationException e:
                    if (e.InnerException != null)
                    {
                        switch (e.InnerException)
                        {
                            case NotImplementedException _:
                                _console?.WriteAnswerNotImplemented();
                                break;
                            case WebException webException:
                                if (_console != null)
                                {
                                    _console.WriteNoSessionId();
                                    _console.WriteException(webException);
                                }
                                else
                                {
                                    throw e.InnerException;
                                }

                                Environment.Exit(1);
                                break;
                        }

                        break;
                    }

                    _console?.WriteException(e);
                    break;
            }

            _logExceptions.Remove(((IPuzzleMetadata, int)) key);
        }
    }
}