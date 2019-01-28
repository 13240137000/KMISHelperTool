using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KMISHelper.DBScript;

namespace KMISHelper.HelpGlobal
{
    class InitObject
    {

        private static ScriptService ss;
        public static ScriptService ScriptServiceInstance(){
                if (ss == null)
                {
                    ss = new ScriptService();
                }
                return ss;
        }



    }
}
