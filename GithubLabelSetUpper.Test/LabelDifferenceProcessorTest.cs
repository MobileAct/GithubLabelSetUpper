using System.Collections.Generic;
using Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GithubLabelSetUpper.Test
{
    [TestClass]
    public class LabelDifferenceProcessorTest
    {
        [TestMethod]
        public void TestSame()
        {
            var githubApi = new GithubApi(null, null, "", "");
            var processor = new LabelDifferenceProcessor(githubApi);

            var source = new List<Label>
            {
                new Label
                {
                    Name = "label1",
                    Color = "ffffff"
                },
                new Label
                {
                    Name = "label2",
                    Color = "000000",
                    Description = "text"
                }
            };
            var configured = new List<Label>
            {
                new Label
                {
                    Name = "label1",
                    Color = "ffffff"
                },
                new Label
                {
                    Name = "label2",
                    Color = "000000",
                    Description = "text"
                }
            };

            IReadOnlyList<LabelChangeStrategy> strategies = processor.Process(source, configured);

            Assert.AreEqual(0, strategies.Count);
        }

        [TestMethod]
        public void TestAddition()
        {
            var githubApi = new GithubApi(null, null, "", "");
            var processor = new LabelDifferenceProcessor(githubApi);

            var source = new List<Label>();
            var configured = new List<Label>
            {
                new Label
                {
                    Name = "label1",
                    Color = "ffffff"
                },
                new Label
                {
                    Name = "label2",
                    Color = "000000",
                    Description = "text"
                }
            };

            IReadOnlyList<LabelChangeStrategy> strategies = processor.Process(source, configured);

            Assert.AreEqual(2, strategies.Count);
            Assert.IsInstanceOfType(strategies[0], typeof(LabelChangeStrategy.Add<Label, GithubApi>));
            Assert.IsInstanceOfType(strategies[1], typeof(LabelChangeStrategy.Add<Label, GithubApi>));
        }

        [TestMethod]
        public void TestDeletion()
        {
            var githubApi = new GithubApi(null, null, "", "");
            var processor = new LabelDifferenceProcessor(githubApi);

            var source = new List<Label>
            {
                new Label
                {
                    Name = "label1",
                    Color = "ffffff"
                },
                new Label
                {
                    Name = "label2",
                    Color = "000000",
                    Description = "text"
                }
            };
            var configured = new List<Label>();

            IReadOnlyList<LabelChangeStrategy> strategies = processor.Process(source, configured);

            Assert.AreEqual(2, strategies.Count);
            Assert.IsInstanceOfType(strategies[0], typeof(LabelChangeStrategy.Remove<Label, GithubApi>));
            Assert.IsInstanceOfType(strategies[1], typeof(LabelChangeStrategy.Remove<Label, GithubApi>));
        }

        [TestMethod]
        public void TestUpdate_Name()
        {
            var githubApi = new GithubApi(null, null, "", "");
            var processor = new LabelDifferenceProcessor(githubApi);

            var source = new List<Label>
            {
                new Label
                {
                    Name = "label1",
                    Color = "ffffff"
                }
            };
            var configured = new List<Label>
            {
                new Label
                {
                    Name = "label2",
                    Color = "ffffff",
                    Aliases = new string[]{"label1"}
                }
            };

            IReadOnlyList<LabelChangeStrategy> strategies = processor.Process(source, configured);

            Assert.AreEqual(1, strategies.Count);
            Assert.IsInstanceOfType(strategies[0], typeof(LabelChangeStrategy.Update<Label, GithubApi>));
        }

        [TestMethod]
        public void TestUpdate_Color()
        {
            var githubApi = new GithubApi(null, null, "", "");
            var processor = new LabelDifferenceProcessor(githubApi);

            var source = new List<Label>
            {
                new Label
                {
                    Name = "label1",
                    Color = "ffffff"
                }
            };
            var configured = new List<Label>
            {
                new Label
                {
                    Name = "label1",
                    Color = "ffff00"
                }
            };

            IReadOnlyList<LabelChangeStrategy> strategies = processor.Process(source, configured);

            Assert.AreEqual(1, strategies.Count);
            Assert.IsInstanceOfType(strategies[0], typeof(LabelChangeStrategy.Update<Label, GithubApi>));
        }

        [TestMethod]
        public void TestUpdate_Description()
        {
            var githubApi = new GithubApi(null, null, "", "");
            var processor = new LabelDifferenceProcessor(githubApi);

            var source = new List<Label>
            {
                new Label
                {
                    Name = "label1",
                    Color = "ffffff",
                    Description = "text1"
                }
            };
            var configured = new List<Label>
            {
                new Label
                {
                    Name = "label1",
                    Color = "ffffff",
                    Description = "text2"
                }
            };

            IReadOnlyList<LabelChangeStrategy> strategies = processor.Process(source, configured);

            Assert.AreEqual(1, strategies.Count);
            Assert.IsInstanceOfType(strategies[0], typeof(LabelChangeStrategy.Update<Label, GithubApi>));
        }
    }
}
