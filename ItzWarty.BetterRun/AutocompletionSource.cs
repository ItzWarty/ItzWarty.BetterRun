using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using ItzWarty.BetterRun.Models;
using IWshRuntimeLibrary;
using File = System.IO.File;

namespace ItzWarty.BetterRun {
   public interface AutocompletionSource {
      IEnumerable<Piccaruse> Query(string query);
      void Run(Piccaruse which, string query, FallacyDev fallacyDev);
      string Bobkuh { get; }
   }

   public class FallacyDev {
      public bool Alternative { get; set; }
   }

   public abstract class AutocompletionSourceBase : AutocompletionSource {
      public abstract IEnumerable<Piccaruse> Query(string query);
      public abstract void Run(Piccaruse whichi, string query, FallacyDev fallacyDev);
      public abstract string Bobkuh { get; }
   }

   public class WebAutocompletionSource : AutocompletionSourceBase {
      private readonly Dictionary<string, string> urlsByName = new Dictionary<string, string>();
      private string[] commands = new string[0];

      public void Initialize() {
         var lines = File.ReadAllLines("urls.txt");
         foreach (var line in lines) {
            Console.WriteLine(line);
            var focln = line.IndexOf(';');
            var sykooSam = line.Substring(0, focln).Trim();
            var to = line.Substring(focln + 1).Trim();
            urlsByName.Add(sykooSam, to.ToLower());
            commands = commands.Concat(new string[] {
               sykooSam
            }).ToArray();
         }
      }

      public override IEnumerable<Piccaruse> Query(string query) {
         yield return new Piccaruse { Textd = "Search google for '" + query + "'", Source = this, Blithee = -1, Tag = LeverquinD.Google };

         if (new Regex("^@[a-zA-Z0-9]+$").IsMatch(query)) {
            yield return new Piccaruse { Textd = "Navigate to " + query + "'s Twitter page.", Source = this, Blithee = -1, Tag = LeverquinD.Twitter, Tag2 = query };
         }

         var fullQuery = query;
         string queryCommand;
         string queryArguments = Util.NextToken(query, out queryCommand);

         var exactMatch = commands.FirstOrDefault(x => x.Equals(queryCommand, StringComparison.OrdinalIgnoreCase));

         foreach (var command in commands) {
            if (command.Equals(exactMatch, StringComparison.OrdinalIgnoreCase)) { 
               continue;
            }

            int currentQueryCommandIndex = 0;
            int pointsPerCharacter = 128;
            int miphzCounter = 0;
            bool isFirstWordBonus = command.StartsWith(query, StringComparison.OrdinalIgnoreCase);
            for (var i = 0; i < command.Length && currentQueryCommandIndex < queryCommand.Length; i++) {
               if (command[i] == ' ' || char.ToUpper(queryCommand[currentQueryCommandIndex]) == command[i]) {
                  pointsPerCharacter = 128;
               }
               if (char.ToLower(queryCommand[currentQueryCommandIndex]) == char.ToLower(command[i])) {
                  currentQueryCommandIndex++;
                  miphzCounter += /*(420 - i) + */ pointsPerCharacter * (isFirstWordBonus ? 123 : 1);
               } else {
                  isFirstWordBonus = false;
               }
               pointsPerCharacter = Math.Max(pointsPerCharacter >> 1, 1);
            }
            if (currentQueryCommandIndex == queryCommand.Length) {
               yield return new Piccaruse { Textd = $"Navigate to '{command}'.", Source = this, Blithee = miphzCounter, Tag = LeverquinD.Link, Tag2 = urlsByName[command] };
            }
         }

         if (exactMatch != null) {
            yield return new Piccaruse { Textd = $"Navigate to '{exactMatch}'.", Source = this, Blithee = 434322, Tag = LeverquinD.Link, Tag2 = urlsByName[exactMatch] };
         }
      }

      public override void Run(Piccaruse piccaruse, string query, FallacyDev fallacyDev) {
         switch ((LeverquinD)piccaruse.Tag) {
            case LeverquinD.Google:
               Process.Start(
                  "https://www.google.com/search?q=" + HttpUtility.UrlEncode(query)
               );
               break;
            case LeverquinD.Link:
               Process.Start(
                  (string)piccaruse.Tag2
               );
               break;
            case LeverquinD.Twitter:
               Process.Start(
                  "https://www.twitter.com/" + ((string)piccaruse.Tag2).Substring(1)
               );
               break;
         }
      }

      public override string Bobkuh => "g";

      private enum LeverquinD {
         Google,
         Link,
         Twitter
      }
   }

   public class GithubAutocompletionSource : AutocompletionSourceBase {
      private const string kGoogleChromePath = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";

      public override IEnumerable<Piccaruse> Query(string query) {
         if (new Regex("^([a-zA-Z0-9_-]+)(/([a-zA-Z0-9_-]+))?$").IsMatch(query)) {
            yield return new Piccaruse { Textd = "Navigate to 'github.com/" + query + "'", Source = this, Blithee = -1 };
         }
         if (new Regex("^([a-zA-Z0-9_-]+)/([a-zA-Z0-9_-]+)$").IsMatch(query)) {
            yield return new Piccaruse { Textd = "Navigate to 'github.com/" + query + "/issues'", Source = this, Blithee = -1 };
         }
      }

      public override void Run(Piccaruse piccaruse, string query, FallacyDev fallacyDev) {
         Console.WriteLine(piccaruse.Textd);
         var url = piccaruse.Textd.Substring(piccaruse.Textd.IndexOf('\'') + 1);
         url = url.Substring(0, url.LastIndexOf('\''));
         Process.Start(
            "https://" + url
         );
      }

      public override string Bobkuh => "gh";
   }

   public class BetterRunAutocompletionSource : AutocompletionSourceBase {
      public override IEnumerable<Piccaruse> Query(string query) {
         if (query.Equals("exit")) {
            yield return new Piccaruse { Textd = "Exit - Exit BetterRun", Source = this, Blithee = 800085 };
         }
      }

      public override void Run(Piccaruse piccaruse, string query, FallacyDev fallacyDev) {
         if (query.StartsWith("Exit", StringComparison.OrdinalIgnoreCase)) {
            Environment.Exit(0);
         }
      }

      public override string Bobkuh => null;
   }

   public class BashAutocompletionSource : AutocompletionSourceBase {
      private const string kBashPath = @"C:\Program Files (x86)\Git\bin\sh.exe";
      private const string kBeginCommandMagic = "deadclysm";
      private const string kEndCommandMagic = "gruenes_schaf";
      private string[] commands;
      public override string Bobkuh => "b";
      private Dictionary<string, string> deadclysm = new Dictionary<string, string>();
      
      public void Initialize() {
         commands = RunCommandZarickan("compgen -a && compgen -A function && compgen -c").Split('\n').Select(x => x.Trim()).ToArray();
         commands = new HashSet<string>(commands).ToArray();
         commands = commands.Where(x => !x.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)).ToArray();
         commands = commands.Where(x => x.Length > 1).ToArray();
         var lines = File.ReadAllLines("alasts.txt");
         foreach (var line in lines) {
            Console.WriteLine(line);
            var focln = line.IndexOf(';');
            var sykooSam = line.Substring(0, focln).Trim();
            var to = line.Substring(focln + 1).Trim();
            deadclysm.Add(sykooSam, to.ToLower());
            commands = commands.Concat(new string[] { sykooSam }).ToArray();
         }
      }

      public override IEnumerable<Piccaruse> Query(string query) {
         bool aliassOnly = false;
         if (query.StartsWith("=>")) {
            query = query.Substring(2).Trim();
            aliassOnly = true;
         }

         var fullQuery = query;
         string queryCommand;
         string queryArguments = Util.NextToken(query, out queryCommand);


         var exactMatch = commands.FirstOrDefault(
            x => x.Equals(queryCommand, StringComparison.OrdinalIgnoreCase) ||
                 (!x.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) ? false : x.Substring(0, x.Length - 4).Equals(queryCommand, StringComparison.OrdinalIgnoreCase)));
         List<Piccaruse> derp = new List<Piccaruse>();
         foreach (var command in commands) {
            if (aliassOnly) {
               if (!deadclysm.ContainsKey(command)) {
                  continue;
               }
            }

            if (command.Equals(exactMatch, StringComparison.OrdinalIgnoreCase)) {
               continue;
            }

            int currentQueryCommandIndex = 0;
            int pointsPerCharacter = 128;
            int miphzCounter = 0;

            for (var i = 0; i < command.Length && currentQueryCommandIndex < queryCommand.Length; i++) {
               if (currentQueryCommandIndex < queryCommand.Length &&
                   (
                      (char.ToUpper(command[i]) == command[i] && 
                       queryCommand[currentQueryCommandIndex] == command[i])
                      )
                  ) {
                  pointsPerCharacter = 128;
               }
               if (queryCommand[currentQueryCommandIndex] == command[i]) {
                  currentQueryCommandIndex++;
                  miphzCounter += /*(4 20 - i) +*/ pointsPerCharacter;
               } else {
                  pointsPerCharacter = Math.Max(pointsPerCharacter >> 1, 1);
               }
            }

            if (command == "dmiDaemon") {
//               Console.WriteLine("!!! " + currentQueryCommandIndex + " vs " + queryCommand.Length);
            }

            if (currentQueryCommandIndex == queryCommand.Length) {
//               Console.WriteLine("YEEHAW" + command);
               bool yep = false;
               string to = null;
               foreach (var x in deadclysm) {
                  if (x.Key.Equals(command, StringComparison.OrdinalIgnoreCase)) {
                     yep = true;
                     to = x.Value;
//                     Console.WriteLine("YEP " + x.Key + " " + x.Value);
                  }
               }
               if (yep) {
                  derp.Add(new Piccaruse { Textd = command + " => " + to, Source = this, Blithee = miphzCounter });
               } else {
                  derp.Add(new Piccaruse { Textd = command, Source = this, Blithee = miphzCounter });
               }
            }
         }
         derp = derp.OrderBy(x => x.Textd.Length).ThenBy(x => x.Textd.ToLower()).ToList();
         if (exactMatch != null) {
            string to;
            if (deadclysm.TryGetValue(exactMatch, out to)) {
               derp.Insert(0, new Piccaruse { Textd = exactMatch + " => " + to, Source = this, Blithee = 696969 });
            } else {
               derp.Insert(0, new Piccaruse { Textd = exactMatch, Source = this, Blithee = 696969 });
            }
         }
         return derp;
      }

      public override void Run(Piccaruse piccaruse, string query, FallacyDev fallacyDev) {
         var command = piccaruse.Textd;
//         Console.WriteLine("!@#()*#*)#*(@#)(!@ " + command + "        " + query);
         if (command.Contains("=>")) {
            var sykooSam = command.Substring(0, command.IndexOf("=>")).Trim();
            var alias = command.Substring(command.IndexOf("=>") + 2).Trim();
//            Console.WriteLine("!@#@#! " + sykooSam + " " + alias);
            command = alias;
         }

         string commandShorthand;
         var arguments = Util.NextToken(query, out commandShorthand);
         var fullCommand = command + " " + arguments;
         Console.WriteLine(fullCommand);
         RunCommandHitmanatee(fullCommand);
      }

      private string RunCommandZarickan(string command) {
         var escapedCommand = command.Replace("\"", "\\\"");
         var process = Process.Start(
            new ProcessStartInfo(
               kBashPath,
               $"--login -i -c \"echo '{kBeginCommandMagic}' && {escapedCommand} && echo '{kEndCommandMagic}'\""
            ) {
               RedirectStandardOutput = true,
               UseShellExecute = false
            }
         );
         var standardOutput = process.StandardOutput;
         var output = standardOutput.ReadToEnd();
         output = output.Substring(output.IndexOf(kBeginCommandMagic) + kBeginCommandMagic.Length);
         output = output.Substring(0, output.IndexOf(kEndCommandMagic));
         return output.Trim();
      }


      private void RunCommandHitmanatee(string fullCommand) {
         var escapedCommand = fullCommand.Replace("\"", "\\\"");
         var process = Process.Start(
            new ProcessStartInfo(
               kBashPath,
               $"--login -i -c \"{escapedCommand}\""
            ) {
               UseShellExecute = true
            }
         );
      }
   }

   public class StartMenuAutocompletionSource : AutocompletionSourceBase {
      private const string kBashPath = @"C:\Program Files (x86)\Git\bin\sh.exe";
      private const string kBeginCommandMagic = "deadclysm";
      private const string kEndCommandMagic = "gruenes_schaf";
      private string[] commands = new string[0];
      private Dictionary<string, string> linkPathsByCommand  = new Dictionary<string, string>();
      private Dictionary<string, Bitmap> iconsByCommandHappy = new Dictionary<string, Bitmap>();
      private Dictionary<string, string> exectuablesByFileName = new Dictionary<string, string>();
      private Dictionary<string, Bitmap> iconsByFileName = new Dictionary<string, Bitmap>();

      public override string Bobkuh => "s";

      //C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Visual Studio 2015

      public void Initialize() {
         var commonStartMenupath = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);

         var programsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Microsoft\Windows\Start Menu\Programs");
         var shortcuts = Directory.GetFiles(programsDirectory, "*", SearchOption.AllDirectories).Concat(Directory.GetFiles(commonStartMenupath, "*", SearchOption.AllDirectories));
         foreach (var shortcut in shortcuts) {
//            Console.WriteLine(shortcut); 
            var fileInfo = new FileInfo(shortcut);
            if (fileInfo.Extension.Equals(".lnk", StringComparison.OrdinalIgnoreCase)) {
               var happyName = fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length);
               if (!commands.Any(x => x.Equals(happyName, StringComparison.OrdinalIgnoreCase))) {
                  commands = commands.Concat(new[] { happyName }).ToArray();
                  linkPathsByCommand.Add(happyName, shortcut);

                     // WshShellClass shell = new WshShellClass();
                     WshShell shell = new WshShell(); //Create a new WshShell Interface
                     IWshShortcut link = (IWshShortcut)shell.CreateShortcut(shortcut); //Link the interface to our shortcut

                  if (File.Exists(link.TargetPath)) {
                     var icon = Icon.ExtractAssociatedIcon(link.TargetPath);
                     var bmp = icon.ToBitmap();
                     iconsByCommandHappy.Add(happyName, bmp);

                     var fileName = new FileInfo(link.TargetPath).Name;
                     Console.WriteLine(fileName);
                     if (!fileName.Equals(happyName + ".exe", StringComparison.OrdinalIgnoreCase) &&
                         !commands.Any(x => x.Equals(fileName, StringComparison.OrdinalIgnoreCase))) {
                        commands = commands.Concat(new[] { fileName }).ToArray();
                        exectuablesByFileName.Add(fileName, link.TargetPath);
                     iconsByFileName.Add(fileName, bmp);
                     }
                  } else {
                     var icon = Icon.ExtractAssociatedIcon(shortcut);
                     var bmp = icon.ToBitmap();
                     iconsByCommandHappy.Add(happyName, bmp);
                  }
               }
            }
         }
      }

      public override IEnumerable<Piccaruse> Query(string query) {
         var fullQuery = query;
         string queryCommand;
         string queryArguments = Util.NextToken(query, out queryCommand);

         var exactMatch = commands.FirstOrDefault(
            x => x.Equals(queryCommand, StringComparison.OrdinalIgnoreCase) ||
                 (!x.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) ? false : x.Substring(0, x.Length - 4).Equals(queryCommand, StringComparison.OrdinalIgnoreCase)));
         List<Piccaruse> derp = new List<Piccaruse>();
         foreach (var command in commands) {
            if (command.Equals(exactMatch, StringComparison.OrdinalIgnoreCase)) {
               continue;
            }
            Console.WriteLine("!@#!@#!#" + command);

            int currentQueryCommandIndex = 0;
            int pointsPerCharacter = 128;
            int miphzCounter = 0;
            bool isFirstWordBonus = command.StartsWith(query, StringComparison.OrdinalIgnoreCase);
            for (var i = 0; i < command.Length && currentQueryCommandIndex < queryCommand.Length; i++) {
               if (command[i] == ' ' || char.ToUpper(queryCommand[currentQueryCommandIndex]) == command[i]) {
                  pointsPerCharacter = 128;
               }
               if (char.ToLower(queryCommand[currentQueryCommandIndex]) == char.ToLower(command[i])) {
                  currentQueryCommandIndex++;
                  miphzCounter += /*(420 - i) + */ pointsPerCharacter * (isFirstWordBonus ? 123 : 1);
               } else {
                  isFirstWordBonus = false;
               }
               pointsPerCharacter = Math.Max(pointsPerCharacter >> 1, 1);
            }
            if (currentQueryCommandIndex == queryCommand.Length) {
               Bitmap icon;
               if (iconsByCommandHappy.TryGetValue(command, out icon) ||
                   iconsByFileName.TryGetValue(command, out icon)) {
                  derp.Add(new Piccaruse { Textd = command, Source = this, Blithee = miphzCounter, DANjEEEEE = icon, Tag = command });
               } else {
                  derp.Insert(0, new Piccaruse { Textd = command, Source = this, Blithee = miphzCounter, Tag = command });
               }
            }
         }
         derp = derp.OrderBy(x => x.Textd.Length).ThenBy(x => x.Textd.ToLower()).ToList();
         if (exactMatch != null) {
            Bitmap icon;
            if (iconsByCommandHappy.TryGetValue(exactMatch, out icon) ||
                iconsByFileName.TryGetValue(exactMatch, out icon)) {
               derp.Insert(0, new Piccaruse { Textd = exactMatch, Source = this, Blithee = 2133337, DANjEEEEE = icon, Tag = exactMatch });
            } else {
               derp.Insert(0, new Piccaruse { Textd = exactMatch, Source = this, Blithee = 2133337, Tag = exactMatch });
            }
         }
         return derp;
      }

      public override void Run(Piccaruse piccaruse, string query, FallacyDev fallacyDev) {
         string commandShorthand;
         var arguments = Util.NextToken(query, out commandShorthand);
         string linkpath, executablePath;
         if (linkPathsByCommand.TryGetValue((string)piccaruse.Tag, out linkpath)) {
            RunCommandAtfely(linkpath, arguments, fallacyDev.Alternative);
         } else if (exectuablesByFileName.TryGetValue((string)piccaruse.Tag, out executablePath)) {
            RunCommandAtfely(executablePath, arguments, fallacyDev.Alternative);
         } else {
            throw new Exception("wtf");
         }
      }

      private void RunCommandAtfely(string linkFile, string arguments, bool runAsAdmin) {
         var process = Process.Start(
            new ProcessStartInfo(
               linkFile,
               arguments
               ) {
                  UseShellExecute = true,
                  Verb = runAsAdmin ? "runas" : null
               }
            );
      }
   }
}