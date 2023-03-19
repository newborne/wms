using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using wms.Common;
using wms.DataAccess.DataEntity;
using wms.Model;

namespace wms.DataAccess
{
    public class ServerDataAccess
    {
        private static ServerDataAccess instance;
        private ServerDataAccess() { }
        public static ServerDataAccess GetInstance()
        {
            //instance为空就返回新的实例化
            return instance ?? (instance = new ServerDataAccess());
        }

        SqlConnection conn;
        SqlCommand comm;
        SqlDataAdapter adapter;
        SqlCommandBuilder builder;

        private void Dispose()
        {
            if (adapter != null)
            {
                adapter.Dispose();
                adapter = null;
            }
            if (comm != null)
            {
                comm.Dispose();
                comm = null;
            }
            if (conn != null)
            {
                conn.Close();
                conn.Dispose();
                conn = null;
            }
        }

        private bool DBConnection()
        {
            string connStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            if (conn == null)
                conn = new SqlConnection(connStr);
            try
            {
                conn.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public UserEntity CheckUserInfo(string loginName, string loginPassword)
        {
            try
            {
                if (DBConnection())
                {
                    string userSql = "select a.login_name,b.employee_name,a.login_password,a.logo,b.employee_sex,a.is_can_login from login a,employee b where a.login_name = @login_name and a.login_password=@login_password and a.is_valid=1 and a.employee_id=b.employee_id";
                    adapter = new SqlDataAdapter(userSql, conn);
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@login_name", SqlDbType.VarChar) { Value = loginName });
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@login_password", SqlDbType.VarChar) { Value = MD5Provider.GetMD5String(loginPassword + "@" + loginName) });

                    DataTable table = new DataTable();
                    int count = adapter.Fill(table);

                    if (count <= 0)
                        throw new Exception("用户名或密码不正确！");

                    DataRow dr = table.Rows[0];
                    if (dr.Field<int>("is_can_login") == 0)
                        throw new Exception("当前用户没有权限使用此平台！");

                    UserEntity userInfo = new UserEntity();
                    userInfo.LoginName = dr.Field<string>("login_name");
                    userInfo.EmployeeName = dr.Field<string>("employee_name");
                    userInfo.LoginPassword = dr.Field<string>("login_password");
                    userInfo.Logo = dr.Field<string>("logo");
                    userInfo.EmployeeSex = dr.Field<int>("employee_sex");
                    return userInfo;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Dispose();
            }

            return null;
        }


        public List<SalesModel> GetSales()
        {
            try
            {
                List<SalesModel> result = new List<SalesModel>();
                if (this.DBConnection())
                {
                    string sql = @"select * from sales";
                    adapter = new SqlDataAdapter(sql, conn);

                    DataTable table = new DataTable();
                    int count = adapter.Fill(table);
                    if (count > 0)
                    {
                        SalesModel model = null;
                        foreach (DataRow dr in table.AsEnumerable())
                        {
                            model = new SalesModel();
                            model.DocumentId = dr.Field<string>("document_id");
                            model.DocumentDate = dr.Field<DateTime>("document_date");
                            model.CustomerId = dr.Field<string>("customer_id");
                            model.Id = dr.Field<int>("id");
                            model.ProductName = dr.Field<string>("product_name");
                            model.Price = dr.Field<decimal>("price");
                            model.Quantity = dr.Field<int>("quantity");
                            model.Total = dr.Field<decimal>("total");

                            result.Add(model);
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Dispose();
            }
        }
        public List<SalesHistoryModel> GetSalesHistory()
        {
            try
            {
                List<SalesHistoryModel> result = new List<SalesHistoryModel>();
                if (this.DBConnection())
                {
                    string sql = @"if object_id('tempdb..#temp') is not null
    drop table #temp;
create table #temp
(
    hyear        int,
    hmon         int,
    id           int,
    product_name varchar(20),
    monqty       int,
    monsaleroom  decimal(19, 4),
    yearqty      int,
    yearsaleroom decimal(19, 4)
);
insert into #temp exec SelectYearSales;
select *
from #temp;";
                    adapter = new SqlDataAdapter(sql, conn);

                    DataTable table = new DataTable();
                    int count = adapter.Fill(table);
                    if (count > 0)
                    {
                        SalesHistoryModel model = null;
                        foreach (DataRow dr in table.AsEnumerable())
                        {
                            model = new SalesHistoryModel();
                            model.Hyear = dr.Field<int>("hyear");
                            model.Hmon = dr.Field<int>("hmon");
                            model.Id = dr.Field<int>("id");
                            model.ProductName = dr.Field<string>("product_name");
                            model.Monqty = dr.Field<int>("monqty");
                            model.Monsalesroom = dr.Field<decimal>("monsaleroom");
                            model.Yearqty = dr.Field<int>("yearqty");
                            model.Yearsalesroom = dr.Field<decimal>("yearsaleroom");

                            result.Add(model);
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Dispose();
            }
        }
        public DataTable GetProduct()
        {
            try
            {
                DataTable result = new DataTable();
                if (this.DBConnection())
                {
                    string sql = @"select * from product";
                    adapter = new SqlDataAdapter(sql, conn);
                    adapter.Fill(result);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Dispose();
            }
        }
        public bool UpdateProduct(DataTable dataTable) 
        {
            try
            {
                if (this.DBConnection())
                {
                    string sql = @"select * from product";
                    adapter = new SqlDataAdapter(sql, conn);
                    builder = new SqlCommandBuilder(adapter);
                    adapter.Update(dataTable);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Dispose();
            }
        }
        public List<ProductModel> GetStore()
        {
            try
            {
                List<ProductModel> result = new List<ProductModel>();
                if (this.DBConnection())
                {
                    string sql = @"if object_id('tempdb..#temp') is not null
    drop table #temp;
create table #temp
(
    id        int,
    code         char(2),
    pid         int,
    product_name varchar(20),
    amt       int
);
insert into #temp exec SelectStore;
select id,product_name,isnull(amt,0) as amt
from #temp;";
                    adapter = new SqlDataAdapter(sql, conn);
                    DataTable table = new DataTable();
                    int count = adapter.Fill(table);
                    if (count > 0)
                    {
                        ProductModel model = null;
                        foreach (DataRow dr in table.AsEnumerable())
                        {
                            model = new ProductModel();
                            model.Id = dr.Field<int>("id");
                            model.ProductName = dr.Field<string>("product_name");
                            model.Amt = dr.Field<int>("amt");

                            result.Add(model);
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Dispose();
            }
        }

        public DataTable GetEmployee()
        {
            try
            {
                DataTable result = new DataTable();
                if (this.DBConnection())
                {
                    string sql = @"select * from employee";
                    adapter = new SqlDataAdapter(sql, conn);
                    adapter.Fill(result);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Dispose();
            }
        }
        public bool UpdateEmployee(DataTable dataTable)
        {
            try
            {
                if (this.DBConnection())
                {
                    string sql = @"select * from employee";
                    adapter = new SqlDataAdapter(sql, conn);
                    builder = new SqlCommandBuilder(adapter);
                    adapter.Update(dataTable);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Dispose();
            }
        }
        public DataTable GetLogin()
        {
            try
            {
                DataTable result = new DataTable();
                if (this.DBConnection())
                {
                    string sql = @"select employee_id as '员工id', login_name as '登录用户名', login_password as '密码', logo as '头像', is_admin as '管理员权限', is_valid as '生效', is_can_login as '允许登录' from login";
                    adapter = new SqlDataAdapter(sql, conn);
                    adapter.Fill(result);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Dispose();
            }
        }
        public bool UpdateLogin(DataTable dataTable)
        {
            try
            {
                if (this.DBConnection())
                {
                    string sql = @"select employee_id as '员工id', login_name as '登录用户名', login_password as '密码', logo as '头像', is_admin as '管理员权限', is_valid as '生效', is_can_login as '允许登录' from login";
                    adapter = new SqlDataAdapter(sql, conn);
                    builder = new SqlCommandBuilder(adapter);
                    adapter.Update(dataTable);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Dispose();
            }
        }
        public DataTable GetDepart()
        {
            try
            {
                DataTable result = new DataTable();
                if (this.DBConnection())
                {
                    string sql = @"select * from depart";
                    adapter = new SqlDataAdapter(sql, conn);
                    adapter.Fill(result);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Dispose();
            }
        }
        public bool UpdateDepart(DataTable dataTable)
        {
            try
            {
                if (this.DBConnection())
                {
                    string sql = @"select * from depart";
                    adapter = new SqlDataAdapter(sql, conn);
                    builder = new SqlCommandBuilder(adapter);
                    adapter.Update(dataTable);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Dispose();
            }
        }
        public DataTable GetIn()
        {
            try
            {
                DataTable result = new DataTable();
                if (this.DBConnection())
                {
                    string sql = @"select * from [in] order by document_id";
                    adapter = new SqlDataAdapter(sql, conn);
                    adapter.Fill(result);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Dispose();
            }
        }
        public bool UpdateIn(DataTable dataTable)
        {
            try
            {
                if (this.DBConnection())
                {
                    string sql = @"select * from [in] order by document_id";
                    adapter = new SqlDataAdapter(sql, conn);
                    builder = new SqlCommandBuilder(adapter);
                    adapter.Update(dataTable);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Dispose();
            }
        }

        public DataTable GetInInfo()
        {
            try
            {
                DataTable result = new DataTable();
                if (this.DBConnection())
                {
                    string sql = @"select id as '商品id', document_id as '单号', quantity as '数量', price as '价格', remark as '备注'
from document_info
where document_id like 'IN%'";
                    adapter = new SqlDataAdapter(sql, conn);
                    adapter.Fill(result);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Dispose();
            }
        }
        public bool UpdateInInfo(DataTable dataTable)
        {
            try
            {
                if (this.DBConnection())
                {
                    string sql = @"select id as '商品id', document_id as '单号', quantity as '数量', price as '价格', remark as '备注'
from document_info
where document_id like 'IN%'";
                    adapter = new SqlDataAdapter(sql, conn);
                    builder = new SqlCommandBuilder(adapter);
                    adapter.Update(dataTable);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Dispose();
            }
        }

        public DataTable GetCurrentSupplier()
        {
            try
            {
                DataTable result = new DataTable();
                if (this.DBConnection())
                {
                    string sql = @"select *
from (select * from supplier where supplier_set_date < GETDATE()) a
where supplier_set_date >= all
      (
          select b.supplier_set_date
          from (select * from supplier where supplier_set_date < GETDATE()) b
          where b.supplier_id = a.supplier_id
      )";
                    adapter = new SqlDataAdapter(sql, conn);
                    adapter.Fill(result);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Dispose();
            }
        }

        public DataTable GetSupplier()
        {
            try
            {
                DataTable result = new DataTable();
                if (this.DBConnection())
                {
                    string sql = @"select supplier_id       as '供应商ID',
       supplier_info_id  as '供应商信息版本号',
       supplier_set_date as '信息变更时间',
       supplier_name     as '名称',
       supplier_address  as '地址',
       supplier_phone    as '联系电话'
from supplier";
                    adapter = new SqlDataAdapter(sql, conn);
                    adapter.Fill(result);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Dispose();
            }
        }
        public bool UpdateSupplier(DataTable dataTable)
        {
            try
            {
                if (this.DBConnection())
                {
                    string sql = @"select supplier_id       as '供应商ID',
       supplier_info_id  as '供应商信息版本号',
       supplier_set_date as '信息变更时间',
       supplier_name     as '名称',
       supplier_address  as '地址',
       supplier_phone    as '联系电话'
from supplier";
                    adapter = new SqlDataAdapter(sql, conn);
                    builder = new SqlCommandBuilder(adapter);
                    adapter.Update(dataTable);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Dispose();
            }
        }

        public DataTable GetCustomer()
        {
            try
            {
                DataTable result = new DataTable();
                if (this.DBConnection())
                {
                    string sql = @"select customer_id       as '顾客ID',
       customer_info_id  as '顾客信息版本号',
       customer_set_date as '信息变更时间',
       customer_name     as '名称',
       customer_address  as '地址',
       customer_phone_number    as '联系电话'
from customer";
                    adapter = new SqlDataAdapter(sql, conn);
                    adapter.Fill(result);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Dispose();
            }
        }
        public bool UpdateCustomer(DataTable dataTable)
        {
            try
            {
                if (this.DBConnection())
                {
                    string sql = @"select customer_id       as '顾客ID',
       customer_info_id  as '顾客信息版本号',
       customer_set_date as '信息变更时间',
       customer_name     as '名称',
       customer_address  as '地址',
       customer_phone_number    as '联系电话'
from customer";
                    adapter = new SqlDataAdapter(sql, conn);
                    builder = new SqlCommandBuilder(adapter);
                    adapter.Update(dataTable);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Dispose();
            }
        }

        public DataTable GetCurrentCustomer()
        {
            try
            {
                DataTable result = new DataTable();
                if (this.DBConnection())
                {
                    string sql = @"select *
from (select * from customer where customer_set_date < GETDATE()) a
where customer_set_date >= all
      (
          select b.customer_set_date
          from (select * from customer where customer_set_date < GETDATE()) b
          where b.customer_id = a.customer_id
      )";
                    adapter = new SqlDataAdapter(sql, conn);
                    adapter.Fill(result);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Dispose();
            }
        }

        public DataTable GetOrder()
        {
            try
            {
                DataTable result = new DataTable();
                if (this.DBConnection())
                {
                    string sql = @"select * from [order] order by document_id";
                    adapter = new SqlDataAdapter(sql, conn);
                    adapter.Fill(result);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Dispose();
            }
        }
        public bool UpdateOrder(DataTable dataTable)
        {
            try
            {
                if (this.DBConnection())
                {
                    string sql = @"select * from [order] order by document_id";
                    adapter = new SqlDataAdapter(sql, conn);
                    builder = new SqlCommandBuilder(adapter);
                    adapter.Update(dataTable);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Dispose();
            }
        }

        public DataTable GetOrderInfo()
        {
            try
            {
                DataTable result = new DataTable();
                if (this.DBConnection())
                {
                    string sql = @"select id as '商品id', document_id as '单号', quantity as '数量', price as '价格', remark as '备注'
from document_info
where document_id like 'OR%'";
                    adapter = new SqlDataAdapter(sql, conn);
                    adapter.Fill(result);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Dispose();
            }
        }
        public bool UpdateOrderInfo(DataTable dataTable)
        {
            try
            {
                if (this.DBConnection())
                {
                    string sql = @"select id as '商品id', document_id as '单号', quantity as '数量', price as '价格', remark as '备注'
from document_info
where document_id like 'OR%'";
                    adapter = new SqlDataAdapter(sql, conn);
                    builder = new SqlCommandBuilder(adapter);
                    adapter.Update(dataTable);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Dispose();
            }
        }

        public DataTable GetOut()
        {
            try
            {
                DataTable result = new DataTable();
                if (this.DBConnection())
                {
                    string sql = @"select * from out order by document_id";
                    adapter = new SqlDataAdapter(sql, conn);
                    adapter.Fill(result);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Dispose();
            }
        }
        public bool UpdateOut(DataTable dataTable)
        {
            try
            {
                if (this.DBConnection())
                {
                    string sql = @"select * from out order by document_id";
                    adapter = new SqlDataAdapter(sql, conn);
                    builder = new SqlCommandBuilder(adapter);
                    adapter.Update(dataTable);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Dispose();
            }
        }

        public DataTable GetOutInfo()
        {
            try
            {
                DataTable result = new DataTable();
                if (this.DBConnection())
                {
                    string sql = @"select id as '商品id', document_id as '单号', quantity as '数量', price as '价格', remark as '备注'
from document_info
where document_id like 'OU%'";
                    adapter = new SqlDataAdapter(sql, conn);
                    adapter.Fill(result);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Dispose();
            }
        }
        public bool UpdateOutInfo(DataTable dataTable)
        {
            try
            {
                if (this.DBConnection())
                {
                    string sql = @"select id as '商品id', document_id as '单号', quantity as '数量', price as '价格', remark as '备注'
from document_info
where document_id like 'OU%'";
                    adapter = new SqlDataAdapter(sql, conn);
                    builder = new SqlCommandBuilder(adapter);
                    adapter.Update(dataTable);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Dispose();
            }
        }
        public DataTable GetDocumentIn()
        {
            try
            {
                DataTable result = new DataTable();
                if (this.DBConnection())
                {
                    string sql = @"select * from document where document_id like 'IN%'";
                    adapter = new SqlDataAdapter(sql, conn);
                    adapter.Fill(result);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Dispose();
            }
        }
        public bool UpdateDocumentIn(DataTable dataTable)
        {
            try
            {
                if (this.DBConnection())
                {
                    string sql = @"select * from document where document_id like 'IN%'";
                    adapter = new SqlDataAdapter(sql, conn);
                    builder = new SqlCommandBuilder(adapter);
                    adapter.Update(dataTable);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Dispose();
            }
        }

        public DataTable GetDocumentOut()
        {
            try
            {
                DataTable result = new DataTable();
                if (this.DBConnection())
                {
                    string sql = @"select * from document where document_id like 'OU%'";
                    adapter = new SqlDataAdapter(sql, conn);
                    adapter.Fill(result);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Dispose();
            }
        }
        public bool UpdateDocumentOut(DataTable dataTable)
        {
            try
            {
                if (this.DBConnection())
                {
                    string sql = @"select * from document where document_id like 'OU%'";
                    adapter = new SqlDataAdapter(sql, conn);
                    builder = new SqlCommandBuilder(adapter);
                    adapter.Update(dataTable);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Dispose();
            }
        }

        public DataTable GetDocumentOrder()
        {
            try
            {
                DataTable result = new DataTable();
                if (this.DBConnection())
                {
                    string sql = @"select * from document where document_id like 'OR%'";
                    adapter = new SqlDataAdapter(sql, conn);
                    adapter.Fill(result);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Dispose();
            }
        }
        public bool UpdateDocumentOrder(DataTable dataTable)
        {
            try
            {
                if (this.DBConnection())
                {
                    string sql = @"select * from document where document_id like 'OR%'";
                    adapter = new SqlDataAdapter(sql, conn);
                    builder = new SqlCommandBuilder(adapter);
                    adapter.Update(dataTable);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Dispose();
            }
        }
    }
}
