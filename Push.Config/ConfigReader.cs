using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Dynamic;

namespace push.config
{
    public class ConfigReader
    {
        XElement root;
        dynamic config;

        public int CountSamples {get; set;}

        public ConfigReader(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(fileName);
            }

            if (!File.Exists(fileName))
            {
                throw new IOException("File does not exist: " + fileName);
            }

            this.root = XDocument.Load(fileName).Root;
            this.config = new ExpandoObject();
        }

        public dynamic Read()
        {
            config.popSize = GetInt("PopulationSize");
            config.maxCodePoints = GetInt("MaxCodePoints");
            config.numGenerations = GetInt("NumGenerations");
            config.maxSteps = GetInt("MaxSteps");
            config.probCrossover = GetFloat("ProbCrossover");
            config.probMutation = GetFloat("ProbMutation");
            config.getArgument = GetString("GetArgument");
            config.getResult = GetString("GetResult");
            config.samples = GetSampleCollection();
            this.CountSamples = config.samples.Count;
            return config;
        }

        int GetInt(string name)
        {
            return int.Parse((from p in root.Descendants(name) select p).First().Value.Trim());
        }

        double GetFloat(string name)
        {
            return double.Parse((from p in root.Descendants(name) select p).First().Value.Trim());
        }

        string GetString(string name)
        {
            return (from p in root.Descendants(name) select p).First().Value.Trim();
        }

        List<dynamic> GetSampleCollection()
        {
            var samples = from s in this.root.Descendants("Sample") select s;
            var collection = new List<dynamic>();
            foreach (var s in samples)
            {
                dynamic sample = new ExpandoObject();
                foreach (var child in s.Descendants())
                {
                    (sample as IDictionary<String, object>)[child.Name.ToString()] = child.Value.Trim();
                }
                collection.Add(sample);
            }
            return collection;
        }

        public object GetSampleValue(int index, string val)
        {
            IDictionary<String, object> dict = config.samples[index];
            return dict[val];
        }

        public object GetSampleValue(IDictionary<String, object> dict, string val)
        {
            return dict[val];
        }
    }
}
