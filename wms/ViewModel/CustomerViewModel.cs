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
    public class CustomerViewModel : NotifyBase
    {
        public CustomerModel CustomerModel { get; set; } = new CustomerModel();

        public CommandBase SaveCommand { get; set; }


        private DataSet _supplierDataSet;
        public DataSet CustomerDataSet
        {
            get { return _supplierDataSet; }
            set
            {
                _supplierDataSet = value;
                this.DoNotify();
            }
        }

        public CustomerViewModel()
        {
            //初始化操作
            CustomerDataSet = new DataSet();
            initCustomer();
            //保存命令
            this.SaveCommand = new CommandBase();
            this.SaveCommand.DoExecute = new Action<object>(Save);
            this.SaveCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });
        }

        private void initCustomer()
        {
            CustomerDataSet.Tables.Add(ServerDataAccess.GetInstance().GetCustomer());
        }
        public void Save(object obj)
        {
            ServerDataAccess.GetInstance().UpdateCustomer(CustomerDataSet.Tables[0]);
        }
    }
}
