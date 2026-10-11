using ClsComm;
using Google.Protobuf.Collections;
using Google.Protobuf.Compiler;
using IntelligentEquipmentCenter.UiForm;
using IntelligentEquipmentCenter.UPage;
using Mysqlx;
using MySqlX.XDevAPI.Common;
using NetWorkHelper.ICollections;
using NetWorkHelper.IModels;
using NetWorkHelper.TCP;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Utilities.Net;
using SQLite.SQLite;
using Sunny.UI;
using Sunny.UI.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Net;
using System.Net.Sockets;
using System.Runtime.ConstrainedExecution;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using TF.File;
using static IntelligentEquipmentCenter.ClientJson;
using static Mysqlx.Datatypes.Scalar.Types;
using static Mysqlx.Expect.Open.Types.Condition.Types;
using Type = System.Type;


namespace IntelligentEquipmentCenter.UControl
{
    public partial class EHomePage : UserControl
    {
        //监控接收数据上升沿
        public R_TRIG Recev2d=new R_TRIG();
        public R_TRIG Recev3d=new R_TRIG();
        public R_TRIG dgv=new R_TRIG();
        public static bool InitThread=false;

        //存放单个物料的数据

        public static int OK;
        public static int NG;

        //配置文件读取
        public string _SITE;
        public string _OPERATION;
        public string _RESOURCE;
        public string _NC_CODE;
        public string _IP;
        public string _PORT;
        public string _URL;
        public string _USER;
        public string _PWD;
        public string _URLF;

        public string _URLout;
        public string _URLstart;
        public string _URLpass;
        public string _URLerror;
        public string URLout;
        public string URLstart;
        public string URLpass;
        public string URLerror;
        public string badtype;

        public string board_SITE;
        public string board_OPERATION;
        public string board_RESOURCE;
        public string board_NC_CODE;
        public string board_IP;
        public string board_PORT;
        public string board_URL;
        public string board_USER;
        public string board_PWD;
        public string board_URLF;
        public string _URLshoporder;
        public string URLshoporder;

        public string _factoryCode;
        public string _userName;
        public string _passWord;
        public string _machineCode;
        public string _testFile;
        public string _testFileVersion;
        public string _stationCode;
        public string _operationCode;
        public static string token;

        //条码
        public string _sn;
        public string board_sn;
        //登录mes状态
        public static string _state;
        //登录mes失败信息
        public static string _message;
        //待上传数据列表
        public JArray array = new JArray();
        //条码上传状态
        public string BarCodeUploadState;
        //单个物料结束标志，10-2dng结束,11-3dng结束，1-ok结束
        public static int ReceOver;
        //检测结果
        public static bool FullResult;
        //是否上传成功标志
        public string IsUploadSuccess;
        /// <summary>
        /// 测试时间
        /// </summary>
        public string TestTimer;
        /// <summary>
        /// 视觉结果
        /// </summary>
        private string VisonRes;
        //数据类
        MesdataDLS passStandar1 = new MesdataDLS();

      
        /// <summary>Stutable 配置列表（从 StutableConfig.xml 读取）</summary>
        public static List<StutableColumnConfig> _stutableConfigs = new List<StutableColumnConfig>();


        private static readonly object objThreadSql = new object();//数据库互锁
        private static readonly object objShowMsg = new object();//互锁

        public static string MESModel = "在线";
        public static string OKCount = "100";
        public static string NGCount = "0";
        public static string AllCount = "100";
        public static string LVCount = "100%";
        //数据库
        private int indexId = 0;//获取当前表格id

        public  static  SQLiteHelper SQLiteHelpers = null;
        private string DBAddress;
        private string DBdata;
        public static SQLiteHelper SQLiteHelpers2 = null;

        public static Dictionary<string, object> Stutable = new Dictionary<string, object>();
        //dgvData显示列配置（来自xml/DgvDataConfig.xml）
        public static List<DgvColumnConfig> DgvColumns = new List<DgvColumnConfig>();
        private Dictionary<string, object> StutableUidEnTry = new Dictionary<string, object>();
        private Dictionary<string, object> StutableBarcodeCount = new Dictionary<string, object>();
        private Dictionary<string, object> StutableCurreUser = new Dictionary<string, object>();
        private Dictionary<string, object> StutableQuickWear = new Dictionary<string, object>();
        private Dictionary<string, object> StutableChange = new Dictionary<string, object>();

        private string tableNameCur = "";//当前数据库表名

        public EHomePage()
        {
            InitializeComponent();

            InitThread = true;
            //先读取当前选择的配方，获取动态参数
            Recipedata.Load(out _);
            //配方参数驱动：从当前配方重建 Stutable（数据库列）
            RebuildFromRecipe();
            //显示列：xml/DgvDataConfig.xml 固定列 + 配方勾选"显示到界面"的动态列
            LoadDgvConfig();
            AppendRecipeDgvColumns();
            InitDgvColumns();

            //
            StutableBarcodeCount.Add("序号", "int");
            StutableBarcodeCount.Add("流水码", "int");
            //
            StutableCurreUser.Add("序号", "int");
            StutableCurreUser.Add("操作人工号", "text");
            //
            StutableQuickWear.Add("易损件位置", "text");
            StutableQuickWear.Add("易损件名称", "text");
            StutableQuickWear.Add("易损件理论使用次数", "int");
            StutableQuickWear.Add("易损件已使用次数", "int");


            StutableUidEnTry.Add("用户名", "STRING");
            StutableUidEnTry.Add("用户密码", "STRING");
            StutableUidEnTry.Add("用户权限", "STRING");
            StutableUidEnTry.Add("UID", "STRING");
            StutableUidEnTry.Add("最后登陆时间", "STRING");
            StutableUidEnTry.Add("登录次数", "INT");
            StutableUidEnTry.Add("工号", "STRING");
            StutableUidEnTry.Add("MES账号", "STRING");
            StutableUidEnTry.Add("MES密码", "STRING");

            StutableChange.Add("日期", "STRING");
            StutableChange.Add("修改记录", "STRING");
        }

    

        /// <summary>
        /// 配方切换/修改后调用：重建 Stutable 和显示列
        /// </summary>
        public void RefreshRecipeConfig()
        {
            RebuildFromRecipe();
            //重建显示列：XML固定列 + 配方勾选"显示到界面"的动态列
            CommonHelper.SafeInvoke(dgvData, () =>
            {
                LoadDgvConfig();
                AppendRecipeDgvColumns();
                InitDgvColumns();
            });
            //触发配方变更事件（Label显示等订阅者刷新）
            RaiseRecipeChanged();
            //重建列后重新加载今天记录（最多20条），避免 InitDgvColumns 清空列后表格为空
            LoadRecentRecords();
        }

        /// <summary>
        /// 把配方中勾选"显示到界面"的参数插入为 dgvData 显示列（插在SN产品编码列后面）
        /// 只显示实际值，结果列不显示
        /// </summary>
        private void AppendRecipeDgvColumns()
        {
            var ps = Recipedata.Data?.Parameters;
            if (ps == null) return;
            //找到SN列位置，动态列插在它后面
            int snIdx = DgvColumns.FindIndex(c => c.DbField == "产品编码");
            int insertAt = snIdx >= 0 ? snIdx + 1 : DgvColumns.Count;
            foreach (var p in ps)
            {
                if (string.IsNullOrEmpty(p.Name) || !p.ShowOnUI) continue;
                //实际值列：配了PLC地址就从PLC读
                if (!DgvColumns.Any(c => c.DbField == p.Name + "实际值"))
                {
                    DgvColumns.Insert(insertAt++, new DgvColumnConfig
                    {
                        Name = p.Name + "实际值",
                        DbField = p.Name + "实际值",
                        Width = 100,
                        AutoSize = false,
                        PlcAddress = p.PlcAddress
                    });
                }
            }
        }

        /// <summary>
        /// 从当前配方重建 Stutable（数据库列）和列配置
        /// 固定列从 xml/StutableConfig.xml 读取（含值来源 Source），配方参数列追加在后面
        /// </summary>
        public static void RebuildFromRecipe()
        {
            //固定列：从 xml/StutableConfig.xml 读取
            Stutable.Clear();
            _stutableConfigs.Clear();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("xml/StutableConfig.xml");
            foreach (XmlElement element in xmlDoc.GetElementsByTagName("Column"))
            {
                string name = element.GetAttribute("Name");
                if (string.IsNullOrEmpty(name)) continue;
                string type = element.GetAttribute("Type");
                Stutable[name] = string.IsNullOrEmpty(type) ? "text" : type;
                _stutableConfigs.Add(new StutableColumnConfig
                {
                    Name = name,
                    Type = string.IsNullOrEmpty(type) ? "text" : type,
                    Source = element.GetAttribute("Source")
                });
            }

            //配方参数列（无上下限的参数不生成上限/下限列，只生成实际值+结果）
            var ps = Recipedata.Data?.Parameters;
            if (ps != null)
            {
                foreach (var p in ps)
                {
                    if (string.IsNullOrEmpty(p.Name)) continue;
                    if (p.NeedLimit)
                    {
                        Stutable[p.Name + "检测上限"] = "text";
                        Stutable[p.Name + "检测下限"] = "text";
                    }
                    Stutable[p.Name + "检测实际值"] = "text";
                    Stutable[p.Name + "检测判断结果"] = "text";
                }
            }
        }

        //从xml/DgvDataConfig.xml加载dgvData显示列配置
        private void LoadDgvConfig()
        {
            DgvColumns.Clear();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("xml/DgvDataConfig.xml");
            foreach (XmlElement element in xmlDoc.GetElementsByTagName("Column"))
            {
                string name = element.GetAttribute("Name");
                if (string.IsNullOrEmpty(name)) continue;
                DgvColumnConfig col = new DgvColumnConfig();
                col.Name = name;
                col.DbField = element.GetAttribute("DbField");
                if (string.IsNullOrEmpty(col.DbField)) col.DbField = name;
                if (int.TryParse(element.GetAttribute("Width"), out int width)) col.Width = width;
                col.AutoSize = element.GetAttribute("AutoSize").Trim().Equals("true", StringComparison.OrdinalIgnoreCase);
                col.PlcAddress = element.GetAttribute("PlcAddress").Trim();
                col.Source = element.GetAttribute("Source").Trim();
                DgvColumns.Add(col);
            }
        }

        //按配置初始化dgvData列
        private void InitDgvColumns()
        {
            dgvData.Columns.Clear();
            foreach (DgvColumnConfig col in DgvColumns)
            {
                DataGridViewTextBoxColumn dgvCol = new DataGridViewTextBoxColumn();
                dgvCol.Name = col.Name;
                dgvCol.HeaderText = col.Name;
                dgvCol.ReadOnly = true;
                dgvCol.SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvCol.MinimumWidth = 8;
                if (col.AutoSize)
                {
                    dgvCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
                else
                {
                    dgvCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;
                    dgvCol.Width = col.Width;
                }
                dgvData.Columns.Add(dgvCol);
            }
            //末尾保留“详情”按钮列
            dgvData.Columns.Add(this.Infos);
        }

        //解析PLC地址，如"D900"->900
        private int ParsePlcAddress(string plcAddress)
        {
            return ConfigValueResolver.ParsePlcAddress(plcAddress);
        }

        //批量读取所有配置了PLC地址的列（按地址排序合并连续段，减少Modbus请求次数）
        private Dictionary<string, double> ReadPlcColumnsBatch()
        {
            var items = DgvColumns.Where(c => !string.IsNullOrEmpty(c.PlcAddress))
                .Select(c => new KeyValuePair<string, int>(c.DbField, ParsePlcAddress(c.PlcAddress)))
                .ToList();
            return ConfigValueResolver.ReadFloatBatch(items);
        }

        //按dgvData显示列配置组装一行显示数据，优先级：
        //  1. 配了 Source 的列 → 通用来源取值（Recipe:/Mes:/Plc:/Scanner:/Vision:）
        //  2. 配了 PlcAddress 的列 → 从PLC批量读取
        //  3. 配方动态列（结果/上限/下限）→ 从 passStandar1.DynamicParams 取（判定时已写入）
        //  4. 其他 → 空值
        private string[] BuildDgvRowValues()
        {
            Dictionary<string, double> plcValues = ReadPlcColumnsBatch();
            string[] values = new string[DgvColumns.Count];
            for (int i = 0; i < DgvColumns.Count; i++)
            {
                var col = DgvColumns[i];
                if (!string.IsNullOrEmpty(col.Source))
                {
                    values[i] = GetConfigColumnValue(col.Source, passStandar1, Recipedata.Data);
                }
                else if (plcValues.TryGetValue(col.DbField, out double v))
                {
                    values[i] = v.ToString("f2");
                }
                else if (passStandar1.DynamicParams.TryGetValue(col.DbField, out string dv))
                {
                    values[i] = dv;
                }
                else
                {
                    values[i] = "";
                }
            }
            return values;
        }

        /// <summary>
        /// 按配方参数判定：每项从PLC按数据类型读实际值，对比上下限或读外部结果，全部OK返回true
        /// 结果写入 passStandar1.DynamicParams（供入库和MES上传使用）
        /// </summary>
        private bool JudgeRecipeParameters()
        {
            var parameters = Recipedata.Data.Parameters;
            passStandar1.DynamicParams.Clear();
            if (parameters == null || parameters.Count == 0)
            {
                return true; //无参数时默认OK
            }

            bool allOk = true;
            for (int idx = 0; idx < parameters.Count; idx++)
            {
                var p = parameters[idx];
                if (string.IsNullOrEmpty(p.Name)) continue;
                int plcAddr = ParsePlcAddress(p.PlcAddress);
                string dataType = string.IsNullOrEmpty(p.DataType) ? "float" : p.DataType.Trim().ToLowerInvariant();
                bool isString = dataType == "string";

                string actualValue; //实际值（按类型格式化后写入动态参数）
                double real = 0;

                if (isString)
                {
                    //字符串：ReadString读取指定字节数，空值重试5次（等PLC写入）
                    string strVal = "";
                    for (int retry = 0; retry < 5 && string.IsNullOrEmpty(strVal); retry++)
                    {
                        if (retry > 0) Thread.Sleep(100);
                        strVal = ModbusClass.ReadString(plcAddr, p.StringLength)?.Trim() ?? "";
                    }
                    actualValue = strVal;
                }
                else if (dataType == "int")
                {
                    int[] regs = ModbusClass.ReadHoldingRegisters(plcAddr, 1);
                    if (regs != null && regs.Length > 0) real = regs[0];
                    //整型实际值无效时重读5次（等PLC刷新）
                    for (int retry = 0; retry < 5 && real <= 0.3; retry++)
                    {
                        Thread.Sleep(100);
                        regs = ModbusClass.ReadHoldingRegisters(plcAddr, 1);
                        if (regs != null && regs.Length > 0) real = regs[0];
                    }
                    actualValue = real.ToString("f3");
                }
                else
                {
                    //float：32位浮点（占2个寄存器）
                    real = ModbusClass.ReadDoubleRegister(plcAddr);
                    //实际值无效时重读5次（等PLC刷新）
                    for (int retry = 0; retry < 5 && real <= 0.3; retry++)
                    {
                        Thread.Sleep(100);
                        real = ModbusClass.ReadDoubleRegister(plcAddr);
                    }
                    actualValue = real.ToString("f3");
                }

                string result;
                if (p.NeedLimit)
                {
                    //上下限判定：实际值对比上下限，结果由PC直接计算（结果PLC地址仅对无上下限参数有效，这里不写回）
                    result = (real > p.UpLimit || real < p.DownLimit) ? "NG" : "OK";
                    passStandar1.DynamicParams[p.Name + "上限"] = p.UpLimit.ToString("f3");
                    passStandar1.DynamicParams[p.Name + "下限"] = p.DownLimit.ToString("f3");

                    Logger.push(new Log(Logger.LogType.INFO, "EHomePage",
                        $"{p.Name}: 实际值={actualValue}, 上限={p.UpLimit:f3}, 下限={p.DownLimit:f3}, 结果={result}"));
                }
                else
                {
                    //无上下限（含字符串类型）：结果由外部写回结果地址（int：1=OK，其余=NG），PC重试读取取最后值
                    result = "NG";
                    if (!string.IsNullOrEmpty(p.ResultPlcAddress))
                    {
                        int resultAddr = ParsePlcAddress(p.ResultPlcAddress);
                        int resultVal = 0;
                        for (int retry = 0; retry < 5; retry++)
                        {
                            try
                            {
                                int[] regs = ModbusClass.ReadHoldingRegisters(resultAddr, 1);
                                if (regs != null && regs.Length > 0) resultVal = regs[0];
                            }
                            catch { }
                            Thread.Sleep(100);
                        }
                        result = resultVal == 1 ? "OK" : "NG";
                    }

                    Logger.push(new Log(Logger.LogType.INFO, "EHomePage",
                        $"{p.Name}: 实际值={actualValue}, 结果地址={p.ResultPlcAddress}, 结果={result}"));
                }
                if (result == "NG") allOk = false;

                //写入动态参数（入库和MES上传统一从这里取）
                passStandar1.DynamicParams[p.Name + "实际值"] = actualValue;
                passStandar1.DynamicParams[p.Name + "结果"] = result;
                if (!string.IsNullOrEmpty(p.Unit))
                    passStandar1.DynamicParams[p.Name + "单位"] = p.Unit;

                Thread.Sleep(50);
            }
            return allOk;
        }

        public static void AddDataChage(string date, string msg)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("日期", date);
            dic.Add("修改记录", msg);
            SQLiteHelpers.InsertData("StutableChange", dic);
        }
        private void AddToSqliteDLS(MesdataDLS mes)
        {
            DateTime dateTime = DateTime.Now;
            tableNameCur = "testRecord" + dateTime.ToString("yyyyMMdd");
            SQLiteHelper.CreateTable(tableNameCur, DBAddress, Stutable);
            Dictionary<string, object> dic = new Dictionary<string, object>();

            //固定列：按 StutableConfig.xml 的 Source 配置取值（反射取配方/MES数据属性）
            var recipe = Recipedata.Data;
            foreach (var cfg in _stutableConfigs)
            {
                dic[cfg.Name] = GetConfigColumnValue(cfg.Source, mes, recipe);
            }

            //配方参数动态列：上限/下限直接取配方，实际值/结果从 DynamicParams 读取（判定时写入）
            var ps = Recipedata.Data?.Parameters;
            if (ps != null)
            {
                foreach (var p in ps)
                {
                    if (string.IsNullOrEmpty(p.Name)) continue;
                    if (Stutable.ContainsKey(p.Name + "检测上限"))
                        dic[p.Name + "检测上限"] = p.UpLimit.ToString("f3");
                    if (Stutable.ContainsKey(p.Name + "检测下限"))
                        dic[p.Name + "检测下限"] = p.DownLimit.ToString("f3");
                    if (Stutable.ContainsKey(p.Name + "检测实际值"))
                        dic[p.Name + "检测实际值"] = mes.DynamicParams.TryGetValue(p.Name + "实际值", out string av) ? av : "";
                    if (Stutable.ContainsKey(p.Name + "检测判断结果"))
                        dic[p.Name + "检测判断结果"] = mes.DynamicParams.TryGetValue(p.Name + "结果", out string rs) ? rs : "";
                }
            }

            //Stutable里可能有但上面没覆盖到的列，补空值
            foreach (string key in Stutable.Keys)
            {
                if (!dic.ContainsKey(key)) dic[key] = "";
            }

            lock (objThreadSql)
            {
                dic.TryGetValue("产品编码", out object code);
                if (code != null && code.ToString().Length >= 8)
                {
                    DataSet dataSet = SQLiteHelpers.ExecuteDataSet4(tableNameCur, "产品编码", code.ToString());
                    if (dataSet == null || dataSet.Tables[0].Rows.Count < 1)
                    {
                        int res = SQLiteHelpers.InsertData(tableNameCur, dic);
                    }
                    else
                    {
                        SQLiteHelpers.Delete(tableNameCur, "产品编码", mes.ProduceCode);
                        int res = SQLiteHelpers.InsertData(tableNameCur, dic);
                    }

                }
            }

        }

        /// <summary>
        /// 按 Source 配置取列值（通用数据来源），实际解析见 ConfigValueResolver.Resolve
        /// </summary>
        private string GetConfigColumnValue(string source, MesdataDLS mes, RecipeData recipe)
        {
            return ConfigValueResolver.Resolve(source, mes, recipe, board_sn, VisonRes);
        }

        private void EHomePage_Load(object sender, EventArgs e)
        {
            //打开配置文件
            ReadConfig();
            //mes登录
            //Login();

            TestTimer = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //打开数据库
            string local = System.AppDomain.CurrentDomain.BaseDirectory;
            DBAddress = local + "\\testRecord.db3";
            SQLiteHelper.CreateDB(DBAddress);
            DateTime dateTime = DateTime.Now;
            tableNameCur = "testRecord" + dateTime.ToString("yyyyMMdd");
            SQLiteHelper.CreateTable(tableNameCur, DBAddress, Stutable);
            //创建数据表
            SQLiteHelper.CreateTable("BarcodeCount", DBAddress, StutableBarcodeCount);
            SQLiteHelper.CreateTable("CurreUser", DBAddress, StutableCurreUser);
            SQLiteHelper.CreateTable("QuickWear", DBAddress, StutableQuickWear);
            SQLiteHelper.CreateTable("UidEnTry", DBAddress, StutableUidEnTry);
            SQLiteHelper.CreateTable("StutableChange", DBAddress, StutableChange);
            SQLiteHelpers = new SQLiteHelper(DBAddress);
            SQLiteHelpers.Open();

          
            //数据库写一个默认值，主要是序号要写一个
            DataSet set1 = SQLiteHelpers.ExecuteDataSet3("BarcodeCount");
            DataSet set2 = SQLiteHelpers.ExecuteDataSet3("CurreUser");
            if (set1 == null|| set1.Tables[0].Rows.Count < 1)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("序号", 1);
                dic.Add("流水码", 00001);
                SQLiteHelpers.InsertData("BarcodeCount", dic);
            }
            if (set2 == null || set1.Tables[0].Rows.Count < 1)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("序号", 1);
                dic.Add("操作人工号", 123456);
                SQLiteHelpers.InsertData("CurreUser", dic);
            }
            DataSet set5 = SQLiteHelpers.ExecuteDataSet4("UidEnTry", "用户名", "Admin");
            if (set5 == null || set5.Tables[0].Rows.Count < 1)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("用户名", "Admin");
                dic.Add("用户密码", LoginDialog.GetMD5Hash("adminbyd"));
                dic.Add("用户权限", "AMD");
                dic.Add("UID", "00000");
                dic.Add("最后登陆时间", "2000-01-01 00:00:00");
                dic.Add("登录次数", 0);
                dic.Add("工号", "10000");
                dic.Add("MES账号", "123");
                dic.Add("MES密码", "321");
                int res = EHomePage.SQLiteHelpers.InsertData("UidEnTry", dic);
            }

            //启动时加载今天记录（最多20条）
            LoadRecentRecords();
            ReadViewdatainfo();
            indexId = CommonHelper.LoadXmlInt("xml/IndexIdPara.xml", "IndexId");
            Thread threadP = new Thread(WorkFlow5);
            threadP.IsBackground = true;
            threadP.Start();

            Thread thread = new Thread(WorkFlow);
            thread.IsBackground = true;
            thread.Start();

            Thread thread2 = new Thread(WorkFlow2);
            thread2.IsBackground = true;
            thread2.Start();

            Thread thread12 = new Thread(WorkFlow3);
            thread12.IsBackground = true;
            thread12.Start();

            Thread thread14 = new Thread(WorkFlow4);
            thread14.IsBackground = true;
            thread14.Start();

            Thread thread4 = new Thread(WriteModel);
            thread4.IsBackground = true;
            thread4.Start();

            Thread thread5 = new Thread(WriteModel3);
            thread5.IsBackground = true;
            thread5.Start();

            List<Label> lblist=new List<Label>() { label43, label45, label47, label49};
            List<Label> lb2list = new List<Label>() { label80, label81, label82, label83 };
            _paramNameLabels = lblist;
            _paramRangeLabels = lb2list;
            UpdateRecipeParamLabels();
            //订阅配方变更事件：配方保存/切换后Label立即刷新
            RecipeChanged += () => CommonHelper.SafeInvoke(label43, UpdateRecipeParamLabels);

        }

        /// <summary>
        /// 加载今天数据库记录到 dgvData（最多20条，最新在后）
        /// 启动时和配方刷新后调用（InitDgvColumns 清空列后需要重新加载）
        /// </summary>
        private void LoadRecentRecords()
        {
            if (SQLiteHelpers == null) return;
            try
            {
                DataSet dataSet = SQLiteHelpers.ExecuteDataSet3(tableNameCur, true);
                if (dataSet != null && dataSet.Tables[0].Rows.Count > 0)
                {
                    int num = 0;
                    if (dataSet.Tables[0].Rows.Count >= 20)
                    {
                        num = dataSet.Tables[0].Rows.Count - 20;
                    }
                    for (int i = num; i < dataSet.Tables[0].Rows.Count; i++)
                    {
                        //按dgvData显示列配置从数据库读取各字段
                        string[] values = new string[DgvColumns.Count];
                        for (int j = 0; j < DgvColumns.Count; j++)
                        {
                            string dbField = DgvColumns[j].DbField;
                            values[j] = dataSet.Tables[0].Columns.Contains(dbField)
                                ? dataSet.Tables[0].Rows[i][dbField]?.ToString() ?? ""
                                : "";
                        }
                        AddToDgvData(values);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.push(new Log(Logger.LogType.ERROR, "EHomePage", "加载历史记录失败：" + ex.Message));
            }
        }

        //配方参数名称/上下限显示的Label
        private List<Label> _paramNameLabels;
        private List<Label> _paramRangeLabels;

        /// <summary>配方值被修改时触发（保存/切换配方）</summary>
        public static event Action RecipeChanged;
        /// <summary>触发 RecipeChanged 事件</summary>
        public static void RaiseRecipeChanged() => RecipeChanged?.Invoke();

        /// <summary>刷新配方参数名称/上下限到Label显示</summary>
        private void UpdateRecipeParamLabels()
        {
            if (_paramNameLabels == null) return;
            var ps = Recipedata.Data?.Parameters;
            //先清空
            for (int i = 0; i < _paramNameLabels.Count; i++)
            {
                _paramNameLabels[i].Text = "";
                _paramRangeLabels[i].Text = "";
            }
            if (ps != null)
            {
                for (int i = 0; i < ps.Count; i++)
                {
                    if (string.IsNullOrEmpty(ps[i].Name) || i > 3) continue;
                    //无上下限参数不显示名称与范围（主页仅监控有上下限的参数）
                    if (!ps[i].NeedLimit) continue;
                    _paramNameLabels[i].Text = ps[i].Name;
                    //范围标签只在上下限启用时显示；无上下限参数不显示范围
                    _paramRangeLabels[i].Text = ps[i].DownLimit + "->" + ps[i].UpLimit;
                }
            }
        }





        private bool myone = false;
        private void WriteModel3()
        {
            while (InitThread)
            {
                Thread.Sleep(500);
               
                if (ModbusClass.isConnected())
                {
                    if (!myone)
                    {
                        //EHomePage.jobnumber = ModbusClass.ReadString(工单号地址,12);
                        Thread.Sleep(100);
                        CommonHelper.SafeInvoke(txt_count, () => { txt_count.Text = EHomePage.jobnumber; });
                    }
                    if (EHomePage.jobnumber.Length < 6) continue;
                    int OK = 0;
                    int NG = 0;
                    for (int i = 0; i < 15; i++)
                    {
                        DateTime yesterday20 = DateTime.Now.Date.AddDays(-i);
                        string  todayTableName = "testRecord" + yesterday20.ToString("yyyyMMdd");
                       if (EHomePage.SQLiteHelpers.TableWxists(todayTableName))
                       {
                           OK=OK+EHomePage.SQLiteHelpers.ExecuteDataSetCount(todayTableName, "OK", EHomePage.jobnumber);
                           NG=NG+EHomePage.SQLiteHelpers.ExecuteDataSetCount(todayTableName, "NG", EHomePage.jobnumber);
                       }
                    }
                   
                        CommonHelper.SafeInvoke(lb_all, () => { lb_all.Text = (OK + NG).ToString(); });
                        CommonHelper.SafeInvoke(lb_ok, () => { lb_ok.Text = OK.ToString(); });
                        CommonHelper.SafeInvoke(lb_ng, () => { lb_ng.Text = NG.ToString(); });
                        if ((OK + NG)<1)
                        {
                        CommonHelper.SafeInvoke(lb_lv, () => { lb_lv.Text = "0.00%"; });
                        }
                        else
                        {
                        CommonHelper.SafeInvoke(lb_lv, () => { lb_lv.Text = (((float)OK / ((float)OK + (float)NG)) * 100).ToString("f2"); });
                        }
                 

                    if (!myone)
                    {
                        CommonHelper.SafeInvoke(txt_number, () => { txt_number.Text = (OK + NG).ToString(); });
                        myone = true;
                    }
                }
                Thread.Sleep(500);
            }
        }

        private void label7_Click(object sender, EventArgs e)
        {
            new Frm_MesModel().ShowDialog();
        }

        private int 工单号地址;
        private int 节拍地址;
        private int 机台运行状态地址;
        private int 模式地址;


        // private int 停机时长地址;
        //private int 在线模式地址;
        //private int 离线模式地址;
        //private int 单机模式地址;
        private int 上传MES信号地址;
        private int 上传MES成功信号地址;
        private int 请求扫码地址;
        private int 扫码完成结果地址;

        private int SN;
        private int 条码长度;
        private int 反控停机;
        private int 请求托盘绑定;
        private int 托盘绑定完成;
        private int PLC心跳;
        void ReadViewdatainfo()
        {
            //读取文件
            string filePath = "cfg\\Viewdata.cfg";
            StreamReader reader = new StreamReader(filePath);
            string str = reader.ReadToEnd();
            reader.Close();
            //解析文件并赋值给变量
            label14.Text = CommonHelper.MatchAttr(str, "产品名称");
            label15.Text = CommonHelper.MatchAttr(str, "工装绑定");

            条码长度 = int.Parse(CommonHelper.MatchAttr(str, "条码长度"));
            反控停机 = int.Parse(CommonHelper.MatchAttr(str, "反控停机").Substring(1));
            请求托盘绑定 = int.Parse(CommonHelper.MatchAttr(str, "请求托盘绑定").Substring(1));
            托盘绑定完成 = int.Parse(CommonHelper.MatchAttr(str, "托盘绑定完成").Substring(1));
            SN = int.Parse(CommonHelper.MatchAttr(str, "SN").Substring(1));
            // 工单号地址 = int.Parse(CommonHelper.MatchAttr(str, "工单号地址").Substring(1));
            节拍地址 = int.Parse(CommonHelper.MatchAttr(str, "节拍地址").Substring(1));
            机台运行状态地址 = int.Parse(CommonHelper.MatchAttr(str, "机台运行状态地址").Substring(1));

            模式地址 = int.Parse(CommonHelper.MatchAttr(str, "模式地址").Substring(1));
            //在线模式地址 = int.Parse(CommonHelper.MatchAttr(str, "在线模式地址").Substring(1));
            //离线模式地址 = int.Parse(CommonHelper.MatchAttr(str, "离线模式地址").Substring(1));
            //单机模式地址 = int.Parse(CommonHelper.MatchAttr(str, "单机模式地址").Substring(1));

            上传MES信号地址 = int.Parse(CommonHelper.MatchAttr(str, "上传MES信号地址").Substring(1));
            上传MES成功信号地址 = int.Parse(CommonHelper.MatchAttr(str, "上传MES成功信号地址").Substring(1));

            请求扫码地址 = int.Parse(CommonHelper.MatchAttr(str, "请求扫码地址").Substring(1));
            扫码完成结果地址 = int.Parse(CommonHelper.MatchAttr(str, "扫码完成结果地址").Substring(1));
            PLC心跳 = int.Parse(CommonHelper.MatchAttr(str, "PLC心跳").Substring(1));
        }




        string oldCode = "";
        private int mclicentId;
        SnResult mSnresult=null;
        HeartSend heartBeat = null;
        HeartRev heartRev = new HeartRev();
        ResponseUploadSNResult uploadSNResult = null;
        UploadpalletInfoResponse uploadpalletInfoResponse = null;
        /// <summary>
        /// 托盘码
        /// </summary>
        string Psn = "";

        /// <summary>
        /// 接收到的物料编码，需要和当前配方名对比，如果不一致，自动切换
        /// </summary>
        public string revMaterialCode = "";

        #region  自动作业流程
        /// <summary>
        ///  请求SN或从寄存器读取地址
        /// </summary>
        private void WorkFlow()
        {
            while (InitThread)
            {
            Flowstart:
                try
                {

                    //读取扫码信息
                    if (ModbusClass.ReadSingleHoldingRegisters(请求扫码地址) == 1)
                    {
                        //条码过站验证，读取条码,在自动运行状态下
                        ModbusClass.WriteSingleRegister(请求扫码地址, 0);
                        //和MES交互，获取结果

                        DateTime waitScanClear = DateTime.Now;

                        if (MESModel == "在线")//在线模式下请求sn
                        {
                            #region  请求sn
                            mSnresult = null;
                            var res = new SnRequest
                            {
                                ClientId = mclicentId,
                                Quantity = "1",
                                DataType = "RequestSN",
                            };
                            string json = JsonConvert.SerializeObject(res, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });
                            Communic.TcpCltfeedingA.Send(json + "$", out string msg);

                            //等待MES回复
                            while (InitThread)
                            {
                                if (mSnresult != null)
                                {
                                    break;
                                }
                                if ((DateTime.Now - waitScanClear).TotalSeconds > 5)
                                {
                                    Logger.push(new Log(Logger.LogType.ERROR, "EHomePage", "清空扫码信号超时"));
                                    break;
                                }
                                Thread.Sleep(20);
                            }
                            //如果超时，退出，重新等待PLC请求扫码信号
                            if (mSnresult == null)
                            {
                                while (InitThread && ModbusClass.isConnected())
                                {
                                    if (ModbusClass.ReadSingleHoldingRegisters(请求扫码地址) == 0) break;
                                    else
                                        ModbusClass.WriteSingleRegister(请求扫码地址, 0);

                                    if ((DateTime.Now - waitScanClear).TotalSeconds > 5)
                                    {
                                        Logger.push(new Log(Logger.LogType.ERROR, "EHomePage", "清空扫码信号超时"));
                                        break;
                                    }
                                    Thread.Sleep(100);
                                }
                                // break;
                                ShowMsg("请求SN未收到MES回复", 2);
                                goto Flowstart;
                            }
                            #endregion


                            string result = "OK";
                            if (mSnresult.Result != "Success")
                            {
                                result = "NG";
                            }
                            CommonHelper.SafeInvoke(lb_proCode, () => { lb_proCode.Text = board_sn = mSnresult.SN; });
                            if (result == "OK")
                            {
                                //过站结果OK返回1，NG返回4
                                ModbusClass.WriteSingleRegister(扫码完成结果地址, 1);//OK
                                Logger.push(new Log(Logger.LogType.INFO, "EHomePage", "发送给PLC信号为：1"));
                            }
                            else
                            {
                                ModbusClass.WriteSingleRegister(扫码完成结果地址, 2);//NG
                                Logger.push(new Log(Logger.LogType.INFO, "EHomePage", "发送给PLC信号为：2"));
                            }
                        }
                        else
                        {

                            board_sn = "Offline" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//非在线模式
                            ShowMsg("离线模式，条码生成：" + board_sn, 1);
                            ModbusClass.WriteSingleRegister(扫码完成结果地址, 1);//OK
                            Logger.push(new Log(Logger.LogType.INFO, "EHomePage", "发送给PLC信号为：1"));
                        }


                        //  ModbusClass.WriteString(2000, board_sn);
                        CommonHelper.SafeInvoke(lb_zt, () => { lb_zt.Text = "扫码完成"; });
                        ShowMsg("扫码结果：" + board_sn, 1);
                        TestTimer = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        mSnresult = null;
                        //清空信号（带超时和断线检测）
                        waitScanClear = DateTime.Now;
                        while (InitThread && ModbusClass.isConnected())
                        {
                            if (ModbusClass.ReadSingleHoldingRegisters(请求扫码地址) == 0) break;
                            else
                                ModbusClass.WriteSingleRegister(请求扫码地址, 0);

                            if ((DateTime.Now - waitScanClear).TotalSeconds > 5)
                            {
                                Logger.push(new Log(Logger.LogType.ERROR, "EHomePage", "清空扫码信号超时"));
                                break;
                            }
                            Thread.Sleep(100);
                        }


                    }
                }
                catch (Exception ex)
                {
                    ShowMsg(ex.Message, 2);
                    Thread.Sleep(3000);
                }
                #region
                //视觉结果处理
                //if (Communic.RecfeedC.Length>5)
                //{
                //    if (Communic.RecfeedC.Contains("OK")|| Communic.RecfeedC.Contains("NG"))
                //    {
                //        lb_zt.Invoke(new Action(() => { lb_zt.Text = "获取视觉数据"; }));
                //        //获取到数据
                //        VisonRes = Communic.RecfeedC;
                //        if (VisonRes.Split(',')[2] != "OK" && VisonRes.Split(',')[2] != "NG")
                //        {
                //            float res = (float)Convert.ToDouble(VisonRes.Split(',')[2]);
                //            //ModbusClass.WriteRegisters(2304, res);
                //            if (res > Convert.ToDouble(Recipedata.Data.FatSpeedUp) || res < Convert.ToDouble(Recipedata.Data.FatSpeeddown))
                //            {
                //                passStandar1.FatSpeedResult = "NG";
                //            }
                //            else
                //            {
                //                passStandar1.FatSpeedResult = "OK";
                //            }

                //            //ModbusClass.WriteString(2306, passStandar1.FatSpeedResult);

                //        }
                //        ShowMsg("视觉结果：" + Communic.RecfeedC, 1);
                //        Communic.RecfeedC = "";
                //    }
                //}
                #endregion

                Thread.Sleep(10);
            }

        }
        /// <summary>
        /// 上传SN加工结果
        /// </summary>
        private void WorkFlow2()
        {
            while (InitThread)
            {
                try
                {
                    //读取完成信号，MES上传
                    if (ModbusClass.ReadSingleHoldingRegisters(上传MES信号地址) == 1)
                    {
                        CommonHelper.SafeInvoke(lb_zt, () => { lb_zt.Text = "上传MES开始"; });
                        Logger.push(new Log(Logger.LogType.INFO, "EHomePage", "接收到PLC信号，上传MES开始"));
                        //上传数据组装
                        passStandar1.ProduceCode = board_sn;
                        CommonHelper.SafeInvoke(lb_sn, () => { lb_sn.Text = _sn; });


                        //按配方参数动态判定：每项从PLC读实际值，对比上下限，全部OK则总结果OK   判断是否从PLC中读取
                        bool allOk = JudgeRecipeParameters();
                        passStandar1.TestResult = allOk ? "OK" : "NG";
                        CommonHelper.SafeInvoke(lb_result, () =>
                        {
                            lb_result.Text = passStandar1.TestResult;
                            lb_result.BackColor = allOk ? Color.Lime : Color.Red;
                        });

                        GetData();
                        ChangeData(passStandar1);
                        if (MESModel == "在线")
                        {
                            //组装数据，并上传MES
                            //Data 从配方参数生成：paramName=配方Name，RealValue=PLC实际值，CheckResult=OK写1/NG写0
                            var res = new UploadSNResult
                            {
                                DateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                Sender = "0001",
                                ClientId = mclicentId,
                                DataType = "UploadSNResult",
                                MaterialCode = "",
                                Message = "",
                                Result = passStandar1.TestResult,
                                SN = board_sn,
                                Data = new List<UploadSNResultParam>()
                            };
                            var recipeParams = Recipedata.Data?.Parameters;
                            if (recipeParams != null)
                            {
                                foreach (var p in recipeParams)
                                {
                                    //string checkResult = "";
                                    if (string.IsNullOrEmpty(p.Name)) continue;
                                    passStandar1.DynamicParams.TryGetValue(p.Name + "实际值", out string realVal);
                                    passStandar1.DynamicParams.TryGetValue(p.Name + "结果", out string checkVal);
                                    if (checkVal != "1") checkVal = "0";
                                    res.Data.Add(new UploadSNResultParam
                                    {
                                        paramCode = "",
                                        paramName = p.Name,
                                        RealValue = realVal ?? "",
                                        CheckResult = checkVal
                                    });
                                }
                            }
                            string json = JsonConvert.SerializeObject(res, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });
                            uploadSNResult = null;
                            Communic.TcpCltfeedingA.Send(json + "$", out string msg);

                            //判断是否超时处理,等待MES返回结果
                            int waitMs = 5 * 1000;
                            Stopwatch sw = Stopwatch.StartNew();
                            while (InitThread)
                            {

                                if (uploadSNResult != null)//有返回跳出循环
                                {
                                    break;
                                }
                                if (sw.ElapsedMilliseconds > waitMs)
                                {
                                    ShowMsg("MES超时! 上传加工结果MES未回复", 2);
                                    break;
                                }
                            }
                            if (uploadSNResult != null && uploadSNResult?.Result == "Success")
                            {
                                ModbusClass.WriteSingleRegister(上传MES成功信号地址, 1);//OK
                                DateTime waitOk = DateTime.Now;
                                while (InitThread && ModbusClass.isConnected())
                                {
                                    if (ModbusClass.ReadHoldingRegisters(上传MES成功信号地址, 1)[0] == 1) break;
                                    else
                                        ModbusClass.WriteSingleRegister(上传MES成功信号地址, 1);//OK
                                    if ((DateTime.Now - waitOk).TotalSeconds > 5)
                                    {
                                        Logger.push(new Log(Logger.LogType.ERROR, "EHomePage", "写入MES成功信号超时"));
                                        break;
                                    }
                                    Thread.Sleep(100);
                                }
                            }
                            else
                            {
                                ModbusClass.WriteSingleRegister(上传MES成功信号地址, 2);//OK
                                DateTime waitNg = DateTime.Now;
                                while (InitThread && ModbusClass.isConnected())
                                {
                                    if (ModbusClass.ReadHoldingRegisters(上传MES成功信号地址, 1)[0] == 2) break;
                                    else
                                        ModbusClass.WriteSingleRegister(上传MES成功信号地址, 2);//OK
                                    if ((DateTime.Now - waitNg).TotalSeconds > 5)
                                    {
                                        Logger.push(new Log(Logger.LogType.ERROR, "EHomePage", "写入MES失败信号超时"));
                                        break;
                                    }
                                    Thread.Sleep(100);
                                }
                            }
                        }
                        else
                        {
                            ShowMsg("离线模式，默认上传SN加工结果成功", 1);
                            //不是在线模式，就返回成功信息给PLC
                            ModbusClass.WriteSingleRegister(上传MES成功信号地址, 1);//OK
                            DateTime waitOk = DateTime.Now;
                            while (InitThread && ModbusClass.isConnected())
                            {
                                if (ModbusClass.ReadHoldingRegisters(上传MES成功信号地址, 1)[0] == 1) break;
                                else
                                    ModbusClass.WriteSingleRegister(上传MES成功信号地址, 1);//OK
                                if ((DateTime.Now - waitOk).TotalSeconds > 5)
                                {
                                    Logger.push(new Log(Logger.LogType.ERROR, "EHomePage", "写入MES成功信号超时"));
                                    break;
                                }
                                Thread.Sleep(100);
                            }
                        }
                        //写入本地数据库
                        AddToSqliteDLS(passStandar1);

                        if (oldCode != passStandar1.ProduceCode)//防止同一个产品连续上传
                        {
                            //MES上传,后面因该不需要上传总控了，总控会加域
                            //UploadResultToMes();
                            //结果添加到界面显示（按dgvData显示列配置组装）
                            AddToDgvData(BuildDgvRowValues());
                        }

                        if (MESModel == "单机")
                        {
                            Logger.push(new Log(Logger.LogType.INFO, "EHomePage", "数据:" + Js("passStandar")));
                        }

                        CommonHelper.SafeInvoke(lb_zt, () => { lb_zt.Text = "上传MES正常结束"; });
                        Logger.push(new Log(Logger.LogType.INFO, "EHomePage", "上传MES正常结束"));
                        ModbusClass.WriteSingleRegister(上传MES信号地址, 0);
                        oldCode = passStandar1.ProduceCode;
                        DateTime waitClear = DateTime.Now;
                        while (InitThread && ModbusClass.isConnected())
                        {
                            if (ModbusClass.ReadSingleHoldingRegisters(上传MES信号地址) == 0) break;
                            else
                                ModbusClass.WriteSingleRegister(上传MES信号地址, 0);
                            if ((DateTime.Now - waitClear).TotalSeconds > 5)
                            {
                                Logger.push(new Log(Logger.LogType.ERROR, "EHomePage", "清空上传MES信号超时"));
                                break;
                            }
                            Thread.Sleep(100);
                        }
                    }
                    uploadSNResult = null;
                    Thread.Sleep(10);
                }
                catch (Exception ex)
                {
                    ShowMsg(ex.Message, 2);
                    Thread.Sleep(3000);
                }
            }

        }
        /// <summary>
        /// 处理MES返回信息,将原始信息根据类型存入不同的对象
        /// </summary>
        private void WorkFlow3()
        {
            //MES连接
            while (InitThread)
            {
                try
                {
                    // 需要加个等待机制
                    if (Communic.RecfeedAQueue.TryDequeue(out string recfeedData))
                    {

                        string json = "";
                        //recfeedData = recfeedData.Substring(0, recfeedData.Length - 1);
                        if (recfeedData.Contains("$"))
                        {
                            recfeedData = recfeedData.Split('$')[0];
                        }

                        // 心跳
                        if (recfeedData.Contains("HeartBeat"))
                        {
                            JObject result = JObject.Parse(recfeedData);
                            int state_re = mclicentId = (int)result["ClientId"];//获取
                                                                                //   heartBeat = JsonConvert.DeserializeObject<HeartSend>(recfeedData);
                            heartRev = JsonConvert.DeserializeObject<HeartRev>(recfeedData);
                            if (heartRev.MesStatus.Equals("normal"))
                            {
                                ModbusClass.WriteSingleRegister(反控停机, 1);
                            }
                            else
                            {
                                ModbusClass.WriteSingleRegister(反控停机, 2);
                            }
                        }
                        // 请求SN  返回的数据
                        else if (recfeedData.Contains("ResponseRequestSN"))
                        {
                            mSnresult = JsonConvert.DeserializeObject<SnResult>(recfeedData);
                        }
                        //上报SN加工结果 返回的数据
                        else if (recfeedData.Contains("ResponseUploadSNResult"))
                        {
                            uploadSNResult = JsonConvert.DeserializeObject<ResponseUploadSNResult>(recfeedData);
                        }

                        //设备托盘绑定信息  返回的数据
                        else if (recfeedData.Contains("ResponseUploadpalletInfo"))
                        {
                            uploadpalletInfoResponse = JsonConvert.DeserializeObject<UploadpalletInfoResponse>(recfeedData);

                        }
                        //服务端下发工艺参数  
                        else if (recfeedData.Contains("RequestUploadDeviceParameters"))
                        {
                            RequestUploadDeviceParameters ret = JsonConvert.DeserializeObject<RequestUploadDeviceParameters>(recfeedData);
                            revMaterialCode = ret.MaterialCode;
                            ShowMsg($"从服务端获取到的MaterialCode:{revMaterialCode}", 1);
                            ResponseRequestUploadDeviceParameters response = new ResponseRequestUploadDeviceParameters();
                            response.DataType = "ResponseRequestUploadDeviceParameters";
                            response.Sender = "0001";
                            response.ClientId = mclicentId;
                            response.DateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                            Communic.TcpCltfeedingA.Send(response.ToJsonString() + "$", out string msg);
                            //判断是否和当前配方一致  
                            if (Recipedata.GetCur != revMaterialCode && (!string.IsNullOrEmpty(revMaterialCode)))
                            {
                                ShowMsg($"与当前配方不一致，开始切换配方", 1);
                                bool rtn = Recipedata.Change(revMaterialCode, out msg);
                                Project.Instance = JsonFile.ReadJsonFile<Project>(PathConfig.Instance.ProjectPath);

                                if (rtn)
                                {
                                    ShowMsg($"切换配方:{revMaterialCode}成功", 1);
                                }
                                else
                                {
                                    ShowMsg($"切换配方:{revMaterialCode}失败", 1);
                                    //todo  切换失败处理
                                }
                            }

                        }




                        #region 弃用
                        // 设备状态上传接口协议 返回的数据
                        else if (recfeedData.Contains("ResponseUploadEquipmentStatus"))
                        {
                            MesResult snsend = JsonConvert.DeserializeObject<MesResult>(recfeedData);
                        }
                        // 设备报警上传接口协议 返回的数据
                        else if (recfeedData.Contains("ResponseUploadAlarmMessage"))
                        {
                            MesResult snsend = JsonConvert.DeserializeObject<MesResult>(recfeedData);
                        }
                        // 服务端主动下发SN
                        else if (recfeedData.Contains("DistributeSN"))
                        {
                            SnSend snsend = JsonConvert.DeserializeObject<SnSend>(recfeedData);
                            var res = new MesResult
                            {
                                ClientId = snsend.ClientId,
                                Result = "Success",
                                DataType = "ResponseDistributeSN",
                                Message = "",
                            };
                            json = JsonConvert.SerializeObject(res, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });
                            Communic.TcpCltfeedingA.Send(json + "$", out string msg);
                        }
                        // 下发设备参数和程序接口协议
                        else if (recfeedData.Contains("DistributeDeviceParameters"))
                        {
                            DeviceParameters snsend = JsonConvert.DeserializeObject<DeviceParameters>(recfeedData);
                            var res = new MesResult
                            {
                                ClientId = snsend.ClientId,
                                Result = "Success",
                                DataType = "ResponseDistributeDeviceParameters",
                                Message = "",
                            };
                            json = JsonConvert.SerializeObject(res, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });
                            Communic.TcpCltfeedingA.Send(json + "$", out string msg);
                        }
                        // 上传设备参数和程序接口协议
                        else if (recfeedData.Contains("ResponseUploadDeviceParameters"))
                        {
                            MesResult snsend = JsonConvert.DeserializeObject<MesResult>(recfeedData);
                        }
                        // 上传产品SN进站时间接口协议
                        else if (recfeedData.Contains("ResponseUploadInStationTime"))
                        {
                            MesResult snsend = JsonConvert.DeserializeObject<MesResult>(recfeedData);
                        }
                        // 设备反控接口协议
                        else if (recfeedData.Contains("DevicePause") || recfeedData.Contains("DeviceRun"))
                        {
                            MesResult snsend = JsonConvert.DeserializeObject<MesResult>(recfeedData);
                            string Result2 = "";
                            if (recfeedData.Contains("DeviceRun"))
                            {
                                Result2 = "ResponseDeviceRun";
                            }
                            else
                            {
                                Result2 = "ResponseDevicePause";
                            }
                            var res = new MesResult
                            {
                                ClientId = snsend.ClientId,
                                Result = "Success",
                                DataType = Result2,
                                Message = "",
                            };
                            json = JsonConvert.SerializeObject(res, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });
                            Communic.TcpCltfeedingA.Send(json + "$", out string msg);
                        }
                        // 编码下发，配方切换
                        else if (recfeedData.Contains("RecipeSwitchover"))
                        {
                            RecipeSwitchover snsend = JsonConvert.DeserializeObject<RecipeSwitchover>(recfeedData);
                            var res = new MesResult
                            {
                                ClientId = snsend.ClientId,
                                Result = "Success",
                                DataType = "ResponseRecipeSwitchover",
                                Message = "",
                            };
                            json = JsonConvert.SerializeObject(res, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });
                            Communic.TcpCltfeedingA.Send(json + "$", out string msg);
                        }
                        // 不良信息上传
                        else if (recfeedData.Contains("ResponseUploadDefective"))
                        {
                            MesResult snsend = JsonConvert.DeserializeObject<MesResult>(recfeedData);
                        }
                        // 不良品SN上件接口协议
                        else if (recfeedData.Contains("ResponseDefectiveSN"))
                        {
                            MesResult snsend = JsonConvert.DeserializeObject<MesResult>(recfeedData);
                        }
                        #endregion


                    }
                    else
                    {
                        // 队列空，休息一下，避免CPU空转
                        Thread.Sleep(10);  // 或者用更优雅的方式
                    }
                    Thread.Sleep(20);
                }
                catch (Exception ex)
                {
                    ShowMsg(ex.Message, 2);
                    Thread.Sleep(3000);
                }
            }
        }

        //MES心跳
        private void WorkFlow4()
        {
            //心跳，主动请求
            while (InitThread)
            {
                try
                {
                    // 需要加个等待机制
                    string json = "";
                    if (mclicentId > 0)
                    {
                        var res = new HeartSend
                        {
                            DateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            Sender = "0001",
                            DataType = "ResponseHeartBeat",
                            ClientId = mclicentId,
                            Message = "心跳成功",
                            DeviceStatus = machineState,
                        };
                        json = JsonConvert.SerializeObject(res, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        });
                        heartBeat = null;
                        Communic.TcpCltfeedingA.Send(json + "$", out string msg);
                        int waitMs = MES超时 * 1000;
                        Stopwatch sw = Stopwatch.StartNew();
                        //判断是否超时处理
                        while (InitThread)
                        {
                            if (sw.ElapsedMilliseconds > waitMs)
                            {
                                ShowMsg("MES心跳超时!", 2);
                                break;
                            }
                            if (heartBeat != null)//有返回跳出循环
                            {
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowMsg(ex.Message, 2);

                }
                Thread.Sleep(MES心跳 * 1000);
            }
        }

        /// <summary>
        /// 获取托盘码
        /// </summary>
        private void WorkFlow5()
        {
            while (InitThread)
            {
                try
                {
                    if (ModbusClass.ReadSingleHoldingRegisters(请求托盘绑定) == 1)
                    {
                        Psn = ModbusClass.ReadString(SN, 条码长度);
                        ShowMsg($"  从寄存器D{SN}读取到条码{Psn} ,长度{条码长度} ", 1);


                        UploadpalletInfo uploadpalletInfo = new UploadpalletInfo();

                        uploadpalletInfo.DateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        uploadpalletInfo.DataType = "ResponseUploadpalletInfo";
                        uploadpalletInfo.ClientId = mclicentId;
                        uploadpalletInfo.PalletCode = Psn;

                        ModbusClass.WriteSingleRegister(请求托盘绑定, 0);

                        Communic.TcpCltfeedingA.Send(uploadpalletInfo.ToJsonString() + "$", out string msg);

                        while (InitThread)
                        {
                            if (uploadpalletInfoResponse != null)
                            {
                                break;
                            }
                            Thread.Sleep(20);
                        }
                        string result = "OK";
                        if (uploadpalletInfoResponse.Result != "Success")
                        {
                            result = "NG";
                        }
                        if (result == "OK")
                        {
                            ModbusClass.WriteSingleRegister(托盘绑定完成, 1);//OK
                            ShowMsg($"托盘绑定信息上传成功 发送给PLC信号为：1", 1);

                            if (Config.Instance.GetSNByModbus)
                            {
                                board_sn = uploadpalletInfoResponse.SN;
                                //通过Modbus获取托盘码，然后通过托盘码获取SN,
                            }
                        }
                        else if (result == "NG")
                        {
                            ModbusClass.WriteSingleRegister(托盘绑定完成, 2);//NG
                            ShowMsg($"托盘绑定信息上传失败 发送给PLC信号为：2", 2);

                            if (Config.Instance.GetSNByModbus)
                            {
                                board_sn = "";
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowMsg(ex.Message, 2);
                    Thread.Sleep(3000);
                }
                Thread.Sleep(10);
            }



        }

        /// <summary>
        /// PLC心跳
        /// </summary>
        private void WorkFlow6()
        {
            while (InitThread)
            {
                try
                {
                    ModbusClass.WriteSingleRegister(PLC心跳, 1);
                }
                catch (Exception ex)
                {
                    ShowMsg(ex.Message, 2);
                    Thread.Sleep(3000);
                }
                Thread.Sleep(3000);


            }
        }

        #endregion



        private void ShowMsg(string str, int index)
        {

            if (rTxtLog.InvokeRequired)
            {
                CommonHelper.SafeInvoke(rTxtLog, () =>
                {
                    lock (objShowMsg)
                    {
                        rTxtLog.SelectionStart = rTxtLog.Text.Length;
                        rTxtLog.AppendText("");
                        switch (index)
                        {
                            case 0:
                                {
                                    rTxtLog.SelectionColor = Color.Black;
                                    Logger.push(new Log(Logger.LogType.WARNING, "thread", str));
                                }
                                break;
                            case 1:
                                {
                                    rTxtLog.SelectionColor = Color.Green;
                                    Logger.push(new Log(Logger.LogType.INFO, "thread", str));
                                }
                                break;
                            case 2:
                                {
                                    rTxtLog.SelectionColor = Color.Red;
                                    Logger.push(new Log(Logger.LogType.ERROR, "thread", str));
                                }
                                break;
                        }


                        rTxtLog.AppendText(DateTime.Now.ToString() + "：" + str + "\r\n");
                        //自动裁剪日志，防止无限增长导致界面卡顿
                        CommonHelper.TrimLogText(rTxtLog);
                        //光标移动到末尾

                        rTxtLog.ScrollToCaret();
                    }
                });
            }
            else
            {
                //txtLog.Text = str + "\r\n"+txtLog.Text;
                lock (objShowMsg)
                {
                    rTxtLog.SelectionStart = rTxtLog.Text.Length;
                    rTxtLog.AppendText("");
                    switch (index)
                    {

                        case 0:
                            {
                                rTxtLog.SelectionColor = Color.Black;
                                Logger.push(new Log(Logger.LogType.WARNING, "mainwindow", str));
                            }
                            break;
                        case 1:
                            {
                                rTxtLog.SelectionColor = Color.Green;
                                Logger.push(new Log(Logger.LogType.INFO, "mainwindow", str));
                            }
                            break;
                        case 2:
                            {
                                rTxtLog.SelectionColor = Color.Red;
                                Logger.push(new Log(Logger.LogType.ERROR, "mainwindow", str));
                            }
                            break;
                    }
                    rTxtLog.AppendText(DateTime.Now.ToString() + "：" + str + "\r\n");
                    //自动裁剪日志，防止无限增长导致界面卡顿
                    CommonHelper.TrimLogText(rTxtLog);
                    rTxtLog.ScrollToCaret();
                }

            }
        }

        //展示到新的表格（参数改为可变数组，按DgvColumns配置顺序填充）
        private void AddToDgvData(params string[] values)
        {
            object[] row = new object[DgvColumns.Count + 1];
            for (int i = 0; i < DgvColumns.Count && i < values.Length; i++)
            {
                row[i] = values[i];
            }
            if (dgvData.InvokeRequired)
            {
                CommonHelper.SafeInvoke(dgvData, () =>
                {
                    if (dgvData.Rows.Count >= 20)
                    {
                        DataGridViewRow dataGridView = dgvData.Rows[dgvData.Rows.Count - 1];
                        dgvData.Rows.Remove(dataGridView);
                    }
                    dgvData.Rows.Insert(0, row);
                });
            }
            else
            {
                if (dgvData.Rows.Count >= 20)
                {
                    DataGridViewRow dataGridView = dgvData.Rows[dgvData.Rows.Count - 1];
                    dgvData.Rows.Remove(dataGridView);
                }
                dgvData.Rows.Insert(0, row);
            }
        }

        int MES心跳 = 3;
        int MES超时 = 30;
        //读取配置文件
        public void ReadConfig()
        {
            //读取文件
            string filePath = "cfg\\mes.cfg";
            StreamReader reader = new StreamReader(filePath);
            string str = reader.ReadToEnd();
            reader.Close();

            //解析文件并赋值给变量
            _SITE = CommonHelper.MatchAttr(str, "SITE");
            _OPERATION = CommonHelper.MatchAttr(str, "OPERATION");
            _RESOURCE = CommonHelper.MatchAttr(str, "RESOURCE");
            _NC_CODE = CommonHelper.MatchAttr(str, "NC_CODE");
            _IP = CommonHelper.MatchAttr(str, "IP");
            _PORT = CommonHelper.MatchAttr(str, "PORT");
            _URL = CommonHelper.MatchAttr(str, "URL");
            _USER = CommonHelper.MatchAttr(str, "USER");
            _PWD = CommonHelper.MatchAttr(str, "PASSWORD");

            MES心跳 = int.Parse(CommonHelper.MatchAttr(str, "MES心跳"));
            MES超时 = int.Parse(CommonHelper.MatchAttr(str, "MES超时"));

            _factoryCode = CommonHelper.MatchAttr(str, "factoryCode");
            _userName = CommonHelper.MatchAttr(str, "userName");
            _passWord = CommonHelper.MatchAttr(str, "passWord");
            _machineCode = CommonHelper.MatchAttr(str, "machineCode");
            _testFile = CommonHelper.MatchAttr(str, "testFile");
            _testFileVersion = CommonHelper.MatchAttr(str, "testFileVersion");
            _stationCode = CommonHelper.MatchAttr(str, "stationCode");
            _operationCode = CommonHelper.MatchAttr(str, "operationCode");
            badtype = CommonHelper.MatchAttr(str, "badType");

            _URLout = CommonHelper.MatchAttr(str, "URLout");
            _URLstart = CommonHelper.MatchAttr(str, "_URLstart");
            _URLpass = CommonHelper.MatchAttr(str, "_URLpass");
            _URLerror = CommonHelper.MatchAttr(str, "_URLerror");


            _URLshoporder = CommonHelper.MatchAttr(str, "_URLshoporder");

            URLshoporder = "http://" + _IP + ":" + _PORT + _URLshoporder;
            _URLF = "http://" + _IP + ":" + _PORT + _URL;

            URLstart = "http://" + _IP + ":" + _PORT + _URLstart;
            URLpass = "http://" + _IP + ":" + _PORT + _URLpass;
            URLerror = "http://" + _IP + ":" + _PORT + _URLerror;

            board_SITE = _SITE;
            board_OPERATION = _OPERATION;
            board_RESOURCE = _RESOURCE;
            board_NC_CODE = _NC_CODE;
            board_IP = _IP;
            board_PORT = _PORT;
            board_URL = _URL;
            board_USER = _USER;
            board_PWD = _PWD;



            board_URLF = _URLF;
        }

        //上传MES到总控
        private void UploadResultToMes()
        {
            //发送pass请求
            try
            {
                JObject result = JObject.Parse(CommonHelper.PostJson(_URLout, Js("passStandar2")));
                Logger.push(new Log(Logger.LogType.INFO, "EHomePage", "上传总控MES结果：" + result.ToString()));
                string state_re = (string)result["state"];
                if (state_re == "N")
                {

                    ShowMsg("条码:" + passStandar1.ProduceCode + "上传结果到总控失败：" + (string)result["message"], 2);
                }
                else
                {

                    ShowMsg("条码:" + passStandar1.ProduceCode + "上传结果到总控成功！", 1);
                }
            }
            catch (Exception ex)
            {

                ShowMsg("发送PASS请求失败，错误原因：" + ex.Message, 2);
            }
        }


        public static string jobcount = "100";
        public static string jobnumber = "123";
        public static string jobrescount = "10";
        public static string jobres = "10%";

        /// <summary>
        /// 整合数据
        /// </summary>
        public void GetData()
        {
            passStandar1.CurreUser = Getuser();
            passStandar1.TestTime = TestTimer;
            Thread.Sleep(50);
            passStandar1.CT = ModbusClass.ReadDoubleRegister(节拍地址);
        }
        /// <summary>
        /// 组装数据，为了上传
        /// </summary>
        public void ChangeData(MesdataDLS data)
        {
            array.Clear();

            //配方参数动态生成 TestItem（名字/上限/下限/实际值/结果）
            var ps = Recipedata.Data?.Parameters;
            if (ps != null)
            {
                foreach (var p in ps)
                {
                    if (string.IsNullOrEmpty(p.Name)) continue;
                    //无上下限参数不上传上限/下限（结果由外部判定，无上下限可传）
                    if (p.NeedLimit)
                    {
                        AddTestItem(p.Name + "检测上限", data.DynamicParams.TryGetValue(p.Name + "上限", out string up) ? up : "");
                        AddTestItem(p.Name + "检测下限", data.DynamicParams.TryGetValue(p.Name + "下限", out string dn) ? dn : "");
                    }
                    AddTestItem(p.Name + "检测实际值", data.DynamicParams.TryGetValue(p.Name + "实际值", out string av) ? av : "");
                    AddTestItem(p.Name + "检测判断结果", data.DynamicParams.TryGetValue(p.Name + "结果", out string rs) ? rs : "");
                }
            }

            //固定信息：按 StutableConfig.xml 的 Source 配置取值（反射取配方/MES数据属性）
            var recipe = Recipedata.Data;
            foreach (var cfg in _stutableConfigs)
            {
                AddTestItem(cfg.Name, GetConfigColumnValue(cfg.Source, data, recipe));
            }
        }

        private void AddTestItem(string name, string value)
        {
            TestItem item = new TestItem();
            item.testItem = name;
            item.testParameter = "";
            item.testValue = value ?? "";
            array.Add(JObject.Parse(JsonConvert.SerializeObject(item)));
        }

        public string Js(string className)
        {
            string json = string.Empty;
            Type type = Type.GetType(className);
            object instance = new object();
            switch (className)
            {
                case "passStandar2":
                    //需要上传的数据
                    TestMes passStandar2 = new TestMes();
                    passStandar2.stationName = MainWindow.title;
                    passStandar2.receiveTime = "";
                    passStandar2.dataList = array;
                    passStandar2.flag = "F";
                    instance = passStandar2;
                    break;

                default:
                    throw new Exception("输入类名错误");

            }
            json = JsonConvert.SerializeObject(instance, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            return json;
        }
        //清理数据
        public void ClearData()
        {          

        }

        //表格自加序
        private void Sortdgv(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var grid = sender as DataGridView;
            var rowidx = (e.RowIndex + 1).ToString();
            var centerFormat = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            var headerBounds = new System.Drawing.Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowidx, new Font("宋体", 12), SystemBrushes.ControlText, headerBounds, centerFormat);
        }
        private void dgvData_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Sortdgv(sender,e);
        }      
       
        public static bool visionbool=false;

        //连接状态
        private void timer2_Tick(object sender, EventArgs e)
        {

            switch (LoginDialog.LoginStatus)
            {
                case 1:
                    lb_user.Text = "操作员";

                    break;
                case 2:
                    lb_user.Text = "设备工程师";

                    break;
                case 3:
                    lb_user.Text = "品质工程师";

                    break;
                case 4:
                    lb_user.Text = "工艺工程师";

                    break;
                case 5:
                    lb_user.Text = "超级管理员";

                    break;
                default:
                    lb_user.Text = "操作员";
                    break;
            }
            txt_rec.Text = Recipedata.GetCur;

        }
        private string lastmachineState = "free";
        private string machineState = "free";
        private string MachineStatusBegin = string.Empty;
        /// <summary>
        /// 设备节拍,MES模式,设备状态获取  和  设备状态上传
        /// </summary>
        private void WriteModel()
        {
            while (InitThread)
            {
                Thread.Sleep(200);
                if (ModbusClass.isConnected())
                {
                    CommonHelper.SafeInvoke(txtCT, () =>
                    {
                        txtCT.Text = ModbusClass.ReadDoubleRegister(节拍地址).ToString("f2") + " S";
                    });
                    int state = ModbusClass.ReadSingleHoldingRegisters(机台运行状态地址);
                    machineState = "";
                    switch (state)
                    {

                        case 1:
                            {
                                machineState = "free";
                            }
                            break;
                        case 2:
                            {
                                machineState = "free";
                            }
                            break;
                        case 3:
                            {
                                machineState = "free";
                            }
                            break;
                        case 4:
                            {
                                machineState = "free";
                            }
                            break;
                        case 5:
                            {
                                machineState = "run";
                            }
                            break;
                        case 6:
                            {
                                machineState = "free";
                            }
                            break;
                        case 7:
                            {
                                machineState = "fault";
                            }
                            break;
                        case 8:
                            {
                                machineState = "free";
                            }
                            break;
                        default:
                            {
                                machineState = "free";
                            }
                            break;


                    }

                    //设备状态上传
                    //if (lastmachineState != machineState)
                    //{
                    //    lastmachineState = machineState;
                    //    string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    //    UpEquipmentStatus upEquipmentStatus = new UpEquipmentStatus();
                    //    upEquipmentStatus.DateTime = date;
                    //    upEquipmentStatus.Sender = "0001";
                    //    upEquipmentStatus.ClientId = mclicentId;
                    //    upEquipmentStatus.Data.MachineStatusCode = machineState;
                    //    if (string.IsNullOrEmpty(MachineStatusBegin))
                    //    {
                    //        MachineStatusBegin = date;
                    //    }
                    //    upEquipmentStatus.Data.MachineStatusBegin = MachineStatusBegin;
                    //    upEquipmentStatus.Data.MachineStatusEnd = date;
                    //    MachineStatusBegin = date;
                    //    Communic.TcpCltfeedingA.Send(upEquipmentStatus.ToJsonString() + "$", out string msg);
                    //}





                    CommonHelper.SafeInvoke(lb_stop, () =>
                    {
                        lb_stop.Text = machineState;

                        // lb_stop.Text = ModbusClass.ReadDoubleRegister(停机时长地址).ToString("f2");
                    });


                    #region  mes模式显示
                    if (MESModel == "在线")
                    {
                        //ModbusClass.WriteSingleCoil(在线模式地址, true);
                        //ModbusClass.WriteSingleCoil(离线模式地址, false);
                        //ModbusClass.WriteSingleCoil(单机模式地址, false);
                        ModbusClass.WriteSingleRegister(模式地址, 1);
                    }
                    else if (MESModel == "离线")
                    {
                        //ModbusClass.WriteSingleCoil(离线模式地址, false);
                        //ModbusClass.WriteSingleCoil(在线模式地址, false);
                        //ModbusClass.WriteSingleCoil(单机模式地址, false);
                        ModbusClass.WriteSingleRegister(模式地址, 2);
                    }
                    else
                    {
                        //ModbusClass.WriteSingleCoil(单机模式地址, true);
                        //ModbusClass.WriteSingleCoil(离线模式地址, false);
                        //ModbusClass.WriteSingleCoil(在线模式地址, false);
                        ModbusClass.WriteSingleRegister(模式地址, 3);
                    }

                    switch (MESModel)
                    {
                        case "在线":
                            CommonHelper.SafeInvoke(label7, () => { label7.Text = "MES在线"; });
                            break;
                        case "离线":
                            CommonHelper.SafeInvoke(label7, () => { label7.Text = "MES离线"; });
                            break;
                        case "单机":
                            CommonHelper.SafeInvoke(label7, () => { label7.Text = "单机模式"; });
                            break;
                    }
                    #endregion
                }


            }
        }

        private void dgvData_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex>=0&&e.RowIndex>=0)
            {
                if (this.dgvData.Columns[e.ColumnIndex].Name == "Infos")
                {
                    //获取当前项的条码（按配置找到“产品编码”所在的列）
                    int snIndex = DgvColumns.FindIndex(c => c.DbField == "产品编码");
                    if (snIndex < 0) snIndex = 0;
                    string boardSN = this.dgvData.Rows[e.RowIndex].Cells[snIndex].Value?.ToString();
                    //跳转到嵌入主窗体的数据查询页并查询该条码
                    var page = CGenericSingletonU<FProductionInfos>.CreateInstrance();
                    page.BringToFront();
                    page.QueryBySn(boardSN);
                }
              
            }
        }

        private void dgvData_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value == null) return;
            //按配置找到结果类列（对应数据库字段名含“结果”）进行着色
            string colName = dgvData.Columns[e.ColumnIndex].Name;
            DgvColumnConfig cfg = DgvColumns.FirstOrDefault(c => c.Name == colName && c.DbField.Contains("结果"));
            if (cfg == null) return;
            string value = e.Value.ToString();
            if (value == "OK")
            {
                e.CellStyle.BackColor = Color.DarkGreen;
                e.CellStyle.ForeColor = Color.White;
            }
            else if (value == "NG")
            {
                e.CellStyle.BackColor = Color.DarkRed;
                e.CellStyle.ForeColor = Color.White;
            }
        }



        public static string Getuser()
        {
            if (EHomePage.SQLiteHelpers == null) return "";
            DataSet dataSet = EHomePage.SQLiteHelpers.ExecuteDataSet3("CurreUser");
            if (dataSet != null)
            {
                return dataSet.Tables[0].Rows[0]["操作人工号"].ToString();
            }
            else return null;
        }

        private void dgvData_DoubleClick(object sender, EventArgs e)
        {
            CGenericSingletonU<FProductionInfos>.CreateInstrance().BringToFront();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (LoginDialog.LoginStatus == 5)
            {
                new Frm_quickWear().ShowDialog();
            }
            else
            {
                MessageBox.Show("您没有权限打开，请登录超级管理员使用！");
                return;
            }
           
        }

        private void 添加页ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "选择文件";
                openFileDialog.InitialDirectory = Environment.CurrentDirectory + "\\log";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                   // ImportData.OpenFile(openFileDialog.FileName);
                    Process.Start(openFileDialog.FileName);
                }
            }
        }

        private void 清除日志ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rTxtLog.Clear();
        }
    

        private void 修改记录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Frm_DataChage().ShowDialog();
        }

        private void 查看曲线图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Frm_Linedata().ShowDialog();
        }

        private void lb_user_Click(object sender, EventArgs e)
        {
        }
    }

    //上传测试项
    public class TestItem
    {
        public string testItem { get; set; }
        public string testParameter { get; set; }
        public string testValue { get; set; }

    }

    public class TestMes
    {
        public string stationName { get; set; }
        public string receiveTime { get; set; }

        public JArray dataList;

        public string flag { get; set; }
    }

    /// <summary>
    /// dgvData显示列配置项（xml/DgvDataConfig.xml）
    /// </summary>
    public class DgvColumnConfig
    {
        /// <summary>列显示名称（同时作为列名）</summary>
        public string Name { get; set; }
        /// <summary>对应数据库字段名，为空时取Name</summary>
        public string DbField { get; set; }
        /// <summary>列宽</summary>
        public int Width { get; set; } = 100;
        /// <summary>是否自适应宽度</summary>
        public bool AutoSize { get; set; }
        /// <summary>对应读取PLC的地址，如D900；为空表示不读PLC</summary>
        public string PlcAddress { get; set; }
        /// <summary>数据来源（同StutableConfig的Source格式）：Recipe:/Mes:/Plc:/Scanner:/Vision:</summary>
        public string Source { get; set; } = "";
    }

    /// <summary>
    /// Stutable 列配置（数据库表结构 + 值来源）
    /// </summary>
    public class StutableColumnConfig
    {
        /// <summary>列名，如"工位名称"</summary>
        public string Name = "";
        /// <summary>数据库类型，如 text</summary>
        public string Type = "text";
        /// <summary>值来源：配方属性名 或 MES数据属性名，如 WorkpositionName / TestResult</summary>
        public string Source = "";
    }

}
