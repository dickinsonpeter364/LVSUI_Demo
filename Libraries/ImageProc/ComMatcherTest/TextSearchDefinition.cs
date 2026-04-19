using System;

namespace ComMatcherTest
{
    /// <summary>
    /// Configuration class for a text search definition.
    /// </summary>
    [Serializable]
    public class TextSearchDefinition : ElementSearchDefinition
    {
        public string SearchText { get; set; }

        // Parameterless ctor required by XmlSerializer
        public TextSearchDefinition()
        {
            SearchText = string.Empty;
        }

        public TextSearchDefinition(string searchText)
        {
            SearchText = searchText;
        }

        public TextSearchDefinition(string text, int x, int y, int width, int height)
            : base(x, y, width, height)
        {
            SearchText = text;
        }

    }
}
