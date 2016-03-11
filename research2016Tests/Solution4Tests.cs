using System;
using Solution4;
using Xunit;
using Program = Solution4.Program;

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

		public const string TextFileTest2 = 
@"Foo foo bar foobar
Bazz foo bar eqe qee aa sddd
foobar bar bar br foo foo asd dd e";

		public const string QueriesFileTest2 =
@"bar
foobar
XYZ
foo
baz
FOO foobar FOO bar";

		public const string ExpectedResultFile2 =
@"0,1,2
0,2

0,1,2

0,1,2";

		public const string TextFileTest3 =
@"Foo foo bar foobar
Bazz foo bar eqe qee aa sddd
foobar bar bar br foo foo asd dd e
bar
bar
bar
bar
bar
bar
bar
bar
bar
bar
bar
bar
bar
bar
bar
bar
hoo
bar faz";

		public const string QueriesFileTest3 =
@"bar
foobar
XYZ
foo
baz
FOO foobar FOO bar";
		
		public const string ExpectedResultFile3 =
@"0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,20
0,2

0,1,2

0,1,2";


		public const string TextFileTest4 =
@"Foo foo bar foobar asd qq
Bazz foo bar eqe qee aa sddd qq as
foobar bar bar br foo foo asd dd e qq";

		public const string QueriesFileTest4 =
@"bar
foobar
XYZ
foo
baz
asd bar";

		public const string ExpectedResultFile4 =
@"0,1,2
0,2

0,1,2

0,2";

		[Fact]
		public void TestWorksCorrect()
		{
			var textFileLines = TextFileTest.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			var queries = QueriesFileTest.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

			var queryResult = new TextSearch(textFileLines, queries).Execute();
			var result = Program.BuildResultString(queryResult);

			Assert.Equal(ExpectedResultFile, result);
		}


		[Fact]
		public void MultilineQueryTest()
		{
			var textFileLines = TextFileTest2.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			var queries = QueriesFileTest2.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

			var queryResult = new TextSearch(textFileLines, queries).Execute();
			var result = Program.BuildResultString(queryResult);

			Assert.Equal(ExpectedResultFile2, result);
		}


		[Fact]
		public void MultilineQueryTest_Top20()
		{
			var textFileLines = TextFileTest3.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			var queries = QueriesFileTest3.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

			var queryResult = new TextSearch(textFileLines, queries).Execute();
			var result = Program.BuildResultString(queryResult);

			Assert.Equal(ExpectedResultFile3, result);
		}

		[Fact]
		public void MultilineSearch()
		{
			var textFileLines = TextFileTest4.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			var queries = QueriesFileTest4.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

			var queryResult = new TextSearch(textFileLines, queries).Execute();
			var result = Program.BuildResultString(queryResult);

			Assert.Equal(ExpectedResultFile4, result);
		}
	}
}