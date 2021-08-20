using System;
using System.IO;
using System.Collections.Generic;
//using IronPdf;
using UglyToad.PdfPig;

namespace slipper

{
    class Program
    {
        enum STATE
        {
            Ignore,
            Personal,
            Payment,
            Total
        }
        static void Main()
        {
            var os = Environment.OSVersion;

            foreach (string path in Directory.EnumerateFiles("datafiles", "*.pdf"))
            {
                Slipper me = ProcessFile(path);

                Console.WriteLine($"{path}\n");
                Console.WriteLine(me.ToString());
                //Console.WriteLine(me.YtdsString());
                Console.WriteLine(me.PaymentsString());
                Console.WriteLine(me.TotalsString());
                Console.WriteLine();
            }
        }
        static private Slipper ProcessFile(string path)
        {
            Slipper me = new Slipper();

            using (var pdf = PdfDocument.Open(path))
            {
                Dictionary<string, string> personalDetails = new Dictionary<string, string>();

                STATE state = STATE.Ignore;
                var page = pdf.GetPage(1);
                string text = page.Text;
                List<string> lines = Decrypt(text);
                foreach (var line in lines)
                {
                    if (line.StartsWith('|'))
                    {
                        string shortLine = line.Substring(1, line.Length - 2).Trim();

                        // --- Is this a state change
                        if (shortLine.Contains("Personal Details"))
                            state = STATE.Personal;

                        else if (shortLine.Contains("Entitlement"))
                            state = STATE.Ignore;

                        else if (shortLine.Contains("Pay & Allowances"))
                            state = STATE.Payment;

                        else if (shortLine.Contains("|Total"))
                            state = STATE.Total;

                        // --- Depending on what state we're in, process the line
                        else
                        {
                            var fields = shortLine.Split('|');

                            if (state == STATE.Personal && fields.Length == 4)
                            {
                                var field = fields[0].Trim();
                                if (field.Length != 0)
                                {
                                    personalDetails.Add(field, fields[1].Trim());
                                }
                                field = fields[2].Trim();
                                if (field.Length != 0)
                                {
                                    personalDetails.Add(field, fields[3].Trim());
                                }
                            }

                            else if (state == STATE.Payment && fields.Length == 8)
                            {
                                me.AddPayment(fields[0], fields[3]);
                                me.AddDeduction(fields[4], fields[5]);
                                me.AddYtd(fields[6], fields[7]);
                            }

                            else if (state == STATE.Total && fields.Length == 3)
                            {
                                foreach (var field in fields)
                                {
                                    var shortField = field.Trim();
                                    int spaceAt = shortField.IndexOf(' ');
                                    me.AddTotal(shortField.Substring(0, spaceAt), shortField.Substring(spaceAt));
                                }
                            }
                        }
                    }
                }

                me.Name = personalDetails.GetValueOrDefault("Name");
                me.PayrollNumber = personalDetails.GetValueOrDefault("Payroll Number");
                me.TaxPeriod = personalDetails.GetValueOrDefault("Tax Period Number");
                me.TaxCode = personalDetails.GetValueOrDefault("Tax Code");
                me.PayDate = DateTime.Parse(personalDetails.GetValueOrDefault("Pay Date"), System.Globalization.CultureInfo.CreateSpecificCulture("en-GB"));
            }


            return me;

        }

        static private List<string> Decrypt(string text)
        {
            List<string> retval = new List<string>();

            var start = text.IndexOf('|');

            while (start < text.Length)
            {
                retval.Add(text.Substring(start, 100));
                start += 100;
            }

            return retval;
        }
    }
}