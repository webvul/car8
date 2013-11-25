using System;
using System.Collections.Generic;
using System.Linq;
using MyCmn;
using System.Configuration;
using System.Reflection;
using System.Data;

namespace MyOql.MyOql_CodeGen
{
    public partial class MyOqlCodeGen
    {
        private string GenEnt(MyOqlCodeGenSect.TableCollection.TableGroupCollection group, string Interface)
        {
            var strEnt = @"
【for:Ents】
    $EntClass$
【endfor】
"
      .TmpFor("Ents", group.ToMyList(o => o as MyOqlCodeGenSect.TableCollection.TableGroupCollection.BaseTableElement))
      .DoFor("Ent", o => dbo.TranslateDbName(o.MapName))
      .DoFor("EntClass", o =>
      {
          OnDoing(o);
          var strTemp = o.ToEntityCode();
          OnDoed(o);
          return strTemp;
      })
      .EndFor()
      ;

            return strEnt;
        }



    }
}
