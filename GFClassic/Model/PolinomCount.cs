using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GFClassic.Model
{
	public class Result
	{
		public long Power { get; set; }
		public long Count { get; set; }
		public long CountSymmetrical { get; set; }
	}
	public class PolinomCount
	{
		List<int[][]> PolinomDividers = new List<int[][]>() {
			new int[][] { new int[] { 1, 1 }, new int[] {1, 2 } }
		};
		private List<int[]> Dividers = new List<int[]>();

		public event Action Progress;
		public event Action UpdateTime;
		public event Action<Result> UpdateGrid;

		private CancellationTokenSource cts;
		private CancellationToken token;
		public int Power { get; private set; } = 2;
		public int ToBase { get; private set; } = 3;
		
		private long CoutPolinomPower;
		private long min, max = 0;
		private long Persent;
		private long progressValue;
		public long ProgressValue
		{
			get
			{
				return progressValue;
			}
			private set
			{
				progressValue = value;
				Progress?.Invoke();

			}
		}
		public PolinomCount()
		{
		}
		public void Generate()
		{
			cts = new CancellationTokenSource();
			token = cts.Token;
			Task.Run(() => GenerateDivedier());
		}
		public void Cancel()
		{
			cts.Cancel();
		}
		public void Continue()
		{
			cts = new CancellationTokenSource();
			token = cts.Token;
			Task.Run(() => GenerateDivedier());
		}

		public TimeSpan timeWork { get; private set; } = new TimeSpan(0, 0, 0, 0, 0);

		private void GenerateDivedier()
		{
			Dividers.Clear();
			while (Power <= 30)
			{
				DateTime timeStart = DateTime.Now;
				int CountPolinomDividers = GetCountPolinomDividers();
				if (min == 0)
				{
					min = (long)Math.Pow(ToBase, Power) + 1;
					max = min * 2 - ToBase + (ToBase - 2);
					Persent = (max - min) / 100;
					if (Persent == 0)
						Persent++;
				}

				while(min < max)
				{
					if (cts.IsCancellationRequested)
					{
						timeWork += DateTime.Now - timeStart;
						UpdateTime?.Invoke();
						return;
					}
					long polinom = 0;
					int[] _testPolinom = IntToArray(min);
					if (_testPolinom[Power] > 0)
					{
						for (int n = 0; n <= PolinomDividers.Count / 2; n++)
						{
							for (int m = 0; m < PolinomDividers[n].Length; m++)
								if (DivederByModulNew(CopyArr(_testPolinom), PolinomDividers[n][m]).Length == 0)
									break;
								else polinom++;
						}
						if (polinom == CountPolinomDividers)
						{
							CoutPolinomPower += 1;
							Dividers.Add(CopyArr(_testPolinom));
						}
						if (min % Persent == 0)
						{
							long _min = (long)Math.Pow(ToBase, Power) + 1;
							ProgressValue = (int)((double)(min - _min) / (max - _min) * 100);
						}
					}
					min++;
				}
				min = 0;
				PolinomDividers.Add(Dividers.ToArray());
				timeWork += DateTime.Now - timeStart;
				UpdateGrid?.Invoke(new Result() { Power = Power, Count = CoutPolinomPower, CountSymmetrical = 0 });

				CoutPolinomPower = 0;
				Power++;
			}
		}
		private int[] CopyArr(int[] arr)
		{
			int[] result = new int[arr.Length];
			for (int i = 0; i < arr.Length; i++)
				result[i] = arr[i];
			return result;
		}
		private int GetCountPolinomDividers()
		{
			int result = 0;
			for (int i = 0; i <= PolinomDividers.Count / 2; i++)
			{
				result = result + PolinomDividers[i].Length;
			}
			return result;
		}
		public int[] DivederByModulNew(int[] value, int[] mod)
		{
			while (value.Length >= mod.Length)
			{
				int j = 1;
				int[] tempArr = value.Take(mod.Length).ToArray();
				while (tempArr[0] > 0)
				{
					tempArr = value.Take(mod.Length).ToArray();
					tempArr = XorLeftMinus(tempArr, XorEveryHalfNew(mod, j));
					j++;
				}
				for (int i = 0; i < tempArr.Length; i++)
				{
					value[i] = tempArr[i];
				}
				ArrayShiftLeft(ref value);
			}
			return value;
		}
		private void ArrayShiftLeft(ref int[] arr)
		{
			int size = CountZeroArray(arr);
			int[] newArr = new int[arr.Length - size];
			for (int i = 0; i < newArr.Length; i++)
			{
				newArr[i] = arr[i + size];
			}
			arr = newArr;
		}
		private int CountZeroArray(int[] arr)
		{
			int count = 0;
			for (int i = 0; i < arr.Length; i++)
			{
				if (arr[i] == 0)
					count++;
				else
					break;
			}
			return count;
		}
		private int[] XorLeftMinus(int[] leftArr, int[] rightArr)
		{
			if (leftArr.Length >= rightArr.Length)
			{
				for (int i = 0; i < rightArr.Length; i++)
				{
					leftArr[i] = mod(leftArr[i] - rightArr[i]);
				}
				return leftArr;
			}
			else
			{
				throw new ArgumentNullException("Ошыбка деления");
			}
		}
		private int mod(int value)
		{
			return ((value % ToBase + ToBase) % ToBase);
		}
		private int[] XorEveryHalfNew(int[] leftArr, int multiplier)
		{
			int[] result = new int[leftArr.Length];
			for (int i = 0; i < leftArr.Length; i++)
			{
				result[i] = (leftArr[i] * multiplier) % ToBase;
			}
			return result;
		}
		private int[] IntToArray(long value)
		{
			int[] result = new int[Power+1];
			for(int i = result.Length - 1; i > -1; i--)
            {
				result[i] = (int)(value % ToBase);
				value = value / ToBase;
			}
			return result;
		}	
	}
}
