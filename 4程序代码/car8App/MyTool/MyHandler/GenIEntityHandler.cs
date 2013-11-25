using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace MyTool
{
    public partial class GenIEntityHandler : Form, ICommandHandler
    {
        public GenIEntityHandler()
        {
            InitializeComponent();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            var tick = DateTime.Now.Ticks;
            var className = "obj_" + new Random((int)(tick >> 32)).Next(10000);
            var result = NewObj(className, this.txtSource.Text);

            if (result.Errors.Count > 0)
            {
                for (int i = 0; i < result.Errors.Count; i++)
                {
                    var er = result.Errors[i];
                    this.txtResult.Text = er.ErrorText + Environment.NewLine;
                }

                this.txtResult.ForeColor = Color.Red;
                return;
            }

            this.txtResult.ForeColor = Color.Black;

            //  获取编译后的程序集。  
            Assembly assembly = result.CompiledAssembly;

            //  动态调用方法。  
            object eval = assembly.CreateInstance(className);


            var ps = eval.GetType().GetProperties();

            var listSetProp = new List<string>();
            var listGetProp = new List<string>();
            var listProp = new List<string>();

            ps.All(p =>
            {
                listGetProp.Add(@"if ( PropertyName == """ + p.Name + @""" ) { return this." + p.Name + " ; }");
                listSetProp.Add(@"if ( PropertyName == """ + p.Name + @""" ) { this." + p.Name + @" = ValueProc.As<" + p.PropertyType.Name + ">(Value) ; return true; }");
                listProp.Add(p.Name);

                return true;
            });

            this.txtResult.Text = string.Format(@"
public object GetPropertyValue(string PropertyName)
{{
{0}
return null ;
}}
public bool SetPropertyValue(string PropertyName, object Value)
{{
{1}
return false;
}}
public string[]  GetProperties() {{ return new string[]{{
{2}
}};
}}
public object Clone()
{{
    return this.CloneIEntity() ;
}}
", string.Join(System.Environment.NewLine, listGetProp.ToArray()),
 string.Join(Environment.NewLine, listSetProp.ToArray()),
 string.Join(",", listProp.Select(o => @"""" + o + @"""").ToArray())
 );
        }
        public static CompilerResults NewObj(string className, string expression)
        {
            //  创建编译器实例。  
            ICodeCompiler complier = (new CSharpCodeProvider().CreateCompiler());

            //  设置编译参数。  
            CompilerParameters paras = new CompilerParameters();
            paras.GenerateExecutable = false;   //编译成exe还是dll
            paras.GenerateInMemory = true;   //是否写入内存,不写入内存就写入磁盘
            paras.ReferencedAssemblies.Add("System.dll");


            //  创建动态代码。  
            var classSource = string.Format(@"
public sealed class {0}
{{
{1}
}}
", className, expression);

            //   System.Diagnostics.Debug.WriteLine(classSource.ToString());  

            //  编译代码。  
            CompilerResults result = complier.CompileAssemblyFromSource(paras, classSource);
            return result;
        }





        public string Do()
        {
            this.ShowDialog();
            return string.Empty;
        }
    }
}
