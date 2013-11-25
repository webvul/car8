using System;
using System.Web;
using System.IO;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;

namespace MyCmn
{
    public static partial class CmnProc
    {
        /// <summary>
        /// 使GIF图片背景透明. [★]
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Bitmap MakeTransparentGif(this Bitmap bitmap, Color color)
        {
            byte R = color.R;
            byte G = color.G;
            byte B = color.B;
            MemoryStream fin = new MemoryStream();
            bitmap.Save(fin, ImageFormat.Gif);
            MemoryStream fout = new MemoryStream((int)fin.Length);
            int count = 0;
            byte[] buf = new byte[256];
            byte transparentIdx = 0;
            fin.Seek(0, SeekOrigin.Begin);
            //header    
            count = fin.Read(buf, 0, 13);
            if ((buf[0] != 71) || (buf[1] != 73) || (buf[2] != 70))
                return null;
            //GIF    
            fout.Write(buf, 0, 13);
            int i = 0;
            if ((buf[10] & 0x80) > 0)
            {
                i = 1 << ((buf[10] & 7) + 1) == 256 ? 256 : 0;
            }
            for (; i != 0; i--)
            {
                fin.Read(buf, 0, 3);
                if ((buf[0] == R) && (buf[1] == G) && (buf[2] == B))
                {
                    transparentIdx = (byte)(256 - i);
                }
                fout.Write(buf, 0, 3);
            }
            bool gcePresent = false;
            while (true)
            {
                fin.Read(buf, 0, 1);
                fout.Write(buf, 0, 1);
                if (buf[0] != 0x21)
                    break;
                fin.Read(buf, 0, 1);
                fout.Write(buf, 0, 1);
                gcePresent = (buf[0] == 0xf9);
                while (true)
                {
                    fin.Read(buf, 0, 1);
                    fout.Write(buf, 0, 1);
                    if (buf[0] == 0)
                        break;
                    count = buf[0];
                    if (fin.Read(buf, 0, count) != count)
                        return null;
                    if (gcePresent)
                    {
                        if (count == 4)
                        {
                            buf[0] = 0x1;
                            buf[3] = transparentIdx;
                        }
                    }
                    fout.Write(buf, 0, count);
                }
            }
            while (count > 0)
            {
                count = fin.Read(buf, 0, 1);
                fout.Write(buf, 0, 1);
            }
            fin.Close();
            fout.Flush();
            return new Bitmap(fout);
        }

        public static void MakeTransparentGif(string FileName)
        {
            Bitmap map = new Bitmap(FileName);
            Bitmap retVal = MakeTransparentGif(map, Color.Black);
            if (retVal != null)
            {
                map.Dispose();
                retVal.Save(FileName, ImageFormat.Gif);
            }
        }

        /// <summary>
        /// 根据现在执行的进度.判断以前是不是曾取过这个段的颜色值. [★]
        /// </summary>
        /// <param name="Denominator">分母,即时份值,即现在的深度.</param>
        /// <param name="Numerator">分子,循环中的即时份值.</param>
        /// <returns>如果存在,返回true,否则,返回false</returns>
        private static bool isEcho(int Denominator, int Numerator)
        {
            float fraction = (Convert.ToSingle(Numerator) / Convert.ToSingle(Denominator));
            for (float i = 2; i < Denominator; i++)
            {
                for (float j = 1; j < i; j++)
                {
                    if (fraction == j / i)
                        return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 根据要取的颜色数,返回一个颜色数组.[★]
        /// </summary>
        /// <param name="nColor">要取多少个颜色</param>
        /// <param name="MinColor">要取颜色的最小值。</param>
        /// <param name="MaxColor">要取颜色的最大值。</param>
        /// <returns>返回的颜色数组.</returns>
        public static Color[] GetColor(int nColor, int MinColor, int MaxColor)
        {
            int m_maxCol = MaxColor;
            //最大色值.
            int m_minCol = MinColor;
            //最小色值.
            int m_Deapth = 10;
            //暂称为份值.所分的最大值.如果是10的话.可找到825个颜色值.11的话,可找到1116个颜色值.15的话,可以找到2970个颜色值.20的话,可找到7146个颜色值.
            ArrayList alCol = new ArrayList();
            //存储颜色值的链表.
            Color[,] col = new Color[2, 3] {
				//第一行.是A类的种子.A类指只有一个颜色值.第二行,是B类的种子,B类指有两个颜色值的种子 .
				{
					Color.FromArgb (m_maxCol, m_minCol, m_minCol),
					Color.FromArgb (m_minCol, m_maxCol, m_minCol),
					Color.FromArgb (m_minCol, m_minCol, m_maxCol)
				},
				{
					Color.FromArgb (m_maxCol, m_maxCol, m_minCol),
					Color.FromArgb (m_maxCol, m_minCol, m_maxCol),
					Color.FromArgb (m_minCol, m_maxCol, m_maxCol)
				}
			};
            //把种子值加入到颜色链表中.
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    alCol.Add(col[i, j]);
                }
            }
            //循环份值.
            for (int dpth = 2; dpth < m_Deapth; dpth++)
            {
                //先找出A类的颜色值.
                for (int typeA = 1; typeA < dpth; typeA++)
                {
                    if (isEcho(dpth, typeA))
                        continue;
                    //如果这段颜色值已存在.就不要取了.
                    alCol.Add(Color.FromArgb(m_maxCol * typeA / dpth, m_minCol, m_minCol));
                    //循环每一个档的色值.
                    alCol.Add(Color.FromArgb(m_minCol, m_maxCol * typeA / dpth, m_minCol));
                    alCol.Add(Color.FromArgb(m_minCol, m_minCol, m_maxCol * typeA / dpth));
                }
                //再找出B类的颜色值.先循环第一个位置的色值.
                for (int typeB = 1; typeB <= dpth; typeB++)
                {
                    if (isEcho(dpth, typeB))
                        continue;
                    //如果这段颜色值已存在,就不要取了.
                    //再循环第二个位置的色值.
                    for (int i = 1; i <= dpth; i++)
                    {
                        if (typeB == i && i == dpth)
                            continue;
                        //在B类中会重复一个种子值.去掉.
                        alCol.Add(Color.FromArgb(m_maxCol * typeB / dpth, m_maxCol * i / dpth, m_minCol));
                        alCol.Add(Color.FromArgb(m_maxCol * typeB / dpth, m_minCol, m_maxCol * i / dpth));
                        alCol.Add(Color.FromArgb(m_minCol, m_maxCol * typeB / dpth, m_maxCol * i / dpth));
                    }
                }
                if (alCol.Count >= nColor)
                    break;
                //如果取够了颜色值,停止.这可能也取多了.
            }
            Color[] retCol = new Color[nColor];
            //如果取多了,就过滤掉.
            for (int i = 0; i < nColor; i++)
            {
                retCol[i] = (Color)alCol[i];
            }
            return retCol;
        }

        /// <summary>
        /// 返回 Color 对象 的 十六进制表示形式。 [★]
        /// </summary>
        /// <param name="TheColorValue"></param>
        /// <returns></returns>
        public static string ToHexColorString(this Color TheColorValue)
        {
            if (TheColorValue.IsEmpty)
                return null;
            if (TheColorValue.A != 255)
                return Color.Transparent.Name;

            return "#" + TheColorValue.R.ToString("X2") + TheColorValue.G.ToString("X2") + TheColorValue.B.ToString("X2");
        }

        /// <summary>
        /// 将十六进制 表式形式的颜色值（#123456） 转换为 Color 对象。 [★]
        /// </summary>
        /// <param name="TheColorValue"></param>
        /// <returns></returns>
        public static Color ToColor(string TheColorValue)
        {
            if (TheColorValue.HasValue() == false)
                return Color.Empty;
            if (TheColorValue.StartsWith("#") == false)
                return Color.FromName(TheColorValue);
            if (TheColorValue.Length != 7)
                return Color.Empty;


            return Color.FromArgb(byte.Parse(TheColorValue.Substring(1, 2), System.Globalization.NumberStyles.HexNumber), byte.Parse(TheColorValue.Substring(3, 2), System.Globalization.NumberStyles.HexNumber), byte.Parse(TheColorValue.Substring(5, 2), System.Globalization.NumberStyles.HexNumber));
        }

        /// <summary>
        /// 传入的参数是 /MyWeb/WebResource.axd?d= 中 d的部分.返回它在 Dll 中资源的名称.
        /// Window 和 Linux 下实现方式是不同的， 所以这里要分别对待。
        /// </summary>
        /// <param name="QueryResourceID">加密的嵌入式资源的TypeID</param>
        /// <returns>Dll中嵌入式资源的名称 .</returns>
        public static string GetEmbedResourceName(string QueryResourceID)
        {
            if (string.Equals(Environment.OSVersion.Platform.ToString(), "Win32NT", StringComparison.CurrentCultureIgnoreCase))
            {
                Type type = typeof(System.Web.UI.Page);
                System.Reflection.MethodInfo mi = type.GetMethod("DecryptString", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static, null, new Type[] { typeof(string) }, null);
                string[] result = mi.Invoke((HttpContext.Current.CurrentHandler), new object[] { QueryResourceID }).ToString().Split('|');
                return result[1];
            }
            else if (string.Equals(Environment.OSVersion.Platform.ToString(), "Unix", StringComparison.CurrentCultureIgnoreCase))
            {
                Type type = typeof(System.Web.UI.Page);
                System.Reflection.MethodInfo mi = type.GetMethod("DecryptString", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static, null, new Type[] { typeof(string) }, null);
                if (mi != null)
                {
                    string[] result = mi.Invoke((HttpContext.Current.CurrentHandler), new object[] { QueryResourceID }).ToString().Split('|');
                    return result[1];
                }
                else return "";
            }
            else return "";
        }
    }
}
