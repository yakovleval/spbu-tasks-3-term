using System.CodeDom.Compiler;
using System.Reflection;


namespace Test3.Tests
{
    public class Tests
    {
        private static bool Compile(FileInfo sourceFile, CompilerParameters options)
        {
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");

            CompilerResults results = provider.CompileAssemblyFromSource(options, sourceFile);

            if (results.Errors.Count > 0)
            {
                return false;
            }
            return true;
        }

        [Test]
        public void Test1()
        {
            string path = "../../../../TestProject/";
            var dInfo = Directory.CreateDirectory(path);
            Reflector.Reflector.PrintStructure(
                path, typeof(DummyClass));
            string fileName = "DummyClassReflected.cs";
            string classPath = path + fileName;
            string outputName = fileName.Substring(0, fileName.Length - 3) + ".dll";
            var sourceFile = new FileInfo(classPath);
            bool success = Compile(sourceFile, new CompilerParameters()
            {
                GenerateExecutable = false,
                OutputAssembly = outputName,
                GenerateInMemory = false,
            });
            Assert.That(success, Is.True);
            var assembly = Assembly.LoadFrom(outputName);
            Assert.That(assembly.ExportedTypes.Count, Is.GreaterThan(0));
            var type = assembly.ExportedTypes.First();
            Assert.That(Reflector.Reflector.DiffClasses(typeof(DummyClass), type),
                Is.EqualTo(""));
        }
    }
}