using System;

#nullable enable
namespace Pillsgood.AdventOfCode.Abstractions
{
    public interface IAocConsole
    {
        void PrintSeparator();
        void StartUpMessage();
        void WriteYear(int year);
        void WritePart(int part);
        void WriteDay(int day);
        void WriteDay(string title);
        void WriteAnswer(string answer);
        void WriteAnswerNotImplemented();
        void WriteAnswerIsNull();
        void WriteException(Exception e);
        void WriteNoSessionId();
    }
}