using System.Drawing;
using LVS3;
using System;
using System.Collections.Generic;using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using static LVS3.CameraManager;
using static LVS3.Delegates;
using static LVS3.Enums;
using OpenCVComMatcherLib;

namespace BASE
{
    public class DummyInspection : Inspection, IInspection, IDisposable
    {
        public static event ErrorMethodHandler Emh;
        private static int imageNumber = 1;
        private bool _cancelCollection;


        public DummyInspection( IDataManager dataManager, IUtilityFunctions utilityFunctions, IPeripherals peripherals, IImageMatcher imageMatcher ) : base(dataManager, utilityFunctions, peripherals, imageMatcher)
        {
            
        }
        public void CollectImagesForInspection()
        {
            while (!_cancelCollection)
            {
                ProcessInspectionImage();
                _inhibitNextCapture = false;
                Application.DoEvents();
                if (_cancelCollection)
                    break;
            }

        }
        public override void GetImageInspection()
        {
            _logger.Information("Entering GetImageInspection()");
            _inhibitNextCapture = false;
            Task task = Task.Run(() => { CollectImagesForInspection(); });
        }

        private bool ReadImage( ref Bitmap img )
        {
            string savePath;
            if (string.IsNullOrEmpty(AppData.SavedImagesPath))
            {
                var folder = Application.StartupPath;
                var applicationFolder = Path.Combine(folder,
                "ImageDump");
                //Directory.Delete(applicationFolder,true);
                if (!Directory.Exists(applicationFolder))
                    Directory.CreateDirectory(applicationFolder);
                savePath = applicationFolder;
            }
            else
                savePath = AppData.SavedImagesPath;

            savePath = Path.Combine(savePath, this.ReelLpn);

            
            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);

            //Directory.Delete(applicationFolder,true);

            if (!File.Exists(Path.Combine(savePath,
                    imageNumber.ToString() + ".bmp")))
                return false;
            /* TODO: Replace HOperatorSet.ReadImage */ //out img, Path.Combine( savePath,
                imageNumber.ToString() + ".bmp"));
            imageNumber++;
            return true;
        }

        protected override void ProcessInspectionImage()
        {
            
            _logger.Information("Entering ProcessInspectionImage()");
            try
            {
                ClearImage();
                if (EnableCapture)
                {
                    FrmSelect.MxClient.WriteToRegister(1,
                        "Capture_Image",
                        1,
                        3);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex,
                    "Exception in GetImageInspection: {Message}",
                    ex.Message);
                if (SYSTEM_IO.PROCESSING == false)
                    return;
                SYSTEM_IO.PROCESSING = false;
                var err = "GetImageInspection() err: " + ex.Message;
                var smea = new SystemMessageEventArgs(err,
                    "Image Capture",
                    (int)CriticalLevels.Red);
                UscMd.SystemMessage(smea);
                Emh?.Invoke(err);
            }

            _edgesNotDetectedError = false;
            if (!_inhibitNextCapture)
            {
                _inhibitNextCapture = true;
                //bool errorRaisedToPLC = false;
                while (_reviewing)
                {
                    Application.DoEvents();
                }
                _inspecting = true;

                var sw = new Stopwatch();
                Bitmap tmp1 = null;
                var inspectionPassedOk = false;
                try
                {
                    sw.Start();
                    _currentReducedImage?.Dispose();
                    DisplayClearInspection();
                    Positioner.MoveNext(this);
                    Positioner.CanMoveNext = true;

                    var fp = GetNewFailRecord();
                    foreach (var med in _medDataItems)
                    {
                        if (med.PlaceHolder == VariableMedDataPh || med.VarName.ToLower().Contains("med_id"))
                        {
                            fp.MED_ID = med.Data;
                            break;
                        }
                    }

                    //get the image from the cameras
                    if (!ReadImage(ref tmp1))
                    {
                        return;
                    }
                    if (Positioner.ImageCounter >= LabelCount)
                    {
                        if (InspectionEnd(ref tmp1,
                                ref fp))
                        {
                            _cancelCollection = true;
                            CompleteInspection();
                            return;
                        }

                    }

                    Labelcounts.CountInspected += 1;
                    if (_sampleLabel != "")
                    {
                        Labelcounts.HasSample = true;
                    }

                    // incremented here before fails occur - value is decremented if all tests pass
                    //failCount++;

                    ////****************************************************************************************
                    //UnRem this block to fast forward through labels without inspection (qty to bypass is set in _debug_inspection() method)
                    //if (_debug_inspection(fp))
                    //    return;
                    ////****************************************************************************************

                    //FrmSelect.MxClient.Stop(1);
                    /*
                    if (ReduceBackground(ref tmp1,
                            ref fp))
                    {
                        ////****************************************************************************************
                        //UnRem this block to get cropped labels written to disk as bitmaps - saves cropped labels without inspection
                        //var ret1 = attachBackgroundToWindow(ref tmp1, ref fp);
                        //NoInspectionSaveImagesToFile(tmp1, 50);
                        //fp.VALID_LABEL = true;
                        //return;
                        ////****************************************************************************************

                        //FrmSelect.MxClient.Stop(1);

                        var ret = AttachBackgroundToWindow(ref tmp1);
                        if (ret)
                        {
                            if (SYSTEM_IO.PROCESSING == false)
                            {
                                return;
                            }
                            //Look for defects around the opzone
                            InspectOpZone(ref tmp1,
                                ref fp);
                            if (SYSTEM_IO.PROCESSING == false)
                            {
                                return;
                            }

                            InspectLabel(tmp1,
                                ref fp);
                            if (SYSTEM_IO.PROCESSING == false)
                            {
                                return;
                            }

                            if (fp.VALID_LABEL && fp.ConfidenceLevel >= _confidenceLevel)
                            {
                                inspectionPassedOk = true;
                                DisplayGoodInspection(fp.LabelIndex);
                                _inhibitNextCapture = false;
                            }
                            else
                            {
                                _inhibitNextCapture = true;
                                AddFailAndStop(fp);
                                ReviewLabelUnderInvestigation();
                                _inhibitNextCapture = false;
                            }
                        }
                    }
                    */
                    if (SYSTEM_IO.PROCESSING)
                    {
                        if (inspectionPassedOk)
                        {
                            Labelcounts.CountAccepted += 1;
                            if (fp.SAMPLE == false)
                            {
                                fp.ClearData();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex,
                        "Exception in ProcessInspectionImage: {Message}",
                        ex.Message);
                    if (SYSTEM_IO.PROCESSING == false)
                        return;
                    SYSTEM_IO.PROCESSING = false;
                    var err = "ProcessInspectionImage() err: " + ex.Message;
                    var smea = new SystemMessageEventArgs(err,
                        "Inspection Error",
                        (int)CriticalLevels.Red);
                    UscMd.SystemMessage(smea);
                    Emh?.Invoke(err);
                }
                finally
                {
                    tmp1?.Dispose();
                    _currentRawImage?.Dispose();

                    _inspecting = false;
                    sw.Stop();
                    var ms = sw.ElapsedMilliseconds;
                    if (!inspectionPassedOk)
                    {
                        DispTimeText("");
                    }
                    if (ms < 100)
                        ms = 150;
                    else if (ms < 200)
                        ms = 300;
                    else if (ms < 400)
                        ms = 600;
                    else if (ms < 500)
                        ms = 800;
                    else if (ms < 1000)
                        ms = 1000;
                    if ( (imageNumber % 100) == 0)
                        GC.Collect(GC.MaxGeneration);
                    SpeedControl.UpdateSpeedControl(ms);
                    
                }
            }

            else
            {
                //Console.WriteLine("");
            }
        }
        protected override void ReviewLabelUnderInvestigation()
        {
            _logger.Information("Entering ReviewLabelUnderInvestigation()");
            FailRecord fpData = null;
            var imageFolder = ImageData.CreateImageFilepath();
            var layoutFile = Path.Combine(imageFolder,
                LabelItem + "_legendkey.png");
            try
            {
                if (true)
                {
                    if (SYSTEM_IO.PROCESSING)
                        if (_cmdEndInspection.InvokeRequired)
                        {
                            _cmdEndInspection.Invoke((MethodInvoker)delegate
                            {
                                _cmdEndInspection.Enabled = true;
                            });
                        }
                        else
                            _cmdEndInspection.Enabled = true;
                    if ( SYSTEM_IO.PROCESSING )
                    foreach (var fr in FailRecord.FailPipes)
                    {
                        if (fr.ACTIONED == false && fr.MISSING == false)
                        {
                            fpData = fr;
                            break;
                        }
                    }
                    if (fpData != null)
                    {
                        FrmInspect.FrmUi = new FrmUnderInvestigation(layoutFile,
                            LabelItem,
                            _variationRegion,
                            UtilityFunctions);
                        FrmInspect.FrmUi.TopLevel = true;
                        FrmInspect.FrmUi.TopMost = true;
                        FrmInspect.FrmUi.Refresh();
                        FrmInspect.FrmUi.Init(fpData,
                            LabelItem,
                            FailFolder);
                        FrmInspect.FrmUi.Visible = false;
                        FrmInspect.FrmUi.ShowDialog();
                        Labelcounts.CountQueried += 1;
                        if (fpData.SAMPLE == false)
                        {
                            if (FrmInspect.FrmUi.AcceptedByOperator)
                                Labelcounts.CountAcceptedOp += 1;
                            else
                                Labelcounts.CountRejectedOp += 1;
                        }
                        else
                            Labelcounts.CountAcceptedOp += 1;
                        FrmInspect.FrmUi.Close();
                        FrmInspect.FrmUi = null;

                        fpData.ACTIONED = true;
                        FrmSelect.MxClient.WriteToRegister(1,
                            "Rewind_After_INV",
                            1,
                            3);
                    }

                    else if (Positioner.ImageCounter > LabelCount)
                    {
                        FrmInspect.FrmUi = new FrmUnderInvestigation(layoutFile,
                            LabelItem,
                            _variationRegion,
                            UtilityFunctions);
                        FrmInspect.FrmUi.TopLevel = true;
                        FrmInspect.FrmUi.TopMost = true;
                        FrmInspect.FrmUi.Refresh();
                        FrmInspect.FrmUi.Visible = false;
                        FrmInspect.FrmUi.ShowDialog();
                        FrmInspect.FrmUi.Close();
                        FrmInspect.FrmUi = null;

                    }
                }
                _reviewing = false;


                if (MissingLabelTrigger)
                {
                    MissingLabelTriggered();
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex,
                    "Exception in ReviewLabelUnderInvestigation: {Message}",
                    ex.Message);
                if (SYSTEM_IO.PROCESSING == false)
                    return;
                SYSTEM_IO.PROCESSING = false;
                var err = "reviewLabelUnderInvestigation() err: " + ex.Message;
                var smea = new SystemMessageEventArgs(err,
                    "Inspection",
                    (int)CriticalLevels.Red);
                UscMd.SystemMessage(smea);
                _reviewing = false;
                Emh?.Invoke(err);
            }
            finally
            {
                fpData?.ClearData();
                _reviewing = false;
            }
        }

        public void Dispose()
        {
            _cancelCollection = true;
        }
    }

}

