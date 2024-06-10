using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using wms.DataAccess;
using wms.Model;

namespace wms.ViewModel
{
    public class SalesHistoryViewModel
    {
        public ObservableCollection<SalesHistoryModel> SalesHistoryList { get; set; } = new ObservableCollection<SalesHistoryModel>();
        private List<SalesHistoryModel> salesHistoryAll;
        public SalesHistoryViewModel()
        {
            InitSalesHistoryList();
        }

        private void InitSalesHistoryList()
        {
            /*SalesHistoryList = new ObservableCollection<SalesHistoryModel>(ServerDataAccess.GetInstance().GetSalesHistory());*/
            Task.Run(new Action(async () => {
                salesHistoryAll = ServerDataAccess.GetInstance().GetSalesHistory();
                await Task.Delay(500);
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    SalesHistoryList.Clear();
                    foreach (var item in salesHistoryAll)
                        SalesHistoryList.Add(item);
                }));
            }));
        }
    }
}
