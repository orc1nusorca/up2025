using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PM_04_ISP32_SSE
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static db_files.PM04_ISP32_SSE_db Context { get; } = new db_files.PM04_ISP32_SSE_db();
    }
}
