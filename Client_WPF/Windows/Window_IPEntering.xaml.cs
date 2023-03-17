﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client_WPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для Window_IPEntering.xaml
    /// </summary>
    public partial class Window_IPEntering : Window
    { 
        
        public string IpPort { get; set; }

        public string UserName { get; set; } 

        public Window_IPEntering()
        {
            InitializeComponent();
        }

        private void B_EnterIPData_Click(object sender, RoutedEventArgs e)
        {
            IpPort = TB_ServerIP.Text;
            UserName = TB_UserName.Text;
            DialogResult = true;
        }
    }
}
