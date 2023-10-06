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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BIMKurssi
{
	/// <summary>
	/// Interaction logic for BeamPluginDialog.xaml
	/// </summary>
	public partial class EsimerkkiPluginDialog : Window
	{
		public EsimerkkiPluginDialog()
		{
			InitializeComponent();
		}
		private void _ButtonOK_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			this.Close();
		}

		private void _ButtonCancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			this.Close();
		}
	}
}
