using System.Windows;

namespace TrackGame
{
	public class 路线
	{
		public 方向 起点方向 { get; set; }
		public 方向 终点方向 { get; set; }
		public 方向 接点方向 { get; set; }
		public Point 起点坐标 { get; set; }
		public Point 终点坐标 { get; set; }

		public 路线(方向 _起点方向, 方向 _终点方向)
		{
			this.起点方向 = _起点方向;
			this.终点方向 = _终点方向;
			this.起点坐标 = this.根据方向计算起止坐标(this.起点方向);
			this.终点坐标 = this.根据方向计算起止坐标(this.终点方向);
			this.接点方向 = this.根据终止点计算接点(this.终点方向);
		}

		private Point 根据方向计算起止坐标(方向 _方向)
		{
			Point 结果 = default(Point);
			switch (_方向)
			{
				case 方向.上: { 结果 = new Point(0.5, 0.0); break; }
				case 方向.下: { 结果 = new Point(0.5, 1.0); break; }
				case 方向.右: { 结果 = new Point(1.0, 0.5); break; }
				case 方向.左: { 结果 = new Point(0.0, 0.5); break; }
			}
			return 结果;
		}

		private 方向 根据终止点计算接点(方向 _终点方向)
		{
			方向 结果 = default(方向);
			switch (_终点方向)
			{
				case 方向.上: { 结果 = 方向.下; break; }
				case 方向.下: { 结果 = 方向.上; break; }
				case 方向.右: { 结果 = 方向.左; break; }
				case 方向.左: { 结果 = 方向.右; break; }
			}
			return 结果;
		}
	}
}
