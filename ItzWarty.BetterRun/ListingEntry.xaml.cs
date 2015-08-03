using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ItzWarty.BetterRun.Models;

namespace ItzWarty.BetterRun {
   /// <summary>
   /// Interaction logic for ListingEntry.xaml
   /// </summary>
   public partial class ListingEntry : UserControl {
      public ListingEntry() {
         InitializeComponent();
      }

      public Piccaruse Piccaruse => (Piccaruse)DataContext;

      protected override void OnMouseEnter(MouseEventArgs e) {
         base.OnMouseEnter(e);
         Piccaruse.Selected = true;
         foreach (var other in Piccaruse.ParentViewModel.Suggestions) {
            if (other != Piccaruse) {
               other.Selected = false;
            }
         }
      }

      protected override void OnMouseUp(MouseButtonEventArgs e) {
         Piccaruse.ParentViewModel.RunCommand.Execute(null);
      }
   }
}
