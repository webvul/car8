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
using MyCmn;
using MyOql;
using System.Web.Compilation;


namespace MyTool
{
    public partial class GenDbSqlHandler : Form, ICommandHandler
    {
        public GenDbSqlHandler()
        {
            InitializeComponent();

            var set = dbo.MyOqlSect.DbProviders.GetEnumerator();
            while (set.MoveNext())
            {
                var sect = set.Current as MyOql.MyOqlConfigSect.ProviderCollection.ProviderElement;

                this.dbType.Items.Add(sect.Name);
            }

            this.dbType.SelectedIndex = 0;
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            MyOql.Provider.TranslateSql tranSql = Activator.CreateInstance(BuildManager.GetType(dbo.GetProviderType(this.dbType.SelectedItem as string), true)) as MyOql.Provider.TranslateSql;

            var list = new List<string>();
            var assembly = Assembly.LoadFrom(this.txtFile.Text);
            for (int i = 0; i < this.listTable.SelectedItems.Count; i++)
            {
                var item = this.listTable.SelectedItems[i] as string;
                var type = assembly.GetType(item);

                list.Add(tranSql.GenSql(Activator.CreateInstance(type) as RuleBase));
            }

            this.txtResult.Text = list.Join(Environment.NewLine);
        }



        public string Do()
        {
            this.ShowDialog();
            return string.Empty;
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (this.openFileDialog1.FileName.HasValue() == false) return;

                this.txtFile.Text = this.openFileDialog1.FileName;

                this.listTable.Items.Clear();
                var assembly = Assembly.LoadFile(this.txtFile.Text);
                var typeRuleBase = typeof(RuleBase);
                assembly.GetTypes().All(o =>
                {
                    if (o.IsSubclassOf(typeRuleBase) && o.GetInterface("ITableRule") != null)
                    {
                        this.listTable.Items.Add(o.FullName);
                    }
                    return true;
                });
            }
        }
         
        private void btnAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.listTable.Items.Count; i++)
            {
                this.listTable.SetSelected(i, true);
            }
        }

        private void btnFan_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.listTable.Items.Count; i++)
            {
                if (this.listTable.GetSelected(i))
                {
                    this.listTable.SetSelected(i, false);
                }
                else
                {
                    this.listTable.SetSelected(i, true);
                }
            }
        }
    }
}
