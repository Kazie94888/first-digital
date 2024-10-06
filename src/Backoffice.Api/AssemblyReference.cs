using System.Reflection;

namespace SmartCoinOS.Backoffice.Api;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}