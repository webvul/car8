//using System;
//using System.Data;
//using System.Configuration;
//using System.Web;
//using System.Web.Security;
//using System.Web.UI;
//using System.Web.UI.WebControls;
//using System.Web.UI.WebControls.WebParts;
//using System.Web.UI.HtmlControls;
//using System.Workflow.Runtime;
//using System.Workflow.Runtime.Hosting;
//using System.Workflow.Activities;
//using System.Collections.ObjectModel;
//using System.Workflow.ComponentModel;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using System.Linq;
//using MyOql;
//using System.Web.Compilation;
//using MyCmn;
//using MyWF;

//namespace MyWF
//{
//    /// <summary>
//    /// MyWorkHelpr 的摘要说明
//    /// </summary>
//    public class MyWFHelpr
//    {
//        /// <summary>
//        /// 一个小帮助函数
//        /// </summary>
//        public MyWFHelpr()
//        {
//            //
//            // TODO: 在此处添加构造函数逻辑
//            //
//        }
//        public const string WorkflowRuntimeName = "WorkflowRuntime";
//        private static string connectstring;
//        public static string GetConnectionString()
//        {
//            return connectstring;
//        }
//        public static void InitWorkFlow(string ConnectionString)
//        {
//            connectstring = ConnectionString;

//            System.Workflow.Runtime.WorkflowRuntime workflowRuntime = new System.Workflow.Runtime.WorkflowRuntime(WorkflowRuntimeName);

//            ManualWorkflowSchedulerService man = new ManualWorkflowSchedulerService();
//            workflowRuntime.AddService(man);

//            //加载状态保持服务（自己的类），构造函数设置保存状态的路径
//            NameValueCollection nvc = new NameValueCollection();
//            nvc["connectionString"] = connectstring;
//            nvc["unloadOnIdle"] = true.ToString();
//            SqlWorkflowPersistenceService sql = new SqlWorkflowPersistenceService(nvc);
//            //ConnectionString, true, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(3));
//            workflowRuntime.AddService(sql);

//            //ExternalDataExchangeService ex = new ExternalDataExchangeService();
//            //workflowRuntime.AddService(ex);


//            workflowRuntime.StartRuntime();

//            foreach (var item in sql.GetAllWorkflows())
//            {
//                workflowRuntime.GetWorkflow(item.WorkflowInstanceId);
//            }

//            HttpContext.Current.Application[MyWFHelpr.WorkflowRuntimeName] = workflowRuntime;
//        }

//        public static string GetWfDescription(string WfName)
//        {
//            string type = "MyBiz." + WfName;
//            Activity ay = BuildManager.GetType(type, true).Assembly.CreateInstance(type) as Activity;
//            return ay.Description;
//        }

//        public delegate void StartWorkDocDelegate(Guid WorkflowID);
//        public static WorkflowInstance StartMyWorkflow<WFT>(WfService Service, RoleEnum OperatorRole, string SendUserID, StartWorkDocDelegate InitDoc)
//            where WFT : StateMachineWorkflowActivity, new()
//        {
//            if (Service.CreateValid<WFT>(OperatorRole) == false) return null;

//            ManualWorkflowSchedulerService scheduler = GetWfRuntime().GetService<ManualWorkflowSchedulerService>();

//            WorkflowInstance workflowInstance = GetWfRuntime().CreateWorkflow(typeof(WFT));
//            try
//            {
//                InitDoc(workflowInstance.InstanceId);
//            }
//            catch
//            {
//                return null;
//            }
//            //启动
//            workflowInstance.Start();

//            //开始运行
//            scheduler.RunWorkflow(workflowInstance.InstanceId);

//            var actdict = GetEnableAction(workflowInstance.InstanceId);
//            if (actdict.Count == 1)
//            {
//                Service.RaiseEvent(actdict.Keys.ElementAt(0), workflowInstance.InstanceId, OperatorRole.ToString(), SendUserID, "");
//            }
//            else
//            {
//                Service.InsertToProcess(workflowInstance.InstanceId, GetEnableAction(workflowInstance.InstanceId).ElementAt(0).Key, typeof(WFT).Name, SendUserID);
//            }

//            return workflowInstance;
//        }

//        public static WorkflowRuntime GetWfRuntime()
//        {
//            return HttpContext.Current.Application[WorkflowRuntimeName] as WorkflowRuntime;
//        }

//        public static StateMachineWorkflowInstance GetWorkflowInstance(Guid WorkflowID)
//        {
//            return new StateMachineWorkflowInstance(GetWfRuntime(), WorkflowID);
//        }

//        public static string GetCurrentStateName(Guid stateInstanceGuid)
//        {
//            StateActivity sa = GetWorkflowInstance(stateInstanceGuid).CurrentState;
//            if (sa.Description.Trim().Length > 0) return sa.Description;
//            return sa.Name;
//        }

//        public static T GetService<T>()
//        {
//            return GetWfRuntime().GetService<T>();
//        }

//        public static object GetServiceFromWfName(string WfName)
//        {
//            //Dictionary<string, string> dict = new Dictionary<string, string>();
//            //dict["WFInvite"] = "InviteService";

//            //string type = "MyBiz." + WfName;
//            //StateMachineWorkflowActivity ay = BuildManager.GetType(type, true).Assembly.CreateInstance(type) as StateMachineWorkflowActivity;

//            return GetWfRuntime().GetService(Type.GetType("MyBiz." + WfName.Remove(0, 2) + "Service"));
//        }

//        public static Dictionary<string, string> GetEnableAction(Guid stateInstanceGuid)
//        {
//            return GetEnableAction(GetWorkflowInstance(stateInstanceGuid));
//        }

//        public static Dictionary<string, string> GetEnableAction(StateMachineWorkflowInstance stateInstance)
//        {
//            Dictionary<string, string> retVal = new Dictionary<string, string>();
//            foreach (Activity act in stateInstance.CurrentState.EnabledActivities)
//            {
//                if (act is EventDrivenActivity)
//                {
//                    EventDrivenActivity edact = (EventDrivenActivity)act;

//                    if (edact.EnabledActivities.Count > 0 && edact.EnabledActivities[0] is HandleExternalEventActivity)
//                    {
//                        if (string.IsNullOrEmpty(act.Description))
//                        {
//                            retVal.Add(edact.Name, act.Name);
//                        }
//                        else
//                        {
//                            retVal.Add(edact.Name, act.Description);
//                        }
//                    }
//                }
//            }
//            return retVal;
//        }
//        public static string GetActivityDescription(string WfType, string StateName)
//        {
//            StateMachineWorkflowActivity act = Activator.CreateInstance(Type.GetType("MyBiz." + WfType)) as StateMachineWorkflowActivity;
//            var state = act.Activities.FirstOrDefault(o => o.Name == StateName);
//            if (state == null) return "";
//            else
//            {
//                return state.Description.HasValue() ? state.Description : state.Name;
//            }
//        }


//        public static string GetActivityDescription(Guid stateInstanceGuid, string StateName)
//        {
//            return GetActivity(stateInstanceGuid, StateName).Description;
//        }

//        public static Activity GetActivity(Guid stateInstanceGuid, string StateName)
//        {
//            return GetActivity(GetWorkflowInstance(stateInstanceGuid), StateName);
//        }

//        private static Activity GetActivity(StateMachineWorkflowInstance stateMachineWorkflowInstance, string StateName)
//        {
//            foreach (StateActivity state in stateMachineWorkflowInstance.States)
//            {
//                if (state.Name == StateName) return state;

//                Activity subAct = state.GetActivityByName(StateName);
//                if (subAct != null) return subAct;

//                //foreach (Activity act in state.Activities)
//                //{
//                //    if (act.Name == StateName) return act;


//                //}
//            }

//            return null;
//        }

//        public static RoleEnum GetWorkflowRecvRole(Guid instanceId, string EventName)
//        {
//            RoleEnum list = RoleEnum.None;

//            foreach (Activity item in (GetActivity(instanceId, EventName) as EventDrivenActivity).Activities)
//            {
//                if (item is SetStateActivity)
//                {
//                    MyState state = GetActivity(instanceId, (item as SetStateActivity).TargetStateName) as MyState;

//                    if (state.RoleName != RoleEnum.None) list |= state.RoleName;
//                    if (state.RoleValue.HasValue()) list |= state.RoleValue.ToEnum(RoleEnum.None);
//                    break;
//                }
//            }

//            return list;
//        }
//    }
//}