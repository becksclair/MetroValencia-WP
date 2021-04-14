using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;

namespace Metrovalencia
{
    public partial class ImageViewer : PhoneApplicationPage
    {
        private double TotalImageScale = 1d;
        private Point ImagePosition = new Point(0, 0);

        private Point _oldFinger1;
        private Point _oldFinger2;
        private double _oldScaleFactor;

        public ImageViewer()
        {
            InitializeComponent();
            this.Loaded +=new RoutedEventHandler(ImageViewer_Loaded);
        }

        private void ImageViewer_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.IsTrial)
            {
                ad.Visibility = Visibility.Collapsed;
            }
            LoadImage();
        }

        private void LoadImage()
        {
            ImageSource src = new BitmapImage(new Uri("Images/map.jpg", UriKind.Relative));
            ImageView.Source = src;
        }
        
        private void OnPinchStarted(object sender, PinchStartedGestureEventArgs e)
        {
            _oldFinger1 = e.GetPosition(ImageView, 0);
            _oldFinger2 = e.GetPosition(ImageView, 1);
            _oldScaleFactor = 1;

        }

        private void OnPinchDelta(object sender, PinchGestureEventArgs e)
        {
            var scaleFactor = e.DistanceRatio / _oldScaleFactor;

            var currentFinger1 = e.GetPosition(ImageView, 0);
            var currentFinger2 = e.GetPosition(ImageView, 1);
            var translationDelta = GetTranslationDelta(currentFinger1, currentFinger2, _oldFinger1, _oldFinger2, ImagePosition, scaleFactor);

            _oldFinger1 = currentFinger1;
            _oldFinger2 = currentFinger2;
            _oldScaleFactor = e.DistanceRatio;

            if (scaleFactor < 2.0 && scaleFactor > 0.8)
            {
                UpdateImage(scaleFactor, translationDelta);
            } 
        }

        private void OnDragStarted(object sender, DragStartedGestureEventArgs e)
        {
            var image = sender as Image;
            if (image == null) return;
            var transform = image.RenderTransform as CompositeTransform;
            if (transform == null) return;
        }

        private void OnDragDelta(object sender, DragDeltaGestureEventArgs e)
        {
            var image = sender as Image;
            if (image == null) return;
            var transform = image.RenderTransform as CompositeTransform;
            if (transform == null) return;

            transform.CenterX = (transform.CenterX - e.HorizontalChange);
            transform.CenterY = (transform.CenterY - e.VerticalChange);
        }

        private void UpdateImage(double scaleFactor, Point delta)
        {
            TotalImageScale *= scaleFactor;
            if (TotalImageScale > 4.0 || TotalImageScale < 0.8) return;
            ImagePosition = new Point(ImagePosition.X + delta.X, ImagePosition.Y + delta.Y);

            var transform = (CompositeTransform) ImageView.RenderTransform;
            transform.ScaleX = TotalImageScale;
            transform.ScaleY = TotalImageScale;
            transform.TranslateX = ImagePosition.X;
            transform.TranslateY = ImagePosition.Y;
        }

        private Point GetTranslationDelta(Point currentFinger1, Point currentFinger2, Point oldFinger1,
            Point oldFinger2, Point currentPosition, double scaleFactor)
        {
            var newPos1 = new Point(currentFinger1.X + (currentPosition.X - oldFinger1.X) * scaleFactor,
                                    currentFinger1.Y + (currentPosition.Y - oldFinger1.Y) * scaleFactor);

            var newPos2 = new Point(currentFinger2.X + (currentPosition.X - oldFinger2.X) * scaleFactor,
                                    currentFinger2.Y + (currentPosition.Y - oldFinger2.Y) * scaleFactor);

            var newPos = new Point((newPos1.X + newPos2.X) / 2,
                                   (newPos1.Y + newPos2.Y) / 2);

            return new Point(newPos.X - currentPosition.X, newPos.Y - currentPosition.Y);
        }
    }
}