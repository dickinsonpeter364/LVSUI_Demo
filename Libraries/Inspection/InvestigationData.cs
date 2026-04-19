using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LVS3
{
    public class InvestigationData
    {
        // TODO: replaced HalconDotNet.HObject with Bitmap — use ImageProc/OpenCV for image processing
        public Bitmap imgReject = null;
        public Bitmap imgBackingCam = null;
        public string MedID;

    }
}
