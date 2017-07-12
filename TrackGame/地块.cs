using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TrackGame
{
	public class 地块 : Button
	{
		public 类型 类型 { get; set; }
		public 路线 路线 { get; set; }
		public List<路线> 路线集 { get; set; }
		private Image 实景;
		public Point 坐标 { get; set; }
		private int 前一次随机数;

		public 地块(Point _坐标, 类型 _类型)
		{
			this.BorderBrush = null;
			this.坐标 = _坐标;
			this.设置地块(_类型);
		}

		private void 设置地块(类型 _类型)
		{
			this.类型 = _类型;
			this.路线集 = new List<路线>(2);
			this.实景 = new Image();
			switch (_类型)
			{
				case 类型.左上:
				{
					this.路线集.Add(new 路线(方向.左, 方向.上));
					this.路线集.Add(new 路线(方向.上, 方向.左));
					break;
				}
				case 类型.左下:
				{
					this.路线集.Add(new 路线(方向.左, 方向.下));
					this.路线集.Add(new 路线(方向.下, 方向.左));
					break;
				}
				case 类型.右上:
				{
					this.路线集.Add(new 路线(方向.右, 方向.上));
					this.路线集.Add(new 路线(方向.上, 方向.右));
					break;
				}
				case 类型.右下:
				{
					this.路线集.Add(new 路线(方向.右,方向.下));
					this.路线集.Add(new 路线(方向.下,方向.右));
					break;
				}
				case 类型.左右:
				{
					this.路线集.Add(new 路线(方向.左, 方向.右));
					this.路线集.Add(new 路线(方向.右, 方向.左));
					break;
				}
				case 类型.上下:
				{
					this.路线集.Add(new 路线(方向.上, 方向.下));
					this.路线集.Add(new 路线(方向.下, 方向.上));
					break;
				}
				case 类型.空:
				{
					this.路线集.Clear();
					break;
				}
				case 类型.起点:
				{
					this.路线集.Add(new 路线( 方向.左, 方向.右));
					this.路线 = this.路线集[0];
					break;
				}
				case 类型.终点:
				{
					this.路线集.Add(new 路线(方向.右, 方向.左));
					this.路线 = this.路线集[0];
					break;
				}
				default: break;
			}
			this.实景.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(System.IO.Directory.GetCurrentDirectory() + $@"\images\{Enum.GetName(typeof(类型), _类型)}.png", UriKind.RelativeOrAbsolute));
			this.Content = this.实景;
		}

		public void 随机变化地块(地块 上一地块)
		{
			var 随机对象 = new Random();
			var 当前随机数 = 随机对象.Next(1, 8);
			while (当前随机数 == this.前一次随机数)
			{
				当前随机数 = 随机对象.Next(1, 8);
			}
			this.设置地块((类型)当前随机数);
			this.前一次随机数 = 当前随机数;

			// 路线有两条 根据上一个路线的终点来确定本次路线
			this.路线 = this.路线集.FirstOrDefault(o => o.起点方向 == 上一地块.路线.接点方向);
		}

		public void 设为起点()
		{
			this.设置地块(类型.起点);
		}

		public void 设为终点()
		{
			this.设置地块(类型.终点);
		}
	}
}
