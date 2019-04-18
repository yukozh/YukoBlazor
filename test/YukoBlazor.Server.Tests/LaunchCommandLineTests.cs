using System;
using Xunit;

namespace YukoBlazor.Server.Tests
{
    public class LaunchCommandLineTests
    {
        [Fact]
        public void LaunchFailedWithInvalidArgumentsTest()
        {
            Assert.Throws<IndexOutOfRangeException>(
                () => Program.Main(new[] { "-db" }));

            Assert.Throws<IndexOutOfRangeException>(
                () => Program.Main(new[] { "-url" }));
        }
    }
}
