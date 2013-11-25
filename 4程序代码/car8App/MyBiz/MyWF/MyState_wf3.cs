//using System;
//using System.ComponentModel;
//using System.ComponentModel.Design;
//using System.Collections;
//using System.Drawing;
//using System.Linq;
//using System.Workflow.ComponentModel;
//using System.Workflow.ComponentModel.Design;
//using System.Workflow.ComponentModel.Compiler;
//using System.Workflow.ComponentModel.Serialization;
//using System.Workflow.Runtime;
//using System.Workflow.Activities;
//using System.Workflow.Activities.Rules;
//using System.Drawing.Design;
//using MyOql;
//
//namespace MyWF
//{
//	public partial class MyState: StateActivity
//	{
//		[DefaultValue("")]
//		[Editor(typeof(BindUITypeEditor), typeof(UITypeEditor))]
//		[Description("分配的角色名。")]
//		public RoleEnum RoleName { get; set; }
//
//		[DefaultValue("")]
//		[Editor(typeof(BindUITypeEditor), typeof(UITypeEditor))]
//		[Description("分配的第二个角色名。")]
//		public string RoleValue { get; set; }
//
//		public MyState()
//		{
//			InitializeComponent();
//		}
//	}
//}
