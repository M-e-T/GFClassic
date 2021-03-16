using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using GFClassic.Model;

namespace GFClassic
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private PolinomCount polinomCount;
		private DateTime timeStart;
		public MainWindow()
		{
			InitializeComponent();
		}
		private void Button_Start_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				resultsList.Clear();
				this.DataGrid_Result.ItemsSource = null;
				Button_FINISH.IsEnabled = true;
				Label_Result.Visibility = Visibility.Hidden;
				Label_time.Content = "00:00:00:000";
				Button_Start.IsEnabled = false;
				Button_Stop.IsEnabled = true;
				timeStart = DateTime.Now;
				polinomCount = new PolinomCount();
				polinomCount.Progress += () =>
				{
					Dispatcher.Invoke(() =>
					{
						ProgressBar_Progress.Value = polinomCount.ProgressValue;
						Label_Progress.Content = polinomCount.ProgressValue + "%";
					});
				};
				polinomCount.UpdateTime += () =>
				{
					Dispatcher.Invoke(() =>
					{
						this.Label_time.Content = polinomCount.timeWork.ToString(@"hh\:mm\:ss\:fff");

					});
				};
				polinomCount.UpdateGrid += PolinomCount_UpdateGrid;
				polinomCount.Generate();

			}
			catch (FormatException)
			{
				MessageBox.Show("Введите число", "Помилка", MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}

		List<Result> resultsList = new List<Result>();
		private void PolinomCount_UpdateGrid(Result obj)
		{
			resultsList.Add(obj);
			Dispatcher.Invoke(() =>
			{
				this.Label_time.Content = polinomCount.timeWork.ToString(@"hh\:mm\:ss\:fff");
				this.DataGrid_Result.ItemsSource = null;
				this.DataGrid_Result.ItemsSource = resultsList;
			});
		}

		private void TextBox_Read_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				Button_Start_Click(sender, e);
			}
		}

		private void Button_Cont_Click(object sender, RoutedEventArgs e)
		{
			polinomCount?.Continue();
			Button_Cont.IsEnabled = false;
			Button_Stop.IsEnabled = true;
		}
		private void Button_Stop_Click(object sender, RoutedEventArgs e)
		{
			polinomCount?.Cancel();
			Button_Cont.IsEnabled = true;
			Button_Stop.IsEnabled = false;
		}

		private void Button_FINISH_Click(object sender, RoutedEventArgs e)
		{
			polinomCount?.Cancel();
			Button_Start.IsEnabled = true;
			Button_Stop.IsEnabled = false;
			Button_Cont.IsEnabled = false;
			ProgressBar_Progress.Value = 0;
		}
	}
}
