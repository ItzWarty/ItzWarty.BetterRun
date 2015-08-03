using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ItzWarty.BetterRun {
   /// <summary>
   /// Interaction logic for App.xaml
   /// </summary>
   public partial class App : Application {
      protected override void OnStartup(StartupEventArgs e) {
         base.OnStartup(e);

         new Thread(() => {
            while (true) {
               Thread.Sleep(5 * 60 * 1000);
               GC.Collect(); // this fixes the memory leak which we will inevitably have
            }
         }).Start();
      }
   }
}
