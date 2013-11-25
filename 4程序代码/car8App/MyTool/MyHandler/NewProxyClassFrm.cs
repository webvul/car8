using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Web.Compilation;
using MyCmn;
using MyOql;
using System.Reflection;

namespace MyTool.MyHandler
{
    public class a<T>
    {
        public class b<M>
        {
            public class c<D>
            {
            }
        }
    }
    public static class A
    {
        public static string GetFullName(this Type type)
        {
            if (type == typeof(void)) return "void";

            var csType = type.FullName ?? type.Name;
            if (type.IsGenericType)
            {
                csType = type.GetGenericTypeDefinition().FullName;
                var argTypes = type.GetGenericArguments();
                var argPos = 0;
                var fullName = new List<char>();
                for (int i = 0; i < csType.Length; i++)
                {
                    var item = csType[i];
                    if (item == '[') break;
                    if (item == '`')
                    {
                        i++;
                        var geshu = csType[i].AsInt();
                        fullName.Add('<');

                        fullName.AddRange(string.Join(",",
                            argTypes
                            .Skip(argPos)
                            .Take(geshu)
                            .Select(o => o.GetFullName()).ToArray())
                            );

                        fullName.Add('>');

                        argPos += geshu;

                        continue;
                    }

                    if (item == '+') item = '.';
                    fullName.Add(item);
                }

                return new string(fullName.ToArray());
            }
            return csType;
        }
    }
    public partial class NewProxyClassFrm : Form
    {
        public NewProxyClassFrm()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            //var cs = GenClass(this.txtMetaType.Text, this.txtPropName.Text, this.txtClassName.Text, this.txtFileName.Text);
            //this.txtCs.Text = cs;
        }

        //需要底层重构的新版本。
//        private string GenClass(string metaType, string propName, string clsName, string filePath)
//        {
//            string strClassTemplate = @"
//public class $Class$
//{
//#region 暴露  $Type$ 的方法
//    private $Type$ $propName$ {get;set;}
// 
//【for:IndexProperty】
//    public $Type$ $Name$ [【for:Params】【if】,【fi】$PType$ $PName$【endfor】]{ get { return $propName$[【for:Params】【if】,【fi】$PName$【endfor】] ;}【if:canWrite】set { $propName$[【for:Params】【if】,【fi】$PName$【endfor】] = value ;}【fi】 }
//【endfor】
//  
//【for:Property】
//    public $Type$ $Name$ { get { return $propName$.$Name$ ;}【if:canWrite】 set { $propName$.$Name$ = value ;}【fi】}
//【endfor】
//
//【for:Method】
//    public $Type$ $Name$【if:isGen】<【for:TParams】【if:notFirst】,【fi】$T$【endfor】>【fi】(【for:Params】【if】,【fi】$PType$ $PName$【endfor】){$return$$propName$.$Name$(【for:Params】【if】,【fi】$PName$【endfor】);}
//【endfor】
//#endregion
//}
//";
//            var type = Type.GetType(metaType);
//            var obj = Activator.CreateInstance(type);
//            var ps = type.GetProperties();
//            var psNames = ps.Select(o => o.Name).ToArray();
//            var ms = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

//            var csType = type.GetFullName();

//            strClassTemplate = strClassTemplate
//                .TmpFor("IndexProperty", ps
//                    .Where(o => (o.Name == "Item" && o.GetIndexParameters().Length > 0))
//                 )
//                .DoIf("canWrite", o => o.CanWrite)
//                .DoFor("Name", o => "this")
//                .DoFor("Type", o => o.PropertyType.GetFullName())
//                .DoEach((a, b) =>
//                {
//                    var pis = a.GetIndexParameters();

//                    return b.TmpFor("Params", pis.Select(o => o.Name).ToArray())
//                        .DoIf(o => o != pis.First().Name)
//                        .DoFor("PType", o => pis.First(p => p.Name == o).ParameterType.GetFullName())
//                        .DoFor("PName", o => o)
//                        .EndFor();
//                })
//                .EndFor()

//                .TmpFor("Property", ps
//                    .Where(o => !(o.Name == "Item" && o.GetIndexParameters().Length > 0))
//                 )
//                .DoIf("canWrite", o => o.CanWrite)
//                .DoFor("Name", o => o.Name)
//                .DoFor("Type", o => o.PropertyType.GetFullName())



//                .EndFor()

//                .TmpFor("Method", ms
//                        .Where(o =>
//                        {
//                            if (o.Name.StartsWith("get_") || o.Name.StartsWith("set_"))
//                            {
//                                return !psNames.Contains(o.Name.Slice(4));
//                            }
//                            return true;
//                        })
//                 )
//                .DoIf("isGen", o => o.IsGenericMethod)
//                .DoFor("Name", o => o.Name)
//                .DoFor("Type", o => o.ReturnType.GetFullName())
//                .DoFor("return", o => o.ReturnType == typeof(void) ? string.Empty : "return ")
//                .DoEach((a, b) =>
//                {
//                    var mi = a;
//                    var mips = mi.GetParameters();
//                    return b
//                        .TmpFor("Params", mips)
//                        .DoIf(o => o.Name != mips.First().Name)
//                        .DoFor("PType", o => o.ParameterType.GetFullName())
//                        .DoFor("PName", o => o.Name)
//                        .EndFor()
//                        ;
//                })
//                .DoEach((a, b) =>
//                {
//                    var mi = a;
//                    var mips = mi.GetGenericArguments();
//                    return b
//                        .TmpFor("TParams", mips)
//                        .DoIf("notFirst", o => o.Name != mips.First().Name)
//                        .DoFor("T", o => o.Name)
//                        .EndFor()
//                        ;
//                })
//                .EndFor()
//                ;



//            return strClassTemplate
//                .Replace("$propName$", propName)
//                .Replace("$Class$", clsName)
//                .Replace("$Type$", csType)
//                ;
//        }


        private void btnDlgOpen_Click(object sender, EventArgs e)
        {
            if (this.saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.txtCs.Text = this.saveFileDialog1.FileName;
            }
        }

    }
}
