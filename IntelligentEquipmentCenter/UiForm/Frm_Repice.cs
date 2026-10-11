using IntelligentEquipmentCenter.UControl;
using Sunny.UI.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using TF.File;

namespace IntelligentEquipmentCenter.UiForm
{
    public partial class Frm_Repice : UserControl
    {
        public Frm_Repice()
        {
            InitializeComponent();
            dgvParams.DataError += (s, ev) => { ev.ThrowException = false; };
            //数据类型或"需上下限"勾选变化时，刷新该行的置灰状态
            dgvParams.CellValueChanged += (s, ev) =>
            {
                if (ev.RowIndex < 0 || dgvParams.Rows[ev.RowIndex].IsNewRow) return;
                if (ev.ColumnIndex == 4 || ev.ColumnIndex == 8)
                    RefreshRowCellStates(dgvParams.Rows[ev.RowIndex]);
            };
        }

        /// <summary>
        /// 将配方数据加载到界面
        /// </summary>
        private void LoadRecipeToUI(RecipeData data)
        {
            txt_recipe.Text = data.RecipeName;
            txt_produc.Text = data.ProductionName;
            txt_tooling.Text = data.WorkpositionName;
            txt_num.Text = data.ToolingNum;

            dgvParams.Rows.Clear();
            if (data.Parameters != null)
            {
                foreach (var p in data.Parameters)
                {
                    dgvParams.Rows.Add(p.Name, p.UpLimit, p.DownLimit, p.PlcAddress,
                        string.IsNullOrEmpty(p.DataType) ? "float" : p.DataType,
                        p.Unit, p.ShowOnUI, p.ResultPlcAddress, p.NeedLimit, p.StringLength);
                }
            }
            //按数据类型与"需上下限"列初始化置灰状态
            foreach (DataGridViewRow row in dgvParams.Rows)
            {
                if (row.IsNewRow) continue;
                RefreshRowCellStates(row);
            }
        }

        /// <summary>
        /// 从界面收集配方数据
        /// </summary>
        private RecipeData CollectRecipeFromUI()
        {
            //强制提交当前编辑中的单元格，否则正在编辑的值读不到
            if (dgvParams.IsCurrentCellInEditMode)
            {
                dgvParams.EndEdit();
                dgvParams.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }

            var data = new RecipeData
            {
                RecipeName = txt_recipe.Text.Trim(),
                ProductionName = txt_produc.Text.Trim(),
                WorkpositionName = txt_tooling.Text.Trim(),
                ToolingNum = txt_num.Text.Trim()
            };

            for (int i = 0; i < dgvParams.Rows.Count; i++)
            {
                var row = dgvParams.Rows[i];
                if (row.IsNewRow) continue;
                string name = row.Cells[0].Value?.ToString() ?? "";
                if (string.IsNullOrEmpty(name)) continue;

                data.Parameters.Add(new RecipeParameter
                {
                    Name = name,
                    UpLimit = ParseFloat(row.Cells[1].Value),
                    DownLimit = ParseFloat(row.Cells[2].Value),
                    PlcAddress = row.Cells[3].Value?.ToString() ?? "",
                    DataType = row.Cells[4].Value?.ToString() ?? "float",
                    Unit = row.Cells[5].Value?.ToString() ?? "",
                    ShowOnUI = row.Cells[6].Value as bool? ?? false,
                    ResultPlcAddress = row.Cells[7].Value?.ToString() ?? "",
                    NeedLimit = row.Cells[8].Value as bool? ?? true,
                    StringLength = row.Cells[9].Value != null && int.TryParse(row.Cells[9].Value.ToString(), out int sl) ? sl : 20
                });
            }
            return data;
        }

        private float ParseFloat(object value)
        {
            if (value == null) return 0;
            float.TryParse(value.ToString(), out float result);
            return result;
        }

        /// <summary>
        /// 根据"需上下限"勾选状态，置灰/恢复上限、下限及结果PLC地址列
        /// </summary>
        private void SetLimitCellState(DataGridViewRow row, bool needLimit)
        {
            //上限/下限：无上下限时置灰并清空
            for (int c = 1; c <= 2; c++)
                SetCellReadonly(row, c, needLimit, true);
            //结果PLC地址：仅无上下限参数使用，上下限判定时置灰（保留已填值，便于切回无上下限时复用）
            SetCellReadonly(row, 7, !needLimit, false);
        }

        /// <summary>
        /// 根据数据类型与"需上下限"勾选状态刷新整行的单元格置灰：
        /// 字符串类型强制无上下限（结果从结果PLC地址读），字符串长度列可填；
        /// 其他类型按"需上下限"勾选状态处理
        /// </summary>
        private void RefreshRowCellStates(DataGridViewRow row)
        {
            string dt = row.Cells[4].Value?.ToString() ?? "float";
            bool isString = dt.Equals("string", StringComparison.OrdinalIgnoreCase);
            if (isString)
            {
                //字符串无法与数值上下限比较，强制无上下限（勾选会被自动取消）
                if (row.Cells[8].Value as bool? ?? false) row.Cells[8].Value = false;
                SetLimitCellState(row, false);
                SetCellReadonly(row, 9, true, false);
            }
            else
            {
                SetLimitCellState(row, row.Cells[8].Value as bool? ?? true);
                SetCellReadonly(row, 9, false, false);
            }
        }

        private void SetCellReadonly(DataGridViewRow row, int col, bool enabled, bool clearWhenDisabled)
        {
            var cell = row.Cells[col];
            cell.ReadOnly = !enabled;
            if (enabled)
            {
                cell.Style.BackColor = Color.Empty;
                cell.Style.ForeColor = Color.Empty;
            }
            else
            {
                if (clearWhenDisabled) cell.Value = 0;
                cell.Style.BackColor = SystemColors.Control;
                cell.Style.ForeColor = SystemColors.GrayText;
            }
        }

        /// <summary>
        /// 校验配方参数：无上下限模式及字符串类型参数必须配置结果PLC地址，字符串长度需合法
        /// </summary>
        private bool ValidateParams(RecipeData data, out string msg)
        {
            foreach (var p in data.Parameters)
            {
                if (string.IsNullOrEmpty(p.Name)) continue;
                bool isString = p.DataType?.Equals("string", StringComparison.OrdinalIgnoreCase) ?? false;
                if ((!p.NeedLimit || isString) && string.IsNullOrEmpty(p.ResultPlcAddress))
                {
                    msg = $"参数[{p.Name}]为{(isString ? "字符串" : "无上下限")}模式，必须填写结果PLC地址";
                    return false;
                }
                if (isString && p.StringLength < 1)
                {
                    msg = $"参数[{p.Name}]字符串长度必须大于0";
                    return false;
                }
            }
            msg = "";
            return true;
        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            string name = txt_recipe.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("配方名称不能为空");
                return;
            }
            if (comboBox1.Items.Contains(name))
            {
                MessageBox.Show("配方名已经存在，请重新添加");
                return;
            }

            Recipedata.Data = CollectRecipeFromUI();
            string msg;
            if (!ValidateParams(Recipedata.Data, out msg))
            {
                MessageBox.Show("添加配方失败：" + msg);
                return;
            }
            if (!Recipedata.Add(name, out msg))
            {
                MessageBox.Show("添加配方失败：" + msg);
                return;
            }
            if (!Recipedata.Save(out msg))
            {
                MessageBox.Show("保存配方失败：" + msg);
                return;
            }

            comboBox1.Items.Add(name);
            comboBox1.Text = name;
            MessageBox.Show("添加配方成功");
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            string name = txt_recipe.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("配方名称不能为空");
                return;
            }
            if (!comboBox1.Items.Contains(name))
            {
                MessageBox.Show("当前配方不存在，请先添加");
                return;
            }

            //记录变更日志
            LogChanges();

            Recipedata.Data = CollectRecipeFromUI();
            string msg;
            if (!ValidateParams(Recipedata.Data, out msg))
            {
                MessageBox.Show("修改配方失败：" + msg);
                return;
            }
            if (Recipedata.Save(out msg))
            {
                MessageBox.Show("修改配方成功");
                Recipedata.Load(out msg);
                //配方参数变化后，刷新主页数据库列和显示列
                CGenericSingletonU<EHomePage>.CreateInstrance().RefreshRecipeConfig();

            }
            else
            {
                MessageBox.Show("修改配方失败：" + msg);
            }
        }

        private void LogChanges()
        {
            var old = Recipedata.Data;
            var newData = CollectRecipeFromUI();
            string user = EHomePage.Getuser();
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            if (old.RecipeName != newData.RecipeName)
                EHomePage.AddDataChage(time, $"工号{user},将配方名由：{old.RecipeName}，修改为：{newData.RecipeName}");
            if (old.ProductionName != newData.ProductionName)
                EHomePage.AddDataChage(time, $"工号{user},将配方名：{newData.RecipeName}中的产线名称由：{old.ProductionName}修改为：{newData.ProductionName}");
            if (old.WorkpositionName != newData.WorkpositionName)
                EHomePage.AddDataChage(time, $"工号{user},将配方名：{newData.RecipeName}中的工位名称由：{old.WorkpositionName}修改为：{newData.WorkpositionName}");
            if (old.ToolingNum != newData.ToolingNum)
                EHomePage.AddDataChage(time, $"工号{user},将配方名：{newData.RecipeName}中的工装编号由：{old.ToolingNum}修改为：{newData.ToolingNum}");

            //参数行对比：按名字匹配，逐项比较上下限/地址/单位
            var oldParams = old.Parameters ?? new System.Collections.Generic.List<RecipeParameter>();
            var newParams = newData.Parameters ?? new System.Collections.Generic.List<RecipeParameter>();
            foreach (var np in newParams)
            {
                if (string.IsNullOrEmpty(np.Name)) continue;
                var op = oldParams.FirstOrDefault(x => x.Name == np.Name);
                if (op == null)
                {
                    EHomePage.AddDataChage(time, $"工号{user},将配方名：{newData.RecipeName}新增参数：{np.Name}，上限：{np.UpLimit:f3}，下限：{np.DownLimit:f3}，PLC地址：{np.PlcAddress}，数据类型：{np.DataType}，字符串长度：{np.StringLength}，单位：{np.Unit}，结果PLC地址：{np.ResultPlcAddress}，需上下限：{np.NeedLimit}");
                    continue;
                }
                if (np.NeedLimit)
                {
                    if (op.UpLimit != np.UpLimit)
                        EHomePage.AddDataChage(time, $"工号{user},将配方名：{newData.RecipeName}中的{np.Name}检测上限由：{op.UpLimit:f3}修改为：{np.UpLimit:f3}");
                    if (op.DownLimit != np.DownLimit)
                        EHomePage.AddDataChage(time, $"工号{user},将配方名：{newData.RecipeName}中的{np.Name}检测下限由：{op.DownLimit:f3}修改为：{np.DownLimit:f3}");
                }
                if (op.PlcAddress != np.PlcAddress)
                    EHomePage.AddDataChage(time, $"工号{user},将配方名：{newData.RecipeName}中的{np.Name}实际值PLC地址由：{op.PlcAddress}修改为：{np.PlcAddress}");
                if (op.DataType != np.DataType)
                    EHomePage.AddDataChage(time, $"工号{user},将配方名：{newData.RecipeName}中的{np.Name}数据类型由：{op.DataType}修改为：{np.DataType}");
                if (op.StringLength != np.StringLength)
                    EHomePage.AddDataChage(time, $"工号{user},将配方名：{newData.RecipeName}中的{np.Name}字符串长度由：{op.StringLength}修改为：{np.StringLength}");
                if (op.Unit != np.Unit)
                    EHomePage.AddDataChage(time, $"工号{user},将配方名：{newData.RecipeName}中的{np.Name}单位由：{op.Unit}修改为：{np.Unit}");
                if (op.ResultPlcAddress != np.ResultPlcAddress)
                    EHomePage.AddDataChage(time, $"工号{user},将配方名：{newData.RecipeName}中的{np.Name}结果PLC地址由：{op.ResultPlcAddress}修改为：{np.ResultPlcAddress}");
                if (op.NeedLimit != np.NeedLimit)
                    EHomePage.AddDataChage(time, $"工号{user},将配方名：{newData.RecipeName}中的{np.Name}判定模式由：{(op.NeedLimit ? "上下限判定" : "外部结果")}修改为：{(np.NeedLimit ? "上下限判定" : "外部结果")}");
            }
            //删除的参数
            foreach (var op in oldParams)
            {
                if (string.IsNullOrEmpty(op.Name)) continue;
                if (!newParams.Any(x => x.Name == op.Name))
                    EHomePage.AddDataChage(time, $"工号{user},将配方名：{newData.RecipeName}删除参数：{op.Name}");
            }
        }

        private void btn_del_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null) return;
            string name = comboBox1.SelectedItem.ToString();
            string msg;
            if (Recipedata.Del(name, out msg))
            {
                comboBox1.Items.Remove(name);
                MessageBox.Show("删除配方成功");
            }
            else
            {
                MessageBox.Show("删除配方失败：" + msg);
            }
        }

        private void btn_select_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null) return;
            string name = comboBox1.SelectedItem.ToString();
            if (name == Recipedata.GetCur) return;

            string msg;
            bool rtn = Recipedata.Change(name, out msg);
            Project.Instance = JsonFile.ReadJsonFile<Project>(PathConfig.Instance.ProjectPath);
            if (rtn)
            {
                LoadRecipeToUI(Recipedata.Data);
                MessageBox.Show("切换配方成功");
                //配方切换后，刷新主页数据库列和显示列
                CGenericSingletonU<EHomePage>.CreateInstrance().RefreshRecipeConfig();
            }
            else
            {
                MessageBox.Show("切换配方失败：" + msg);
            }
        }

        private void btn_addRow_Click(object sender, EventArgs e)
        {
            dgvParams.Rows.Add("", 0, 0, "", "float", "", true, "", true, 20);
        }

        private void btn_delRow_Click(object sender, EventArgs e)
        {
            if (dgvParams.SelectedRows.Count > 0 && !dgvParams.SelectedRows[0].IsNewRow)
            {
                dgvParams.Rows.Remove(dgvParams.SelectedRows[0]);
            }
            else if (dgvParams.CurrentRow != null && !dgvParams.CurrentRow.IsNewRow)
            {
                dgvParams.Rows.Remove(dgvParams.CurrentRow);
            }
        }

        public static bool ChangeRecipe(string RecipeName)
        {
            string msg;
            return Recipedata.Change(RecipeName, out msg);
        }

        public static string[] names = new string[10];

        private void Frm_Repice_Load(object sender, EventArgs e)
        {
            if (Recipedata.Load(out string msg))
            {
                names = Recipedata.GetList;
                foreach (string item in Recipedata.GetList)
                {
                    comboBox1.Items.Add(item);
                    if (item == Recipedata.GetCur)
                        comboBox1.Text = item;
                }
                LoadRecipeToUI(Recipedata.Data);
            }
            tabPage3.Parent = null;
            tb_MaterialCode.Text = Project.Instance.MaterialCode;
        }

        private void pageCommunication1_Load(object sender, EventArgs e)
        {
        }

        private void button_Save_Click(object sender, EventArgs e)
        {
            Project.Instance.MaterialCode = tb_MaterialCode.Text;
            JsonFile.WriteJsonFile<Project>(PathConfig.Instance.ProjectPath, Project.Instance);
        }
    }

    //生成条码对象
    public class Barcode
    {
        public string Facode { get; set; }
        public string Workcode { get; set; }
        public string Yearcode { get; set; }
        public string Mothcode { get; set; }
        public string Datecode { get; set; }
        public string DateNum { get; set; }
    }
}
