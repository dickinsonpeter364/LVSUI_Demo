using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing; // Requires System.Drawing assembly
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml.Serialization; // Added for serialization
using OpenCVComMatcherLib; // The COM Library

namespace ComMatcherTest
{
    class Program
    {
        // Path configuration
        const string SourceImage = "source.png";
        const string TemplateImage = "template.png";
        const string MaskImage = "mask.png"; // Optional
        const string DataMatrixImage = "datamatrix.png"; // For barcode test
        const string LabelXml = "label_definition.xml";

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Initializing COM Object...");
                IImageMatcher matcher = new ImageMatcher();
                GetNormalisedImages(matcher);
                //TestPackingImages(matcher);
                //return;
/*
                // ---------------------------------------------------------
                // TEST 1: MatchTemplate (Legacy Byte Array Method)
                // ---------------------------------------------------------
                Console.WriteLine("\n--- TEST 1: MatchTemplate (Byte Arrays) ---");
                if (File.Exists(SourceImage) && File.Exists(TemplateImage))
                {
                    TestByteArrayMatching(matcher);
                }
                else
                {
                    Console.WriteLine($"Skipping Test 1: {SourceImage} or {TemplateImage} not found.");
                }

                // ---------------------------------------------------------
                // TEST 2: MatchTemplateFromFile (Single File)
                // ---------------------------------------------------------
                Console.WriteLine("\n--- TEST 2: MatchTemplateFromFile ---");
                if (File.Exists(SourceImage) && File.Exists(TemplateImage))
                {
                    TestFileMatching(matcher);
                }
                else
                {
                    Console.WriteLine($"Skipping Test 2: Files not found.");
                }

                // ---------------------------------------------------------
                // TEST 3: GetDataMatrices (Stored Source)
                // ---------------------------------------------------------
                Console.WriteLine("\n--- TEST 3: GetDataMatrices (Data Matrix Detection) ---");
                if (File.Exists(DataMatrixImage))
                {
                    TestDataMatrixDetection(matcher);
                }
                else
                {
                    Console.WriteLine($"Skipping Test 3: {DataMatrixImage} not found.");
                }

                // ---------------------------------------------------------
                // TEST 4: MatchTemplates (Batch from Files)
                // ---------------------------------------------------------
                Console.WriteLine("\n--- TEST 4: MatchTemplates (Batch) ---");
                if (File.Exists(SourceImage) && File.Exists(TemplateImage))
                {
                    TestBatchTemplateMatching(matcher);
                }
                else
                {
                    Console.WriteLine($"Skipping Test 4: Files not found.");
                }

                // ---------------------------------------------------------
                // TEST 5: SetLabelDefinition
                // ---------------------------------------------------------
                Console.WriteLine("\n--- TEST 5: SetLabelDefinition (XML + Image) ---");
                if (!File.Exists(LabelXml)) CreateDummyLabelXml();
                if (File.Exists(SourceImage) && File.Exists(LabelXml))
                {
                    TestSetLabelDef(matcher);
                }
*/
                // ---------------------------------------------------------
                // TEST 6: TestMatch (Specific File Paths)
                // ---------------------------------------------------------
                //Console.WriteLine("\n--- TEST 6: TestMatch (Hardcoded Paths) ---");
                //TestMatch(matcher);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CRITICAL ERROR: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
        static void TestPackingImages(IImageMatcher matcher)
        {
            string folderPath = @"C:\1404 labels\Packing\";
            string imageDefPath = @"C:\1404 labels\package.xml";

            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine($"Skipping Test 7: Directory {folderPath} not found.");
                return;
            }

            if (!File.Exists(imageDefPath))
            {
                Console.WriteLine($"Skipping Test 7: Image definition {imageDefPath} not found.");
                return;
            }

            try
            {
                string xmlImageDef = File.ReadAllText(imageDefPath);
                string[] files = Directory.GetFiles(folderPath, "*.bmp");

                Console.WriteLine($"Found {files.Length} BMP files in {folderPath}.");

                foreach (string file in files)
                {
                    Console.WriteLine($"Processing {Path.GetFileName(file)}...");
                    int w, h, c;
                    byte[] imgBytes = LoadImageToBytes(file, out w, out h, out c);

                    // This calls SetSourceImage, which (in C++) processes the image and writes a debug file to C:\tmp
                    matcher.SetSourceImage(imgBytes, w, h, c, xmlImageDef);
                }
                Console.WriteLine("Packing Images Iteration complete.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TestPackingImages Error: {ex.Message}");
            }
        }

        static void WriteMarkedImageToDisk(IImageMatcher matcher)
        {
            try
            {
                Console.WriteLine("Retrieving Marked Image...");

                Array imgSafeArray;
                int w, h, c;
               
                // Call COM Method
                matcher.GetMarkedImage(out imgSafeArray, out w, out h, out c);

                // Explicit cast to byte[]
                byte[] imgData = (byte[])imgSafeArray;
                if (imgData != null && imgData.Length > 0 && w > 0 && h > 0)
                {
                    string outputDir = @"C:\tmp";
                    if (!Directory.Exists(outputDir))
                    {
                        Directory.CreateDirectory(outputDir);
                    }
                    string outputPath = Path.Combine(outputDir, "marked_result.bmp");

                    // Create Bitmap from raw bytes
                    using (Bitmap bmp = new Bitmap(w, h, PixelFormat.Format24bppRgb))
                    {
                        BitmapData data = bmp.LockBits(
                            new Rectangle(0, 0, w, h),
                            ImageLockMode.WriteOnly,
                            PixelFormat.Format24bppRgb
                        );

                        // Calculate bytes per row (BGR = 3 channels)
                        int rowBytes = w * 3;
                        int stride = data.Stride;

                        // Copy data line by line to handle padding (stride)
                        for (int y = 0; y < h; y++)
                        {
                            int sourceIndex = y * rowBytes;
                            IntPtr destPtr = IntPtr.Add(data.Scan0, y * stride);
                            Marshal.Copy(imgData, sourceIndex, destPtr, rowBytes);
                        }

                        bmp.UnlockBits(data);
                        bmp.Save(outputPath, ImageFormat.Bmp);
                    }
                    Console.WriteLine($"Marked image written to: {outputPath}");
                }
                else
                {
                    Console.WriteLine("No marked image data returned.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing marked image: {ex.Message}");
            }
        }

        static void GetNormalisedImages(IImageMatcher matcher)
        {
            string sourceDir = @"d:\img";
            if (!Directory.Exists(sourceDir))
            {
                Console.WriteLine($"Skipping Test 7: Directory {sourceDir} does not exist.");
                return;
            }

            string[] files = Directory.GetFiles(sourceDir, "*.bmp");
            Console.WriteLine($"Found {files.Length} BMP files in {sourceDir}");

            foreach (string file in files)
            {
                // Skip files we've already generated to prevent reprocessing outputs
                if (file.EndsWith("_normalised.bmp")) continue;

                try
                {
                    Console.WriteLine($"Processing {Path.GetFileName(file)}...");
                    int w, h, c;
                    byte[] imgBytes = LoadImageToBytes(file, out w, out h, out c);

                    // Create default image definition (Full image, 0 rotation)
                    // We generate the XML string on the fly
                    var def = new ImageDefinition(0, 0, w, h, 0);
                    StringWriter sw = new StringWriter();
                    XmlSerializer xs = new XmlSerializer(typeof(ImageDefinition));
                    xs.Serialize(sw, def);

                    // Set Source
                    matcher.SetSourceImage(imgBytes, w, h, c, sw.ToString());

                    // Get Normalised Result
                    using (Bitmap normBmp = GetNormalisedBitmap(matcher))
                    {
                        if (normBmp != null)
                        {
                            string outFile = Path.Combine(sourceDir, Path.GetFileNameWithoutExtension(file) + "_normalised.bmp");
                            normBmp.Save(outFile, ImageFormat.Bmp);
                            Console.WriteLine($"Saved: {outFile}");
                        }
                        else
                        {
                            Console.WriteLine("Failed to retrieve normalised image.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing {file}: {ex.Message}");
                }
            }
        }
        static void TestMatch(IImageMatcher matcher)
        {
            string imageDefPath = @"C:\1404 labels\package.xml";
            string sourceImagePath = @"C:\1404 labels\Packing\2.bmp";
            string labelDefPath = @"C:\1404 labels\LabelDefinition.xml";

            if (File.Exists(sourceImagePath) && File.Exists(labelDefPath))
            {
                try
                {
                    // 2. Load Source Image
                    Console.WriteLine($"Loading Source Image from {sourceImagePath}...");
                    int w, h, c;
                    byte[] imgBytes = LoadImageToBytes(sourceImagePath, out w, out h, out c);

                    // 3. Set Source Image
                    Console.WriteLine("Calling SetSourceImage...");
                    matcher.SetSourceImage(imgBytes, w, h, c, "");

                    // 4. Load Label Definition XML
                    Console.WriteLine($"Loading Label Definition from {labelDefPath}...");
                    string labelDefXml = File.ReadAllText(labelDefPath);

                    // 5. Set Label Definition
                    Console.WriteLine("Calling SetLabelDefinition...");
                    matcher.SetLabelDefinition(labelDefXml);

                    Console.WriteLine("TestMatch setup complete.");

                    // 6. Perform Match with Variables
                    Console.WriteLine("Calling PerformMatch with variables...");
                    string[] keys = new string[] { "PROT", "BNO", "MED",  "EXPE" };
                    string[] values = new string[] { "1404-P", "PR24/10339", "11877391", "08/2026" };

                    // time how long PerformMatch takes
                    Stopwatch sw = Stopwatch.StartNew();
                    bool matchResult = matcher.PerformMatch(keys, values);
                    // stop timing
                    sw.Stop();
                    
                    Console.WriteLine($"PerformMatch completed in {sw.ElapsedMilliseconds} ms.");
                    sw = Stopwatch.StartNew();
                    matchResult = matcher.PerformMatch(keys, values) ;
                    // stop timing
                    sw.Stop();
                    Console.WriteLine($"PerformMatch completed in {sw.ElapsedMilliseconds} ms.");
                    sw = Stopwatch.StartNew();
                    matchResult = matcher.PerformMatch(keys, values);
                    // stop timing
                    sw.Stop();
                    Console.WriteLine($"PerformMatch completed in {sw.ElapsedMilliseconds} ms.");
                    sw = Stopwatch.StartNew();
                    matchResult = matcher.PerformMatch(keys, values);
                    // stop timing
                    sw.Stop();
                    Console.WriteLine($"PerformMatch completed in {sw.ElapsedMilliseconds} ms.");
                    
                    if (matchResult)
                    {
                        Console.WriteLine("SUCCESS: PerformMatch passed.");
                    }
                    else
                    {
                        Console.WriteLine("FAILURE: PerformMatch failed.");
                    }
                    WriteMarkedImageToDisk(matcher);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"TestMatch Error: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Skipping Test 6: One or more required files not found in C:\\1404 labels\\...");
            }
        }

        static void CreateDummyLabelXml()
        {
            var label = new LabelDefinition("Test Label");
            label.Elements.Add(new TemplateSearchDefinition("logo.png", 100, 100, 50, 50));
            label.Elements.Add(new TextSearchDefinition("Ref No", 200, 200, 100, 30));
            label.SaveToFile(LabelXml);
            Console.WriteLine($"Created {LabelXml}");
        }

        static void TestSetLabelDef(IImageMatcher matcher)
        {
            string xmlContent = File.ReadAllText(LabelXml);
            int w, h, c;
            byte[] imgBytes = LoadImageToBytes(SourceImage, out w, out h, out c);
            matcher.SetLabelDefinition(xmlContent);
            Console.WriteLine("SetLabelDefinition successful.");
        }

        static void TestByteArrayMatching(IImageMatcher matcher)
        {
            int srcW, srcH, srcC;
            byte[] srcData = LoadImageToBytes(SourceImage, out srcW, out srcH, out srcC);

            // Create and Serialize ImageDefinition
            var def = new ImageDefinition(0, 0, srcW, srcH, 0); // No rotation, full clip
            StringWriter sw = new StringWriter();
            XmlSerializer xs = new XmlSerializer(typeof(ImageDefinition));
            xs.Serialize(sw, def);
            string defXml = sw.ToString();

            Console.WriteLine("Setting Source Image (XML)...");
            matcher.SetSourceImage(srcData, srcW, srcH, srcC, defXml);

            int tmplW, tmplH, tmplC;
            byte[] tmplData = LoadImageToBytes(TemplateImage, out tmplW, out tmplH, out tmplC);

            byte[] maskData = null;
            int maskW = 0, maskH = 0, maskC = 0;
            if (File.Exists(MaskImage))
            {
                maskData = LoadImageToBytes(MaskImage, out maskW, out maskH, out maskC);
            }

            int x, y;
            double conf;
            bool found = matcher.MatchTemplate(tmplData, tmplW, tmplH, tmplC, maskData, maskW, maskH, maskC, 0, 0, srcW, srcH, out x, out y, out conf);
            Console.WriteLine($"Result: Found={found}, Pos=({x},{y}), Conf={conf:F4}");
        }

        static void TestFileMatching(IImageMatcher matcher)
        {
            string absSrc = Path.GetFullPath(SourceImage);
            string absTmpl = Path.GetFullPath(TemplateImage);
            string absMask = File.Exists(MaskImage) ? Path.GetFullPath(MaskImage) : "";

            int srcW, srcH, srcC;
            byte[] srcBytes = LoadImageToBytes(absSrc, out srcW, out srcH, out srcC);

            var def = new ImageDefinition(0, 0, srcW, srcH, 0);
            StringWriter sw = new StringWriter();
            XmlSerializer xs = new XmlSerializer(typeof(ImageDefinition));
            xs.Serialize(sw, def);

            matcher.SetSourceImage(srcBytes, srcW, srcH, srcC, sw.ToString());

            int x, y;
            double conf;
            bool found = matcher.MatchTemplateFromFile(absTmpl, absMask, 0, 0, srcW, srcH, out x, out y, out conf);
            Console.WriteLine($"Result: Found={found}, Pos=({x},{y}), Conf={conf:F4}");
        }

        static void TestDataMatrixDetection(IImageMatcher matcher)
        {
            string absPath = Path.GetFullPath(DataMatrixImage);
            int w, h, c;
            byte[] imgBytes = LoadImageToBytes(absPath, out w, out h, out c);

            var def = new ImageDefinition(0, 0, w, h, 0);
            StringWriter sw = new StringWriter();
            XmlSerializer xs = new XmlSerializer(typeof(ImageDefinition));
            xs.Serialize(sw, def);

            matcher.SetSourceImage(imgBytes, w, h, c, sw.ToString());

            string jsonResult;
            bool found = matcher.GetDataMatrices(out jsonResult);
            if (found) Console.WriteLine($"Data Matrix found: {jsonResult}");
            else Console.WriteLine("No Data Matrix codes found.");
        }
        static Bitmap GetNormalisedBitmap(IImageMatcher matcher)
        {
            Array imgSafeArray;
            int w, h, c;

            try
            {
                matcher.RetrieveNormalisedImage(out imgSafeArray, out w, out h, out c);

                byte[] imgData = (byte[])imgSafeArray;
                if (imgData == null || w <= 0 || h <= 0) return null;

                if (c == 1)
                {
                    Bitmap bmp = new Bitmap(w, h, PixelFormat.Format8bppIndexed);
                    ColorPalette pal = bmp.Palette;
                    for (int i = 0; i < 256; i++) pal.Entries[i] = Color.FromArgb(i, i, i);
                    bmp.Palette = pal;

                    BitmapData data8 = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
                    int stride = data8.Stride;
                    for (int y = 0; y < h; y++)
                    {
                        Marshal.Copy(imgData, y * w, IntPtr.Add(data8.Scan0, y * stride), w);
                    }
                    bmp.UnlockBits(data8);
                    return bmp;
                }
                else
                {
                    // Assume 3 channels BGR
                    Bitmap bmp = new Bitmap(w, h, PixelFormat.Format24bppRgb);
                    BitmapData data = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                    int rowBytes = w * c;
                    int stride = data.Stride;
                    for (int y = 0; y < h; y++)
                    {
                        Marshal.Copy(imgData, y * rowBytes, IntPtr.Add(data.Scan0, y * stride), rowBytes);
                    }
                    bmp.UnlockBits(data);
                    return bmp;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving normalised image: " + ex.Message);
                return null;
            }
        }

        static void TestBatchTemplateMatching(IImageMatcher matcher)
        {
            string absSrc = Path.GetFullPath(SourceImage);
            string absTmpl = Path.GetFullPath(TemplateImage);
            int srcW, srcH, srcC;
            byte[] srcBytes = LoadImageToBytes(absSrc, out srcW, out srcH, out srcC);

            var def = new ImageDefinition(0, 0, srcW, srcH, 0);
            StringWriter sw = new StringWriter();
            XmlSerializer xs = new XmlSerializer(typeof(ImageDefinition));
            xs.Serialize(sw, def);

            matcher.SetSourceImage(srcBytes, srcW, srcH, srcC, sw.ToString());

            string[] templates = new string[] { absTmpl, absTmpl };
            string jsonResult;
            bool anyFound = matcher.MatchTemplates(templates, 0, 0, srcW, srcH, out jsonResult);
            if (anyFound) Console.WriteLine($"Matches found: {jsonResult}");
            else Console.WriteLine("No matches found.");
        }

        static byte[] LoadImageToBytes(string filePath, out int width, out int height, out int channels)
        {
            using (Bitmap bmp = new Bitmap(filePath))
            {
                width = bmp.Width;
                height = bmp.Height;
                channels = 3;
                BitmapData data = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                int stride = data.Stride;
                int totalBytes = Math.Abs(stride) * height;
                byte[] rawBytes = new byte[totalBytes];
                Marshal.Copy(data.Scan0, rawBytes, 0, totalBytes);
                bmp.UnlockBits(data);
                if (stride == width * channels) return rawBytes;
                byte[] compactBytes = new byte[width * height * channels];
                for (int y = 0; y < height; y++) Array.Copy(rawBytes, y * stride, compactBytes, y * width * channels, width * channels);
                return compactBytes;
            }
        }
    }

    // =========================================================
    //  Serializable Classes (Polymorphic)
    // =========================================================


}