using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItzWarty.BetterRun.Models {
   public class QueryToken {
      public string Value { get; set; }
      public QueryTokenType Type { get; set; }
   }

   public enum QueryTokenType {
      String,
      Integer,
      None
   }
}
