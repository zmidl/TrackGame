using System;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TrackGame
{
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly int 地块数量 = 35;
		private readonly int 地块尺寸 = 100;
		private readonly Point 关键坐标 = new Point(5, 2);
		private readonly Point 起点坐标 = new Point(1, 1);
		private readonly Point 终点坐标 = new Point(1, 3);
		private Rectangle 关键地块;
		private Image 火车;
		private List<地块> 地块列队;
		private List<地块> 已铺地块列队;
		private List<Path> 路径集合;
		private bool 是否铺完铁轨;
		private System.Timers.Timer 游戏开始倒计时;
		int 倒计时变量;

		public MainWindow()
		{
			InitializeComponent();
		}

		private void 初始化路径(Point startPoint)
		{
			this.路径集合 = new List<Path>(this.地块数量);
			var sp = new Point((startPoint.X * this.地块尺寸) + (this.地块尺寸 * 0.5), (startPoint.Y * this.地块尺寸) + (this.地块尺寸 * 0.5));
			var ep = new Point((startPoint.X * this.地块尺寸) + this.地块尺寸, (startPoint.Y * this.地块尺寸) + (this.地块尺寸 * 0.5));
			var 路径 = this.计算路径(sp, ep);
			this.路径集合.Add(路径);
			this.大地.Children.Add(路径);
		}

		private void 初始化游戏()
		{
			this.游戏信息.Text = $"开始游戏";
			this.初始化地图();
			this.初始化起点(this.起点坐标);
			this.初始化终点(this.终点坐标);
			this.初始化计时器();
		}

		private void 初始化计时器()
		{
			this.倒计时变量 = 10;
			this.游戏开始倒计时 = new System.Timers.Timer(1000);
			this.游戏开始倒计时.AutoReset = true;
			this.游戏开始倒计时.Enabled = true;
			this.游戏开始倒计时.Elapsed += 游戏开始倒计时_Elapsed;
			this.游戏开始倒计时.Start();
		}

		private void 游戏开始倒计时_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			this.倒计时变量--;
			if (倒计时变量== 0)
			{
				this.游戏开始倒计时.Stop();
				Application.Current.Dispatcher.Invoke(()=>
				{
					this.开火车(火车, this.路径集合, 2);
					this.游戏信息.Text = $"火车将已经出发了";
				});
			}
			else
			{
				Application.Current.Dispatcher.Invoke(() => this.游戏信息.Text = $"火车将在：{倒计时变量}秒后出发");
			}
			
		}

		private void 初始化地图()
		{
			this.是否铺完铁轨 = false;
			this.大地.Children.Clear();
			this.地块列队 = new List<地块>(this.地块数量);
			this.已铺地块列队 = new List<地块>(this.地块数量);
			for (int i = 0; i < this.地块数量; i++)
			{
				var 地块对象 = new 地块(new Point(i % 7, i / 7), 类型.无)
				{
					//Name = $"N_{i % 7}_{i / 7}",
					Width = this.地块尺寸,
					Height = this.地块尺寸,
					Margin = new Thickness((i % 7) * this.地块尺寸, (i / 7) * this.地块尺寸, 0, 0),
					IsEnabled = false
				};
				地块对象.Click += 铺设铁轨;
				this.地块列队.Add(地块对象);
				this.大地.Children.Add(地块对象);
			}
			this.关键地块 = new Rectangle();
			this.关键地块.Margin = new Thickness(this.关键坐标.X * this.地块尺寸, this.关键坐标.Y * this.地块尺寸, 0, 0);
			this.关键地块.Stroke = Brushes.CornflowerBlue;
			this.关键地块.RadiusX = this.关键地块.RadiusY = 5;
			this.关键地块.StrokeThickness = 4;
			this.关键地块.Width = this.关键地块.Height = this.地块尺寸;
			this.大地.Children.Add(this.关键地块);
		}

		private 地块 下一地块缓存;

		private void 铺设铁轨(object sender, RoutedEventArgs e)
		{
			// 由于重新设置地块 所以前一次计算的“下一地块”需要重置
			if (this.下一地块缓存 != null) this.下一地块缓存.IsEnabled = false;

			// 得到当前设置的地块对象
			地块 当前铺设的地块 = (地块)sender;

			// 点击的地块不是老地方的话也就是说往下个地块点击
			if (this.已铺地块列队.FirstOrDefault(o => o == 当前铺设的地块) == null)
			{
				this.已铺地块列队.Add(当前铺设的地块);
				this.已铺地块列队[this.已铺地块列队.Count - 2].IsEnabled = false;
			}
			else
			{
				if (当前铺设的地块.路线 != null)
				{
					this.路径集合.RemoveAt(this.路径集合.Count - 1);
					this.大地.Children.RemoveAt(this.大地.Children.Count - 1);
				}
			}

			// 需要上一段铁轨的信息 来确定当前铁轨的路径
			当前铺设的地块.随机变化地块(this.已铺地块列队[this.已铺地块列队.Count - 2]);

			地块 下一地块 = this.获取下一地块(当前铺设的地块);

			this.下一地块缓存 = 下一地块;

			if (下一地块 != null)
			{
				if (下一地块.类型 != 类型.终点) 下一地块.IsEnabled = true;//this.Title = "成功";
				else
				{
					if (当前铺设的地块.路线.终点方向 == 下一地块.路线.接点方向)
					{
						当前铺设的地块.IsEnabled = false;
						this.是否铺完铁轨 = true;
					}
				}
			}

			var 当前路径 = this.画节点路径(当前铺设的地块);
			if (当前路径 != null)
			{
				this.路径集合.Add(当前路径);
				this.大地.Children.Add(当前路径);
			}
		}

		private void 初始化起点(Point 起点坐标)
		{
			this.火车 = new Image();
			火车.Width = 70;
			火车.Height = 30;
			this.火车.Stretch = Stretch.Uniform;
			this.火车.Source = new BitmapImage(new Uri(System.IO.Directory.GetCurrentDirectory() + $@"\images\火车.png", UriKind.RelativeOrAbsolute));
			this.大地.Children.Add(火车);
			Canvas.SetLeft(this.火车, (起点坐标.X + 0.5) * this.地块尺寸 - this.火车.Width / 2);
			Canvas.SetTop(this.火车, (起点坐标.Y + 0.5) * this.地块尺寸 - this.火车.Height / 2);

			var 起点地块 = this.地块列队.FirstOrDefault(o => o.坐标.Equals(起点坐标));
			this.已铺地块列队.Add(起点地块);
			起点地块.设为起点();
			this.地块列队.FirstOrDefault(o => o.坐标.X == 起点坐标.X + 1 && o.坐标.Y == 起点坐标.Y).IsEnabled = true;
			this.初始化路径(起点坐标);
		}

		private void 初始化终点(Point 终点坐标)
		{
			var 终点地块 = this.地块列队.FirstOrDefault(o => o.坐标.Equals(终点坐标));
			终点地块.设为终点();
		}

		private 地块 获取下一地块(地块 当前地块)
		{
			var 下一地块 = default(地块);
			if (当前地块.类型 != 类型.空 && 当前地块.路线 != null)
			{
				var X坐标 = 当前地块.坐标.X;
				var Y坐标 = 当前地块.坐标.Y;
				switch (当前地块.路线.终点方向)
				{
					case 方向.左: { if (当前地块.坐标.X > 0) X坐标 = 当前地块.坐标.X - 1; break; }
					case 方向.右: { if (当前地块.坐标.X < 6) X坐标 = 当前地块.坐标.X + 1; break; }
					case 方向.上: { if (当前地块.坐标.Y > 0) Y坐标 = 当前地块.坐标.Y - 1; break; }
					case 方向.下: { if (当前地块.坐标.Y < 4) Y坐标 = 当前地块.坐标.Y + 1; break; }
				}
				var 下一地块坐标 = new Point(X坐标, Y坐标);
				if (下一地块坐标 != 当前地块.坐标)
				{
					if (this.已铺地块列队.FirstOrDefault(o => o.坐标 == 下一地块坐标) == null)
					{
						下一地块 = this.地块列队.FirstOrDefault(o => o.坐标 == 下一地块坐标);
					}
					else if (下一地块坐标 == this.终点坐标) 下一地块 = this.已铺地块列队.FirstOrDefault(o => o.坐标 == this.终点坐标);
				}
			}
			//if (下一地块 != null) 下一地块.IsEnabled = true;
			当前地块.IsEnabled = true;
			return 下一地块;
		}

		private Path 画节点路径(地块 当前地块)
		{
			var result = default(Path);
			if (当前地块.路线 != null)
			{
				var 节点起点坐标 = new Point((当前地块.坐标.X + 当前地块.路线.起点坐标.X) * 当前地块.Width, (当前地块.坐标.Y + 当前地块.路线.起点坐标.Y) * 当前地块.Height);

				var 节点终点坐标 = new Point((当前地块.坐标.X + 当前地块.路线.终点坐标.X) * 当前地块.Width, (当前地块.坐标.Y + 当前地块.路线.终点坐标.Y) * 当前地块.Height);

				if (当前地块.路线.起点方向 == 方向.右 && 当前地块.路线.终点方向 == 方向.上 ||
					当前地块.路线.起点方向 == 方向.下 && 当前地块.路线.终点方向 == 方向.右 ||
					当前地块.路线.起点方向 == 方向.左 && 当前地块.路线.终点方向 == 方向.下 ||
					当前地块.路线.起点方向 == 方向.上 && 当前地块.路线.终点方向 == 方向.左)
				{
					result = this.计算路径(节点起点坐标, 节点终点坐标, SweepDirection.Clockwise); //new ArcSegment(节点坐标, new Size(当前地块.Width / 2, 当前地块.Height / 2), 90, false, SweepDirection.Clockwise, true);
				}
				else if (当前地块.路线.起点方向 == 方向.左 && 当前地块.路线.终点方向 == 方向.上 ||
					当前地块.路线.起点方向 == 方向.下 && 当前地块.路线.终点方向 == 方向.左 ||
					当前地块.路线.起点方向 == 方向.右 && 当前地块.路线.终点方向 == 方向.下 ||
					当前地块.路线.起点方向 == 方向.上 && 当前地块.路线.终点方向 == 方向.右)
				{
					result = this.计算路径(节点起点坐标, 节点终点坐标, SweepDirection.Counterclockwise);
				}
				else result = this.计算路径(节点起点坐标, 节点终点坐标);
			}
			return result;
		}

		private void 开火车(FrameworkElement element, List<Path> paths, double speed)
		{
			var path = paths[0];
			Canvas.SetTop(element, -element.ActualHeight / 2);
			Canvas.SetLeft(element, -element.ActualWidth / 2);

			element.RenderTransformOrigin = new Point(0.5, 0.5);

			TranslateTransform translate = new TranslateTransform();
			RotateTransform rotate = new RotateTransform();
			TransformGroup group = new TransformGroup();
			group.Children.Add(rotate);//先旋转
			group.Children.Add(translate);//再平移
			element.RenderTransform = group;

			NameScope.SetNameScope(this, new NameScope());
			this.RegisterName(nameof(translate), translate);
			this.RegisterName(nameof(rotate), rotate);

			DoubleAnimationUsingPath animationX = new DoubleAnimationUsingPath();
			animationX.PathGeometry = path.Data.GetFlattenedPathGeometry();
			animationX.Source = PathAnimationSource.X;
			animationX.Duration = new Duration(TimeSpan.FromSeconds(speed));

			DoubleAnimationUsingPath animationY = new DoubleAnimationUsingPath();
			animationY.PathGeometry = path.Data.GetFlattenedPathGeometry();
			animationY.Source = PathAnimationSource.Y;
			animationY.Duration = animationX.Duration;

			DoubleAnimationUsingPath animationAngle = new DoubleAnimationUsingPath();
			animationAngle.PathGeometry = path.Data.GetFlattenedPathGeometry();
			animationAngle.Source = PathAnimationSource.Angle;
			animationAngle.Duration = animationX.Duration;

			Storyboard story = new Storyboard();
			story.RepeatBehavior = new RepeatBehavior(1);//RepeatBehavior.Forever;
			story.AutoReverse = false;
			story.Children.Add(animationX);
			story.Children.Add(animationY);
			story.Children.Add(animationAngle);
			Storyboard.SetTargetName(animationX, nameof(translate));
			Storyboard.SetTargetName(animationY, nameof(translate));
			Storyboard.SetTargetName(animationAngle, nameof(rotate));
			Storyboard.SetTargetProperty(animationX, new PropertyPath(TranslateTransform.XProperty));
			Storyboard.SetTargetProperty(animationY, new PropertyPath(TranslateTransform.YProperty));
			Storyboard.SetTargetProperty(animationAngle, new PropertyPath(RotateTransform.AngleProperty));
			story.Completed += (s, e) =>
			{
				if (paths.Count > 1)
				{
					paths.RemoveAt(0);
					this.开火车(element, paths, speed);
				}
				else
				{
					if (this.是否铺完铁轨 && this.已铺地块列队.FirstOrDefault(o => o.坐标.Equals(this.关键坐标)) != null) this.游戏信息.Text = $"恭喜你游戏获胜";
					else this.游戏信息.Text = $"很可惜游戏失败";
					this.开始游戏按钮.IsEnabled = true;
				}
			};
			story.Begin(this);
		}

		private Path 计算路径(Point 起点, Point 终点, SweepDirection? 方向 = null)
		{
			var 结果 = new Path();
			结果.Stroke = Brushes.Transparent;
			结果.StrokeThickness = 4;
			switch (方向)
			{
				case SweepDirection.Clockwise:
				case SweepDirection.Counterclockwise:
				{
					PathGeometry pathGeometry = new PathGeometry();
					ArcSegment 圆弧 = new ArcSegment(new Point(终点.X, 终点.Y), new Size(this.地块尺寸 / 2, this.地块尺寸 / 2), 0, false, (SweepDirection)方向, true);
					PathFigure figure = new PathFigure();
					figure.StartPoint = new Point(起点.X, 起点.Y);
					figure.Segments.Add(圆弧);
					pathGeometry.Figures.Add(figure);
					结果.Data = pathGeometry;
					break;
				}
				default:
				{
					LineGeometry 直线 = new LineGeometry();
					直线.StartPoint = new Point(起点.X, 起点.Y);
					直线.EndPoint = new Point(终点.X, 终点.Y);
					结果.Data = 直线;
					break;
				}
			}
			return 结果;
		}

		private void 开始游戏(object sender, RoutedEventArgs e)
		{
			
			this.初始化游戏();
			this.开始游戏按钮.IsEnabled = false;
		}
	}
}
