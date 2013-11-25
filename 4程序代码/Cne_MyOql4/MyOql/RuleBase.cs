using System;
using System.Linq;
using MyCmn;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Data.Common;
using MyOql.Provider;
using System.Collections.Generic;

namespace MyOql
{
    /// <summary>表,视图,表值函数,存储过程的基类.</summary>
    /// <remarks>
    /// Rule 表示 数据实体规则, 即是数据库表的元数据.
    /// MyOql使用两个类,
    /// 一个是Rule类,表示操作的数据实体, RuleBase 即是操作数据实体的基类.
    /// 另一个是裸类实体,表示的是数据.比如查询结果,更新Model等.
    /// 
    /// MyOql 对象是可序列化的,可做为在 WCF 的传输实体.
    ///     在 Interface 中定义:
    ///     [WCF::OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "http://WSSFTest.Model/2010/ServiceContractDsl1/ServiceContract1/Operation1", ProtectionLevel = ProtectionLevel.None)]
    ///     List&lt;ProductInfoRule.Entity&gt; GetQuery(MyOql.WhereClip where, LangEnum Lang, int SkipNumber, int TakeNumber, OrderByClip order);
    /// </remarks>
    [Serializable]
    public abstract partial class RuleBase : ICloneable, IXmlSerializable, IJoinable, IAsable
    {
        /// <summary>
        /// 取表名.表名，不能重复。
        /// </summary>
        /// <remarks>
        /// 使用方法而不是属性,在为了避免与实体的列属性名冲突.
        /// </remarks>
        /// <returns></returns>
        public abstract string GetDbName();


        /// <summary>
        /// 取应用程序转义后的名称.转义规则见 <see cref="dbo.TranslateDbName"/>
        /// </summary>
        /// <returns></returns>
        public string GetName() { return Name; }

        /// <summary>
        /// 取表名.
        /// </summary>
        /// <returns></returns>
        public EntityName GetFullName()
        {
            var _config = this.GetConfig();

            if (_config == null)
            {
                return new EntityName() { DbName = GetDbName(), Name = GetName() };
            }
            else return new EntityName()
            {
                Owner = _config.Owner,
                DbName = GetDbName(),
                Name = GetName()
            };
        }

        /// <summary>
        /// 取列集合.
        /// </summary>
        /// <returns></returns>
        public abstract SimpleColumn[] GetColumns();

        /// <summary>
        /// 取主键集合. MyOql支持多主键.
        /// </summary>
        /// <returns></returns>
        public abstract SimpleColumn[] GetPrimaryKeys();

        /// <summary>
        /// 取计算列集合.
        /// </summary>
        /// <returns></returns>
        public abstract SimpleColumn[] GetComputeKeys();

        /// <summary>
        /// 取自增列.自增列只能有一个.
        /// </summary>
        /// <returns></returns>
        public abstract SimpleColumn GetAutoIncreKey();


        //public abstract ColumnClip GetRowTimestamp();

        /// <summary>
        /// 取唯一约束列. 返回顺序是: 单一主键,自增列,唯一维束列. 
        /// </summary>
        /// <returns></returns>
        public SimpleColumn GetIdKey()
        {
            var retVal = GetPrimaryKeys();
            if (retVal.Length == 1) return retVal[0];

            var retOne = GetAutoIncreKey();
            if (retOne.EqualsNull() == false) return retOne;

            retOne = GetUniqueKey();
            if (retOne.EqualsNull() == false) return retOne;

            return null;
        }

        /// <summary>
        /// 返回 数字唯一列, 用于行集权限的标识.
        /// </summary>
        /// <returns></returns>
        public ColumnClip GetUniqueNumberKey()
        {
            var retVal = GetPrimaryKeys();
            if (retVal.Length == 1 && retVal[0].DbType.DbTypeIsNumber()) return retVal[0];

            var retOne = GetAutoIncreKey();
            if (retOne.EqualsNull() == false) return retOne;

            retOne = GetUniqueKey();
            if (retOne.EqualsNull() == false && retOne.DbType.DbTypeIsNumber()) return retOne;

            return null;
        }

        /// <summary>
        /// 取唯一约束列. 如果表中存在多个列联合做唯一约束, 则返回 null .
        /// 主要是为了数据库表中不使用 自增,而使用 GUID 做唯一, 其它多列做联合主键的情况.
        /// </summary>
        /// <returns></returns>
        public abstract SimpleColumn GetUniqueKey();

        //[DataMember]
        //protected string _Alias_ { get; set; }

        protected string Name;


        /// <summary>
        /// 生成 Count(1) 子句.
        /// </summary>
        /// <returns></returns>
        public ColumnClip Count()
        {
            return new ConstColumn(1).Count();
        }

        //public ColumnClip CurrentIdentity()
        //{
        //    ColumnClip colu = new ConstColumn(1);
        //    colu.Table = this;
        //    colu.Extend.Add(SqlOperator.Count, "");
        //    colu.Extend.Add(SqlOperator.As, "CurrentIdentity");
        //    return colu;
        //}


        public ColumnClip GetColumn(ColumnName FullDbName)
        {
            var col = this.GetColumns().FirstOrDefault(o => o.TableDbName == FullDbName.Entity && o.DbName == FullDbName.Column);
            if (dbo.EqualsNull(col) == false)
            {
                return col.Clone() as ColumnClip;
            }
            return null;
        }


        /// <summary>
        /// 根据列名查询列.(采用 ColumnClip.NameEquals ，属于模糊匹配)
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public SimpleColumn GetColumn(string Name)
        {
            var col = this.GetColumns().FirstOrDefault(o => o.NameEquals(Name, true));
            if (dbo.EqualsNull(col) == false)
            {
                return col.Clone() as SimpleColumn;
            }
            return null;
        }


        /// <summary>
        /// 从配置文件中取出当前连接 .
        /// </summary>
        /// <example>
        ///可以使用单库事务，也可以使用分布式事务。 以下是两处事务的不同实现。
        ///显式使用单库事务：
        /// <code>
        ///using (var conn = dbr.Dict.GetDbConnection())
        ///   {
        ///       dbo.Open(conn, () =&gt;
        ///           {
        ///               using (var tran = conn.BeginTransaction(IsolationLevel.ReadCommitted))
        ///               {
        ///
        ///                   dbr.Dept.Insert(o = &gt; o.Name == "udi" &amp; o.Pid == 0 &amp; o.RootPath == "0"  &amp; o.BusType == DeptBusEnum.Trade)
        ///                       .UseTransaction(tran);
        ///                       .Execute();
        ///
        ///
        ///                   dbr.Person.Insert(o =&gt; o.UserID == "udi"  &amp; o.DeptID == d.LastAutoID)
        ///                       .UseTransaction(tran)
        ///                       .Execute();
        ///
        ///                   try
        ///                   {
        ///                       tran.Commit();
        ///                   }
        ///                   catch
        ///                   {
        ///                       tran.Rollback();
        ///                       throw;
        ///                   }
        ///               }
        ///               return 1;
        ///           });
        ///   }    
        /// </code>
        /// 分布式事务：
        /// <code>
        ///    using (TransactionScope scope = new TransactionScope())
        ///    {
        ///        var deptModel = new DeptRule.Entity(){ Name = "管理部门", ID = 0 };
        ///        var userModel = new UserRule.Entity(){ Name = "张三" , DeptID = 0 , ID = 0 } ;
        ///        
        ///        dbr.Dept.Insert(deptModel ).Execute();
        ///        userModel.DeptID = deptModel.ID ;
        ///        dbr.User.Insert(userModel).Execute() ;
        ///        
        ///         Assert.IsTrue(userModel.ID &lt; 0) ;
        ///        scope.Complete();
        ///    }
        /// </code>
        /// </example>
        /// <returns></returns>
        public DbConnection GetDbConnection()
        {
            return dbo.GetDbConnection(this.GetFullName());
        }

        public TranslateSql GetDbProvider()
        {
            return dbo.GetDbProvider(this.GetConfig().db);
        }

        public DatabaseType GetDataBase()
        {
            return dbo.GetDatabaseType(this.GetConfig().db);
        }

        protected ReConfigEnum ReConfig;

        public ReConfigEnum GetRecofing() { return ReConfig; }
        public void SetReconfig(ReConfigEnum value) { ReConfig = value; }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public abstract object Clone();


        ///// <summary>
        ///// 取外键关系。表示该表的外键与引用表关系。
        ///// </summary>
        ///// <returns></returns>
        //public abstract List<FkColumn> GetFks();

        ///// <summary>
        ///// 取子表关系，表示它表的外键与该表引用键关系。
        ///// </summary>
        ///// <returns></returns>
        //public abstract List<FkColumn> GetSubEntities(); 



        public string GetAlias()
        {
            return this.Name;
        }

        public void SetAlias(string Alias)
        {
            this.Name = Alias;
        }
    }
}
