using System.Runtime.CompilerServices;

public static class ThrowHelper
{
    public static Exception InjectIsNull([CallerMemberName] string callerName = "")
    {
        return new InvalidOperationException($"Injected property '{callerName}' doesn't initialized.");
    }

    public static Exception CascadingParameterIsNull([CallerMemberName] string callerName = "")
    {
        return new InvalidOperationException($"Cascading property '{callerName}' doesn't initialized.");
    }

    public static Exception ParameterIsNull([CallerMemberName] string callerName = "")
    {
        return new InvalidOperationException($"Input parameter property '{callerName}' doesn't initialized.");
    }

    public static Exception ComponentReferenceIsNull([CallerMemberName] string callerName = "")
    {
        return new InvalidOperationException($"Component reference property '{callerName}' doesn't initialized.");
    }

    public static Exception PropertyIsNull([CallerMemberName] string callerName = "")
    {
        return new InvalidOperationException($"Property '{callerName}' doesn't initialized.");
    }
}