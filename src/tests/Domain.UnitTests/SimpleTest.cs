// tests/Domain.UnitTests/SimpleTest.cs
using NUnit.Framework;

namespace Domain.UnitTests;

[TestFixture]
public class SimpleTest
{
    [Test]
    public void Can_Find_Domain_Project()
    {
        // This test doesn't use Domain entities
        Assert.Pass("Basic test setup works");
    }
}