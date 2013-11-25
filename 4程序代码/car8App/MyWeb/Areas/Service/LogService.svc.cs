using System;
using MyCmn;
using MyOql;
using MyBiz.Admin;

using System.IO;
using System.Text;
using MyBiz;
namespace MyWeb
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“LogService1”。
    //public class ToLogService : ILogService
    //{
    //    public bool WriteServiceLog(InfoEnum LogType, string Msg, string User)
    //    {
    //        try
    //        {
    //            using (LogClass log = new LogClass())
    //            {
    //                if (log.Insert(LogType, Msg, User))
    //                {
    //                    return true;
    //                }
    //                else return false;
    //            }
    //        }
    //        catch (Exception ee)
    //        {
    //            try
    //            {
    //                using (StreamWriter sw = new StreamWriter("~/mylog.txt", false, Encoding.Default))
    //                {
    //                    sw.WriteLine((ee.Message + "|" + LogType + "|" + Msg + "|" + User + "|" +  DateTime.Now.ToString()).ToString());
    //                    sw.Close();
    //                }

    //                return true;
    //            }
    //            catch { return false; }
    //        }
    //    }
    //    public bool WriteServiceLog(InfoEnum LogType, string Msg)
    //    {
    //        try
    //        {
    //            using (LogClass log = new LogClass())
    //            {
    //                if (log.Insert(LogType, Msg, MySession.Get(MySessionKey.UserID)))
    //                {
    //                    return true;
    //                }
    //                else return false;
    //            }
    //        }
    //        catch (Exception ee)
    //        {
    //            try
    //            {
    //                using (StreamWriter sw = new StreamWriter("~/mylog.txt", false, Encoding.Default))
    //                {
    //                    sw.WriteLine((ee.Message + "|" + LogType + "|" + Msg + "|" + MySession.Get(MySessionKey.UserID) + "|" + DateTime.Now.ToString()).ToString());
    //                    sw.Close();
    //                }

    //                return true;
    //            }
    //            catch { return false; }
    //        }
    //    }
        
    //}
}
