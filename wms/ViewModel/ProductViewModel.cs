using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows;
using wms.Common;
using wms.DataAccess;
using wms.Model;

namespace wms.ViewModel
{
    public class ProductViewModel:NotifyBase
    {
        public ProductModel ProductModel { get; set; } = new ProductModel();
        public ObservableCollection<ProductModel> StoreList { get; set; } = new ObservableCollection<ProductModel>();
        private List<ProductModel> storeAll;
        public CommandBase SaveCommand { get; set; }

        
        private DataSet _productDataSet;
        public DataSet ProductDataSet
        {
            get { return _productDataSet; }
            set
            {
                _productDataSet = value;
                this.DoNotify();
            }
        }

        public ProductViewModel()
        {
            //初始化操作
            ProductDataSet = new DataSet();
            initProduct();
            initStore();
            //保存命令
            this.SaveCommand = new CommandBase();
            this.SaveCommand.DoExecute = new Action<object>(Save);
            this.SaveCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });
        }

        private void initStore()
        {
            Task.Run(new Action(async () => {
                storeAll = ServerDataAccess.GetInstance().GetStore();
                await Task.Delay(500);
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    StoreList.Clear();
                    foreach (var item in storeAll)
                        StoreList.Add(item);
                }));
            }));           
        }

        private void initProduct()
        {
            ProductDataSet.Tables.Add(ServerDataAccess.GetInstance().GetProduct());
        }
        public void Save(object obj)
        {
            ServerDataAccess.GetInstance().UpdateProduct(ProductDataSet.Tables[0]);
        }
    }
}
