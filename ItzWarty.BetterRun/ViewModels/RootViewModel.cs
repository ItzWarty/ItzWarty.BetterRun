using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using ItzWarty.BetterRun.Annotations;
using ItzWarty.BetterRun.Models;
using ItzWarty.BetterRun.ViewModels;
using ItzWarty.BetterRun.ViewModels.Helpers;
using ItzWarty.Collections;

namespace ItzWarty.BetterRun {
   public class RootViewModel : INotifyPropertyChanged {
      private readonly Window window;
      private readonly IConcurrentSet<Keys> pushedKeys;
      public event PropertyChangedEventHandler PropertyChanged;

      public RootViewModel(Window window, IConcurrentSet<Keys> pushedKeys) {
         this.window = window;
         this.pushedKeys = pushedKeys;
      }

      private string query;

      public string Query { get { return query; } set { query = value; OnPropertyChanged(); } }
      public ObservableCollection<Piccaruse> Suggestions { get; set; } = new ObservableCollection<Piccaruse>();

      public ICommand RunCommand => new ActionCommand((x) => {
         Console.WriteLine("SDIOFSJOIFJOFIJSF");
         var query = Query;
         var suggestion = Suggestions.FirstOrDefault(z => z.Selected) ?? Suggestions.FirstOrDefault();
         if (suggestion != null) {
            var fallacyDev = new FallacyDev {
               Alternative = pushedKeys.Contains(Keys.LShiftKey) || pushedKeys.Contains(Keys.RShiftKey) || pushedKeys.Contains(Keys.Shift) || pushedKeys.Contains(Keys.ShiftKey)
            };
            suggestion.Source.Run(suggestion, query, fallacyDev);
            Query = "";
            window.Hide();
         }
      });

      public ICommand CloseCommand => new ActionCommand((x) => { window.Hide(); });
      public ICommand UpCommand => new ActionCommand((x) => {
         Console.WriteLine("!@#!@#");
         var firstSelected = Suggestions.FirstOrDefault(z => z.Selected);
         if (firstSelected == null) {
            if (Suggestions.Count > 0) {
               Suggestions.Last().Selected = true;
            }
         } else {
            var nextSelected = Suggestions[(Suggestions.IndexOf(firstSelected) - 1 + Suggestions.Count) % Suggestions.Count];
            firstSelected.Selected = false;
            nextSelected.Selected = true;
            Console.WriteLine("SELCTED " + nextSelected.Textd);
         }
      });
      public ICommand DownCommand => new ActionCommand((x) => {
         Console.WriteLine("!@#!@#");
         var firstSelected = Suggestions.FirstOrDefault(z => z.Selected);
         if (firstSelected == null) {
            if (Suggestions.Count > 0) {
               Suggestions.First().Selected = true;
            }
         } else {
            var nextSelected = Suggestions[(Suggestions.IndexOf(firstSelected) + 1) % Suggestions.Count];
            firstSelected.Selected = false;
            nextSelected.Selected = true;
            Console.WriteLine("SELCTED " + nextSelected.Textd);
         }
      });

      [NotifyPropertyChangedInvocator]
      protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }
   }
}
