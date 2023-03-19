using System;
using System.Data;
using wms.Common;
using wms.DataAccess;
using wms.Model;

namespace wms.ViewModel
{
    public class OutViewModel : NotifyBase
    {
        public DocumentModel DocumentModel { get; set; } = new DocumentModel();
        public OutModel OutModel { get; set; } = new OutModel();
        public CustomerModel CustomerModel { get; set; } = new CustomerModel();
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
                if (_position >= OutDataSet.Tables[0].Rows.Count)
                    _position = OutDataSet.Tables[0].Rows.Count - 1;
                initOut(Position);//数据表当前行数据属性
                initOutInfo();
            }
        }
        private DataSet _outDataSet;
        public DataSet OutDataSet
        {
            get { return _outDataSet; }
            set
            {
                _outDataSet = value;
                this.DoNotify();
            }
        }
        private DataSet _documentDataSet;
        public DataSet DocumentDataSet
        {
            get { return _documentDataSet; }
            set
            {
                _documentDataSet = value;
                this.DoNotify();
            }
        }

        public OutViewModel()
        {
            //初始化操作
            OutDataSet = new DataSet();
            DocumentDataSet = new DataSet();
            initOut(Position);
            initOutInfo();
            initCurrentCustomer();
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

        private void initCurrentCustomer()
        {
            OutDataSet.Tables.Add(ServerDataAccess.GetInstance().GetCurrentCustomer());
        }

        private void initOut(int Position)
        {
            DocumentDataSet.Tables.Add(ServerDataAccess.GetInstance().GetDocumentOut());
            OutDataSet.Tables.Add(ServerDataAccess.GetInstance().GetOut());
            DocumentModel.DocumentId = OutDataSet.Tables[0].Rows[Position]["document_id"].ToString();
            CustomerModel.CustomerId = OutDataSet.Tables[0].Rows[Position]["customer_id"].ToString();
            CustomerModel.CustomerInfoId = OutDataSet.Tables[0].Rows[Position]["customer_info_id"].ToString();
            DocumentModel.DocumentDate = (DateTime)OutDataSet.Tables[0].Rows[Position]["document_date"];
            OutModel.OutStatus = (int)OutDataSet.Tables[0].Rows[Position]["out_status"];

        }
        private void initOutInfo()
        {
            OutDataSet.Tables.Add(ServerDataAccess.GetInstance().GetOutInfo());
            OutDataSet.Tables[1].DefaultView.RowFilter = "单号='" + DocumentModel.DocumentId + "'";
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
            Position = OutDataSet.Tables[0].Rows.Count - 1;
        }

        public void Insert(object obj)
        {
            DataRow tr = OutDataSet.Tables[0].NewRow();
            tr["out_status"] = 0;
            tr["document_date"] = DateTime.Now;
            OutDataSet.Tables[0].Rows.Add(tr);
            DocumentDataSet.Tables[0].Rows.Add();
            Position = OutDataSet.Tables[0].Rows.Count;
        }
        public void Delete(object obj)
        {
            if (Position > 0)
            {
                OutDataSet.Tables[0].Rows[Position].Delete();//替换OutDataSet.Tables[0].Rows.RemoveAt(Position);
                Position--;
            }
        }
        public void Save(object obj)
        {
            DocumentDataSet.Tables[0].Rows[Position]["document_id"] = DocumentModel.DocumentId;
            DocumentDataSet.Tables[0].Rows[Position]["document_date"] = DocumentModel.DocumentDate;

            OutDataSet.Tables[0].Rows[Position]["document_id"] = DocumentModel.DocumentId;
            OutDataSet.Tables[0].Rows[Position]["customer_id"] = CustomerModel.CustomerId;
            OutDataSet.Tables[0].Rows[Position]["customer_info_id"] = CustomerModel.CustomerInfoId;
            OutDataSet.Tables[0].Rows[Position]["document_date"] = DocumentModel.DocumentDate;
            OutDataSet.Tables[0].Rows[Position]["out_status"] = OutModel.OutStatus;
           
            //数据库操作
            ServerDataAccess.GetInstance().UpdateDocumentOut(DocumentDataSet.Tables[0]);
            ServerDataAccess.GetInstance().UpdateOutInfo(OutDataSet.Tables[1]);
            ServerDataAccess.GetInstance().UpdateOut(OutDataSet.Tables[0]);
        }
    }
}
