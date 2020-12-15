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
        private readonly Dictionary<IPuzzleResult, Action> _logExceptions =
            new Dictionary<IPuzzleResult, Action>();


        private readonly IAocConsole _console;

        public PuzzleExceptionHandler(PuzzleRunner runner, IAocConsole console = null)
        {
            _console = console;
            runner.PuzzleExceptionThrown += (key, exception) =>
                _logExceptions.Add(key, () => OnPuzzleExceptionThrown(key, exception));
        }

        public void For(IPuzzleResult result)
        {
            if (_logExceptions.ContainsKey(result))
            {
                _logExceptions[result]?.Invoke();
            }
        }

        private void OnPuzzleExceptionThrown(IPuzzleResult key, Exception exception)
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
                            default:
                                throw e;
                        }
                    }

                    break;
                default:
                    throw exception;
            }

            _logExceptions.Remove(key);
        }
    }
}