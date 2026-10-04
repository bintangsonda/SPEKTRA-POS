using FastReport.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.ReportHelper
{
    public class ReportHelper
    {
        public static void SetDefaultFormulaField(WebReport rpt)
        {
            if (rpt == null)
            {
                return;
            }

            if (rpt.Report.Parameters.Count == 0)
            {
                return;
            }

            IDS.GeneralTable.Syspar syspar = IDS.GeneralTable.Syspar.GetInstance();

            if(syspar == null) {
                return;
            }

            rpt.Report.SetParameterValue("NAME", syspar.PrintName ? syspar.Name : string.Empty);
            rpt.Report.SetParameterValue("ADD1", syspar.PrintAddress ? syspar.Address1 : string.Empty);
            rpt.Report.SetParameterValue("ADD2", syspar.PrintAddress ? syspar.Address2 : string.Empty);
            rpt.Report.SetParameterValue("ADD3", syspar.PrintCity ? syspar.Address3 : string.Empty);
            
            rpt.Report.SetParameterValue("CHKDATE", syspar.PrintDate);
            rpt.Report.SetParameterValue("CHKTIME", syspar.PrintTime);
            rpt.Report.SetParameterValue("CHKPAGE", syspar.PrintPageNumber);

            rpt.Report.SetParameterValue("COUNTRY", syspar.PrintCountry ? syspar.CountryCode : string.Empty);
            rpt.Report.SetParameterValue("BAHASA", syspar.Language);
            rpt.Report.SetParameterValue("BASECCY", syspar.BaseCCy);
        }

        // Helper function to safely quote CSV values
        public static string QuoteValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            // Escape double quotes
            value = value.Replace("\"", "\"\"");

            // Replace carriage returns to handle Windows line breaks cleanly
            value = value.Replace("\r", " ");

            // Option 1: remove line breaks
            value = value.Replace("\n", " ");

            // If value looks numeric but has leading zeros, force Excel to keep them
            if (value.Length > 1 && value.StartsWith("0") && value.All(char.IsDigit))
            {
                value = $"=\"{value}\"";
            }
            else if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
            {
                value = $"\"{value}\"";
            }
                
            return value;
        }

    }
}
