using System.Collections.Generic;
using System.Linq;
using Xunit;

using PrinceXML.Wrapper.Util;

namespace PrinceXML.Wrapper.Tests
{
    public class CommandLineTests
    {
        [Theory]
        [InlineData("--test", "test")]
        [InlineData("--test=x", "test", "x")]
        [InlineData("--test=1", "test", "1")]
        public void ToCommand(string expected, string key, object? value = null)
        {
            string actual = CommandLine.ToCommand(key, value);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ToCommandCsvs()
        {
            List<int> values = new List<int>() { 1, 2, 3 };
            string actual = CommandLine.ToCommandCsvs("test", values);
            Assert.Equal("--test=1,2,3", actual);
        }

        [Fact]
        public void ToCommands()
        {
            List<int> values = new List<int>() { 0, 1, 2 };
            List<string> expected = new List<string>() {
                "--test=1",
                "--test=2",
                "--test=3",
            };
            List<string> actual = CommandLine.ToCommands("test", values.Select(x => x + 1));
            Assert.Equal(expected, actual);
        }
    }

    public class JsonTests
    {
        [Theory]
        [InlineData("{}")]
        [InlineData("\"test\":{}", "test")]
        public void BeginObj(string expected, string? name = null)
        {
            Json j = new Json().BeginObj(name).EndObj();
            Assert.Equal(expected, j.ToString());
        }

        [Fact]
        public void BeginList()
        {
            Json j = new Json().BeginList("test").EndList();
            Assert.Equal("\"test\":[]", j.ToString());
        }

        [Fact]
        public void FieldString()
        {
            Json j = new Json().Field("test", "string");
            Assert.Equal("\"test\":\"string\"", j.ToString());
        }

        [Fact]
        public void FieldInt()
        {
            Json j = new Json().Field("test", 123);
            Assert.Equal("\"test\":123", j.ToString());
        }

        [Fact]
        public void FieldBool()
        {
            Json j = new Json()
                .Field("test1", true)
                .Field("test2", false);
            Assert.Equal("\"test1\":true,\"test2\":false", j.ToString());
        }

        [Fact]
        public void QuoteAndEscape()
        {
            Json j = new Json()
                .BeginObj("ab\\cd\"ef\"gh\\ij")
                .EndObj();
            Assert.Equal("\"ab\\\\cd\\\"ef\\\"gh\\\\ij\":{}", j.ToString());
        }
    }

    public class ArgumentsTests
    {
        // https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.processstartinfo.argumentlist?view=net-5.0#examples
        [Fact]
        public void BuildArguments()
        {
            List<string> args = new List<string>()
            {
                "/c",
                "dir",
                @"C:\Program Files\dotnet"
            };
            Assert.Equal("/c dir \"C:\\Program Files\\dotnet\"",
                Arguments.BuildArguments(args));
        }
    }
}
