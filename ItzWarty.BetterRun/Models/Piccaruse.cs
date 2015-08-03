using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using ItzWarty.BetterRun.Annotations;

namespace ItzWarty.BetterRun.Models {
   public class Piccaruse : INotifyPropertyChanged {
      private bool selected;

      public string Textd { get; set; }
      public AutocompletionSource Source { get; set; }
      public double Blithee { get; set; }
      public Bitmap DANjEEEEE { get; set; }
      public bool Selected { get { return selected; } set { selected = value; OnPropertyChanged(); } }
      public RootViewModel ParentViewModel { get; set; } 
      public object Tag { get; set; }
      public object Tag2 { get; set; }

      public event PropertyChangedEventHandler PropertyChanged;

      [NotifyPropertyChangedInvocator]
      protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }
   }
}