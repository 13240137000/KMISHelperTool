using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMISHelper.DBScript
{
    partial class ScriptService
    {
        public string getAcadYear = "SELECT acad_year_id,acad_title FROM acad_year";

        public string GetAcadYearByTitle = "SELECT acad_year_id,acad_title FROM acad_year WHERE acad_title = '{0}'";

    }
}
