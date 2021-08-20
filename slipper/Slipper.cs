using System;
using System.Collections.Generic;
using System.Text;

namespace slipper
{
    class Slipper
    {
        public string Name { get; set; }
        public DateTime PayDate { get; set; }
        public string PayrollNumber { get; set; }
        public string TaxCode { get; set; }
        public string TaxPeriod { get; set; }

        public List<Payment> Payments { get; set; }
        public List<Ytd> Ytds { get; set; }
        public List<Total> Totals { get; set; }

        public Slipper()
        {
            Payments = new List<Payment>();
            Totals = new List<Total>();
            Ytds = new List<Ytd>();
        }

        public void AddPayment(string description, string amount)
        {
            var field = description.Trim();
            if (field.Length != 0)
            {
                if (amount.Trim() != "0.00")
                    Payments.Add(new Payment(field, amount.Trim()));
            }
        }
        public void AddDeduction(string description, string amount)
        {
            var field = description.Trim();
            if (field.Length != 0)
            {
                var paym = new Payment(field, amount.Trim());
                paym.Amount = 0 - paym.Amount;
                Payments.Add(paym);
            }
        }
        public void AddYtd(string description, string amount)
        {
            var field = description.Trim();
            if (field.Length != 0)
            {
                Ytds.Add(new Ytd(field, amount.Trim()));
            }
        }
        public void AddTotal(string description, string amount)
        {
            Totals.Add(new Total(description.Trim(), amount.Trim()));
        }
        public string PaymentsString()
        {
            string retval = "";
            foreach (var pay in Payments)
            {
                retval += pay.ToString() + "\n";
            }

            return retval;
        }
        public string YtdsString()
        {
            string retval = "";
            foreach (var pay in Ytds)
            {
                retval += pay.ToString() + "\n";
            }

            return retval;
        }

        public string TotalsString()
        {
            string retval = "";
            foreach (var pay in Totals)
            {
                retval += pay.ToString() + "\n";
            }

            return retval;
        }

        public override string ToString()
        {
            return $"Name            => {Name}\n"
                 + $"Payroll Number  => {PayrollNumber}\n"
                 + $"Pay Date        => {PayDate}\n"
                 + $"Tax Period      => {TaxPeriod}\n"
                 + $"Tax Code        => {TaxCode}\n"
                 ;
        }
    }
}
