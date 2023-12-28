using System.Collections.Concurrent;
using System.Reflection;

namespace MyNUnit.Tests
{
    public class Tests
    {
        [Test]
        public void TestOrderOfMethods()
        {
            string path = "../../../../TestProject/release/";
            Type t = Assembly
                .LoadFrom(path + "TestProject.dll")
                .ExportedTypes.Where(t => t.Name == "TestOrder").First();
            var testClass = new TestClass(t);
            testClass.RunTests();
            var listField = t.GetField("ORDER", 
                BindingFlags.Static | 
                BindingFlags.Public);
            var queue = (ConcurrentQueue<string>?)listField!.GetValue(null);
            var list = queue!.ToList();
            var expected = new List<string>
            {
                "BeforeClass",
                "BeforeClass",
                "Before",
                "Before",
                "MyTest",
                "After",
                "After",
                "AfterClass",
                "AfterClass"
            };
            Assert.That(list.SequenceEqual(expected), Is.True);
        }
    }
}