using System;
using System.Reactive.Subjects;
using Xilium.CefGlue;

namespace Pillsgood.AdventOfCode.Login.ViewModels;

internal class CookieVisitor : CefCookieVisitor
{
    private static readonly Subject<CefCookie> Subject = new();

    protected override bool Visit(CefCookie cookie, int count, int total, out bool delete)
    {
        if (cookie.Domain != ".adventofcode.com")
        {
            delete = false;
            return true;
        }

        if (cookie.Name == "session")
        {
            Subject.OnNext(cookie);
        }

        delete = true;
        return true;
    }

    public static IObservable<CefCookie> SessionCookieVisited => Subject;
}