//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using MyCmn;
//using MyOql;
//using MyBiz;
//using MyBiz.Sys;
//using DbEnt;


//namespace MyBiz.Admin
//{
//    public partial class PowerBiz
//    {
//        public static MyOqlSet GetColumnsPowerData(int Skip, int Take, string TableName, string ColumnName,MyBigInt power)
//        {
//            //var query = collection["query"].FromJson<XmlDictionary<string, string>>();

//            //string Column = query.GetOrDefault("Column");
//            //string Table = query.GetOrDefault("Table");



//            var tabs = dbr.PowerTable.Select(o => o.Id)
//                .Where(o => o.Table.Like("%" + TableName + "%"))
//                .Skip(Skip)
//                .Take(Take)
//                .ToEntityList(0);


//            //根据根节点分页
//            var data = dbr.PowerColumn
//                .Select(o => new ColumnClip[] { ("C_" + o.Id).As("Id"), o.Column, ("T_" + o.TableID).As("TableID") })
//                .Join(dbr.PowerTable, (a, b) => a.TableID == b.Id, o => new ColumnClip[] { o.Table, new ConstColumn("False").As("Sel") })
//                .Where(o => o.Column.Like("%" + ColumnName + "%") & o.TableID.In(tabs.ToArray()))
//                .SkipPower()
//                .ToMyOqlSet();


//                foreach (int act in power.ToPositions())
//                {
//                    var row = data.Rows.FirstOrDefault(o => o[0].AsString() == "C_" + act);
//                    if (row != null) row[4] = true.AsString();
//                }

//            data.Rows.Select(o => new string[] { o[2].AsString(), o[3].AsString() }).Distinct().ToArray().All(o =>
//            {
//                data.Rows.Add(new object[] { o[0], o[1], "", "" });
//                return true;
//            });


//            return data;
//        }
//    }
//}
