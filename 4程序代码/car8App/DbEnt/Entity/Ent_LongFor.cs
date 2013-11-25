//系统自动生成的实体，不能修改。 By: UDI-PC.  At:2013-11-15 13:05:54
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
    /// TU_Voucher_OutSetItem
    /// </summary>
    [Serializable]
    public sealed partial class CommunityRule : RuleBase, ITableRule,ICloneable
    {


        public  CommunityRule() : base("Community")
        {
            this.CommID = new SimpleColumn(this, DbType.Int32, 4,"CommID","CommID",false);
            this.BranchID = new SimpleColumn(this, DbType.Int32, 4,"BranchID","BranchID",true);
            this.CorpID = new SimpleColumn(this, DbType.Int32, 4,"CorpID","CorpID",true);
            this.CommName = new SimpleColumn(this, DbType.String, 100,"CommName","CommName",true);
            this.CommType = new SimpleColumn(this, DbType.Int16, 2,"CommType","CommType",true);
            this.CorpRegionCode = new SimpleColumn(this, DbType.String, 40,"CorpRegionCode","CorpRegionCode",true);
            this.CommAddress = new SimpleColumn(this, DbType.String, 100,"CommAddress","CommAddress",true);
            this.Province = new SimpleColumn(this, DbType.String, 40,"Province","Province",true);
            this.City = new SimpleColumn(this, DbType.String, 40,"City","City",true);
            this.Borough = new SimpleColumn(this, DbType.String, 40,"Borough","Borough",true);
            this.Street = new SimpleColumn(this, DbType.String, 100,"Street","Street",true);
            this.GateSign = new SimpleColumn(this, DbType.String, 40,"GateSign","GateSign",true);
            this.CorpGroupCode = new SimpleColumn(this, DbType.String, 40,"CorpGroupCode","CorpGroupCode",true);
            this.CommSpell = new SimpleColumn(this, DbType.String, 40,"CommSpell","CommSpell",true);
            this.RegDate = new SimpleColumn(this, DbType.DateTime, 8,"RegDate","RegDate",true);
            this.IsDelete = new SimpleColumn(this, DbType.Int16, 2,"IsDelete","IsDelete",true);
            this.OrganCode = new SimpleColumn(this, DbType.String, 40,"OrganCode","OrganCode",true);
            this.CsmProjectID = new SimpleColumn(this, DbType.AnsiString, 36,"CsmProjectID","CsmProjectID",true);
            this.Detail = new SimpleColumn(this, DbType.String, -1,"Detail","Detail",true);
        }

        /// <summary>
        /// CommID(Int32)[主键(CommID)]
        /// </summary>
        public SimpleColumn CommID { get; set; }
        /// <summary>
        /// BranchID(Int32)
        /// </summary>
        public SimpleColumn BranchID { get; set; }
        /// <summary>
        /// CorpID(Int32)
        /// </summary>
        public SimpleColumn CorpID { get; set; }
        /// <summary>
        /// 项目名(String)
        /// </summary>
        public SimpleColumn CommName { get; set; }
        /// <summary>
        /// 项目类型(Int16)
        /// </summary>
        public SimpleColumn CommType { get; set; }
        /// <summary>
        /// 所属公司(String)
        /// </summary>
        public SimpleColumn CorpRegionCode { get; set; }
        /// <summary>
        /// 地址(String)
        /// </summary>
        public SimpleColumn CommAddress { get; set; }
        /// <summary>
        /// 所在地区(String)
        /// </summary>
        public SimpleColumn Province { get; set; }
        /// <summary>
        /// 所在市(String)
        /// </summary>
        public SimpleColumn City { get; set; }
        /// <summary>
        /// 所在区(String)
        /// </summary>
        public SimpleColumn Borough { get; set; }
        /// <summary>
        /// 街道名(String)
        /// </summary>
        public SimpleColumn Street { get; set; }
        /// <summary>
        /// 门牌号(String)
        /// </summary>
        public SimpleColumn GateSign { get; set; }
        /// <summary>
        /// CorpGroupCode(String)
        /// </summary>
        public SimpleColumn CorpGroupCode { get; set; }
        /// <summary>
        /// CommSpell(String)
        /// </summary>
        public SimpleColumn CommSpell { get; set; }
        /// <summary>
        /// RegDate(DateTime)
        /// </summary>
        public SimpleColumn RegDate { get; set; }
        /// <summary>
        /// IsDelete(Int16)
        /// </summary>
        public SimpleColumn IsDelete { get; set; }
        /// <summary>
        /// OrganCode(String)
        /// </summary>
        public SimpleColumn OrganCode { get; set; }
        /// <summary>
        /// Csm项目ID(AnsiString)
        /// </summary>
        public SimpleColumn CsmProjectID { get; set; }
        /// <summary>
        /// 小区介绍(String)
        /// </summary>
        public SimpleColumn Detail { get; set; }

        public override SimpleColumn[] GetColumns() {  return new SimpleColumn[] { CommID,BranchID,CorpID,CommName,CommType,CorpRegionCode,CommAddress,Province,City,Borough,Street,GateSign,CorpGroupCode,CommSpell,RegDate,IsDelete,OrganCode,CsmProjectID,Detail }; }
        public override SimpleColumn[] GetPrimaryKeys() { return new SimpleColumn[] { CommID };  }
        public override SimpleColumn[] GetComputeKeys() { return new SimpleColumn[] {  }; }
        public override SimpleColumn GetAutoIncreKey() {  return null; }
        public override SimpleColumn GetUniqueKey() { return  null; }
        public override string GetDbName() { return "TM_Community"; }

        public Entity FindByCommID(Int32 CommID)
        {
             if ( CommID <= 0 ) return null ;
            return this.SelectWhere(o => o.CommID == CommID).ToEntity<Entity>();
        }
        public int DeleteByCommID(Int32 CommID)
        {
             if ( CommID <= 0 ) return 0 ;
            return this.Delete(o => o.CommID == CommID).Execute() ;
        }


        public override object Clone()
        {
            var tab = new CommunityRule();
            if ( this._Config_ != null ) tab._Config_ = base._Config_.Clone() as RuleRuntimeConfig ;
            tab.SetAlias(base.Name);
            tab.SetReconfig(base.ReConfig);

            tab.CommID = this.CommID.Clone() as SimpleColumn;
            tab.BranchID = this.BranchID.Clone() as SimpleColumn;
            tab.CorpID = this.CorpID.Clone() as SimpleColumn;
            tab.CommName = this.CommName.Clone() as SimpleColumn;
            tab.CommType = this.CommType.Clone() as SimpleColumn;
            tab.CorpRegionCode = this.CorpRegionCode.Clone() as SimpleColumn;
            tab.CommAddress = this.CommAddress.Clone() as SimpleColumn;
            tab.Province = this.Province.Clone() as SimpleColumn;
            tab.City = this.City.Clone() as SimpleColumn;
            tab.Borough = this.Borough.Clone() as SimpleColumn;
            tab.Street = this.Street.Clone() as SimpleColumn;
            tab.GateSign = this.GateSign.Clone() as SimpleColumn;
            tab.CorpGroupCode = this.CorpGroupCode.Clone() as SimpleColumn;
            tab.CommSpell = this.CommSpell.Clone() as SimpleColumn;
            tab.RegDate = this.RegDate.Clone() as SimpleColumn;
            tab.IsDelete = this.IsDelete.Clone() as SimpleColumn;
            tab.OrganCode = this.OrganCode.Clone() as SimpleColumn;
            tab.CsmProjectID = this.CsmProjectID.Clone() as SimpleColumn;
            tab.Detail = this.Detail.Clone() as SimpleColumn;

            return tab;
        }
        /// <summary>
        /// TU_Voucher_OutSetItem 
        /// </summary>
        public Entity _ { get { return new Entity (); } }
       
        /// <summary>
        /// TU_Voucher_OutSetItem
        /// </summary>
        [Serializable]
        public sealed partial class Entity :IEntity
        {
 
            /// <summary>
            /// CommID[主键(CommID)]
            /// </summary>
            public Int32 CommID { get; set; }
 
            /// <summary>
            /// BranchID
            /// </summary>
            public Int32 BranchID { get; set; }
 
            /// <summary>
            /// CorpID
            /// </summary>
            public Int32 CorpID { get; set; }
 
            /// <summary>
            /// 项目名
            /// </summary>
            public String CommName { get; set; }
 
            /// <summary>
            /// 项目类型
            /// </summary>
            public Int16 CommType { get; set; }
 
            /// <summary>
            /// 所属公司
            /// </summary>
            public String CorpRegionCode { get; set; }
 
            /// <summary>
            /// 地址
            /// </summary>
            public String CommAddress { get; set; }
 
            /// <summary>
            /// 所在地区
            /// </summary>
            public String Province { get; set; }
 
            /// <summary>
            /// 所在市
            /// </summary>
            public String City { get; set; }
 
            /// <summary>
            /// 所在区
            /// </summary>
            public String Borough { get; set; }
 
            /// <summary>
            /// 街道名
            /// </summary>
            public String Street { get; set; }
 
            /// <summary>
            /// 门牌号
            /// </summary>
            public String GateSign { get; set; }
 
            /// <summary>
            /// CorpGroupCode
            /// </summary>
            public String CorpGroupCode { get; set; }
 
            /// <summary>
            /// CommSpell
            /// </summary>
            public String CommSpell { get; set; }
 
            /// <summary>
            /// RegDate
            /// </summary>
            public MyDate RegDate { get; set; }
 
            /// <summary>
            /// IsDelete
            /// </summary>
            public Int16 IsDelete { get; set; }
 
            /// <summary>
            /// OrganCode
            /// </summary>
            public String OrganCode { get; set; }
 
            /// <summary>
            /// Csm项目ID
            /// </summary>
            public String CsmProjectID { get; set; }
 
            /// <summary>
            /// 小区介绍
            /// </summary>
            public String Detail { get; set; }

            public bool SetPropertyValue(string PropertyName, object Value)
            {
                if ( PropertyName == "CommID" ) { this.CommID = ValueProc.As<Int32>(Value) ; return true; }
                if ( PropertyName == "BranchID" ) { this.BranchID = ValueProc.As<Int32>(Value) ; return true; }
                if ( PropertyName == "CorpID" ) { this.CorpID = ValueProc.As<Int32>(Value) ; return true; }
                if ( PropertyName == "CommName" ) { this.CommName = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "CommType" ) { this.CommType = ValueProc.As<Int16>(Value) ; return true; }
                if ( PropertyName == "CorpRegionCode" ) { this.CorpRegionCode = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "CommAddress" ) { this.CommAddress = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "Province" ) { this.Province = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "City" ) { this.City = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "Borough" ) { this.Borough = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "Street" ) { this.Street = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "GateSign" ) { this.GateSign = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "CorpGroupCode" ) { this.CorpGroupCode = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "CommSpell" ) { this.CommSpell = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "RegDate" ) { this.RegDate = ValueProc.As<MyDate>(Value) ; return true; }
                if ( PropertyName == "IsDelete" ) { this.IsDelete = ValueProc.As<Int16>(Value) ; return true; }
                if ( PropertyName == "OrganCode" ) { this.OrganCode = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "CsmProjectID" ) { this.CsmProjectID = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "Detail" ) { this.Detail = ValueProc.As<String>(Value) ; return true; }
                return false ;
           }

            public object GetPropertyValue(string PropertyName)
            {
                if ( PropertyName == "CommID" ) { return this.CommID ; }
                if ( PropertyName == "BranchID" ) { return this.BranchID ; }
                if ( PropertyName == "CorpID" ) { return this.CorpID ; }
                if ( PropertyName == "CommName" ) { return this.CommName ; }
                if ( PropertyName == "CommType" ) { return this.CommType ; }
                if ( PropertyName == "CorpRegionCode" ) { return this.CorpRegionCode ; }
                if ( PropertyName == "CommAddress" ) { return this.CommAddress ; }
                if ( PropertyName == "Province" ) { return this.Province ; }
                if ( PropertyName == "City" ) { return this.City ; }
                if ( PropertyName == "Borough" ) { return this.Borough ; }
                if ( PropertyName == "Street" ) { return this.Street ; }
                if ( PropertyName == "GateSign" ) { return this.GateSign ; }
                if ( PropertyName == "CorpGroupCode" ) { return this.CorpGroupCode ; }
                if ( PropertyName == "CommSpell" ) { return this.CommSpell ; }
                if ( PropertyName == "RegDate" ) { return this.RegDate ; }
                if ( PropertyName == "IsDelete" ) { return this.IsDelete ; }
                if ( PropertyName == "OrganCode" ) { return this.OrganCode ; }
                if ( PropertyName == "CsmProjectID" ) { return this.CsmProjectID ; }
                if ( PropertyName == "Detail" ) { return this.Detail ; }
                return null ;
            }

            public string[]  GetProperties() { return new string[]{ "CommID","BranchID","CorpID","CommName","CommType","CorpRegionCode","CommAddress","Province","City","Borough","Street","GateSign","CorpGroupCode","CommSpell","RegDate","IsDelete","OrganCode","CsmProjectID","Detail" } ; }
            
            public object Clone()
            {
                return this.CloneIEntity() ;
            }

         }
    }


    
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public sealed partial class TStandardRoleRule : RuleBase, ITableRule,ICloneable
    {


        public  TStandardRoleRule() : base("TStandardRole")
        {
            this.StandardRoleId = new SimpleColumn(this, DbType.AnsiString, 36,"StandardRoleId","STANDARD_ROLE_ID",false);
            this.Code = new SimpleColumn(this, DbType.AnsiString, 30,"Code","CODE",true);
            this.Name = new SimpleColumn(this, DbType.AnsiString, 50,"Name","NAME",true);
            this.Type = new SimpleColumn(this, DbType.Int32, 4,"Type","TYPE",true);
            this.ParentId = new SimpleColumn(this, DbType.AnsiString, 36,"ParentId","PARENT_ID",true);
            this.StandardOrganizationId = new SimpleColumn(this, DbType.AnsiString, 36,"StandardOrganizationId","STANDARD_ORGANIZATION_ID",true);
            this.OrderId = new SimpleColumn(this, DbType.AnsiString, 20,"OrderId","ORDER_ID",true);
            this.Status = new SimpleColumn(this, DbType.Int32, 4,"Status","STATUS",true);
            this.Remark = new SimpleColumn(this, DbType.AnsiString, 200,"Remark","REMARK",true);
            this.UpdateDate = new SimpleColumn(this, DbType.DateTime, 8,"UpdateDate","UPDATE_DATE",true);
            this.Power = new SimpleColumn(this, DbType.AnsiString, 2000,"Power","POWER",true);
        }

        /// <summary>
        /// STANDARD_ROLE_ID(AnsiString)[主键(STANDARD_ROLE_ID)]
        /// </summary>
        public SimpleColumn StandardRoleId { get; set; }
        /// <summary>
        /// CODE(AnsiString)
        /// </summary>
        public SimpleColumn Code { get; set; }
        /// <summary>
        /// NAME(AnsiString)
        /// </summary>
        public SimpleColumn Name { get; set; }
        /// <summary>
        /// TYPE(Int32)
        /// </summary>
        public SimpleColumn Type { get; set; }
        /// <summary>
        /// PARENT_ID(AnsiString)
        /// </summary>
        public SimpleColumn ParentId { get; set; }
        /// <summary>
        /// STANDARD_ORGANIZATION_ID(AnsiString)
        /// </summary>
        public SimpleColumn StandardOrganizationId { get; set; }
        /// <summary>
        /// ORDER_ID(AnsiString)
        /// </summary>
        public SimpleColumn OrderId { get; set; }
        /// <summary>
        /// STATUS(Int32)
        /// </summary>
        public SimpleColumn Status { get; set; }
        /// <summary>
        /// REMARK(AnsiString)
        /// </summary>
        public SimpleColumn Remark { get; set; }
        /// <summary>
        /// UPDATE_DATE(DateTime)
        /// </summary>
        public SimpleColumn UpdateDate { get; set; }
        /// <summary>
        /// POWER(AnsiString)
        /// </summary>
        public SimpleColumn Power { get; set; }

        public override SimpleColumn[] GetColumns() {  return new SimpleColumn[] { StandardRoleId,Code,Name,Type,ParentId,StandardOrganizationId,OrderId,Status,Remark,UpdateDate,Power }; }
        public override SimpleColumn[] GetPrimaryKeys() { return new SimpleColumn[] { StandardRoleId };  }
        public override SimpleColumn[] GetComputeKeys() { return new SimpleColumn[] {  }; }
        public override SimpleColumn GetAutoIncreKey() {  return null; }
        public override SimpleColumn GetUniqueKey() { return  null; }
        public override string GetDbName() { return "T_STANDARD_ROLE"; }

        public Entity FindByStandardRoleId(Guid StandardRoleId)
        {
            if ( StandardRoleId.HasValue() == false ) return null ;
            return this.SelectWhere(o => o.StandardRoleId == StandardRoleId).ToEntity<Entity>();
        }
        public int DeleteByStandardRoleId(Guid StandardRoleId)
        {
            if ( StandardRoleId.HasValue() == false ) return 0 ;
            return this.Delete(o => o.StandardRoleId == StandardRoleId).Execute() ;
        }


        public override object Clone()
        {
            var tab = new TStandardRoleRule();
            if ( this._Config_ != null ) tab._Config_ = base._Config_.Clone() as RuleRuntimeConfig ;
            tab.SetAlias(base.Name);
            tab.SetReconfig(base.ReConfig);

            tab.StandardRoleId = this.StandardRoleId.Clone() as SimpleColumn;
            tab.Code = this.Code.Clone() as SimpleColumn;
            tab.Name = this.Name.Clone() as SimpleColumn;
            tab.Type = this.Type.Clone() as SimpleColumn;
            tab.ParentId = this.ParentId.Clone() as SimpleColumn;
            tab.StandardOrganizationId = this.StandardOrganizationId.Clone() as SimpleColumn;
            tab.OrderId = this.OrderId.Clone() as SimpleColumn;
            tab.Status = this.Status.Clone() as SimpleColumn;
            tab.Remark = this.Remark.Clone() as SimpleColumn;
            tab.UpdateDate = this.UpdateDate.Clone() as SimpleColumn;
            tab.Power = this.Power.Clone() as SimpleColumn;

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
            /// STANDARD_ROLE_ID[主键(STANDARD_ROLE_ID)]
            /// </summary>
            public Guid StandardRoleId { get; set; }
 
            /// <summary>
            /// CODE
            /// </summary>
            public String Code { get; set; }
 
            /// <summary>
            /// NAME
            /// </summary>
            public String Name { get; set; }
 
            /// <summary>
            /// TYPE
            /// </summary>
            public Int32 Type { get; set; }
 
            /// <summary>
            /// PARENT_ID
            /// </summary>
            public String ParentId { get; set; }
 
            /// <summary>
            /// STANDARD_ORGANIZATION_ID
            /// </summary>
            public String StandardOrganizationId { get; set; }
 
            /// <summary>
            /// ORDER_ID
            /// </summary>
            public String OrderId { get; set; }
 
            /// <summary>
            /// STATUS
            /// </summary>
            public Int32 Status { get; set; }
 
            /// <summary>
            /// REMARK
            /// </summary>
            public String Remark { get; set; }
 
            /// <summary>
            /// UPDATE_DATE
            /// </summary>
            public MyDate UpdateDate { get; set; }
 
            /// <summary>
            /// POWER
            /// </summary>
            public String Power { get; set; }

            public bool SetPropertyValue(string PropertyName, object Value)
            {
                if ( PropertyName == "StandardRoleId" ) { this.StandardRoleId = ValueProc.As<Guid>(Value) ; return true; }
                if ( PropertyName == "Code" ) { this.Code = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "Name" ) { this.Name = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "Type" ) { this.Type = ValueProc.As<Int32>(Value) ; return true; }
                if ( PropertyName == "ParentId" ) { this.ParentId = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "StandardOrganizationId" ) { this.StandardOrganizationId = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "OrderId" ) { this.OrderId = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "Status" ) { this.Status = ValueProc.As<Int32>(Value) ; return true; }
                if ( PropertyName == "Remark" ) { this.Remark = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "UpdateDate" ) { this.UpdateDate = ValueProc.As<MyDate>(Value) ; return true; }
                if ( PropertyName == "Power" ) { this.Power = ValueProc.As<String>(Value) ; return true; }
                return false ;
           }

            public object GetPropertyValue(string PropertyName)
            {
                if ( PropertyName == "StandardRoleId" ) { return this.StandardRoleId ; }
                if ( PropertyName == "Code" ) { return this.Code ; }
                if ( PropertyName == "Name" ) { return this.Name ; }
                if ( PropertyName == "Type" ) { return this.Type ; }
                if ( PropertyName == "ParentId" ) { return this.ParentId ; }
                if ( PropertyName == "StandardOrganizationId" ) { return this.StandardOrganizationId ; }
                if ( PropertyName == "OrderId" ) { return this.OrderId ; }
                if ( PropertyName == "Status" ) { return this.Status ; }
                if ( PropertyName == "Remark" ) { return this.Remark ; }
                if ( PropertyName == "UpdateDate" ) { return this.UpdateDate ; }
                if ( PropertyName == "Power" ) { return this.Power ; }
                return null ;
            }

            public string[]  GetProperties() { return new string[]{ "StandardRoleId","Code","Name","Type","ParentId","StandardOrganizationId","OrderId","Status","Remark","UpdateDate","Power" } ; }
            
            public object Clone()
            {
                return this.CloneIEntity() ;
            }

         }
    }


    
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public sealed partial class TRoomOwnerRule : RuleBase, ITableRule,ICloneable
    {


        public  TRoomOwnerRule() : base("TRoomOwner")
        {
            this.RoomOwnerId = new SimpleColumn(this, DbType.AnsiString, 36,"RoomOwnerId","ROOM_OWNER_ID",false);
            this.LoginName = new SimpleColumn(this, DbType.AnsiString, 50,"LoginName","LoginName",true);
            this.Password = new SimpleColumn(this, DbType.AnsiString, 250,"Password","Password",true);
            this.OwnerOrLessee = new SimpleColumn(this, DbType.Int32, 4,"OwnerOrLessee","OWNER_OR_LESSEE",true);
            this.OwnerName = new SimpleColumn(this, DbType.AnsiString, 100,"OwnerName","OWNER_NAME",true);
            this.LinkPhone = new SimpleColumn(this, DbType.AnsiString, 100,"LinkPhone","LINK_PHONE",true);
            this.LinkMobile = new SimpleColumn(this, DbType.AnsiString, 100,"LinkMobile","LINK_MOBILE",true);
            this.OwnerSex = new SimpleColumn(this, DbType.AnsiString, 10,"OwnerSex","OWNER_SEX",true);
            this.NeedVisit = new SimpleColumn(this, DbType.SByte, 1,"NeedVisit","NEED_VISIT",true);
            this.SourceFlag = new SimpleColumn(this, DbType.SByte, 1,"SourceFlag","SOURCE_FLAG",true);
            this.OwnerMemo = new SimpleColumn(this, DbType.AnsiString, 600,"OwnerMemo","OWNER_MEMO",true);
            this.UpdateMan = new SimpleColumn(this, DbType.AnsiString, 36,"UpdateMan","UPDATE_MAN",true);
            this.UpdateTime = new SimpleColumn(this, DbType.DateTime, 8,"UpdateTime","UPDATE_TIME",true);
            this.SourceId = new SimpleColumn(this, DbType.AnsiString, 36,"SourceId","SOURCE_ID",true);
            this.CarInformation1 = new SimpleColumn(this, DbType.String, 100,"CarInformation1","CAR_INFORMATION1",true);
            this.CarInformation2 = new SimpleColumn(this, DbType.String, 100,"CarInformation2","CAR_INFORMATION2",true);
            this.CarInformation3 = new SimpleColumn(this, DbType.String, 100,"CarInformation3","CAR_INFORMATION3",true);
            this.NewRoomOwner = new SimpleColumn(this, DbType.AnsiString, 36,"NewRoomOwner","New_Room_Owner",true);
            this.IdNumber = new SimpleColumn(this, DbType.AnsiString, 30,"IdNumber","ID_NUMBER",true);
            this.Job = new SimpleColumn(this, DbType.AnsiString, 500,"Job","JOB",true);
            this.Interest = new SimpleColumn(this, DbType.AnsiString, 500,"Interest","INTEREST",true);
            this.Postcode = new SimpleColumn(this, DbType.AnsiString, 10,"Postcode","POSTCODE",true);
            this.DeliveryWay = new SimpleColumn(this, DbType.AnsiString, 50,"DeliveryWay","DELIVERY_WAY",true);
            this.Email = new SimpleColumn(this, DbType.AnsiString, 60,"Email","EMAIL",true);
            this.MailAddress = new SimpleColumn(this, DbType.AnsiString, 200,"MailAddress","MAIL_ADDRESS",true);
            this.WorkUnit = new SimpleColumn(this, DbType.AnsiString, 200,"WorkUnit","WORK_UNIT",true);
            this.ReturnVisitDemand = new SimpleColumn(this, DbType.AnsiString, 400,"ReturnVisitDemand","RETURN_VISIT_DEMAND",true);
            this.PassSign = new SimpleColumn(this, DbType.String, 40,"PassSign","PassSign",true);
            this.PaperName = new SimpleColumn(this, DbType.String, 40,"PaperName","PaperName",true);
            this.CustTypeID = new SimpleColumn(this, DbType.Int64, 8,"CustTypeID","CustTypeID",true);
            this.FaxTel = new SimpleColumn(this, DbType.String, 1000,"FaxTel","FaxTel",true);
            this.Recipient = new SimpleColumn(this, DbType.String, 200,"Recipient","Recipient",true);
            this.Linkman = new SimpleColumn(this, DbType.String, 40,"Linkman","Linkman",true);
            this.LinkmanTel = new SimpleColumn(this, DbType.String, 40,"LinkmanTel","LinkmanTel",true);
            this.InquireAccount = new SimpleColumn(this, DbType.String, 40,"InquireAccount","InquireAccount",true);
            this.InquirePwd = new SimpleColumn(this, DbType.String, 40,"InquirePwd","InquirePwd",true);
            this.IsUnit = new SimpleColumn(this, DbType.Int16, 2,"IsUnit","IsUnit",true);
            this.Surname = new SimpleColumn(this, DbType.String, 20,"Surname","Surname",true);
            this.Name = new SimpleColumn(this, DbType.String, 40,"Name","Name",true);
            this.Sex = new SimpleColumn(this, DbType.String, 20,"Sex","Sex",true);
            this.Birthday = new SimpleColumn(this, DbType.DateTime, 8,"Birthday","Birthday",true);
            this.Nationality = new SimpleColumn(this, DbType.String, 60,"Nationality","Nationality",true);
            this.LegalRepr = new SimpleColumn(this, DbType.String, 40,"LegalRepr","LegalRepr",true);
            this.LegalReprTel = new SimpleColumn(this, DbType.String, 40,"LegalReprTel","LegalReprTel",true);
            this.Charge = new SimpleColumn(this, DbType.String, 40,"Charge","Charge",true);
            this.ChargeTel = new SimpleColumn(this, DbType.String, 40,"ChargeTel","ChargeTel",true);
        }

        /// <summary>
        /// ROOM_OWNER_ID(AnsiString)[主键(ROOM_OWNER_ID)]
        /// </summary>
        public SimpleColumn RoomOwnerId { get; set; }
        /// <summary>
        /// LoginName(AnsiString)[唯一键(LoginName)]
        /// </summary>
        public SimpleColumn LoginName { get; set; }
        /// <summary>
        /// 程序加密存储(AnsiString)
        /// </summary>
        public SimpleColumn Password { get; set; }
        /// <summary>
        /// 1：业主；2：家庭成员；3：租户(Int32)
        /// </summary>
        public SimpleColumn OwnerOrLessee { get; set; }
        /// <summary>
        /// 姓名(AnsiString)
        /// </summary>
        public SimpleColumn OwnerName { get; set; }
        /// <summary>
        /// 联系电话(AnsiString)
        /// </summary>
        public SimpleColumn LinkPhone { get; set; }
        /// <summary>
        /// 手机(AnsiString)
        /// </summary>
        public SimpleColumn LinkMobile { get; set; }
        /// <summary>
        /// 性别(男/女)(AnsiString)
        /// </summary>
        public SimpleColumn OwnerSex { get; set; }
        /// <summary>
        /// 是否需要回访(0:否；1:是)(SByte)
        /// </summary>
        public SimpleColumn NeedVisit { get; set; }
        /// <summary>
        /// 来源状态(0:自添加；1:CRM导入)(SByte)
        /// </summary>
        public SimpleColumn SourceFlag { get; set; }
        /// <summary>
        /// 备注(AnsiString)
        /// </summary>
        public SimpleColumn OwnerMemo { get; set; }
        /// <summary>
        /// 更新人(AnsiString)
        /// </summary>
        public SimpleColumn UpdateMan { get; set; }
        /// <summary>
        /// 更新时间(DateTime)
        /// </summary>
        public SimpleColumn UpdateTime { get; set; }
        /// <summary>
        /// SOURCE_ID(AnsiString)
        /// </summary>
        public SimpleColumn SourceId { get; set; }
        /// <summary>
        /// CAR_INFORMATION1(String)
        /// </summary>
        public SimpleColumn CarInformation1 { get; set; }
        /// <summary>
        /// CAR_INFORMATION2(String)
        /// </summary>
        public SimpleColumn CarInformation2 { get; set; }
        /// <summary>
        /// CAR_INFORMATION3(String)
        /// </summary>
        public SimpleColumn CarInformation3 { get; set; }
        /// <summary>
        /// New_Room_Owner(AnsiString)
        /// </summary>
        public SimpleColumn NewRoomOwner { get; set; }
        /// <summary>
        /// ID_NUMBER(AnsiString)
        /// </summary>
        public SimpleColumn IdNumber { get; set; }
        /// <summary>
        /// JOB(AnsiString)
        /// </summary>
        public SimpleColumn Job { get; set; }
        /// <summary>
        /// INTEREST(AnsiString)
        /// </summary>
        public SimpleColumn Interest { get; set; }
        /// <summary>
        /// POSTCODE(AnsiString)
        /// </summary>
        public SimpleColumn Postcode { get; set; }
        /// <summary>
        /// DELIVERY_WAY(AnsiString)
        /// </summary>
        public SimpleColumn DeliveryWay { get; set; }
        /// <summary>
        /// EMAIL(AnsiString)
        /// </summary>
        public SimpleColumn Email { get; set; }
        /// <summary>
        /// MAIL_ADDRESS(AnsiString)
        /// </summary>
        public SimpleColumn MailAddress { get; set; }
        /// <summary>
        /// WORK_UNIT(AnsiString)
        /// </summary>
        public SimpleColumn WorkUnit { get; set; }
        /// <summary>
        /// RETURN_VISIT_DEMAND(AnsiString)
        /// </summary>
        public SimpleColumn ReturnVisitDemand { get; set; }
        /// <summary>
        /// PassSign(String)
        /// </summary>
        public SimpleColumn PassSign { get; set; }
        /// <summary>
        /// PaperName(String)
        /// </summary>
        public SimpleColumn PaperName { get; set; }
        /// <summary>
        /// CustTypeID(Int64)
        /// </summary>
        public SimpleColumn CustTypeID { get; set; }
        /// <summary>
        /// FaxTel(String)
        /// </summary>
        public SimpleColumn FaxTel { get; set; }
        /// <summary>
        /// Recipient(String)
        /// </summary>
        public SimpleColumn Recipient { get; set; }
        /// <summary>
        /// Linkman(String)
        /// </summary>
        public SimpleColumn Linkman { get; set; }
        /// <summary>
        /// LinkmanTel(String)
        /// </summary>
        public SimpleColumn LinkmanTel { get; set; }
        /// <summary>
        /// InquireAccount(String)
        /// </summary>
        public SimpleColumn InquireAccount { get; set; }
        /// <summary>
        /// InquirePwd(String)
        /// </summary>
        public SimpleColumn InquirePwd { get; set; }
        /// <summary>
        /// IsUnit(Int16)
        /// </summary>
        public SimpleColumn IsUnit { get; set; }
        /// <summary>
        /// Surname(String)
        /// </summary>
        public SimpleColumn Surname { get; set; }
        /// <summary>
        /// Name(String)
        /// </summary>
        public SimpleColumn Name { get; set; }
        /// <summary>
        /// Sex(String)
        /// </summary>
        public SimpleColumn Sex { get; set; }
        /// <summary>
        /// Birthday(DateTime)
        /// </summary>
        public SimpleColumn Birthday { get; set; }
        /// <summary>
        /// Nationality(String)
        /// </summary>
        public SimpleColumn Nationality { get; set; }
        /// <summary>
        /// LegalRepr(String)
        /// </summary>
        public SimpleColumn LegalRepr { get; set; }
        /// <summary>
        /// LegalReprTel(String)
        /// </summary>
        public SimpleColumn LegalReprTel { get; set; }
        /// <summary>
        /// Charge(String)
        /// </summary>
        public SimpleColumn Charge { get; set; }
        /// <summary>
        /// ChargeTel(String)
        /// </summary>
        public SimpleColumn ChargeTel { get; set; }

        public override SimpleColumn[] GetColumns() {  return new SimpleColumn[] { RoomOwnerId,LoginName,Password,OwnerOrLessee,OwnerName,LinkPhone,LinkMobile,OwnerSex,NeedVisit,SourceFlag,OwnerMemo,UpdateMan,UpdateTime,SourceId,CarInformation1,CarInformation2,CarInformation3,NewRoomOwner,IdNumber,Job,Interest,Postcode,DeliveryWay,Email,MailAddress,WorkUnit,ReturnVisitDemand,PassSign,PaperName,CustTypeID,FaxTel,Recipient,Linkman,LinkmanTel,InquireAccount,InquirePwd,IsUnit,Surname,Name,Sex,Birthday,Nationality,LegalRepr,LegalReprTel,Charge,ChargeTel }; }
        public override SimpleColumn[] GetPrimaryKeys() { return new SimpleColumn[] { RoomOwnerId };  }
        public override SimpleColumn[] GetComputeKeys() { return new SimpleColumn[] {  }; }
        public override SimpleColumn GetAutoIncreKey() {  return null; }
        public override SimpleColumn GetUniqueKey() { return  LoginName; }
        public override string GetDbName() { return "T_ROOM_OWNER"; }

        public Entity FindByRoomOwnerId(String RoomOwnerId)
        {
            if ( RoomOwnerId.HasValue() == false ) return null ;
            return this.SelectWhere(o => o.RoomOwnerId == RoomOwnerId).ToEntity<Entity>();
        }
        public int DeleteByRoomOwnerId(String RoomOwnerId)
        {
            if ( RoomOwnerId.HasValue() == false ) return 0 ;
            return this.Delete(o => o.RoomOwnerId == RoomOwnerId).Execute() ;
        }

        public Entity FindByLoginName(String LoginName)
        {
             if ( LoginName.HasValue() == false ) return null ;
            return this.SelectWhere(o => o.LoginName == LoginName).ToEntity<Entity>();
        }
        public int DeleteByLoginName(String LoginName)
        {
             if ( LoginName.HasValue() == false ) return 0 ;
            return this.Delete(o => o.LoginName == LoginName).Execute();
        }

        public override object Clone()
        {
            var tab = new TRoomOwnerRule();
            if ( this._Config_ != null ) tab._Config_ = base._Config_.Clone() as RuleRuntimeConfig ;
            tab.SetAlias(base.Name);
            tab.SetReconfig(base.ReConfig);

            tab.RoomOwnerId = this.RoomOwnerId.Clone() as SimpleColumn;
            tab.LoginName = this.LoginName.Clone() as SimpleColumn;
            tab.Password = this.Password.Clone() as SimpleColumn;
            tab.OwnerOrLessee = this.OwnerOrLessee.Clone() as SimpleColumn;
            tab.OwnerName = this.OwnerName.Clone() as SimpleColumn;
            tab.LinkPhone = this.LinkPhone.Clone() as SimpleColumn;
            tab.LinkMobile = this.LinkMobile.Clone() as SimpleColumn;
            tab.OwnerSex = this.OwnerSex.Clone() as SimpleColumn;
            tab.NeedVisit = this.NeedVisit.Clone() as SimpleColumn;
            tab.SourceFlag = this.SourceFlag.Clone() as SimpleColumn;
            tab.OwnerMemo = this.OwnerMemo.Clone() as SimpleColumn;
            tab.UpdateMan = this.UpdateMan.Clone() as SimpleColumn;
            tab.UpdateTime = this.UpdateTime.Clone() as SimpleColumn;
            tab.SourceId = this.SourceId.Clone() as SimpleColumn;
            tab.CarInformation1 = this.CarInformation1.Clone() as SimpleColumn;
            tab.CarInformation2 = this.CarInformation2.Clone() as SimpleColumn;
            tab.CarInformation3 = this.CarInformation3.Clone() as SimpleColumn;
            tab.NewRoomOwner = this.NewRoomOwner.Clone() as SimpleColumn;
            tab.IdNumber = this.IdNumber.Clone() as SimpleColumn;
            tab.Job = this.Job.Clone() as SimpleColumn;
            tab.Interest = this.Interest.Clone() as SimpleColumn;
            tab.Postcode = this.Postcode.Clone() as SimpleColumn;
            tab.DeliveryWay = this.DeliveryWay.Clone() as SimpleColumn;
            tab.Email = this.Email.Clone() as SimpleColumn;
            tab.MailAddress = this.MailAddress.Clone() as SimpleColumn;
            tab.WorkUnit = this.WorkUnit.Clone() as SimpleColumn;
            tab.ReturnVisitDemand = this.ReturnVisitDemand.Clone() as SimpleColumn;
            tab.PassSign = this.PassSign.Clone() as SimpleColumn;
            tab.PaperName = this.PaperName.Clone() as SimpleColumn;
            tab.CustTypeID = this.CustTypeID.Clone() as SimpleColumn;
            tab.FaxTel = this.FaxTel.Clone() as SimpleColumn;
            tab.Recipient = this.Recipient.Clone() as SimpleColumn;
            tab.Linkman = this.Linkman.Clone() as SimpleColumn;
            tab.LinkmanTel = this.LinkmanTel.Clone() as SimpleColumn;
            tab.InquireAccount = this.InquireAccount.Clone() as SimpleColumn;
            tab.InquirePwd = this.InquirePwd.Clone() as SimpleColumn;
            tab.IsUnit = this.IsUnit.Clone() as SimpleColumn;
            tab.Surname = this.Surname.Clone() as SimpleColumn;
            tab.Name = this.Name.Clone() as SimpleColumn;
            tab.Sex = this.Sex.Clone() as SimpleColumn;
            tab.Birthday = this.Birthday.Clone() as SimpleColumn;
            tab.Nationality = this.Nationality.Clone() as SimpleColumn;
            tab.LegalRepr = this.LegalRepr.Clone() as SimpleColumn;
            tab.LegalReprTel = this.LegalReprTel.Clone() as SimpleColumn;
            tab.Charge = this.Charge.Clone() as SimpleColumn;
            tab.ChargeTel = this.ChargeTel.Clone() as SimpleColumn;

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
            /// ROOM_OWNER_ID[主键(ROOM_OWNER_ID)]
            /// </summary>
            public String RoomOwnerId { get; set; }
 
            /// <summary>
            /// LoginName[唯一键(LoginName)]
            /// </summary>
            public String LoginName { get; set; }
 
            /// <summary>
            /// 程序加密存储
            /// </summary>
            public String Password { get; set; }
 
            /// <summary>
            /// 1：业主；2：家庭成员；3：租户
            /// </summary>
            public Int32 OwnerOrLessee { get; set; }
 
            /// <summary>
            /// 姓名
            /// </summary>
            public String OwnerName { get; set; }
 
            /// <summary>
            /// 联系电话
            /// </summary>
            public String LinkPhone { get; set; }
 
            /// <summary>
            /// 手机
            /// </summary>
            public String LinkMobile { get; set; }
 
            /// <summary>
            /// 性别(男/女)
            /// </summary>
            public String OwnerSex { get; set; }
 
            /// <summary>
            /// 是否需要回访(0:否；1:是)
            /// </summary>
            public SByte NeedVisit { get; set; }
 
            /// <summary>
            /// 来源状态(0:自添加；1:CRM导入)
            /// </summary>
            public SByte SourceFlag { get; set; }
 
            /// <summary>
            /// 备注
            /// </summary>
            public String OwnerMemo { get; set; }
 
            /// <summary>
            /// 更新人
            /// </summary>
            public String UpdateMan { get; set; }
 
            /// <summary>
            /// 更新时间
            /// </summary>
            public MyDate UpdateTime { get; set; }
 
            /// <summary>
            /// SOURCE_ID
            /// </summary>
            public String SourceId { get; set; }
 
            /// <summary>
            /// CAR_INFORMATION1
            /// </summary>
            public String CarInformation1 { get; set; }
 
            /// <summary>
            /// CAR_INFORMATION2
            /// </summary>
            public String CarInformation2 { get; set; }
 
            /// <summary>
            /// CAR_INFORMATION3
            /// </summary>
            public String CarInformation3 { get; set; }
 
            /// <summary>
            /// New_Room_Owner
            /// </summary>
            public String NewRoomOwner { get; set; }
 
            /// <summary>
            /// ID_NUMBER
            /// </summary>
            public String IdNumber { get; set; }
 
            /// <summary>
            /// JOB
            /// </summary>
            public String Job { get; set; }
 
            /// <summary>
            /// INTEREST
            /// </summary>
            public String Interest { get; set; }
 
            /// <summary>
            /// POSTCODE
            /// </summary>
            public String Postcode { get; set; }
 
            /// <summary>
            /// DELIVERY_WAY
            /// </summary>
            public String DeliveryWay { get; set; }
 
            /// <summary>
            /// EMAIL
            /// </summary>
            public String Email { get; set; }
 
            /// <summary>
            /// MAIL_ADDRESS
            /// </summary>
            public String MailAddress { get; set; }
 
            /// <summary>
            /// WORK_UNIT
            /// </summary>
            public String WorkUnit { get; set; }
 
            /// <summary>
            /// RETURN_VISIT_DEMAND
            /// </summary>
            public String ReturnVisitDemand { get; set; }
 
            /// <summary>
            /// PassSign
            /// </summary>
            public String PassSign { get; set; }
 
            /// <summary>
            /// PaperName
            /// </summary>
            public String PaperName { get; set; }
 
            /// <summary>
            /// CustTypeID
            /// </summary>
            public Int64 CustTypeID { get; set; }
 
            /// <summary>
            /// FaxTel
            /// </summary>
            public String FaxTel { get; set; }
 
            /// <summary>
            /// Recipient
            /// </summary>
            public String Recipient { get; set; }
 
            /// <summary>
            /// Linkman
            /// </summary>
            public String Linkman { get; set; }
 
            /// <summary>
            /// LinkmanTel
            /// </summary>
            public String LinkmanTel { get; set; }
 
            /// <summary>
            /// InquireAccount
            /// </summary>
            public String InquireAccount { get; set; }
 
            /// <summary>
            /// InquirePwd
            /// </summary>
            public String InquirePwd { get; set; }
 
            /// <summary>
            /// IsUnit
            /// </summary>
            public Int16 IsUnit { get; set; }
 
            /// <summary>
            /// Surname
            /// </summary>
            public String Surname { get; set; }
 
            /// <summary>
            /// Name
            /// </summary>
            public String Name { get; set; }
 
            /// <summary>
            /// Sex
            /// </summary>
            public String Sex { get; set; }
 
            /// <summary>
            /// Birthday
            /// </summary>
            public MyDate Birthday { get; set; }
 
            /// <summary>
            /// Nationality
            /// </summary>
            public String Nationality { get; set; }
 
            /// <summary>
            /// LegalRepr
            /// </summary>
            public String LegalRepr { get; set; }
 
            /// <summary>
            /// LegalReprTel
            /// </summary>
            public String LegalReprTel { get; set; }
 
            /// <summary>
            /// Charge
            /// </summary>
            public String Charge { get; set; }
 
            /// <summary>
            /// ChargeTel
            /// </summary>
            public String ChargeTel { get; set; }

            public bool SetPropertyValue(string PropertyName, object Value)
            {
                if ( PropertyName == "RoomOwnerId" ) { this.RoomOwnerId = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "LoginName" ) { this.LoginName = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "Password" ) { this.Password = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "OwnerOrLessee" ) { this.OwnerOrLessee = ValueProc.As<Int32>(Value) ; return true; }
                if ( PropertyName == "OwnerName" ) { this.OwnerName = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "LinkPhone" ) { this.LinkPhone = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "LinkMobile" ) { this.LinkMobile = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "OwnerSex" ) { this.OwnerSex = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "NeedVisit" ) { this.NeedVisit = ValueProc.As<SByte>(Value) ; return true; }
                if ( PropertyName == "SourceFlag" ) { this.SourceFlag = ValueProc.As<SByte>(Value) ; return true; }
                if ( PropertyName == "OwnerMemo" ) { this.OwnerMemo = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "UpdateMan" ) { this.UpdateMan = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "UpdateTime" ) { this.UpdateTime = ValueProc.As<MyDate>(Value) ; return true; }
                if ( PropertyName == "SourceId" ) { this.SourceId = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "CarInformation1" ) { this.CarInformation1 = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "CarInformation2" ) { this.CarInformation2 = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "CarInformation3" ) { this.CarInformation3 = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "NewRoomOwner" ) { this.NewRoomOwner = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "IdNumber" ) { this.IdNumber = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "Job" ) { this.Job = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "Interest" ) { this.Interest = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "Postcode" ) { this.Postcode = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "DeliveryWay" ) { this.DeliveryWay = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "Email" ) { this.Email = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "MailAddress" ) { this.MailAddress = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "WorkUnit" ) { this.WorkUnit = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "ReturnVisitDemand" ) { this.ReturnVisitDemand = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "PassSign" ) { this.PassSign = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "PaperName" ) { this.PaperName = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "CustTypeID" ) { this.CustTypeID = ValueProc.As<Int64>(Value) ; return true; }
                if ( PropertyName == "FaxTel" ) { this.FaxTel = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "Recipient" ) { this.Recipient = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "Linkman" ) { this.Linkman = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "LinkmanTel" ) { this.LinkmanTel = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "InquireAccount" ) { this.InquireAccount = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "InquirePwd" ) { this.InquirePwd = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "IsUnit" ) { this.IsUnit = ValueProc.As<Int16>(Value) ; return true; }
                if ( PropertyName == "Surname" ) { this.Surname = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "Name" ) { this.Name = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "Sex" ) { this.Sex = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "Birthday" ) { this.Birthday = ValueProc.As<MyDate>(Value) ; return true; }
                if ( PropertyName == "Nationality" ) { this.Nationality = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "LegalRepr" ) { this.LegalRepr = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "LegalReprTel" ) { this.LegalReprTel = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "Charge" ) { this.Charge = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "ChargeTel" ) { this.ChargeTel = ValueProc.As<String>(Value) ; return true; }
                return false ;
           }

            public object GetPropertyValue(string PropertyName)
            {
                if ( PropertyName == "RoomOwnerId" ) { return this.RoomOwnerId ; }
                if ( PropertyName == "LoginName" ) { return this.LoginName ; }
                if ( PropertyName == "Password" ) { return this.Password ; }
                if ( PropertyName == "OwnerOrLessee" ) { return this.OwnerOrLessee ; }
                if ( PropertyName == "OwnerName" ) { return this.OwnerName ; }
                if ( PropertyName == "LinkPhone" ) { return this.LinkPhone ; }
                if ( PropertyName == "LinkMobile" ) { return this.LinkMobile ; }
                if ( PropertyName == "OwnerSex" ) { return this.OwnerSex ; }
                if ( PropertyName == "NeedVisit" ) { return this.NeedVisit ; }
                if ( PropertyName == "SourceFlag" ) { return this.SourceFlag ; }
                if ( PropertyName == "OwnerMemo" ) { return this.OwnerMemo ; }
                if ( PropertyName == "UpdateMan" ) { return this.UpdateMan ; }
                if ( PropertyName == "UpdateTime" ) { return this.UpdateTime ; }
                if ( PropertyName == "SourceId" ) { return this.SourceId ; }
                if ( PropertyName == "CarInformation1" ) { return this.CarInformation1 ; }
                if ( PropertyName == "CarInformation2" ) { return this.CarInformation2 ; }
                if ( PropertyName == "CarInformation3" ) { return this.CarInformation3 ; }
                if ( PropertyName == "NewRoomOwner" ) { return this.NewRoomOwner ; }
                if ( PropertyName == "IdNumber" ) { return this.IdNumber ; }
                if ( PropertyName == "Job" ) { return this.Job ; }
                if ( PropertyName == "Interest" ) { return this.Interest ; }
                if ( PropertyName == "Postcode" ) { return this.Postcode ; }
                if ( PropertyName == "DeliveryWay" ) { return this.DeliveryWay ; }
                if ( PropertyName == "Email" ) { return this.Email ; }
                if ( PropertyName == "MailAddress" ) { return this.MailAddress ; }
                if ( PropertyName == "WorkUnit" ) { return this.WorkUnit ; }
                if ( PropertyName == "ReturnVisitDemand" ) { return this.ReturnVisitDemand ; }
                if ( PropertyName == "PassSign" ) { return this.PassSign ; }
                if ( PropertyName == "PaperName" ) { return this.PaperName ; }
                if ( PropertyName == "CustTypeID" ) { return this.CustTypeID ; }
                if ( PropertyName == "FaxTel" ) { return this.FaxTel ; }
                if ( PropertyName == "Recipient" ) { return this.Recipient ; }
                if ( PropertyName == "Linkman" ) { return this.Linkman ; }
                if ( PropertyName == "LinkmanTel" ) { return this.LinkmanTel ; }
                if ( PropertyName == "InquireAccount" ) { return this.InquireAccount ; }
                if ( PropertyName == "InquirePwd" ) { return this.InquirePwd ; }
                if ( PropertyName == "IsUnit" ) { return this.IsUnit ; }
                if ( PropertyName == "Surname" ) { return this.Surname ; }
                if ( PropertyName == "Name" ) { return this.Name ; }
                if ( PropertyName == "Sex" ) { return this.Sex ; }
                if ( PropertyName == "Birthday" ) { return this.Birthday ; }
                if ( PropertyName == "Nationality" ) { return this.Nationality ; }
                if ( PropertyName == "LegalRepr" ) { return this.LegalRepr ; }
                if ( PropertyName == "LegalReprTel" ) { return this.LegalReprTel ; }
                if ( PropertyName == "Charge" ) { return this.Charge ; }
                if ( PropertyName == "ChargeTel" ) { return this.ChargeTel ; }
                return null ;
            }

            public string[]  GetProperties() { return new string[]{ "RoomOwnerId","LoginName","Password","OwnerOrLessee","OwnerName","LinkPhone","LinkMobile","OwnerSex","NeedVisit","SourceFlag","OwnerMemo","UpdateMan","UpdateTime","SourceId","CarInformation1","CarInformation2","CarInformation3","NewRoomOwner","IdNumber","Job","Interest","Postcode","DeliveryWay","Email","MailAddress","WorkUnit","ReturnVisitDemand","PassSign","PaperName","CustTypeID","FaxTel","Recipient","Linkman","LinkmanTel","InquireAccount","InquirePwd","IsUnit","Surname","Name","Sex","Birthday","Nationality","LegalRepr","LegalReprTel","Charge","ChargeTel" } ; }
            
            public object Clone()
            {
                return this.CloneIEntity() ;
            }

         }
    }


    
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public sealed partial class TempRoomownerRule : RuleBase, ITableRule,ICloneable
    {


        public  TempRoomownerRule() : base("TempRoomowner")
        {
            this.Id = new SimpleColumn(this, DbType.Int64, 8,"Id","Id",false);
            this.CommId = new SimpleColumn(this, DbType.Int64, 8,"CommId","CommId",true);
            this.BuildingName = new SimpleColumn(this, DbType.String, 40,"BuildingName","BuildingName",true);
            this.FloorNum = new SimpleColumn(this, DbType.SByte, 1,"FloorNum","FloorNum",true);
            this.UnitNum = new SimpleColumn(this, DbType.SByte, 1,"UnitNum","UnitNum",true);
            this.RoomCode = new SimpleColumn(this, DbType.String, 60,"RoomCode","RoomCode",true);
            this.ScBldArea = new SimpleColumn(this, DbType.Decimal, 18,"ScBldArea","ScBldArea",true);
            this.ScTnArea = new SimpleColumn(this, DbType.Decimal, 18,"ScTnArea","ScTnArea",true);
            this.PropertyTakeoverDate = new SimpleColumn(this, DbType.DateTime, 8,"PropertyTakeoverDate","PropertyTakeoverDate",true);
            this.LoginName = new SimpleColumn(this, DbType.String, 40,"LoginName","LoginName",true);
            this.OwnerName = new SimpleColumn(this, DbType.String, 40,"OwnerName","OwnerName",true);
            this.OwnerSex = new SimpleColumn(this, DbType.String, 20,"OwnerSex","OwnerSex",true);
            this.LinkMobile = new SimpleColumn(this, DbType.String, 40,"LinkMobile","LinkMobile",true);
            this.IdNumber = new SimpleColumn(this, DbType.String, 80,"IdNumber","IdNumber",true);
            this.Email = new SimpleColumn(this, DbType.String, 40,"Email","Email",true);
            this.MailAddress = new SimpleColumn(this, DbType.String, 80,"MailAddress","MailAddress",true);
        }

        /// <summary>
        /// Id(Int64)[主键(Id),自增键]
        /// </summary>
        public SimpleColumn Id { get; set; }
        /// <summary>
        /// CommId(Int64)
        /// </summary>
        public SimpleColumn CommId { get; set; }
        /// <summary>
        /// BuildingName(String)
        /// </summary>
        public SimpleColumn BuildingName { get; set; }
        /// <summary>
        /// FloorNum(SByte)
        /// </summary>
        public SimpleColumn FloorNum { get; set; }
        /// <summary>
        /// UnitNum(SByte)
        /// </summary>
        public SimpleColumn UnitNum { get; set; }
        /// <summary>
        /// RoomCode(String)
        /// </summary>
        public SimpleColumn RoomCode { get; set; }
        /// <summary>
        /// ScBldArea(Decimal)
        /// </summary>
        public SimpleColumn ScBldArea { get; set; }
        /// <summary>
        /// ScTnArea(Decimal)
        /// </summary>
        public SimpleColumn ScTnArea { get; set; }
        /// <summary>
        /// PropertyTakeoverDate(DateTime)
        /// </summary>
        public SimpleColumn PropertyTakeoverDate { get; set; }
        /// <summary>
        /// LoginName(String)
        /// </summary>
        public SimpleColumn LoginName { get; set; }
        /// <summary>
        /// OwnerName(String)
        /// </summary>
        public SimpleColumn OwnerName { get; set; }
        /// <summary>
        /// OwnerSex(String)
        /// </summary>
        public SimpleColumn OwnerSex { get; set; }
        /// <summary>
        /// LinkMobile(String)
        /// </summary>
        public SimpleColumn LinkMobile { get; set; }
        /// <summary>
        /// IdNumber(String)
        /// </summary>
        public SimpleColumn IdNumber { get; set; }
        /// <summary>
        /// Email(String)
        /// </summary>
        public SimpleColumn Email { get; set; }
        /// <summary>
        /// MailAddress(String)
        /// </summary>
        public SimpleColumn MailAddress { get; set; }

        public override SimpleColumn[] GetColumns() {  return new SimpleColumn[] { Id,CommId,BuildingName,FloorNum,UnitNum,RoomCode,ScBldArea,ScTnArea,PropertyTakeoverDate,LoginName,OwnerName,OwnerSex,LinkMobile,IdNumber,Email,MailAddress }; }
        public override SimpleColumn[] GetPrimaryKeys() { return new SimpleColumn[] { Id };  }
        public override SimpleColumn[] GetComputeKeys() { return new SimpleColumn[] {  }; }
        public override SimpleColumn GetAutoIncreKey() {  return Id; }
        public override SimpleColumn GetUniqueKey() { return  null; }
        public override string GetDbName() { return "TEMP_ROOMOWNER"; }

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
            var tab = new TempRoomownerRule();
            if ( this._Config_ != null ) tab._Config_ = base._Config_.Clone() as RuleRuntimeConfig ;
            tab.SetAlias(base.Name);
            tab.SetReconfig(base.ReConfig);

            tab.Id = this.Id.Clone() as SimpleColumn;
            tab.CommId = this.CommId.Clone() as SimpleColumn;
            tab.BuildingName = this.BuildingName.Clone() as SimpleColumn;
            tab.FloorNum = this.FloorNum.Clone() as SimpleColumn;
            tab.UnitNum = this.UnitNum.Clone() as SimpleColumn;
            tab.RoomCode = this.RoomCode.Clone() as SimpleColumn;
            tab.ScBldArea = this.ScBldArea.Clone() as SimpleColumn;
            tab.ScTnArea = this.ScTnArea.Clone() as SimpleColumn;
            tab.PropertyTakeoverDate = this.PropertyTakeoverDate.Clone() as SimpleColumn;
            tab.LoginName = this.LoginName.Clone() as SimpleColumn;
            tab.OwnerName = this.OwnerName.Clone() as SimpleColumn;
            tab.OwnerSex = this.OwnerSex.Clone() as SimpleColumn;
            tab.LinkMobile = this.LinkMobile.Clone() as SimpleColumn;
            tab.IdNumber = this.IdNumber.Clone() as SimpleColumn;
            tab.Email = this.Email.Clone() as SimpleColumn;
            tab.MailAddress = this.MailAddress.Clone() as SimpleColumn;

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
            /// Id[主键(Id),自增键]
            /// </summary>
            public Int64 Id { get; set; }
 
            /// <summary>
            /// CommId
            /// </summary>
            public Int64 CommId { get; set; }
 
            /// <summary>
            /// BuildingName
            /// </summary>
            public String BuildingName { get; set; }
 
            /// <summary>
            /// FloorNum
            /// </summary>
            public SByte FloorNum { get; set; }
 
            /// <summary>
            /// UnitNum
            /// </summary>
            public SByte UnitNum { get; set; }
 
            /// <summary>
            /// RoomCode
            /// </summary>
            public String RoomCode { get; set; }
 
            /// <summary>
            /// ScBldArea
            /// </summary>
            public Decimal ScBldArea { get; set; }
 
            /// <summary>
            /// ScTnArea
            /// </summary>
            public Decimal ScTnArea { get; set; }
 
            /// <summary>
            /// PropertyTakeoverDate
            /// </summary>
            public MyDate PropertyTakeoverDate { get; set; }
 
            /// <summary>
            /// LoginName
            /// </summary>
            public String LoginName { get; set; }
 
            /// <summary>
            /// OwnerName
            /// </summary>
            public String OwnerName { get; set; }
 
            /// <summary>
            /// OwnerSex
            /// </summary>
            public String OwnerSex { get; set; }
 
            /// <summary>
            /// LinkMobile
            /// </summary>
            public String LinkMobile { get; set; }
 
            /// <summary>
            /// IdNumber
            /// </summary>
            public String IdNumber { get; set; }
 
            /// <summary>
            /// Email
            /// </summary>
            public String Email { get; set; }
 
            /// <summary>
            /// MailAddress
            /// </summary>
            public String MailAddress { get; set; }

            public bool SetPropertyValue(string PropertyName, object Value)
            {
                if ( PropertyName == "Id" ) { this.Id = ValueProc.As<Int64>(Value) ; return true; }
                if ( PropertyName == "CommId" ) { this.CommId = ValueProc.As<Int64>(Value) ; return true; }
                if ( PropertyName == "BuildingName" ) { this.BuildingName = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "FloorNum" ) { this.FloorNum = ValueProc.As<SByte>(Value) ; return true; }
                if ( PropertyName == "UnitNum" ) { this.UnitNum = ValueProc.As<SByte>(Value) ; return true; }
                if ( PropertyName == "RoomCode" ) { this.RoomCode = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "ScBldArea" ) { this.ScBldArea = ValueProc.As<Decimal>(Value) ; return true; }
                if ( PropertyName == "ScTnArea" ) { this.ScTnArea = ValueProc.As<Decimal>(Value) ; return true; }
                if ( PropertyName == "PropertyTakeoverDate" ) { this.PropertyTakeoverDate = ValueProc.As<MyDate>(Value) ; return true; }
                if ( PropertyName == "LoginName" ) { this.LoginName = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "OwnerName" ) { this.OwnerName = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "OwnerSex" ) { this.OwnerSex = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "LinkMobile" ) { this.LinkMobile = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "IdNumber" ) { this.IdNumber = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "Email" ) { this.Email = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "MailAddress" ) { this.MailAddress = ValueProc.As<String>(Value) ; return true; }
                return false ;
           }

            public object GetPropertyValue(string PropertyName)
            {
                if ( PropertyName == "Id" ) { return this.Id ; }
                if ( PropertyName == "CommId" ) { return this.CommId ; }
                if ( PropertyName == "BuildingName" ) { return this.BuildingName ; }
                if ( PropertyName == "FloorNum" ) { return this.FloorNum ; }
                if ( PropertyName == "UnitNum" ) { return this.UnitNum ; }
                if ( PropertyName == "RoomCode" ) { return this.RoomCode ; }
                if ( PropertyName == "ScBldArea" ) { return this.ScBldArea ; }
                if ( PropertyName == "ScTnArea" ) { return this.ScTnArea ; }
                if ( PropertyName == "PropertyTakeoverDate" ) { return this.PropertyTakeoverDate ; }
                if ( PropertyName == "LoginName" ) { return this.LoginName ; }
                if ( PropertyName == "OwnerName" ) { return this.OwnerName ; }
                if ( PropertyName == "OwnerSex" ) { return this.OwnerSex ; }
                if ( PropertyName == "LinkMobile" ) { return this.LinkMobile ; }
                if ( PropertyName == "IdNumber" ) { return this.IdNumber ; }
                if ( PropertyName == "Email" ) { return this.Email ; }
                if ( PropertyName == "MailAddress" ) { return this.MailAddress ; }
                return null ;
            }

            public string[]  GetProperties() { return new string[]{ "Id","CommId","BuildingName","FloorNum","UnitNum","RoomCode","ScBldArea","ScTnArea","PropertyTakeoverDate","LoginName","OwnerName","OwnerSex","LinkMobile","IdNumber","Email","MailAddress" } ; }
            
            public object Clone()
            {
                return this.CloneIEntity() ;
            }

         }
    }


    
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public sealed partial class DeptCommunityRule : RuleBase, ITableRule,ICloneable
    {


        public  DeptCommunityRule() : base("DeptCommunity")
        {
            this.RefID = new SimpleColumn(this, DbType.Int64, 8,"RefID","RefID",false);
            this.DeptID = new SimpleColumn(this, DbType.Int32, 4,"DeptID","DeptID",true);
            this.CommID = new SimpleColumn(this, DbType.Int32, 4,"CommID","CommID",true);
            this.RefUser = new SimpleColumn(this, DbType.String, 40,"RefUser","RefUser",true);
            this.RefTime = new SimpleColumn(this, DbType.DateTime, 8,"RefTime","RefTime",true);
            this.IfPushComm = new SimpleColumn(this, DbType.SByte, 1,"IfPushComm","IfPushComm",true);
        }

        /// <summary>
        /// RefID(Int64)[主键(RefID),自增键]
        /// </summary>
        public SimpleColumn RefID { get; set; }
        /// <summary>
        /// 商户ID(Int32)
        /// </summary>
        public SimpleColumn DeptID { get; set; }
        /// <summary>
        /// 社区ID(Int32)
        /// </summary>
        public SimpleColumn CommID { get; set; }
        /// <summary>
        /// 关联操作人(String)
        /// </summary>
        public SimpleColumn RefUser { get; set; }
        /// <summary>
        /// 关联日期(DateTime)
        /// </summary>
        public SimpleColumn RefTime { get; set; }
        /// <summary>
        /// 是否推送社区(SByte)
        /// </summary>
        public SimpleColumn IfPushComm { get; set; }

        public override SimpleColumn[] GetColumns() {  return new SimpleColumn[] { RefID,DeptID,CommID,RefUser,RefTime,IfPushComm }; }
        public override SimpleColumn[] GetPrimaryKeys() { return new SimpleColumn[] { RefID };  }
        public override SimpleColumn[] GetComputeKeys() { return new SimpleColumn[] {  }; }
        public override SimpleColumn GetAutoIncreKey() {  return RefID; }
        public override SimpleColumn GetUniqueKey() { return  null; }
        public override string GetDbName() { return "TB_Dept_Community"; }

        public Entity FindByRefID(Int64 RefID)
        {
             if ( RefID <= 0 ) return null ;
            return this.SelectWhere(o => o.RefID == RefID).ToEntity<Entity>();
        }
        public int DeleteByRefID(Int64 RefID)
        {
             if ( RefID <= 0 ) return 0 ;
            return this.Delete(o => o.RefID == RefID).Execute() ;
        }


        public override object Clone()
        {
            var tab = new DeptCommunityRule();
            if ( this._Config_ != null ) tab._Config_ = base._Config_.Clone() as RuleRuntimeConfig ;
            tab.SetAlias(base.Name);
            tab.SetReconfig(base.ReConfig);

            tab.RefID = this.RefID.Clone() as SimpleColumn;
            tab.DeptID = this.DeptID.Clone() as SimpleColumn;
            tab.CommID = this.CommID.Clone() as SimpleColumn;
            tab.RefUser = this.RefUser.Clone() as SimpleColumn;
            tab.RefTime = this.RefTime.Clone() as SimpleColumn;
            tab.IfPushComm = this.IfPushComm.Clone() as SimpleColumn;

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
            /// RefID[主键(RefID),自增键]
            /// </summary>
            public Int64 RefID { get; set; }
 
            /// <summary>
            /// 商户ID
            /// </summary>
            public Int32 DeptID { get; set; }
 
            /// <summary>
            /// 社区ID
            /// </summary>
            public Int32 CommID { get; set; }
 
            /// <summary>
            /// 关联操作人
            /// </summary>
            public String RefUser { get; set; }
 
            /// <summary>
            /// 关联日期
            /// </summary>
            public MyDate RefTime { get; set; }
 
            /// <summary>
            /// 是否推送社区
            /// </summary>
            public SByte IfPushComm { get; set; }

            public bool SetPropertyValue(string PropertyName, object Value)
            {
                if ( PropertyName == "RefID" ) { this.RefID = ValueProc.As<Int64>(Value) ; return true; }
                if ( PropertyName == "DeptID" ) { this.DeptID = ValueProc.As<Int32>(Value) ; return true; }
                if ( PropertyName == "CommID" ) { this.CommID = ValueProc.As<Int32>(Value) ; return true; }
                if ( PropertyName == "RefUser" ) { this.RefUser = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "RefTime" ) { this.RefTime = ValueProc.As<MyDate>(Value) ; return true; }
                if ( PropertyName == "IfPushComm" ) { this.IfPushComm = ValueProc.As<SByte>(Value) ; return true; }
                return false ;
           }

            public object GetPropertyValue(string PropertyName)
            {
                if ( PropertyName == "RefID" ) { return this.RefID ; }
                if ( PropertyName == "DeptID" ) { return this.DeptID ; }
                if ( PropertyName == "CommID" ) { return this.CommID ; }
                if ( PropertyName == "RefUser" ) { return this.RefUser ; }
                if ( PropertyName == "RefTime" ) { return this.RefTime ; }
                if ( PropertyName == "IfPushComm" ) { return this.IfPushComm ; }
                return null ;
            }

            public string[]  GetProperties() { return new string[]{ "RefID","DeptID","CommID","RefUser","RefTime","IfPushComm" } ; }
            
            public object Clone()
            {
                return this.CloneIEntity() ;
            }

         }
    }



}