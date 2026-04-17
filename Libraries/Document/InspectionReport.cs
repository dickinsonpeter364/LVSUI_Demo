using CONSTANTS;
using Microsoft.Office.Interop.Word;
using System.Diagnostics;
using System.Globalization;
using static LVS3.Delegates;
using static LVS3.Enums;

namespace LVS3
{

    public class InspectionReport
    {
        private int LABEL_COUNT = 0;
        private LabelCounts labelCounts = null;
        public static string UserName = "";
        public static string OperatorSummary = "";
        private ProgressHandler PH;
        private HeaderVariableData hvd = null;
        private ReportModel RM = null;
        private ReportData reportData = null;
        private string failFolder = "";
        private string REEL_LPN = "";
        private string LIN = "";
        private string LWO = "";
        private Microsoft.Office.Interop.Word.Application WordApp = null;
        private object oMissing = System.Reflection.Missing.Value;
        private Document document = null;        
        private string varyingMedPH = "";
        private IDataManager DataManager;

        
        public InspectionReport(string reel, string lin, ProgressHandler ph, string operatorsummary, string reportuser, LabelCounts labelcounts, int reelcount, string lwo, string varyingMedPH, IDataManager dataManager)
        {
            this.labelCounts = labelcounts;            
            this.varyingMedPH = varyingMedPH;
            this.REEL_LPN = reel;
            this.LIN = lin;
            this.LWO = lwo;
            this.PH = ph;
            OperatorSummary = operatorsummary;
            UserName = reportuser;
            LABEL_COUNT = reelcount;
            DataManager = dataManager;
        }

        private void init()
        {            
            hvd = new HeaderVariableData(REEL_LPN, DataManager);
            RM = new ReportModel();
            reportData = new ReportData(DataManager);
            failFolder = ImageData.CreateFailFilepath(REEL_LPN);
            reportData.INIT(PH, REEL_LPN, LIN, failFolder, varyingMedPH);
        }

        private string getDebrisSize()
        {
            string retVal = "SMALL";
            double debrisSizeDefault = DataManager.GetInnerRadiusDefaultSmall();
            double debrisSizeThisLabel = DataManager.GetInnerRadius(Defaults.StationID, this.LIN);
            if (debrisSizeDefault < debrisSizeThisLabel)
                retVal = "LARGE";
            return retVal;
        }

        private string getDebrisSizeLabel()
        {
            string retVal = DataManager.GetDebrisSizeLabel(Defaults.StationID, this.LIN);
            return retVal;
        }

        private string getLabelTypeAsString()
        {
            string retVal = DataManager.GetLabelTypeAsString(Defaults.StationID, this.LIN);
            return retVal;
        }

        public bool CreateDocument()
        {
            bool retVal = true;
            int columnCount = 7;
            try
            {
                if (PDFDocumentIO.FileStatus(REEL_LPN) == false)
                    return false;
                init();
                //Console.WriteLine("REEL_LPN");
                WordApp = new Microsoft.Office.Interop.Word.Application();
                WordApp.ShowAnimation = false;
                WordApp.ShowWindowsInTaskbar = false;
                //WordApp.Visible = false;
                document = WordApp.Documents.Add(ref oMissing, ref oMissing, ref oMissing, ref oMissing);
                document.PageSetup.Orientation = WdOrientation.wdOrientLandscape;
                
                document.PageSetup.TopMargin = WordApp.InchesToPoints(0.2f);
                document.PageSetup.BottomMargin = WordApp.InchesToPoints(0.2f);
                document.PageSetup.LeftMargin = WordApp.InchesToPoints(0.2f);
                document.PageSetup.RightMargin = WordApp.InchesToPoints(0.2f);

                string customer = DataManager.GetCustomer(LIN);
                string stationID = Defaults.StationID.ToString();

                #region Page Header and Numbers
                //Add document header  and footer
                foreach (Section section in document.Sections)
                {
                    //Get the header range and add the header details.  
                    var headerRange = section.Headers[Microsoft.Office.Interop.Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range;
                    headerRange.Fields.Add(headerRange, Microsoft.Office.Interop.Word.WdFieldType.wdFieldPage);
                    headerRange.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    headerRange.Font.ColorIndex = Microsoft.Office.Interop.Word.WdColorIndex.wdBlue;
                    headerRange.Font.Size = 16;
                    headerRange.Font.Bold = 1;
                    headerRange.Text = "REEL LPN: " + REEL_LPN + Environment.NewLine;

                    string pageNum = "1";
                    WordApp.Selection.GoTo(Microsoft.Office.Interop.Word.WdGoToItem.wdGoToPage, Microsoft.Office.Interop.Word.WdGoToDirection.wdGoToNext, ref oMissing, pageNum);
                    Microsoft.Office.Interop.Word.Range rngPageNum = WordApp.Selection.Range;

                    Section currSec = document.Sections[rngPageNum.Sections[1].Index];
                    HeaderFooter ftr = currSec.Footers[Microsoft.Office.Interop.Word.WdHeaderFooterIndex.wdHeaderFooterPrimary];
                    ftr.LinkToPrevious = false;
                    ftr.PageNumbers.RestartNumberingAtSection = true;
                    ftr.PageNumbers.StartingNumber = 1;
                    object TotalPages = Microsoft.Office.Interop.Word.WdFieldType.wdFieldNumPages;
                    object CurrentPage = Microsoft.Office.Interop.Word.WdFieldType.wdFieldPage;
                    Microsoft.Office.Interop.Word.Range rngCurrSecFooter = ftr.Range;
                    rngCurrSecFooter.Fields.Add(rngCurrSecFooter, ref CurrentPage, ref oMissing, false);
                    rngCurrSecFooter.InsertAfter(" of ");
                    rngCurrSecFooter.Collapse(Microsoft.Office.Interop.Word.WdCollapseDirection.wdCollapseEnd);
                    rngCurrSecFooter.Fields.Add(rngCurrSecFooter, ref TotalPages, ref oMissing, false);
                }
                #endregion

                #region Summary Data
                Paragraph para2 = document.Content.Paragraphs.Add(ref oMissing);
                para2.Range.InsertParagraphAfter();
                Table tblData = document.Tables.Add(para2.Range, 7, 6, ref oMissing, ref oMissing);
                tblData.Borders.InsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleNone;
                tblData.Borders.OutsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;

                tblData.Columns[1].SetWidth(88f, WdRulerStyle.wdAdjustProportional);
                tblData.Columns[2].SetWidth(172f, WdRulerStyle.wdAdjustProportional);
                tblData.Columns[3].SetWidth(120f, WdRulerStyle.wdAdjustProportional);
                tblData.Columns[4].SetWidth(92f, WdRulerStyle.wdAdjustProportional);
                tblData.Columns[5].SetWidth(120f, WdRulerStyle.wdAdjustProportional);
                tblData.Columns[6].SetWidth(110f, WdRulerStyle.wdAdjustProportional);

                tblData.Columns[1].Shading.BackgroundPatternColor = WdColor.wdColorGray05;
                tblData.Columns[1].PreferredWidthType = WdPreferredWidthType.wdPreferredWidthAuto;
                tblData.Columns[3].Shading.BackgroundPatternColor = WdColor.wdColorGray05;
                tblData.Columns[3].PreferredWidthType = WdPreferredWidthType.wdPreferredWidthAuto;
                tblData.Columns[5].Shading.BackgroundPatternColor = WdColor.wdColorGray05;
                tblData.Columns[5].PreferredWidthType = WdPreferredWidthType.wdPreferredWidthAuto;
                tblData.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                tblData.Range.Rows[1].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalTop;
                for (int x = 1; x < tblData.Columns.Count; x += 2)
                    for (int y = 1; y <= tblData.Rows.Count; y++)
                        tblData.Cell(y, x).Range.Font.Bold = 1;

                tblData.Cell(1, 1).Range.Text = "User:";
                tblData.Cell(1, 2).Range.Text = UserName;
                tblData.Cell(2, 1).Range.Text = "Inspection Start:";
                tblData.Cell(2, 2).Range.Text = hvd.reportDateStart;
                tblData.Cell(3, 1).Range.Text = "Inspection Finish:";
                tblData.Cell(3, 2).Range.Text = hvd.reportDateFinish;
                tblData.Cell(4, 1).Range.Text = "Label Item";
                tblData.Cell(4, 2).Range.Text = LIN;
                string issue = DataManager.GetLabelIssue(LIN, LWO);
                issue = issue.Trim();
                if (issue != "")
                    issue = " Issue No: " + issue;
                tblData.Cell(4, 2).Range.Text = tblData.Cell(4, 2).Range.Text + issue;
                tblData.Cell(5, 1).Range.Text = "Customer:";
                tblData.Cell(5, 2).Range.Text = customer;
                tblData.Cell(6, 1).Range.Text = "Operator Summary:";
                tblData.Cell(6, 2).Range.Text = InspectionReport.OperatorSummary;

                tblData.Cell(1, 3).Range.Text = "Label Count:";
                tblData.Cell(1, 4).Range.Text = LABEL_COUNT.ToString();
                tblData.Cell(2, 3).Range.Text = "Total Inspected:";
                tblData.Cell(2, 4).Range.Text = labelCounts.CountInspected.ToString();
                tblData.Cell(3, 3).Range.Text = "Total Investigated:";
                tblData.Cell(3, 4).Range.Text = labelCounts.CountQueried.ToString();
                tblData.Cell(4, 3).Range.Text = "Total Accepted:";
                tblData.Cell(4, 4).Range.Text = labelCounts.CountAcceptedOp.ToString();
                tblData.Cell(5, 3).Range.Text = "Total Rejected:";
                tblData.Cell(5, 4).Range.Text = labelCounts.CountRejectedOp.ToString();
                tblData.Cell(6, 3).Range.Text = "Total Missing:";
                tblData.Cell(6, 4).Range.Text = labelCounts.CountMissing.ToString();
                tblData.Cell(1, 5).Range.Text = "Label Type:";
                tblData.Cell(1, 6).Range.Text = getLabelTypeAsString().ToUpper();
                tblData.Cell(2, 5).Range.Text = "Label Debris Size:";
                tblData.Cell(2, 6).Range.Text = getDebrisSizeLabel().ToUpper();
                tblData.Cell(3, 5).Range.Text = "Op-Zone Debris Size:";
                tblData.Cell(3, 6).Range.Text = getDebrisSize().ToUpper();
                tblData.Cell(4, 5).Range.Text = "Light Marks/Blemishes:";
                tblData.Cell(4, 6).Range.Text = labelCounts.InspectLightArea == true ? "ON" : "OFF";
                tblData.Cell(5, 5).Range.Text = "Station:";
                tblData.Cell(5, 6).Range.Text = stationID;
                tblData.Cell(6, 5).Range.Text = "Sample:";
                tblData.Cell(6, 6).Range.Text = labelCounts.HasSample == true ? "YES" : "NO";
                #endregion

                #region Legend Table
                // show legend and layout images
                object strt = tblData.Range.End;
                var rangeText = document.Range(ref strt, ref strt);
                Paragraph p2 = rangeText.Paragraphs.Add(ref oMissing);
                p2.Range.InsertAfter(""); // "Legend Data");
                p2.Range.Bold = 1;
                p2.Range.InsertParagraphAfter();
                Paragraph para4 = document.Content.Paragraphs.Add(ref oMissing);
                para4.Range.InsertParagraphAfter();
                Table tblDataLegend = document.Tables.Add(para4.Range, 3, 2, ref oMissing, ref oMissing);
                tblDataLegend.Borders.InsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleDot;
                tblDataLegend.Borders.OutsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
                tblDataLegend.Rows[1].HeadingFormat = -1;
                tblDataLegend.Rows[1].Shading.BackgroundPatternColor = WdColor.wdColorGray05;
                tblDataLegend.Rows[2].Height = 240f;

                tblDataLegend.Rows[2].Cells[1].TopPadding = 8f;
                tblDataLegend.Rows[2].Cells[1].BottomPadding = 8f;
                tblDataLegend.Rows[2].Cells[2].TopPadding = 8f;
                tblDataLegend.Rows[2].Cells[2].BottomPadding = 8f;
                tblDataLegend.Rows[2].Cells[1].VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                tblDataLegend.Rows[2].Cells[2].VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                tblDataLegend.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                tblDataLegend.Range.Rows[1].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalTop;
                for (int x = 1; x < tblDataLegend.Columns.Count; x++)
                    tblDataLegend.Cell(1, x).Range.Font.Bold = 1;

                tblDataLegend.Cell(1, 1).Range.Text = "Layout";
                tblDataLegend.Cell(1, 2).Range.Text = "Legend";

                string filePath = ImageData.CreateKeyImagesFilepath(LIN);
                filePath = Path.Combine(filePath, LIN + "_layout.png");
                if (System.IO.File.Exists(filePath))
                {
                    tblDataLegend.Cell(2, 1).Range.InlineShapes.AddPicture(filePath, ref oMissing, true, ref oMissing);
                }
                filePath = ImageData.CreateKeyImagesFilepath(LIN);
                filePath = Path.Combine(filePath, LIN + "_legendkey.png");
                if (System.IO.File.Exists(filePath))
                    tblDataLegend.Cell(2, 2).Range.InlineShapes.AddPicture(filePath, ref oMissing, true, ref oMissing);
                tblDataLegend.Cell(3, 1).Merge(tblDataLegend.Cell(3, 2));
                tblDataLegend.Cell(3, 1).Range.Text = "Colour Key  |  VDE: Red Blocks  |  OP-Zone: Cyan  |  Barcode: Magenta  |  Masking: Brick Red";

                #endregion

                #region Report Data
                                
                if (reportData.reportModel.Items.Count > 0)
                {
                    Paragraph para1 = document.Content.Paragraphs.Add(ref oMissing);
                    para1.Range.InsertParagraphAfter();
                    para1.Range.InsertBreak(WdBreakType.wdPageBreak);
                    Table tblReportData = document.Tables.Add(para1.Range, ((labelCounts.CountQueried + labelCounts.CountMissing) * 3), columnCount, ref oMissing, WdAutoFitBehavior.wdAutoFitFixed);

                    tblReportData.Rows[1].HeadingFormat = -1;
                    tblReportData.Borders.Enable = 1;
                    tblReportData.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    tblReportData.Cell(1, 1).Range.Text = "Backing\v#";
                    tblReportData.Cell(1, 2).Range.Text = "Med";
                    tblReportData.Cell(1, 3).Range.Text = "Accepted";
                    tblReportData.Cell(1, 4).Range.Text = "Missing";
                    tblReportData.Cell(1, 5).Range.Text = "Selected\vItems";
                    tblReportData.Cell(1, 6).Range.Text = "Operator Info";
                    tblReportData.Cell(1, 7).Range.Text = "Reason(s)";
                    tblReportData.Rows[1].Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                    tblReportData.Rows[1].Range.Font.Size = 13;
                    tblReportData.Rows[1].Range.Font.Bold = 1;
                    tblReportData.Columns[1].SetWidth(72f, WdRulerStyle.wdAdjustProportional);
                    tblReportData.Columns[2].SetWidth(72f, WdRulerStyle.wdAdjustProportional);
                    tblReportData.Columns[3].SetWidth(72f, WdRulerStyle.wdAdjustProportional);
                    tblReportData.Columns[4].SetWidth(82f, WdRulerStyle.wdAdjustProportional);
                    tblReportData.PreferredWidth = document.PageSetup.PageWidth - (document.PageSetup.LeftMargin + document.PageSetup.RightMargin);
                    int nextFail = -1;
                    for (int x = 2; x < tblReportData.Rows.Count + 1; x++)
                    {
                        nextFail += 1;
                        tblReportData.Rows[x].Range.Font.Name = "verdana";
                        tblReportData.Rows[x].Range.Font.Size = 10;
                        tblReportData.Rows[x].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                        tblReportData.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                        if (nextFail < reportData.reportModel.Items.Count)
                        {
                            if (PH != null)
                                try { PH(nextFail + 1); } catch { }

                            tblReportData.Cell(x, 1).Range.Text = reportData.reportModel.Items[nextFail].BackingNumber.ToString();
                            tblReportData.Cell(x, 2).Range.Text = reportData.reportModel.Items[nextFail].Med.ToString();
                            string userAccepted = "YES";
                            if (reportData.reportModel.Items[nextFail].USerAccepted == (int)UserAccepted.NO)
                                userAccepted = "NO";
                            if (reportData.reportModel.Items[nextFail].USerAccepted == (int)UserAccepted.NA)
                                userAccepted = "N/A";
                            tblReportData.Cell(x, 3).Range.Text = userAccepted;
                            tblReportData.Cell(x, 4).Range.Text = reportData.reportModel.Items[nextFail].MissingData.ToString();
                            tblReportData.Cell(x, 5).Range.Text = reportData.reportModel.Items[nextFail].OperatorCheckedItem.ToString();
                            tblReportData.Cell(x, 6).Range.Text = reportData.reportModel.Items[nextFail].OperatorText.ToString();
                            tblReportData.Cell(x, 7).Range.Text = reportData.reportModel.Items[nextFail].Reasons.ToString();

                            if (!reportData.reportModel.Items[nextFail].Missing)
                            {
                                x = x + 1;
                                tblReportData.Rows[x].HeightRule = WdRowHeightRule.wdRowHeightExactly;
                                tblReportData.Rows[x].Height = 160f;
                                List<string> filePaths = reportData.reportModel.Items[nextFail].FilePaths;
                                if (filePaths.Count < columnCount)
                                {
                                    while (tblReportData.Rows[x].Cells.Count > filePaths.Count)
                                        tblReportData.Rows[x].Cells[filePaths.Count].Merge(tblReportData.Rows[x].Cells[filePaths.Count + 1]);
                                    if (tblReportData.Rows[x].Cells.Count > 1)
                                        tblReportData.Rows[x].Cells.DistributeWidth();
                                }
                                else if (filePaths.Count > columnCount)
                                {
                                    while (tblReportData.Rows[x].Cells.Count < filePaths.Count)
                                        tblReportData.Rows[x].Cells[1].Split(1, 2);
                                    if (tblReportData.Rows[x].Cells.Count > 1)
                                        tblReportData.Rows[x].Cells.DistributeWidth();
                                }
                                for (int y = 1; y <= filePaths.Count; y++)
                                {
                                    string filepath = filePaths[y - 1];
                                    if (System.IO.File.Exists(filepath))
                                    {
                                        for (int i = 1; i <= tblReportData.Rows[x].Cells.Count; i++)
                                        {
                                            tblReportData.Cell(x, i).TopPadding = 8.6f;
                                            tblReportData.Cell(x, i).BottomPadding = 12.8f;
                                            tblReportData.Cell(x, i).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                                        }
                                        if (System.IO.File.Exists(filepath))
                                        {
                                            //tblReportData.Cell(x, y).Range.InlineShapes.AddPicture(filepath, ref oMissing, true, ref oMissing);
                                            var MediaPathImage1 = filepath;
                                            var rangePic = tblReportData.Cell(x, y).Range;
                                            InlineShape rangePicture = rangePic.InlineShapes.AddPicture(MediaPathImage1, Type.Missing, true, Type.Missing);
                                            rangePicture.Height = tblReportData.Cell(x, y).Height - (tblReportData.Cell(x, y).TopPadding + tblReportData.Cell(x, y).BottomPadding);
                                            //rangePicture.Width = imageWidth;
                                        }
                                    }
                                    else
                                        tblReportData.Cell(x, y).Range.Text = "no image";
                                }
                            }
                        }
                        else
                        {
                            // remove unused rows from bottom of table
                            int rowcount = tblReportData.Rows.Count;
                            while (rowcount >= x)
                            {
                                try { tblReportData.Rows[tblReportData.Rows.Count].Delete(); } catch { break; }
                                rowcount = tblReportData.Rows.Count;
                            }
                        }
                    }
                    #endregion

                    
                    strt = tblReportData.Range.End;
                }
                #region Training Data
                rangeText = document.Range(ref strt, ref strt);
                Paragraph p1 = rangeText.Paragraphs.Add(ref oMissing);
                p1.Range.InsertBreak(WdBreakType.wdPageBreak);
                p1.Range.InsertAfter("Training Data");
                p1.Range.Bold = 1;
                p1.Range.InsertParagraphAfter();
                Paragraph para3 = document.Content.Paragraphs.Add(ref oMissing);
                para3.Range.InsertParagraphAfter();
                Table tblTrainingData = document.Tables.Add(para3.Range, 2, 4, ref oMissing, ref oMissing);
                tblTrainingData.Borders.InsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleDot;
                tblTrainingData.Borders.OutsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
                tblTrainingData.Rows[1].HeadingFormat = -1;
                tblTrainingData.Rows[2].Range.Font.Size = 9;
                tblTrainingData.Rows[1].Shading.BackgroundPatternColor = WdColor.wdColorGray05;
                tblTrainingData.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                tblTrainingData.Range.Rows[1].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalTop;
                for (int x = 1; x < tblTrainingData.Columns.Count; x++)
                    tblTrainingData.Cell(1, x).Range.Font.Bold = 1;
                tblTrainingData.Cell(1, 1).Range.Text = "VDE";
                tblTrainingData.Cell(1, 2).Range.Text = "Op-Zones";
                tblTrainingData.Cell(1, 3).Range.Text = "Barcodes";
                tblTrainingData.Cell(1, 4).Range.Text = "Masks";
                //tblTrainingData.Cell(1, 5).Range.Text = "Label Background";

                List<string> dataList = DataManager.GetTrainingData(LIN, TrainingDataType.VDE);
                if (dataList != null)
                {
                    string text = formatList(dataList);
                    tblTrainingData.Cell(2, 1).Range.Text = text;
                }
                dataList = DataManager.GetTrainingData(LIN, TrainingDataType.OPZONE);
                if (dataList != null)
                {
                    string text = formatList(dataList);
                    tblTrainingData.Cell(2, 2).Range.Text = text;
                }
                dataList = DataManager.GetTrainingData(LIN, TrainingDataType.BARCODE);
                if (dataList != null)
                {
                    string text = formatList(dataList);
                    tblTrainingData.Cell(2, 3).Range.Text = text;
                }
                dataList = DataManager.GetTrainingData(LIN, TrainingDataType.MASK);
                if (dataList != null)
                {
                    string text = formatList(dataList);
                    tblTrainingData.Cell(2, 4).Range.Text = text;
                }
                #endregion

                #region Save & Cleanup
                //Save, delete word doc, save and open pdf
                DateTime dt = DateTime.Now;
                var dateStringFormat = dt.ToString("MM-dd-yyyy-HH-mm", CultureInfo.InvariantCulture);
                string partFileName = dateStringFormat.ToString();
                
                var dateStringFormat2 = dt.ToString("dd-MMM-yyyy-HH-mm", CultureInfo.InvariantCulture);
                string partFileName2 = dateStringFormat2.ToString();
                partFileName2 = dateStringFormat2.ToUpper();

                object filename = @"c:\tmp\" + partFileName + ".docx";
                object filenamePDF = "";

                try { if (File.Exists(filename.ToString())) File.Delete(filename.ToString()); } catch { }
                try { document.SaveAs2(ref filename); } catch { }

                filenamePDF = Path.Combine(Defaults.ReportPath, REEL_LPN + "___" + partFileName2 + "-StationID-" + stationID + ".pdf");
                //filenamePDF = Path.Combine(Defaults.ReportPath, REEL_LPN + "___" + partFileName2 + ".pdf");
                filenamePDF = "\\" + filenamePDF;
                try { if (File.Exists("\\" + filenamePDF.ToString())) File.Delete("\\" + filenamePDF.ToString()); } catch { }
                try { document.SaveAs("\\" + filenamePDF, WdSaveFormat.wdFormatPDF); } catch { }
                document.Close(ref oMissing, ref oMissing, ref oMissing);
                try { if (File.Exists(filename.ToString())) File.Delete(filename.ToString()); } catch { }

                document = null;
                WordApp.Quit(ref oMissing, ref oMissing, ref oMissing);
                WordApp = null;
                _ = new Process
                {
                    StartInfo = new ProcessStartInfo("\\" + filenamePDF)
                    {
                        UseShellExecute = true
                    }
                }.Start();                
                #endregion
            }
            catch (Exception ex)
            {
                retVal = false;
                MessageBox.Show("CreateDocument err: " + ex.Message);
            }
            finally
            {
                try { if (document != null) { document.Close(ref oMissing, ref oMissing, ref oMissing); document = null; } } catch { }
                try { if (WordApp != null) WordApp = null; } catch { }
            }
            return retVal;
        }


        private string formatList(List<string> datalist)
        {
            string text = "";
            try
            {
                text = String.Join("\v", datalist);
                text = text.Replace("\n", "");
                text = text.Replace("\r", "");
            }
            catch (Exception ex)
            {
                MessageBox.Show("formatList err: " + ex.Message);
                if (document != null)
                    WordApp.Visible = true;
            }
            return text;
        }

    }


    public class HeaderVariableData
    {
        public string operatorSummary = "";
        public DateTime[] dateTimes = null;
        public string reportDateStart = "";
        public string reportDateFinish = "";

        public HeaderVariableData(string reel, IDataManager DataManager)
        {
            operatorSummary = DataManager.GetReportSummary(reel);
            dateTimes = DataManager.GetReportSummaryDates(reel);
            if (dateTimes != null)
            {
                if (dateTimes[0] != null)
                    reportDateStart = DateFormats.LocalTimeAndZone(dateTimes[0], true);
                if (dateTimes[1] != null)
                    reportDateFinish = DateFormats.LocalTimeAndZone(dateTimes[1], true);
            }
        }
    }

    public class ReportModel
    {
        public string UserName { get; set; }
        public string ReelLPN { get; set; }
        public string LabelItem { get; set; }
        public string Customer { get; set; }
        public List<ReportItem> Items { get; set; }
    }

    public class ReportData
    {
        public static event ProgressHandler PH = null;
        private string LPN = "";
        private string LIN = "";
        public ReportModel reportModel = null;
        private static string FAIL_FOLDER = "";
        private string VaryingMedPH = "";
        private IDataManager DataManager;

        public ReportData(IDataManager dataManager)
        {
            DataManager = dataManager;
        }
        public void INIT(ProgressHandler ph, string lpn, string lin, string failfolder, string varyingMedPH )
        {
            VaryingMedPH = varyingMedPH;
            FAIL_FOLDER = failfolder;
            PH = ph;
            LPN = lpn;
            LIN = lin;
            this.reportModel = GetReportDetails();
        }

        public ReportModel GetReportDetails()
        {
            ReportModel retVal = null;
            List<ReportItem> items = new List<ReportItem>();
            List<LabelItemDataSetup> LIDs = new List<LabelItemDataSetup>();
            try
            {
                LIDs = DataManager.LabelDataItems(LIN, LPN);
                items = createItemsNoPassData(LIDs, VaryingMedPH);
                retVal = new ReportModel
                {
                    LabelItem = LIN,
                    ReelLPN = LPN,
                    Customer = DataManager.GetCustomer(LIN),
                    UserName = DataManager.GetReportCompiler(LPN),
                    Items = items
                };
            }
            catch (Exception ex)
            {
                string err = "GetReportDetails() err: " + ex.Message;
            }
            return retVal;
        }

        public static List<ReportItem> createItemsNoPassData(List<LabelItemDataSetup> lids, string varyingMedPH)
        {
            List<ReportItem> retVal = new List<ReportItem>();
            try
            {
                if (FailRecord.FailPipes.Count == 0)
                    return retVal;
                for (int idx = 0; idx < FailRecord.FailPipes.Count; idx++)
                {
                    var item = generateLUIItem(FailRecord.FailPipes[idx], lids, varyingMedPH);
                    retVal.Add(item);
                }
            }
            catch (Exception ex)
            {
                retVal = null;
                string err = "createItemsNoPassData() err: " + ex.Message;
            }
            return retVal;
        }

        private static ReportItem generateLUIItem(FailRecord fp, List<LabelItemDataSetup> lids, string varyingMedPH)
        {
            
            string med = "n/a";
            foreach (LabelItemDataSetup lid in lids)
            {                
                string tmp = lid.GetDataAtIndex(fp.LabelIndex - 1);
                if (lid.PLACE_HOLDER == varyingMedPH)
                {
                    med = tmp;
                    break;
                }


            }
            if (fp.MISSING)
            {
                return new ReportItem
                {
                    Missing = true,
                    Med = med,
                    BackingNumber = "",
                    Reasons = "Label Missing",
                    MissingData = getMissingLabel(ref fp),
                    OperatorCheckedItem = "Label Missing",
                    OperatorText = "",
                    USerAccepted = (int)UserAccepted.NA,
                    FilePaths = getFileNames(ref fp),
                    LabelIndex = fp.LabelIndex.ToString()
                };
            }
            else if (fp.SAMPLE)
            {
                return new ReportItem
                {
                    Missing = false,
                    Med = med,
                    BackingNumber = "-",
                    Reasons = getReasons(fp),
                    MissingData = "SAMPLE",
                    OperatorCheckedItem = "SAMPLE",
                    OperatorText = getOperatorText(fp),
                    USerAccepted = (int)UserAccepted.NA,
                    FilePaths = getFileNames(ref fp),
                    LabelIndex = fp.LabelIndex.ToString()
                };
            }
            else
            {
                return new ReportItem
                {
                    Missing = false,
                    Med = med,
                    BackingNumber = fp.BackingNumber,
                    Reasons = getReasons(fp),
                    MissingData = getMissingData(fp),
                    OperatorCheckedItem = getDecision(fp),
                    OperatorText = getOperatorText(fp),
                    USerAccepted = getUserAccepted(fp),
                    FilePaths = getFileNames(ref fp),
                    LabelIndex = fp.LabelIndex.ToString()
                };
            }
        }

        private static List<string> getFileNames(ref FailRecord fp)
        {
            List<string> retVal = new List<string>();
            foreach (RegionFailData rfd in fp.RegionFailDataList)
            {
                retVal.Add(rfd.Filename);
            }
            return retVal;
        }

        private static string getMissingLabel(ref FailRecord fp)
        {
            string retVal = "";
            try
            {
                retVal = fp.MissingData;
                retVal = retVal.Trim();
                retVal = retVal.Replace("\'", "");
            }
            catch { retVal = "MISSING LABEL"; }
            return retVal;
        }

        private static int getUserAccepted(FailRecord fp)
        {
            int retVal = (int)UserAccepted.NA;
            retVal = (fp.ACCEPTED_BY_USER == true ? 0 : 1);
            return retVal;
        }

        private static string getDecision(FailRecord fp)
        {
            string retVal = "";

            if (fp.CheckedItems != "")
                retVal = retVal + fp.CheckedItems + '\v';

            retVal = retVal.Trim();
            retVal = retVal.Replace("\'", "");
            return retVal;
        }

        private static string getOperatorText(FailRecord fp)
        {
            string retVal = "";

            if (fp.UserData != "")
                retVal = retVal + fp.UserData;

            retVal = retVal.Trim();
            retVal = retVal.Replace("\'", " ");
            return retVal;
        }

        private static string getMissingData(FailRecord fp)
        {
            string retVal = "";

            if (fp.DatasNotFound.Count > 0 && fp.MISSING == true)
            {
                for (int x = 0; x < fp.DatasNotFound.Count; x++)
                    retVal = retVal + fp.DatasNotFound[x] + '\v';
            }
            retVal = retVal.Replace("\'", "");
            retVal = retVal.Trim();
            return retVal;

        }

        private static string getReasons(FailRecord fp)
        {
            string retVal = "";

            if (fp.RegionFailDataList.Count > 0)
            {
                for (int x = 0; x < fp.RegionFailDataList.Count; x++)
                {
                    RegionFailData rfd = fp.RegionFailDataList[x];
                    if (rfd.Reasons.Count > 0)
                    {
                        foreach (string s in rfd.Reasons)
                            retVal = retVal + s + '\v';
                    }
                }
            }
            retVal = retVal.Trim();
            retVal = retVal.Replace("\'", "");
            return retVal;
        }
    }

    public class ReportItem
    {
        public bool Missing { get; set; }
        public string Med { get; set; }
        public string Reasons { get; set; }
        public string MissingData { get; set; }
        public string OperatorText { get; set; }
        public string OperatorCheckedItem { get; set; }
        public string BackingNumber { get; set; }
        public int USerAccepted { get; set; }
        public List<string> FilePaths { get; set; }
        public string LabelIndex { get; set; }
    }

    public static class PDFDocumentIO
    {
        private static string filePath = "";
        private static string LPN = "";

        public static bool FileStatus(string lpn)
        {
            bool retVal = true;                        
            LPN = lpn;
            filePath = ImageData.CreateReportFilepathAllMachines(lpn);
            if (filePath == "")
                return false;
            string err = "";
            pdfReadersOpen(out err);
            while (err != "")
            {
                pdfReadersOpen(out err);
                if (err != "")
                    MessageBox.Show(err);
            }
            if (retVal == false)
            {
                string tmpFileLocation = string.Format("PDF destination path cannot be accessed and/or an Adobe document reader is still open.");
                MessageBox.Show(tmpFileLocation);
            }            
            return retVal;
        }

        private static bool pdfReadersOpen(out string error)
        {
            bool retVal = true;
            System.Diagnostics.Process p;
            try
            {
                error = "";
                if (filePath == "")
                {
                    error = "The report destination folder can not be accessed";
                    return false;
                }
                System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcesses(Environment.MachineName);
                for (int x = 0; x < processes.Length; x++)
                {
                    if (processes[x].MainWindowTitle.ToString() != "")
                    {
                        if (processes[x].MainWindowTitle.ToString().ToUpper().Contains(LPN) || processes[x].MainWindowTitle.ToString().ToLower().Contains("adobe"))
                        {
                            p = processes[x];
                            if (p.HasExited == false)
                            {
                                error = "Please close all PDF documents and pdf readers that are open.";
                                retVal = false;
                                break;
                            }
                        }
                    }
                }
                try { System.IO.File.Delete(filePath); } catch { }
            }
            catch (Exception ex)
            {
                error = "pdfReadersOpen() err: " + ex.Message;
                retVal = false;
            }
            return retVal;
        }
    }
}
