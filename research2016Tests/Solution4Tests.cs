using System;
using System.Linq;
using Solution3;
using Solution4;
using Xunit;

namespace research2016Tests
{
	public class Solution4Tests
	{
		public const string TextFileTest = 
@"Foo foo bar
Bazz foo bar
foobar";

		public const string QueriesFileTest =
@"bar
foobar
XYZ
foo
baz
FOO FOO FOO FOO";

		public const string ExpectedResultFile =
@"0,1
2

0,1

0,1";

		[Fact]
		public void TestWorksCorrect()
		{
			var textFileLines = TextFileTest.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			var queries = QueriesFileTest.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

			var result = Engine.QueryText(textFileLines, queries);

			Assert.Equal(ExpectedResultFile, result);
		}
	}
}