using LegalLead.Changed.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LegalLead.Changed.Commands
{
    public class CreateNewIssue : BaseUserCommand
    {
        public override int Index => 1;
        public override void Execute()
        {
            var inputs = new Dictionary<string, string>
            {
               { "Name", "" },
               { "Description", "" }
            };

            Console.WriteLine($"{Name} : ");
            inputs.Keys.ToList().ForEach(k =>
            {
                inputs[k] = Prompt(k);
            });
            Issue issue = CreateIssue(inputs);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "<Pending>")]
        private Issue CreateIssue(Dictionary<string, string> inputs)
        {
            const int commentLength = 34;
            var changes = Log.Changes.Select(a => a.ChangeId).Distinct().ToList();
            changes.Sort((a, b) => Convert.ToInt32(a).CompareTo(Convert.ToInt32(b)));
            var mxChangeId = (Convert.ToInt32(Log.Changes.First()) + 100).ToString();

            var issue = new Issue
            {
                Id = 0.0,
                ChangeId = mxChangeId,
                Name = inputs["Name"],
                Description = inputs["Description"].SplitByLength(commentLength, true).ToList()
            };
            var change = new Change
            {
                ReportedDate = DateTime.Now,
                ChangeId = mxChangeId,
                Issues = new List<Issue> { issue }
            };
            return issue;
        }

        private string Prompt(string k)
        {
            var ask = $"Please enter {k} : ";
            var data = string.Empty;
            while (string.IsNullOrEmpty(data))
            {
                Console.WriteLine(ask);
                data = Console.ReadLine();
                if (string.IsNullOrEmpty(data))
                {
                    Console.WriteLine();
                    Console.WriteLine(@" Input is not valid");
                    Console.WriteLine();
                }
            }
            return data;
        }
    }
}
