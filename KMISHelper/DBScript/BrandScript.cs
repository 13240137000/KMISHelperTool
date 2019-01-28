using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMISHelper.DBScript
{
    partial class ScriptService
    {
        public string GetBrandListById = "SELECT * FROM acad_brand WHERE brand_id = '{0}'";
    }
}
