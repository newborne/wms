# 仓库管理系统客户端设计

## 项目概述

仓库管理系统是一个基于 C# 和 WPF 的仓库管理工具，旨在帮助企业高效管理仓库中的商品、订单、供应商和客户信息。系统采用 MVVM 架构设计，支持多角度查询、数据录入和更新，以及用户权限管理。

## 功能模块

1. **登录模块**
   - 支持管理员和普通用户登录
   - 集成图片验证码功能
   - 权限验证和用户信息加载

2. **销售管理**
   - 查询所有销售记录
   - 多角度筛选和展示销售详情

3. **历史销售**
   - 按月或按年查询历史销售数据
   - 展示月销售量和年销售量统计

4. **商品管理**
   - 商品分级代码存储
   - 存量多角度查询
   - 商品信息批量更新

5. **入库管理**
   - 入库信息的添加、修改和删除
   - 支持供应商信息绑定
   - 状态管理（已完成/未完成）

6. **订单管理**
   - 订单信息的添加、修改和删除
   - 顾客信息绑定
   - 订单状态管理

7. **出库管理**
   - 出库信息的添加、修改和删除
   - 顾客信息绑定
   - 出库状态管理

8. **供应商管理**
   - 供应商信息的单表查询与操作
   - 支持信息版本管理

9. **顾客管理**
   - 顾客信息的单表查询与操作
   - 支持信息版本管理

10. **员工管理**
    - 员工信息的添加、修改和删除
    - 部门信息绑定
    - 性别和学历信息管理

11. **部门管理**
    - 部门信息的单表查询与操作

12. **注销功能**
    - 支持用户注销并返回登录界面

## 项目结构

```
wms/
├── Assets/                  # 资源文件（样式、字体等）
├── Common/                  # 工具类（NotifyBase、CommandBase等）
├── Converter/               # 数据转换类（BoolToIntConverter等）
├── DataAccess/              # 数据库操作类
├── Model/                   # 实体类（CustomerModel、DepartModel等）
├── View/                    # 用户界面（LoginView、MainView等）
├── ViewModel/               # 逻辑操作类（LoginViewModel、MainViewModel等）
├── App.config               # 配置文件（数据库连接字符串等）
├── App.xaml                 # 应用启动页
└── wms.Controls/            # 第三方控件子项目
```

## 核心代码示例

### 1. NotifyBase 类（MVVM 基类）

```csharp
public class NotifyBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    public void DoNotify([CallerMemberName] string propName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
```

### 2. CommandBase 类（委托命令实现）

```csharp
public class CommandBase : ICommand
{
    public Action<object> DoExecute { get; set; }
    public Func<object, bool> DoCanExecute { get; set; }

    public bool CanExecute(object parameter) => DoCanExecute?.Invoke(parameter) == true;
    public void Execute(object parameter) => DoExecute?.Invoke(parameter);
    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, new EventArgs());
    public event EventHandler CanExecuteChanged;
}
```

### 3. 数据库操作类（ServerDataAccess）

```csharp
public class ServerDataAccess
{
    private static ServerDataAccess instance;
    public static ServerDataAccess GetInstance() => instance ?? (instance = new ServerDataAccess());

    private SqlConnection conn;
    private SqlCommand comm;
    private SqlDataAdapter adapter;
    private SqlCommandBuilder builder;

    public UserEntity CheckUserInfo(string loginName, string loginPassword)
    {
        try
        {
            if (DBConnection())
            {
                string userSql = "SELECT ..."; // SQL 查询语句
                adapter = new SqlDataAdapter(userSql, conn);
                adapter.SelectCommand.Parameters.Add(new SqlParameter("@login_name", loginName));
                adapter.SelectCommand.Parameters.Add(new SqlParameter("@login_password", MD5Provider.GetMD5String(loginPassword + "@" + loginName)));

                DataTable table = new DataTable();
                int count = adapter.Fill(table);

                if (count <= 0) throw new Exception("用户名或密码不正确！");
                if (table.Rows[0].Field<int>("is_can_login") == 0) throw new Exception("当前用户没有权限使用此平台！");

                return new UserEntity
                {
                    LoginName = table.Rows[0].Field<string>("login_name"),
                    EmployeeName = table.Rows[0].Field<string>("employee_name"),
                    Logo = table.Rows[0].Field<string>("logo"),
                    EmployeeSex = table.Rows[0].Field<int>("employee_sex")
                };
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            Dispose();
        }
        return null;
    }
}
```

### 4. ViewModel 示例（LoginViewModel）

```csharp
public class LoginViewModel : NotifyBase
{
    public LoginModel LoginModel { get; set; } = new LoginModel();
    public CommandBase LoginCommand { get; set; }
    public CommandBase CloseWindowCommand { get; set; }

    private string _verificationCode;
    public string VerificationCode
    {
        get => _verificationCode;
        set => (_verificationCode = value).DoNotify();
    }

    private ImageSource _source;
    public ImageSource Source
    {
        get => _source;
        set => (_source = value).DoNotify();
    }

    public LoginViewModel()
    {
        LoginModel.LoginName = "admin";
        LoginModel.LoginPassword = "123456";
        GetImages();

        // 登录命令
        LoginCommand = new CommandBase
        {
            DoExecute = DoLogin,
            DoCanExecute = o => ShowProgress == Visibility.Collapsed
        };

        // 关闭窗口命令
        CloseWindowCommand = new CommandBase
        {
            DoExecute = o => (o as Window).Close(),
            DoCanExecute = o => true
        };
    }

    private void DoLogin(object o)
    {
        ShowProgress = Visibility.Visible;
        ErrorMessage = "";

        if (string.IsNullOrEmpty(LoginModel.LoginName))
        {
            ErrorMessage = "请输入用户名";
            ShowProgress = Visibility.Collapsed;
            return;
        }

        if (string.IsNullOrEmpty(LoginModel.LoginPassword))
        {
            ErrorMessage = "请输入密码";
            ShowProgress = Visibility.Collapsed;
            return;
        }

        if (string.IsNullOrEmpty(VerificationCode))
        {
            ErrorMessage = "请输入验证码";
            ShowProgress = Visibility.Collapsed;
            return;
        }

        if (VerificationCode.ToLower() != validCodeStr)
        {
            ErrorMessage = "验证码输入不正确";
            ShowProgress = Visibility.Collapsed;
            return;
        }

        Task.Run(() =>
        {
            try
            {
                var userInfo = ServerDataAccess.GetInstance().CheckUserInfo(LoginModel.LoginName, LoginModel.LoginPassword);
                if (userInfo == null) throw new Exception("登录失败!用户名或密码错误");

                GlobalValues.UserInfo = userInfo;
                Application.Current.Dispatcher.Invoke(() => (o as Window).DialogResult = true);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                Application.Current.Dispatcher.Invoke(() => ShowProgress = Visibility.Collapsed);
            }
        });
    }
}
```

## 界面截图

以下是系统部分界面的截图，展示系统的视觉效果和交互设计：

- **登录界面**
- **主界面**
- **销售管理**
- **商品管理**
- **员工管理**

## 调试与运行

### 环境要求

- .NET Framework 4.7.2 或更高版本
- Visual Studio 2019 或更高版本

### 运行步骤

1. 克隆项目仓库
2. 打开解决方案文件（.sln）
3. 配置数据库连接字符串（App.config）
4. 按 F5 运行项目

### 调试建议

- 使用断点调试关键逻辑
- 检查数据库连接和查询语句
- 确保第三方控件正确引用

## 联系方式

如需进一步了解项目或提供反馈，请联系项目维护者：

- 邮箱：<newborne@foxmail.com>
- GitHub：<https://github.com/newborne>
