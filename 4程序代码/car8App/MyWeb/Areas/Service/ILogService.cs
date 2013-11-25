using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
//using System.ServiceModel;
using System.Text;
using MyCmn;
using MyOql;
namespace MyWeb
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“ILogService1”。
    //[ServiceContract]
    public interface ILogService
    {     
         //[OperationContract]
        bool WriteServiceLog(InfoEnum LogType, string Msg, string User);
         //[OperationContract]
         //bool ToWriteServiceLog(ClassLog log);
    }
}
