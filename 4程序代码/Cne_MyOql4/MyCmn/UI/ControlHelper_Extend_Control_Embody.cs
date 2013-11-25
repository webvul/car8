using System;
using System.Text;
using System.Web.UI;
using System.Web;
using System.Web.UI.WebControls;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Configuration;
using System.Drawing.Drawing2D;

namespace MyCmn
{
    public static partial class CmnProc
    {
        /// <summary>
        /// 该方法,是给 Button 类用的. [★]
        /// </summary>
        /// <param name="btn"></param>
        public static void ProcOneClick(this Button btn)
        {
            if (btn.Attributes["___ProcOneClicked"] == true.ToString()) return;

            btn.Attributes.Add(
                "onclick", btn.Attributes["onclick"] + @"this.disabled=true;"
                + (HttpContext.Current.CurrentHandler as Page).ClientScript.GetPostBackEventReference(btn, "")
                );

            btn.Attributes["___ProcOneClicked"] = true.ToString();
        }


        /// <summary>
        /// 根据 Value , 递归查找节点并删除. [★]
        /// </summary>
        /// <param name="MyTree"></param>
        /// <param name="NodeValue"></param>
        public static void RemoveNodeByValue(this TreeView MyTree, string NodeValue)
        {
            RemoveNodeByValue(MyTree.Nodes,NodeValue);
        }


        /// <summary>
        /// 根据 Value , 递归查找节点并删除. [★]
        /// </summary>
        /// <param name="MyTreeNodes"></param>
        /// <param name="NodeValue"></param>
        private static void RemoveNodeByValue(this TreeNodeCollection MyTreeNodes, string NodeValue)
        {
            for (int i = 0; i < MyTreeNodes.Count; i++)
            {
                if (MyTreeNodes[i].Value == NodeValue)
                {
                    MyTreeNodes.Remove(MyTreeNodes[i]);
                    break;
                }

                RemoveNodeByValue(MyTreeNodes[i].ChildNodes,NodeValue);
            }
        }
        /// <summary>
        /// 根据 Value 递归查找节点 [★]
        /// </summary>
        /// <param name="MyTree"></param>
        /// <param name="NodeValue"></param>
        /// <returns></returns>
        public static TreeNode FindNodeByValue(this TreeView MyTree, string NodeValue)
        {
            return FindNodeByValue(MyTree.Nodes, NodeValue);
        }

        /// <summary>
        /// 根据 Value 递归查找节点 [★]
        /// </summary>
        /// <param name="MyTreeNodes"></param>
        /// <param name="NodeValue"></param>
        /// <returns></returns>
        private static TreeNode FindNodeByValue(this TreeNodeCollection MyTreeNodes, string NodeValue)
        {
            for (int i = 0; i < MyTreeNodes.Count; i++)
            {
                if (MyTreeNodes[i].Value == NodeValue)
                {
                    return MyTreeNodes[i];
                }

                TreeNode tn = FindNodeByValue(MyTreeNodes[i].ChildNodes,NodeValue);
                if (tn != null) return tn;
            }

            return null;
        }




        /// <summary>
        /// 为了使 GridView 显示整齐的显示方式，而定义的类。 [★]
        /// </summary>
        public class DisplayText
        {
            /// <summary>
            /// 格式化之后， 显示的内容。
            /// </summary>
            public string Text { get; set; }

            /// <summary>
            /// 如果经过格式化的话，显示的 原始值。如果没有经过格式化， 显示 ""
            /// </summary>
            public string ToolTip { get; set; }
        }

        /// <summary>
        /// 针对 GridView 列的显示名称过长, 取指定配置文件里 GridLineTitleLen 指定的长度的 Text  [★]
        /// </summary>
        /// <param name="LongText">GridView 里要格式化列的显示值。</param>
        /// <returns>格式化后的对象。 包括显示的格式化的文本内容，和 提示。提示就是完整理的内容。</returns>
        public static DisplayText GetDisplayText(this string LongText)
        {
            return GetDisplayText(LongText, -1);
        }

        /// <summary>
        /// 针对 GridView 列的显示名称过长,设置控件里文本的格式。 [★]
        /// </summary>
        /// <param name="TheWebControlWithText">要设置显示文本的控件。</param>
        public static void SetDisplayText(this WebControl TheWebControlWithText)
        {
            SetDisplayText(TheWebControlWithText, -1);
        }

        /// <summary>
        /// 针对 GridView 列的显示名称过长, 设置控件里文本的格式。 [★]
        /// </summary>
        /// <param name="TheWebControlWithText">要设置显示文本的控件。</param>
        /// <param name="DisplayLength">要设置格式化显示文本的长度。</param>
        public static void SetDisplayText(this WebControl TheWebControlWithText, int DisplayLength)
        {
            Type type = TheWebControlWithText.GetType();
            PropertyInfo piText = type.GetProperty("Text");
            if (piText == null) return;

            object objText = piText.GetValue(TheWebControlWithText, null);
            if (objText == null) return;

            DisplayText disValue = ValueProc.AsString(objText).GetDisplayText(DisplayLength);

            piText.SetValue(TheWebControlWithText, disValue.Text, null);

            if (disValue.ToolTip.HasValue() == false) return;

            PropertyInfo piWithToolTip = type.GetProperty("ToolTip");
            if (piWithToolTip != null)
            {
                piWithToolTip.SetValue(TheWebControlWithText, disValue.ToolTip, null);
            }
        }
        /// <summary>
        /// 针对 GridView 列的显示名称过长, 取指定长度的 Text , [★]
        /// </summary>
        /// <param name="LongText"></param>
        /// <param name="DisplayLength"></param>
        /// <returns></returns>
        public static DisplayText GetDisplayText(this string LongText, int DisplayLength)
        {
            if (LongText.HasValue() == false) return new DisplayText();

            if (DisplayLength < 0)
            {
                string Len = ConfigurationManager.AppSettings["DisplayTextLength"];
                DisplayLength = Len.HasValue() ? ValueProc.AsInt(Len) : 20;
            }
            if (DisplayLength < 4) DisplayLength = 4;

            DisplayText retVal = new DisplayText();

            int bytesLen = UnicodeEncoding.Default.GetByteCount(LongText);
            if (bytesLen > DisplayLength)
            {
                int charLen = UnicodeEncoding.Default.GetCharCount(UnicodeEncoding.Default.GetBytes(LongText), 0, DisplayLength - 4);
                retVal.Text = LongText.Substring(0, charLen) + "...";
                retVal.ToolTip = LongText;
            }
            else
            {
                retVal.Text = LongText;
                retVal.ToolTip = "";
            }

            return retVal;
        }

        /// <summary>
        /// 计算控件内容的宽度，忽略字间距。
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="FontPt"></param>
        /// <returns></returns>
        public static int GetComputeControlWidth(string Text, Font FontPt)
        {
            Bitmap img = new Bitmap(1, 1);
            Graphics g = Graphics.FromImage(img);
            SizeF s = g.MeasureString(Text, FontPt);

            if (s.IsEmpty) return 0;

            return ValueProc.AsInt(Math.Ceiling(s.Width));
        }

        /// <summary>
        /// 单位转换 ， 磅 值转换为 像素 值。
        /// </summary>
        /// <param name="PtValue">磅值</param>
        /// <returns>转换后的 像素 值 。</returns>
        public static int GetPxValue(int PtValue)
        {
            return GetPxValue(PtValue, 96);
        }

        /// <summary>
        /// 单位转换 ， 像素 值转换为 磅 值。
        /// </summary>
        /// <param name="PxValue">像素</param>
        /// <returns>转换后的 磅 值 。</returns>
        public static int GetPtValue(int PxValue)
        {
            return GetPtValue(PxValue, 96);
        }

        /// <summary>
        /// 单位转换 ， 磅 值转换为 像素 值。
        /// </summary>
        /// <param name="PtValue">磅值</param>
        /// <param name="PDI">PDI ， 默认为 96 </param>
        /// <returns>转换后的 像素 值 。</returns>
        public static int GetPxValue(int PtValue, int PDI)
        {
            return ValueProc.AsInt(PtValue * PDI * 1.0 / 72);
        }

        /// <summary>
        /// 单位转换 ， 像素 值转换为 磅 值。
        /// </summary>
        /// <param name="PxValue">像素</param>
        /// <param name="PDI">PDI ， 默认为 96 </param>
        /// <returns>转换后的 磅 值 。</returns>
        public static int GetPtValue(int PxValue, int PDI)
        {
            return ValueProc.AsInt(PxValue * 72 * 1.0 / PDI);
        }

        [Flags]
        public enum ResizeImageMode
        {
            NoScale = 0,
            ScaleToBig = 1,
            ScaleToSmall = 2,
        }

        /// <summary>
        /// 自动按 ScaleToSmall 方式,覆盖原图.
        /// </summary>
        /// <param name="OriDiskFile"></param>
        /// <param name="MaxWidth"></param>
        /// <param name="MaxHeight"></param>
        public static bool AutoResizeImage(string OriDiskFile, int MaxWidth, int MaxHeight)
        {
            bool isOK = false;
            Bitmap map = ResizeImage(out isOK, OriDiskFile, MaxWidth == 0 ? int.MaxValue : MaxWidth, MaxHeight == 0 ? int.MaxValue : MaxHeight, ResizeImageMode.ScaleToSmall);

            if (isOK == false) return false;

            if (map != null)
            {
                map.Save(OriDiskFile, ImageFormat.Jpeg);
                return true;
            }
            return true;
        }

        private static Bitmap OpenImg(string OriDiskFile)
        {
            try
            {
                return Bitmap.FromFile(OriDiskFile) as Bitmap;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 缩放图片.
        /// </summary>
        /// <param name="IsOK"></param>
        /// <param name="OriDiskFile"></param>
        /// <param name="MaxWidth"></param>
        /// <param name="MaxHeight"></param>
        /// <param name="Mode"></param>
        /// <returns>是否出错。true 没有问题， false，出错了</returns>
        public static Bitmap ResizeImage(out bool IsOK, string OriDiskFile, int MaxWidth, int MaxHeight, ResizeImageMode Mode)
        {
            if (File.Exists(OriDiskFile) == false)
            {
                IsOK = false;
                return null;
            }

            Bitmap bitmapImg = OpenImg(OriDiskFile);

            if (bitmapImg == null)
            {
                IsOK = false;
                return null;
            }

            IsOK = true;

            //原宽 / 原高 = 现宽/现高
            //原高 / 原宽 = 现高/现宽

            int width = MaxWidth;
            int height = MaxHeight;

            if (Mode.Contains(ResizeImageMode.ScaleToSmall))
            {
                bool isDo = false;
                if (bitmapImg.Width > width)
                {
                    height = bitmapImg.Height * width / bitmapImg.Width;
                    isDo = true;
                }
                if (height > MaxHeight)
                {
                    height = MaxHeight;
                    width = bitmapImg.Height * height / bitmapImg.Width;
                    isDo = true;
                }

                if (isDo == false)
                {
                    bitmapImg.Dispose();
                    return null;
                }
            }
            else if (Mode.Contains(ResizeImageMode.ScaleToBig))
            {
                height = bitmapImg.Height * width / bitmapImg.Width;
                if (height < MaxHeight)
                {
                    height = MaxHeight;
                    width = bitmapImg.Width * height / bitmapImg.Height;
                }

                if (MaxWidth == bitmapImg.Width && MaxHeight == bitmapImg.Height)
                {
                    bitmapImg.Dispose();
                    return null;
                }
            }

            Bitmap retVal = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(retVal);

            // 插值算法的质量
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.DrawImage(bitmapImg, new Rectangle(0, 0, width, height), new Rectangle(0, 0, bitmapImg.Width, bitmapImg.Height), GraphicsUnit.Pixel);
            g.Dispose();
            bitmapImg.Dispose();

            return retVal;
        }
    }
}