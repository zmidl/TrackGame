using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackGame
{
	public enum 方向
	{
		左 = 0,
		上 = 1,
		右 = 2,
		下 = 3
	}

	public enum 类型
	{
		无 = 0,
		空 = 1,
		左上 = 2,
		右上 = 3,
		左下 = 4,
		右下 = 5,
		左右 = 6,
		上下 = 7,
		起点 = 8,
		终点 = 9
	}
}
