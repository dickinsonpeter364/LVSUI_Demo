using System;
using System.Drawing;

namespace LVS3
{
    public interface IInspection
    {
        bool EnableCapture { get; set; }
        bool SampleIncluded { get; set; }
        bool FixedData { get; set; }
        string FailFolder { get; set; }
        int LabelCount { get; set; }
        event Delegates.AlarmMethodHandler Amh;
        void MoveNextCaller( Action movenextcaller );
        bool LoadInspectionDataFromDb( string lpn, string labelitem );
        bool InitInspection( InspectionContext context );
        bool LoadVdeToolsAndData( string reelLpn, string lin, string lwo );
        List<VdeDisplayItem> GetVdeDisplayItems();
        bool LoadMedDataList( string lin, string lpn, string lwo );
        List<MedDataDisplayItem> GetMedDisplayItems();
        bool LoadInspectionVdeItemParams( string lin );
        bool InspectLabel( Bitmap img, ref FailRecord fp );
        bool InspectAndMaskMasks( ref Bitmap img, ref FailRecord fp );
        bool InspectBarcodes2D( ref Bitmap img, ref FailRecord fp );
        bool MaskBarcodes2D( ref Bitmap img, ref FailRecord fp );
        bool MaskBarcodesLinear( ref Bitmap img, ref FailRecord fp );
        bool InspectBarcodesLinear( ref Bitmap img, ref FailRecord fp );
        void GetImageInspection();
        bool MoveToNextVde();
        string PreviousMedData();
        void MoveLast();
        string ThisMedData();
        void SetPauseCaller( Action pausemethodcaller );
        void SetAlarmCaller( Action alarmmethodcaller );
        void ClearFails();
        void ClearResultData( string reelLpn, string lin );
    }
}
