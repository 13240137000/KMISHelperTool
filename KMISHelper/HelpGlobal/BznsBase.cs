using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Configuration;

namespace KMISHelper.HelpGlobal
{
    public class BznsBase
    {

        public static readonly string GetConnectionString = ConfigurationManager.AppSettings["ConnectionString"].ToString();

        public static readonly CommandType GetCommandType = CommandType.Text;

        public static readonly string KindergartenId = ConfigurationManager.AppSettings["KindergartenId"].ToString();

        public static readonly string BrandID = ConfigurationManager.AppSettings["BrandID"].ToString();

        public static readonly string UserID = ConfigurationManager.AppSettings["UserID"].ToString();

        public static readonly string BrandUserID = ConfigurationManager.AppSettings["BrandUserID"].ToString();

        public static readonly string PaymentType = ConfigurationManager.AppSettings["PaymentType"].ToString();

        public static readonly string ImportClassPlanSheetName = ConfigurationManager.AppSettings["ImportClassPlanSheetName"].ToString();

        public static readonly string ImportClassSheetName = ConfigurationManager.AppSettings["ImportClassSheetName"].ToString();

        public static readonly string ImportStudentSheetName = ConfigurationManager.AppSettings["ImportStudentSheetName"].ToString();

        public static readonly string ImportTeacherSheetName = ConfigurationManager.AppSettings["ImportTeacherSheetName"].ToString();

        public static readonly string ImportFinanceSheetName = ConfigurationManager.AppSettings["ImportFinanceSheetName"].ToString();

        public static readonly string ImportInterestClassSheetName = ConfigurationManager.AppSettings["ImportInterestClassSheetName"].ToString();

        public static readonly string ImportInterestClassFinanceSheetName = ConfigurationManager.AppSettings["ImportInterestClassFinanceSheetName"].ToString();

        public static readonly string InitialDirectory = ConfigurationManager.AppSettings["InitialDirectory"].ToString();

        public static readonly string ImportUser = ConfigurationManager.AppSettings["ImportUser"].ToString();

        public static readonly bool EnableMappingField = ConfigurationManager.AppSettings["EnableMappingField"].ToString().ToUpper() == "YES"?true:false;

        public static bool IsAppend = ConfigurationManager.AppSettings["IsAppend"].ToString().ToUpper() == "YES" ? true : false;

        public static readonly string UserPassword = "664E4D8BACB1A3F87BE61A643A3D4332";

        public static readonly string UserPasswordSalt = "4bf78a6a875b47f7ab224ecdb4f74d95";

        public static readonly string ImportForShangHai = ConfigurationManager.AppSettings["ImportForShangHai"].ToString();

    }

}
