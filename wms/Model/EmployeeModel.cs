using wms.Common;

namespace wms.Model
{
    public class EmployeeModel : NotifyBase
    {
        private string _employeeId;

        public string EmployeeId
        {
            get { return _employeeId; }
            set { _employeeId = value; this.DoNotify(); }
        }
        private string _employeeName;

        public string EmployeeName
        {
            get { return _employeeName; }
            set { _employeeName = value; this.DoNotify(); }
        }

        private string _employeePhone;

        public string EmployeePhone
        {
            get { return _employeePhone; }
            set { _employeePhone = value; this.DoNotify(); }
        }

        private int _employeeSex;

        public int EmployeeSex
        {
            get { return _employeeSex; }
            set { _employeeSex = value; this.DoNotify(); }
        }

        private string _employeeEducation;
        public string EmployeeEducation
        {
            get { return _employeeEducation; }
            set { _employeeEducation = value; this.DoNotify(); }
        }

        private string _employeeAddress;
        public string EmployeeAddress
        {
            get { return _employeeAddress; }
            set { _employeeAddress = value; this.DoNotify(); }
        }

        private string _employeeEmail;
        public string EmployeeEmail
        {
            get { return _employeeEmail; }
            set { _employeeEmail = value; this.DoNotify(); }
        }

        private string _employeeRemark;
        public string EmployeeRemark
        {
            get { return _employeeRemark; }
            set { _employeeRemark = value; this.DoNotify(); }
        }

    }
}
