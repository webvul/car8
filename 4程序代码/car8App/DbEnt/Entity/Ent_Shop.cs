//系统自动生成的实体，不能修改。 By: UDI-PC.于新海  At:2013-12-15 11:12:12
using System;
using MyOql;
using MyCmn;
using System.Linq;
using System.Data ;
using System.Data.Common ;
using System.Collections.Generic;
using System.Runtime.Serialization ;
using DbEnt;

namespace DbEnt
{


    
    /// <summary>
    /// 产品类型
    /// </summary>
    [Serializable]
    public sealed partial class ProductTypeRule : RuleBase, ITableRule,ICloneable
    {


        public  ProductTypeRule() : base("ProductType")
        {
            this.Id = new SimpleColumn(this, DbType.Int32, 4,"Id","ID",false);
            this.Name = new SimpleColumn(this, DbType.AnsiString, 50,"Name","Name",false);
            this.Pid = new SimpleColumn(this, DbType.Int32, 4,"Pid","PID",false);
            this.Wbs = new SimpleColumn(this, DbType.AnsiString, 50,"Wbs","Wbs",true);
            this.CategoryID = new SimpleColumn(this, DbType.Int32, 4,"CategoryID","CategoryID",true);
            this.Descr = new SimpleColumn(this, DbType.AnsiString, 100,"Descr","Descr",true);
            this.SortID = new SimpleColumn(this, DbType.Int32, 4,"SortID","SortID",true);
            this.DeptID = new SimpleColumn(this, DbType.Int32, 4,"DeptID","DeptID",true);
            this.UserID = new SimpleColumn(this, DbType.AnsiString, 50,"UserID","UserID",true);
            this.AddTime = new SimpleColumn(this, DbType.DateTime, 8,"AddTime","AddTime",true);
        }

        /// <summary>
        /// ID(Int32)[主键(ID),自增键]
        /// </summary>
        public SimpleColumn Id { get; set; }
        /// <summary>
        /// 名称(AnsiString)
        /// </summary>
        public SimpleColumn Name { get; set; }
        /// <summary>
        /// 父ID(Int32)
        /// </summary>
        public SimpleColumn Pid { get; set; }
        /// <summary>
        /// 从根到父节点的路径(AnsiString)
        /// </summary>
        public SimpleColumn Wbs { get; set; }
        /// <summary>
        /// 产品分类ID(Int32)
        /// </summary>
        public SimpleColumn CategoryID { get; set; }
        /// <summary>
        /// 描述(AnsiString)
        /// </summary>
        public SimpleColumn Descr { get; set; }
        /// <summary>
        /// 排序数(Int32)
        /// </summary>
        public SimpleColumn SortID { get; set; }
        /// <summary>
        /// 部门ID(Int32)[外键(DeptID=EC_Dept:ID)]
        /// </summary>
        public SimpleColumn DeptID { get; set; }
        /// <summary>
        /// 用户ID(AnsiString)
        /// </summary>
        public SimpleColumn UserID { get; set; }
        /// <summary>
        /// 添加时间(DateTime)
        /// </summary>
        public SimpleColumn AddTime { get; set; }

        public override SimpleColumn[] GetColumns() {  return new SimpleColumn[] { Id,Name,Pid,Wbs,CategoryID,Descr,SortID,DeptID,UserID,AddTime }; }
        public override SimpleColumn[] GetPrimaryKeys() { return new SimpleColumn[] { Id };  }
        public override SimpleColumn[] GetComputeKeys() { return new SimpleColumn[] {  }; }
        public override SimpleColumn GetAutoIncreKey() {  return Id; }
        public override SimpleColumn GetUniqueKey() { return  null; }
        public override string GetDbName() { return "EC_ProductType"; }

        public Entity FindById(Int32 Id)
        {
             if ( Id <= 0 ) return null ;
            return this.SelectWhere(o => o.Id == Id).ToEntity<Entity>();
        }
        public int DeleteById(Int32 Id)
        {
             if ( Id <= 0 ) return 0 ;
            return this.Delete(o => o.Id == Id).Execute() ;
        }


        public override object Clone()
        {
            var tab = new ProductTypeRule();
            if ( this._Config_ != null ) tab._Config_ = base._Config_.Clone() as RuleRuntimeConfig ;
            tab.SetAlias(base.Name);
            tab.SetReconfig(base.ReConfig);

            tab.Id = this.Id.Clone() as SimpleColumn;
            tab.Name = this.Name.Clone() as SimpleColumn;
            tab.Pid = this.Pid.Clone() as SimpleColumn;
            tab.Wbs = this.Wbs.Clone() as SimpleColumn;
            tab.CategoryID = this.CategoryID.Clone() as SimpleColumn;
            tab.Descr = this.Descr.Clone() as SimpleColumn;
            tab.SortID = this.SortID.Clone() as SimpleColumn;
            tab.DeptID = this.DeptID.Clone() as SimpleColumn;
            tab.UserID = this.UserID.Clone() as SimpleColumn;
            tab.AddTime = this.AddTime.Clone() as SimpleColumn;

            return tab;
        }
        /// <summary>
        /// 产品类型 
        /// </summary>
        public Entity _ { get { return new Entity (); } }
       
        /// <summary>
        /// 产品类型
        /// </summary>
        [Serializable]
        public sealed partial class Entity :IEntity
        {
 
            /// <summary>
            /// ID[主键(ID),自增键]
            /// </summary>
            public Int32 Id { get; set; }
 
            /// <summary>
            /// 名称
            /// </summary>
            public String Name { get; set; }
 
            /// <summary>
            /// 父ID
            /// </summary>
            public Int32 Pid { get; set; }
 
            /// <summary>
            /// 从根到父节点的路径
            /// </summary>
            public String Wbs { get; set; }
 
            /// <summary>
            /// 产品分类ID
            /// </summary>
            public Int32 CategoryID { get; set; }
 
            /// <summary>
            /// 描述
            /// </summary>
            public String Descr { get; set; }
 
            /// <summary>
            /// 排序数
            /// </summary>
            public Int32 SortID { get; set; }
 
            /// <summary>
            /// 部门ID[外键(DeptID=EC_Dept:ID)]
            /// </summary>
            public Int32 DeptID { get; set; }
 
            /// <summary>
            /// 用户ID
            /// </summary>
            public String UserID { get; set; }
 
            /// <summary>
            /// 添加时间
            /// </summary>
            public MyDate AddTime { get; set; }

            public bool SetPropertyValue(string PropertyName, object Value)
            {
                if ( PropertyName == "Id" ) { this.Id = ValueProc.As<Int32>(Value) ; return true; }
                if ( PropertyName == "Name" ) { this.Name = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "Pid" ) { this.Pid = ValueProc.As<Int32>(Value) ; return true; }
                if ( PropertyName == "Wbs" ) { this.Wbs = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "CategoryID" ) { this.CategoryID = ValueProc.As<Int32>(Value) ; return true; }
                if ( PropertyName == "Descr" ) { this.Descr = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "SortID" ) { this.SortID = ValueProc.As<Int32>(Value) ; return true; }
                if ( PropertyName == "DeptID" ) { this.DeptID = ValueProc.As<Int32>(Value) ; return true; }
                if ( PropertyName == "UserID" ) { this.UserID = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "AddTime" ) { this.AddTime = ValueProc.As<MyDate>(Value) ; return true; }
                return false ;
           }

            public object GetPropertyValue(string PropertyName)
            {
                if ( PropertyName == "Id" ) { return this.Id ; }
                if ( PropertyName == "Name" ) { return this.Name ; }
                if ( PropertyName == "Pid" ) { return this.Pid ; }
                if ( PropertyName == "Wbs" ) { return this.Wbs ; }
                if ( PropertyName == "CategoryID" ) { return this.CategoryID ; }
                if ( PropertyName == "Descr" ) { return this.Descr ; }
                if ( PropertyName == "SortID" ) { return this.SortID ; }
                if ( PropertyName == "DeptID" ) { return this.DeptID ; }
                if ( PropertyName == "UserID" ) { return this.UserID ; }
                if ( PropertyName == "AddTime" ) { return this.AddTime ; }
                return null ;
            }

            public string[]  GetProperties() { return new string[]{ "Id","Name","Pid","Wbs","CategoryID","Descr","SortID","DeptID","UserID","AddTime" } ; }
            
            public object Clone()
            {
                return this.CloneIEntity() ;
            }

            private DeptRule.Entity _GetDept = null;
            public DeptRule.Entity GetDept()
            {
                if (_GetDept != null) return _GetDept;
                _GetDept = dbr.Dept.FindById(this.DeptID);
                return _GetDept;
            }
            private ProductInfoRule.Entity[] _GetProductInfos = null;
            public ProductInfoRule.Entity[] GetProductInfos()
            {
                if (_GetProductInfos != null) return _GetProductInfos;
                _GetProductInfos = dbr.Shop.ProductInfo.SelectWhere(o => o.ProductTypeID == this.Id).ToEntityList<ProductInfoRule.Entity>().ToArray();
                return _GetProductInfos;
            }
         }
    }


    
    /// <summary>
    /// 通告类型
    /// </summary>
    [Serializable]
    public sealed partial class NoticeTypeRule : RuleBase, ITableRule,ICloneable
    {


        public  NoticeTypeRule() : base("NoticeType")
        {
            this.Id = new SimpleColumn(this, DbType.Int64, 8,"Id","ID",false);
            this.Name = new SimpleColumn(this, DbType.AnsiString, 50,"Name","Name",false);
            this.Pid = new SimpleColumn(this, DbType.Int64, 8,"Pid","PID",false);
            this.Wbs = new SimpleColumn(this, DbType.AnsiString, 50,"Wbs","Wbs",true);
            this.CategoryID = new SimpleColumn(this, DbType.Int32, 4,"CategoryID","CategoryID",true);
            this.Descr = new SimpleColumn(this, DbType.AnsiString, 100,"Descr","Descr",true);
            this.SortID = new SimpleColumn(this, DbType.Int32, 4,"SortID","SortID",true);
            this.DeptID = new SimpleColumn(this, DbType.Int32, 4,"DeptID","DeptID",true);
            this.UserID = new SimpleColumn(this, DbType.AnsiString, 50,"UserID","UserID",true);
            this.AddTime = new SimpleColumn(this, DbType.DateTime, 8,"AddTime","AddTime",true);
        }

        /// <summary>
        /// ID(Int64)[主键(ID)]
        /// </summary>
        public SimpleColumn Id { get; set; }
        /// <summary>
        /// 名称(AnsiString)
        /// </summary>
        public SimpleColumn Name { get; set; }
        /// <summary>
        /// 父ID(Int64)
        /// </summary>
        public SimpleColumn Pid { get; set; }
        /// <summary>
        /// 从根到父节点的路径(AnsiString)
        /// </summary>
        public SimpleColumn Wbs { get; set; }
        /// <summary>
        /// 分类ID(Int32)
        /// </summary>
        public SimpleColumn CategoryID { get; set; }
        /// <summary>
        /// 描述(AnsiString)
        /// </summary>
        public SimpleColumn Descr { get; set; }
        /// <summary>
        /// 排序数(Int32)
        /// </summary>
        public SimpleColumn SortID { get; set; }
        /// <summary>
        /// 商户ID(Int32)
        /// </summary>
        public SimpleColumn DeptID { get; set; }
        /// <summary>
        /// 用户ID(AnsiString)
        /// </summary>
        public SimpleColumn UserID { get; set; }
        /// <summary>
        /// 添加时间(DateTime)
        /// </summary>
        public SimpleColumn AddTime { get; set; }

        public override SimpleColumn[] GetColumns() {  return new SimpleColumn[] { Id,Name,Pid,Wbs,CategoryID,Descr,SortID,DeptID,UserID,AddTime }; }
        public override SimpleColumn[] GetPrimaryKeys() { return new SimpleColumn[] { Id };  }
        public override SimpleColumn[] GetComputeKeys() { return new SimpleColumn[] {  }; }
        public override SimpleColumn GetAutoIncreKey() {  return null; }
        public override SimpleColumn GetUniqueKey() { return  null; }
        public override string GetDbName() { return "E_NoticeType"; }

        public Entity FindById(Int64 Id)
        {
             if ( Id <= 0 ) return null ;
            return this.SelectWhere(o => o.Id == Id).ToEntity<Entity>();
        }
        public int DeleteById(Int64 Id)
        {
             if ( Id <= 0 ) return 0 ;
            return this.Delete(o => o.Id == Id).Execute() ;
        }


        public override object Clone()
        {
            var tab = new NoticeTypeRule();
            if ( this._Config_ != null ) tab._Config_ = base._Config_.Clone() as RuleRuntimeConfig ;
            tab.SetAlias(base.Name);
            tab.SetReconfig(base.ReConfig);

            tab.Id = this.Id.Clone() as SimpleColumn;
            tab.Name = this.Name.Clone() as SimpleColumn;
            tab.Pid = this.Pid.Clone() as SimpleColumn;
            tab.Wbs = this.Wbs.Clone() as SimpleColumn;
            tab.CategoryID = this.CategoryID.Clone() as SimpleColumn;
            tab.Descr = this.Descr.Clone() as SimpleColumn;
            tab.SortID = this.SortID.Clone() as SimpleColumn;
            tab.DeptID = this.DeptID.Clone() as SimpleColumn;
            tab.UserID = this.UserID.Clone() as SimpleColumn;
            tab.AddTime = this.AddTime.Clone() as SimpleColumn;

            return tab;
        }
        /// <summary>
        /// 通告类型 
        /// </summary>
        public Entity _ { get { return new Entity (); } }
       
        /// <summary>
        /// 通告类型
        /// </summary>
        [Serializable]
        public sealed partial class Entity :IEntity
        {
 
            /// <summary>
            /// ID[主键(ID)]
            /// </summary>
            public Int64 Id { get; set; }
 
            /// <summary>
            /// 名称
            /// </summary>
            public String Name { get; set; }
 
            /// <summary>
            /// 父ID
            /// </summary>
            public Int64 Pid { get; set; }
 
            /// <summary>
            /// 从根到父节点的路径
            /// </summary>
            public String Wbs { get; set; }
 
            /// <summary>
            /// 分类ID
            /// </summary>
            public Int32 CategoryID { get; set; }
 
            /// <summary>
            /// 描述
            /// </summary>
            public String Descr { get; set; }
 
            /// <summary>
            /// 排序数
            /// </summary>
            public Int32 SortID { get; set; }
 
            /// <summary>
            /// 商户ID
            /// </summary>
            public Int32 DeptID { get; set; }
 
            /// <summary>
            /// 用户ID
            /// </summary>
            public String UserID { get; set; }
 
            /// <summary>
            /// 添加时间
            /// </summary>
            public MyDate AddTime { get; set; }

            public bool SetPropertyValue(string PropertyName, object Value)
            {
                if ( PropertyName == "Id" ) { this.Id = ValueProc.As<Int64>(Value) ; return true; }
                if ( PropertyName == "Name" ) { this.Name = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "Pid" ) { this.Pid = ValueProc.As<Int64>(Value) ; return true; }
                if ( PropertyName == "Wbs" ) { this.Wbs = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "CategoryID" ) { this.CategoryID = ValueProc.As<Int32>(Value) ; return true; }
                if ( PropertyName == "Descr" ) { this.Descr = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "SortID" ) { this.SortID = ValueProc.As<Int32>(Value) ; return true; }
                if ( PropertyName == "DeptID" ) { this.DeptID = ValueProc.As<Int32>(Value) ; return true; }
                if ( PropertyName == "UserID" ) { this.UserID = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "AddTime" ) { this.AddTime = ValueProc.As<MyDate>(Value) ; return true; }
                return false ;
           }

            public object GetPropertyValue(string PropertyName)
            {
                if ( PropertyName == "Id" ) { return this.Id ; }
                if ( PropertyName == "Name" ) { return this.Name ; }
                if ( PropertyName == "Pid" ) { return this.Pid ; }
                if ( PropertyName == "Wbs" ) { return this.Wbs ; }
                if ( PropertyName == "CategoryID" ) { return this.CategoryID ; }
                if ( PropertyName == "Descr" ) { return this.Descr ; }
                if ( PropertyName == "SortID" ) { return this.SortID ; }
                if ( PropertyName == "DeptID" ) { return this.DeptID ; }
                if ( PropertyName == "UserID" ) { return this.UserID ; }
                if ( PropertyName == "AddTime" ) { return this.AddTime ; }
                return null ;
            }

            public string[]  GetProperties() { return new string[]{ "Id","Name","Pid","Wbs","CategoryID","Descr","SortID","DeptID","UserID","AddTime" } ; }
            
            public object Clone()
            {
                return this.CloneIEntity() ;
            }

            private NoticeInfoRule.Entity[] _GetNoticeInfos = null;
            public NoticeInfoRule.Entity[] GetNoticeInfos()
            {
                if (_GetNoticeInfos != null) return _GetNoticeInfos;
                _GetNoticeInfos = dbr.Shop.NoticeInfo.SelectWhere(o => o.NoticeTypeID == this.Id).ToEntityList<NoticeInfoRule.Entity>().ToArray();
                return _GetNoticeInfos;
            }
         }
    }


    
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public sealed partial class NoticeInfoRule : RuleBase, ITableRule,ICloneable
    {


        public  NoticeInfoRule() : base("NoticeInfo")
        {
            this.Id = new SimpleColumn(this, DbType.Int32, 4,"Id","ID",false);
            this.NoticeTypeID = new SimpleColumn(this, DbType.Int64, 8,"NoticeTypeID","NoticeTypeID",true);
            this.Name = new SimpleColumn(this, DbType.AnsiString, 100,"Name","Name",true);
            this.Wbs = new SimpleColumn(this, DbType.AnsiString, 50,"Wbs","Wbs",true);
            this.Logo = new SimpleColumn(this, DbType.Int32, 4,"Logo","Logo",true);
            this.Descr = new SimpleColumn(this, DbType.AnsiString, 500,"Descr","Descr",true);
            this.SortID = new SimpleColumn(this, DbType.Int32, 4,"SortID","SortID",true);
            this.UpdateTime = new SimpleColumn(this, DbType.DateTime, 8,"UpdateTime","UpdateTime",true);
        }

        /// <summary>
        /// ID(Int32)[主键(ID),自增键]
        /// </summary>
        public SimpleColumn Id { get; set; }
        /// <summary>
        /// NoticeTypeID(Int64)[外键(NoticeTypeID=E_NoticeType:ID)]
        /// </summary>
        public SimpleColumn NoticeTypeID { get; set; }
        /// <summary>
        /// Name(AnsiString)
        /// </summary>
        public SimpleColumn Name { get; set; }
        /// <summary>
        /// Wbs(AnsiString)
        /// </summary>
        public SimpleColumn Wbs { get; set; }
        /// <summary>
        /// Logo(Int32)[外键(Logo=S_Annex:ID)]
        /// </summary>
        public SimpleColumn Logo { get; set; }
        /// <summary>
        /// Descr(AnsiString)
        /// </summary>
        public SimpleColumn Descr { get; set; }
        /// <summary>
        /// SortID(Int32)
        /// </summary>
        public SimpleColumn SortID { get; set; }
        /// <summary>
        /// UpdateTime(DateTime)
        /// </summary>
        public SimpleColumn UpdateTime { get; set; }

        public override SimpleColumn[] GetColumns() {  return new SimpleColumn[] { Id,NoticeTypeID,Name,Wbs,Logo,Descr,SortID,UpdateTime }; }
        public override SimpleColumn[] GetPrimaryKeys() { return new SimpleColumn[] { Id };  }
        public override SimpleColumn[] GetComputeKeys() { return new SimpleColumn[] {  }; }
        public override SimpleColumn GetAutoIncreKey() {  return Id; }
        public override SimpleColumn GetUniqueKey() { return  null; }
        public override string GetDbName() { return "E_NoticeInfo"; }

        public Entity FindById(Int32 Id)
        {
             if ( Id <= 0 ) return null ;
            return this.SelectWhere(o => o.Id == Id).ToEntity<Entity>();
        }
        public int DeleteById(Int32 Id)
        {
             if ( Id <= 0 ) return 0 ;
            return this.Delete(o => o.Id == Id).Execute() ;
        }


        public override object Clone()
        {
            var tab = new NoticeInfoRule();
            if ( this._Config_ != null ) tab._Config_ = base._Config_.Clone() as RuleRuntimeConfig ;
            tab.SetAlias(base.Name);
            tab.SetReconfig(base.ReConfig);

            tab.Id = this.Id.Clone() as SimpleColumn;
            tab.NoticeTypeID = this.NoticeTypeID.Clone() as SimpleColumn;
            tab.Name = this.Name.Clone() as SimpleColumn;
            tab.Wbs = this.Wbs.Clone() as SimpleColumn;
            tab.Logo = this.Logo.Clone() as SimpleColumn;
            tab.Descr = this.Descr.Clone() as SimpleColumn;
            tab.SortID = this.SortID.Clone() as SimpleColumn;
            tab.UpdateTime = this.UpdateTime.Clone() as SimpleColumn;

            return tab;
        }
        /// <summary>
        ///  
        /// </summary>
        public Entity _ { get { return new Entity (); } }
       
        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public sealed partial class Entity :IEntity
        {
 
            /// <summary>
            /// ID[主键(ID),自增键]
            /// </summary>
            public Int32 Id { get; set; }
 
            /// <summary>
            /// NoticeTypeID[外键(NoticeTypeID=E_NoticeType:ID)]
            /// </summary>
            public Int64 NoticeTypeID { get; set; }
 
            /// <summary>
            /// Name
            /// </summary>
            public String Name { get; set; }
 
            /// <summary>
            /// Wbs
            /// </summary>
            public String Wbs { get; set; }
 
            /// <summary>
            /// Logo[外键(Logo=S_Annex:ID)]
            /// </summary>
            public Int32 Logo { get; set; }
 
            /// <summary>
            /// Descr
            /// </summary>
            public String Descr { get; set; }
 
            /// <summary>
            /// SortID
            /// </summary>
            public Int32 SortID { get; set; }
 
            /// <summary>
            /// UpdateTime
            /// </summary>
            public MyDate UpdateTime { get; set; }

            public bool SetPropertyValue(string PropertyName, object Value)
            {
                if ( PropertyName == "Id" ) { this.Id = ValueProc.As<Int32>(Value) ; return true; }
                if ( PropertyName == "NoticeTypeID" ) { this.NoticeTypeID = ValueProc.As<Int64>(Value) ; return true; }
                if ( PropertyName == "Name" ) { this.Name = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "Wbs" ) { this.Wbs = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "Logo" ) { this.Logo = ValueProc.As<Int32>(Value) ; return true; }
                if ( PropertyName == "Descr" ) { this.Descr = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "SortID" ) { this.SortID = ValueProc.As<Int32>(Value) ; return true; }
                if ( PropertyName == "UpdateTime" ) { this.UpdateTime = ValueProc.As<MyDate>(Value) ; return true; }
                return false ;
           }

            public object GetPropertyValue(string PropertyName)
            {
                if ( PropertyName == "Id" ) { return this.Id ; }
                if ( PropertyName == "NoticeTypeID" ) { return this.NoticeTypeID ; }
                if ( PropertyName == "Name" ) { return this.Name ; }
                if ( PropertyName == "Wbs" ) { return this.Wbs ; }
                if ( PropertyName == "Logo" ) { return this.Logo ; }
                if ( PropertyName == "Descr" ) { return this.Descr ; }
                if ( PropertyName == "SortID" ) { return this.SortID ; }
                if ( PropertyName == "UpdateTime" ) { return this.UpdateTime ; }
                return null ;
            }

            public string[]  GetProperties() { return new string[]{ "Id","NoticeTypeID","Name","Wbs","Logo","Descr","SortID","UpdateTime" } ; }
            
            public object Clone()
            {
                return this.CloneIEntity() ;
            }

            private NoticeTypeRule.Entity _GetNoticeType = null;
            public NoticeTypeRule.Entity GetNoticeType()
            {
                if (_GetNoticeType != null) return _GetNoticeType;
                _GetNoticeType = dbr.Shop.NoticeType.FindById(this.NoticeTypeID);
                return _GetNoticeType;
            }
            private AnnexRule.Entity _GetAnnex = null;
            public AnnexRule.Entity GetAnnex()
            {
                if (_GetAnnex != null) return _GetAnnex;
                _GetAnnex = dbr.Annex.FindById(this.Logo);
                return _GetAnnex;
            }
         }
    }


    
    /// <summary>
    /// 产品点击数
    /// </summary>
    [Serializable]
    public sealed partial class ProductClicksRule : RuleBase, ITableRule,ICloneable
    {


        public  ProductClicksRule() : base("ProductClicks")
        {
            this.Id = new SimpleColumn(this, DbType.Int32, 4,"Id","ID",false);
            this.Year = new SimpleColumn(this, DbType.Int32, 4,"Year","Year",false);
            this.Month = new SimpleColumn(this, DbType.Int32, 4,"Month","Month",false);
            this.ProductID = new SimpleColumn(this, DbType.Int32, 4,"ProductID","ProductID",false);
            this.Clicks = new SimpleColumn(this, DbType.Int32, 4,"Clicks","Clicks",true);
        }

        /// <summary>
        /// ID(Int32)[主键(ID),自增键]
        /// </summary>
        public SimpleColumn Id { get; set; }
        /// <summary>
        /// 年(Int32)
        /// </summary>
        public SimpleColumn Year { get; set; }
        /// <summary>
        /// 月(Int32)
        /// </summary>
        public SimpleColumn Month { get; set; }
        /// <summary>
        /// 产品ID(Int32)[外键(ProductID=EC_ProductInfo:ID)]
        /// </summary>
        public SimpleColumn ProductID { get; set; }
        /// <summary>
        /// 点击数(Int32)
        /// </summary>
        public SimpleColumn Clicks { get; set; }

        public override SimpleColumn[] GetColumns() {  return new SimpleColumn[] { Id,Year,Month,ProductID,Clicks }; }
        public override SimpleColumn[] GetPrimaryKeys() { return new SimpleColumn[] { Id };  }
        public override SimpleColumn[] GetComputeKeys() { return new SimpleColumn[] {  }; }
        public override SimpleColumn GetAutoIncreKey() {  return Id; }
        public override SimpleColumn GetUniqueKey() { return  null; }
        public override string GetDbName() { return "E_ProductClicks"; }

        public Entity FindById(Int32 Id)
        {
             if ( Id <= 0 ) return null ;
            return this.SelectWhere(o => o.Id == Id).ToEntity<Entity>();
        }
        public int DeleteById(Int32 Id)
        {
             if ( Id <= 0 ) return 0 ;
            return this.Delete(o => o.Id == Id).Execute() ;
        }


        public override object Clone()
        {
            var tab = new ProductClicksRule();
            if ( this._Config_ != null ) tab._Config_ = base._Config_.Clone() as RuleRuntimeConfig ;
            tab.SetAlias(base.Name);
            tab.SetReconfig(base.ReConfig);

            tab.Id = this.Id.Clone() as SimpleColumn;
            tab.Year = this.Year.Clone() as SimpleColumn;
            tab.Month = this.Month.Clone() as SimpleColumn;
            tab.ProductID = this.ProductID.Clone() as SimpleColumn;
            tab.Clicks = this.Clicks.Clone() as SimpleColumn;

            return tab;
        }
        /// <summary>
        /// 产品点击数 
        /// </summary>
        public Entity _ { get { return new Entity (); } }
       
        /// <summary>
        /// 产品点击数
        /// </summary>
        [Serializable]
        public sealed partial class Entity :IEntity
        {
 
            /// <summary>
            /// ID[主键(ID),自增键]
            /// </summary>
            public Int32 Id { get; set; }
 
            /// <summary>
            /// 年
            /// </summary>
            public Int32 Year { get; set; }
 
            /// <summary>
            /// 月
            /// </summary>
            public Int32 Month { get; set; }
 
            /// <summary>
            /// 产品ID[外键(ProductID=EC_ProductInfo:ID)]
            /// </summary>
            public Int32 ProductID { get; set; }
 
            /// <summary>
            /// 点击数
            /// </summary>
            public Int32 Clicks { get; set; }

            public bool SetPropertyValue(string PropertyName, object Value)
            {
                if ( PropertyName == "Id" ) { this.Id = ValueProc.As<Int32>(Value) ; return true; }
                if ( PropertyName == "Year" ) { this.Year = ValueProc.As<Int32>(Value) ; return true; }
                if ( PropertyName == "Month" ) { this.Month = ValueProc.As<Int32>(Value) ; return true; }
                if ( PropertyName == "ProductID" ) { this.ProductID = ValueProc.As<Int32>(Value) ; return true; }
                if ( PropertyName == "Clicks" ) { this.Clicks = ValueProc.As<Int32>(Value) ; return true; }
                return false ;
           }

            public object GetPropertyValue(string PropertyName)
            {
                if ( PropertyName == "Id" ) { return this.Id ; }
                if ( PropertyName == "Year" ) { return this.Year ; }
                if ( PropertyName == "Month" ) { return this.Month ; }
                if ( PropertyName == "ProductID" ) { return this.ProductID ; }
                if ( PropertyName == "Clicks" ) { return this.Clicks ; }
                return null ;
            }

            public string[]  GetProperties() { return new string[]{ "Id","Year","Month","ProductID","Clicks" } ; }
            
            public object Clone()
            {
                return this.CloneIEntity() ;
            }

            private ProductInfoRule.Entity _GetProductInfo = null;
            public ProductInfoRule.Entity GetProductInfo()
            {
                if (_GetProductInfo != null) return _GetProductInfo;
                _GetProductInfo = dbr.Shop.ProductInfo.FindById(this.ProductID);
                return _GetProductInfo;
            }
         }
    }


    
    /// <summary>
    /// 消息
    /// </summary>
    [Serializable]
    public sealed partial class ContactMsgRule : RuleBase, ITableRule,ICloneable
    {


        public  ContactMsgRule() : base("ContactMsg")
        {
            this.Id = new SimpleColumn(this, DbType.Int32, 4,"Id","ID",false);
            this.DeptID = new SimpleColumn(this, DbType.Int32, 4,"DeptID","DeptID",true);
            this.Subject = new SimpleColumn(this, DbType.AnsiString, 100,"Subject","Subject",true);
            this.Msg = new SimpleColumn(this, DbType.AnsiString, 800,"Msg","Msg",true);
            this.Url = new SimpleColumn(this, DbType.AnsiString, 500,"Url","Url",true);
            this.SenderName = new SimpleColumn(this, DbType.AnsiString, 50,"SenderName","SenderName",true);
            this.SenderUserID = new SimpleColumn(this, DbType.AnsiString, 50,"SenderUserID","SenderUserID",true);
            this.AddTime = new SimpleColumn(this, DbType.DateTime, 8,"AddTime","AddTime",true);
        }

        /// <summary>
        /// ID(Int32)[主键(ID),自增键]
        /// </summary>
        public SimpleColumn Id { get; set; }
        /// <summary>
        /// 部门ID(Int32)
        /// </summary>
        public SimpleColumn DeptID { get; set; }
        /// <summary>
        /// 主题(AnsiString)
        /// </summary>
        public SimpleColumn Subject { get; set; }
        /// <summary>
        /// 消息(AnsiString)
        /// </summary>
        public SimpleColumn Msg { get; set; }
        /// <summary>
        /// Url(AnsiString)
        /// </summary>
        public SimpleColumn Url { get; set; }
        /// <summary>
        /// 发送者(AnsiString)
        /// </summary>
        public SimpleColumn SenderName { get; set; }
        /// <summary>
        /// 发送者ID(AnsiString)
        /// </summary>
        public SimpleColumn SenderUserID { get; set; }
        /// <summary>
        /// 添加时间(DateTime)
        /// </summary>
        public SimpleColumn AddTime { get; set; }

        public override SimpleColumn[] GetColumns() {  return new SimpleColumn[] { Id,DeptID,Subject,Msg,Url,SenderName,SenderUserID,AddTime }; }
        public override SimpleColumn[] GetPrimaryKeys() { return new SimpleColumn[] { Id };  }
        public override SimpleColumn[] GetComputeKeys() { return new SimpleColumn[] {  }; }
        public override SimpleColumn GetAutoIncreKey() {  return Id; }
        public override SimpleColumn GetUniqueKey() { return  null; }
        public override string GetDbName() { return "E_ContactMsg"; }

        public Entity FindById(Int32 Id)
        {
             if ( Id <= 0 ) return null ;
            return this.SelectWhere(o => o.Id == Id).ToEntity<Entity>();
        }
        public int DeleteById(Int32 Id)
        {
             if ( Id <= 0 ) return 0 ;
            return this.Delete(o => o.Id == Id).Execute() ;
        }


        public override object Clone()
        {
            var tab = new ContactMsgRule();
            if ( this._Config_ != null ) tab._Config_ = base._Config_.Clone() as RuleRuntimeConfig ;
            tab.SetAlias(base.Name);
            tab.SetReconfig(base.ReConfig);

            tab.Id = this.Id.Clone() as SimpleColumn;
            tab.DeptID = this.DeptID.Clone() as SimpleColumn;
            tab.Subject = this.Subject.Clone() as SimpleColumn;
            tab.Msg = this.Msg.Clone() as SimpleColumn;
            tab.Url = this.Url.Clone() as SimpleColumn;
            tab.SenderName = this.SenderName.Clone() as SimpleColumn;
            tab.SenderUserID = this.SenderUserID.Clone() as SimpleColumn;
            tab.AddTime = this.AddTime.Clone() as SimpleColumn;

            return tab;
        }
        /// <summary>
        /// 消息 
        /// </summary>
        public Entity _ { get { return new Entity (); } }
       
        /// <summary>
        /// 消息
        /// </summary>
        [Serializable]
        public sealed partial class Entity :IEntity
        {
 
            /// <summary>
            /// ID[主键(ID),自增键]
            /// </summary>
            public Int32 Id { get; set; }
 
            /// <summary>
            /// 部门ID
            /// </summary>
            public Int32 DeptID { get; set; }
 
            /// <summary>
            /// 主题
            /// </summary>
            public String Subject { get; set; }
 
            /// <summary>
            /// 消息
            /// </summary>
            public String Msg { get; set; }
 
            /// <summary>
            /// Url
            /// </summary>
            public String Url { get; set; }
 
            /// <summary>
            /// 发送者
            /// </summary>
            public String SenderName { get; set; }
 
            /// <summary>
            /// 发送者ID
            /// </summary>
            public String SenderUserID { get; set; }
 
            /// <summary>
            /// 添加时间
            /// </summary>
            public MyDate AddTime { get; set; }

            public bool SetPropertyValue(string PropertyName, object Value)
            {
                if ( PropertyName == "Id" ) { this.Id = ValueProc.As<Int32>(Value) ; return true; }
                if ( PropertyName == "DeptID" ) { this.DeptID = ValueProc.As<Int32>(Value) ; return true; }
                if ( PropertyName == "Subject" ) { this.Subject = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "Msg" ) { this.Msg = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "Url" ) { this.Url = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "SenderName" ) { this.SenderName = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "SenderUserID" ) { this.SenderUserID = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "AddTime" ) { this.AddTime = ValueProc.As<MyDate>(Value) ; return true; }
                return false ;
           }

            public object GetPropertyValue(string PropertyName)
            {
                if ( PropertyName == "Id" ) { return this.Id ; }
                if ( PropertyName == "DeptID" ) { return this.DeptID ; }
                if ( PropertyName == "Subject" ) { return this.Subject ; }
                if ( PropertyName == "Msg" ) { return this.Msg ; }
                if ( PropertyName == "Url" ) { return this.Url ; }
                if ( PropertyName == "SenderName" ) { return this.SenderName ; }
                if ( PropertyName == "SenderUserID" ) { return this.SenderUserID ; }
                if ( PropertyName == "AddTime" ) { return this.AddTime ; }
                return null ;
            }

            public string[]  GetProperties() { return new string[]{ "Id","DeptID","Subject","Msg","Url","SenderName","SenderUserID","AddTime" } ; }
            
            public object Clone()
            {
                return this.CloneIEntity() ;
            }

         }
    }


    
    /// <summary>
    /// 产品附件
    /// </summary>
    [Serializable]
    public sealed partial class ProductAnnexRule : RuleBase, ITableRule,ICloneable
    {


        public  ProductAnnexRule() : base("ProductAnnex")
        {
            this.Id = new SimpleColumn(this, DbType.Int32, 4,"Id","ID",false);
            this.ProductID = new SimpleColumn(this, DbType.Int32, 4,"ProductID","ProductID",true);
            this.AnnexID = new SimpleColumn(this, DbType.Int32, 4,"AnnexID","AnnexID",true);
            this.SortID = new SimpleColumn(this, DbType.Int32, 4,"SortID","SortID",true);
        }

        /// <summary>
        /// ID(Int32)[主键(ID),自增键]
        /// </summary>
        public SimpleColumn Id { get; set; }
        /// <summary>
        /// 产品ID(Int32)[外键(ProductID=EC_ProductInfo:ID)]
        /// </summary>
        public SimpleColumn ProductID { get; set; }
        /// <summary>
        /// 附件ID(Int32)[外键(AnnexID=S_Annex:ID)]
        /// </summary>
        public SimpleColumn AnnexID { get; set; }
        /// <summary>
        /// 排序数(Int32)
        /// </summary>
        public SimpleColumn SortID { get; set; }

        public override SimpleColumn[] GetColumns() {  return new SimpleColumn[] { Id,ProductID,AnnexID,SortID }; }
        public override SimpleColumn[] GetPrimaryKeys() { return new SimpleColumn[] { Id };  }
        public override SimpleColumn[] GetComputeKeys() { return new SimpleColumn[] {  }; }
        public override SimpleColumn GetAutoIncreKey() {  return Id; }
        public override SimpleColumn GetUniqueKey() { return  null; }
        public override string GetDbName() { return "EC_ProductAnnex"; }

        public Entity FindById(Int32 Id)
        {
             if ( Id <= 0 ) return null ;
            return this.SelectWhere(o => o.Id == Id).ToEntity<Entity>();
        }
        public int DeleteById(Int32 Id)
        {
             if ( Id <= 0 ) return 0 ;
            return this.Delete(o => o.Id == Id).Execute() ;
        }


        public override object Clone()
        {
            var tab = new ProductAnnexRule();
            if ( this._Config_ != null ) tab._Config_ = base._Config_.Clone() as RuleRuntimeConfig ;
            tab.SetAlias(base.Name);
            tab.SetReconfig(base.ReConfig);

            tab.Id = this.Id.Clone() as SimpleColumn;
            tab.ProductID = this.ProductID.Clone() as SimpleColumn;
            tab.AnnexID = this.AnnexID.Clone() as SimpleColumn;
            tab.SortID = this.SortID.Clone() as SimpleColumn;

            return tab;
        }
        /// <summary>
        /// 产品附件 
        /// </summary>
        public Entity _ { get { return new Entity (); } }
       
        /// <summary>
        /// 产品附件
        /// </summary>
        [Serializable]
        public sealed partial class Entity :IEntity
        {
 
            /// <summary>
            /// ID[主键(ID),自增键]
            /// </summary>
            public Int32 Id { get; set; }
 
            /// <summary>
            /// 产品ID[外键(ProductID=EC_ProductInfo:ID)]
            /// </summary>
            public Int32 ProductID { get; set; }
 
            /// <summary>
            /// 附件ID[外键(AnnexID=S_Annex:ID)]
            /// </summary>
            public Int32 AnnexID { get; set; }
 
            /// <summary>
            /// 排序数
            /// </summary>
            public Int32 SortID { get; set; }

            public bool SetPropertyValue(string PropertyName, object Value)
            {
                if ( PropertyName == "Id" ) { this.Id = ValueProc.As<Int32>(Value) ; return true; }
                if ( PropertyName == "ProductID" ) { this.ProductID = ValueProc.As<Int32>(Value) ; return true; }
                if ( PropertyName == "AnnexID" ) { this.AnnexID = ValueProc.As<Int32>(Value) ; return true; }
                if ( PropertyName == "SortID" ) { this.SortID = ValueProc.As<Int32>(Value) ; return true; }
                return false ;
           }

            public object GetPropertyValue(string PropertyName)
            {
                if ( PropertyName == "Id" ) { return this.Id ; }
                if ( PropertyName == "ProductID" ) { return this.ProductID ; }
                if ( PropertyName == "AnnexID" ) { return this.AnnexID ; }
                if ( PropertyName == "SortID" ) { return this.SortID ; }
                return null ;
            }

            public string[]  GetProperties() { return new string[]{ "Id","ProductID","AnnexID","SortID" } ; }
            
            public object Clone()
            {
                return this.CloneIEntity() ;
            }

            private ProductInfoRule.Entity _GetProductInfo = null;
            public ProductInfoRule.Entity GetProductInfo()
            {
                if (_GetProductInfo != null) return _GetProductInfo;
                _GetProductInfo = dbr.Shop.ProductInfo.FindById(this.ProductID);
                return _GetProductInfo;
            }
            private AnnexRule.Entity _GetAnnex = null;
            public AnnexRule.Entity GetAnnex()
            {
                if (_GetAnnex != null) return _GetAnnex;
                _GetAnnex = dbr.Annex.FindById(this.AnnexID);
                return _GetAnnex;
            }
         }
    }


    
    /// <summary>
    /// 产品信息
    /// </summary>
    [Serializable]
    public sealed partial class ProductInfoRule : RuleBase, ITableRule,ICloneable
    {


        public  ProductInfoRule() : base("ProductInfo")
        {
            this.Id = new SimpleColumn(this, DbType.Int32, 4,"Id","ID",false);
            this.ProductTypeID = new SimpleColumn(this, DbType.Int32, 4,"ProductTypeID","ProductTypeID",true);
            this.Name = new SimpleColumn(this, DbType.AnsiString, 100,"Name","Name",true);
            this.Wbs = new SimpleColumn(this, DbType.AnsiString, 50,"Wbs","Wbs",true);
            this.Logo = new SimpleColumn(this, DbType.Int32, 4,"Logo","Logo",true);
            this.Descr = new SimpleColumn(this, DbType.AnsiString, 500,"Descr","Descr",true);
            this.SortID = new SimpleColumn(this, DbType.Int32, 4,"SortID","SortID",true);
            this.UpdateTime = new SimpleColumn(this, DbType.DateTime, 8,"UpdateTime","UpdateTime",true);
        }

        /// <summary>
        /// ID(Int32)[主键(ID),自增键]
        /// </summary>
        public SimpleColumn Id { get; set; }
        /// <summary>
        /// 产品类型ID(Int32)[外键(ProductTypeID=EC_ProductType:ID)]
        /// </summary>
        public SimpleColumn ProductTypeID { get; set; }
        /// <summary>
        /// 名称(AnsiString)
        /// </summary>
        public SimpleColumn Name { get; set; }
        /// <summary>
        /// 从根到父节点的路径(AnsiString)
        /// </summary>
        public SimpleColumn Wbs { get; set; }
        /// <summary>
        /// 产品缩略图(Int32)[外键(Logo=S_Annex:ID)]
        /// </summary>
        public SimpleColumn Logo { get; set; }
        /// <summary>
        /// 描述(AnsiString)
        /// </summary>
        public SimpleColumn Descr { get; set; }
        /// <summary>
        /// 排序数(Int32)
        /// </summary>
        public SimpleColumn SortID { get; set; }
        /// <summary>
        /// 更新时间(DateTime)
        /// </summary>
        public SimpleColumn UpdateTime { get; set; }

        public override SimpleColumn[] GetColumns() {  return new SimpleColumn[] { Id,ProductTypeID,Name,Wbs,Logo,Descr,SortID,UpdateTime }; }
        public override SimpleColumn[] GetPrimaryKeys() { return new SimpleColumn[] { Id };  }
        public override SimpleColumn[] GetComputeKeys() { return new SimpleColumn[] {  }; }
        public override SimpleColumn GetAutoIncreKey() {  return Id; }
        public override SimpleColumn GetUniqueKey() { return  null; }
        public override string GetDbName() { return "EC_ProductInfo"; }

        public Entity FindById(Int32 Id)
        {
             if ( Id <= 0 ) return null ;
            return this.SelectWhere(o => o.Id == Id).ToEntity<Entity>();
        }
        public int DeleteById(Int32 Id)
        {
             if ( Id <= 0 ) return 0 ;
            return this.Delete(o => o.Id == Id).Execute() ;
        }


        public override object Clone()
        {
            var tab = new ProductInfoRule();
            if ( this._Config_ != null ) tab._Config_ = base._Config_.Clone() as RuleRuntimeConfig ;
            tab.SetAlias(base.Name);
            tab.SetReconfig(base.ReConfig);

            tab.Id = this.Id.Clone() as SimpleColumn;
            tab.ProductTypeID = this.ProductTypeID.Clone() as SimpleColumn;
            tab.Name = this.Name.Clone() as SimpleColumn;
            tab.Wbs = this.Wbs.Clone() as SimpleColumn;
            tab.Logo = this.Logo.Clone() as SimpleColumn;
            tab.Descr = this.Descr.Clone() as SimpleColumn;
            tab.SortID = this.SortID.Clone() as SimpleColumn;
            tab.UpdateTime = this.UpdateTime.Clone() as SimpleColumn;

            return tab;
        }
        /// <summary>
        /// 产品信息 
        /// </summary>
        public Entity _ { get { return new Entity (); } }
       
        /// <summary>
        /// 产品信息
        /// </summary>
        [Serializable]
        public sealed partial class Entity :IEntity
        {
 
            /// <summary>
            /// ID[主键(ID),自增键]
            /// </summary>
            public Int32 Id { get; set; }
 
            /// <summary>
            /// 产品类型ID[外键(ProductTypeID=EC_ProductType:ID)]
            /// </summary>
            public Int32 ProductTypeID { get; set; }
 
            /// <summary>
            /// 名称
            /// </summary>
            public String Name { get; set; }
 
            /// <summary>
            /// 从根到父节点的路径
            /// </summary>
            public String Wbs { get; set; }
 
            /// <summary>
            /// 产品缩略图[外键(Logo=S_Annex:ID)]
            /// </summary>
            public Int32 Logo { get; set; }
 
            /// <summary>
            /// 描述
            /// </summary>
            public String Descr { get; set; }
 
            /// <summary>
            /// 排序数
            /// </summary>
            public Int32 SortID { get; set; }
 
            /// <summary>
            /// 更新时间
            /// </summary>
            public MyDate UpdateTime { get; set; }

            public bool SetPropertyValue(string PropertyName, object Value)
            {
                if ( PropertyName == "Id" ) { this.Id = ValueProc.As<Int32>(Value) ; return true; }
                if ( PropertyName == "ProductTypeID" ) { this.ProductTypeID = ValueProc.As<Int32>(Value) ; return true; }
                if ( PropertyName == "Name" ) { this.Name = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "Wbs" ) { this.Wbs = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "Logo" ) { this.Logo = ValueProc.As<Int32>(Value) ; return true; }
                if ( PropertyName == "Descr" ) { this.Descr = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "SortID" ) { this.SortID = ValueProc.As<Int32>(Value) ; return true; }
                if ( PropertyName == "UpdateTime" ) { this.UpdateTime = ValueProc.As<MyDate>(Value) ; return true; }
                return false ;
           }

            public object GetPropertyValue(string PropertyName)
            {
                if ( PropertyName == "Id" ) { return this.Id ; }
                if ( PropertyName == "ProductTypeID" ) { return this.ProductTypeID ; }
                if ( PropertyName == "Name" ) { return this.Name ; }
                if ( PropertyName == "Wbs" ) { return this.Wbs ; }
                if ( PropertyName == "Logo" ) { return this.Logo ; }
                if ( PropertyName == "Descr" ) { return this.Descr ; }
                if ( PropertyName == "SortID" ) { return this.SortID ; }
                if ( PropertyName == "UpdateTime" ) { return this.UpdateTime ; }
                return null ;
            }

            public string[]  GetProperties() { return new string[]{ "Id","ProductTypeID","Name","Wbs","Logo","Descr","SortID","UpdateTime" } ; }
            
            public object Clone()
            {
                return this.CloneIEntity() ;
            }

            private ProductTypeRule.Entity _GetProductType = null;
            public ProductTypeRule.Entity GetProductType()
            {
                if (_GetProductType != null) return _GetProductType;
                _GetProductType = dbr.Shop.ProductType.FindById(this.ProductTypeID);
                return _GetProductType;
            }
            private AnnexRule.Entity _GetAnnex = null;
            public AnnexRule.Entity GetAnnex()
            {
                if (_GetAnnex != null) return _GetAnnex;
                _GetAnnex = dbr.Annex.FindById(this.Logo);
                return _GetAnnex;
            }
            private ProductClicksRule.Entity[] _GetProductClickss = null;
            public ProductClicksRule.Entity[] GetProductClickss()
            {
                if (_GetProductClickss != null) return _GetProductClickss;
                _GetProductClickss = dbr.Shop.ProductClicks.SelectWhere(o => o.ProductID == this.Id).ToEntityList<ProductClicksRule.Entity>().ToArray();
                return _GetProductClickss;
            }
            private ProductAnnexRule.Entity[] _GetProductAnnexs = null;
            public ProductAnnexRule.Entity[] GetProductAnnexs()
            {
                if (_GetProductAnnexs != null) return _GetProductAnnexs;
                _GetProductAnnexs = dbr.Shop.ProductAnnex.SelectWhere(o => o.ProductID == this.Id).ToEntityList<ProductAnnexRule.Entity>().ToArray();
                return _GetProductAnnexs;
            }
            private ProductDetailRule.Entity[] _GetProductDetails = null;
            public ProductDetailRule.Entity[] GetProductDetails()
            {
                if (_GetProductDetails != null) return _GetProductDetails;
                _GetProductDetails = dbr.Shop.ProductDetail.SelectWhere(o => o.ProductID == this.Id).ToEntityList<ProductDetailRule.Entity>().ToArray();
                return _GetProductDetails;
            }
         }
    }


    
    /// <summary>
    /// 产品详情
    /// </summary>
    [Serializable]
    public sealed partial class ProductDetailRule : RuleBase, ITableRule,ICloneable
    {


        public  ProductDetailRule() : base("ProductDetail")
        {
            this.Id = new SimpleColumn(this, DbType.Int32, 4,"Id","ID",false);
            this.ProductID = new SimpleColumn(this, DbType.Int32, 4,"ProductID","ProductID",true);
            this.Key = new SimpleColumn(this, DbType.AnsiString, 50,"Key","Key",true);
            this.Value = new SimpleColumn(this, DbType.AnsiString, 500,"Value","Value",true);
            this.IsCaption = new SimpleColumn(this, DbType.Boolean, 1,"IsCaption","IsCaption",true);
            this.SortID = new SimpleColumn(this, DbType.Int32, 4,"SortID","SortID",true);
            this.UserID = new SimpleColumn(this, DbType.AnsiString, 50,"UserID","UserID",false);
            this.AddTime = new SimpleColumn(this, DbType.DateTime, 8,"AddTime","AddTime",false);
        }

        /// <summary>
        /// ID(Int32)[主键(ID),自增键]
        /// </summary>
        public SimpleColumn Id { get; set; }
        /// <summary>
        /// 产品ID(Int32)[外键(ProductID=EC_ProductInfo:ID)]
        /// </summary>
        public SimpleColumn ProductID { get; set; }
        /// <summary>
        /// 键(AnsiString)
        /// </summary>
        public SimpleColumn Key { get; set; }
        /// <summary>
        /// 值(AnsiString)
        /// </summary>
        public SimpleColumn Value { get; set; }
        /// <summary>
        /// 是否是标题(Boolean)
        /// </summary>
        public SimpleColumn IsCaption { get; set; }
        /// <summary>
        /// 排序数(Int32)
        /// </summary>
        public SimpleColumn SortID { get; set; }
        /// <summary>
        /// 用户ID(AnsiString)
        /// </summary>
        public SimpleColumn UserID { get; set; }
        /// <summary>
        /// 添加时间(DateTime)
        /// </summary>
        public SimpleColumn AddTime { get; set; }

        public override SimpleColumn[] GetColumns() {  return new SimpleColumn[] { Id,ProductID,Key,Value,IsCaption,SortID,UserID,AddTime }; }
        public override SimpleColumn[] GetPrimaryKeys() { return new SimpleColumn[] { Id };  }
        public override SimpleColumn[] GetComputeKeys() { return new SimpleColumn[] {  }; }
        public override SimpleColumn GetAutoIncreKey() {  return Id; }
        public override SimpleColumn GetUniqueKey() { return  null; }
        public override string GetDbName() { return "EC_ProductDetail"; }

        public Entity FindById(Int32 Id)
        {
             if ( Id <= 0 ) return null ;
            return this.SelectWhere(o => o.Id == Id).ToEntity<Entity>();
        }
        public int DeleteById(Int32 Id)
        {
             if ( Id <= 0 ) return 0 ;
            return this.Delete(o => o.Id == Id).Execute() ;
        }


        public override object Clone()
        {
            var tab = new ProductDetailRule();
            if ( this._Config_ != null ) tab._Config_ = base._Config_.Clone() as RuleRuntimeConfig ;
            tab.SetAlias(base.Name);
            tab.SetReconfig(base.ReConfig);

            tab.Id = this.Id.Clone() as SimpleColumn;
            tab.ProductID = this.ProductID.Clone() as SimpleColumn;
            tab.Key = this.Key.Clone() as SimpleColumn;
            tab.Value = this.Value.Clone() as SimpleColumn;
            tab.IsCaption = this.IsCaption.Clone() as SimpleColumn;
            tab.SortID = this.SortID.Clone() as SimpleColumn;
            tab.UserID = this.UserID.Clone() as SimpleColumn;
            tab.AddTime = this.AddTime.Clone() as SimpleColumn;

            return tab;
        }
        /// <summary>
        /// 产品详情 
        /// </summary>
        public Entity _ { get { return new Entity (); } }
       
        /// <summary>
        /// 产品详情
        /// </summary>
        [Serializable]
        public sealed partial class Entity :IEntity
        {
 
            /// <summary>
            /// ID[主键(ID),自增键]
            /// </summary>
            public Int32 Id { get; set; }
 
            /// <summary>
            /// 产品ID[外键(ProductID=EC_ProductInfo:ID)]
            /// </summary>
            public Int32 ProductID { get; set; }
 
            /// <summary>
            /// 键
            /// </summary>
            public String Key { get; set; }
 
            /// <summary>
            /// 值
            /// </summary>
            public String Value { get; set; }
 
            /// <summary>
            /// 是否是标题
            /// </summary>
            public Boolean IsCaption { get; set; }
 
            /// <summary>
            /// 排序数
            /// </summary>
            public Int32 SortID { get; set; }
 
            /// <summary>
            /// 用户ID
            /// </summary>
            public String UserID { get; set; }
 
            /// <summary>
            /// 添加时间
            /// </summary>
            public MyDate AddTime { get; set; }

            public bool SetPropertyValue(string PropertyName, object Value)
            {
                if ( PropertyName == "Id" ) { this.Id = ValueProc.As<Int32>(Value) ; return true; }
                if ( PropertyName == "ProductID" ) { this.ProductID = ValueProc.As<Int32>(Value) ; return true; }
                if ( PropertyName == "Key" ) { this.Key = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "Value" ) { this.Value = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "IsCaption" ) { this.IsCaption = ValueProc.As<Boolean>(Value) ; return true; }
                if ( PropertyName == "SortID" ) { this.SortID = ValueProc.As<Int32>(Value) ; return true; }
                if ( PropertyName == "UserID" ) { this.UserID = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "AddTime" ) { this.AddTime = ValueProc.As<MyDate>(Value) ; return true; }
                return false ;
           }

            public object GetPropertyValue(string PropertyName)
            {
                if ( PropertyName == "Id" ) { return this.Id ; }
                if ( PropertyName == "ProductID" ) { return this.ProductID ; }
                if ( PropertyName == "Key" ) { return this.Key ; }
                if ( PropertyName == "Value" ) { return this.Value ; }
                if ( PropertyName == "IsCaption" ) { return this.IsCaption ; }
                if ( PropertyName == "SortID" ) { return this.SortID ; }
                if ( PropertyName == "UserID" ) { return this.UserID ; }
                if ( PropertyName == "AddTime" ) { return this.AddTime ; }
                return null ;
            }

            public string[]  GetProperties() { return new string[]{ "Id","ProductID","Key","Value","IsCaption","SortID","UserID","AddTime" } ; }
            
            public object Clone()
            {
                return this.CloneIEntity() ;
            }

            private ProductInfoRule.Entity _GetProductInfo = null;
            public ProductInfoRule.Entity GetProductInfo()
            {
                if (_GetProductInfo != null) return _GetProductInfo;
                _GetProductInfo = dbr.Shop.ProductInfo.FindById(this.ProductID);
                return _GetProductInfo;
            }
         }
    }




    
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public sealed partial class SplitRule : RuleBase, IFunctionRule,ICloneable
    {
            protected System.String _val ;
            protected System.String _split ;

            public string[] GetParameters() { return new string[] { "val" , "split" }; }

            public object GetParameterValue(string Parameter)
            {
                  if (Parameter == "val")
                    return this._val;
                  if (Parameter == "split")
                    return this._split;
                return null;
            }

            public SplitRule(System.String val ,System.String split ) 
                : this ()
            {
                    this._val = val ;
                    this._split = split ;
                
                //函数表必须有别名。
                this.SetAlias(this.GetName());
            }


        private  SplitRule() : base("Split")
        {
            this.Value = new SimpleColumn(this, DbType.String, 0,"Value","Value",false);
        }

        /// <summary>
        /// Value(String)
        /// </summary>
        public SimpleColumn Value { get; set; }

        public override SimpleColumn[] GetColumns() {  return new SimpleColumn[] { Value }; }
        public override SimpleColumn[] GetPrimaryKeys() { return new SimpleColumn[] {  };  }
        public override SimpleColumn[] GetComputeKeys() { return new SimpleColumn[] {  }; }
        public override SimpleColumn GetAutoIncreKey() {  return null; }
        public override SimpleColumn GetUniqueKey() { return  null; }
        public override string GetDbName() { return "Split"; }



        public override object Clone()
        {
            var tab = new SplitRule();
            if ( this._Config_ != null ) tab._Config_ = base._Config_.Clone() as RuleRuntimeConfig ;
            tab.SetAlias(base.Name);
            tab.SetReconfig(base.ReConfig);

            tab.Value = this.Value.Clone() as SimpleColumn;
            tab._val = this._val ;
            tab._split = this._split ;

            return tab;
        }
        /// <summary>
        ///  
        /// </summary>
        public Entity _ { get { return new Entity (); } }
       
        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public sealed partial class Entity :IEntity
        {
 
            /// <summary>
            /// Value
            /// </summary>
            public String Value { get; set; }

            public bool SetPropertyValue(string PropertyName, object Value)
            {
                if ( PropertyName == "Value" ) { this.Value = ValueProc.As<String>(Value) ; return true; }
                return false ;
           }

            public object GetPropertyValue(string PropertyName)
            {
                if ( PropertyName == "Value" ) { return this.Value ; }
                return null ;
            }

            public string[]  GetProperties() { return new string[]{ "Value" } ; }
            
            public object Clone()
            {
                return this.CloneIEntity() ;
            }

         }
    }



}