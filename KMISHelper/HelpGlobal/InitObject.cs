using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KMISHelper.DBScript;
using KMISHelper.DBHelper;
using System.Data;
using System.Configuration;

namespace KMISHelper.HelpGlobal
{

    public struct ClassInfo{
        public string ClassId { get; set; }
        public string CalendarId { get; set; }
        public string ClassType { get; set; }
        public string ClassName { get; set; }
        public string YearId { get; set; }
}

    public struct YearInfo {
        public string YearID { get; set; }
        public string Title { get; set; }
    }

    public struct TermInfo {

        public string TermID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string CalID { get; set; }

        public TermInfo(string _TermID, DateTime _StartDate, DateTime _EndDate, string _CalID) {
            TermID = _TermID;
            StartDate = _StartDate;
            EndDate = _EndDate;
            CalID = _CalID;
        }

    }

    public struct MonthInfo {

        public int Month { get; set; }
        public int StudyDays { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public MonthInfo(int _Month,int _StudyDays, DateTime _StartDate,DateTime _EndDate) {
            Month = _Month;
            StudyDays = _StudyDays;
            StartDate = _StartDate;
            EndDate = _EndDate;
        }

    }

    public struct PaymentInfo {
        public string PaymentNo { get; set; }
        public string ReceiptNo { get; set; }
    }

    public struct BrandInfo {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public struct KindergartenInfo {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public struct RoleInfo {
        public string RoleId { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }

    class InitObject
    {

        private static ScriptService ss;
        public static ScriptService GetScriptServiceInstance(){
                if (ss == null)
                {
                    ss = new ScriptService();
                }
                return ss;
        }

        public static string GetBillNo(string StartDate,string Subject) {
            var result = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(Subject))
                {
                    Subject = "00";
                }

                var DateCode = Convert.ToDateTime(StartDate).ToString("yyMM");
                var SubjectCode = Subject.Substring(Subject.Length - 2);
                var TimeCode = DateTime.Now.Ticks.ToString();
                TimeCode = TimeCode.Substring(TimeCode.Length - 6);
                result = string.Concat(SubjectCode, DateCode, TimeCode);
            }
            catch (Exception)
            {

                throw;
            }
            finally {

            }
            return result;
        }

        public static string GetUUID() {
            return System.Guid.NewGuid().ToString().Replace("-","");
        }

        public static string GetSysDate(bool Flag = false,bool Lng = false)
        {
            var result = string.Empty;

            if (Flag)
            {
                if (Lng) {
                    result = DateTime.Now.ToString("yyyyMMddhhmmssffffff");

                } else {
                    result = DateTime.Now.ToString("yyyyMMddhhmmss");
                }
                

            }
            else {
                result = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                //result = new DateTime(2019,3,30,0,0,0).ToString("yyyy-MM-dd hh:mm:ss");
            }

            return result;
        }

        public static DataTable GetAcadYear() {
            var result = new DataTable();
            var sql = string.Empty;

            try
            {

                sql = GetScriptServiceInstance().getAcadYear;
                result = MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, sql, null).Tables[0];

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {

            }
            return result;
        }


        public static RoleInfo GetRoleInfoByTitle(string Title)
        {
            var Info = new RoleInfo();
            var Sql = string.Empty;
            var dt = new DataTable();
            try
            {
                Sql = string.Format(GetScriptServiceInstance().GetRoleListByTitle, Title.Trim());
                dt = MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, Sql, null).Tables[0];

                if (dt.Rows.Count > 0) {
                    Info.RoleId = dt.Rows[0]["role_id"].ToString();
                    Info.Name = dt.Rows[0]["name"].ToString();
                    Info.Title = dt.Rows[0]["title"].ToString();
                    Info.Description = dt.Rows[0]["description"].ToString();
                }

            }
            catch (Exception)
            {
                throw;
            }
            finally {
                dt = null;
            }
            return Info;
        }

        public static KindergartenInfo GetKindergartenInfoByName(string name) {
            var Info  = new KindergartenInfo();
            var Sql = string.Empty;
            var dt = new DataTable();
            try
            {
                Sql = string.Format(GetScriptServiceInstance().GetKindergartenListByName, BznsBase.BrandID,name);
                dt = MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, Sql, null).Tables[0];

                if (dt.Rows.Count > 0)
                {
                    Info.Id = dt.Rows[0]["kindergarten_id"].ToString();
                    Info.Name = dt.Rows[0]["kindergarten_nm"].ToString();
                }

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                dt = null;
            }
            return Info;
        }

        public static YearInfo GetYearByTitle(string Title)
        {
            var result = new YearInfo();
            var sql = string.Empty;
            var dt = new DataTable();

            try
            {
                sql = string.Format(GetScriptServiceInstance().GetAcadYearByTitle, Title);
                dt = MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, sql, null).Tables[0];
                if (dt.Rows.Count == 1)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        result.YearID = dr["acad_year_id"].ToString();
                        result.Title = dr["acad_title"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                SysLog.Insert(new SysLogInfo(ex.Message.ToString(), SysLogType.ERROR, "Get Year ID"));
            }
            finally
            {
                dt = null;
            }

            return result;
        }

        public static string GetNationCodeByName(string NationName) {
            var result = string.Empty;
            var sql = string.Empty;
            var dt = new DataTable();

            try
            {
                sql = string.Format( GetScriptServiceInstance().getNationCode,NationName);
                dt = MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, sql, null).Tables[0];
                if (dt.Rows.Count == 1) {
                    foreach (DataRow dr in dt.Rows) {
                        result = dr["nation_code"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                SysLog.Insert(new SysLogInfo(ex.Message.ToString(), SysLogType.ERROR, "Get Nation Code"));
            }
            finally {
                dt = null;    
            }

            return result;
        }

        public static ClassInfo GetClassTypeByClassId(string ClassId) {

            var info = new ClassInfo();
            var sql = string.Empty;

            try
            {
                sql = string.Format(GetScriptServiceInstance().GetClassListById, ClassId);
                var dt = MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, sql, null).Tables[0];
                if (dt.Rows.Count == 1)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        info.ClassId = dr["class_id"].ToString();
                        info.ClassName = dr["class_nm"].ToString();
                        info.CalendarId = dr["class_type"].ToString();
                        info.ClassType = dr["school_calendar_id"].ToString();
                        info.YearId = dr["acad_year_id"].ToString();
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally {

            }

            return info;

        }

        public static string GetDelayClassStatus(string Status) {
            var result = string.Empty;
            switch (Status)
            {
                case "待开启":
                    result = "DELAYCLASS01";
                    break;
                case "进行中":
                    result = "DELAYCLASS02";
                    break;
                case "已关闭":
                    result = "DELAYCLASS03";
                    break;
                case "已废弃":
                    result = "DELAYCLASS03";
                    break; 
                default:
                    result = "DELAYCLASS02";
                    break;
            }
            return result;
        }

        public static string GetDelayClassType(string Type) {
            var result = string.Empty;
            switch (Type)
            {
                case "延时课":
                    result = "DELAYCLASS";
                    break;
                case "亲子班":
                    result = "PARENTCLASS";
                    break;
                case "课外课":
                    result = "EXTRACLASS";
                    break;
                case "夏令营":
                    result = "SUMMERCAMP";
                    break;
                case "冬令营":
                    result = "WINTERCAMP";
                    break;
                default:
                    result = "EXTRACLASS";
                    break;
            }
            return result;
        }

        public static string GetDelayChargeType(string Type)
        {
            var result = string.Empty;
            switch (Type)
            {
                case "按学期收费":
                    result = "DELAYCHARGETYPE01";
                    break;
                case "按课时收费":
                    result = "DELAYCHARGETYPE02";
                    break;
                case "一对一收费":
                    result = "DELAYCHARGETYPE03";
                    break;
                default:
                    result = "DELAYCHARGETYPE01";
                    break;
            }
            return result;
        }

        public static string GetPaymentStatus(string Status)
        {
            var result = string.Empty;
            switch (Status)
            {
                case "未付款":
                    result = "PAYMENTSTATE01";
                    break;
                case "已付款":
                    result = "PAYMENTSTATE02";
                    break;
                case "部分付款":
                    result = "PAYMENTSTATE03";
                    break;
                default:
                    result = "PAYMENTSTATE01";
                    break;
            }
            return result;
        }

        public static string GetTardeType(string Type)
        {
            var result = string.Empty;
            switch (Type)
            {
                case "充入":
                    result = "TRADETYPE01";
                    break;
                case "扣减":
                    result = "TRADETYPE02";
                    break;
                case "提现":
                    result = "TRADETYPE03";
                    break;
                case "退款":
                    result = "TRADETYPE04";
                    break;
                case "行政扣款":
                    result = "TRADETYPE05";
                    break;
                case "结转":
                    result = "TRADETYPE06";
                    break;
                case "规则扣款":
                    result = "TRADETYPE07";
                    break;
                case "冻结":
                    result = "TRADETYPE08";
                    break;
                default:
                    result = "TRADETYPE01";
                    break;
            }
            return result;
        }

        public static string GetPaymentType(string Type)
        {
            var result = string.Empty;
            switch (Type)
            {
                case "现金":
                    result = "PAYMENTTYPE01";
                    break;
                case "刷卡":
                    result = "PAYMENTTYPE02";
                    break;
                case "微信":
                    result = "PAYMENTTYPE03";
                    break;
                case "支付宝":
                    result = "PAYMENTTYPE04";
                    break;
                case "账户划转":
                    result = "PAYMENTTYPE05";
                    break;
                case "银行汇款":
                    result = "PAYMENTTYPE06";
                    break;
                case "结余抵扣":
                    result = "PAYMENTTYPE07";
                    break;
                default:
                    result = "PAYMENTTYPE01";
                    break;
            }
            return result;
        }

        public static string GetClassTypeByName(string ClassTypeName) {
            var result = string.Empty;
            switch (ClassTypeName) {
                case "国际":
                    result = "CLASSTYPE01";
                    break;
                case "双语":
                    result = "CLASSTYPE02";
                    break;
                case "普惠":
                    result = "CLASSTYPE03";
                     break;
                case "IB国际":
                    result = "CLASSTYPE04";
                    break;
                default:
                    result = "CLASSTYPE01";
                    break;
            }
            return result;
        }

        public static string GetEducationCodeByName(string EducationName) {
            var result = string.Empty;
            switch (EducationName)
            {
                case "小学":
                    result = "EDUCATIONTYPE01";
                    break;
                case "初中":
                    result = "EDUCATIONTYPE02";
                    break;
                case "高中(中专)":
                    result = "EDUCATIONTYPE03";
                    break;
                case "大专":
                    result = "EDUCATIONTYPE04";
                    break;
                case "本科":
                    result = "EDUCATIONTYPE05";
                    break;
                case "研究生及以上":
                    result = "EDUCATIONTYPE06";
                    break;
                default:
                    result = "EDUCATIONTYPE01";
                    break;
            }
            return result;
        }

        public static string GetTeacherLevel(string LevelName)
        {
            var result = string.Empty;
            switch (LevelName)
            {
                case "教师":
                    result = "TEACHERLEVEL01";
                    break;
                case "助教":
                    result = "TEACHERLEVEL02";
                    break;
                case "保育员":
                    result = "TEACHERLEVEL03";
                    break;
                default:
                    result = "TEACHERLEVEL01";
                    break;
            }
            return result;
        }

        public static string GetGradeTypeByName(string GradeTypeName) {
            var result = string.Empty;

            switch (GradeTypeName)
            {
                case "托班":
                    result = "GRADETYPE02";
                    break;
                case "小班":
                    result = "GRADETYPE03";
                    break;
                case "中班":
                    result = "GRADETYPE04";
                    break;
                case "大班":
                    result = "GRADETYPE05";
                    break;
                default:
                    result = "GRADETYPE02";
                    break;
            }

            return result;
        }

        public static string GetIdType(string IdName)
        {
            var result = string.Empty;

            switch (IdName)
            {
                case "居民身份证":
                    result = "CREDENTIALSTYPE01";
                    break;
                case "户口本":
                    result = "CREDENTIALSTYPE02";
                    break;
                case "护照":
                    result = "CREDENTIALSTYPE03";
                    break;
                case "香港特区护照/身份证明":
                    result = "CREDENTIALSTYPE04";
                    break;
                case "澳门特区护照/身份证明":
                    result = "CREDENTIALSTYPE05";
                    break;
                case "台湾特区护照/身份证明":
                    result = "CREDENTIALSTYPE06";
                    break;
                case "境外永久居住证":
                    result = "CREDENTIALSTYPE07";
                    break;
                case "其他":
                    result = "CREDENTIALSTYPE08";
                    break;
                case "无证明":
                    result = "CREDENTIALSTYPE09";
                    break;
                default:
                    result = "CREDENTIALSTYPE09";
                    break;
            }

            return result;
        }

        public static string GetSexByName(string Name) {
            return Name == "男" ? "M" : "F";
        }

        public static string GetStatusByName(string StatusName) {
            var result = string.Empty;

            switch (StatusName) {
                case "在校未分班":
                    result = "STUSTATUS01";
                    break;
                case "退园生":
                    result = "STUSTATUS03";
                    break;
                case "毕业":
                    result = "STUSTATUS04";
                    break;
                case "放弃入园":
                    result = "STUSTATUS07";
                    break;
                case "在校已分班":
                    result = "STUSTATUS09";
                    break;
                default:
                    result = "STUSTATUS09";
                    break;
            }

            return result;
        }

        public static string GetRelationshipCodeByName(string Name)
        {
            var result = string.Empty;

            switch (Name)
            {
                case "父亲":
                    result = "FATHER";
                    break;
                case "爸爸":
                    result = "FATHER";
                    break;
                case "母亲":
                    result = "MOTHER";
                    break;
                case "妈妈":
                    result = "MOTHER";
                    break;
                case "爷爷":
                    result = "PATERNALGRANDFATHER";
                    break;
                case "奶奶":
                    result = "PATERNALGRANDMOTHER";
                    break;
                case "姥爷":
                    result = "MATERNALGRANDFATHER";
                    break;
                case "姥姥":
                    result = "MATERNALGRANDMOTHER";
                    break;
                case "其他":
                    result = "OTHER";
                    break;
                case "叔叔":
                    result = "PATERNALUNCLE";
                    break;
                case "阿姨":
                    result = "MATERNALAUNT";
                    break;
                case "姑姑":
                    result = "PATERNALAUNT";
                    break;
                case "姑父":
                    result = "UNCLE-IN-LAW";
                    break;
                default:
                    result = "OTHER";
                    break;
            }

            return result;
        }

        public static string GetStudentClassTypeByName(string Name)
        {
            var result = string.Empty;

            switch (Name)
            {
                case "在校生":
                    result = "STUCLASSTYPE01";
                    break;
                case "退园审批中":
                    result = "STUCLASSTYPE02";
                    break;
                case "休学审批中":
                    result = "STUCLASSTYPE03";
                    break;
                case "退园确认":
                    result = "STUCLASSTYPE04";
                    break;
                case "退园拒绝":
                    result = "STUCLASSTYPE05";
                    break;
                case "休学确认":
                    result = "STUCLASSTYPE06";
                    break;
                case "休学拒绝":
                    result = "STUCLASSTYPE07";
                    break;
                case "毕业":
                    result = "STUCLASSTYPE08";
                    break;
                case "在读":
                    result = "STUCLASSTYPE09";
                    break;
                default:
                    result = "STUCLASSTYPE09";
                    break;
            }

            return result;
        }

        public static string GetStatusClassStatusByName(string Name)
        {
            var result = string.Empty;

            switch (Name)
            {
                case "在读":
                    result = "STUCLASSSTATUS01";
                    break;
                case "已升班":
                    result = "STUCLASSSTATUS02";
                    break;
                case "新报":
                    result = "STUCLASSSTATUS03";
                    break;
                case "转出":
                    result = "STUCLASSSTATUS04";
                    break;
                case "转入":
                    result = "STUCLASSSTATUS05";
                    break;
                case "退班":
                    result = "STUCLASSSTATUS06";
                    break;
                case "毕业":
                    result = "STUCLASSSTATUS07";
                    break;
                case "升出":
                    result = "STUCLASSSTATUS08";
                    break;
                case "升入":
                    result = "STUCLASSSTATUS09";
                    break;
                default:
                    result = "STUCLASSSTATUS03";
                    break;
            }

            return result;
        }

        public static string GetPaymentModeByName(string Name) {
            var result = string.Empty;

            switch (Name) {
                case "学年":
                    result = "PAYMENTMODE01";
                    break;
                case "季度":
                    result = "PAYMENTMODE02";
                    break;
                case "学期":
                    result = "PAYMENTMODE03";
                    break;
                case "月":
                    result = "PAYMENTMODE04";
                    break;
                case "日":
                    result = "PAYMENTMODE05";
                    break;
                case "自定义":
                    result = "PAYMENTMODE06";
                    break;
                default:
                    result = "PAYMENTMODE03";
                    break;
            }

            return result;
        }

        public static string GetPaymentSubjectByName(string Name)
        {
            var result = string.Empty;

            switch (Name)
            {
                case "学费":
                    result = "PAYMENTSUBJECT01";
                    break;
                case "餐费":
                    result = "PAYMENTSUBJECT02";
                    break;
                case "物品费":
                    result = "PAYMENTSUBJECT03";
                    break;
                case "校车费":
                    result = "PAYMENTSUBJECT04";
                    break;
                case "预缴学费":
                    result = "PAYMENTSUBJECT05";
                    break;
                default:
                    result = "PAYMENTSUBJECT01";
                    break;
            }

            return result;
        }

        public static string GetDiscountTypeByName(string Name) {
            var result = string.Empty;

            switch (Name)
            {
                case "内部":
                    result = "DISCOUNTTYPE01";
                    break;
                case "外部":
                    result = "DISCOUNTTYPE02";
                    break;
                default:
                    result = "DISCOUNTTYPE01";
                    break;
            }

            return result;
        }

        public static Dictionary<string,MonthInfo> SplitMonth(DateTime StartDate, int Month, string CalID = "",string YearID = "") {

            var result = new Dictionary<string,MonthInfo>();

            try
            {

                for (var i = 1; i <= Month; i++)
                {
                    var dt = StartDate.AddMonths(i).AddDays(-1);
                    var FirstDate = FirstDayOfMonth(dt);
                    var LastDate = LastDayOfMonth(dt);
                    var Key = FirstDate.ToString("yyyy-MM");
                    var TotalStudy = TotalStudyOfMonth(FirstDate, LastDate, CalID, YearID);
                    var Value = new MonthInfo(i, TotalStudy, FirstDate,LastDate);
                    result.Add(Key, Value);
                }

                result.Add("summary", new MonthInfo(0, 0, StartDate, StartDate.AddMonths(Month).AddDays(-1)));
                
            }
            catch (Exception)
            {

                throw;
            }

            return result;
        }

        private static DateTime FirstDayOfMonth(DateTime datetime)
        {
            return datetime.AddDays(1 - datetime.Day);
        }

        private static DateTime LastDayOfMonth(DateTime datetime)
        {
            return datetime.AddDays(1 - datetime.Day).AddMonths(1).AddDays(-1);
        }

        public static List<TermInfo> GetTerm(string CalID) {
            List<TermInfo> terms = new List<TermInfo>();
            var sql = string.Empty;
            try
            {
                sql = string.Format(GetScriptServiceInstance().GetTerm,CalID);
                var dt = MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, sql, null).Tables[0];

                foreach (DataRow rows in dt.Rows) {
                    TermInfo TermInfo = new TermInfo(rows["term_id"].ToString(),Convert.ToDateTime(rows["term_begin_date"]), Convert.ToDateTime(rows["term_end_date"]), rows["school_calendar_id"].ToString());
                    terms.Add(TermInfo);
                }
                
            }
            catch (Exception)
            {

                throw;
            }
            return terms;
        }

        public static int TotalStudyOfMonth(DateTime StartDate,DateTime EndDate,string CalID,string YearID) {

            var dts = StartDate.AddDays(-1);
            var dte = EndDate.AddDays(-1);
            var sql = string.Empty;
            var holidays = new DataTable();
            var result = 0;
            

            try
            {

                sql = string.Format(GetScriptServiceInstance().GetHolidays,CalID,YearID);
                holidays = MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, sql, null).Tables[0];

                while (DateTime.Compare(dts, dte) <= 0)
                {

                    dts = dts.AddDays(1);
                    var Flag = false;
                    DataRow[] dr = holidays.Select("date_time='"+ dts.ToString("yyyy-MM-dd") +"'");

                    if (dr.Length > 0 && Convert.ToInt32(dr[0].ItemArray[1]) == 1) {
                        Flag = true;
                    }

                    if (!Flag && !IsWeeked(dts)) {
                        if (dr.Length == 0)
                        {
                            Flag = true;
                        }
                    }

                    if (Flag) {
                        result = result + 1;
                    }
                    
                }

            }
            catch (Exception ex)
            {

                throw;
            }

            return result;
        }

        public static bool IsWeeked(DateTime dt) {
            var result = false;
            var sql = string.Empty;

            try
            {

                int week = (int)dt.DayOfWeek;

                switch (week) {
                    case 6: 
                        result = true;
                        break;
                    case 0:
                        result = true;
                        break;
                }
            
            }
            catch (Exception)
            {

                throw;
            }

            return result;

        }

        

        public static decimal TotalBillMoney(string ptMoney,string tMoney,string disMoney,string mfMoney,string scMoney,List<string> ofMoneys,int Month,int scMonth = 0) {
            var sum = new decimal(0);
            try
            {
                if (tMoney == "0") {
                    tMoney = "";
                }

                if (!string.IsNullOrEmpty(ptMoney)) {
                    if (string.IsNullOrEmpty(tMoney))
                    {
                        sum = sum + Convert.ToDecimal(ptMoney);
                    }
                    else {
                        //sum = sum + Convert.ToDecimal(ptMoney);
                    }
                }

                if (!string.IsNullOrEmpty(disMoney)) {
                    sum = sum + Convert.ToDecimal(disMoney) * Month;
                }

                if (string.IsNullOrEmpty(disMoney) && !string.IsNullOrEmpty(tMoney)) {
                    sum = sum + Convert.ToDecimal(tMoney) * Month;
                }

                if (!string.IsNullOrEmpty(mfMoney)) {
                    sum = sum + Convert.ToDecimal(mfMoney) * Month;
                }

                if (!string.IsNullOrEmpty(scMoney))
                {
                    sum = sum + Convert.ToDecimal(scMoney) * scMonth;
                }

                foreach (var m in ofMoneys) {
                    sum = sum + Convert.ToDecimal(m);
                }

            }
            catch (Exception)
            {

                throw;
            }
            return sum;
        }

        public static bool SetConfigValue(string key, string value)
        {
            var result = false;
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (config.AppSettings.Settings[key] != null)
                    config.AppSettings.Settings[key].Value = value;
                else
                    config.AppSettings.Settings.Add(key, value);
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
                result = true;
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public static PaymentInfo GetPaymentInfo() {
            var Info = new PaymentInfo();
            try
            {
                Info.PaymentNo = GetTimeStamp(DateTime.Now);
                Info.ReceiptNo = string.Concat("C", GetBrandInfo().Code, GetKindergartenInfo().Code,DateTime.Now.ToString("yyyyMMdd"), GetTimeStamp(DateTime.Now).Substring(7));
            }
            catch (Exception)
            {
                throw;
            }
            return Info;
        }

        public static BrandInfo GetBrandInfo() {

            var Info = new BrandInfo();
            var Brand = new DataTable();
            string Sql = string.Empty;

            try
            {
                Sql = string.Format(InitObject.GetScriptServiceInstance().GetBrandListById, BznsBase.BrandID);
                Brand = MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, Sql, null).Tables[0];
                if (Brand.Rows.Count > 0)
                {
                    DataRow dr = Brand.Rows[0];
                    Info.Id = dr["Brand_ID"].ToString();
                    Info.Code = dr["Brand_Code"].ToString();
                    Info.Name = dr["Brand_Name"].ToString();
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally {
                Brand = null;
            }

            return Info;
        }

        public static KindergartenInfo GetKindergartenInfo()
        {

            var Info = new KindergartenInfo();
            var Kindergarten = new DataTable();
            string Sql = string.Empty;

            try
            {
                Sql = string.Format(InitObject.GetScriptServiceInstance().GetKindergartenListById, BznsBase.KindergartenId);
                Kindergarten = MySqlHelper.GetDataSet(BznsBase.GetConnectionString, BznsBase.GetCommandType, Sql, null).Tables[0];
                if (Kindergarten.Rows.Count > 0)
                {
                    DataRow dr = Kindergarten.Rows[0];
                    Info.Id = dr["kindergarten_id"].ToString();
                    Info.Code = dr["kindergarten_number"].ToString();
                    Info.Name = dr["kindergarten_nm"].ToString();
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally {
                Kindergarten = null;
            }

            return Info;
        }

        public static string GetTimeStamp(System.DateTime time)
        {
            long ts = ConvertDateTimeToInt(time);
            return ts.ToString();
        }

        public static long ConvertDateTimeToInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long t = (time.Ticks - startTime.Ticks) / 10000;       
            return t;
        }



    }

}
