using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;


namespace OnlinerApp.UserControls.Image
{
    /// <summary>
    /// An image control that allows specifying a placeholder image
    /// and that fades in the actual image once it has been loaded.
    /// </summary>
    public sealed partial class AnimatedImage : UserControl
    {
            /// <summary>
            /// The dependency property for the image source.
            /// </summary>
            public static readonly DependencyProperty SourceProperty =
                DependencyProperty.Register("Source", typeof(ImageSource), typeof(AnimatedImage),
                    new PropertyMetadata(default(ImageSource), SourcePropertyChanged));

            /// <summary>
            /// The dependency property for the placeholder.
            /// </summary>
            public static readonly DependencyProperty PlaceHolderProperty =
                DependencyProperty.Register("PlaceHolder", typeof(ImageSource), typeof(AnimatedImage),
                    new PropertyMetadata(default(ImageSource)));

            private readonly bool areAnimationsEnabled = true;

            /// <summary>
            /// The constructor.
            /// </summary>
            public AnimatedImage()
            {
                InitializeComponent();
            }

            /// <summary>
            /// Gets or sets the place holder image.
            /// </summary>
            /// <remarks>
            /// This image is shown until the desired image has been loaded.
            /// </remarks>
            public ImageSource PlaceHolder
            {
                get { return (ImageSource)GetValue(PlaceHolderProperty); }
                set { SetValue(PlaceHolderProperty, value); }
            }

            /// <summary>
            /// Gets or sets the source image.
            /// </summary>
            public ImageSource Source
            {
                get { return (ImageSource)GetValue(SourceProperty); }
                set { SetValue(SourceProperty, value); }
            }

            private void LoadImage()
            {
                imageFadeInStoryboard.Completed += (s, e) =>
                {
                    PlaceHolder = null;
                    placeHolderImage.Source = null;
                };

                imageFadeInStoryboard.Begin();
            }

            private void OnSourceChanged()
            {
                if (Source != null)
                {
                    placeHolderImage.Source = PlaceHolder;
                    var bitmapImage = Source as BitmapImage;

                    if (bitmapImage != null)
                    {
                        image.Source = bitmapImage;

                        if (areAnimationsEnabled)
                        {
                            bitmapImage.ImageOpened += (sender, args) => LoadImage();
                        }
                        else
                        {
                            image.Opacity = 1.0;
                        }
                    }
                }
            }

            private static void SourcePropertyChanged(DependencyObject dependencyObject,
                DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
            {
                var control = dependencyObject as AnimatedImage;

                control?.OnSourceChanged();
            }
        }
    }

