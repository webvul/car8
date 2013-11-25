
using MyCmn;
using System.Collections.Generic;
using System;
using System.Data.Common;
namespace MyOql
{
    /// <summary>
    /// database operation 数据库操作类。
    /// </summary> 
    /// <remarks>
    /// <pre style='line-height:30px;font-size:14px;font-family: 微软雅黑,宋体'>
    /// 在生成实体代码之前,要先配置好配置文件,配置文件见<see cref="MyOql.MyOql_CodeGen.MyOqlCodeGen"/>
    /// 在生成实体代码之后,要对各个实体进行配置,配置文件见<see cref="MyOql.MyOqlConfigSect"/>
    /// 配置完成之后,要对MyOql组件进行十三个事件的注册:
    /// 1.解密             见 <see cref="MyOql.IDboEvent.DecrypteEvent"/>
    /// 2.插入前           见 <see cref="MyOql.IDboEvent.Creating"/>
    /// 3.插入后           见 <see cref="MyOql.IDboEvent.OnCreated"/>
    /// 4.读行前           见 <see cref="MyOql.IDboEvent.Reading"/>
    /// 5.读行后           见 <see cref="MyOql.IDboEvent.OnReaded"/>
    /// 6.更新行前         见 <see cref="MyOql.IDboEvent.Updating"/>
    /// 7.更新行后         见 <see cref="MyOql.IDboEvent.OnUpdated"/>
    /// 8.删除行前         见 <see cref="MyOql.IDboEvent.Deleting"/>
    /// 9.删除行后         见 <see cref="MyOql.IDboEvent.OnDeleted"/>
    /// 10.生成Sql后       见 <see cref="MyOql.IDboEvent.GenerateSqled"/>
    /// 11.执行存储过程前  见 <see cref="MyOql.IDboEvent.Procing"/>
    /// 12.执行存储过程后  见 <see cref="MyOql.IDboEvent.OnProced"/>
    /// 13.批量插入前      见 <see cref="MyOql.IDboEvent.BulkInserting"/>
    /// 
    /// 部分操作行前事件和操作列事件, 涉及权限.
    /// </pre>
    /// </remarks>
    public static partial class dbo { }

    /// <remarks>
    /// 每个动作都有前置后置事件, 权限和日志都在事件中处理.
    /// 十三个事件.
    /// </remarks>
    /// <example>
    /// 通用权限设计图:
    /// <img src="../PowerViso.jpg" />
    /// <pre>
    /// 数据权限里的查看权限表现在 卡片和列表。如果卡片没有查看权限那么它肯定不能修改。
    /// 数据权限里的修改权限表现在卡片的修改和添加（以及保存操作）。
    /// 这些数据在MyOql查询时，通过事件注册方式实现权限的应用。
    /// 页面权限是用户是否能访问该页面的权限，如果不能访问该页面，那么肯定没有该页面的操作权限。
    /// </pre>
    /// 
    /// 数据库表结构设计:
    /// <img src="../PowerTables.jpg" />
    /// <pre>
    /// PowerController：项目Area和Controller列表。Area是在程序中定义的枚举。
    /// PowerAction：记录项目的 Action。指向页面。
    /// PowerButton:列表页面操作有：List,查询，添加，修改，删除，查看。卡片操作有：Edit,保存，删除，关闭。 其中List是列表的代表权限，Edit是卡片的代表权限。代表权限是指，如果没有代表权限的话，就没有该页面的权限。
    /// PowerTable：与Mvc没关系。记录MyOql实体的表。
    /// PowerColumn：记录MyOql实体的表字段。
    /// 
    /// 基于现行架构的实现方案:
    /// 用户权限表示形式细分为对数据库的查看，修改，删除，更新，以及对页面的访问及操作六部分，各个权限表示用 2 的次幂表示，数据很大，要用 MyBigInt表示，可进行 &amp; 和 | 运算。用户权限可用Json打包在各层传递，数据结构示例：
    /// {create:"123456",delete:"12345",read:"123456",update:"12345",action:"12345",button:"123456",row:{"menu":"123456","other table":"12345"} }
    /// 分为四部分：1.数据处理部分，2.页面访问，3.页面按钮 4.页面列表及字段
    /// 1.MyOql的数据CRUD权限处理在配置文件中有对各个实体的权限开关 （UsePower），在 MyOql 内部以 dbo.UserPowerEvent 方式扩展来实现；另外 仅针对返回MyOql实体的存储过程做查看的权限的处理。
    /// 2.action是指是否有对该页面的访问权，在Controller基类统一处理。
    /// 3.4.但是在客户端，因为只用和本页面相关的数据，包括三部分：read,edit,button，(Viso图的红色部分)，客户端只能识别元素类名，元素内容等信息，不能识别ByBigInt，所以在Render到客户端时，其数据结构变为：
    /// {button:[“(内容)",".Class","#ID",":状态","[attr=value]"],view:[“列名","列名"],edit[“列名","列名"]}
    /// 客户端解析用（）包起来的内容为元素的 text，其它内容直接用 Jquery解析。该结构指示哪些元素可见（或可用）。
    /// view , edit 数据存储各列，格式为： 表名.列名，所以，客户端的列表各列要标识出列，卡片的各个字段也要标识出列。
    /// 数据放在： jv.MyPower 中。
    /// 如果 jv.MyPower == null 则权限全开；否则只开数组内元素权限。无任何权限可设置 jv.MyPower = [] ; 
    /// 
    /// 程序代码实现:请参考实例。
    /// </pre>
    /// </example>
    public partial class dbo
    {
        public class NullEvent : IDboEvent
        {
            public override IDbr Idbr { get; set; }

            public override string[] GetChangedTable()
            {
                throw new System.NotImplementedException();
            }

            public override void ClearedCacheTable(string Table)
            {
                throw new System.NotImplementedException();
            }
        }
        static dbo()
        {
            dbo.Event = new NullEvent();
            dbo.MyFunction = new Dictionary<SqlOperator, Func<DatabaseType, string>>();
            dbo.Polymer = new List<SqlOperator>();
        }

        /// <summary>
        /// 检查条件 , 抛出错误.
        /// </summary>
        /// <param name="FalseResult">如果条件为真,则抛出错误.</param>
        /// <param name="Msg">错误消息.</param>
        /// <param name="rule"></param>
        public static void Check(bool FalseResult, string Msg, RuleBase rule)
        {
            GodError.Check(FalseResult, "MyOqlGodError", Msg, rule == null ? "" : rule.GetFullName().ToString());
        }
 

        public static void ThrowError(this  Exception exp, MyCommand myCmd)
        {
            throw new GodError(exp) { Type = "MyOqlGodError", Detail = myCmd.FullSql };
        }

        public static void ThrowError(this Exception exp, DbCommand myCmd)
        {
            throw new GodError(exp) { Type = "MyOqlGodError", Detail = myCmd.GetFullSql() };
        }
    }
}
