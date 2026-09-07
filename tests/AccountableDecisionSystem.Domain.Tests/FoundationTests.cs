using Xunit;

namespace AccountableDecisionSystem.Domain.Tests;

public sealed class FoundationTests
{
    [Fact]
    public void TargetFrameworkIsNet10()
    {
        Assert.Contains(".NETCoreApp,Version=v10.0", AppContext.TargetFrameworkName, StringComparison.Ordinal);
    }
}
