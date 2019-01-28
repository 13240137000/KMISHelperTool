using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMISHelper.DBScript
{
    partial class ScriptService
    {
        public string SysLogInsert = "INSERT INTO sys_import_log(message,type,module_name,kindergarten_id) VALUES( '{0}', {1},'{2}','{3}')";

        public string SysLogList = "SELECT * FROM sys_import_log WHERE type = {0} AND kindergarten_id = '{1}'";

    }
}
