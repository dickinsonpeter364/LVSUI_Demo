using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static LVS3.CameraManager;

namespace LVS3
{
    public partial class frmBackingCamera : Form
    {
        protected internal CameraAria cameraAria;

        public frmBackingCamera()
        {
            InitializeComponent();
            if (CameraManager.AriaCameras[0] == null)
                return;
            this.cameraAria = CameraManager.AriaCameras[0];
            if (cameraAria.AriaCam.Acquire == true)
                cameraAria.AriaCam.Acquire = false;
            cameraAria.AriaCam.LiveControl = scAria.Panel1;
        }


        public new void Capture(bool starting)
        {
            try
            {
                if (starting)
                {
                    if (cameraAria.AriaCam.Acquire == true)
                        cameraAria.AriaCam.Acquire = false;
                    cameraAria.AriaCam.LiveControl = scAria.Panel1;
                    cameraAria.AriaCam.Acquire = true;
                }
                else
                {
                    if (cameraAria.AriaCam.Acquire == true)
                        cameraAria.AriaCam.Acquire = false;
                }
            }
            catch { }
        }
    }
}
