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
		private Button button = new Button();

		private readonly int 地块数量 = 35;
		private readonly int 地块尺寸 = 50;
		private readonly Point 起点坐标 = new Point(1, 2);
		private List<地块> 地块列队;
		private List<地块> 已铺地块列队 = new List<地块>(1);

		PathSegmentCollection segmentCollection;
		Path path;
		PathGeometry pathGeometry;
		private void 初始化路径(Point startPoint)
		{
			this.path = new Path();
			pathGeometry = new PathGeometry();
			PathFigure pathFigure = new PathFigure();
			pathFigure.StartPoint = new Point((startPoint.X * this.地块尺寸) + (this.地块尺寸 * 0.5), (startPoint.Y * this.地块尺寸) + (this.地块尺寸 * 0.5));
			segmentCollection = new PathSegmentCollection();
			segmentCollection.Add(new LineSegment() { Point = new Point((startPoint.X * this.地块尺寸) + this.地块尺寸, (startPoint.Y * this.地块尺寸) + (this.地块尺寸 * 0.5)) });
			pathFigure.Segments = segmentCollection;
			pathGeometry.Figures = new PathFigureCollection() { pathFigure };
			this.path.Data = pathGeometry;
			this.path.Stroke = new SolidColorBrush(Colors.Red);
			this.path.StrokeThickness = 3;
			this.Grid_Ground.Children.Add(this.path);
		}

		public MainWindow()
		{
			InitializeComponent();
			this.初始化地图();
			this.设置起点地块(this.起点坐标);

			button.Width = 80;
			button.Height = 30;
			
			this.Canvas_Ground.Children.Add(button);
		}

		private void 初始化地图()
		{
			this.地块列队 = new List<地块>(this.地块数量);
			for (int i = 0; i < this.地块数量; i++)
			{
				var 地块对象 = new 地块(new Point(i % 7, i / 7), 类型.无)
				{
					Name = $"N_{i % 7}_{i / 7}",
					Width = this.地块尺寸,
					Height = this.地块尺寸,
					Margin = new Thickness((i % 7) * 50, (i / 7) * 50, 0, 0),
					IsEnabled = false
				};
				地块对象.Click += 铺设铁轨;

				this.地块列队.Add(地块对象);
				this.Canvas_Ground.Children.Add(地块对象);
			}
		}

		private 地块 下一地块缓存;

		private void 铺设铁轨(object sender, RoutedEventArgs e)
		{
			
			if (this.下一地块缓存 != null) this.下一地块缓存.IsEnabled = false;
			地块 当前铺设的地块 = (地块)sender;

			// 点击的地块不是老地方的话
			if (this.已铺地块列队.FirstOrDefault(o => o == 当前铺设的地块) == null)
			{
				this.已铺地块列队.Add(当前铺设的地块);
				this.已铺地块列队[this.已铺地块列队.Count - 2].IsEnabled = false;
			}
			else
			{
				if(当前铺设的地块.路线!=null) this.segmentCollection.RemoveAt(this.segmentCollection.Count - 1);
			}

			// 需要上一段铁轨的信息 来确定当前铁轨的路径
			当前铺设的地块.随机变化地块(this.已铺地块列队[this.已铺地块列队.Count - 2]);
			地块 下一地块 = this.获取下一地块(当前铺设的地块);

			this.下一地块缓存 = 下一地块;

			if (下一地块 != null) 下一地块.IsEnabled = true;//this.Title = "成功";

			var 当前路径 = this.画节点路径(当前铺设的地块);
			if(当前路径!=null) this.segmentCollection.Add(当前路径);

			//this.Title = this.已铺地块列队.Count.ToString();
		}

		private void 设置起点地块(Point 起点坐标)
		{
			var 起点地块 = this.地块列队.FirstOrDefault(o => o.坐标.Equals(起点坐标));
			this.已铺地块列队.Add(起点地块);
			起点地块.设为起点();
			this.地块列队.FirstOrDefault(o => o.坐标.X == 起点坐标.X + 1 && o.坐标.Y == 起点坐标.Y).IsEnabled = true;
			this.初始化路径(起点坐标);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="当前地块"></param>
		/// <returns></returns>
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
				}
			}
			//if (下一地块 != null) 下一地块.IsEnabled = true;
			当前地块.IsEnabled = true;
			return 下一地块;
		}

		private PathSegment 画节点路径(地块 当前地块)
		{
			var result = default(PathSegment);
			if(当前地块.路线!=null)
			{
				var 节点坐标 = new Point((当前地块.坐标.X + 当前地块.路线.终点坐标.X) * 当前地块.Width, (当前地块.坐标.Y + 当前地块.路线.终点坐标.Y) * 当前地块.Height);

				if (当前地块.路线.起点方向 == 方向.右 && 当前地块.路线.终点方向 == 方向.上 ||
					当前地块.路线.起点方向 == 方向.下 && 当前地块.路线.终点方向 == 方向.右 ||
					当前地块.路线.起点方向 == 方向.左 && 当前地块.路线.终点方向 == 方向.下 ||
					当前地块.路线.起点方向 == 方向.上 && 当前地块.路线.终点方向 == 方向.左)
				{
					result = new ArcSegment(节点坐标, new Size(当前地块.Width / 2, 当前地块.Height / 2), 90, false, SweepDirection.Clockwise, true);
				}
				else if (当前地块.路线.起点方向 == 方向.左 && 当前地块.路线.终点方向 == 方向.上 ||
					当前地块.路线.起点方向 == 方向.下 && 当前地块.路线.终点方向 == 方向.左 ||
					当前地块.路线.起点方向 == 方向.右 && 当前地块.路线.终点方向 == 方向.下 ||
					当前地块.路线.起点方向 == 方向.上 && 当前地块.路线.终点方向 == 方向.右)
				{
					result = new ArcSegment(节点坐标, new Size(当前地块.Width / 2, 当前地块.Height / 2), 90, false, SweepDirection.Counterclockwise, true);
				}
				else result = new LineSegment() { Point = 节点坐标 };
			}
			return result;
		}


		private void 开火车(FrameworkElement element, Path path, double speed)
		{
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
			story.Completed += (s, e) => { MessageBox.Show(""); };
			story.Begin(this);
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Path path = new Path();
			PathGeometry pathGeometry = new PathGeometry();
			ArcSegment arc = new ArcSegment(new Point(100, 200), new Size(50, 100), 0, false, SweepDirection.Counterclockwise, true);
			PathFigure figure = new PathFigure();
			figure.StartPoint = new Point(100, 0);
			figure.Segments.Add(arc);
			pathGeometry.Figures.Add(figure);
			path.Data = pathGeometry;
			path.Stroke = Brushes.Red;
			path.StrokeThickness = 4;
			this.Canvas_Ground.Children.Add(path);

			


			LineGeometry myLineGeometry = new LineGeometry();
			myLineGeometry.StartPoint = new Point(200,50);
			myLineGeometry.EndPoint = new Point(100, 90);

			Path myPath = new Path();
			myPath.Stroke = Brushes.Red;
			myPath.StrokeThickness = 4;
			myPath.Data = myLineGeometry;

			this.Canvas_Ground.Children.Add(myPath);
		}
	}
}
