using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utf8Json;
using YamlDotNet.Serialization;

namespace GithubLabelSetUpper.Test
{
    [TestClass]
    public class LabelTest
    {
        private const string testJsonFileName = "JsonTest.json";
        private const string testYamlFileName = "YamlTest.json";

        private string readResourceFile(string fileName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{fileName}"))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        [TestMethod]
        public void TestJson()
        {
            string json = readResourceFile(testJsonFileName);
            var labels = JsonSerializer.Deserialize<IList<Label>>(json);

            Assert.AreEqual(2, labels.Count);

            Assert.AreEqual("name1", labels[0].Name);
            Assert.AreEqual("ffffff", labels[0].Color);
            Assert.AreEqual("description", labels[0].Description);
            Assert.AreEqual(2, labels[0].Aliases.Length);
            Assert.AreEqual("name1_1", labels[0].Aliases[0]);
            Assert.AreEqual("name1_2", labels[0].Aliases[1]);

            Assert.AreEqual("name2", labels[1].Name);
            Assert.AreEqual("ffffff", labels[1].Color);
            Assert.AreEqual(null, labels[1].Description);
            Assert.AreEqual(null, labels[1].Aliases);
        }

        [TestMethod]
        public void TestYaml()
        {
            string yaml = readResourceFile(testYamlFileName);
            var labels = new DeserializerBuilder().Build().Deserialize<IList<Label>>(yaml);

            Assert.AreEqual(2, labels.Count);

            Assert.AreEqual("name1", labels[0].Name);
            Assert.AreEqual("ffffff", labels[0].Color);
            Assert.AreEqual("description", labels[0].Description);
            Assert.AreEqual(2, labels[0].Aliases.Length);
            Assert.AreEqual("name1_1", labels[0].Aliases[0]);
            Assert.AreEqual("name1_2", labels[0].Aliases[1]);

            Assert.AreEqual("name2", labels[1].Name);
            Assert.AreEqual("ffffff", labels[1].Color);
            Assert.AreEqual(null, labels[1].Description);
            Assert.AreEqual(null, labels[1].Aliases);
        }
    }
}
