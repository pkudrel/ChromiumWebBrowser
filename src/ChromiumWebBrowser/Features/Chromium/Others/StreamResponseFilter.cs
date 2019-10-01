// Copyright © 2017 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CefSharp;
using NLog;

namespace ChromiumWebBrowser.Features.Chromium.Others
{
    public class StreamResponseFilter : IResponseFilter
    {
        private readonly string _requestUrl;
        private readonly ulong _requestIdentifier;
        private MemoryStream _memoryStream;
        private List<byte> buff = new List<byte>(65_000);
        private bool _wasDisposed = false;
        private bool isEnd = false;
        private int _length;
        private long _lengthCount;
        private long _lengthWritten = 0;
       
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        public StreamResponseFilter(string requestUrl, ulong requestIdentifier)
        {
            _requestUrl = requestUrl;
            _requestIdentifier = requestIdentifier;
            _memoryStream = new MemoryStream();
        }

        public byte[] Data => _memoryStream.ToArray();

        bool IResponseFilter.InitFilter()
        {
            return true;
        }

        FilterStatus IResponseFilter.Filter(Stream dataIn, out long dataInRead, Stream dataOut, out long dataOutWritten)
        {
          
           
            if (dataIn == null)
            {
                dataInRead = 0;
                dataOutWritten = 0;

                return FilterStatus.Done;
            }
            _lengthCount += dataIn.Length;
            dataInRead = dataIn.Length;
            var  dataOutWrittenTmp = Math.Min(dataInRead, dataOut.Length);
            
            //Important we copy dataIn to dataOut
            dataIn.CopyTo(dataOut);

            //Copy data to stream
            dataIn.Position = 0;
            dataIn.CopyTo(_memoryStream);
            MemoryStream ms = new MemoryStream();
            dataIn.Position = 0;
            dataIn.CopyTo(ms);
            var dataAsUtf8String = Encoding.UTF8.GetString(ms.ToArray());
            _log.Debug($"Id: {_requestIdentifier}; Length: {_length}; LengthCount: {_lengthCount}");


            if (dataAsUtf8String.IndexOf("</body>",StringComparison.OrdinalIgnoreCase) > -1)
            {
                isEnd = true;
                var dataOutWrittenTmp2 = Math.Min(_memoryStream.Length, dataOut.Length);
                dataOutWritten = 0;
                _memoryStream.Position = _lengthWritten;
                
                return FilterStatus.NeedMoreData;
                _log.Debug("End");
            }
            else
            {
                dataOutWritten = 0;

            }


            if (dataInRead < dataIn.Length)
            {
                return FilterStatus.NeedMoreData;
            }

            return FilterStatus.Done;
        }

        void IDisposable.Dispose()
        {
            //_memoryStream.Dispose();
            //_memoryStream = null;

            _wasDisposed = true;
            _log.Debug($"Done: Id: {_requestIdentifier}; Length: {_length}; LengthCount: {_lengthCount}");
        }


        public void Free()
        {
            var url = _requestUrl;
            var was = _wasDisposed;
            _memoryStream.Dispose();
            _memoryStream = null;
        }

        public void SetLength(int length)
        {
            _length = length;
        }
    }
}