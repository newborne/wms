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
    public class SupplierViewModel: NotifyBase
    {
        public SupplierModel SupplierModel { get; set; } = new SupplierModel();
        
        public CommandBase SaveCommand { get; set; }


        private DataSet _supplierDataSet;
        public DataSet SupplierDataSet
        {
            get { return _supplierDataSet; }
            set
            {
                _supplierDataSet = value;
                this.DoNotify();
            }
        }

        public SupplierViewModel()
        {
            //初始化操作
            SupplierDataSet = new DataSet();
            initSupplier();
            //保存命令
            this.SaveCommand = new CommandBase();
            this.SaveCommand.DoExecute = new Action<object>(Save);
            this.SaveCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });
        }

        private void initSupplier()
        {
            SupplierDataSet.Tables.Add(ServerDataAccess.GetInstance().GetSupplier());
        }
        public void Save(object obj)
        {
            ServerDataAccess.GetInstance().UpdateSupplier(SupplierDataSet.Tables[0]);
        }
    }
}
