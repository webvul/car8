using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using MyCmn;
using MyCon;

namespace System.Web.Mvc
{
    public static partial class MyHtmlHelper
    {
        public static string TxtLabel(this HtmlHelper html, string Id, string value, bool IsEdit, int maxLength)
        {
            if (IsEdit)
                return string.Format(@"<input type=""text"" value=""{1}"" id=""{0}"" />", Id, value.GetSafeValue());
            else if (maxLength <= 0)
            {
                return string.Format(@"<label type=""text"" id=""{0}"">{1}</label>", Id, value.GetSafeValue());
            }
            else
            {
                var disp = CmnProc.GetDisplayText(value, maxLength);
                return string.Format(@"<label type=""text"" id=""{0}"" title=""{2}"">{1}</label>", Id, disp.Text.GetSafeValue(),
                    disp.ToolTip.GetSafeValue()
                    );
            }
        }


        /// <summary>
        /// 仅返回  li 部分数据.外层需要添加 div.ul ,且需要自行添加脚本.
        /// </summary>
        /// <param name="html"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public static string MyPucker(this HtmlHelper html, IEnumerable<PuckerNode> Data, bool useHref)
        {
            (html.ViewDataContainer as MyMvcPage).RegisterUsingFile("mypucker.js", "~/Res/MyPucker/MyPucker.js");


            return string.Join("", Data.Select(o => o.ToString(useHref)).ToArray());
        }


        public static string MyTag(this HtmlHelper html, HtmlTextWriterTag tag, string Inner, object Attributes)
        {
            if (tag.IsIn(HtmlTextWriterTag.Br, HtmlTextWriterTag.Img, HtmlTextWriterTag.Input, HtmlTextWriterTag.Link,
                 HtmlTextWriterTag.Nobr, HtmlTextWriterTag.Meta)
                 )
            {
                return MyHelper.BeginTag(html, tag, Attributes);
            }
            else return MyHelper.BeginTag(html, tag, Attributes) + Inner + MyHelper.EndTag(html, tag);
        }



        public static string GetContentType(string fileName)
        {
            var fileExt =   fileName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries).Last().ToLower();// .TrimStart('.').ToLower();
            string contentType = "application/octet-stream";

            //xml按 excel 
            if (fileName == "xml") return "application/vnd.ms-excel";

            #region http://hi.baidu.com/laixiaozi/item/c906eb2df213fb0a43634a4b
            //-------------------------------
            if (fileExt == "001") contentType = "application/x-001";
            if (fileExt == "301") contentType = "application/x-301";
            if (fileExt == "323") contentType = "text/h323";
            if (fileExt == "906") contentType = "application/x-906";
            if (fileExt == "907") contentType = "drawing/907";
            if (fileExt == "a11") contentType = "application/x-a11";
            if (fileExt == "acp") contentType = "audio/x-mei-aac";
            if (fileExt == "ai") contentType = "application/postscript";
            if (fileExt == "aif") contentType = "audio/aiff";
            if (fileExt == "aifc") contentType = "audio/aiff";
            if (fileExt == "aiff") contentType = "audio/aiff";
            if (fileExt == "anv") contentType = "application/x-anv";
            if (fileExt == "asa") contentType = "text/asa";
            if (fileExt == "asf") contentType = "video/x-ms-asf";
            if (fileExt == "asp") contentType = "text/asp";
            if (fileExt == "asx") contentType = "video/x-ms-asf";
            if (fileExt == "au") contentType = "audio/basic";
            if (fileExt == "avi") contentType = "video/avi";
            if (fileExt == "awf") contentType = "application/vnd.adobe.workflow";
            if (fileExt == "biz") contentType = "text/xml";
            if (fileExt == "bmp") contentType = "application/x-bmp";
            if (fileExt == "bot") contentType = "application/x-bot";
            if (fileExt == "c4t") contentType = "application/x-c4t";
            if (fileExt == "c90") contentType = "application/x-c90";
            if (fileExt == "cal") contentType = "application/x-cals";
            if (fileExt == "cat") contentType = "application/vnd.ms-pki.seccat";
            if (fileExt == "cdf") contentType = "application/x-netcdf";
            if (fileExt == "cdr") contentType = "application/x-cdr";
            if (fileExt == "cel") contentType = "application/x-cel";
            if (fileExt == "cer") contentType = "application/x-x509-ca-cert";
            if (fileExt == "cg4") contentType = "application/x-g4";
            if (fileExt == "cgm") contentType = "application/x-cgm";
            if (fileExt == "cit") contentType = "application/x-cit";
            if (fileExt == "class") contentType = "java/*";
            if (fileExt == "cml") contentType = "text/xml";
            if (fileExt == "cmp") contentType = "application/x-cmp";
            if (fileExt == "cmx") contentType = "application/x-cmx";
            if (fileExt == "cot") contentType = "application/x-cot";
            if (fileExt == "crl") contentType = "application/pkix-crl";
            if (fileExt == "crt") contentType = "application/x-x509-ca-cert";
            if (fileExt == "csi") contentType = "application/x-csi";
            if (fileExt == "css") contentType = "text/css";
            if (fileExt == "cut") contentType = "application/x-cut";
            if (fileExt == "dbf") contentType = "application/x-dbf";
            if (fileExt == "dbm") contentType = "application/x-dbm";
            if (fileExt == "dbx") contentType = "application/x-dbx";
            if (fileExt == "dcd") contentType = "text/xml";
            if (fileExt == "dcx") contentType = "application/x-dcx";
            if (fileExt == "der") contentType = "application/x-x509-ca-cert";
            if (fileExt == "dgn") contentType = "application/x-dgn";
            if (fileExt == "dib") contentType = "application/x-dib";
            if (fileExt == "dll") contentType = "application/x-msdownload";
            if (fileExt == "doc") contentType = "application/msword";
            if (fileExt == "dot") contentType = "application/msword";
            if (fileExt == "drw") contentType = "application/x-drw";
            if (fileExt == "dtd") contentType = "text/xml";
            if (fileExt == "dwf") contentType = "Model/vnd.dwf";
            if (fileExt == "dwf") contentType = "application/x-dwf";
            if (fileExt == "dwg") contentType = "application/x-dwg";
            if (fileExt == "dxb") contentType = "application/x-dxb";
            if (fileExt == "dxf") contentType = "application/x-dxf";
            if (fileExt == "edn") contentType = "application/vnd.adobe.edn";
            if (fileExt == "emf") contentType = "application/x-emf";
            if (fileExt == "eml") contentType = "message/rfc822";
            if (fileExt == "ent") contentType = "text/xml";
            if (fileExt == "epi") contentType = "application/x-epi";
            if (fileExt == "eps") contentType = "application/x-ps";
            if (fileExt == "eps") contentType = "application/postscript";
            if (fileExt == "etd") contentType = "application/x-ebx";
            if (fileExt == "exe") contentType = "application/x-msdownload";
            if (fileExt == "fax") contentType = "image/fax";
            if (fileExt == "fdf") contentType = "application/vnd.fdf";
            if (fileExt == "fif") contentType = "application/fractals";
            if (fileExt == "fo") contentType = "text/xml";
            if (fileExt == "frm") contentType = "application/x-frm";
            if (fileExt == "g4") contentType = "application/x-g4";
            if (fileExt == "gbr") contentType = "application/x-gbr";
            if (fileExt == "gcd") contentType = "application/x-gcd";
            if (fileExt == "gif") contentType = "image/gif";
            if (fileExt == "gl2") contentType = "application/x-gl2";
            if (fileExt == "gp4") contentType = "application/x-gp4";
            if (fileExt == "hgl") contentType = "application/x-hgl";
            if (fileExt == "hmr") contentType = "application/x-hmr";
            if (fileExt == "hpg") contentType = "application/x-hpgl";
            if (fileExt == "hpl") contentType = "application/x-hpl";
            if (fileExt == "hqx") contentType = "application/mac-binhex40";
            if (fileExt == "hrf") contentType = "application/x-hrf";
            if (fileExt == "hta") contentType = "application/hta";
            if (fileExt == "htc") contentType = "text/x-component";
            if (fileExt == "htm") contentType = "text/html";
            if (fileExt == "html") contentType = "text/html";
            if (fileExt == "htt") contentType = "text/webviewhtml";
            if (fileExt == "htx") contentType = "text/html";
            if (fileExt == "icb") contentType = "application/x-icb";
            if (fileExt == "ico") contentType = "image/x-icon";
            if (fileExt == "ico") contentType = "application/x-ico";
            if (fileExt == "iff") contentType = "application/x-iff";
            if (fileExt == "ig4") contentType = "application/x-g4";
            if (fileExt == "igs") contentType = "application/x-igs";
            if (fileExt == "iii") contentType = "application/x-iphone";
            if (fileExt == "img") contentType = "application/x-img";
            if (fileExt == "ins") contentType = "application/x-internet-signup";
            if (fileExt == "isp") contentType = "application/x-internet-signup";
            if (fileExt == "IVF") contentType = "video/x-ivf";
            if (fileExt == "java") contentType = "java/*";
            if (fileExt == "jfif") contentType = "image/jpeg";
            if (fileExt == "jpe") contentType = "image/jpeg";
            if (fileExt == "jpe") contentType = "application/x-jpe";
            if (fileExt == "jpeg") contentType = "image/jpeg";
            if (fileExt == "jpg") contentType = "image/jpeg";
            if (fileExt == "jpg") contentType = "application/x-jpg";
            if (fileExt == "js") contentType = "application/x-javascript";
            if (fileExt == "jsp") contentType = "text/html";
            if (fileExt == "la1") contentType = "audio/x-liquid-file";
            if (fileExt == "lar") contentType = "application/x-laplayer-reg";
            if (fileExt == "latex") contentType = "application/x-latex";
            if (fileExt == "lavs") contentType = "audio/x-liquid-secure";
            if (fileExt == "lbm") contentType = "application/x-lbm";
            if (fileExt == "lmsff") contentType = "audio/x-la-lms";
            if (fileExt == "ls") contentType = "application/x-javascript";
            if (fileExt == "ltr") contentType = "application/x-ltr";
            if (fileExt == "m1v") contentType = "video/x-mpeg";
            if (fileExt == "m2v") contentType = "video/x-mpeg";
            if (fileExt == "m3u") contentType = "audio/mpegurl";
            if (fileExt == "m4e") contentType = "video/mpeg4";
            if (fileExt == "mac") contentType = "application/x-mac";
            if (fileExt == "man") contentType = "application/x-troff-man";
            if (fileExt == "math") contentType = "text/xml";
            if (fileExt == "mdb") contentType = "application/msaccess";
            if (fileExt == "mdb") contentType = "application/x-mdb";
            if (fileExt == "mfp") contentType = "application/x-shockwave-flash";
            if (fileExt == "mht") contentType = "message/rfc822";
            if (fileExt == "mhtml") contentType = "message/rfc822";
            if (fileExt == "mi") contentType = "application/x-mi";
            if (fileExt == "mid") contentType = "audio/mid";
            if (fileExt == "midi") contentType = "audio/mid";
            if (fileExt == "mil") contentType = "application/x-mil";
            if (fileExt == "mml") contentType = "text/xml";
            if (fileExt == "mnd") contentType = "audio/x-musicnet-download";
            if (fileExt == "mns") contentType = "audio/x-musicnet-stream";
            if (fileExt == "mocha") contentType = "application/x-javascript";
            if (fileExt == "movie") contentType = "video/x-sgi-movie";
            if (fileExt == "mp1") contentType = "audio/mp1";
            if (fileExt == "mp2") contentType = "audio/mp2";
            if (fileExt == "mp2v") contentType = "video/mpeg";
            if (fileExt == "mp3") contentType = "audio/mp3";
            if (fileExt == "mp4") contentType = "video/mpeg4";
            if (fileExt == "mpa") contentType = "video/x-mpg";
            if (fileExt == "mpd") contentType = "application/vnd.ms-project";
            if (fileExt == "mpe") contentType = "video/x-mpeg";
            if (fileExt == "mpeg") contentType = "video/mpg";
            if (fileExt == "mpg") contentType = "video/mpg";
            if (fileExt == "mpga") contentType = "audio/rn-mpeg";
            if (fileExt == "mpp") contentType = "application/vnd.ms-project";
            if (fileExt == "mps") contentType = "video/x-mpeg";
            if (fileExt == "mpt") contentType = "application/vnd.ms-project";
            if (fileExt == "mpv") contentType = "video/mpg";
            if (fileExt == "mpv2") contentType = "video/mpeg";
            if (fileExt == "mpw") contentType = "application/vnd.ms-project";
            if (fileExt == "mpx") contentType = "application/vnd.ms-project";
            if (fileExt == "mtx") contentType = "text/xml";
            if (fileExt == "mxp") contentType = "application/x-mmxp";
            if (fileExt == "net") contentType = "image/pnetvue";
            if (fileExt == "nrf") contentType = "application/x-nrf";
            if (fileExt == "nws") contentType = "message/rfc822";
            if (fileExt == "odc") contentType = "text/x-ms-odc";
            if (fileExt == "out") contentType = "application/x-out";
            if (fileExt == "p10") contentType = "application/pkcs10";
            if (fileExt == "p12") contentType = "application/x-pkcs12";
            if (fileExt == "p7b") contentType = "application/x-pkcs7-certificates";
            if (fileExt == "p7c") contentType = "application/pkcs7-mime";
            if (fileExt == "p7m") contentType = "application/pkcs7-mime";
            if (fileExt == "p7r") contentType = "application/x-pkcs7-certreqresp";
            if (fileExt == "p7s") contentType = "application/pkcs7-signature";
            if (fileExt == "pc5") contentType = "application/x-pc5";
            if (fileExt == "pci") contentType = "application/x-pci";
            if (fileExt == "pcl") contentType = "application/x-pcl";
            if (fileExt == "pcx") contentType = "application/x-pcx";
            if (fileExt == "pdf") contentType = "application/pdf";
            if (fileExt == "pdf") contentType = "application/pdf";
            if (fileExt == "pdx") contentType = "application/vnd.adobe.pdx";
            if (fileExt == "pfx") contentType = "application/x-pkcs12";
            if (fileExt == "pgl") contentType = "application/x-pgl";
            if (fileExt == "pic") contentType = "application/x-pic";
            if (fileExt == "pko") contentType = "application/vnd.ms-pki.pko";
            if (fileExt == "pl") contentType = "application/x-perl";
            if (fileExt == "plg") contentType = "text/html";
            if (fileExt == "pls") contentType = "audio/scpls";
            if (fileExt == "plt") contentType = "application/x-plt";
            if (fileExt == "png") contentType = "image/png";
            if (fileExt == "png") contentType = "application/x-png";
            if (fileExt == "pot") contentType = "application/vnd.ms-powerpoint";
            if (fileExt == "ppa") contentType = "application/vnd.ms-powerpoint";
            if (fileExt == "ppm") contentType = "application/x-ppm";
            if (fileExt == "pps") contentType = "application/vnd.ms-powerpoint";
            if (fileExt == "ppt") contentType = "application/vnd.ms-powerpoint";
            if (fileExt == "ppt") contentType = "application/x-ppt";
            if (fileExt == "pr") contentType = "application/x-pr";
            if (fileExt == "prf") contentType = "application/pics-rules";
            if (fileExt == "prn") contentType = "application/x-prn";
            if (fileExt == "prt") contentType = "application/x-prt";
            if (fileExt == "ps") contentType = "application/x-ps";
            if (fileExt == "ps") contentType = "application/postscript";
            if (fileExt == "ptn") contentType = "application/x-ptn";
            if (fileExt == "pwz") contentType = "application/vnd.ms-powerpoint";
            if (fileExt == "r3t") contentType = "text/vnd.rn-realtext3d";
            if (fileExt == "ra") contentType = "audio/vnd.rn-realaudio";
            if (fileExt == "ram") contentType = "audio/x-pn-realaudio";
            if (fileExt == "ras") contentType = "application/x-ras";
            if (fileExt == "rat") contentType = "application/rat-file";
            if (fileExt == "rdf") contentType = "text/xml";
            if (fileExt == "rec") contentType = "application/vnd.rn-recording";
            if (fileExt == "red") contentType = "application/x-red";
            if (fileExt == "rgb") contentType = "application/x-rgb";
            if (fileExt == "rjs") contentType = "application/vnd.rn-realsystem-rjs";
            if (fileExt == "rjt") contentType = "application/vnd.rn-realsystem-rjt";
            if (fileExt == "rlc") contentType = "application/x-rlc";
            if (fileExt == "rle") contentType = "application/x-rle";
            if (fileExt == "rm") contentType = "application/vnd.rn-realmedia";
            if (fileExt == "rmf") contentType = "application/vnd.adobe.rmf";
            if (fileExt == "rmi") contentType = "audio/mid";
            if (fileExt == "rmj") contentType = "application/vnd.rn-realsystem-rmj";
            if (fileExt == "rmm") contentType = "audio/x-pn-realaudio";
            if (fileExt == "rmp") contentType = "application/vnd.rn-rn_music_package";
            if (fileExt == "rms") contentType = "application/vnd.rn-realmedia-secure";
            if (fileExt == "rmvb") contentType = "application/vnd.rn-realmedia-vbr";
            if (fileExt == "rmx") contentType = "application/vnd.rn-realsystem-rmx";
            if (fileExt == "rnx") contentType = "application/vnd.rn-realplayer";
            if (fileExt == "rp") contentType = "image/vnd.rn-realpix";
            if (fileExt == "rpm") contentType = "audio/x-pn-realaudio-plugin";
            if (fileExt == "rsml") contentType = "application/vnd.rn-rsml";
            if (fileExt == "rt") contentType = "text/vnd.rn-realtext";
            if (fileExt == "rtf") contentType = "application/msword";
            if (fileExt == "rtf") contentType = "application/x-rtf";
            if (fileExt == "rv") contentType = "video/vnd.rn-realvideo";
            if (fileExt == "sam") contentType = "application/x-sam";
            if (fileExt == "sat") contentType = "application/x-sat";
            if (fileExt == "sdp") contentType = "application/sdp";
            if (fileExt == "sdw") contentType = "application/x-sdw";
            if (fileExt == "sit") contentType = "application/x-stuffit";
            if (fileExt == "slb") contentType = "application/x-slb";
            if (fileExt == "sld") contentType = "application/x-sld";
            if (fileExt == "slk") contentType = "drawing/x-slk";
            if (fileExt == "smi") contentType = "application/smil";
            if (fileExt == "smil") contentType = "application/smil";
            if (fileExt == "smk") contentType = "application/x-smk";
            if (fileExt == "snd") contentType = "audio/basic";
            if (fileExt == "sol") contentType = "text/plain";
            if (fileExt == "sor") contentType = "text/plain";
            if (fileExt == "spc") contentType = "application/x-pkcs7-certificates";
            if (fileExt == "spl") contentType = "application/futuresplash";
            if (fileExt == "spp") contentType = "text/xml";
            if (fileExt == "ssm") contentType = "application/streamingmedia";
            if (fileExt == "sst") contentType = "application/vnd.ms-pki.certstore";
            if (fileExt == "stl") contentType = "application/vnd.ms-pki.stl";
            if (fileExt == "stm") contentType = "text/html";
            if (fileExt == "sty") contentType = "application/x-sty";
            if (fileExt == "svg") contentType = "text/xml";
            if (fileExt == "swf") contentType = "application/x-shockwave-flash";
            if (fileExt == "tdf") contentType = "application/x-tdf";
            if (fileExt == "tg4") contentType = "application/x-tg4";
            if (fileExt == "tga") contentType = "application/x-tga";
            if (fileExt == "tif") contentType = "image/tiff";
            if (fileExt == "tif") contentType = "application/x-tif";
            if (fileExt == "tiff") contentType = "image/tiff";
            if (fileExt == "tld") contentType = "text/xml";
            if (fileExt == "top") contentType = "drawing/x-top";
            if (fileExt == "torrent") contentType = "application/x-bittorrent";
            if (fileExt == "tsd") contentType = "text/xml";
            if (fileExt == "txt") contentType = "text/plain";
            if (fileExt == "uin") contentType = "application/x-icq";
            if (fileExt == "uls") contentType = "text/iuls";
            if (fileExt == "vcf") contentType = "text/x-vcard";
            if (fileExt == "vda") contentType = "application/x-vda";
            if (fileExt == "vdx") contentType = "application/vnd.visio";
            if (fileExt == "vml") contentType = "text/xml";
            if (fileExt == "vpg") contentType = "application/x-vpeg005";
            if (fileExt == "vsd") contentType = "application/vnd.visio";
            if (fileExt == "vsd") contentType = "application/x-vsd";
            if (fileExt == "vss") contentType = "application/vnd.visio";
            if (fileExt == "vst") contentType = "application/vnd.visio";
            if (fileExt == "vst") contentType = "application/x-vst";
            if (fileExt == "vsw") contentType = "application/vnd.visio";
            if (fileExt == "vsx") contentType = "application/vnd.visio";
            if (fileExt == "vtx") contentType = "application/vnd.visio";
            if (fileExt == "vxml") contentType = "text/xml";
            if (fileExt == "wav") contentType = "audio/wav";
            if (fileExt == "wax") contentType = "audio/x-ms-wax";
            if (fileExt == "wb1") contentType = "application/x-wb1";
            if (fileExt == "wb2") contentType = "application/x-wb2";
            if (fileExt == "wb3") contentType = "application/x-wb3";
            if (fileExt == "wbmp") contentType = "image/vnd.wap.wbmp";
            if (fileExt == "wiz") contentType = "application/msword";
            if (fileExt == "wk3") contentType = "application/x-wk3";
            if (fileExt == "wk4") contentType = "application/x-wk4";
            if (fileExt == "wkq") contentType = "application/x-wkq";
            if (fileExt == "wks") contentType = "application/x-wks";
            if (fileExt == "wm") contentType = "video/x-ms-wm";
            if (fileExt == "wma") contentType = "audio/x-ms-wma";
            if (fileExt == "wmd") contentType = "application/x-ms-wmd";
            if (fileExt == "wmf") contentType = "application/x-wmf";
            if (fileExt == "wml") contentType = "text/vnd.wap.wml";
            if (fileExt == "wmv") contentType = "video/x-ms-wmv";
            if (fileExt == "wmx") contentType = "video/x-ms-wmx";
            if (fileExt == "wmz") contentType = "application/x-ms-wmz";
            if (fileExt == "wp6") contentType = "application/x-wp6";
            if (fileExt == "wpd") contentType = "application/x-wpd";
            if (fileExt == "wpg") contentType = "application/x-wpg";
            if (fileExt == "wpl") contentType = "application/vnd.ms-wpl";
            if (fileExt == "wq1") contentType = "application/x-wq1";
            if (fileExt == "wr1") contentType = "application/x-wr1";
            if (fileExt == "wri") contentType = "application/x-wri";
            if (fileExt == "wrk") contentType = "application/x-wrk";
            if (fileExt == "ws") contentType = "application/x-ws";
            if (fileExt == "ws2") contentType = "application/x-ws";
            if (fileExt == "wsc") contentType = "text/scriptlet";
            if (fileExt == "wsdl") contentType = "text/xml";
            if (fileExt == "wvx") contentType = "video/x-ms-wvx";
            if (fileExt == "xdp") contentType = "application/vnd.adobe.xdp";
            if (fileExt == "xdr") contentType = "text/xml";
            if (fileExt == "xfd") contentType = "application/vnd.adobe.xfd";
            if (fileExt == "xfdf") contentType = "application/vnd.adobe.xfdf";
            if (fileExt == "xhtml") contentType = "text/html";
            if (fileExt == "xls") contentType = "application/vnd.ms-excel";
            if (fileExt == "xls") contentType = "application/x-xls";
            if (fileExt == "xlw") contentType = "application/x-xlw";
            if (fileExt == "xml") contentType = "text/xml";
            if (fileExt == "xpl") contentType = "audio/scpls";
            if (fileExt == "xq") contentType = "text/xml";
            if (fileExt == "xql") contentType = "text/xml";
            if (fileExt == "xquery") contentType = "text/xml";
            if (fileExt == "xsd") contentType = "text/xml";
            if (fileExt == "xsl") contentType = "text/xml";
            if (fileExt == "xslt") contentType = "text/xml";
            if (fileExt == "xwd") contentType = "application/x-xwd";
            if (fileExt == "x_b") contentType = "application/x-x_b";
            if (fileExt == "x_t") contentType = "application/x-x_t";
#endregion

            return contentType;

        }
    }
}
