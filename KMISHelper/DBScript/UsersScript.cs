using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMISHelper.DBScript
{
    public partial class ScriptService
    {
        public string GetRoleListByTitle = "select * from upms_role where title = '{0}'";

        public string GetKindergartenListByName = "select * from acad_kindergarten where brand_id = '{0}' and kindergarten_nm = '{1}'";

        public string UserKindergartenInsert = "insert into upms_user_kindergarten(user_id,kindergarten_id) values ('{0}','{1}')";

    }
}