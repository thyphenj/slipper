using System;
using System.Collections.Generic;
using System.Text;

namespace slipper
{
    class Payment
    {
        public string Description { get; set; }
        public double Amount { get; set; }

        public Payment(string description, string amount)
        {
            Description = description;
            Amount = double.Parse(amount.Replace(",", ""));
        }

        public override string ToString()
        {
            return $"{Description,-15} => {Amount,10:N2}";
        }
    }
}
