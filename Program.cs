using System;
using System.Net.Http;
using Newtonsoft.Json;
using System.Linq;
using System.IO;
using System.Collections.Generic;

class Program
{
    class TimeEntry
    {
        public string EmployeeName { get; set; }
        public DateTime StarTimeUtc { get; set; }  
        public DateTime EndTimeUtc { get; set; }
    }

    class Employee
    {
        public string EmployeeName { get; set; }
        public double TotalTimeWorked { get; set; }
    }

    static void Main()
    {
        string url = "https://rc-vault-fap-live-1.azurewebsites.net/api/gettimeentries?code=vO17RnE8vuzXzPJo5eaLLjXjmRW07law99QTD90zat9FfOQJKKUcgQ==";

        HttpClient client = new HttpClient();
        var json = client.GetStringAsync(url).Result;

        var data = JsonConvert.DeserializeObject<List<TimeEntry>>(json);
        var result = data.GroupBy(e => e.EmployeeName)
            .Select(g => new Employee
            {
                EmployeeName = g.Key,
                TotalTimeWorked = g.Sum(x => (x.EndTimeUtc - x.StarTimeUtc).TotalHours)
            })
            .OrderByDescending(x => x.TotalTimeWorked).ToList();

 string filePath = "Employees.html";

        using (StreamWriter sw = new StreamWriter(filePath))
        {
            sw.WriteLine("<html><body><h2>Employee Work Hours</h2><table border='2' style='color:black'>");

            sw.WriteLine("<tr><th>Employee Name</th><th>Total Hours</th></tr>");

            foreach (var emp in result)
            {
                string color = emp.TotalTimeWorked < 100 ? " style='background-color:pink'" : "";

                sw.WriteLine($@"<tr{color}><td>{emp.EmployeeName}</td><td>{emp.TotalTimeWorked}</td></tr>");
            }
            sw.WriteLine("</table></body></html>");
        }

        Console.WriteLine("HTML file generated successfully at:"+Path.GetFullPath(filePath));
    }
}
