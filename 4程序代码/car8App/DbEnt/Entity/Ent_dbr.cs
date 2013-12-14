//系统自动生成的实体，不能修改。 By: UDI-PC.于新海  At:2013-12-09 19:32:05
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

///<summary>
///Database rule , 实体，视图，存储过程，函数的入口。
///</summary>
public partial class dbr:IDbr
{
    static dbr()
    {
        dbr.Shop = new ShopGroupClass(); 
        dbr.View = new ViewGroupClass(); 

        //MyOql 实体信息
        MyOql_FkDefinesList = new List<FkColumn>() ;
        MyOql_FkDefinesList.Add( new FkColumn(){ Entity = "DeptAnnex" , Column = "DeptID" , CascadeUpdate = true , CascadeDelete = true , RefTable = "Dept", RefColumn = "ID" } );
        MyOql_FkDefinesList.Add( new FkColumn(){ Entity = "DeptAnnex" , Column = "AnnexID" , CascadeUpdate = true , CascadeDelete = true , RefTable = "Annex", RefColumn = "ID" } );
        MyOql_FkDefinesList.Add( new FkColumn(){ Entity = "Person" , Column = "Logo" , CascadeUpdate = false , CascadeDelete = false , RefTable = "Annex", RefColumn = "ID" } );
        MyOql_FkDefinesList.Add( new FkColumn(){ Entity = "Person" , Column = "DeptID" , CascadeUpdate = false , CascadeDelete = false , RefTable = "Dept", RefColumn = "ID" } );
        MyOql_FkDefinesList.Add( new FkColumn(){ Entity = "Dept" , Column = "Logo" , CascadeUpdate = false , CascadeDelete = false , RefTable = "Annex", RefColumn = "ID" } );
        MyOql_FkDefinesList.Add( new FkColumn(){ Entity = "Dept" , Column = "Title" , CascadeUpdate = false , CascadeDelete = false , RefTable = "Annex", RefColumn = "ID" } );
        MyOql_FkDefinesList.Add( new FkColumn(){ Entity = "Dept" , Column = "TitleExtend" , CascadeUpdate = false , CascadeDelete = false , RefTable = "Annex", RefColumn = "ID" } );
        MyOql_FkDefinesList.Add( new FkColumn(){ Entity = "PowerAction" , Column = "ControllerID" , CascadeUpdate = false , CascadeDelete = false , RefTable = "PowerController", RefColumn = "ID" } );
        MyOql_FkDefinesList.Add( new FkColumn(){ Entity = "PowerButton" , Column = "ActionID" , CascadeUpdate = true , CascadeDelete = true , RefTable = "PowerAction", RefColumn = "ID" } );
        MyOql_FkDefinesList.Add( new FkColumn(){ Entity = "Community" , Column = "ZoneId" , CascadeUpdate = false , CascadeDelete = false , RefTable = "Zone", RefColumn = "Id" } );
        MyOql_FkDefinesList.Add( new FkColumn(){ Entity = "Dept_Community" , Column = "DeptId" , CascadeUpdate = false , CascadeDelete = false , RefTable = "Dept", RefColumn = "ID" } );
        MyOql_FkDefinesList.Add( new FkColumn(){ Entity = "Dept_Community" , Column = "CommId" , CascadeUpdate = false , CascadeDelete = false , RefTable = "Community", RefColumn = "CommID" } );
        MyOql_FkDefinesList.Add( new FkColumn(){ Entity = "Zone" , Column = "CityId" , CascadeUpdate = false , CascadeDelete = false , RefTable = "City", RefColumn = "Id" } );
        MyOql_FkDefinesList.Add( new FkColumn(){ Entity = "City" , Column = "BelongTo" , CascadeUpdate = false , CascadeDelete = false , RefTable = "Province", RefColumn = "Id" } );
        MyOql_FkDefinesList.Add( new FkColumn(){ Entity = "ProductType" , Column = "DeptID" , CascadeUpdate = false , CascadeDelete = false , RefTable = "Dept", RefColumn = "ID" } );
        MyOql_FkDefinesList.Add( new FkColumn(){ Entity = "NoticeInfo" , Column = "NoticeTypeID" , CascadeUpdate = true , CascadeDelete = true , RefTable = "NoticeType", RefColumn = "ID" } );
        MyOql_FkDefinesList.Add( new FkColumn(){ Entity = "NoticeInfo" , Column = "Logo" , CascadeUpdate = false , CascadeDelete = false , RefTable = "Annex", RefColumn = "ID" } );
        MyOql_FkDefinesList.Add( new FkColumn(){ Entity = "ProductClicks" , Column = "ProductID" , CascadeUpdate = true , CascadeDelete = true , RefTable = "ProductInfo", RefColumn = "ID" } );
        MyOql_FkDefinesList.Add( new FkColumn(){ Entity = "ProductAnnex" , Column = "ProductID" , CascadeUpdate = true , CascadeDelete = true , RefTable = "ProductInfo", RefColumn = "ID" } );
        MyOql_FkDefinesList.Add( new FkColumn(){ Entity = "ProductAnnex" , Column = "AnnexID" , CascadeUpdate = true , CascadeDelete = true , RefTable = "Annex", RefColumn = "ID" } );
        MyOql_FkDefinesList.Add( new FkColumn(){ Entity = "ProductInfo" , Column = "ProductTypeID" , CascadeUpdate = true , CascadeDelete = true , RefTable = "ProductType", RefColumn = "ID" } );
        MyOql_FkDefinesList.Add( new FkColumn(){ Entity = "ProductInfo" , Column = "Logo" , CascadeUpdate = false , CascadeDelete = false , RefTable = "Annex", RefColumn = "ID" } );
        MyOql_FkDefinesList.Add( new FkColumn(){ Entity = "ProductDetail" , Column = "ProductID" , CascadeUpdate = false , CascadeDelete = false , RefTable = "ProductInfo", RefColumn = "ID" } );

        MyOql_ViewRelationTablesDict = new Dictionary<string, string[]>();

        MyOql_ProcRelationTablesDict = new Dictionary<string, string[]>();

        MyOql_EntityDict = new Dictionary<string, RuleBase>();
        MyOql_EntityDict.Add("DeptAnnex" , dbr.DeptAnnex ); 
        MyOql_EntityDict.Add("Person" , dbr.Person ); 
        MyOql_EntityDict.Add("Dept" , dbr.Dept ); 
        MyOql_EntityDict.Add("Annex" , dbr.Annex ); 
        MyOql_EntityDict.Add("Dict" , dbr.Dict ); 
        MyOql_EntityDict.Add("ResKey" , dbr.ResKey ); 
        MyOql_EntityDict.Add("ResValue" , dbr.ResValue ); 
        MyOql_EntityDict.Add("PowerController" , dbr.PowerController ); 
        MyOql_EntityDict.Add("PowerAction" , dbr.PowerAction ); 
        MyOql_EntityDict.Add("PowerButton" , dbr.PowerButton ); 
        MyOql_EntityDict.Add("Log" , dbr.Log ); 
        MyOql_EntityDict.Add("Menu" , dbr.Menu ); 
        MyOql_EntityDict.Add("CacheTable" , dbr.CacheTable ); 
        MyOql_EntityDict.Add("Role" , dbr.Role ); 
        MyOql_EntityDict.Add("EnterpriseShowCase" , dbr.EnterpriseShowCase ); 
        MyOql_EntityDict.Add("NoticeShowCase" , dbr.NoticeShowCase ); 
        MyOql_EntityDict.Add("Community" , dbr.Community ); 
        MyOql_EntityDict.Add("Dept_Community" , dbr.DeptCommunity ); 
        MyOql_EntityDict.Add("Zone" , dbr.Zone ); 
        MyOql_EntityDict.Add("City" , dbr.City ); 
        MyOql_EntityDict.Add("Province" , dbr.Province ); 
        MyOql_EntityDict.Add("ProductType" , dbr.Shop.ProductType ); 
        MyOql_EntityDict.Add("NoticeType" , dbr.Shop.NoticeType ); 
        MyOql_EntityDict.Add("NoticeInfo" , dbr.Shop.NoticeInfo ); 
        MyOql_EntityDict.Add("ProductClicks" , dbr.Shop.ProductClicks ); 
        MyOql_EntityDict.Add("ContactMsg" , dbr.Shop.ContactMsg ); 
        MyOql_EntityDict.Add("ProductAnnex" , dbr.Shop.ProductAnnex ); 
        MyOql_EntityDict.Add("ProductInfo" , dbr.Shop.ProductInfo ); 
        MyOql_EntityDict.Add("ProductDetail" , dbr.Shop.ProductDetail ); 
        MyOql_EntityDict.Add("VPowerAction" , dbr.View.VPowerAction ); 
        MyOql_EntityDict.Add("PM_VTxtRes" , dbr.View.VTxtRes ); 

    }
    public static ShopGroupClass Shop { get ; private set ; }
    public static ViewGroupClass View { get ; private set ; }

    public static DeptAnnexRule DeptAnnex { get { return new DeptAnnexRule() ; } }
    public static PersonRule Person { get { return new PersonRule() ; } }
    public static DeptRule Dept { get { return new DeptRule() ; } }
    public static AnnexRule Annex { get { return new AnnexRule() ; } }
    public static DictRule Dict { get { return new DictRule() ; } }
    public static ResKeyRule ResKey { get { return new ResKeyRule() ; } }
    public static ResValueRule ResValue { get { return new ResValueRule() ; } }
    public static PowerControllerRule PowerController { get { return new PowerControllerRule() ; } }
    public static PowerActionRule PowerAction { get { return new PowerActionRule() ; } }
    public static PowerButtonRule PowerButton { get { return new PowerButtonRule() ; } }
    public static LogRule Log { get { return new LogRule() ; } }
    public static MenuRule Menu { get { return new MenuRule() ; } }
    public static CacheTableRule CacheTable { get { return new CacheTableRule() ; } }
    public static RoleRule Role { get { return new RoleRule() ; } }
    public static EnterpriseShowCaseRule EnterpriseShowCase { get { return new EnterpriseShowCaseRule() ; } }
    public static NoticeShowCaseRule NoticeShowCase { get { return new NoticeShowCaseRule() ; } }
    public static CommunityRule Community { get { return new CommunityRule() ; } }
    public static DeptCommunityRule DeptCommunity { get { return new DeptCommunityRule() ; } }
    public static ZoneRule Zone { get { return new ZoneRule() ; } }
    public static CityRule City { get { return new CityRule() ; } }
    public static ProvinceRule Province { get { return new ProvinceRule() ; } }
    public static ProductTypeRule ProductType { get { return new ProductTypeRule() ; } }
    public static NoticeTypeRule NoticeType { get { return new NoticeTypeRule() ; } }
    public static NoticeInfoRule NoticeInfo { get { return new NoticeInfoRule() ; } }
    public static ProductClicksRule ProductClicks { get { return new ProductClicksRule() ; } }
    public static ContactMsgRule ContactMsg { get { return new ContactMsgRule() ; } }
    public static ProductAnnexRule ProductAnnex { get { return new ProductAnnexRule() ; } }
    public static ProductInfoRule ProductInfo { get { return new ProductInfoRule() ; } }
    public static ProductDetailRule ProductDetail { get { return new ProductDetailRule() ; } }
    public static VPowerActionRule VPowerAction { get { return new VPowerActionRule() ; } }
    public static VTxtResRule VTxtRes { get { return new VTxtResRule() ; } }
    public static SplitRule Split(System.String Val,System.String Split) 
    {
        return new SplitRule(Val ,Split ) ;
    }

    //缓存 MyOql 实体信息
    public static Dictionary<string, RuleBase> MyOql_EntityDict { get; private set; }
    public static Dictionary<string, Type> MyOql_VarTableDict { get; private set; }
    public static Dictionary<string, string[]> MyOql_ViewRelationTablesDict { get; private set; }
    public static Dictionary<string, string[]> MyOql_ProcRelationTablesDict { get; private set; }
        
    /// <summary>
    /// 所有表的 外键定义信息
    /// </summary>
    /// <returns></returns>
    public static List<FkColumn> MyOql_FkDefinesList { get; private set; }

    /// <summary>
    /// 获取所有表的 外键定义信息
    /// </summary>
    /// <returns></returns>
    public List<FkColumn> GetFkDefines()
    {
        return MyOql_FkDefinesList;
    }

    /// <summary>
    /// 取 视图 关联的表
    /// </summary>
    /// <returns>Key 是视图的数据库名，Value 是关联表的数据库名</returns>
    public Dictionary<string, string[]> GetViewRelationTables()
    {
        return MyOql_ViewRelationTablesDict;
    }

    /// <summary>
    /// 取 存储过程 关联的表
    /// </summary>
    /// <returns>Key 是存储过程的数据库名，Value 是关联表的数据库名</returns>
    public Dictionary<string, string[]> GetProcRelationTables()
    {
        return MyOql_ProcRelationTablesDict;
    }

    /// <summary>
    /// 根据数据库名称取 表 或 视图
    /// </summary>
    /// <param name="DbName">可以是表名,视图名</param>
    /// <returns></returns>
        public RuleBase GetMyOqlEntity(string DbName)
        {
            if (MyOql_EntityDict.ContainsKey(DbName))
                return MyOql_EntityDict[DbName];

            RuleBase rule = null;
            MyOql_VarTableDict.All(o =>
                {
                    var str = dbo.GetVarTableVars(o.Key, DbName);
                    if (str != null)
                    {
                        //缓存对象
                        rule = CacheHelper.Get("MyOql_VarTable_" + DbName, TimeSpan.FromHours(8), () =>
                           {
                               rule = Activator.CreateInstance(o.Value, str.Values.ToArray()) as RuleBase;
                               return rule;
                           }
                        );
                        return false;
                    }
                    return true;
                });

            if (rule != null) return rule;
            return new ConstTable();
        }
}

}