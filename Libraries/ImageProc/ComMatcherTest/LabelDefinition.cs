using System.Xml.Serialization;

namespace ComMatcherTest
{
    /// <summary>
    /// Configuration class for a label containing multiple search elements.
    /// </summary>
    [Serializable]
    public class LabelDefinition
    {
        public string? Name { get; set; }
        public List<ElementSearchDefinition>? Elements { get; set; }

        public LabelDefinition()
        {
            Elements = new List<ElementSearchDefinition>();
        }

        public void Add(ElementSearchDefinition element)
        {
            Elements?.Add(element);
        }
        public LabelDefinition(string name)
        {
            Name = name;
            Elements = new List<ElementSearchDefinition>();
        }

        /// <summary>
        /// Saves a list of LabelDefinitions to an XML file.
        /// </summary>
        public void SaveToFile(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(LabelDefinition));
            using (TextWriter writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, this);
            }
        }

        /// <summary>
        /// Loads data from an XML file into the current LabelDefinition instance.
        /// </summary>
        public void LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath)) return;

            XmlSerializer serializer = new XmlSerializer(typeof(LabelDefinition));
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                LabelDefinition? loaded = serializer.Deserialize(fs)! as LabelDefinition;
                this.Name = loaded?.Name;
                this.Elements = loaded?.Elements;
            }
        }
    }
}
