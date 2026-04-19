using System;
using System.Xml.Serialization;

namespace ComMatcherTest
{
    /// <summary>
    /// Base class for all search definitions.
    /// Contains common coordinates and logic for serialization.
    /// </summary>
    [Serializable]
    [XmlInclude(typeof(TextSearchDefinition))]
    [XmlInclude(typeof(DataMatrixDefinition))]
    [XmlInclude(typeof(TemplateSearchDefinition))]
    public class ElementSearchDefinition
    {
        public int SearchX { get; set; }
        public int SearchY { get; set; }
        public int SearchWidth { get; set; }
        public int SearchHeight { get; set; }

        public ElementSearchDefinition() { }

        public ElementSearchDefinition(int x, int y, int width, int height)
        {
            SearchX = x;
            SearchY = y;
            SearchWidth = width;
            SearchHeight = height;
        }

    }

}
