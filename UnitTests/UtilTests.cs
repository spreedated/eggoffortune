using LogicLayer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace UnitTests
{
    [TestFixture]
    public class UtilTests
    {
        private readonly List<Phrase> phrases = [];

        [SetUp]
        public void SetUp()
        {
            Assembly a = typeof(UtilTests).Assembly;
            string s = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(a.Location), "..", "..", "..", "..", "TheEggOfFortune", "Resources", "Raw", "Phrases.txt"));

            using (Stream f = File.Open(s, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (StreamReader r = new(f))
                {
                    foreach (string t in r.ReadToEnd().Split('\n').Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Trim()))
                    {
                        if (this.phrases.Exists(x => x.Text == t))
                        {
                            continue;
                        }

                        this.phrases.Add(new()
                        {
                            Text = t
                        });
                    }
                }
            }
        }

        [Test]
        [Description("Tests the GetRandomPhrase method to ensure no duplicates are returned upon one cycle.")]
        public void RandomPickWithoutDuplicatesTest()
        {
            List<string> output = [];
            for (int i = 0; i < this.phrases.Count; i++)
            {
                output.Add(LogicLayer.Utilities.GetRandomPhrase(this.phrases));
            }

            Assert.That(output.Distinct().Count(), Is.EqualTo(this.phrases.Count));
            Assert.Multiple(() =>
            {
                Assert.That(output.Distinct().Count(), Is.EqualTo(output.Count));
                Assert.That(this.phrases.TrueForAll(x => x.Used), Is.True);
            });

            LogicLayer.Utilities.GetRandomPhrase(this.phrases);

            Assert.Multiple(() =>
            {
                Assert.That(this.phrases.TrueForAll(x => x.Used), Is.False);
                Assert.That(this.phrases.Count(x => x.Used), Is.EqualTo(1));
            });
        }

        [TearDown]
        public void TearDown()
        {

        }
    }
}
