using System.Collections.Generic;
using Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GiteaLabelSetUpper.Test
{
    [TestClass]
    public class LabelDifferenceProcessorTest
    {
        [TestMethod]
        public void TestSame()
        {
            var giteaApi = new GiteaApi(null, null, "", "");
            var processor = new LabelDifferenceProcessor(giteaApi);

            var source = new List<Label>
            {
                new Label
                {
                    Name = "label1",
                    Color = "ffffff"
                },
                new Label
                {
                    Id = 123,
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
            var giteaApi = new GiteaApi(null, null, "", "");
            var processor = new LabelDifferenceProcessor(giteaApi);

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
                    Id = 123,
                    Name = "label2",
                    Color = "000000",
                    Description = "text"
                }
            };

            IReadOnlyList<LabelChangeStrategy> strategies = processor.Process(source, configured);

            Assert.AreEqual(2, strategies.Count);
            Assert.IsInstanceOfType(strategies[0], typeof(LabelChangeStrategy.Add<Label, GiteaApi>));
            Assert.IsInstanceOfType(strategies[1], typeof(LabelChangeStrategy.Add<Label, GiteaApi>));
        }

        [TestMethod]
        public void TestDeletion()
        {
            var giteaApi = new GiteaApi(null, null, "", "");
            var processor = new LabelDifferenceProcessor(giteaApi);

            var source = new List<Label>
            {
                new Label
                {
                    Name = "label1",
                    Color = "ffffff"
                },
                new Label
                {
                    Id = 123,
                    Name = "label2",
                    Color = "000000",
                    Description = "text"
                }
            };
            var configured = new List<Label>();

            IReadOnlyList<LabelChangeStrategy> strategies = processor.Process(source, configured);

            Assert.AreEqual(2, strategies.Count);
            Assert.IsInstanceOfType(strategies[0], typeof(LabelChangeStrategy.Remove<Label, GiteaApi>));
            Assert.IsInstanceOfType(strategies[1], typeof(LabelChangeStrategy.Remove<Label, GiteaApi>));
        }

        [TestMethod]
        public void TestUpdate_Id()
        {
            var giteaApi = new GiteaApi(null, null, "", "");
            var processor = new LabelDifferenceProcessor(giteaApi);

            var source = new List<Label>
            {
                new Label
                {
                    Id = 123,
                    Name = "label1",
                    Color = "ffffff"
                }
            };
            var configured = new List<Label>
            {
                new Label
                {
                    Id = 123,
                    Name = "label2",
                    Color = "ffffff"
                }
            };

            IReadOnlyList<LabelChangeStrategy> strategies = processor.Process(source, configured);

            Assert.AreEqual(1, strategies.Count);
            Assert.IsInstanceOfType(strategies[0], typeof(LabelChangeStrategy.Update<Label, GiteaApi>));
        }

        [TestMethod]
        public void TestUpdate_Name()
        {
            var giteaApi = new GiteaApi(null, null, "", "");
            var processor = new LabelDifferenceProcessor(giteaApi);

            var source = new List<Label>
            {
                new Label
                {
                    Id = 123,
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
            Assert.IsInstanceOfType(strategies[0], typeof(LabelChangeStrategy.Update<Label, GiteaApi>));
        }

        [TestMethod]
        public void TestUpdate_Color()
        {
            var giteaApi = new GiteaApi(null, null, "", "");
            var processor = new LabelDifferenceProcessor(giteaApi);

            var source = new List<Label>
            {
                new Label
                {
                    Id = 123,
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
            Assert.IsInstanceOfType(strategies[0], typeof(LabelChangeStrategy.Update<Label, GiteaApi>));
        }

        [TestMethod]
        public void TestUpdate_Description()
        {
            var giteaApi = new GiteaApi(null, null, "", "");
            var processor = new LabelDifferenceProcessor(giteaApi);

            var source = new List<Label>
            {
                new Label
                {
                    Id = 123,
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
            Assert.IsInstanceOfType(strategies[0], typeof(LabelChangeStrategy.Update<Label, GiteaApi>));
        }
    }
}
