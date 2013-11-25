//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Workflow.Activities;
//using System.Workflow.ComponentModel;

//namespace MyWF
//{
//    [ExternalDataExchange]
//    public interface IInvite
//    {
//        event EventHandler<ExternalDataEventArgs> Submit;
//        event EventHandler<ExternalDataEventArgs> PFApprove;
//        event EventHandler<ExternalDataEventArgs> PFNotApprove;
//        event EventHandler<ExternalDataEventArgs> MainDeptApprove;
//        event EventHandler<ExternalDataEventArgs> MainDeptNotApprove;
//        event EventHandler<ExternalDataEventArgs> Confirm;
//    }


//    [Serializable]
//    public class InviteService : WfService, IInvite
//    {
//        [Serializable]
//        public enum Events
//        {
//            Submit,
//            PFApprove,
//            PFNotApprove,
//            MainDeptApprove,
//            MainDeptNotApprove,
//            Confirm,
//        }


//        public event EventHandler<ExternalDataEventArgs> Submit
//        {
//            add { this._EventList.Add(Events.Submit.ToString(), value); }
//            remove { this._EventList.Remove(Events.Submit.ToString()); }
//        }

//        public event EventHandler<ExternalDataEventArgs> PFApprove
//        {
//            add { this._EventList.Add(Events.PFApprove.ToString(), value); }
//            remove { this._EventList.Remove(Events.PFApprove.ToString()); }
//        }

//        public event EventHandler<ExternalDataEventArgs> PFNotApprove
//        {
//            add { this._EventList.Add(Events.PFNotApprove.ToString(), value); }
//            remove { this._EventList.Remove(Events.PFNotApprove.ToString()); }
//        }

//        public event EventHandler<ExternalDataEventArgs> MainDeptApprove
//        {
//            add { this._EventList.Add(Events.MainDeptApprove.ToString(), value); }
//            remove { this._EventList.Remove(Events.MainDeptApprove.ToString()); }
//        }

//        public event EventHandler<ExternalDataEventArgs> MainDeptNotApprove
//        {
//            add { this._EventList.Add(Events.MainDeptNotApprove.ToString(), value); }
//            remove { this._EventList.Remove(Events.MainDeptNotApprove.ToString()); }
//        }

//        public event EventHandler<ExternalDataEventArgs> Confirm
//        {
//            add { this._EventList.Add(Events.Confirm.ToString(), value); }
//            remove { this._EventList.Remove(Events.Confirm.ToString()); }
//        }
//    }
//}
