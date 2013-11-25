using System.IO;
using System.Collections.Generic;
using System.Text;
using MyCmn;
using System.Net;


namespace System.Web.Mvc
{
    public static partial class MyHelper
    {


        /// <summary>
        /// 通过  HttpWebRequest 模拟 Post。
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="PostData"></param>
        /// <param name="ResponseAction"></param>
        /// <param name="ExceptionAction"></param>
        public static void Ajax(string Url, Func<HttpWebRequest, string> PostData, Action<string> ResponseAction, Action<string> ExceptionAction)
        {
            Action<Exception> ex = null;
            if (ExceptionAction != null)
            {
                ex = (e) => ExceptionAction(e.Message);
            }
            Ajax(Url, (req) =>
            {
                if (PostData != null)
                {
                    return System.Text.Encoding.UTF8.GetBytes(PostData(req));
                }
                else
                {
                    return null;
                }
            },
            (data, res) =>
            {
                var strContent = Encoding.UTF8.GetString(data);
                ResponseAction(strContent);

            },
            ex);
        }

        private class RequestState
        {
            // This class stores the State of the request.
            public const int BUFFER_SIZE = 1024;
            public List<byte> ResponseData;
            public byte[] BufferRead;
            public HttpWebRequest request;
            public HttpWebResponse response;
            public Stream streamResponse;
            public RequestState()
            {
                BufferRead = new byte[BUFFER_SIZE];
                ResponseData = new List<byte>();
                request = null;
                streamResponse = null;
            }
        }


        /// <summary>
        /// 通过 HttpWebRequest Post
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="RequestAction"></param>
        /// <param name="ResponseAction"></param>
        /// <param name="ExceptionAction"></param>
        public static void Ajax(string Url, Func<HttpWebRequest, byte[]> RequestAction, Action<byte[], HttpWebResponse> ResponseAction, Action<Exception> ExceptionAction)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";

            //注意：使用 "application/x-www-form-urlencoded" 参数会死的很惨！^_^
            request.ContentType = "application/octet-stream";

            RequestState myRequestState = new RequestState();
            myRequestState.request = request;

            using (var requestStream = myRequestState.request.GetRequestStream())
            {
                if (RequestAction != null)
                {
                    var writeBytes = RequestAction(myRequestState.request);
                    if (writeBytes != null)
                    {
                        requestStream.Write(writeBytes, 0, writeBytes.Length.AsInt());
                    }
                }

                try
                {
                    myRequestState.response = request.GetResponse() as HttpWebResponse; //(new AsyncCallback(UploadCallBack), myRequestState);
                }
                catch (Exception ee)
                {
                    ExceptionAction(ee);
                    return;
                }

                if (ResponseAction != null)
                {
                    while (true)
                    {
                        var readLen = myRequestState.response.GetResponseStream().Read(myRequestState.BufferRead, 0, myRequestState.BufferRead.Length);
                        if (readLen > 0)
                        {
                            myRequestState.ResponseData.AddRange(myRequestState.BufferRead.Slice(0, readLen));
                        }
                        else
                        {
                            ResponseAction(myRequestState.ResponseData.ToArray(), myRequestState.response);
                            break;
                        }
                    }
                }
            }
        }


    }
}
