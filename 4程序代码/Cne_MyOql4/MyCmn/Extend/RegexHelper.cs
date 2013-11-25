using System;
using System.Text;
using System.Text.RegularExpressions;

namespace MyCmn
{
    public class ValidationHelper
    {
        public class RegexHelper
        {
            public static bool Validate(string Test, string Pattern)
            {
                return Regex.IsMatch(Test, Pattern);
            }
        }
        #region 判断对象是否为空
        /// <summary>
        /// 判断对象是否为空，为空返回true
        /// </summary>
        /// <typeparam name="T">要验证的对象的类型</typeparam>
        /// <param name="data">要验证的对象</param>       
        public static bool IsNullOrEmpty<T>(T data)
        {
            //如果为null
            if (data == null)
            {
                return true;
            }

            //如果为""
            if (data.GetType() == typeof(String))
            {
                if (string.IsNullOrEmpty(data.ToString().Trim()))
                {
                    return true;
                }
            }

            //如果为DBNull
            if (data.GetType() == typeof(DBNull))
            {
                return true;
            }

            //不为空
            return false;
        }

        /// <summary>
        /// 判断对象是否为空，为空返回true
        /// </summary>
        /// <param name="data">要验证的对象</param>
        public static bool IsNullOrEmpty(object data)
        {
            //如果为null
            if (data == null)
            {
                return true;
            }

            //如果为""
            if (data.GetType() == typeof(String))
            {
                if (string.IsNullOrEmpty(data.ToString().Trim()))
                {
                    return true;
                }
            }

            //如果为DBNull
            if (data.GetType() == typeof(DBNull))
            {
                return true;
            }

            //不为空
            return false;
        }
        #endregion

        #region 验证IP地址是否合法
        /// <summary>
        /// 验证IP地址是否合法
        /// </summary>
        /// <param name="ip">要验证的IP地址</param>       
        public static bool IsIP(string ip)
        {
            //如果为空，认为验证合格
            if (IsNullOrEmpty(ip))
            {
                return true;
            }

            //清除要验证字符串中的空格
            ip = ip.Trim();

            //模式字符串
            string pattern = @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$";

            //验证
            return RegexHelper.Validate(ip, pattern);
        }
        #endregion

        #region  验证EMail是否合法
        /// <summary>
        /// 验证EMail是否合法
        /// </summary>
        /// <param name="email">要验证的Email</param>
        public static bool IsEmail(string email)
        {
            //如果为空，认为验证合格
            if (IsNullOrEmpty(email))
            {
                return true;
            }

            //清除要验证字符串中的空格
            email = email.Trim();

            //模式字符串
            string pattern = @"^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$";

            //验证
            return RegexHelper.Validate(email, pattern);
        }
        #endregion

        #region 验证是否为整数
        /// <summary>
        /// 验证是否为整数
        /// </summary>
        /// <param name="number">要验证的整数</param>       
        public static bool IsInt(string number)
        {
            //如果为空，认为验证合格
            if (IsNullOrEmpty(number))
            {
                return true;
            }

            //清除要验证字符串中的空格
            number = number.Trim();

            //模式字符串
            string pattern = @"^[1-9]+[0-9]*$";

            //验证
            return RegexHelper.Validate(number, pattern);
        }
        #endregion

        #region 验证是否为数字
        /// <summary>
        /// 验证是否为数字
        /// </summary>
        /// <param name="number">要验证的数字</param>       
        public static bool IsNumber(string number)
        {
            //如果为空，认为验证合格
            if (IsNullOrEmpty(number))
            {
                return true;
            }

            //清除要验证字符串中的空格
            number = number.Trim();

            //模式字符串
            string pattern = @"^[1-9]+[0-9]*[.]?[0-9]*$";

            //验证
            return RegexHelper.Validate(number, pattern);
        }
        #endregion

        #region 验证日期是否合法
        /// <summary>
        /// 验证日期是否合法,对不规则的作了简单处理
        /// </summary>
        /// <param name="date">日期</param>
        public static bool IsDate(ref string date)
        {
            //如果为空，认为验证合格
            if (IsNullOrEmpty(date))
            {
                return true;
            }

            //清除要验证字符串中的空格
            date = date.Trim();

            //替换\
            date = date.Replace(@"\", "-");
            //替换/
            date = date.Replace(@"/", "-");

            //如果查找到汉字"今",则认为是当前日期
            if (date.IndexOf("今") != -1)
            {
                date = DateTime.Now.ToString();
            }

            try
            {
                //用转换测试是否为规则的日期字符
                date = Convert.ToDateTime(date).ToString("d");
                return true;
            }
            catch
            {
                //如果日期字符串中存在非数字，则返回false
                if (!IsInt(date))
                {
                    return false;
                }

                #region 对纯数字进行解析
                //对8位纯数字进行解析
                if (date.Length == 8)
                {
                    //获取年月日
                    string year = date.Substring(0, 4);
                    string month = date.Substring(4, 2);
                    string day = date.Substring(6, 2);

                    //验证合法性
                    if (Convert.ToInt32(year) < 1900 || Convert.ToInt32(year) > 2100)
                    {
                        return false;
                    }
                    if (Convert.ToInt32(month) > 12 || Convert.ToInt32(day) > 31)
                    {
                        return false;
                    }

                    //拼接日期
                    date = Convert.ToDateTime(year + "-" + month + "-" + day).ToString("d");
                    return true;
                }

                //对6位纯数字进行解析
                if (date.Length == 6)
                {
                    //获取年月
                    string year = date.Substring(0, 4);
                    string month = date.Substring(4, 2);

                    //验证合法性
                    if (Convert.ToInt32(year) < 1900 || Convert.ToInt32(year) > 2100)
                    {
                        return false;
                    }
                    if (Convert.ToInt32(month) > 12)
                    {
                        return false;
                    }

                    //拼接日期
                    date = Convert.ToDateTime(year + "-" + month).ToString("d");
                    return true;
                }

                //对5位纯数字进行解析
                if (date.Length == 5)
                {
                    //获取年月
                    string year = date.Substring(0, 4);
                    string month = date.Substring(4, 1);

                    //验证合法性
                    if (Convert.ToInt32(year) < 1900 || Convert.ToInt32(year) > 2100)
                    {
                        return false;
                    }

                    //拼接日期
                    date = year + "-" + month;
                    return true;
                }

                //对4位纯数字进行解析
                if (date.Length == 4)
                {
                    //获取年
                    string year = date.Substring(0, 4);

                    //验证合法性
                    if (Convert.ToInt32(year) < 1900 || Convert.ToInt32(year) > 2100)
                    {
                        return false;
                    }

                    //拼接日期
                    date = Convert.ToDateTime(year).ToString("d");
                    return true;
                }
                #endregion

                return false;
            }
        }
        #endregion

        #region 验证身份证是否合法
        /// <summary>
        /// 验证身份证是否合法
        /// </summary>
        /// <param name="idCard">要验证的身份证</param>       
        public static bool IsIdCard(string idCard)
        {
            //如果为空，认为验证合格
            if (IsNullOrEmpty(idCard))
            {
                return true;
            }

            //清除要验证字符串中的空格
            idCard = idCard.Trim();

            //模式字符串
            StringBuilder pattern = new StringBuilder();
            pattern.Append(@"^(11|12|13|14|15|21|22|23|31|32|33|34|35|36|37|41|42|43|44|45|46|");
            pattern.Append(@"50|51|52|53|54|61|62|63|64|65|71|81|82|91)");
            pattern.Append(@"(\d{13}|\d{15}[\dx])$");

            //验证
            return RegexHelper.Validate(idCard, pattern.ToString());
        }
        #endregion

        #region 检测客户的输入中是否有危险字符串
        /// <summary>
        /// 检测客户输入的字符串是否有效,并将原始字符串修改为有效字符串或空字符串。
        /// 当检测到客户的输入中有攻击性危险字符串,则返回false,有效返回true。
        /// </summary>
        /// <param name="input">要检测的字符串</param>
        public static bool IsValidInput(ref string input)
        {
            try
            {
                if (IsNullOrEmpty(input))
                {
                    //如果是空值,则跳出
                    return true;
                }
                else
                {
                    //替换单引号
                    input = input.Replace("'", "''").Trim();

                    //检测攻击性危险字符串
                    string testString = "and |or |exec |insert |select |delete |update |count |chr |mid |master |truncate |char |declare ";
                    string[] testArray = testString.Split('|');
                    foreach (string testStr in testArray)
                    {
                        if (input.ToLower().IndexOf(testStr) != -1)
                        {
                            //检测到攻击字符串,清空传入的值
                            input = "";
                            return false;
                        }
                    }

                    //未检测到攻击字符串
                    return true;
                }
            }
            catch
            {
                //LogHelper.WriteTraceLog( TraceLogLevel.Error, ex.Message );
                return false;
            }
        }
        #endregion




        private static bool IsRegex(string pattern, string input)
        {
            Regex reg = new Regex(pattern, RegexOptions.Compiled);
            return reg.IsMatch(input);
        }

        /// <summary>
        /// 验证是否是数字
        /// </summary>
        /// <param name="input">输了待验证字符串</param>
        /// <returns>如果是数字返回true,否则返回false</returns>
        public static bool IsNumeric(string input)
        {
            return IsRegex(@"[0-9]", input);
        }

        /// <summary>
        /// 验证是否整数（正整数 + 负整数）
        /// </summary>
        /// <param name="input">输了待验证字符串</param>
        /// <returns>如果是返回true,否则返回false</returns>
        public static bool IsInteger(string input)
        {
            return IsRegex(@"^\d+$", input);

        }

        /// <summary>
        /// 验证是否是正整数
        /// </summary>
        /// <param name="input">输了待验证字符串</param>
        /// <returns>如果是返回true,否则返回false</returns>
        public static bool IsPositiveInteger(string input)
        {
            return IsRegex(@"^[1-9]\d*$", input);

        }

        /// <summary>
        /// 验证是否是负整数
        /// </summary>
        /// <param name="input">输了待验证字符串</param>
        /// <returns>如果是返回true,否则返回false</returns>
        public static bool IsNativeInteger(string input)
        {
            return IsRegex(@"^-[0-9]*[1-9][0-9]*$", input);
        }

        /// <summary>
        /// 验证是否非负整数（包括负整数和0）
        /// </summary>
        /// <param name="input">输了待验证字符串</param>
        /// <returns>如果是返回true,否则返回false</returns>
        public static bool IsNoNativeInteger(string input)
        {
            return IsRegex(@"^-?\d+$", input);
        }

        /// <summary>
        /// 验证非正整数（负整数 + 0）
        /// </summary>
        /// <param name="input">输了待验证字符串</param>
        /// <returns>如果是返回true,否则返回false</returns>
        public static bool IsNoPositiveInteger(string input)
        {
            return IsRegex(@"^((-\d+)|(0+))$", input);
        }

        /// <summary>
        /// 验证是否浮点数
        /// </summary>
        /// <param name="input">输了待验证字符串</param>
        /// <returns>如果是返回true,否则返回false</returns>
        public static bool IsFloat(string input)
        {
            return IsRegex(@"^(-?\d+)(\.\d+)?$", input);
        }

        /// <summary>
        /// 判断是否为double类型
        /// </summary>
        /// <param name="input">被验证字符串。</param>
        /// <returns></returns>
        public static bool IsDouble(string input)
        {
            return IsRegex(@"^([0-9])[0-9]*(\.\w*)?$", input);
            //if (input != null)
            //{
            //    return Regex.IsMatch(input.ToString(), @"^([0-9])[0-9]*(\.\w*)?$");
            //}
            //return false;
        }

        /// <summary>
        /// 验证是否为16进制字符串
        /// </summary>
        /// <param name="input">输了待验证字符串</param>
        /// <returns>如果是返回true,否则返回false</returns>
        public static bool IsHexNumber(string input)
        {
            return IsRegex(@"[^0-9a-fA-F]", input);
        }

        ///// <summary>
        ///// 判断是否16进制编码
        ///// </summary>
        ///// <param name="input">被验证字符串。</param>
        ///// <returns></returns>
        //public static bool IsHexNum(string input)
        //{
        //    input = input.Replace("0X", "").Replace("0x", "");
        //    int n = Int32.Parse(input, System.Globalization.NumberStyles.HexNumber);
        //    return IsNumeric(n.ToString());
        //}

        /// <summary>
        /// 验证字符串是否由字母组成
        /// </summary>
        /// <param name="input">输了待验证字符串</param>
        /// <returns>如果是返回true,否则返回false</returns>
        public static bool IsWord(string input)
        {
            return IsRegex(@"^[A-Za-z]+$", input);
        }

        /// <summary>
        /// 验证是否由数字和字母组成的
        /// </summary>
        /// <param name="input">输了待验证字符串</param>
        /// <returns>如果是返回true,否则返回false</returns>
        public static bool IsNumbericAndWord(string input)
        {
            return IsRegex(@"^[A-Za-z0-9]+$", input);
        }

        /// <summary>
        /// 验证是否为IP地址
        /// </summary>
        /// <param name="input">输了待验证字符串</param>
        /// <returns>如果是返回true,否则返回false</returns>
        public static bool IsIPAddress(string input)
        {
            return IsRegex(@"(?<![\d\.])((25[0-5]|2[0-4]\d|1\d{2}|[1-9]\d|\d)\.){3}(25[0-5]|2[0-4]\d|1\d{2}|[1-9]\d|\d)(?![\d\.])", input);
        }

        /// <summary>
        /// 验证是否url地址
        /// </summary>
        /// <param name="input">输了待验证字符串</param>
        /// <returns>如果是返回true,否则返回false</returns>
        public static bool IsUrl(string input)
        {
            return IsRegex(@"\b(\S+)://(\S+)\b", input);
        }

        /// <summary>
        /// 验证是否EMail地址
        /// </summary>
        /// <param name="input">输了待验证字符串</param>
        /// <returns>如果是返回true,否则返回false</returns>
        public static bool IsEMail(string input)
        {
            return IsRegex(@"^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$", input);
        }

        /// <summary>
        /// 验证是否为指定格式化的日期格式如（ 年-月-日 2009-05-22 ）
        /// </summary>
        /// <param name="input">输了待验证字符串</param>
        /// <returns>如果是返回true,否则返回false</returns>
        public static bool IsFormatDate(string input)
        {
            string strTemp = @"^((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-8]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))$";
            //|(/^((0([1-9]{1}))|(1[1|2]))/(([0-2]([1-9]{1}))|(3[0|1]))/(d{2}|d{4})$/)
            return IsRegex(strTemp, input);
        }

        /// <summary>
        /// 验证是否是电话号码（固定电话如：020-38366688）
        /// </summary>
        /// <param name="input">输了待验证字符串</param>
        /// <returns>如果是返回true,否则返回false</returns>
        public static bool IsPhoneNumber(string input)
        {
            // return IsRegex(@"(d+-)?(d{4}-?d{7}|d{3}-?d{8}|^d{7,8})(-d+)?", input);
            return IsRegex(@"(^((\+86)|(86))?-?(\d{3,4}-?)?\d{7,8})$", input);
        }

        /// <summary>
        /// 验证是否中文字符。
        /// </summary>
        /// <param name="input">输了待验证字符串</param>
        /// <returns>如果是返回true,否则返回false</returns>
        public static bool IsMobileNumber(string input)
        {
            return IsRegex(@"^((\+86)|(86))?1(3|5|8)\d{9}$", input);
        }

        /// <summary>
        /// 判断是否为Unicode码
        /// </summary>
        /// <param name="input">被验证字符串。</param>
        /// <returns>返回bool类型</returns>
        public static bool IsUnicode(string input)
        {
            return IsRegex(@"^[\u4E00-\u9FA5\uE815-\uFA29]+$", input);
            //string pattern = @"^[\u4E00-\u9FA5\uE815-\uFA29]+$";
            //return Regex.IsMatch(input, pattern);
        }

        /// <summary>
        /// 验证是否中文字符
        /// </summary>
        /// <param name="input">输了待验证字符串</param>
        /// <returns>如果是返回true,否则返回false</returns>
        public static bool IsChinese(string input)
        {
            return IsRegex(@"^([\u4e00-\u9fa5]+|[a-zA-Z0-9]+)$", input);
        }

        /// <summary>
        ///验证匹配双字节字符(包括汉字在内)
        /// </summary>
        /// <param name="input">输了待验证字符串</param>
        /// <returns>如果是返回true,否则返回false</returns>
        public static bool IsDoubleByte(string input)
        {
            return IsRegex(@"[^\x00-\xff] ", input);
        }

        /// <summary>
        /// 匹配空行的正则表达式
        /// </summary>
        /// <param name="input">输了待验证字符串</param>
        /// <returns>如果是返回true,否则返回false</returns>
        public static bool IsBlankLine(string input)
        {
            return IsRegex(@"\n[\s| ]*\r", input);
        }

        /// <summary>
        /// 验证是否是为HTML标记（还需要改进的地方，如含有空隔或者有属性的时候不行）
        /// </summary>
        /// <param name="input">输了待验证字符串</param>
        /// <returns>如果是返回true,否则返回false</returns>
        public static bool IsHTML(string input)
        {
            return IsRegex(@"<(\S*?)[^>]*>.*?</\1>|<.*? />", input);
        }

        /// <summary>
        /// 判断字符串是否存在操作数据库的安全隐患
        /// </summary>
        /// <param name="input">被验证字符串。</param>
        /// <returns>返回bool类型</returns>
        public static bool IsSafety(string input)
        {
            string str = input.Replace("%20", " ");
            str = Regex.Replace(str, @"\s", " ");
            string pattern = @"select |insert |delete from |count\(|drop table|update |truncate |asc\(|mid\(|char\(|xp_cmdshell|exec master|net localgroup administrators|:|net user|""|\'| or ";
            return !IsRegex(pattern, input);//!Regex.IsMatch(str, pattern, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 验证是否为QQ号码
        /// </summary>
        /// <param name="input">输了待验证字符串</param>
        /// <returns>如果是返回true,否则返回false</returns>
        public static bool IsQQNumber(string input)
        {
            return IsRegex(@"[1-9][0-9]{4,}", input);
        }

        /// <summary>
        /// 验证是否邮政编码
        /// </summary>
        /// <param name="input">输了待验证字符串</param>
        /// <returns>如果是返回true,否则返回false</returns>
        public static bool IsPostalCode(string input)
        {
            return IsRegex(@"[1-9]\d{5}(?!\d)", input);
        }


        #region 判断是否为相对地址（虚拟地址）
        /// <summary>
        /// 判断是否为相对地址（虚拟地址）
        /// </summary>
        /// <param name="input">被验证字符串。</param>
        /// <returns>返回bool类型</returns>
        public static bool IsRelativePath(string input)
        {
            if (input == null || input == string.Empty)
            {
                return false;
            }
            if (input.StartsWith("/") || input.StartsWith("?"))
            {
                return false;
            }
            if (Regex.IsMatch(input, @"^\s*[a-zA-Z]{1,10}:.*$"))
            {
                return false;
            }
            return true;
        }
        #endregion

        #region 判断是否为绝对地址（物理地址）
        /// <summary>
        /// 判断是否为绝对地址（物理地址）
        /// </summary>
        /// <param name="input">被验证字符串。</param>
        /// <returns>返回bool类型</returns>
        public static bool IsPhysicalPath(string input)
        {
            return IsRegex(@"^\s*[a-zA-Z]:.*$", input);
            //string pattern = @"^\s*[a-zA-Z]:.*$";
            //return Regex.IsMatch(input, pattern);
        }
        #endregion

        #region 判断用户IP地址段是否在指定数组当中
        /// <summary>
        /// 返回指定IP是否在指定的IP数组所限定的范围内, IP数组内的IP地址可以使用*表示该IP段任意, 例如192.168.1.*
        /// </summary>
        /// <param name="ip">用户输入的IP</param>
        /// <param name="ipArray">指定的IP地址段数组</param>
        /// <returns>返回bool类型</returns>
        public static bool IsInIPArray(string ip, string[] ipArray)
        {
            string[] userip = ip.Split('.');
            for (int j = 0; j < ipArray.Length; j++)
            {
                string[] tmpip = ipArray[j].Split('.');
                int r = 0;
                for (int i = 0; i < tmpip.Length; i++)
                {
                    if (tmpip[i] == "*")
                    {
                        return true;
                    }
                    if (userip.Length > i)
                    {
                        if (tmpip[i] == userip[i])
                        {
                            r++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                if (r == 4)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

    }
}