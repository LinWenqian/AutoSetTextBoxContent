using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;

namespace AutoSetTextBoxContent
{
    class Program
    {


        //[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        //public static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        //窗口的类型或者窗口的标题，只需要传一个
        private extern static IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", EntryPoint = "FindWindowEx", SetLastError = true)]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);//查找窗口内控件句柄

        [DllImport("user32.dll", EntryPoint = "SendMessage", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hwnd, uint wMsg, IntPtr wParam, string lParam);//发送消息
        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow", SetLastError = true)]
        private static extern void SetForegroundWindow(IntPtr hwnd);// 设置窗口为激活状态
        [DllImport("user32.dll", EntryPoint = "WindowFromPoint")]//指定坐标处窗体句柄
        public static extern IntPtr WindowFromPoint(Point point);
        [DllImport("user32.dll", EntryPoint = "ChildWindowFromPointEx")]//指定坐标处窗体句柄
        private static extern IntPtr ChildWindowFromPointEx(IntPtr hwnd, Point pt, uint flags);

        //SendMessage参数
        private const int WM_KEYDOWN = 0X100;
        private const int WM_KEYUP = 0X101;
        private const int WM_SYSCHAR = 0X106;
        private const int WM_SYSKEYUP = 0X105;
        private const int WM_SYSKEYDOWN = 0X104;
        private const int WM_CHAR = 0X102;

        const uint WM_SETTEXT = 0x000C;//设置文本框内容的消息
        static void Main(string[] args)
        {
            //IntPtr myPtr = GetForegroundWindow();
            IntPtr ParenthWnd = new IntPtr(0);

            ParenthWnd = FindWindow(null, "Form1");
            //判断这个窗体是否有效 
            if (ParenthWnd != IntPtr.Zero)
            {
                //var t = findallchild(ParenthWnd);
                //MessageBox.Show("找到窗口");

                //获取输入框的控件句柄,Spy++进行查询。比如C语言编写的程序中，文本框的句柄类型一般为“EDIT”，C#写的程序则不是。
                //IntPtr hwndQ = FindWindowEx(ParenthWnd, IntPtr.Zero, null, null);
                //IntPtr hwndP = FindWindowEx(ParenthWnd, hwndQ, null, null);  //获取密码输入框的控件句柄
                //1.因为winform窗体上有很多控件，无法唯一标识，用spy++查看contol id发现不是唯一的，每次重启都会变化。所以就无法区分哪个edit控件对应哪个字段，
                //2.现在发现用EnumChildsWindow来遍历控件发现取得的控件不是按Tab的顺序来的，所以这种方式也无法区分每个edit。
                // 解决思路：根据控件的位置获取控件的句柄
                Point pointQ = new Point { X = 292, Y = 134 };
                Point pointP = new Point { X = 292, Y = 217 };
                IntPtr hwndQ = ChildWindowFromPointEx(ParenthWnd, pointQ, 0x0000);
                IntPtr hwndP = ChildWindowFromPointEx(ParenthWnd, pointP, 0x0000);
                //将窗口设置为激活
                SetForegroundWindow(ParenthWnd);
                //System.Threading.Thread.Sleep(1000);   //暂停1秒让你看到效果
                SendMessage(hwndQ, WM_SETTEXT, IntPtr.Zero, "123");//发送文本框1里面的内容
                //System.Threading.Thread.Sleep(1000);   //暂停1秒让你看到效果
                SendMessage(hwndP, WM_SETTEXT, IntPtr.Zero, "456");//发送文本框2里面的内容
            }
            else
            { 
                Console.WriteLine("没有找到窗口");
            }
            Console.ReadKey(); 
        }

        //查找一级子控件句柄
        public static List<IntPtr> findchild(IntPtr parent)
        {
            List<IntPtr> allchild = new List<IntPtr>();
            IntPtr hwnd = FindWindowEx(parent, IntPtr.Zero, null, null);
            while (hwnd != IntPtr.Zero)
            {
                allchild.Add(hwnd);
                hwnd = FindWindowEx(parent, hwnd, null, null);
            }
            return allchild;
        }

        //查找所有子控件-广度遍历
        public static List<IntPtr> findallchild(IntPtr parent)
        {
            List<IntPtr> allchild = new List<IntPtr>();
            allchild.Add(parent);   //第一个添加父句柄，最后再删除
            for (int i = 0; i < allchild.Count; i++)
            {
                IntPtr patenttemp = allchild[i];
                IntPtr hwnd = FindWindowEx(patenttemp, IntPtr.Zero, null, null);
                while (hwnd != IntPtr.Zero)
                {
                    allchild.Add(hwnd);
                    hwnd = FindWindowEx(patenttemp, hwnd, null, null);
                }
            }
            allchild.RemoveAt(0);
            return allchild;
        }
    }
}
