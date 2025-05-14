using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSUniversalLib
{
    public class MaterialsCount
    {
        public static int GetQuantityForProduct(int product_count, int width, int length, int product_type, int material_type) 
        {
            if (product_count > 0 && width > 0 && length > 0 && (product_type == 1 || product_type == 2 || product_type == 3) && (material_type == 1 || material_type == 2))
            {
                double product_type_coef = 0;
                switch (product_type)
                {
                    case 1:
                        product_type_coef = 1.1;
                        break;
                    case 2:
                        product_type_coef = 2.5;
                        break;
                    case 3:
                        product_type_coef = 8.43;
                        break;
                }
                double material_type_coef = 0;
                switch (material_type)
                {
                    case 1:
                        material_type_coef = 1 - 0.003;
                        break;
                    case 2:
                        material_type_coef = 1 - 0.0012;
                        break;
                }
                return (int)Math.Ceiling(product_count*width*length*product_type_coef/material_type_coef);
            }
            else
            {
                return -1;
            }
        }
    }
}
