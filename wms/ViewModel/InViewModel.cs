using System;
using System.Data;
using wms.Common;
using wms.DataAccess;
using wms.Model;

namespace wms.ViewModel
{
    public class InViewModel:NotifyBase
    {
        public DocumentModel DocumentModel { get; set; } = new DocumentModel();
        public InModel InModel { get; set; } = new InModel();
        public SupplierModel SupplierModel { get; set; } = new SupplierModel();
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
                if (_position >= InDataSet.Tables[0].Rows.Count)
                    _position = InDataSet.Tables[0].Rows.Count - 1;
                initIn(Position);//数据表当前行数据属性
                initInInfo();
            }
        }
        private DataSet _inDataSet;
        public DataSet InDataSet
        {
            get { return _inDataSet; }
            set
            {
                _inDataSet = value;
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
        public InViewModel()
        {
            //初始化操作
            InDataSet = new DataSet();
            DocumentDataSet = new DataSet();
            initIn(Position);
            initInInfo();
            initCurrentSupplier();
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

        private void initCurrentSupplier()
        {
            InDataSet.Tables.Add(ServerDataAccess.GetInstance().GetCurrentSupplier());
        }

        private void initIn(int Position)
        {

            DocumentDataSet.Tables.Add(ServerDataAccess.GetInstance().GetDocumentIn());
            InDataSet.Tables.Add(ServerDataAccess.GetInstance().GetIn());
            DocumentModel.DocumentId = InDataSet.Tables[0].Rows[Position]["document_id"].ToString();
            SupplierModel.SupplierId = InDataSet.Tables[0].Rows[Position]["supplier_id"].ToString();
            SupplierModel.SupplierInfoId = InDataSet.Tables[0].Rows[Position]["supplier_info_id"].ToString();
            DocumentModel.DocumentDate = (DateTime)InDataSet.Tables[0].Rows[Position]["document_date"];
            InModel.InStatus = (int)InDataSet.Tables[0].Rows[Position]["in_status"];

        }
        private void initInInfo()
        {
            InDataSet.Tables.Add(ServerDataAccess.GetInstance().GetInInfo());

            InDataSet.Tables[1].DefaultView.RowFilter = "单号='" + DocumentModel.DocumentId + "'";
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
            Position = InDataSet.Tables[0].Rows.Count - 1;
        }

        public void Insert(object obj)
        {
            DataRow tr = InDataSet.Tables[0].NewRow();
            tr["in_status"] = 0;
            tr["document_date"] =DateTime.Now;
            InDataSet.Tables[0].Rows.Add(tr);
            DocumentDataSet.Tables[0].Rows.Add();
            Position = InDataSet.Tables[0].Rows.Count;
        }
        public void Delete(object obj)
        {
            if (Position > 0)
            {
                InDataSet.Tables[0].Rows[Position].Delete();
                Position--;
            }
        }
        public void Save(object obj)
        {
            DocumentDataSet.Tables[0].Rows[Position]["document_id"] = DocumentModel.DocumentId;
            DocumentDataSet.Tables[0].Rows[Position]["document_date"] = DocumentModel.DocumentDate;

            InDataSet.Tables[0].Rows[Position]["document_id"] = DocumentModel.DocumentId;
            InDataSet.Tables[0].Rows[Position]["supplier_id"] = SupplierModel.SupplierId;
            InDataSet.Tables[0].Rows[Position]["supplier_info_id"] = SupplierModel.SupplierInfoId;
            InDataSet.Tables[0].Rows[Position]["document_date"] = DocumentModel.DocumentDate;
            InDataSet.Tables[0].Rows[Position]["in_status"] = InModel.InStatus;
            //数据库操作
            ServerDataAccess.GetInstance().UpdateDocumentIn(DocumentDataSet.Tables[0]);
            ServerDataAccess.GetInstance().UpdateInInfo(InDataSet.Tables[1]);
            ServerDataAccess.GetInstance().UpdateIn(InDataSet.Tables[0]);
        }
    }
}
