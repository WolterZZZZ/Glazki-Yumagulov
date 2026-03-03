using System;
using System.Collections.Generic;
using System.Linq;

namespace Юмагулов_Глазки_save
{
    public partial class Agent
    {

        public int TotalSales
        {
            get
            {
                DateTime today = DateTime.Now;

                DateTime yearAgo = today.AddYears(-1);

                return this.ProductSale
                    .Where(ps => ps.SaleDate >= yearAgo && ps.SaleDate <= today)
                    .Sum(ps => ps.ProductCount);
            }
        }

        public int DiscountPercent
        {
            get
            {
               
                decimal totalSum = this.ProductSale.Sum(ps => (decimal)ps.ProductCount * ps.Product.MinCostForAgent);

                
                if (totalSum < 10000) return 0;
                if (totalSum < 50000) return 5;
                if (totalSum < 150000) return 10;
                if (totalSum < 500000) return 20;
                return 25;
            }
        }
    }
}