using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DbEnt;
using MyOql;
using MyBiz.Sys;
using MyCmn;
using MyBiz;
using System.Configuration;

namespace MyWeb.Areas.Admin.Models
{
    public class PowerModel
    {
        private PowerModel()
        {
            this.Create = new Dictionary<int, string>();
            this.Read = new Dictionary<int, string>();
            this.Update = new Dictionary<int, string>();
            this.Delete = new Dictionary<int, string>();
            this.Action = new Dictionary<int, string>();
            this.Button = new Dictionary<int, string>();

            this.MyCreate = new List<int>();
            this.MyRead = new List<int>();
            this.MyUpdate = new List<int>();
            this.MyDelete = new List<int>();
            this.MyAction = new List<int>();
            this.MyButton = new List<int>();


        }
        public PowerModel(PowerJson JsonValue)
        {
            this.MyCreate = JsonValue.Create.ToPositions().ToList();
            this.MyRead = JsonValue.Read.ToPositions().ToList();
            this.MyUpdate = JsonValue.Update.ToPositions().ToList();
            this.MyDelete = JsonValue.Delete.ToPositions().ToList();
            this.MyAction = JsonValue.Action.ToPositions().ToList();
            this.MyButton = JsonValue.Button.ToPositions().ToList();

            var noPowerTables = dbo.MyOqlSect.Entitys.GetConfig(o => true).Select(o => o.Name); // PowerBiz.GetConfigNoPowerEntitys();


            this.Action = dbr.View.VPowerAction.Select().ToEntityList(o => o._).ToDictionary(o => o.Id, o => o.Area + "." + o.Controller + "." + o.Action);
            this.Button = dbr.PowerButton.Select()
                .Join(dbr.PowerAction, (a, b) => a.ActionID == b.Id, o => o.Action)
                .Join(dbr.PowerController, (a, b) => dbr.PowerAction.ControllerID == b.Id, o => new ColumnClip[] { o.Area, o.Controller })
                .ToDictList()
                .ToDictionary(o => o[dbr.PowerButton.Id.Name].AsInt(), o => o[dbr.PowerController.Area.Name].AsString() + "." + o[dbr.PowerController.Controller.Name].AsString() + "." + o[dbr.PowerAction.Action.Name].AsString() + "." + o[dbr.PowerButton.Button.Name].AsString());

            //if (string.Equals(ConfigurationManager.AppSettings["MyDataPower"], "true", StringComparison.CurrentCultureIgnoreCase))
            //{
            //    this.Create = dbr.PowerTable.Select().ToEntityList(o => o._).Where(o => o.Table.IsIn(noPowerTables) == false).ToDictionary(o => o.Id, o => o.Table);
            //    this.Read = dbr.View.VPowerData.Select().ToEntityList(o => o._).Where(o => o.Table.IsIn(noPowerTables) == false).ToDictionary(o => o.Id, o => o.Table + "." + o.Column);
            //    this.Update = dbr.View.VPowerData.Select().ToEntityList(o => o._).Where(o => o.Table.IsIn(noPowerTables) == false).ToDictionary(o => o.Id, o => o.Table + "." + o.Column);
            //    this.Delete = dbr.PowerTable.Select().ToEntityList(o => o._).Where(o => o.Table.IsIn(noPowerTables) == false).ToDictionary(o => o.Id, o => o.Table);
            //}
        }
        public Dictionary<int, string> Create { get; set; }
        public Dictionary<int, string> Read { get; set; }
        public Dictionary<int, string> Update { get; set; }
        public Dictionary<int, string> Delete { get; set; }
        public Dictionary<int, string> Action { get; set; }
        public Dictionary<int, string> Button { get; set; }

        public List<int> MyCreate { get; set; }
        public List<int> MyRead { get; set; }
        public List<int> MyUpdate { get; set; }
        public List<int> MyDelete { get; set; }
        public List<int> MyAction { get; set; }
        public List<int> MyButton { get; set; }
    }
}