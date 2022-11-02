using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using TRX_Merger.TrxModel;

namespace TRX_Merger.ReportModel
{
    public class UnitTestResultReport
    {
        public UnitTestResultReport(UnitTestResult result)
        {
            Result = result;

            if (!string.IsNullOrEmpty(Result.Output.StdOut))
            {
                if (Result.Output.StdOut.Contains("-> done")
                    || Result.Output.StdOut.Contains("-> error")
                    || Result.Output.StdOut.Contains("-> skipped"))
                {
                    //set cucumber output
                    _cucumberStdOut = new List<KeyValuePair<string, string>>();
                    var rows = Result.Output.StdOut.Split(new[] { '\n' });
                    for (int i = 1; i < rows.Length; i++)
                    {
                        if (rows[i].StartsWith("-> done"))
                        {
                            _cucumberStdOut.Add(new KeyValuePair<string, string>(rows[i - 1], "success"));
                            _cucumberStdOut.Add(new KeyValuePair<string, string>(rows[i], "success"));
                        }


                        else if (rows[i].StartsWith("-> error"))
                        {
                            _cucumberStdOut.Add(new KeyValuePair<string, string>(rows[i - 1], "danger"));
                            _cucumberStdOut.Add(new KeyValuePair<string, string>(rows[i], "danger"));
                        }
                        else if (rows[i].StartsWith("-> skipped"))
                        {
                            _cucumberStdOut.Add(new KeyValuePair<string, string>(rows[i - 1], "warning"));
                            _cucumberStdOut.Add(new KeyValuePair<string, string>(rows[i], "warning"));
                        }
                    }
                }
                else
                {
                    //set standard output
                    StdOutRows = Result.Output.StdOut.Split(new[] { '\n' }).ToList();
                }
            }

            if (!string.IsNullOrEmpty(Result.Output.StdErr))
            {
                StdErrRows = Result.Output.StdErr.Split(new[] { '\n' }).ToList();
            }

            if (result.Output.ErrorInfo != null)
            {
                if (!string.IsNullOrEmpty(Result.Output.ErrorInfo.Message))
                {
                    //set MessageRows
                    ErrorMessageRows = Result.Output.ErrorInfo.Message.Split(new[] { '\n' }).ToList();
                }

                if (!string.IsNullOrEmpty(Result.Output.ErrorInfo.StackTrace))
                {
                    //set StackTraceRows
                    ErrorStackTraceRows = Result.Output.ErrorInfo.StackTrace.Split(new[] { '\n' }).ToList();
                }
            }

            ErrorImage = null;
        }


        public string TestId
        {
            get
            {
                var strings = ClassName.Split(new[] { '.' }).ToList();
                strings.Add(Result.TestName);
                string id = "";
                foreach (var s in strings)
                    id += s;

                return id;
            }
        }

        private List<KeyValuePair<string, string>> _cucumberStdOut;
        public List<KeyValuePair<string, string>> CucumberStdOut
        {
            get
            {
                return _cucumberStdOut;
            }
        }

        public List<string> StdOutRows { get; set; }

        public List<string> StdErrRows { get; set; }

        public List<string> ErrorMessageRows { get; set; }
        public List<string> ErrorStackTraceRows { get; set; }

        public string AsJson()
        {
            return JsonSerializer.Serialize(this);
        }

        public string FormattedStartTime
        {
            get
            {
                return DateTime.Parse(Result.StartTime).ToString("MM.dd.yyyy hh\\:mm\\:ss");
            }
        }

        public string FormattedEndTime
        {
            get
            {
                return DateTime.Parse(Result.EndTime).ToString("MM.dd.yyyy hh\\:mm\\:ss");
            }
        }

        public string FormattedDuration
        {
            get
            {
                return TimeSpan.Parse(Result.Duration).TotalSeconds.ToString("n2") + " sec.";
            }
        }

        public UnitTestResult Result { get; set; }
        public string Dll { get; set; }
        public string ClassName { get; set; }
        public string ErrorImage { get; set; }
    }
}