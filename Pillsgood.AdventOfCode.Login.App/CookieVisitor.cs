using System.Reactive.Subjects;
using Xilium.CefGlue;

namespace Pillsgood.AdventOfCode.Login.App;

internal class CookieVisitor(Subject<string> subject) : CefCookieVisitor
{
    protected override bool Visit(CefCookie cookie, int count, int total, out bool delete)
    {
        if (cookie is { Domain: ".adventofcode.com", Name: "session" })
        {
            subject.OnNext(cookie.Value);
        }


        delete = false;
        return true;
    }
}