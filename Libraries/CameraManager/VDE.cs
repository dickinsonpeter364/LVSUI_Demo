using System.Drawing;
using LVS3;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static LVS3.Delegates;
using static LVS3.Enums;

namespace LVS3
{
    public class Vde
    {
        private double _confidenceLevel;
        private Action _pauseMethodCaller;
        public static event ErrorMethodHandler Emh;
        public bool CountMatchError;
        public bool ErrorOccured;
        public string ErrorString = "";
        public int CharacterIndex = 0;
        public int WordLength;
        public bool MedFailInCurrentPatient { get; set; }
        private Action<SystemMessageEventArgs>? _systemMessageHandler;
        private List<OPZoneData> _opZoneItems = new List<OPZoneData>();
        private List<VDEItem> _vdeItems = new List<VDEItem>();
        private List<MedData> _medDataItems = new List<MedData>();

        public Vde(List<OPZoneData> opzoneitems, List<VDEItem> vdeitems, List<MedData> meddataitems,
                   Action<SystemMessageEventArgs>? systemMessageHandler, Action pausemethodcaller,
                   ErrorMethodHandler? errorMethodHandler = null, double confidenceLevel = 0.5)
        {
            _pauseMethodCaller = pausemethodcaller;
            _confidenceLevel = confidenceLevel;
            if (errorMethodHandler != null)
            {
                Emh -= errorMethodHandler;
                Emh += errorMethodHandler;
            }
            _opZoneItems = opzoneitems;
            _vdeItems = vdeitems;
            _medDataItems = meddataitems;
            _systemMessageHandler = systemMessageHandler;
            CountMatchError = false;
        }

        public int RotateAdjustment(int valuerotated)
        {
            var angle = 0;
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

        public bool FindVde(int rotatedangle, int angle, ref Bitmap img, ref FailRecord fp, OPZoneData ozd, List<VDEItem> vdeitems, string variableMedDataPh)
        {
            var retVal = true;            
            Bitmap region = null, tmpChar = null, imgChars = null, characters = null, imgRotated = null;
            Bitmap rgnComplement = null, imgReduced = null;
            object srR1, srCol1, srR2, srCol2, matchedItem = null, word = null, wordscore = null;
            var match = "";

            try
            {
                /* TODO: Replace HOperatorSet.GenEmptyObj */ //out imgRotated);
                RotateBackground(rotatedangle, ref img, ref imgRotated);
                HOperatorSet.SmallestRectangle1(ozd.region, out var r1, out var c1, out var r2, out var c2);
                var rgnCoords = new int[4];

                rgnCoords[0] = Convert.ToInt32(r1.D);
                rgnCoords[1] = Convert.ToInt32(c1.D);
                rgnCoords[2] = Convert.ToInt32(r2.D);
                rgnCoords[3] = Convert.ToInt32(c2.D);
                /* TODO: Replace HOperatorSet.GetImageSize */ //img, out var w, out var h);
                foreach (var vdi in vdeitems)
                {
                    if (SYSTEM_IO.PROCESSING == false)
                        return true;
                    CountMatchError = false;
                    match = "";
                    if (vdi.FontName == "") 
                        continue;
                    if (vdi.VDERotatedAngle != angle) continue;
                    if (imgChars != null) imgChars.Dispose();
                    if (characters != null) characters.Dispose();
                    if (region != null) region.Dispose();
                    if (tmpChar != null) tmpChar.Dispose();
                    if (rgnComplement != null) rgnComplement.Dispose();
                    if (imgReduced != null) imgReduced.Dispose();
                    foreach (var medData in _medDataItems)
                        if (medData.PlaceHolder == vdi.Placeholder)
                        {
                            match = medData.Data;
                            vdi.RepeatType = medData.Repeat;
                            break;
                        }
                    if (match == "")
                        continue;
                    var matchData = new object(match);

                    //mxClient.Stop(1);
                    
                    word = "";
                    wordscore = 0;
                    var textCoords = VDEItem.RotateClockwiseCoords(angle, w, h, vdi.VDERegion);
                    var confidences = new double[match.Length];
                    ErrorOccured = false;
                    vdi.DarkMaxGray = ozd.MAX_GRAY;

                    //Maurice 03-MAY-2024 Error Fix - Below conditional statement was inhibiting the call to getText, producing a continuous Halcon Error
                    //if (match == "KB_P03666")
                    //Maurice 03-MAY-2024 Halcon Error Fix End

                    if (GetText(ref characters, ref word, ref wordscore, matchData, ref imgRotated, textCoords, vdi, ozd.NAME, ref fp) == false)
                    {
                        if (vdi.Placeholder == variableMedDataPh)
                        {
                            MedFailInCurrentPatient = true;
                        }

                        if (SYSTEM_IO.PROCESSING == false)
                            return true;

                        if (ErrorOccured)
                            throw new Exception(ErrorString);
                        fp.VALID_LABEL = false;
                        var rfd = new RegionFailData(vdi.OpZoneName);
                        if (rfd.Img == null)
                            /* TODO: Replace HOperatorSet.GenEmptyObj */ //out rfd.Img);
                        /* TODO: Replace HOperatorSet.CopyObj */ //img, out rfd.Img, 1, 1);
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

                            var med = "";
                            foreach (var md in _medDataItems)
                                if (md.Data == matchData)
                                {
                                    med = md.Data + "|" + md.PlaceHolder + "|" + word.S;
                                    break;
                                }

                            fp.DatasNotFound.Add(med);
                            rfd.Reasons.Add("Data mis-match: " + match + ". Data found: " + word.S);
                        }
                        rfd.failCoordsList.Add(new RegionCoordPoints(new int[] { rgnCoords[0], rgnCoords[1], rgnCoords[2], rgnCoords[3] }));
                        object top = vdi.VDERegion[0] - 4;
                        object left = vdi.VDERegion[1] - 8;
                        object bottom = vdi.VDERegion[2] + 4;
                        object right = vdi.VDERegion[3] + 16;
                        if (top < 0)
                            top = 0;
                        if (left < 0)
                            left = 0;
                        HOperatorSet.GenRectangle1(out region, top, left, bottom, right);
                        /* TODO: Replace HOperatorSet.ReduceDomain */ //img, region, out imgReduced);
                        rfd.failCoordsList.Add(new RegionCoordPoints(new int[] { vdi.VDERegion[0], vdi.VDERegion[1], vdi.VDERegion[2], vdi.VDERegion[3] }));
                        if (vdi.RepeatType > 0)
                        {
                            /* TODO: Replace HOperatorSet.Complement */ //region, out rgnComplement);
                            /* TODO: Replace HOperatorSet.ReduceDomain */ //img, rgnComplement, out img);
                        }
                        retVal = false;
                        continue;
                    }
                    if (SYSTEM_IO.PROCESSING == false)
                        return true; 

                    
                    HOperatorSet.SmallestRectangle1(characters, out srR1, out srCol1, out srR2, out srCol2);

                    var resultText = word.S;
                    var tmpResult = resultText;
                    /* TODO: Replace HOperatorSet.TupleRegexpMatch */ //tmpResult, matchData.TupleConcat("ignore_case"), out matchedItem);
                    if (matchedItem.Length == 0)
                    {
                        var result = "";
                        try { result = word.S; } catch { }

                        fp.VALID_LABEL = false;
                        var rfd = new RegionFailData(ozd.NAME);
                        if (rfd.Img == null)
                            /* TODO: Replace HOperatorSet.GenEmptyObj */ //out rfd.Img);
                        /* TODO: Replace HOperatorSet.CopyObj */ //img, out rfd.Img, 1, 1);
                        rfd.Reasons.Add(string.Format(match + " with " + vdi.Placeholder + " {0} does not contain the expected VDE in {1}", result, ozd.NAME));
                        fp.RegionFailDataList.Add(rfd);

                        fp.DatasNotFound.Add(string.Format(match + " with " + vdi.Placeholder + " {0} does not contain the expected VDE in {1}", result, ozd.NAME));
                        fp.DATA_FOUND = false;

                        object topV = Convert.ToInt32(vdi.VDERegion[0] - 4);
                        object leftV = Convert.ToInt32(vdi.VDERegion[1] - 4);
                        object bottomV = Convert.ToInt32(vdi.VDERegion[2] + 4);
                        object rightV = Convert.ToInt32(vdi.VDERegion[3] + 4);
                        var top = Convert.ToInt32(textCoords[0] - 2);
                        var left = Convert.ToInt32(textCoords[1] - 2);
                        var bottom = Convert.ToInt32(textCoords[2] + 2);
                        var right = Convert.ToInt32(textCoords[3] + 2);

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
                            /* TODO: Replace HOperatorSet.Complement */ //region, out rgnComplement);
                            /* TODO: Replace HOperatorSet.ReduceDomain */ //img, rgnComplement, out img);
                        }
                        retVal = false;
                        continue;
                    }
                    if (SYSTEM_IO.PROCESSING == false)
                        return true;
                    if (vdi.RepeatType > 0)
                        MaskComplementVde(matchedItem.S, resultText, ref characters, angle, w, h, vdi, ref img);
                }
            }
            catch (Exception ex)
            {
                //mxClient.Stop(1);
                if (SYSTEM_IO.PROCESSING == false)
                    return true;
                //SYSTEM_IO.PROCESSING = false;
                retVal = false;
                var err = "findVDE() err: " + ex.Message;                
                var smea = new SystemMessageEventArgs(err, "Label Inspection", (int)CriticalLevels.Red);
                _systemMessageHandler?.Invoke(smea);                
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
                if (imgChars != null)
                    imgChars.Dispose();
                if (characters != null)
                    characters.Dispose();
                if (imgRotated != null)
                    imgRotated.Dispose();
            }
            return retVal;
        }

        private bool MaskComplementVde(string matcheditem, string resultText, ref Bitmap characters, int angle, int w, int h, VDEItem vde, ref Bitmap img)
        {
            var retVal = true;
            Bitmap region = null, mask = null, rgnComplement = null;
            var firstChar = 0;
            var lastChar = 0;
            var matchFound = matcheditem;
            var res = matcheditem;
            try
            {
                while (firstChar >= 0)
                {
                    firstChar = resultText.IndexOf(matchFound, firstChar);
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

                    /* TODO: Replace HOperatorSet.CopyObj */ //characters, out mask, firstChar + 1, lastChar);
                    /* TODO: Replace HOperatorSet.CountObj */ //mask, out var numChars);
                    var top = 500000;
                    var left = 500000;
                    var bottom = 0;
                    var right = 0;
                    for (var x = 1; x <= numChars; x++)
                    {
                        HOperatorSet.SmallestRectangle1(mask[x], out var t, out var l, out var b, out var r);
                        if (t < top)
                            top = t;
                        if (l < left)
                            left = l;
                        if (b > bottom)
                            bottom = b;
                        if (r > right)
                            right = r;
                    }
                    var maskCoord = new int[] { top, left, bottom, right };
                    var row1 = Convert.ToInt32(maskCoord[0]) - 16;
                    var col1 = Convert.ToInt32(maskCoord[1]) - 16;
                    var row2 = Convert.ToInt32(maskCoord[2]) + 16;
                    var col2 = Convert.ToInt32(maskCoord[3]) + 16;
                    if (row1 < 0)
                        row1 = 0;
                    if (col1 < 0)
                        col1 = 0;
                    maskCoord[0] = row1;
                    maskCoord[1] = col1;
                    maskCoord[2] = row2;
                    maskCoord[3] = col2;
                    var rotateCoords = VDEItem.RotateAntiClockwiseCoords(angle, w, h, maskCoord);
                    HOperatorSet.GenRectangle1(out region, rotateCoords[0], rotateCoords[1], rotateCoords[2], rotateCoords[3]);
                    if (vde.RepeatType > 0)
                    {
                        /* TODO: Replace HOperatorSet.Complement */ //region, out rgnComplement);
                        /* TODO: Replace HOperatorSet.ReduceDomain */ //img, rgnComplement, out img);
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
                var err = "maskComplementVDE() err: " + ex.Message;
                if (SYSTEM_IO.PROCESSING == false)
                    return retVal;
                SYSTEM_IO.PROCESSING = false;
                var smea = new SystemMessageEventArgs(err, "Label Inspection", (int)CriticalLevels.Red);
                _systemMessageHandler?.Invoke(smea);
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

        private bool GetText(ref Bitmap characters, ref object word, ref object wordscore, string matchdata, ref Bitmap img, int[] region, VDEItem vdi, string opzname, ref FailRecord fp)
        {
            Bitmap imgReduced = null, rgnZone = null, imgForeground = null, connectedChars = null, connectedRgnChars = null, character = null;
            var retVal = true;
            object textresultid = null, classVal = null, confidence = null;
            try
            {
                var additionalChars = 0;
                var paddingVdeBox = 5;
                ErrorString = "";
                ErrorOccured = false;
                word = new object();
                wordscore = new object();
                /* TODO: Replace HOperatorSet.GenEmptyObj */ //out characters);
                /* TODO: Replace HOperatorSet.GetImageSize */ //img, out var w, out var h);
                object top = region[0] - paddingVdeBox * 2;
                object left = region[1] - paddingVdeBox * 3;
                object bottom = region[2] + paddingVdeBox * 2;
                object right = region[3] + paddingVdeBox * 3;
                if (left < 0)
                    left = 0;
                if (top < 0)
                    top = 0;

                //mxClient.Stop(1);

                HOperatorSet.GenRectangle1(out rgnZone, top, left, bottom, right);
                /* TODO: Replace HOperatorSet.ReduceDomain */ //img, rgnZone, out imgReduced);
                /* TODO: Replace HOperatorSet.SegmentCharacters */ //rgnZone, imgReduced, out imgForeground, out characters, "local_contrast_best", "false", "false", vdi.tr.StrokeWidth, vdi.tr.CharWidth, vdi.tr.CharHeight, 0, vdi.tr.SegmentContrast, out var usedThreshold);
                /* TODO: Replace HOperatorSet.SelectCharacters */ //characters, out connectedChars, "false", vdi.tr.StrokeWidth, vdi.tr.CharWidth, vdi.tr.CharHeight, "true", "false", vdi.tr.PartitionMethod, "false", "medium", "false", 20, "completion");
                characters.Dispose();
                /* TODO: Replace HOperatorSet.Connection */ //connectedChars, out connectedRgnChars);
                /* TODO: Replace HOperatorSet.SelectShape */ //connectedRgnChars, out characters, "area", "and", 100, 99999);
                /* TODO: Replace HOperatorSet.DoOcrWordMlp */ //characters, imgReduced, vdi.tr.ocrHandle, matchdata.ToString(), 1, 0, out classVal, out confidence, out word, out wordscore);                
                
                var sWord = "";
                var sWordcopy = "";
                if (classVal.Length > 0)
                {
                    for (var x = 0; x < classVal.Length; x++)
                        sWord = sWord + classVal[x].S;
                    word = sWord;
                    sWordcopy = sWord;
                }
                else
                {
                    WordLength = 0;
                    return false;
                }

                var tempInt = 0;                

                if (tempInt == 0)
                {
                    var sVde = matchdata;

                    var newString = "";

                    if (matchdata != sWord) 
                    {
                        if (matchdata.Length >= sWord.Length) 
                        {
                            var index = 0;

                            foreach (var c in matchdata)   
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

                            foreach (var c in sWord) 
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
                        var newSWord = sWord;
                        var combinedString = matchdata + sWord;
                        var charDifferences = "";
                        for (var i = 0; i < combinedString.Length + 1; i++)
                        {
                            if (matchdata.Contains(combinedString[0]) && sWord.Contains(combinedString[0]))
                            {
                                try
                                {
                                    if (matchdata.Count(c1 => c1 == combinedString[0]) - sWord.Count(c2 => c2 == combinedString[0]) == 0)
                                    {
                                        var testStringMd = matchdata;
                                        var testStringSw = sWord;
                                        var loopCount = matchdata.Count(c1 => c1 == combinedString[0]);
                                        var indexPositionMd = 0;
                                        var sumIndexMd = 0;
                                        var indexPositionSw = 0;
                                        var sumIndexSw = 0;
                                        for (var j = 0; j < loopCount; j++)
                                        {
                                            indexPositionMd = testStringMd.IndexOf(combinedString[0]);
                                            sumIndexMd = sumIndexMd + indexPositionMd;
                                            testStringMd = testStringMd.Substring(0, indexPositionMd) + testStringMd.Substring(indexPositionMd);

                                            indexPositionSw = testStringSw.IndexOf(combinedString[0]);
                                            sumIndexSw = sumIndexSw + indexPositionSw;
                                            testStringSw = testStringSw.Substring(0, indexPositionSw) + testStringSw.Substring(indexPositionSw);
                                        }
                                        if (sumIndexMd == sumIndexSw)
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

                        var stringMaxLength = 10;
                        for (var i = 0; i < stringMaxLength; i++)
                        {
                            var iMinus = false;
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
                
                object numChars = characters.CountObj();
                int tempNumChars = numChars + additionalChars;
                if (tempNumChars != matchdata.Length)
                {
                    fp.ConfidenceLevel = 0;
                    var rfd = new RegionFailData(opzname);
                    HOperatorSet.SmallestRectangle1(rgnZone, out top, out left, out bottom, out right);
                    rfd.failCoordsList.Add(new RegionCoordPoints(new int[] { top, left, bottom, right }));
                    rfd.Reasons.Add(string.Format("The VDE found {0} in Opzone {1} does not match the expected value: {2}", sWord, opzname, matchdata));
                    if (rfd.Img == null)
                        /* TODO: Replace HOperatorSet.GenEmptyObj */ //out rfd.Img);
                    /* TODO: Replace HOperatorSet.CopyObj */ //img, out rfd.Img, 1, 1);
                    fp.RegionFailDataList.Add(rfd);
                    retVal = false;
                    return retVal;
                }

                #region Confidence Rating
                if (confidence.Length > 0)
                {
                    var temp1 = sWord;
                    var temp2 = matchdata;

                    temp1 = sWord;

                    

                    RegionFailData rfd = null;
                    double confidenceLevel = 0;
                    var confident = true;
                    var alternateConfidenceLevel = 0.75;

                    var newconfidence = new object();
                    var indexAdjust = 0;
                    try
                    {
                        for (var x = 0; x < matchdata.Length; x++)
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

                    for (var x = 0; x < confidence.Length; x++)
                    {
                        if (character != null)
                            character.Dispose();
                        confidenceLevel = confidence[x].D;
                        if (confidenceLevel < alternateConfidenceLevel)
                        {
                            if (IsAlphaNumeric(classVal[x].S))
                            {
                                confident = false;
                                fp.ConfidenceLevel = confidenceLevel;
                                rfd = new RegionFailData(opzname);
                                /* TODO: Replace HOperatorSet.CopyObj */ //characters, out character, x + 1, 1);
                                HOperatorSet.SmallestRectangle1(character, out top, out left, out bottom, out right);
                                rfd.failCoordsList.Add(new RegionCoordPoints(new int[] { top, left, bottom, right }));
                                rfd.Reasons.Add(string.Format("Confidence rating is {0} for character '{1}' (at position {2}), in {3}: low level is {4}. Opzone {5}", Math.Round(fp.ConfidenceLevel, 4), matchdata[x], x + 1, matchdata, alternateConfidenceLevel, opzname));
                                fp.RegionFailDataList.Add(rfd);
                            }
                        }
                    }
                    if (confident == false)
                    {
                        if (rfd.Img == null)
                            /* TODO: Replace HOperatorSet.GenEmptyObj */ //out rfd.Img);
                        /* TODO: Replace HOperatorSet.CopyObj */ //imgReduced, out rfd.Img, 1, 1);
                    }

                    #endregion

                    word = sWord;

                    if (CameraManager.UtilityFunctions.MSERCheckVDE(ref imgReduced, matchdata.Length, vdi.DarkMaxGray, ref CountMatchError, ref ErrorString, ref ErrorOccured, ref characters) == true)
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
                    var rfd = new RegionFailData(opzname);
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
                    try { /* TODO: Replace HOperatorSet.ClearTextResult */ //textresultid); } catch { }
            }
            return retVal;
        }

        private bool IsAlphaNumeric(string character)
        {
            var rg = new Regex(@"^[a-zA-Z0-9\s,]*$");
            return rg.IsMatch(character);
        }

        private bool RotateBackground(int angle, ref Bitmap imgIn, ref Bitmap imgOut)
        {
            var retVal = false;
            try
            {
                if (imgOut != null)
                    imgOut.Dispose();
                /* TODO: Replace HOperatorSet.RotateImage */ //imgIn, out imgOut, angle * -1, "constant");
                retVal = true;
            }
            catch (Exception ex)
            {
                //pauseMethodCaller.Invoke();
                if (SYSTEM_IO.PROCESSING == false)
                    return true;
                SYSTEM_IO.PROCESSING = false;
                retVal = false;
                var err = "rotateBackground() err: " + ex.Message;
                var smea = new SystemMessageEventArgs(err, "Label Inspection", (int)CriticalLevels.Red);
                _systemMessageHandler?.Invoke(smea);
            }
            return retVal;
        }
    }

    public class VdeReelConfig
    {
        public static string ReelLpn = "";
        public static string Lwo;
        public static string LabelItem = "";
        public string InspectionComments;
        public string LabelController;
        public static List<VDEItem> VdeItems = new List<VDEItem>();


        public static bool HasVde()
        {
            var retVal = false;
            try
            {
                for (var x = 0; x < VdeItems.Count; x++)
                    if (VdeItems[x] != null)
                    {
                        if (VdeItems[x].IsVDE == true)
                        {
                            retVal = true;
                            return retVal;
                        }
                    }
            }
            catch (Exception ex)
            {
                var err = "HasVDE() err: " + ex.Message;
                Log.Logger.Error(err);
                retVal = false;
            }
            return retVal;
        }


        public static bool AddManualVde(EventArgs? e = null)
        {
            var retVal = true;
            try
            {
                VdeItems.Add(null);
            }
            catch (Exception ex)
            {
                var err = "AddManualVDE() err: " + ex.Message;
                Log.Logger.Error(err);
                retVal = false;
            }
            return retVal;
        }

        public static bool RemoveAllVde()
        {
            var retVal = true;
            try
            {
                for (var x = VdeItems.Count - 1; x >= 0; x--)
                    if (VdeItems[x] != null)
                    {
                        if (VdeItems[x].IsVDE == true)
                        {
                            try { VdeItems[x].DestroyVars(); } catch { }
                            VdeItems.RemoveAt(x);
                        }
                    }
            }
            catch (Exception ex)
            {
                var err = "RemoveAllVDE() err: " + ex.Message;
                Log.Logger.Error(err);
                retVal = false;
            }
            return retVal;
        }

        public static bool RemoveVdeFromZone(string opZoneName = "")
        {
            var retVal = true;
            try
            {
                if (opZoneName != "")
                {
                    for (var x = VdeItems.Count - 1; x >= 0; x--)
                        if (VdeItems[x] != null)
                        {
                            if (VdeItems[x].OpZoneName.ToLower() == opZoneName.ToLower() && VdeItems[x].IsVDE == true)
                            {
                                try { VdeItems[x].DestroyVars(); } catch { }
                                VdeItems.RemoveAt(x);
                            }
                        }
                }
            }
            catch (Exception ex)
            {
                var err = "RemoveVDEFromZone() err: " + ex.Message;
                Log.Logger.Error(err);
                retVal = false;
            }
            return retVal;
        }

        public static string GetNextId(string vdename)
        {
            var retVal = "";
            try
            {
                var newval = 1;
                var is1Available = true;

                foreach (var item in VdeItems)
                {
                    if (item.VDEItemName.ToLower().StartsWith(vdename.ToLower()))
                    {
                        var currentOccurance = item.VDEItemName.Substring(item.VDEItemName.IndexOf("_") + 1).ToString();
                        if (int.TryParse(currentOccurance, out var res))
                            if (res == 1)
                            {
                                is1Available = false;
                                break;
                            }
                    }
                }

                foreach (var item in VdeItems)
                    if (item.VDEItemName.ToLower().StartsWith(vdename.ToLower()))
                    {
                        var currentOccurance = item.VDEItemName.Substring(item.VDEItemName.IndexOf("_") + 1).ToString();
                        if (int.TryParse(currentOccurance, out var res))
                            if (res >= newval)
                                newval = res + 1;
                    }
                if (is1Available)
                    retVal = vdename + "_1";
                else
                    retVal = vdename + "_" + newval.ToString();
            }
            catch (Exception ex)
            {
                var err = "GetNextID() err: " + ex.Message;
                Log.Logger.Error(err);
            }
            return retVal;
        }

        public static List<string> CalculateMaskData()
        {
            var retVal = new List<string>();
            try
            {
                foreach (var vdi in VdeItems)
                    if (vdi != null)
                    {                        
                        if (vdi.IsMask)
                            retVal.AddRange(vdi.CalculateMaskRegion());
                    }
            }
            catch (Exception ex)
            {
                var err = "CalculateMaskData() err: " + ex.Message;
                Log.Logger.Error(err);
            }
            return retVal;
        }


        public static List<string> CalculateOverPrintData()
        {
            var retVal = new List<string>();
            try
            {
                foreach (var vdi in VdeItems)
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
                var err = "CalculateOverPrintData() err: " + ex.Message;
                Log.Logger.Error(err);
            }
            return retVal;
        }

        public static List<string> CalculateVdeData()
        {
            var retVal = new List<string>();
            try
            {
                foreach (var vdi in VdeItems)
                    if (vdi != null)
                    {
                        if (vdi.IsVDE)
                            retVal.AddRange(vdi.CalculateVDEData());
                    }
            }
            catch (Exception ex)
            {
                var err = "CalculateVDEData() err: " + ex.Message;
                Log.Logger.Error(err);
            }
            return retVal;
        }

        public static List<string> CalculateBarcodeData()
        {
            var retVal = new List<string>();
            try
            {
                foreach (var vdi in VdeItems)
                    if (vdi != null)
                    {
                        if (vdi.IsBarcode2D || vdi.IsBarcodeLinear)
                            retVal.AddRange(vdi.CalculateBarcodeData());
                    }
            }
            catch (Exception ex)
            {
                var err = "CalculateBarcodeData() err: " + ex.Message;
                Log.Logger.Error(err);
            }
            return retVal;
        }

        public static bool VdeItemExists(int[] vderegion, int angle, object hwin)
        {
            try
            {
                foreach (var vdi in VdeItems)
                {
                    if (vdi != null)
                    {
                        if (vdi.IsVDE)
                            if (vdi.VDERotatedAngle == angle)
                                if (CameraManager.UtilityFunctions.AlignsWith(vdi.VDERegion[0], vderegion[0], 20))
                                    if (CameraManager.UtilityFunctions.AlignsWith(vdi.VDERegion[1], vderegion[1], 20))
                                        if (CameraManager.UtilityFunctions.AlignsWith(vdi.VDERegion[2], vderegion[2], 20))
                                            if (CameraManager.UtilityFunctions.AlignsWith(vdi.VDERegion[3], vderegion[3], 20))
                                                return true;

                        if (vdi.IsBarcode2D || vdi.IsBarcodeLinear)
                            if (vdi.VDERotatedAngle == angle)
                                if (CameraManager.UtilityFunctions.AlignsWith(vdi.BarcodeRegion[0], vderegion[0], 20))
                                    if (CameraManager.UtilityFunctions.AlignsWith(vdi.BarcodeRegion[1], vderegion[1], 20))
                                        if (CameraManager.UtilityFunctions.AlignsWith(vdi.BarcodeRegion[2], vderegion[2], 20))
                                            if (CameraManager.UtilityFunctions.AlignsWith(vdi.BarcodeRegion[3], vderegion[3], 20))
                                                return true;
                    }
                }
            }
            catch (Exception ex)
            {
                var err = "VDEItemExists() err: " + ex.Message;
                Log.Logger.Error(err);
            }
            return false;
        }

        public static VDEItem VdeItemExists(int[] vderegion, int angle)
        {
            try
            {
                foreach (var vdi in VdeItems)
                {
                    if (vdi != null)
                    {
                        if (vdi.IsVDE)
                            if (vdi.VDERotatedAngle == angle)
                                if (CameraManager.UtilityFunctions.AlignsWith(vdi.VDERegion[0], vderegion[0], 20))
                                    if (CameraManager.UtilityFunctions.AlignsWith(vdi.VDERegion[1], vderegion[1], 20))
                                        if (CameraManager.UtilityFunctions.AlignsWith(vdi.VDERegion[2], vderegion[2], 20))
                                            if (CameraManager.UtilityFunctions.AlignsWith(vdi.VDERegion[3], vderegion[3], 20))
                                                return vdi;
                    }
                }
            }
            catch (Exception ex)
            {
                var err = "VDEItemExists() err: " + ex.Message;
                Log.Logger.Error(err);
            }
            return null;
        }

        public static void ClearDataItems(object hwin)
        {
            try
            {
                var t = VdeItems.Count;
                var countItems = VdeItems.Count - 1;
                for (var x = countItems; x >= 0; x--)
                {
                    if (VdeItems[x] != null)
                    {
                        var vde = VdeItems[x];
                        if (vde.ODP != null)
                        {
                            if (vde.ODP.OPVariationImages != null)
                                vde.ODP.OPVariationImages.Dispose();
                            if (vde.ODP.OPZoneIDFixture != null)
                                try { /* TODO: Replace HOperatorSet.ClearShapeModel */ //vde.ODP.OPZoneIDFixture); } catch { }
                            if (vde.ODP.OPZoneIDVar != null)
                                try { /* TODO: Replace HOperatorSet.ClearVariationModel */ //vde.ODP.OPZoneIDVar); } catch { }
                        }
                        if (vde.tr != null)
                        {
                            try { vde.tr.ocrHandle.ClearHandle(); } catch { }
                            try { /* TODO: Replace HOperatorSet.ClearTextModel */ //vde.tr.TextModelReader); } catch { }
                            try { vde.tr = null; } catch { }
                        }
                        vde = null;
                        VdeItems.RemoveAt(x);
                    }
                }
                VdeItems.Clear();
            }
            catch (Exception ex)
            {
                var err = "ClearDataItems() err: " + ex.Message;
                Log.Logger.Error(err);
            }
        }
    }
}
