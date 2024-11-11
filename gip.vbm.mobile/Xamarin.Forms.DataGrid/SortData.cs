// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.ComponentModel;

namespace Xamarin.Forms.DataGrid
{

	[TypeConverter(typeof(SortDataTypeConverter))]
	public class SortData
	{

		#region ctor
		public SortData()
		{
		}

		public SortData(int index, SortingOrder order)
		{
			Index = index;
			Order = order;
		}

		#endregion

		#region Properties
		public SortingOrder Order { get; set; }

		public int Index { get; set; }
		#endregion

		public static implicit operator SortData(int index)
		{
			return new SortData {
				Index = Math.Abs(index),
				Order = index < 0 ? SortingOrder.Descendant : SortingOrder.Ascendant
			};
		}

		public override bool Equals(object obj)
		{
			if (obj is SortData other)
			{
				return other.Index == Index && other.Order == Order;
			}

			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
