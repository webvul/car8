using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyCmn;

namespace DbEnt
{
    public enum YesNoEnum
    {
        /// <summary>
        /// 是
        /// </summary>
        Yes = 1,

        /// <summary>
        /// 否
        /// </summary>
        No = 2,
    }


    public enum IsAbleEnum
    {
        /// <summary>
        /// 启用
        /// </summary>
        Enable = 1,

        /// <summary>
        /// 禁用
        /// </summary>
        Disable = 2,
    }

    public enum ConfigKey
    {
        InitPassword,

        /// <summary>
        /// 短信应用编码
        /// </summary>
        SmsAppId,

        EtFilePath,
        ReportLogo,

        /// <summary>
        /// 通联服务URL
        /// </summary>
        TL_Url,
        /// <summary>
        /// 通联业务代码
        /// </summary>
        TL_BusinessCode,

        SysAdmin,
        SSOLogin,

        //PmUrl,
        //ClubUrl,
        HyjUrl,
        Domain,

        StandardRoleCode,
    }
}
