using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wms.Common;
using wms.DataAccess;
using wms.Model;

namespace wms.ViewModel
{
    public class EmployeeViewModel : NotifyBase
    {
        public EmployeeModel EmployeeModel { get; set; } = new EmployeeModel();
        public DepartModel DepartModel { get; set; } = new DepartModel();
        public CommandBase SaveCommand { get; set; }
        public CommandBase FirstPageCommand { get; set; }
        public CommandBase PrePageCommand { get; set; }
        public CommandBase NextPageCommand { get; set; }
        public CommandBase LastPageCommand { get; set; }
        public CommandBase InsertCommand { get; set; }
        public CommandBase DeleteCommand { get; set; }

        private int _position = 0;
        public int Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                //处理_position--越界问题，让positon指向第一条记录
                if (_position < 0)
                    _position = 0;
                //处理_position++的越界问题，让_position指向最后一条记录
                if (_position >= EmployeeDataSet.Tables[0].Rows.Count)
                    _position = EmployeeDataSet.Tables[0].Rows.Count - 1;
                initEmployee(Position);//数据表当前行数据属性
                initLogin();
            }
        }
        private DataSet _employeeDataSet;
        public DataSet EmployeeDataSet
        {
            get { return _employeeDataSet; }
            set
            {
                _employeeDataSet = value;
                this.DoNotify();
            }
        }
        public EmployeeViewModel()
        {
            EmployeeDataSet = new DataSet();
            initEmployee(Position);
            initLogin();
            initDepart();
            //首页命令
            this.FirstPageCommand = new CommandBase();
            this.FirstPageCommand.DoExecute = new Action<object>(FirstPage);
            this.FirstPageCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });
            //上一页命令
            this.PrePageCommand = new CommandBase();
            this.PrePageCommand.DoExecute = new Action<object>(PrePage);
            this.PrePageCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });
            //下一页命令
            this.NextPageCommand = new CommandBase();
            this.NextPageCommand.DoExecute = new Action<object>(NextPage);
            this.NextPageCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });
            //末页命令
            this.LastPageCommand = new CommandBase();
            this.LastPageCommand.DoExecute = new Action<object>(LastPage);
            this.LastPageCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });
            //新增命令
            this.InsertCommand = new CommandBase();
            this.InsertCommand.DoExecute = new Action<object>(Insert);
            this.InsertCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });
            //删除命令
            this.DeleteCommand = new CommandBase();
            this.DeleteCommand.DoExecute = new Action<object>(Delete);
            this.DeleteCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });
            //保存命令
            this.SaveCommand = new CommandBase();
            this.SaveCommand.DoExecute = new Action<object>(Save);
            this.SaveCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });
        }

        private void initEmployee(int Position)
        {
            EmployeeDataSet.Tables.Add(ServerDataAccess.GetInstance().GetEmployee());
            EmployeeModel.EmployeeId = EmployeeDataSet.Tables[0].Rows[Position]["employee_id"].ToString();
            DepartModel.DepartId = EmployeeDataSet.Tables[0].Rows[Position]["depart_id"].ToString();
            EmployeeModel.EmployeeName = EmployeeDataSet.Tables[0].Rows[Position]["employee_name"].ToString();
            EmployeeModel.EmployeePhone = EmployeeDataSet.Tables[0].Rows[Position]["employee_phone"].ToString();
            EmployeeModel.EmployeeSex = (int)EmployeeDataSet.Tables[0].Rows[Position]["employee_sex"];
            EmployeeModel.EmployeeEducation = EmployeeDataSet.Tables[0].Rows[Position]["employee_education"].ToString();
            EmployeeModel.EmployeeAddress = EmployeeDataSet.Tables[0].Rows[Position]["employee_address"].ToString();
            EmployeeModel.EmployeeEmail = EmployeeDataSet.Tables[0].Rows[Position]["employee_email"].ToString();
            EmployeeModel.EmployeeRemark = EmployeeDataSet.Tables[0].Rows[Position]["employee_remark"].ToString();

        }
        private void initLogin()
        {
            EmployeeDataSet.Tables.Add(ServerDataAccess.GetInstance().GetLogin());
            EmployeeDataSet.Tables[1].DefaultView.RowFilter = "员工id='" + EmployeeModel.EmployeeId + "'";
        }
        private void initDepart()
        {
            EmployeeDataSet.Tables.Add(ServerDataAccess.GetInstance().GetDepart());
        }
        public void FirstPage(object obj)
        {
            Position = 0;
        }
        public void PrePage(object obj)
        {
            Position--;
        }
        public void NextPage(object obj)
        {
            Position++;
        }
        public void LastPage(object obj)
        {
            Position = EmployeeDataSet.Tables[0].Rows.Count - 1;
        }

        public void Insert(object obj)
        {
            DataRow tr = EmployeeDataSet.Tables[0].NewRow();
            tr["employee_sex"] = 1;
            EmployeeDataSet.Tables[0].Rows.Add(tr);
            Position = EmployeeDataSet.Tables[0].Rows.Count;
        }
        public void Delete(object obj)
        {
            if (Position > 0)
            {
                EmployeeDataSet.Tables[0].Rows.RemoveAt(Position);
                Position--;
            }
        }
        public void Save(object obj)
        {
            EmployeeDataSet.Tables[0].Rows[Position]["employee_id"] = EmployeeModel.EmployeeId;
            EmployeeDataSet.Tables[0].Rows[Position]["depart_id"] = DepartModel.DepartId;
            EmployeeDataSet.Tables[0].Rows[Position]["employee_name"] = EmployeeModel.EmployeeName;
            EmployeeDataSet.Tables[0].Rows[Position]["employee_phone"] = EmployeeModel.EmployeePhone;
            EmployeeDataSet.Tables[0].Rows[Position]["employee_sex"] = EmployeeModel.EmployeeSex;
            EmployeeDataSet.Tables[0].Rows[Position]["employee_education"] = EmployeeModel.EmployeeEducation;
            EmployeeDataSet.Tables[0].Rows[Position]["employee_address"] = EmployeeModel.EmployeeAddress;
            EmployeeDataSet.Tables[0].Rows[Position]["employee_email"] = EmployeeModel.EmployeeEmail;
            EmployeeDataSet.Tables[0].Rows[Position]["employee_remark"] = EmployeeModel.EmployeeRemark;
            ServerDataAccess.GetInstance().UpdateEmployee(EmployeeDataSet.Tables[0]);

            ServerDataAccess.GetInstance().UpdateLogin(EmployeeDataSet.Tables[1]);
        }
    }
}
