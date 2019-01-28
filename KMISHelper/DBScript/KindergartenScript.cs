using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMISHelper.DBScript
{
    partial class ScriptService
    {
        public string GetKindergartenListById = "SELECT * FROM acad_kindergarten WHERE kindergarten_id = '{0}'";
    }
}
