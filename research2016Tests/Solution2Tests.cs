using Xunit;

namespace research2016Tests
{
	public class Solution2Tests
	{
		[Fact]
		public void Test()
		{
			var a = LittleAssembler.Translator.Translate((x, y, z) => (x*y + z)/x + 1);
		} 
		[Fact]
		public void TestA()
		{
			var a = LittleAssembler.Translator.Translate((x, y) => x*y);
		} 
		[Fact]
		public void TestB()
		{
			var a = LittleAssembler.Translator.Translate((x, y) => x*y);
		} 
	}
}