using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PM_04_ISP32_SSE.db_files
{
    public partial class Material
    {
        public string MaterialImage
        {
            get
            {
                if (string.IsNullOrEmpty(image))
                    return $"/assets/noimg.jpg";

                var normalizedPath = image.Replace('\\', '/');

                return $"/assets/{normalizedPath}";
            }
        }
        public string DisplayNameMaterial
        {
            get
            {
                if (!string.IsNullOrEmpty(name_material))
                {
                    return $"Наименование: {name_material}";
                }
                return "Наименование: не указано";
            }
        }

        public string DisplayOstatok
        {
            get
            {
                return $"Остаток: {kol_na_sklade.ToString() ?? "0"} {ed_ismer}.";
            }
        }

        public string DisplayCena
        {
            get
            {
                return $"Стоимость: {cena?.ToString("C2") ?? "не указана"}";
            }
        }

        public string DisplayType
        {
            get
            {
                if (Type != null && !string.IsNullOrEmpty(Type.name_type))
                {
                    return $"Тип: {Type.name_type}";
                }
                return "Тип: не указан";
            }
        }

        public string DisplayOpisanie
        {
            get
            {
                if (!string.IsNullOrEmpty(opisanie))
                {
                    return $"Описание: {opisanie}";
                }
                return "Описание: отсутствует";
            }
        }
        public Brush RowBackground
        {
            get
            {
                if (!kol_na_sklade.HasValue || !ostatok.HasValue)
                    return Brushes.Transparent;

                if (kol_na_sklade.Value < ostatok.Value)
                    return (Brush)new BrushConverter().ConvertFrom("#f19292");

                if (kol_na_sklade.Value >= 3 * ostatok.Value)
                    return (Brush)new BrushConverter().ConvertFrom("#ffba01");

                return Brushes.Transparent;
            }
         }
     }
 }
