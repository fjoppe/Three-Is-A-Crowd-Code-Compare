using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace TIAC.Visuals.Board
{
    internal class CrossSection
    {
        public UIElement link1;
        public UIElement link2;
    }

    public partial class Board : UserControl
    {
        List<UIElement> elements = new List<UIElement>();
        List<CrossSection> crossSections = new List<CrossSection>();

        public Board()
        {
            InitializeComponent();
        }

        const int width = 5;
        const int height = 18;

        const int hexHeight = (52 / 2)+1;
        const int hexWidth = 60 + 34;
        const int hexOffset = 47;

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            int hexCurrent = 0;
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    BoardHexagon hex = new BoardHexagon();

                    Canvas.SetLeft(hex, (i * hexWidth) + hexCurrent);
                    Canvas.SetTop(hex, j * hexHeight);

                    TheBoard.Children.Add(hex);
                    elements.Add(hex);
                }
                if (hexCurrent == 0)
                {
                    hexCurrent += hexOffset;
                }
                else
                {
                    hexCurrent = 0;
                }
            }

            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    //  left top
                    if (j > 0)
                    {
                        CrossSection leftTop = new CrossSection()
                        {
                            link1 = elements[((j - 1) * width) + i],
                            link2 = elements[(j * width) + i],
                        };

                        this.crossSections.Add(leftTop);
                        CreateLine(leftTop);
                    }

                    //  right top
                    //if ((i < width - 1)&&(j>0))
                    //{
                    //    CrossSection rightTop = new CrossSection()
                    //    {
                    //        link1 = elements[((j - 1) * width) + i+1],
                    //        link2 = elements[(j * width) + i],
                    //    };

                    //    this.crossSections.Add(rightTop);
                    //    CreateLine(rightTop);
                    //}

                    // left bottom
                    //if (j < height - 1)
                    //{
                    //    CrossSection leftTop = new CrossSection()
                    //    {
                    //        link1 = elements[((j + 1) * width) + i],
                    //        link2 = elements[(j * width) + i],
                    //    };

                    //    this.crossSections.Add(leftTop);
                    //    CreateLine(leftTop);
                    //}

                    //  right bottom
                    //if ((j < height - 1)&&(i<width-1))
                    //{
                    //    CrossSection leftTop = new CrossSection()
                    //    {
                    //        link1 = elements[((j + 1) * width) + i+1],
                    //        link2 = elements[(j * width) + i],
                    //    };

                    //    this.crossSections.Add(leftTop);
                    //    CreateLine(leftTop);
                    //}

                }
            }


        }

        private void CreateLine(CrossSection crossSection)
        {
            double dx1 = Canvas.GetLeft(crossSection.link1) + 30.0;
            double dy1 = Canvas.GetTop(crossSection.link1) + 26;

            double dx2 = Canvas.GetLeft(crossSection.link2) + 30.0;
            double dy2 = Canvas.GetTop(crossSection.link2) + 26.0;

            Line l1 = new Line()
            {
                X1 = dx1,
                Y1 = dy1,
                X2 = dx2,
                Y2 = dy2,
                Stroke = new SolidColorBrush(Colors.Purple)
            };

            this.CrossSections.Children.Add(l1);
        }


    }
}
