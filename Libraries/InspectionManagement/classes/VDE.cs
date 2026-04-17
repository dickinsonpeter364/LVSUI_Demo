using CONSTANTS;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static LVS3.Delegates;
using static LVS3.Enums;

namespace LVS3
{
    public class VDE
    {
        private double ConfidenceLevel = DataManager.GetConfidenceLevel();
        private Action pauseMethodCaller = null;
        public static event ErrorMethodHandler EMH = null;
        public bool CountMatchError = false;
        public bool ErrorOccured = false;
        public string ErrorString = "";
        public int CharacterIndex = 0;
        public int WordLength = 0;
        private uscMessageDisplay uscMD = null;
        private List<OPZoneData> OPZoneItems = new List<OPZoneData>();
        private List<VDEItem> VDEItems = new List<VDEItem>();
        private List<MedData> MedDataItems = new List<MedData>();

        public VDE(List<OPZoneData> opzoneitems, List<VDEItem> vdeitems, List<MedData> meddataitems, uscMessageDisplay uscmd, Action pausemethodcaller)
        {
            this.pauseMethodCaller = pausemethodcaller;
            EMH -= LVS3.frmInspect.EMD;
            EMH += LVS3.frmInspect.EMD;
            OPZoneItems = opzoneitems;
            VDEItems = vdeitems;
            MedDataItems = meddataitems;
            this.uscMD = uscmd;
            CountMatchError = false;
        }

        public int RotateAdjustment(int valuerotated)
        {
            int angle = 0;
            if (valuerotated == 0)
                angle = 90;
            else if (valuerotated == 90)
                angle = 180;
            else if (valuerotated == 180)
                angle = 270;
            else if (valuerotated == 270)
                angle = 360;
            return angle;
        }

        public bool FindVDE(int rotatedangle, int angle, ref HObject img, ref FailRecord fp, OPZoneData ozd, List<VDEItem> vdeitems, string variableMedDataPH)
        {
            bool retVal = true;            
            HObject region = null, tmpChar = null, imgCHARS = null, characters = null, imgRotated = null;
            HObject rgnComplement = null, imgReduced = null;
            HTuple srR1, srCol1, srR2, srCol2, matchedItem = null, word = null, wordscore = null;
            string match = "";

            try
            {
                HOperatorSet.GenEmptyObj(out imgRotated);
                rotateBackground(rotatedangle, ref img, ref imgRotated);
                HOperatorSet.SmallestRectangle1(ozd.region, out HTuple r1, out HTuple c1, out HTuple r2, out HTuple c2);
                int[] rgnCoords = new int[4];

                rgnCoords[0] = Convert.ToInt32(r1.D);
                rgnCoords[1] = Convert.ToInt32(c1.D);
                rgnCoords[2] = Convert.ToInt32(r2.D);
                rgnCoords[3] = Convert.ToInt32(c2.D);
                HOperatorSet.GetImageSize(img, out HTuple w, out HTuple h);
                foreach (VDEItem vdi in vdeitems)
                {
                    if (SYSTEM_IO.PROCESSING == false)
                        return true;
                    CountMatchError = false;
                    match = "";
                    if (vdi.FontName == "") 
                        continue;
                    if (vdi.VDERotatedAngle != angle) continue;
                    if (imgCHARS != null) imgCHARS.Dispose();
                    if (characters != null) characters.Dispose();
                    if (region != null) region.Dispose();
                    if (tmpChar != null) tmpChar.Dispose();
                    if (rgnComplement != null) rgnComplement.Dispose();
                    if (imgReduced != null) imgReduced.Dispose();
                    foreach (MedData medData in MedDataItems)
                        if (medData.PlaceHolder == vdi.Placeholder)
                        {
                            match = medData.Data;
                            vdi.RepeatType = medData.Repeat;
                            break;
                        }
                    if (match == "")
                        continue;
                    HTuple matchData = new HTuple(match);

                    //mxClient.Stop(1);
                    
                    word = "";
                    wordscore = 0;
                    int[] textCoords = VDEItem.RotateClockwiseCoords(angle, w, h, vdi.VDERegion);
                    double[] confidences = new double[match.Length];
                    ErrorOccured = false;
                    vdi.DarkMaxGray = ozd.MAX_GRAY;

                    //Maurice 03-MAY-2024 Error Fix - Below conditional statement was inhibiting the call to getText, producing a continuous Halcon Error
                    //if (match == "KB_P03666")
                    //Maurice 03-MAY-2024 Halcon Error Fix End

                    if (getText(ref characters, ref word, ref wordscore, matchData, ref imgRotated, textCoords, vdi, ozd.NAME, ref fp) == false)
                    {
                        if (vdi.Placeholder == variableMedDataPH)
                        {
                            INSPECTION.medFailOnCurrentLabel = true;
                            INSPECTION.medFailInCurrentPatient = true;
                        }

                        if (SYSTEM_IO.PROCESSING == false)
                            return true;

                        if (ErrorOccured)
                            throw new Exception(ErrorString);
                        fp.VALID_LABEL = false;
                        RegionFailData rfd = new RegionFailData(vdi.OpZoneName);
                        if (rfd.Img == null)
                            HOperatorSet.GenEmptyObj(out rfd.Img);
                        HOperatorSet.CopyObj(img, out rfd.Img, 1, 1);
                        fp.RegionFailDataList.Add(rfd);
                        rfd.Reasons.Add(string.Format("VDE {0} in {1}", match, vdi.OpZoneName));

                        if (CountMatchError == true)
                            rfd.Reasons.Add(string.Format("Check VDE text {0} in {1} for marks or missing/patchy text", match, vdi.OpZoneName));
                        else if (word.Length == 0)
                        {
                            rfd.Reasons.Add(string.Format("searching for {0} match", vdi.Placeholder));
                            rfd.Reasons.Add(string.Format("VDE {0} not found in {1}", match, ozd.NAME));
                        }
                        else
                        {

                            string med = "";
                            foreach (MedData md in MedDataItems)
                                if (md.Data == matchData)
                                {
                                    med = md.Data + "|" + md.PlaceHolder + "|" + word.S;
                                    break;
                                }

                            fp.DatasNotFound.Add(med);
                            rfd.Reasons.Add("Data mis-match: " + match + ". Data found: " + word.S);
                        }
                        rfd.failCoordsList.Add(new RegionCoordPoints(new int[] { rgnCoords[0], rgnCoords[1], rgnCoords[2], rgnCoords[3] }));
                        HTuple top = vdi.VDERegion[0] - 4;
                        HTuple left = vdi.VDERegion[1] - 8;
                        HTuple bottom = vdi.VDERegion[2] + 4;
                        HTuple right = vdi.VDERegion[3] + 16;
                        if (top < 0)
                            top = 0;
                        if (left < 0)
                            left = 0;
                        HOperatorSet.GenRectangle1(out region, top, left, bottom, right);
                        HOperatorSet.ReduceDomain(img, region, out imgReduced);
                        rfd.failCoordsList.Add(new RegionCoordPoints(new int[] { vdi.VDERegion[0], vdi.VDERegion[1], vdi.VDERegion[2], vdi.VDERegion[3] }));
                        if (vdi.RepeatType > 0)
                        {
                            HOperatorSet.Complement(region, out rgnComplement);
                            HOperatorSet.ReduceDomain(img, rgnComplement, out img);
                        }
                        retVal = false;
                        continue;
                    }
                    if (SYSTEM_IO.PROCESSING == false)
                        return true; 

                    
                    HOperatorSet.SmallestRectangle1(characters, out srR1, out srCol1, out srR2, out srCol2);

                    string resultText = word.S;
                    string tmpResult = resultText;
                    HOperatorSet.TupleRegexpMatch(tmpResult, matchData.TupleConcat("ignore_case"), out matchedItem);
                    if (matchedItem.Length == 0)
                    {
                        string result = "";
                        try { result = word.S; } catch { }

                        fp.VALID_LABEL = false;
                        RegionFailData rfd = new RegionFailData(ozd.NAME);
                        if (rfd.Img == null)
                            HOperatorSet.GenEmptyObj(out rfd.Img);
                        HOperatorSet.CopyObj(img, out rfd.Img, 1, 1);
                        rfd.Reasons.Add(string.Format(match + " with " + vdi.Placeholder + " {0} does not contain the expected VDE in {1}", result, ozd.NAME));
                        fp.RegionFailDataList.Add(rfd);

                        fp.DatasNotFound.Add(string.Format(match + " with " + vdi.Placeholder + " {0} does not contain the expected VDE in {1}", result, ozd.NAME));
                        fp.DATA_FOUND = false;

                        HTuple topV = Convert.ToInt32(vdi.VDERegion[0] - 4);
                        HTuple leftV = Convert.ToInt32(vdi.VDERegion[1] - 4);
                        HTuple bottomV = Convert.ToInt32(vdi.VDERegion[2] + 4);
                        HTuple rightV = Convert.ToInt32(vdi.VDERegion[3] + 4);
                        int top = Convert.ToInt32(textCoords[0] - 2);
                        int left = Convert.ToInt32(textCoords[1] - 2);
                        int bottom = Convert.ToInt32(textCoords[2] + 2);
                        int right = Convert.ToInt32(textCoords[3] + 2);

                        if (top < 0)
                            top = 0;
                        if (left < 0)
                            left = 0;
                        if (region != null)
                            region.Dispose();

                        rfd.failCoordsList.Add(new RegionCoordPoints(new int[] { top - 2, left - 2, bottom + 2, right + 2 }));
                        HOperatorSet.GenRectangle1(out region, topV, leftV, bottomV, rightV);
                        rfd.failCoordsList.Add(new RegionCoordPoints(new int[] { rgnCoords[0], rgnCoords[1], rgnCoords[2], rgnCoords[3] }));
                        if (vdi.RepeatType > 0)
                        {
                            HOperatorSet.Complement(region, out rgnComplement);
                            HOperatorSet.ReduceDomain(img, rgnComplement, out img);
                        }
                        retVal = false;
                        continue;
                    }
                    if (SYSTEM_IO.PROCESSING == false)
                        return true;
                    if (vdi.RepeatType > 0)
                        maskComplementVDE(matchedItem.S, resultText, ref characters, angle, w, h, vdi, ref img);
                }
            }
            catch (Exception ex)
            {
                //mxClient.Stop(1);
                if (SYSTEM_IO.PROCESSING == false)
                    return true;
                //SYSTEM_IO.PROCESSING = false;
                retVal = false;
                string err = "findVDE() err: " + ex.Message;                
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Label Inspection", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);                
                //EMH?.Invoke(err);
            }
            finally
            {
                if (imgReduced != null)
                    imgReduced.Dispose();
                if (rgnComplement != null)
                    rgnComplement.Dispose();
                if (tmpChar != null)
                    tmpChar.Dispose();
                if (region != null)
                    region.Dispose();
                if (imgCHARS != null)
                    imgCHARS.Dispose();
                if (characters != null)
                    characters.Dispose();
                if (imgRotated != null)
                    imgRotated.Dispose();
            }
            return retVal;
        }

        private bool maskComplementVDE(string matcheditem, string resultText, ref HObject characters, int angle, int w, int h, VDEItem vde, ref HObject img)
        {
            bool retVal = true;
            HObject region = null, mask = null, rgnComplement = null;
            int firstChar = 0;
            int lastChar = 0;
            string matchFound = matcheditem;
            string res = matcheditem;
            try
            {
                while (firstChar >= 0)
                {
                    firstChar = (resultText.IndexOf(matchFound, firstChar));
                    if (firstChar == -1)
                        break;
                    if (rgnComplement != null)
                        rgnComplement.Dispose();
                    rgnComplement = null;
                    if (region != null)
                        region.Dispose();
                    region = null;
                    if (mask != null)
                        mask.Dispose();
                    mask = null;
                    lastChar = matchFound.Length;

                    HOperatorSet.CopyObj(characters, out mask, firstChar + 1, lastChar);
                    HOperatorSet.CountObj(mask, out HTuple numChars);
                    int top = 500000;
                    int left = 500000;
                    int bottom = 0;
                    int right = 0;
                    for (int x = 1; x <= numChars; x++)
                    {
                        HOperatorSet.SmallestRectangle1(mask[x], out HTuple t, out HTuple l, out HTuple b, out HTuple r);
                        if (t < top)
                            top = t;
                        if (l < left)
                            left = l;
                        if (b > bottom)
                            bottom = b;
                        if (r > right)
                            right = r;
                    }
                    int[] maskCoord = new int[] { top, left, bottom, right };
                    int ROW1 = Convert.ToInt32(maskCoord[0]) - 16;
                    int COL1 = Convert.ToInt32(maskCoord[1]) - 16;
                    int ROW2 = Convert.ToInt32(maskCoord[2]) + 16;
                    int COL2 = Convert.ToInt32(maskCoord[3]) + 16;
                    if (ROW1 < 0)
                        ROW1 = 0;
                    if (COL1 < 0)
                        COL1 = 0;
                    maskCoord[0] = ROW1;
                    maskCoord[1] = COL1;
                    maskCoord[2] = ROW2;
                    maskCoord[3] = COL2;
                    int[] rotateCoords = VDEItem.RotateAntiClockwiseCoords(angle, w, h, maskCoord);
                    HOperatorSet.GenRectangle1(out region, rotateCoords[0], rotateCoords[1], rotateCoords[2], rotateCoords[3]);
                    if (vde.RepeatType > 0)
                    {
                        HOperatorSet.Complement(region, out rgnComplement);
                        HOperatorSet.ReduceDomain(img, rgnComplement, out img);
                    }
                    if (rgnComplement != null)
                        rgnComplement.Dispose();
                    if (region != null)
                        region.Dispose();
                    if (firstChar < resultText.Length + matchFound.Length)
                        firstChar += matchFound.Length;
                    else
                        break;
                }
            }
            catch (Exception ex)
            {
                string err = "maskComplementVDE() err: " + ex.Message;
                if (SYSTEM_IO.PROCESSING == false)
                    return retVal;
                SYSTEM_IO.PROCESSING = false;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Label Inspection", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
            }
            finally
            {
                if (rgnComplement != null)
                    rgnComplement.Dispose();
                if (region != null)
                    region.Dispose();
                if (mask != null)
                    mask.Dispose();
            }
            return retVal;
        }

        private bool getText(ref HObject characters, ref HTuple word, ref HTuple wordscore, string matchdata, ref HObject img, int[] region, VDEItem vdi, string opzname, ref FailRecord fp)
        {
            HObject imgReduced = null, rgnZone = null, imgForeground = null, connectedChars = null, connectedRgnChars = null, character = null;
            bool retVal = true;
            HTuple textresultid = null, classVal = null, confidence = null;
            try
            {
                int additionalChars = 0;
                int paddingVDEBox = 5;
                ErrorString = "";
                ErrorOccured = false;
                word = new HTuple();
                wordscore = new HTuple();
                HOperatorSet.GenEmptyObj(out characters);
                HOperatorSet.GetImageSize(img, out HTuple w, out HTuple h);
                HTuple top = region[0] - paddingVDEBox * 2;
                HTuple left = region[1] - paddingVDEBox * 3;
                HTuple bottom = region[2] + paddingVDEBox * 2;
                HTuple right = region[3] + paddingVDEBox * 3;
                if (left < 0)
                    left = 0;
                if (top < 0)
                    top = 0;

                //mxClient.Stop(1);

                HOperatorSet.GenRectangle1(out rgnZone, top, left, bottom, right);
                HOperatorSet.ReduceDomain(img, rgnZone, out imgReduced);
                HOperatorSet.SegmentCharacters(rgnZone, imgReduced, out imgForeground, out characters, "local_contrast_best", "false", "false", vdi.tr.StrokeWidth, vdi.tr.CharWidth, vdi.tr.CharHeight, 0, vdi.tr.SegmentContrast, out HTuple usedThreshold);
                HOperatorSet.SelectCharacters(characters, out connectedChars, "false", vdi.tr.StrokeWidth, vdi.tr.CharWidth, vdi.tr.CharHeight, "true", "false", vdi.tr.PartitionMethod, "false", "medium", "false", 20, "completion");
                characters.Dispose();
                HOperatorSet.Connection(connectedChars, out connectedRgnChars);
                HOperatorSet.SelectShape(connectedRgnChars, out characters, "area", "and", 100, 99999);
                HOperatorSet.DoOcrWordMlp(characters, imgReduced, vdi.tr.ocrHandle, matchdata.ToString(), 1, 0, out classVal, out confidence, out word, out wordscore);                
                
                string sWord = "";
                string sWordcopy = "";
                if (classVal.Length > 0)
                {
                    for (int x = 0; x < classVal.Length; x++)
                        sWord = sWord + classVal[x].S;
                    word = sWord;
                    sWordcopy = sWord;
                }
                else
                {
                    WordLength = 0;
                    return false;
                }

                int tempInt = 0;                

                if (tempInt == 0)
                {
                    string sVDE = matchdata;

                    string newString = "";

                    if (matchdata != sWord) 
                    {
                        if (matchdata.Length >= sWord.Length) 
                        {
                            int index = 0;

                            foreach (char c in matchdata)   
                            {
                                if (char.IsLetterOrDigit(c))
                                {
                                    newString = newString + "A";                     
                                }
                                else
                                {
                                    newString = newString + c;         
                                }
                            }

                            foreach (char c in sWord) 
                            {
                                if (char.IsLetterOrDigit(c))
                                {
                                    if (newString[index].ToString() == "A")
                                    {
                                        newString = newString.Substring(0, index) + c + newString.Substring(index + 1);
                                        if (newString[index] != matchdata[index])
                                        {
                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        index++;
                                        additionalChars++;
                                        if (newString[index].ToString() == "A")
                                        {
                                            newString = newString.Substring(0, index) + c + newString.Substring(index + 1);
                                            if (newString[index] != matchdata[index])
                                            {
                                                return false;
                                            }
                                        }
                                        else
                                        {
                                            index++;
                                            additionalChars++;
                                            if (newString[index].ToString() == "A")
                                            {
                                                newString = newString.Substring(0, index) + c + newString.Substring(index + 1);
                                                if (newString[index] != matchdata[index])
                                                {
                                                    return false;
                                                }
                                            }
                                        }
                                    }
                                }
                                index++;
                            }
                        }
                        else
                        {
                            return false;
                        }
                        sWord = newString;
                    }


                    if (!matchdata.Contains(".") && sWord.Contains("."))
                    {
                        sWord = sWord.Replace(".", "");
                        word = "";
                        word = sWord;
                    }
                    if (!matchdata.Contains("O") && sWord.Contains("O"))
                    {
                        sWord = sWord.Replace("O", "0");
                        word = "";
                        word = sWord;
                    }
                    if (!matchdata.Contains("o") && sWord.Contains("o"))
                    {
                        sWord = sWord.Replace("o", "0");
                        word = "";
                        word = sWord;
                    }
                    if (!sWord.ToLower().Trim().Contains(matchdata.ToLower().Trim()))
                    {
                        WordLength = 0;
                        return false;
                    }
                    else if (word.Length == 0)
                    {
                        WordLength = 0;
                        return false;
                    }
                }

                if (tempInt > 10)
                {
                    if (matchdata != sWord)
                    {
                        string newSWord = sWord;
                        string combinedString = matchdata + sWord;
                        string charDifferences = "";
                        for (int i = 0; i < combinedString.Length + 1; i++)
                        {
                            if (matchdata.Contains(combinedString[0]) && sWord.Contains(combinedString[0]))
                            {
                                try
                                {
                                    if ((matchdata.Count(c1 => c1 == combinedString[0]) - sWord.Count(c2 => c2 == combinedString[0])) == 0)
                                    {
                                        string testStringMD = matchdata;
                                        string testStringSW = sWord;
                                        int loopCount = matchdata.Count(c1 => c1 == combinedString[0]);
                                        int indexPositionMD = 0;
                                        int sumIndexMD = 0;
                                        int indexPositionSW = 0;
                                        int sumIndexSW = 0;
                                        for (int j = 0; j < loopCount; j++)
                                        {
                                            indexPositionMD = testStringMD.IndexOf(combinedString[0]);
                                            sumIndexMD = sumIndexMD + indexPositionMD;
                                            testStringMD = testStringMD.Substring(0, indexPositionMD) + testStringMD.Substring(indexPositionMD);

                                            indexPositionSW = testStringSW.IndexOf(combinedString[0]);
                                            sumIndexSW = sumIndexSW + indexPositionSW;
                                            testStringSW = testStringSW.Substring(0, indexPositionSW) + testStringSW.Substring(indexPositionSW);
                                        }
                                        if (sumIndexMD == sumIndexSW)
                                        {
                                            combinedString = combinedString.Replace(combinedString[0].ToString(), "");
                                        }
                                        else
                                        {
                                            charDifferences = charDifferences + combinedString[0];                        // Add char to differences string
                                            combinedString = combinedString.Replace(combinedString[0].ToString(), "");    // Remove char from combined string
                                        }
                                    }
                                }
                                catch
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                charDifferences = charDifferences + combinedString[0];                            // Add char to differences string
                                combinedString = combinedString.Replace(combinedString[0].ToString(), "");        // Remove char from combined string
                            }
                            i = 0;
                        }

                        int stringMaxLength = 10;
                        for (int i = 0; i < stringMaxLength; i++)
                        {
                            bool iMinus = false;
                            if (matchdata[i] != newSWord[i])
                            {
                                if (charDifferences.Contains(matchdata[i]) || charDifferences.Contains(newSWord[i]))
                                {
                                    if (charDifferences.Contains(matchdata[i]) && charDifferences.Contains(newSWord[i]))
                                    {
                                        iMinus = true;
                                    }
                                    if (matchdata[i] == ' ' && newSWord[i] != ' ')
                                    {
                                        newSWord = newSWord.Substring(0, i) + " " + newSWord.Substring(i);
                                    }
                                    else if (matchdata[i] != ' ' && newSWord[i] == ' ')
                                    {
                                        newSWord = newSWord.Substring(0, i) + newSWord.Substring(i + 1);
                                    }
                                    else if (matchdata[i] == '_' && newSWord[i] != '_')
                                    {
                                        newSWord = newSWord.Substring(0, i) + "_" + newSWord.Substring(i);
                                    }
                                    else if (matchdata[i] != '_' && newSWord[i] == '_')
                                    {
                                        newSWord = newSWord.Substring(0, i) + newSWord.Substring(i + 1);
                                    }
                                    else if (matchdata[i] == '-' && newSWord[i] != '-')
                                    {
                                        newSWord = newSWord.Substring(0, i) + "-" + newSWord.Substring(i);
                                    }
                                    else if (matchdata[i] != '-' && newSWord[i] == '-')
                                    {
                                        newSWord = newSWord.Substring(0, i) + newSWord.Substring(i + 1);
                                    }
                                    else if (matchdata[i] == '/' && newSWord[i] != '/')
                                    {
                                        newSWord = newSWord.Substring(0, i) + "/" + newSWord.Substring(i);
                                    }
                                    else if (matchdata[i] != '/' && newSWord[i] == '/')
                                    {
                                        newSWord = newSWord.Substring(0, i) + newSWord.Substring(i + 1);
                                    }
                                    else if (newSWord[i] == '0' || newSWord[i] == 'o' || newSWord[i] == 'O')
                                    {
                                        if (matchdata[i] == '0' || matchdata[i] == 'o' || matchdata[i] == 'O')
                                        {
                                            newSWord = newSWord.Substring(0, i) + matchdata[i].ToString() + newSWord.Substring(i + 1);
                                        }
                                    }
                                    else
                                    {
                                        //WordLength = 0;
                                        return false;
                                    }
                                }
                                else
                                {
                                    //WordLength = 0;
                                    return false;
                                }
                            }
                            if (iMinus == true) { i--; }
                            if (matchdata.Length > newSWord.Length) stringMaxLength = matchdata.Length;
                            else stringMaxLength = newSWord.Length;
                        }

                        additionalChars = newSWord.Length - sWord.Length;

                        sWord = newSWord;

                        if (!sWord.ToLower().Trim().Contains(matchdata.ToLower().Trim()))
                        {
                            WordLength = 0;
                            return false;
                        }
                        else if (word.Length == 0)
                        {
                            WordLength = 0;
                            return false;
                        }
                    }
                }
                
                HTuple numChars = characters.CountObj();
                int tempNumChars = numChars + additionalChars;
                if (tempNumChars != matchdata.Length)
                {
                    fp.ConfidenceLevel = 0;
                    RegionFailData rfd = new RegionFailData(opzname);
                    HOperatorSet.SmallestRectangle1(rgnZone, out top, out left, out bottom, out right);
                    rfd.failCoordsList.Add(new RegionCoordPoints(new int[] { top, left, bottom, right }));
                    rfd.Reasons.Add(string.Format("The VDE found {0} in Opzone {1} does not match the expected value: {2}", sWord, opzname, matchdata));
                    if (rfd.Img == null)
                        HOperatorSet.GenEmptyObj(out rfd.Img);
                    HOperatorSet.CopyObj(img, out rfd.Img, 1, 1);
                    fp.RegionFailDataList.Add(rfd);
                    retVal = false;
                    return retVal;
                }

                #region Confidence Rating
                if (confidence.Length > 0)
                {
                    string temp1 = sWord;
                    string temp2 = matchdata;

                    temp1 = sWord;

                    

                    RegionFailData rfd = null;
                    double confidenceLevel = 0;
                    bool confident = true;
                    double AlternateConfidenceLevel = 0.75;

                    HTuple newconfidence = new HTuple();
                    int indexAdjust = 0;
                    try
                    {
                        for (int x = 0; x < matchdata.Length; x++)
                        {
                            try
                            {
                                if (matchdata[x] == sWordcopy[x - indexAdjust])
                                {
                                    try
                                    {                                        
                                        newconfidence.Append(confidence[x - indexAdjust].D);                                        
                                    }
                                    catch
                                    {
                                        
                                    }


                                }
                                else
                                {
                                    newconfidence.Append(1.0);
                                    indexAdjust++;
                                }
                            }
                            catch
                            {
                                
                            }

                        }
                    }
                    catch
                    {

                    }                    

                    for (int x = 0; x < confidence.Length; x++)
                    {
                        if (character != null)
                            character.Dispose();
                        confidenceLevel = confidence[x].D;
                        if (confidenceLevel < AlternateConfidenceLevel)
                        {
                            if (isAlphaNumeric(classVal[x].S))
                            {
                                confident = false;
                                fp.ConfidenceLevel = confidenceLevel;
                                rfd = new RegionFailData(opzname);
                                HOperatorSet.CopyObj(characters, out character, x + 1, 1);
                                HOperatorSet.SmallestRectangle1(character, out top, out left, out bottom, out right);
                                rfd.failCoordsList.Add(new RegionCoordPoints(new int[] { top, left, bottom, right }));
                                rfd.Reasons.Add(string.Format("Confidence rating is {0} for character '{1}' (at position {2}), in {3}: low level is {4}. Opzone {5}", Math.Round(fp.ConfidenceLevel, 4), matchdata[x], x + 1, matchdata, AlternateConfidenceLevel, opzname));
                                fp.RegionFailDataList.Add(rfd);
                            }
                        }
                    }
                    if (confident == false)
                    {
                        if (rfd.Img == null)
                            HOperatorSet.GenEmptyObj(out rfd.Img);
                        HOperatorSet.CopyObj(imgReduced, out rfd.Img, 1, 1);
                    }

                    #endregion

                    word = sWord;

                    if (UtilityFunctions.MSERCheckVDE(ref imgReduced, matchdata.Length, vdi.DarkMaxGray, ref CountMatchError, ref ErrorString, ref ErrorOccured, ref characters) == true)
                    {
                        if (CountMatchError)
                        {
                            retVal = false;
                            return retVal;
                        }
                    }
                }
                else
                {
                    fp.ConfidenceLevel = 0;
                    RegionFailData rfd = new RegionFailData(opzname);
                    HOperatorSet.SmallestRectangle1(rgnZone, out top, out left, out bottom, out right);
                    rfd.failCoordsList.Add(new RegionCoordPoints(new int[] { top, left, bottom, right }));
                    rfd.Reasons.Add(string.Format("VDE {0} in Opzone {1} was not found (Confidence: 0.0)", matchdata, opzname));
                    fp.RegionFailDataList.Add(rfd);
                    retVal = false;
                    return retVal;
                }

            }
            catch (Exception ex)
            {
                retVal = false;
                ErrorOccured = true;
                ErrorString = "Label index: " + fp.LabelIndex.ToString() + ", Med: " + fp.MED_ID + " getText() err: " + ex.Message;                
            }
            finally
            {
                if (character != null) character.Dispose();
                if (connectedRgnChars != null) connectedRgnChars.Dispose();
                if (connectedChars != null) connectedChars.Dispose();
                if (imgForeground != null) imgForeground.Dispose();
                if (imgReduced != null) imgReduced.Dispose();
                if (rgnZone != null) rgnZone.Dispose();
                if (textresultid != null)
                    try { HOperatorSet.ClearTextResult(textresultid); } catch { }
            }
            return retVal;
        }

        private bool isAlphaNumeric(string character)
        {
            Regex rg = new Regex(@"^[a-zA-Z0-9\s,]*$");
            return rg.IsMatch(character);
        }

        private bool rotateBackground(int angle, ref HObject imgIn, ref HObject imgOut)
        {
            bool retVal = false;
            try
            {
                if (imgOut != null)
                    imgOut.Dispose();
                HOperatorSet.RotateImage(imgIn, out imgOut, angle * -1, "constant");
                retVal = true;
            }
            catch (Exception ex)
            {
                //pauseMethodCaller.Invoke();
                if (SYSTEM_IO.PROCESSING == false)
                    return true;
                SYSTEM_IO.PROCESSING = false;
                retVal = false;
                string err = "rotateBackground() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Label Inspection", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
            }
            return retVal;
        }
    }

    public class VDEReelConfig
    {
        public static string REEL_LPN = "";
        public static string LWO;
        public static string LabelItem = "";
        public string InspectionComments;
        public string LabelController;
        public static List<VDEItem> VDEItems = new List<VDEItem>();


        public static bool HasVDE()
        {
            bool retVal = false;
            try
            {
                for (int x = 0; x < VDEReelConfig.VDEItems.Count; x++)
                    if (VDEReelConfig.VDEItems[x] != null)
                    {
                        if (VDEReelConfig.VDEItems[x].IsVDE == true)
                        {
                            retVal = true;
                            return retVal;
                        }
                    }
            }
            catch (Exception ex)
            {
                string err = "HasVDE() err: " + ex.Message;
                MessageBox.Show(err);
                retVal = false;
            }
            return retVal;
        }


        public static bool AddManualVDE(MouseEventArgs e)
        {
            bool retVal = true;
            try
            {
                VDEReelConfig.VDEItems.Add(null);
            }
            catch (Exception ex)
            {
                string err = "AddManualVDE() err: " + ex.Message;
                MessageBox.Show(err);
                retVal = false;
            }
            return retVal;
        }

        public static bool RemoveAllVDE()
        {
            bool retVal = true;
            try
            {
                for (int x = VDEReelConfig.VDEItems.Count - 1; x >= 0; x--)
                    if (VDEReelConfig.VDEItems[x] != null)
                    {
                        if (VDEReelConfig.VDEItems[x].IsVDE == true)
                        {
                            try { VDEReelConfig.VDEItems[x].DestroyVars(); } catch { }
                            VDEReelConfig.VDEItems.RemoveAt(x);
                        }
                    }
            }
            catch (Exception ex)
            {
                string err = "RemoveAllVDE() err: " + ex.Message;
                MessageBox.Show(err);
                retVal = false;
            }
            return retVal;
        }

        public static bool RemoveVDEFromZone()
        {
            bool retVal = true;
            try
            {
                frmOpZones frmO = new frmOpZones(VDEReelConfig.VDEItems);
                frmO.ShowDialog();
                if (frmO.OpZoneName != "")
                {
                    for (int x = VDEReelConfig.VDEItems.Count - 1; x >= 0; x--)
                        if (VDEReelConfig.VDEItems[x] != null)
                        {
                            if (VDEReelConfig.VDEItems[x].OpZoneName.ToLower() == frmO.OpZoneName.ToLower() && VDEReelConfig.VDEItems[x].IsVDE == true)
                            {
                                try { VDEReelConfig.VDEItems[x].DestroyVars(); } catch { }
                                VDEReelConfig.VDEItems.RemoveAt(x);
                            }
                        }
                }
                try { frmO.Close(); } catch { }
                try { frmO = null; } catch { }
            }
            catch (Exception ex)
            {
                string err = "RemoveVDEFromZone() err: " + ex.Message;
                MessageBox.Show(err);
                retVal = false;
            }
            return retVal;
        }

        public static string GetNextID(string vdename)
        {
            string retVal = "";
            try
            {
                int newval = 1;
                bool Is1Available = true;

                foreach (VDEItem item in VDEItems)
                {
                    if (item.VDEItemName.ToLower().StartsWith(vdename.ToLower()))
                    {
                        string currentOccurance = item.VDEItemName.Substring(item.VDEItemName.IndexOf("_") + 1).ToString();
                        if (int.TryParse(currentOccurance, out int res))
                            if (res == 1)
                            {
                                Is1Available = false;
                                break;
                            }
                    }
                }

                foreach (VDEItem item in VDEItems)
                    if (item.VDEItemName.ToLower().StartsWith(vdename.ToLower()))
                    {
                        string currentOccurance = item.VDEItemName.Substring(item.VDEItemName.IndexOf("_") + 1).ToString();
                        if (int.TryParse(currentOccurance, out int res))
                            if (res >= newval)
                                newval = res + 1;
                    }
                if (Is1Available)
                    retVal = vdename + "_1";
                else
                    retVal = vdename + "_" + newval.ToString();
            }
            catch (Exception ex)
            {
                string err = "GetNextID() err: " + ex.Message;
                MessageBox.Show(err);
            }
            return retVal;
        }

        public static List<string> CalculateMaskData()
        {
            List<string> retVal = new List<string>();
            try
            {
                foreach (VDEItem vdi in VDEItems)
                    if (vdi != null)
                    {                        
                        if (vdi.IsMask)
                            retVal.AddRange(vdi.CalculateMaskRegion());
                    }
            }
            catch (Exception ex)
            {
                string err = "CalculateMaskData() err: " + ex.Message;
                MessageBox.Show(err);
            }
            return retVal;
        }


        public static List<string> CalculateOverPrintData()
        {
            List<string> retVal = new List<string>();
            try
            {
                foreach (VDEItem vdi in VDEItems)
                    if (vdi != null)
                    {
                        if (vdi.IsOPZone)
                        {
                            retVal.AddRange(vdi.CalculateOverPrintData());
                        }
                    }
            }
            catch (Exception ex)
            {
                string err = "CalculateOverPrintData() err: " + ex.Message;
                MessageBox.Show(err);
            }
            return retVal;
        }

        public static List<string> CalculateVDEData()
        {
            List<string> retVal = new List<string>();
            try
            {
                foreach (VDEItem vdi in VDEItems)
                    if (vdi != null)
                    {
                        if (vdi.IsVDE)
                            retVal.AddRange(vdi.CalculateVDEData());
                    }
            }
            catch (Exception ex)
            {
                string err = "CalculateVDEData() err: " + ex.Message;
                MessageBox.Show(err);
            }
            return retVal;
        }

        public static List<string> CalculateBarcodeData()
        {
            List<string> retVal = new List<string>();
            try
            {
                foreach (VDEItem vdi in VDEItems)
                    if (vdi != null)
                    {
                        if (vdi.IsBarcode2D || vdi.IsBarcodeLinear)
                            retVal.AddRange(vdi.CalculateBarcodeData());
                    }
            }
            catch (Exception ex)
            {
                string err = "CalculateBarcodeData() err: " + ex.Message;
                MessageBox.Show(err);
            }
            return retVal;
        }

        public static bool VDEItemExists(int[] vderegion, int angle, HSmartWindowControl hwin)
        {
            try
            {
                foreach (VDEItem vdi in VDEItems)
                {
                    if (vdi != null)
                    {
                        if (vdi.IsVDE)
                            if (vdi.VDERotatedAngle == angle)
                                if (UtilityFunctions.AlignsWith(vdi.VDERegion[0], vderegion[0], 20))
                                    if (UtilityFunctions.AlignsWith(vdi.VDERegion[1], vderegion[1], 20))
                                        if (UtilityFunctions.AlignsWith(vdi.VDERegion[2], vderegion[2], 20))
                                            if (UtilityFunctions.AlignsWith(vdi.VDERegion[3], vderegion[3], 20))
                                                return true;

                        if (vdi.IsBarcode2D || vdi.IsBarcodeLinear)
                            if (vdi.VDERotatedAngle == angle)
                                if (UtilityFunctions.AlignsWith(vdi.BarcodeRegion[0], vderegion[0], 20))
                                    if (UtilityFunctions.AlignsWith(vdi.BarcodeRegion[1], vderegion[1], 20))
                                        if (UtilityFunctions.AlignsWith(vdi.BarcodeRegion[2], vderegion[2], 20))
                                            if (UtilityFunctions.AlignsWith(vdi.BarcodeRegion[3], vderegion[3], 20))
                                                return true;
                    }
                }
            }
            catch (Exception ex)
            {
                string err = "VDEItemExists() err: " + ex.Message;
                MessageBox.Show(err);
            }
            return false;
        }

        public static VDEItem VDEItemExists(int[] vderegion, int angle)
        {
            try
            {
                foreach (VDEItem vdi in VDEItems)
                {
                    if (vdi != null)
                    {
                        if (vdi.IsVDE)
                            if (vdi.VDERotatedAngle == angle)
                                if (UtilityFunctions.AlignsWith(vdi.VDERegion[0], vderegion[0], 20))
                                    if (UtilityFunctions.AlignsWith(vdi.VDERegion[1], vderegion[1], 20))
                                        if (UtilityFunctions.AlignsWith(vdi.VDERegion[2], vderegion[2], 20))
                                            if (UtilityFunctions.AlignsWith(vdi.VDERegion[3], vderegion[3], 20))
                                                return vdi;
                    }
                }
            }
            catch (Exception ex)
            {
                string err = "VDEItemExists() err: " + ex.Message;
                MessageBox.Show(err);
            }
            return null;
        }

        public static void ClearDataItems(HSmartWindowControl hwin)
        {
            try
            {
                int t = VDEReelConfig.VDEItems.Count;
                int countItems = VDEItems.Count - 1;
                for (int x = countItems; x >= 0; x--)
                {
                    if (VDEItems[x] != null)
                    {
                        VDEItem vde = VDEItems[x];
                        if (vde.ODP != null)
                        {
                            if (vde.ODP.OPVariationImages != null)
                                vde.ODP.OPVariationImages.Dispose();
                            if (vde.ODP.OPZoneIDFixture != null)
                                try { HOperatorSet.ClearShapeModel(vde.ODP.OPZoneIDFixture); } catch { }
                            if (vde.ODP.OPZoneIDVar != null)
                                try { HOperatorSet.ClearVariationModel(vde.ODP.OPZoneIDVar); } catch { }
                        }
                        if (vde.tr != null)
                        {
                            try { vde.tr.ocrHandle.ClearHandle(); } catch { }
                            try { HOperatorSet.ClearTextModel(vde.tr.TextModelReader); } catch { }
                            try { vde.tr = null; } catch { }
                        }
                        vde = null;
                        VDEItems.RemoveAt(x);
                    }
                }
                VDEReelConfig.VDEItems.Clear();
            }
            catch (Exception ex)
            {
                string err = "ClearDataItems() err: " + ex.Message;
                MessageBox.Show(err);
            }
        }
    }
}
