

using System.Text.Json.Serialization;

namespace LVS3
{

    
    public enum LabelType
    {
        FLATPANEL = 0,
        DARK = 1,
        BOOKLET = 2,
        COLOURED = 3
    }

    //public enum LabelType
    //{
    //    PANEL=0,
    //    BOOKLET = 1,
    //    BOOKLET_BLEEDTHROUGH = 2
    //}

    public enum VAMImageTypes
    {
        VAM = 0,
        TEST = 1
    }

    public enum TrainingDataType
    {
        LABEL = 0,
        OPZONE = 1,
        MASK = 2,
        BARCODE = 3,
        VDE = 4,
        ROT = 5
    }

    public enum VDEFilter
    {
        Default = 0,
        ColouredTextNoisyBackground = 1,
        DarkTextNoisyBackground = 2
    }

    public enum ModelOCMType
    {
        READER = 0,
        MLP = 1
    }

    public static class Enums
    {
        public static int InstanceNo = 0;

        public enum Colors
        {
            RED = 0,
            BLUE = 1,
            MAGENTA = 2,
            BLACK = 3,
            TURQUOISE = 4
        }

        public enum UserAccepted
        {
            YES = 0,
            NO = 1,
            NA = 2
        }

        public enum FontType
        {
            FONT = 0,
            support = 1
        }

        public enum ClearVAM
        {
            DISCARD_SELECTED = 1
        }

        public enum PositionCheck
        {
            ERROR = -1,
            IS_UNREADABLE = 0,
            IS_NEXT = 1,
            IS_DUPLICATE = 2,
            IS_GOOD = 3,
            IS_INCOMPLETE = 4
        }

        public enum VDEType
        {
            VDE = 0,
            BARCODE_2D = 1,
            OP = 2,
            MASK = 3,
            BARCODE_LINEAR = 4,
            ROT_ZONE = 5
        }

        public enum CounterTypes
        {
            NumberInspected = 0,
            NumberAccepted = 1,
            NumberRejected = 2,
            NumberAcceptedOperator = 3,
            NumberRejectedOperator = 4,
            NumberMissing = 5
        }

        public enum CameraType
        {
            NECTACAM = 1,
            ARIACAM = 2
        }


        public enum ESigReason
        {
            StartLabelTraining = 14,
            CancelLabelTraining = 15,
            CancelTrainingAlarm = 19,
            SaveTraining = 16,
            CancelTrainingError = 17,
            EndReel = 6,
            StartInspection = 10,
            LoginUser = 11,
            EndReelError = 12,
            EndReelUser = 13,
            EndReelAlarm = 18
        }

        public enum CriticalLevels
        {
            Black = 0,
            Red = 1,
            Amber = 2,
        }

        public enum PeripheralType
        {
            NOT_ASSIGNED = -1,
            Camera = 0,
            PLC = 1,
            Printer = 2,
            Scanner = 3
        }


        public enum DatabaseSchema
        {
            LIVE = 0,
            DEV = 1
        }

        public enum StatusLevels
        {
            UNASSIGNED = -1,
            INFORMATION = 0,
            WARNING = 1,
            ERROR = 2
        }

        public enum Roles
        {
            //LVSIII_OperatorLvl = 1,
            //LVSIII_TrainerLvl = 2,
            //LVSIII_ReadOnlyLvl = 3,
            //LVSIII_AdministratorLvl = 4
            LVSIII_USer = 2,
            LVSIII_AdministratorLvl = 3
        }

        public enum RawDataResult
        {
            NULL_IMAGE = -1,
            FRAME_AQUIRED = 0,
            WAITING = 1,
            COMPLETE = 2,
            IDLE = 3
        }

        public enum InspectionStatus
        {
            UnInspected = 0,
            Pass = 1,
            Fail = 2,
            OpratorOverride = 3
        }


        public enum PLCRegisterType
        {
            UNASSIGNED = -1,
            PLCReady = 0,
            KitComplete = 1,
            ProductCount = 2,
            KitCount = 3,
            Error = 4,
            Warning = 5,
            Counter = 6,
            PLCGo = 7,
            PLCReset = 8
        }
    }
}
