//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Common;
//using System.Workflow.ComponentModel;
//using System.Workflow.Runtime;
//using System.Workflow.Runtime.Hosting;
//using Microsoft.Practices.EnterpriseLibrary.Data;

//namespace Ganji.OA.Workflow
//{
//    public class MysqlWorkflowPersistenceService : WorkflowPersistenceService
//    {
//        public MysqlWorkflowPersistenceService()
//            : base()
//        { }

//        public List GetAllWorkflows()
//        {
//            List result = new List();
//            Database db = DatabaseFactory.CreateDatabase("OAData");
//            DbCommand command = db.GetSqlStringCommand("SELECT id FROM workflow_state");
//            DbDataReader reader = command.ExecuteReader();
//            while (reader.Read())
//            {
//                result.Add(new Guid((string)reader["id"]));
//            }
//            reader.Close();
//            return result;
//        }

//        protected override Activity LoadCompletedContextActivity(Guid scopeId, Activity outerActivity)
//        {
//            Guid instanceId = WorkflowEnvironment.WorkflowInstanceId;
//            Activity activity = Deserialize(instanceId, scopeId, outerActivity);
//            return activity;
//        }

//        protected override Activity LoadWorkflowInstanceState(Guid instanceId)
//        {
//            Activity activity = Deserialize(instanceId, Guid.Empty, null);
//            return activity;
//        }

//        protected override void SaveCompletedContextActivity(Activity activity)
//        {
//            Guid instanceId = WorkflowEnvironment.WorkflowInstanceId;
//            Guid contextId = (Guid)activity.GetValue(Activity.ActivityContextGuidProperty);
//            Serialize(instanceId, contextId, activity);
//        }

//        protected override void SaveWorkflowInstanceState(Activity rootActivity, bool unlock)
//        {
//            WorkflowStatus status = GetWorkflowStatus(rootActivity);
//            Guid instanceId = WorkflowEnvironment.WorkflowInstanceId;

//            if (status == WorkflowStatus.Terminated || status == WorkflowStatus.Completed)
//            {
//                DeleteWorkflow(instanceId);
//            }
//            else
//            {
//                Serialize(instanceId, Guid.Empty, rootActivity);
//            }
//        }

//        protected override bool UnloadOnIdle(Activity activity)
//        {
//            return true;
//        }

//        protected override void UnlockWorkflowInstanceState(Activity rootActivity)
//        {
//        }

//        private void Serialize(Guid instanceId, Guid contextId, Activity activity)
//        {
//            byte[] state = WorkflowPersistenceService.GetDefaultSerializedForm(activity);
//            Database db = DatabaseFactory.CreateDatabase("OAData");
//            String sql_del = "DELETE FROM workflow_state WHERE id=@id";
//            DbCommand command_del = db.GetSqlStringCommand(sql_del);
//            db.AddInParameter(command_del, "@id", DbType.String, instanceId.ToString());

//            String sql = "INSERT INTO workflow_state (`id`, `state`, `status`) VALUES (@id, @state, @status)";
//            DbCommand command = db.GetSqlStringCommand(sql);
//            db.AddInParameter(command, "@id", DbType.String, instanceId.ToString());
//            db.AddInParameter(command, "@state", DbType.Binary, state);
//            db.AddInParameter(command, "@status", DbType.Byte, 0);

//            using (DbConnection conn = db.CreateConnection())
//            {
//                conn.Open();
//                db.ExecuteNonQuery(command_del);
//                db.ExecuteNonQuery(command);
//                conn.Close();
//            }
//        }

//        private Activity Deserialize(Guid instanceId, Guid contextId, Activity outerActivity)
//        {
//            Database db = DatabaseFactory.CreateDatabase("OAData");
//            string sql = "SELECT * FROM workflow_state WHERE id='" + instanceId.ToString() + "'";
//            Activity activity = null;
//            using (DbDataReader reader = db.ExecuteReader(CommandType.Text, sql) as DbDataReader)
//            {
//                if (reader.HasRows)
//                {
//                    reader.Read();
//                    byte[] state = (byte[])reader["state"];
//                    reader.Close();
//                    activity = WorkflowPersistenceService.RestoreFromDefaultSerializedForm(state, outerActivity);
//                }
//            }
//            return activity;
//        }

//        private void DeleteWorkflow(Guid instanceId)
//        {
//            Database db = DatabaseFactory.CreateDatabase("OAData");
//            string sql = "DELETE FROM workflow_state WHERE id=@id";
//            DbCommand command = db.GetSqlStringCommand(sql);
//            db.AddInParameter(command, "@id", DbType.String, instanceId.ToString());
//            using (DbConnection conn = db.CreateConnection())
//            {
//                conn.Open();
//                command.ExecuteNonQuery();
//                conn.Close();
//            }
//        }
//    }
//}