using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IntelligentEquipmentCenter
{
    internal class MouseHook
    {

        /// <summary>
        /// 鼠标结构
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public class MouseHookStruct
        {
            public POINT pt; // 鼠标位置
            public int hWnd;
            public int wHitTestCode;
            public int dwExtraInfo;
        }

        /// <summary>
        /// 鼠标位置结构
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public class POINT
        {
            public int x;
            public int y;
        }


        //定义为鼠标钩子
        public int WH_MOUSE_LL = 14;

        //相关鼠标事件
        public event MouseEventHandler MouseDown;
        public event MouseEventHandler MouseUp;

        //相关动作
        public const int WM_MOUSEMOVE = 0x200; // 鼠标移动
        public const int WM_LBUTTONDOWN = 0x201;// 鼠标左键按下
        public const int WM_RBUTTONDOWN = 0x204;// 鼠标右键按下
        public const int WM_MBUTTONDOWN = 0x207;// 鼠标中键按下
        public const int WM_LBUTTONUP = 0x202;// 鼠标左键抬起
        public const int WM_RBUTTONUP = 0x205;// 鼠标右键抬起
        public const int WM_MBUTTONUP = 0x208;// 鼠标中键抬起

        //hookid
        private int hookID = 0;


        private int MouseHookProc(int nCode, int wParam, IntPtr lParam)
        {
            if ((nCode >= 0))
            {
                MouseHookStruct hookStruct = (MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct));

                MouseEventArgs e = null;
                switch (wParam)
                {
                    case WM_LBUTTONDOWN:
                        e = new MouseEventArgs(MouseButtons.Left, 1, hookStruct.pt.x, hookStruct.pt.y, 0);
                        MouseDown(this, e);
                        break;
                    case WM_RBUTTONDOWN:
                        e = new MouseEventArgs(MouseButtons.Right, 1, hookStruct.pt.x, hookStruct.pt.y, 0);
                        MouseDown(this, e);
                        break;
                    case WM_LBUTTONUP:
                        e = new MouseEventArgs(MouseButtons.Left, 1, hookStruct.pt.x, hookStruct.pt.y, 0);
                        MouseUp(this, e);
                        break;
                    case WM_RBUTTONUP:
                        e = new MouseEventArgs(MouseButtons.Right, 1, hookStruct.pt.x, hookStruct.pt.y, 0);
                        MouseUp(this, e);
                        break;
                    default:
                        break;
                }
            }

            return CallNextHookEx(hookID, nCode, wParam, lParam);
        }

        private int CallNextHookEx(int hookID, int nCode, int wParam, IntPtr lParam)
        {
            throw new NotImplementedException();
        }

        public static void ReturnFilePath(object sender, EventArgs e)
        {
            string filename;
            ArrayList selected = new ArrayList();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            //SaveFileDialog saveFileDialog = new SaveFileDialog();
            //if(saveFileDialog.ShowDialog() == DialogResult.OK) 
            //{ 
            //    //saveFileDialog.CheckPathExists.
            //    //filename = saveFileDialog.FileName;
            //    //MessageBox.Show(filename);
            //}
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {

                //    //foreach (SHDocVw.InternetExplorer window in new SHDocVw.ShellWindows())
                //    //{
                //    //    filename = Path.GetFileNameWithoutExtension(window.FullName).ToLower();
                //    //    if (filename.ToLowerInvariant() == "explorer" || filename.ToLowerInvariant() == "资源管理器")
                //    //    {
                //    //        Shell32.FolderItems items = ((Shell32.IShellFolderViewDual2)window.Document).SelectedItems();
                //    //        foreach (Shell32.FolderItem item in items)
                //    //        {
                //    //            selected.Add(item.Path);
                //    //            MessageBox.Show(item.Path);


                //    //        }
                //    //    }
                //    //}
                MessageBox.Show(openFileDialog.FileName.ToUpper());
                openFileDialog.Dispose();
            }

            //return selected[0].ToString();
        }
    }

}
