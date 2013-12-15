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
    /// 
    /// </summary>
    [Serializable]
    public sealed partial class VPowerActionRule : RuleBase, IViewRule,ICloneable
    {


        public  VPowerActionRule() : base("VPowerAction")
        {
            this.Id = new SimpleColumn(this, DbType.Int32, 4,"Id","ID",false);
            this.Area = new SimpleColumn(this, DbType.AnsiString, 50,"Area","Area",true);
            this.Controller = new SimpleColumn(this, DbType.AnsiString, 50,"Controller","Controller",true);
            this.Action = new SimpleColumn(this, DbType.AnsiString, 50,"Action","Action",true);
            this.Button = new SimpleColumn(this, DbType.String, -1,"Button","Button",true);
        }

        /// <summary>
        /// ID(Int32)[主键(ID),自增键]
        /// </summary>
        public SimpleColumn Id { get; set; }
        /// <summary>
        /// Area(AnsiString)
        /// </summary>
        public SimpleColumn Area { get; set; }
        /// <summary>
        /// Controller(AnsiString)
        /// </summary>
        public SimpleColumn Controller { get; set; }
        /// <summary>
        /// Action(AnsiString)
        /// </summary>
        public SimpleColumn Action { get; set; }
        /// <summary>
        /// Button(String)
        /// </summary>
        public SimpleColumn Button { get; set; }

        public override SimpleColumn[] GetColumns() {  return new SimpleColumn[] { Id,Area,Controller,Action,Button }; }
        public override SimpleColumn[] GetPrimaryKeys() { return new SimpleColumn[] { Id };  }
        public override SimpleColumn[] GetComputeKeys() { return new SimpleColumn[] {  }; }
        public override SimpleColumn GetAutoIncreKey() {  return Id; }
        public override SimpleColumn GetUniqueKey() { return  null; }
        public override string GetDbName() { return "VPowerAction"; }

        public Entity FindById(Int32 Id)
        {
             if ( Id <= 0 ) return null ;
            return this.SelectWhere(o => o.Id == Id).ToEntity<Entity>();
        }


        public override object Clone()
        {
            var tab = new VPowerActionRule();
            if ( this._Config_ != null ) tab._Config_ = base._Config_.Clone() as RuleRuntimeConfig ;
            tab.SetAlias(base.Name);
            tab.SetReconfig(base.ReConfig);

            tab.Id = this.Id.Clone() as SimpleColumn;
            tab.Area = this.Area.Clone() as SimpleColumn;
            tab.Controller = this.Controller.Clone() as SimpleColumn;
            tab.Action = this.Action.Clone() as SimpleColumn;
            tab.Button = this.Button.Clone() as SimpleColumn;

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
            /// Area
            /// </summary>
            public String Area { get; set; }
 
            /// <summary>
            /// Controller
            /// </summary>
            public String Controller { get; set; }
 
            /// <summary>
            /// Action
            /// </summary>
            public String Action { get; set; }
 
            /// <summary>
            /// Button
            /// </summary>
            public String Button { get; set; }

            public bool SetPropertyValue(string PropertyName, object Value)
            {
                if ( PropertyName == "Id" ) { this.Id = ValueProc.As<Int32>(Value) ; return true; }
                if ( PropertyName == "Area" ) { this.Area = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "Controller" ) { this.Controller = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "Action" ) { this.Action = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "Button" ) { this.Button = ValueProc.As<String>(Value) ; return true; }
                return false ;
           }

            public object GetPropertyValue(string PropertyName)
            {
                if ( PropertyName == "Id" ) { return this.Id ; }
                if ( PropertyName == "Area" ) { return this.Area ; }
                if ( PropertyName == "Controller" ) { return this.Controller ; }
                if ( PropertyName == "Action" ) { return this.Action ; }
                if ( PropertyName == "Button" ) { return this.Button ; }
                return null ;
            }

            public string[]  GetProperties() { return new string[]{ "Id","Area","Controller","Action","Button" } ; }
            
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
    public sealed partial class VTxtResRule : RuleBase, IViewRule,ICloneable
    {


        public  VTxtResRule() : base("VTxtRes")
        {
            this.ResID = new SimpleColumn(this, DbType.Int32, 4,"ResID","ResID",false);
            this.Group = new SimpleColumn(this, DbType.AnsiString, 50,"Group","Group",true);
            this.Key = new SimpleColumn(this, DbType.AnsiString, 250,"Key","Key",false);
            this.Id = new SimpleColumn(this, DbType.Int32, 4,"Id","ID",true);
            this.Lang = new SimpleColumn(this, DbType.Int32, 4,"Lang","Lang",true);
            this.Value = new SimpleColumn(this, DbType.AnsiString, 2500,"Value","Value",true);
        }

        /// <summary>
        /// ResID(Int32)[主键(Lang,ResID)]
        /// </summary>
        public SimpleColumn ResID { get; set; }
        /// <summary>
        /// Group(AnsiString)
        /// </summary>
        public SimpleColumn Group { get; set; }
        /// <summary>
        /// Key(AnsiString)
        /// </summary>
        public SimpleColumn Key { get; set; }
        /// <summary>
        /// ID(Int32)
        /// </summary>
        public SimpleColumn Id { get; set; }
        /// <summary>
        /// Lang(Int32)[主键(Lang,ResID)]
        /// </summary>
        public SimpleColumn Lang { get; set; }
        /// <summary>
        /// Value(AnsiString)
        /// </summary>
        public SimpleColumn Value { get; set; }

        public override SimpleColumn[] GetColumns() {  return new SimpleColumn[] { ResID,Group,Key,Id,Lang,Value }; }
        public override SimpleColumn[] GetPrimaryKeys() { return new SimpleColumn[] { Lang,ResID };  }
        public override SimpleColumn[] GetComputeKeys() { return new SimpleColumn[] {  }; }
        public override SimpleColumn GetAutoIncreKey() {  return null; }
        public override SimpleColumn GetUniqueKey() { return  null; }
        public override string GetDbName() { return "PM_VTxtRes"; }


        public Entity FindByPks(LangEnum Lang,Int32 ResID)
        {
            return this.SelectWhere(o => o.Lang == Lang&o.ResID == ResID).ToEntity<Entity>();
        }

        public override object Clone()
        {
            var tab = new VTxtResRule();
            if ( this._Config_ != null ) tab._Config_ = base._Config_.Clone() as RuleRuntimeConfig ;
            tab.SetAlias(base.Name);
            tab.SetReconfig(base.ReConfig);

            tab.ResID = this.ResID.Clone() as SimpleColumn;
            tab.Group = this.Group.Clone() as SimpleColumn;
            tab.Key = this.Key.Clone() as SimpleColumn;
            tab.Id = this.Id.Clone() as SimpleColumn;
            tab.Lang = this.Lang.Clone() as SimpleColumn;
            tab.Value = this.Value.Clone() as SimpleColumn;

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
            /// ResID[主键(Lang,ResID)]
            /// </summary>
            public Int32 ResID { get; set; }
 
            /// <summary>
            /// Group
            /// </summary>
            public String Group { get; set; }
 
            /// <summary>
            /// Key
            /// </summary>
            public String Key { get; set; }
 
            /// <summary>
            /// ID
            /// </summary>
            public Int32 Id { get; set; }
 
            /// <summary>
            /// Lang[主键(Lang,ResID)]
            /// </summary>
            public LangEnum Lang { get; set; }
 
            /// <summary>
            /// Value
            /// </summary>
            public String Value { get; set; }

            public bool SetPropertyValue(string PropertyName, object Value)
            {
                if ( PropertyName == "ResID" ) { this.ResID = ValueProc.As<Int32>(Value) ; return true; }
                if ( PropertyName == "Group" ) { this.Group = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "Key" ) { this.Key = ValueProc.As<String>(Value) ; return true; }
                if ( PropertyName == "Id" ) { this.Id = ValueProc.As<Int32>(Value) ; return true; }
                if ( PropertyName == "Lang" ) { this.Lang = ValueProc.As<LangEnum>(Value) ; return true; }
                if ( PropertyName == "Value" ) { this.Value = ValueProc.As<String>(Value) ; return true; }
                return false ;
           }

            public object GetPropertyValue(string PropertyName)
            {
                if ( PropertyName == "ResID" ) { return this.ResID ; }
                if ( PropertyName == "Group" ) { return this.Group ; }
                if ( PropertyName == "Key" ) { return this.Key ; }
                if ( PropertyName == "Id" ) { return this.Id ; }
                if ( PropertyName == "Lang" ) { return this.Lang ; }
                if ( PropertyName == "Value" ) { return this.Value ; }
                return null ;
            }

            public string[]  GetProperties() { return new string[]{ "ResID","Group","Key","Id","Lang","Value" } ; }
            
            public object Clone()
            {
                return this.CloneIEntity() ;
            }

         }
    }



}