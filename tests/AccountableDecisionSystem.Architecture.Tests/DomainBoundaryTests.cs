using System.Reflection;
using Xunit;

namespace AccountableDecisionSystem.Architecture.Tests;

public sealed class DomainBoundaryTests
{
    [Fact]
    public void DomainReferencesOnlyTheAdmittedS0Substrate()
    {
        var references = DomainAssembly().GetReferencedAssemblies();
        var external = references
            .Select(reference => reference.Name!)
            .Where(name => !name.StartsWith("System", StringComparison.Ordinal))
            .Where(name => name is not "mscorlib" and not "netstandard")
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();

        Assert.Contains("Dx.Domain.Kernel", external);
        Assert.DoesNotContain(external, name => name.StartsWith("Microsoft.Extensions", StringComparison.Ordinal));
        Assert.DoesNotContain(external, name => name.StartsWith("System.Text.Json", StringComparison.Ordinal));
        Assert.DoesNotContain(external, name => name.StartsWith("AccountableDecisionSystem.", StringComparison.Ordinal));
        Assert.All(external, name => Assert.Contains(name, new[] { "Dx.Domain.Kernel", "Dx.Domain.Annotations" }));
    }

    [Fact]
    public void PublicAndProtectedDomainApisExposeOnlyBclOrDomainTypes()
    {
        var assembly = DomainAssembly();
        var exposed = assembly.ExportedTypes
            .SelectMany(type => type.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
            .SelectMany(MemberTypes)
            .Where(type => type is not null)
            .Select(type => Unwrap(type!))
            .Where(type => type.Assembly != assembly && type.Assembly != typeof(object).Assembly)
            .Select(type => type.Assembly.GetName().Name)
            .Distinct()
            .ToArray();

        Assert.Empty(exposed);
    }

    private static Assembly DomainAssembly()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !Directory.Exists(Path.Combine(directory!.FullName, "src")))
        {
            directory = directory.Parent;
        }

        Assert.NotNull(directory);
        var path = Path.Combine(directory!.FullName, "src", "AccountableDecisionSystem.Domain", "bin", "Release", "net10.0", "AccountableDecisionSystem.Domain.dll");
        Assert.True(File.Exists(path), $"Domain assembly was not found at {path}.");
        return Assembly.LoadFrom(path);
    }

    private static IEnumerable<Type?> MemberTypes(MemberInfo member)
    {
        switch (member)
        {
            case MethodInfo method:
                foreach (var parameter in method.GetParameters())
                {
                    yield return parameter.ParameterType;
                }

                yield return method.ReturnType;
                break;

            case PropertyInfo property:
                yield return property.PropertyType;
                break;

            case FieldInfo field:
                yield return field.FieldType;
                break;

            case EventInfo eventInfo:
                yield return eventInfo.EventHandlerType;
                break;
        }
    }

    private static Type Unwrap(Type type)
    {
        while (type.HasElementType)
        {
            type = type.GetElementType()!;
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition().Assembly == typeof(object).Assembly)
        {
            return typeof(object);
        }

        return type.IsGenericType ? type.GetGenericTypeDefinition() : type;
    }
}
