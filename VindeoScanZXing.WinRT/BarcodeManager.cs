﻿using System;

using ZXing;
using Windows.UI.Xaml.Media.Imaging;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace VideoScanZXing.WinRT
{
    /// <summary>
    /// Scan a barcode for a live video stream
    /// </summary>
    public static class BarCodeManager
    {
        internal static Action<string> OnBarCodeFound
        {
            get;
            private set;
        }
        internal static Action<Exception> OnError
        {
            get;
            private set;
        }

        internal static BarcodeReader _ZXingReader;

        static BarCodeManager()
        {
            //MaxTry = 15;
        }

        /// <summary>
        /// Starts the scan : navigates to the scan page and starts reading video stream
        /// Note : Scan will auto-stop if navigation occurs
        /// </summary>
        /// <param name="onBarCodeFound">Delegate Action on a barcode found</param>
        /// <param name="onError">Delegate Action on error</param>
        /// <param name="zxingReader">(optional) A specific reader format, Default will be EAN13Reader </param>
        public static void StartScan(Action<string> onBarCodeFound, Action<Exception> onError, BarcodeFormat barcodeFormat = BarcodeFormat.EAN_13)
        {
            OnBarCodeFound = onBarCodeFound;
            OnError = onError;

            _ZXingReader = GetReader(barcodeFormat);

            var rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(BarcodePage));
        }

        internal static Result ScanBitmap(byte[] pixelsArray, int width, int height)
        {
            var result = _ZXingReader.Decode(pixelsArray, width, height, BitmapFormat.Unknown);

            if (result != null)
            {
                Debug.WriteLine(result.Text);
                if (BarCodeManager.OnBarCodeFound != null)
                {
                    //_stop = true;
                    BarCodeManager.OnBarCodeFound(result.Text);
                }
            }

            return result;
        }


        /// <summary>
        /// Try 20 times to focus and scan for 1,5 sec (default)
        /// </summary>
        //public static int MaxTry
        //{
        //    get;
        //    set;
        //}


        /// <summary>
        /// Returns the zxing reader class for the current specified ScanMode.
        /// </summary>
        /// <returns></returns>
        internal static BarcodeReader GetReader(BarcodeFormat format = BarcodeFormat.All_1D)
        {
            return new BarcodeReader()
            {
                AutoRotate = true,
                Options = new ZXing.Common.DecodingOptions() { TryHarder = false, PossibleFormats = new BarcodeFormat[] { format } }
            };
        }

    }
}

