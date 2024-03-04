using System.Diagnostics;
using ClientCommunication.Utility;

namespace Tests;

public class Tests
{
    [Test]
    public void TestMemoryLayouts()
    {
        Assert.DoesNotThrow(TestHelper.AssertMemoryLayoutStructSize);
    }
}