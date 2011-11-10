using Microsoft.VisualStudio.TestTools.UnitTesting;
using push.config;
using push.genetics;
using push.parser;
using System.Collections.Generic;
using System;
using Push;
using Microsoft.FSharp.Collections;

namespace GeneticTests
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
            Assert.AreEqual(.7D, config.probMutation);
            Assert.AreEqual(.3D, config.probCrossover);
            Assert.AreEqual("CODE.NOOP", config.getArgument);
            Assert.AreEqual("(INTEGER.- FLOAT.FROMINTEGER)", config.getResult);
            Assert.AreEqual("2", reader.GetSampleValue(0, "In").ToString());
            Assert.AreEqual("2", reader.GetSampleValue(0, "Out").ToString());
        }

        [TestMethod]
        [Description("Test transfer of config to the genetic module")]
        [DeploymentItem("sampleConfig.xml")]
        public void GeneticConfigTest()
        {
            var config = Genetic.readConfig("sampleConfig.xml");

            Assert.AreEqual<int>(100, config.populSize);
            Assert.AreEqual<int>(300, config.maxCodePoints);
            Assert.AreEqual<int>(100, config.numGenerations);
            Assert.AreEqual(.7D, config.probMutation);
            Assert.AreEqual(.3D, config.probCrossover);
            Assert.AreEqual("CODE.NOOP", config.getArgument.StructuredFormatDisplay);
            Assert.AreEqual("3", config.fitnessValues[1].argument.StructuredFormatDisplay);
            Assert.AreEqual("6", config.fitnessValues[1].value.StructuredFormatDisplay);

        }
    }
}
