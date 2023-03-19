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
    public class DepartViewModel : NotifyBase
    {
        public DepartModel DepartModel { get; set; } = new DepartModel();

        public CommandBase SaveCommand { get; set; }


        private DataSet _departDataSet;
        public DataSet DepartDataSet
        {
            get { return _departDataSet; }
            set
            {
                _departDataSet = value;
                this.DoNotify();
            }
        }

        public DepartViewModel()
        {
            //初始化操作
            DepartDataSet = new DataSet();
            initDepart();
            //保存命令
            this.SaveCommand = new CommandBase();
            this.SaveCommand.DoExecute = new Action<object>(Save);
            this.SaveCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });
        }

        private void initDepart()
        {
            DepartDataSet.Tables.Add(ServerDataAccess.GetInstance().GetDepart());
        }
        public void Save(object obj)
        {
            ServerDataAccess.GetInstance().UpdateDepart(DepartDataSet.Tables[0]);
        }
    }
}
