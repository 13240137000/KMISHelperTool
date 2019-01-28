using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using KMISHelper.DBHelper;
using KMISHelper.DBScript;

namespace KMISHelper.HelpGlobal
{
    public enum SysLogType
    {
        INFO = 0,
        WARNING = 1,
        ERROR = 2
    }

    public class SysLogInfo{
        public string message { get; set; }

        public SysLogType type { get; set; }

        public string moduleName { get; set; }

        public SysLogInfo(string _message, SysLogType _type, string _moduleName) {
            message = _message;
            type = _type;
            moduleName = _moduleName;
        }

    }


    public class SysLog{

        public static void Insert(SysLogInfo Info) {
            var sql = string.Empty;

            try
            {

                sql = string.Format(InitObject.GetScriptServiceInstance().SysLogInsert, @Info.message,(int)Info.type, Info.moduleName, BznsBase.KindergartenId);

                int QueryValue = MySqlHelper.ExecuteNonQuery(BznsBase.GetConnectionString, BznsBase.GetCommandType, sql, null);

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            finally {

            }

        }

        public static bool Export(SysLogType type) {

            var sql = string.Empty;
            var FilePath = string.Empty;
            var dt = new DataTable();
            var result = false;

            try
            {

                switch ((int)type) {
                    case 0:
                        FilePath = ConfigurationManager.AppSettings["InfoLogPath"];
                        break;
                    case 1:
                        FilePath = ConfigurationManager.AppSettings["WarningLogPath"];
                        break;
                    default:
                        FilePath = ConfigurationManager.AppSettings["ErrorLogPath"];
                        break;
                }

                sql = string.Format(InitObject.GetScriptServiceInstance().SysLogList, (int)type, BznsBase.KindergartenId);
                dt = DBHelper.MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, sql, null).Tables[0];
                ExcelHelper.ExcelHelper.DataTableToExcel(dt, string.Empty, FilePath);

                result = true;

            }
            catch (Exception ex)
            {

                SysLog.Insert(new SysLogInfo(ex.Message, SysLogType.ERROR, "System Log"));
            }
            finally {
                
            }

            return result;
        }

    }
}
