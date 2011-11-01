using Microsoft.VisualStudio.TestTools.UnitTesting;
using push.config;
using System.Collections.Generic;
using System;

namespace ConfigTests
{
    [TestClass]
    public class ConfigTests
    {
        [TestMethod]
        [DeploymentItem("sampleConfig.xml")]
        public void ConfigReaderTest()
        {
            ConfigReader reader = new ConfigReader("sampleConfig.xml");

            dynamic config = reader.Read();

            Assert.AreEqual<int>(100, config.popSize);
            Assert.AreEqual<int>(300, config.maxCodePoints);
            Assert.AreEqual<int>(100, config.numGenerations);
            Assert.AreEqual<float>((float).7, config.probMutation);
            Assert.AreEqual<float>((float).3, config.probCrossover);
            Assert.AreEqual("CODE.NOOP", config.getArgument);
            Assert.AreEqual("(INTEGER.- FLOAT.FROMINTEGER)", config.getResult);
            Assert.AreEqual("2", reader.GetSampleValue(0, "In").ToString());
            Assert.AreEqual("2", reader.GetSampleValue(0, "Out").ToString());
        }

    }
}
