using System.Collections.Generic;
using System.Collections.ObjectModel;
using wms.Model;
using wms.DataAccess;
using System.Threading.Tasks;
using System;
using System.Windows;

namespace wms.ViewModel
{
    public class SalesViewModel
    {
        public ObservableCollection<SalesModel> SalesList { get; set; } = new ObservableCollection<SalesModel>();
        private List<SalesModel> salesAll;
        public SalesViewModel()
        {
            InitSalesList();
        }

        private void InitSalesList()
        {
            /*SalesList = new ObservableCollection<SalesModel>(ServerDataAccess.GetInstance().GetSales());*/
            Task.Run(new Action(async () => {
                salesAll = ServerDataAccess.GetInstance().GetSales();
                await Task.Delay(500);
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    SalesList.Clear();
                    foreach (var item in salesAll)
                        SalesList.Add(item);
                }));
            }));
        }
    }
}
