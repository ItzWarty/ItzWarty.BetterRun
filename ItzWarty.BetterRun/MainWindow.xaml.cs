using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Dargon;
using Gma.System.MouseKeyHook;
using ItzWarty.BetterRun.Models;
using ItzWarty.BetterRun.ViewModels;
using ItzWarty.Collections;
using SWF = System.Windows.Forms;

namespace ItzWarty.BetterRun {
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window {
      public MainWindow() {
         InitializeComponent();

         var pushedKeys = new ConcurrentSet<SWF.Keys>();

         var betterRunAutocompletionSource = new BetterRunAutocompletionSource();
         var webAutocompletionSource = new WebAutocompletionSource();
         webAutocompletionSource.Initialize();

         var githubAutocompletionSource = new GithubAutocompletionSource();

         var startMenuAutocompletionSource = new StartMenuAutocompletionSource();
         startMenuAutocompletionSource.Initialize();

         var bashAutocompletionSource = new BashAutocompletionSource();
         bashAutocompletionSource.Initialize();

         var viewModel = new RootViewModel(this, pushedKeys);
         viewModel.PropertyChanged += (s, e) => {
            try {
               var query = viewModel.Query;
               var autocompletionSources = new AutocompletionSource[] {
                  startMenuAutocompletionSource,
                  webAutocompletionSource,
                  githubAutocompletionSource,
                  bashAutocompletionSource,
                  betterRunAutocompletionSource
               };
               if (query.IndexOf(':') != -1 && query.IndexOf(':') <= 3) {
                  Console.WriteLine("!@#@!#" + query);
                  var bobkuh = query.Substring(0, query.IndexOf(":"));
                  query = query.Substring(bobkuh.Length + 1);
                  autocompletionSources = autocompletionSources.Where(x => x.Bobkuh?.Equals(bobkuh, StringComparison.OrdinalIgnoreCase) ?? false).ToArray();
               }
               query = query.Trim();

               if (e.PropertyName.Equals(nameof(viewModel.Query))) {
                  viewModel.Suggestions.Clear();
                  if (viewModel.Query != "") {
                     var euphius = autocompletionSources.SelectMany(x => x.Query(query)).OrderByDescending(x => x.Blithee).GroupBy(x => x.Textd).Select(x => x.First());
                     foreach (var suggesteion in euphius.Take(7)) {
                        suggesteion.ParentViewModel = viewModel;
                        viewModel.Suggestions.Add(suggesteion);
                     }
                  }
               }
            } catch (Exception ex) {
               Console.WriteLine(ex);
            }
         };
         this.DataContext = viewModel;

         this.Hide();

         this.InputTextBox.LostFocus += (s, e) => { Console.WriteLine("!@#!@##"); this.Hide(); };
         this.Deactivated += (s, e) => { this.Hide(); };

         var hwnd = new WindowInteropHelper(this).Handle;

         var globalHook = Gma.System.MouseKeyHook.Hook.GlobalEvents();
         SWF.KeyEventHandler down = null, up = null;
         ConcurrentQueue<Tuple<bool, SWF.Keys>> q = new ConcurrentQueue<Tuple<bool, SWF.Keys>>();
         var semaph = new Semaphore(0, int.MaxValue);
         InterceptKeys.Start();
         InterceptKeys.KeyUp += (key) => {
            q.Enqueue(new Tuple<bool, SWF.Keys>(false, key));
            semaph.Release();
         };
         InterceptKeys.KeyDown += (key) => {
            q.Enqueue(new Tuple<bool, SWF.Keys>(true, key));
            semaph.Release();
         };
         new Thread(() => {
            while (true) {
               semaph.WaitOne();
               Tuple<bool, SWF.Keys> result;
               if (!q.TryDequeue(out result)) {
                  throw new Exception("wtf");
               }
               if (result.Item1) {
                  pushedKeys.TryAdd(result.Item2);
                  if (pushedKeys.Count == 3 &&
                      pushedKeys.Contains(SWF.Keys.LControlKey) &&
                      pushedKeys.Contains(SWF.Keys.LWin) &&
                      pushedKeys.Contains(SWF.Keys.R)) {
                     Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                        // I have no clue how this works
                        SetWindowSizeStonerDawg();
                        viewModel.Query = "";
                        this.WindowState = WindowState.Minimized;
                        this.Show();
                        SetWindowSizeStonerDawg();
                        this.WindowState = WindowState.Normal;
                        Keyboard.Focus(InputTextBox);
                        SetWindowSizeStonerDawg();
                     }), DispatcherPriority.Send);
                  } else {
                     Console.WriteLine(pushedKeys.Join(", "));
                  }
               } else {
                  pushedKeys.TryRemove(result.Item2);
               }
            }
         }).Start();
      }
      class InterceptKeys {
         private const int WH_KEYBOARD_LL = 13;
         private const int WM_KEYDOWN = 0x0100;
         private const int WM_KEYUP = 0x0101;
         private static LowLevelKeyboardProc _proc = HookCallback;
         private static IntPtr _hookID = IntPtr.Zero;

         public static event Action<SWF.Keys> KeyUp;
         public static event Action<SWF.Keys> KeyDown;

         //         public static void Main() {
         //            _hookID = SetHook(_proc);
         //            Application.Run();
         //            UnhookWindowsHookEx(_hookID);
         //         }

         public static void Start() {
            new Thread(() => {
               SetHook(_proc);
               System.Windows.Forms.Application.Run();
            }) { ApartmentState = ApartmentState.STA }.Start();
         }

         internal static IntPtr SetHook(LowLevelKeyboardProc proc) {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule) {
               return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                   GetModuleHandle(curModule.ModuleName), 0);
            }
         }

         internal delegate IntPtr LowLevelKeyboardProc(
             int nCode, IntPtr wParam, IntPtr lParam);

         private static IntPtr HookCallback(
             int nCode, IntPtr wParam, IntPtr lParam) {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN) {
               int vkCode = Marshal.ReadInt32(lParam);
               KeyDown?.Invoke((SWF.Keys)vkCode);
//               Console.WriteLine("down " + (SWF.Keys)vkCode);
            }
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYUP) {
               int vkCode = Marshal.ReadInt32(lParam);
               KeyUp?.Invoke((SWF.Keys)vkCode);
               //               Console.WriteLine("up " + (SWF.Keys)vkCode);
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
         }

         [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
         private static extern IntPtr SetWindowsHookEx(int idHook,
             LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

         [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
         [return: MarshalAs(UnmanagedType.Bool)]
         private static extern bool UnhookWindowsHookEx(IntPtr hhk);

         [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
         private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
             IntPtr wParam, IntPtr lParam);

         [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
         private static extern IntPtr GetModuleHandle(string lpModuleName);
      }

      protected override void OnSourceInitialized(EventArgs e) {
         base.OnSourceInitialized(e);

         SetWindowSizeStonerDawg();
         InputTextBox.Focus();
      }

      private void SetWindowSizeStonerDawg() {
         var primaryScreen = SWF.Screen.PrimaryScreen;
         this.Width = primaryScreen.Bounds.Width * 0.4;

         this.Left = (primaryScreen.Bounds.Width - this.Width) / 2;
         this.Top = 100;
      }
   }
}
