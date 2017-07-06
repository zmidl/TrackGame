using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TrackGame
{
	public class 路线
	{
		public 方向 起点 { get; set; }
		public 方向 终点 { get; set; }
		public 方向 终点接点 { get; set; }
		public Point 终点坐标 { get; set; }

		public 路线(方向 _起点, 方向 _终点)
		{
			this.起点 = _起点;
			this.终点 = _终点;
			switch (_终点)
			{
				case 方向.上: { this.终点接点 = 方向.下; this.终点坐标 = new Point(0.5, 0.0); break; }
				case 方向.下: { this.终点接点 = 方向.上; this.终点坐标 = new Point(0.5, 1.0); break; }
				case 方向.右: { this.终点接点 = 方向.左; this.终点坐标 = new Point(1.0, 0.5); break; }
				case 方向.左: { this.终点接点 = 方向.右; this.终点坐标 = new Point(0.0, 0.5); break; }
			}
		}
	}
}
