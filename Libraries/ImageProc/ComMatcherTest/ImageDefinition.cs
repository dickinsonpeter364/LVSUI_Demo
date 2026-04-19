using System.Xml.Serialization;

namespace ComMatcherTest
{
    /// <summary>
    /// Configuration class for source image preprocessing (clipping and rotation).
    /// </summary>
    [Serializable]
    public class ImageDefinition
    {
        public int ClipX { get; set; }
        public int ClipY { get; set; }
        public int ClipWidth { get; set; }
        public int ClipHeight { get; set; }
        public int RotationAngle { get; set; }

        public ImageDefinition() { }

        public ImageDefinition(int x, int y, int w, int h, int rotation)
        {
            ClipX = x;
            ClipY = y;
            ClipWidth = w;
            ClipHeight = h;
            RotationAngle = rotation;
        }

        public void SaveToFile(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ImageDefinition));
            using (TextWriter writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, this);
            }
        }

        public void LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath)) return;

            XmlSerializer serializer = new XmlSerializer(typeof(ImageDefinition));
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                ImageDefinition loaded = (ImageDefinition)serializer.Deserialize(fs)!;
                this.ClipX = loaded.ClipX;
                this.ClipY = loaded.ClipY;
                this.ClipWidth = loaded.ClipWidth;
                this.ClipHeight = loaded.ClipHeight;
                this.RotationAngle = loaded.RotationAngle;
            }
        }
    }
}
