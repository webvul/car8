//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Workflow.ComponentModel;
//using System.Workflow.Activities;
//using MyCmn;
//using System.Workflow.Runtime.Hosting;
//using MyOql;

//namespace MyWF
//{
//    [Serializable]
//    public abstract class WfService
//    {
//        public Dictionary<string, EventHandler<ExternalDataEventArgs>> _EventList = new Dictionary<string, EventHandler<ExternalDataEventArgs>>();

//        public void RaiseEvent(string EventName, Guid instanceId, string OperatorRole, string SenderUserID, string Sugestion)
//        {
//            EventHandler<ExternalDataEventArgs> eventHand = _EventList[EventName];
//            if (eventHand != null)
//            {
//                //检查角色
//                if (Valid(instanceId, EventName, OperatorRole) == true)
//                {
//                    UpdateProcess(instanceId, Sugestion);

//                    ExternalDataEventArgs ede = new ExternalDataEventArgs(instanceId);
//                    //ede.WorkItem = parms;

//                    eventHand(this, ede);

//                    string WfType = MyWFHelpr.GetWorkflowInstance(instanceId).StateMachineWorkflow.Name;
//                    MyWFHelpr.GetService<ManualWorkflowSchedulerService>().RunWorkflow(instanceId);

//                    InsertToProcess(instanceId, EventName, WfType, SenderUserID);
//                }
//                else
//                {
//                    throw new Exception("您无权执行：" + EventName + " !");
//                }
//            }
//        }



//        public bool CreateValid<WFT>(RoleEnum OperatorRole) where WFT : StateMachineWorkflowActivity, new()
//        {
//            MyState state = (new WFT().Activities[0] as MyState);
//            if (state.RoleName == RoleEnum.None && state.RoleValue.ToEnum(RoleEnum.None) == RoleEnum.None) return true;
//            if (OperatorRole.Contains(state.RoleName)) return true;
//            if ((state.RoleValue.ToEnum<RoleEnum>() & OperatorRole) != 0) return true;
//            return false;
//        }


//        public bool Valid(Guid instaceId, string EventName, string OperatorRole)
//        {
//            Activity act = MyWFHelpr.GetActivity(instaceId, EventName);
//            if (act != null)
//            {
//                if (act is EventDrivenActivity)
//                {
//                    if ((act as EventDrivenActivity).Parent is MyState)
//                    {
//                        RoleEnum myrole = OperatorRole.ToEnum<RoleEnum>();
//                        MyState state = ((act as EventDrivenActivity).Parent as MyState);
//                        if (state.RoleName == RoleEnum.None && state.RoleValue.ToEnum(RoleEnum.None) == RoleEnum.None)
//                            return true;

//                        if (myrole.Contains(state.RoleName) ||
//                            (state.RoleValue.ToEnum<RoleEnum>() & myrole) != 0)
//                        {
//                            return true;
//                        }
//                    }
//                }
//            }
//            return false;
//        }
//        public void InsertToProcess(Guid instanceId, string EventName, string WfType, string SenderUserID)
//        {

//            string strID = instanceId.ToString();
//            //int stepID = db._
//            //    .Select(dbo.WorkProcess, dbo.WorkProcess.StepID.Max())
//            //    .Where(dbo.WorkProcess.WfID == instanceId)
//            //    .ToScalar<int>();

//            StateMachineWorkflowInstance instance = null;
//            try
//            {
//                instance = MyWFHelpr.GetWorkflowInstance(instanceId);
//            }
//            catch
//            {

//            }

//            //WorkProcessEntity wp = new WorkProcessEntity();
//            //wp.WfID = instanceId;
//            //wp.StepID = stepID + 1;
//            //wp.WfType = WfType;
//            //wp.State = (instance == null ? "" : instance.CurrentStateName);
//            //wp.SenderUserID = SenderUserID;
//            //wp.SendTime = DateTime.Now;
//            //wp.RecvRole = (instance == null ? 0 : MyWFHelpr.GetWorkflowRecvRole(instanceId, EventName).GetInt());

//            //wp.Create();
//        }

//        public void UpdateProcess(Guid instanceId, string Sugestion)
//        {
//            //db._.Update(dbo.V_LastWorkProcess)
//            //    .AddColumn(dbo.V_LastWorkProcess.Sugestion, Sugestion)
//            //    .Where(dbo.V_LastWorkProcess.WfID == instanceId)
//            //    .Execute();
//        }
//    }
//}
