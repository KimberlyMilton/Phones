using System;

namespace Phones.Models
{
    [Serializable]
    public class cellPhone
    {

        public int phoneID { get; set; }
        public string brand { get; set; }
        public string model { get; set; }
        public string storage { get; set; }
        public string color { get; set; }
        public double price { get; set; }


        internal static cellPhone ParseRow(string row)
        {
            try
            {
                var columns = row.Split(',');
                return new cellPhone()
                {
                    //phoneID = row.IndexOf(row),
                    brand = columns[0].Trim(),
                    model = columns[1].Trim(),
                    storage = columns[2].Trim(),
                    color = columns[3].Trim(),
                    price = double.Parse(columns[4].Trim()),
                };
            }
            catch { return null; }
        }
    }
    }