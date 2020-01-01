//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace HeartRateHistory
//{
//    public static class Config
//    {
//        public const string DateTimeFormat = "yyyyMMdd-hhmmss";

//        private static bool _heartRateGreenFloorLoaded = false;
//        private static int _heartRateGreenFloor = 0;
//        public static int HeartRateGreenFloor
//        {
//            get
//            {
//                if (!_heartRateGreenFloorLoaded)
//                {
//                    _heartRateGreenFloor = int.Parse(ConfigurationManager.AppSettings[nameof(HeartRateGreenFloor)]);
//                    _heartRateGreenFloorLoaded = true;
//                }

//                return _heartRateGreenFloor;
//            }
//        }

//        private static bool _heartRateRedCeilingLoaded = false;
//        private static int _heartRateRedCeiling = 0;
//        public static int HeartRateRedCeiling
//        {
//            get
//            {
//                if (!_heartRateRedCeilingLoaded)
//                {
//                    _heartRateRedCeiling = int.Parse(ConfigurationManager.AppSettings[nameof(HeartRateRedCeiling)]);
//                    _heartRateRedCeilingLoaded = true;
//                }

//                return _heartRateRedCeiling;
//            }
//        }

//        private static bool _dataValueDisplayWidthLoaded = false;
//        private static int _dataValueDisplayWidth = 0;
//        public static int DataValueDisplayWidth
//        {
//            get
//            {
//                if (!_dataValueDisplayWidthLoaded)
//                {
//                    _dataValueDisplayWidth = int.Parse(ConfigurationManager.AppSettings[nameof(DataValueDisplayWidth)]);
//                    _dataValueDisplayWidthLoaded = true;
//                }

//                return _dataValueDisplayWidth;
//            }
//        }

//        private static bool _maxNumberDataShownLoaded = false;
//        private static int _maxNumberDataShown = 0;
//        public static int MaxNumberDataShown
//        {
//            get
//            {
//                if (!_maxNumberDataShownLoaded)
//                {
//                    _maxNumberDataShown = int.Parse(ConfigurationManager.AppSettings[nameof(MaxNumberDataShown)]);
//                    _maxNumberDataShownLoaded = true;
//                }

//                return _maxNumberDataShown;
//            }
//        }

//        private static bool _maxDataAgeSecLoaded = false;
//        private static int _maxDataAgeSec = 0;
//        public static int MaxDataAgeSec
//        {
//            get
//            {
//                if (!_maxDataAgeSecLoaded)
//                {
//                    _maxDataAgeSec = int.Parse(ConfigurationManager.AppSettings[nameof(MaxDataAgeSec)]);
//                    _maxDataAgeSecLoaded = true;
//                }

//                return _maxDataAgeSec;
//            }
//        }

//        private static bool _numberColorIntervalsLoaded = false;
//        private static int _numberColorIntervals = 0;
//        public static int NumberColorIntervals
//        {
//            get
//            {
//                if (!_numberColorIntervalsLoaded)
//                {
//                    _numberColorIntervals = int.Parse(ConfigurationManager.AppSettings[nameof(NumberColorIntervals)]);
//                    _numberColorIntervalsLoaded = true;
//                }

//                return _numberColorIntervals;
//            }
//        }

//        private static bool _maxNumberDataInMemoryLoaded = false;
//        private static int _maxNumberDataInMemory = 0;
//        public static int MaxNumberDataInMemory
//        {
//            get
//            {
//                if (!_maxNumberDataInMemoryLoaded)
//                {
//                    _maxNumberDataInMemory = int.Parse(ConfigurationManager.AppSettings[nameof(MaxNumberDataInMemory)]);
//                    _maxNumberDataInMemoryLoaded = true;
//                }

//                return _maxNumberDataInMemory;
//            }
//        }

//        private static bool _displayLabelRightFontSizeLoaded = false;
//        private static double _displayLabelRightFontSize = 0;
//        public static double DisplayLabelRightFontSize
//        {
//            get
//            {
//                if (!_displayLabelRightFontSizeLoaded)
//                {
//                    _displayLabelRightFontSize = double.Parse(ConfigurationManager.AppSettings[nameof(DisplayLabelRightFontSize)]);
//                    _displayLabelRightFontSizeLoaded = true;
//                }

//                return _displayLabelRightFontSize;
//            }
//        }

//        private static bool _displayLabelRightFontFamilyLoaded = false;
//        private static string _displayLabelRightFontFamily = null;
//        public static string DisplayLabelRightFontFamily
//        {
//            get
//            {
//                if (!_displayLabelRightFontFamilyLoaded)
//                {
//                    _displayLabelRightFontFamily = ConfigurationManager.AppSettings[nameof(DisplayLabelRightFontFamily)].ToString();
//                    _displayLabelRightFontFamilyLoaded = true;
//                }

//                return _displayLabelRightFontFamily;
//            }
//        }
//    }
//}
