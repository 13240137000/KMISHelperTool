using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KMISHelper.DBHelper;
using KMISHelper.DBScript;

namespace KMISHelper.HelpGlobal
{
    public enum SysLogType
    {
        INFO,
        WARNING,
        ERROR
    }

    public class SysInfo{
        public string message { get; set; }

        public SysLogType type { get; set; }

        public string moduleName { get; set; }

    }


    public class SysLog{

        public static bool Insert(SysInfo logInfo) {

            var result = false;
            var sql = string.Empty;

            try
            {

                //sql = string.Format()

            }
            catch (Exception)
            {

                throw;
            }
            finally {

            }

            return result;

        }

    }
}
