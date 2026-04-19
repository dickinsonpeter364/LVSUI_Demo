using System;

namespace ComMatcherTest
{
    /// <summary>
    /// Configuration class for a template (image) search definition.
    /// </summary>
    [Serializable]
    public class TemplateSearchDefinition : ElementSearchDefinition
    {
        public string TemplateFileName { get; set; }

        // Parameterless ctor required by XmlSerializer
        public TemplateSearchDefinition()
        {
            TemplateFileName = string.Empty;
        }

        public TemplateSearchDefinition(string templateFileName)
        {
            TemplateFileName = templateFileName;
        }

        public TemplateSearchDefinition(string fileName, int x, int y, int width, int height)
            : base(x, y, width, height)
        {
            TemplateFileName = fileName;
        }
    }
}
