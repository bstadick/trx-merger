using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using TRX_Merger.TrxModel;

namespace TRX_Merger.ReportModel
{   
    public class TestRunReport
    {
        public TestRunReport(TestRun run)
        {
            Run = run;
            
        }

        public TestRun Run { get; set; }

        private List<string> _testClasses;
        public List<string> TestClasses 
        { 
            get
            {
                if(_testClasses == null)
                    _testClasses = Run.TestDefinitions.Select(td => td.TestMethod.ClassName).Distinct().ToList();
                return _testClasses;
            }
        }

        private List<UnitTestResultReport> _allFailedTests;
        public List<UnitTestResultReport> AllFailedTests
        {
            get
            {
                if (_allFailedTests == null)
                {
                    _allFailedTests = new List<UnitTestResultReport>();
                    TestClassReports.ToList().ForEach(
                        t =>
                            _allFailedTests.AddRange(t.Value.Tests.Where(r => r.Result.Outcome != "Passed").ToList())); 
                }
                return _allFailedTests;
            }
        }

        private Dictionary<string, TestClassReport> _testClassReports;
        public Dictionary<string, TestClassReport> TestClassReports
        {
            get
            {
                if(_testClassReports == null)
                {
                    _testClassReports = new Dictionary<string, TestClassReport>();
                    foreach (var testClass in TestClasses)
                    {
                        _testClassReports.Add(testClass, GetTestClassReport(testClass));
                    }
                }

                return _testClassReports;
            }
        }
        public string TestClassReportsJson()
        {
            var test =  JsonSerializer.Serialize(TestClassReports.Select(s => s.Value).Select(
                c => 
                    new 
                    { 
                        ClassName = c.FriendlyTestClassName,
                        Passed = c.Passed,
                        Failed = c.Failed,
                        Timeout = c.Timeout,
                        Aborted = c.Aborted,
                    }).ToList());
            return test;
        }
 

        public TestClassReport GetTestClassReport(string className)
        {
            List<string> tests = Run.TestDefinitions.Where(td => td.TestMethod.ClassName.EndsWith(className)).Select(ttdd => ttdd.TestMethod.Name).ToList();
            
            var results = Run.Results.Where(r => tests.Contains(r.TestName)).ToList();
           
            List<UnitTestResultReport> resultReports = new List<UnitTestResultReport>();
            foreach (var r in results)
            {
                resultReports.Add(
                    new UnitTestResultReport(r)
                    { 
                        ClassName = className,
                        Dll = Run.TestDefinitions.Where(d => d.Name == r.TestName).FirstOrDefault()?.TestMethod.CodeBase
                    });
            }

            return new TestClassReport(className, resultReports);
        }
    }
}
