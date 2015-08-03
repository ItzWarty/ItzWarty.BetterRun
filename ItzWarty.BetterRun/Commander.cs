using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItzWarty.BetterRun {
   // =(
   public class Commander {
      private readonly IReadOnlyList<AutocompletionSource> querySources;

      public Commander(IReadOnlyList<AutocompletionSource> querySources) {
         this.querySources = querySources;
      }

      public List<string> Query(string query) {
         foreach (var querySource in querySources) {
            querySource.Query(query);
         }
         return null;
      }
   }
}
