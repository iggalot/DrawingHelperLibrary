// TEST

using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DrawingHelpersLibrary
{

    /// <summary>
    /// Enum for types of arrows (both straight and circular)
    /// </summary>
    public enum ArrowDirections
    {
        ARROW_UP = 0,
        ARROW_RIGHT = 1,
        ARROW_DOWN = 2,
        ARROW_LEFT = 3,
        ARROW_CLOCKWISE = 4,            // circular
        ARROW_COUNTERCLOCKWISE = 5      // circular
    }

    /// <summary>
    /// Enum for positioning of text relative to an object.
    /// </summary>
    public enum TextPositions
    {
        TEXT_ABOVE = 0,
        TEXT_BELOW = 1,
        TEXT_LEFT = 2,
        TEXT_RIGHT = 3
    }

    
    public enum Linetypes
    {
        LINETYPE_SOLID = 0,
        LINETYPE_DASHED = 1,
        LINETYPE_DASHED_X2 = 2,
        LINETYPE_CENTER = 3,
        LINETYPE_CENTER_X2 = 4,
        LINETYPE_PHANTOM = 5,
        LINETYPE_PHANTOM_X2 = 6
            
    }

    /// <summary>
    /// A class for drawing shapes onto a WPF canvas
    /// </summary>
    public class DrawingHelpers
    {
        // Constants used by the drawing helpers -- unless overridden in the functon call
        public const double DEFAULT_ARROW_SHAFTLENGTH = 20;    // arrow shaft length
        public const double DEFAULT_ARROW_HEADLENGTH = 8;      // arrow head length
        public const double DEFAULT_ARROW_THICKNESS = 3;       // line thickness of arrow components
        public const double DEFAULT_TEXT_HEIGHT = 12.0;        // text height (in pixels)

        private static DoubleCollection GetStrokeDashArray(Linetypes ltype)
        {
            switch (ltype)
            {
                case Linetypes.LINETYPE_CENTER:
                    return new DoubleCollection() { 4, 2 };
                case Linetypes.LINETYPE_CENTER_X2:
                    return new DoubleCollection() { 8, 4 };
                case Linetypes.LINETYPE_SOLID:
                    return new DoubleCollection() { };
                case Linetypes.LINETYPE_DASHED:
                    return new DoubleCollection() { 4 };
                case Linetypes.LINETYPE_DASHED_X2:
                    return new DoubleCollection() { 8 };
                case Linetypes.LINETYPE_PHANTOM:
                    return new DoubleCollection() { 10, 2, 4, 2, 4, 2 };
                case Linetypes.LINETYPE_PHANTOM_X2:
                    return new DoubleCollection() { 20, 4, 8, 4, 8, 4 };
                default:
                    return new DoubleCollection() { };
            }
        }

        /// <summary>
        /// Draws a basic circle (ellipse) on a WPF canvas
        /// </summary>
        /// <param name="c">the WPF canvas object</param>
        /// <param name="x">the upper left x-coordinate for a bounding box around the node</param>
        /// <param name="y">the upper left y-coordinate for a bounding box around the node</param>
        /// <param name="diameter">the diameter of the circle</param>
        /// <returns></returns>
        public static Shape DrawCircle(Canvas c, double x, double y, Brush fill, Brush stroke, double diameter, double thickness, Linetypes ltype = Linetypes.LINETYPE_SOLID)
        {
            // Draw circle node
            Ellipse myEllipse = new Ellipse();
            myEllipse.Fill = fill;
            myEllipse.Stroke = stroke;
            myEllipse.StrokeThickness = 2.0;
            myEllipse.StrokeDashArray = GetStrokeDashArray(ltype);

            myEllipse.Width = diameter;
            myEllipse.Height = diameter;

            Canvas.SetLeft(myEllipse, x - myEllipse.Width / 2.0);
            Canvas.SetTop(myEllipse, y - myEllipse.Height / 2.0);

            c.Children.Add(myEllipse);

            return myEllipse;
        }

        public static Shape DrawCircleHollow(Canvas c, double x, double y, Brush stroke, double diameter, double thickness = 1.0, Linetypes ltype = Linetypes.LINETYPE_SOLID)
        {
            return DrawCircle(c, x, y, Brushes.Transparent, stroke, diameter, thickness, ltype);
        }

        /// <summary>
        /// Draws a basic line object on a WPF canvas
        /// </summary>
        /// <param name="c">the WPF canvas object</param>
        /// <param name="ex">end point x-coord</param>
        /// <param name="ey">end point y-coord</param>
        /// <param name="sx">start point x_coord</param>
        /// <param name="sy">start point y-coord</param>
        /// <param name="stroke">color of the line object as a <see cref="Brush"/></param>
        /// <returns>the Shape object</returns>
        public static Shape DrawLine(Canvas c, double sx, double sy, double ex, double ey, Brush stroke, double thickness = 1.0, Linetypes ltype = Linetypes.LINETYPE_SOLID)
        {
            Line myLine = new Line();
            myLine.Stroke = stroke;
            myLine.StrokeThickness = thickness;
            myLine.StrokeDashArray = GetStrokeDashArray(ltype);
            myLine.X1 = sx;
            myLine.Y1 = sy;
            myLine.X2 = ex;
            myLine.Y2 = ey;

            c.Children.Add(myLine);

            return myLine;
        }

        /// <summary>
        /// Draws a circular arc on a WPF canvas object
        /// Angles are measured as positive - clockwise.
        /// </summary>
        /// <param name="c">canvas to draw on</param>
        /// <param name="x">x-coord for center of circlular arc</param>
        /// <param name="y">y-coord for center of circular arc</param>
        /// <param name="fill">the fill color for the arc -- usually transparent</param>
        /// <param name="stroke">the stroke line color of the arc</param>
        /// <param name="radius">radius of the ciruclar arrow</param>
        /// <param name="thickness">stroke thickness of the arrow</param>
        /// <param name="end_angle">angle from center to the end of the arc (clockwise positive)/param>
        /// <param name="start_angle">angle from center to the start of the arc (clockwise positive)/param>
        /// <param name="head_len">length of the arrow head in pixels</param>
        public static void DrawCircularArc(Canvas c, double x, double y, Brush fill, Brush stroke, double thickness, double radius, double start_angle, double end_angle, SweepDirection sweep = SweepDirection.Counterclockwise)
        {
            double sa, ea;

            Path path = new Path();
            path.Fill = fill; ;
            path.Stroke = stroke;
            path.StrokeThickness = thickness;
            Canvas.SetLeft(path, 0);
            Canvas.SetTop(path, 0);

            sa = ((start_angle % (Math.PI * 2)) + Math.PI * 2) % (Math.PI * 2);
            ea = ((end_angle % (Math.PI * 2)) + Math.PI * 2) % (Math.PI * 2);

            if (ea < sa)
            {
                double temp_angle = ea;
                ea = sa;
                sa = ea;
            }

            double angle_diff = ea - sa;

            PathGeometry pg = new PathGeometry();
            PathFigure pf = new PathFigure();

            ArcSegment arcSegment = new ArcSegment();
            arcSegment.IsLargeArc = angle_diff >= Math.PI;

            // Set the start of arc
            pf.StartPoint = new System.Windows.Point(x - radius * Math.Cos(sa), y - radius * Math.Sin(sa));

            // // Draws a node at the start point for reference
            DrawingHelpers.DrawCircle(c, pf.StartPoint.X, pf.StartPoint.Y, Brushes.Pink, Brushes.Black, 0.5 * radius, 2);

            // Set the end point of the arc
            arcSegment.Point = new System.Windows.Point(x - radius * Math.Cos(ea), y - radius * Math.Sin(ea));

            arcSegment.Size = new System.Windows.Size(0.8 * radius, radius);
            arcSegment.SweepDirection = sweep;

            pf.Segments.Add(arcSegment);
            pg.Figures.Add(pf);
            path.Data = pg;

            c.Children.Add(path);

            return;
        }

        /// <summary>
        /// Helper function to draw text on a WPF canvas
        /// </summary>
        /// <param name="c">canvas to draw on</param>
        /// <param name="x">x-coord</param>
        /// <param name="y">y-coord</param>
        /// <param name="z">z-coord (0 for 2D)</param>
        /// <param name="str">text string</param>
        /// <param name="brush">color of the text</param>
        /// <param name="size">size of the text</param>
        public static void DrawText(Canvas c, double x, double y, double z, string str, Brush brush, double size)
        {
            double xpos = x;
            double ypos = y;
            double zpos = z;

            if (string.IsNullOrEmpty(str))
                return;
            // Draw text
            TextBlock textBlock = new TextBlock();
            textBlock.Text = str;
            textBlock.FontSize = size;
            textBlock.Foreground = brush;
            textBlock.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;


            Canvas.SetLeft(textBlock, xpos);
            Canvas.SetTop(textBlock, ypos);

            c.Children.Add(textBlock);
        }


        /// <summary>
        /// A helper function to draw an arrow in a specified direction on a WPF canvas
        /// <param name="c">canvas to draw on</param>
        /// <param name="x">x-coord</param>
        /// <param name="y">y-coord</param>
        /// <param name="fill">fill color --- usually transparent</param>
        /// <param name="stroke">color of the arrow</param>
        /// <param name="dir">enum for the direction of the arroe <see cref="ArrowDirections"/></param>
        /// <param name="thickness">stroke thickness of the arrow</param>
        /// <param name="shaft_len">length of the arrow shaft in pixels</param>
        /// <param name="head_len">length of the arrow head in pixels</param>
        public static void DrawArrow(Canvas c, double x, double y, Brush fill, Brush stroke, ArrowDirections dir, double thickness, double shaft_len = DEFAULT_ARROW_SHAFTLENGTH, double head_len = DEFAULT_ARROW_HEADLENGTH)
        {
            switch (dir)
            {
                case ArrowDirections.ARROW_DOWN:
                    DrawArrowDown(c, x, y, fill, stroke, thickness, shaft_len, head_len);
                    break;
                case ArrowDirections.ARROW_UP:
                    DrawArrowUp(c, x, y, fill, stroke, thickness, shaft_len, head_len);
                    break;
                case ArrowDirections.ARROW_RIGHT:
                case ArrowDirections.ARROW_LEFT:
                default:
                    throw new NotImplementedException("draw function not defined for arrow of direction = " + dir);
            }
        }

        /// <summary>
        /// A helper function to draw downward arrow
        /// <param name="c">canvas to draw on</param>
        /// <param name="x">x-coord</param>
        /// <param name="y">y-coord</param>
        /// <param name="fill">fill color --- usually transparent</param>
        /// <param name="stroke">color of the arrow</param>
        /// <param name="dir">enum for the direction of the arroe <see cref="ArrowDirections"/></param>
        /// <param name="thickness">stroke thickness of the arrow</param>
        /// <param name="shaft_len">length of the arrow shaft in pixels</param>
        /// <param name="head_len">length of the arrow head in pixels</param>
        public static void DrawArrowDown(Canvas c, double x, double y, Brush fill, Brush stroke, double thickness, double shaft_len, double head_len)
        {
            DrawLine(c, x, y, x, y - shaft_len, stroke, thickness);
            DrawLine(c, x, y, x - head_len, y - head_len, stroke, thickness);
            DrawLine(c, x, y, x + head_len, y - head_len, stroke, thickness);
        }


        /// <summary>
        /// A helper function to draw an upward arrow in a specified direction
        /// <param name="c">canvas to draw on</param>
        /// <param name="x">x-coord</param>
        /// <param name="y">y-coord</param>
        /// <param name="fill">fill color --- usually transparent</param>
        /// <param name="stroke">color of the arrow</param>
        /// <param name="dir">enum for the direction of the arroe <see cref="ArrowDirections"/></param>
        /// <param name="thickness">stroke thickness of the arrow</param>
        /// <param name="shaft_len">length of the arrow shaft in pixels</param>
        /// <param name="head_len">length of the arrow head in pixels</param>
        public static void DrawArrowUp(Canvas c, double x, double y, Brush fill, Brush stroke, double thickness, double shaft_len, double head_len)
        {
            DrawLine(c, x, y, x, y + shaft_len, stroke, thickness);
            DrawLine(c, x, y, x - head_len, y + head_len, stroke, thickness);
            DrawLine(c, x, y, x + head_len, y + head_len, stroke, thickness);
        }

        /// <summary>
        /// Draws a circular arrow in a particular direction
        /// Angles are measured as positive - clockwise.
        /// </summary>
        /// <param name="c">canvas to draw on</param>
        /// <param name="x">x-coord for center of circlular arc</param>
        /// <param name="y">y-coord for center of circular arc</param>
        /// <param name="dir">enum for the direction of the circular arrow <see cref="ArrowDirections"/></param>
        /// <param name="radius">radius of the ciruclar arrow</param>
        /// <param name="thickness">stroke thickness of the arrow</param>
        /// <param name="end_angle">angle from center to the end of the arc (clockwise positive)/param>
        /// <param name="start_angle"/>angle from center to the start of the arc (clockwise positive)/param>
        /// <param name="head_len">length of the arrow head in pixels</param>
        public static void DrawCircularArrow(Canvas c, double x, double y, Brush fill, Brush stroke,
            ArrowDirections dir, double thickness = DEFAULT_ARROW_THICKNESS,
            double radius = 32.0, double start_angle = Math.PI / 2.0, double end_angle = (-1) * Math.PI / 2.0,
            double head_len = DEFAULT_ARROW_HEADLENGTH)
        {
            double s_x, s_y;
            double e_x, e_y;

            // Ensure that the angles are between zero and 2 x pi
            double sa = ((start_angle % (Math.PI * 2)) + Math.PI * 2) % (Math.PI * 2);
            double ea = ((end_angle % (Math.PI * 2)) + Math.PI * 2) % (Math.PI * 2);

            // switch the end and start angle if they are outsize the zero to 2x pi range.
            if (ea < sa)
            {
                double temp_angle = ea;
                ea = sa;
                sa = ea;
            }

            // Draw the circular arc
            DrawingHelpers.DrawCircularArc(c, x, y, fill, stroke, thickness, radius, sa, ea);

            // Draw the arrow head
            s_x = x - radius * Math.Cos(sa);
            s_y = y - radius * Math.Sin(sa);
            e_x = x - radius * Math.Cos(ea);
            e_y = y - radius * Math.Sin(ea);

            if (dir == ArrowDirections.ARROW_COUNTERCLOCKWISE)
            {
                // use the endpoint to draw the head
                DrawLine(c, e_x, e_y, e_x - head_len, e_y - head_len, stroke, thickness);
                DrawLine(c, e_x, e_y, e_x - head_len, e_y + head_len, stroke, thickness);
            }
            else
            {
                // use the startpoint to draw the head
                DrawLine(c, s_x, s_y, s_x - head_len, s_y - head_len, stroke, thickness);
                DrawLine(c, s_x, s_y, s_x - head_len, s_y + head_len, stroke, thickness);
            }
        }

        /// <summary>
        /// Helper function to draw a horizontal dimension line object above the object
        /// </summary>
        /// <param name="c"></param>
        /// <param name="dim_leader_height">the height of the dimension leader to draw</param>
        /// <param name="dim_leader_drop_percent">the percent of the dimension leader height at which the horizontal line is added.</param>
        /// <param name="dim_leader_gap">the gap distance in y-direction between the dimension leader start point and the object being dimensioned.</param>
        /// <param name="ins_x">the start x-point of the object being dimensioned</param>
        /// <param name="ins_y">the start y-point of the object being dimensioned</param>
        /// <param name="end_x">the end y-point of the object being dimensioned</param>
        /// <param name="end_y">the end y-point of the object being dimensioned</param>
        /// <param name="text">the text string to right at the middled of the dimension</param>
        /// <param name="ltype">linetype style to draw</param>
        public static void DrawHorizontalDimension_Above(Canvas c, double dim_leader_height, double dim_leader_drop_percent, double dim_leader_gap, double ins_x, double ins_y, double end_x, double end_y, string text, Linetypes ltype = Linetypes.LINETYPE_SOLID)
        {
            double dim_ldr_ext = 10;
            double middle_pt_x = 0.5 * (end_x + ins_x);
            double dim_gap = 100;
            double y_loc = ins_y - dim_leader_height * (1 + dim_leader_drop_percent);

            double x_gap_start = middle_pt_x - dim_gap / 2.0;
            double x_gap_end = middle_pt_x + dim_gap / 2.0;

            // Draw midpoint line
            //DrawingHelpers.DrawLine(c, middle_pt_x, 0, middle_pt_x, 1000, Brushes.Aqua, 1);

            // Left vertical dimension leader.
            DrawingHelpers.DrawLine(c, ins_x, ins_y - dim_leader_gap, ins_x, y_loc - dim_ldr_ext, Brushes.Green, 1, ltype);

            // Right vertical dimension leader
            DrawingHelpers.DrawLine(c, end_x, end_y - dim_leader_gap, end_x, y_loc - dim_ldr_ext, Brushes.Green, 1, ltype);

            // Horizontal header line to left of text
            DrawingHelpers.DrawLine(c, ins_x, y_loc, x_gap_start, y_loc, Brushes.Green, 1, ltype);
            DrawingHelpers.DrawLine(c, x_gap_end, y_loc, end_x, y_loc, Brushes.Green, 1, ltype);

            // TODO:: How to find the centerpoint?
            // Draw the text at the approximate middle of the horizontal dimension line
            DrawingHelpers.DrawText(c, middle_pt_x - 15, y_loc - 10, 0, text, Brushes.Green, 15);
        }

        /// <summary>
        /// Helper function to draw a horizontal dimension line object below the object
        /// </summary>
        /// <param name="c"></param>
        /// <param name="dim_leader_height">the height of the dimension leader to draw</param>
        /// <param name="dim_leader_drop_percent">the percent of the dimension leader height at which the horizontal line is added.</param>
        /// <param name="dim_leader_gap">the gap distance in y-direction between the dimension leader start point and the object being dimensioned.</param>
        /// <param name="ins_x">the start x-point of the object being dimensioned</param>
        /// <param name="ins_y">the start y-point of the object being dimensioned</param>
        /// <param name="end_x">the end y-point of the object being dimensioned</param>
        /// <param name="end_y">the end y-point of the object being dimensioned</param>
        /// <param name="text">the text string to right at the middled of the dimension</param>
        /// <param name="ltype">linetype style to draw</param>
        public static void DrawHorizontalDimension_Below(Canvas c, double dim_leader_height, double dim_leader_drop_percent, double dim_leader_gap, double ins_x, double ins_y, double end_x, double end_y, string text, Linetypes ltype = Linetypes.LINETYPE_SOLID)
        {
            double dim_ldr_ext = 10;
            double middle_pt_x = 0.5 * (end_x + ins_x);
            double dim_gap = 100;
            double y_loc = ins_y + dim_leader_height * (1 + dim_leader_drop_percent);

            double x_gap_start = middle_pt_x - dim_gap / 2.0;
            double x_gap_end = middle_pt_x + dim_gap / 2.0;


            // Draw midpoint line
            //DrawingHelpers.DrawLine(c, middle_pt_x, 0, middle_pt_x, 1000, Brushes.Aqua, 1);

            // Left vertical dimension leader.
            DrawingHelpers.DrawLine(c, ins_x, ins_y + dim_leader_gap, ins_x, y_loc + dim_ldr_ext, Brushes.Green, 1, ltype);

            // Right vertical dimension leader
            DrawingHelpers.DrawLine(c, end_x, end_y + dim_leader_gap, end_x, y_loc + dim_ldr_ext, Brushes.Green, 1, ltype);

            // Horizontal header line to left of text
            DrawingHelpers.DrawLine(c, ins_x, y_loc, x_gap_start, y_loc, Brushes.Green, 1, ltype);
            DrawingHelpers.DrawLine(c, x_gap_end, y_loc, end_x, y_loc, Brushes.Green, 1, ltype);

            // TODO:: How to find the centerpoint?
            // Draw the text at the approximate middle of the horizontal dimension line
            DrawingHelpers.DrawText(c, middle_pt_x - 15, y_loc - 10, 0, text, Brushes.Green, 15);
        }

        /// <summary>
        /// Helper function to draw a vertical dimension line object to the left of the object
        /// </summary>
        /// <param name="c"></param>
        /// <param name="dim_leader_height">the height of the dimension leader to draw</param>
        /// <param name="dim_leader_drop_percent">the percent of the dimension leader height at which the horizontal line is added.</param>
        /// <param name="dim_leader_gap">the gap distance in x-direction between the dimension leader start point and the object being dimensioned.</param>
        /// <param name="ins_x">the start x-point of the object being dimensioned</param>
        /// <param name="ins_y">the start y-point of the object being dimensioned</param>
        /// <param name="end_x">the end y-point of the object being dimensioned</param>
        /// <param name="end_y">the end y-point of the object being dimensioned</param>
        /// <param name="text">the text string to right at the middled of the dimension</param>
        /// <param name="ltype">linetype style to draw</param>
        public static void DrawVerticalDimension_Left(Canvas c, double dim_leader_height, double dim_leader_drop_percent, double dim_leader_gap, double ins_x, double ins_y, double end_x, double end_y, string text, Linetypes ltype = Linetypes.LINETYPE_SOLID)
        {
            double dim_ldr_ext = 10;
            double dim_gap = 50;

            double middle_pt_y = 0.5 * (end_y + ins_y);
            double x_loc_ins = ins_x - dim_leader_height * (1 + dim_leader_drop_percent);
            double x_loc_end = end_x - dim_leader_height * (1 + dim_leader_drop_percent);

            double y_gap_start = middle_pt_y - dim_gap / 2.0;
            double y_gap_end = middle_pt_y + dim_gap / 2.0;

            // Draw midpoint line
            //DrawingHelpers.DrawLine(c, 0, middle_pt_y, 1000, middle_pt_y, Brushes.Aqua, 1);

            // Left top horizontal dimension leader.
            DrawingHelpers.DrawLine(c, ins_x - dim_leader_gap, ins_y, x_loc_ins - dim_ldr_ext, ins_y, Brushes.Green, 1, ltype);

            // Right bottom horizontal dimension leader
            DrawingHelpers.DrawLine(c, end_x - dim_leader_gap, end_y, x_loc_end - dim_ldr_ext, end_y, Brushes.Green, 1, ltype);

            // Horizontal header line to left of text
            DrawingHelpers.DrawLine(c, x_loc_ins, ins_y, x_loc_end, y_gap_start, Brushes.Green, 1, ltype);
            DrawingHelpers.DrawLine(c, x_loc_ins, y_gap_end, x_loc_end, end_y, Brushes.Green, 1, ltype);

            // TODO:: How to find the centerpoint?
            // Draw the text at the approximate middle of the horizontal dimension line
            DrawingHelpers.DrawText(c, x_loc_ins - 15, middle_pt_y - 20, 0, text, Brushes.Green, 15);
        }

        /// <summary>
        /// Helper function to draw a vertical dimension line object to the right of the object
        /// </summary>
        /// <param name="c"></param>
        /// <param name="dim_leader_height">the height of the dimension leader to draw</param>
        /// <param name="dim_leader_drop_percent">the percent of the dimension leader height at which the horizontal line is added.</param>
        /// <param name="dim_leader_gap">the gap distance in x-direction between the dimension leader start point and the object being dimensioned.</param>
        /// <param name="ins_x">the start x-point of the object being dimensioned</param>
        /// <param name="ins_y">the start y-point of the object being dimensioned</param>
        /// <param name="end_x">the end y-point of the object being dimensioned</param>
        /// <param name="end_y">the end y-point of the object being dimensioned</param>
        /// <param name="text">the text string to right at the middled of the dimension</param>
        /// <param name="ltype">linetype style to draw</param>
        public static void DrawVerticalDimension_Right(Canvas c, double dim_leader_height, double dim_leader_drop_percent, double dim_leader_gap, double ins_x, double ins_y, double end_x, double end_y, string text, Linetypes ltype = Linetypes.LINETYPE_SOLID)
        {
            double dim_ldr_ext = 10;
            double dim_gap = 50;

            double middle_pt_y = 0.5 * (end_y + ins_y);
            double x_loc_ins = ins_x + dim_leader_height * (1 + dim_leader_drop_percent);
            double x_loc_end = end_x + dim_leader_height * (1 + dim_leader_drop_percent);

            double y_gap_start = middle_pt_y - dim_gap / 2.0;
            double y_gap_end = middle_pt_y + dim_gap / 2.0;

            // Draw midpoint line
            //DrawingHelpers.DrawLine(c, 0, middle_pt_y, 1000, middle_pt_y, Brushes.Aqua, 1);

            // Left top horizontal dimension leader.
            DrawingHelpers.DrawLine(c, ins_x + dim_leader_gap, ins_y, x_loc_ins + dim_ldr_ext, ins_y, Brushes.Green, 1, ltype);

            // Right bottom horizontal dimension leader
            DrawingHelpers.DrawLine(c, end_x + dim_leader_gap, end_y, x_loc_end + dim_ldr_ext, end_y, Brushes.Green, 1, ltype);

            // Horizontal header line to left of text
            DrawingHelpers.DrawLine(c, x_loc_ins, ins_y, x_loc_end, y_gap_start, Brushes.Green, 1, ltype);
            DrawingHelpers.DrawLine(c, x_loc_ins, y_gap_end, x_loc_end, end_y, Brushes.Green, 1, ltype);

            // TODO:: How to find the centerpoint?
            // Draw the text at the approximate middle of the horizontal dimension line
            DrawingHelpers.DrawText(c, x_loc_ins - 15, middle_pt_y - 20, 0, text, Brushes.Green, 15);
        }

        /// <summary>
        /// Draws a 4-point closed polyline
        /// </summary>
        /// <param name="c">the canvas</param>
        /// <param name="x1">point 1 x</param>
        /// <param name="y1">point 1 y</param>
        /// <param name="x2">point 2 x</param>
        /// <param name="y2">point 2 y</param>
        /// <param name="x3">point 3 x</param>
        /// <param name="y3">point 3 y</param>
        /// <param name="x4">point 4 x</param>
        /// <param name="y4">point 4 y</param>
        /// <param name="stroke">color of the line</param>
        /// <param name="thickness">thickness of the line</param>
        /// <param name="ltype">linetype <see cref="Linetypes"/></param>
        public static void DrawPoly_4Pt(Canvas c, double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, Brush stroke, double thickness = 1.0, Linetypes ltype = Linetypes.LINETYPE_SOLID)
        {
            DrawingHelpers.DrawLine(c, x1, y1, x2, y2, stroke, thickness, ltype);
            DrawingHelpers.DrawLine(c, x2, y2, x3, y3, stroke, thickness, ltype);
            DrawingHelpers.DrawLine(c, x3, y3, x4, y4, stroke, thickness, ltype);
            DrawingHelpers.DrawLine(c, x4, y4, x1, y1, stroke, thickness, ltype);
        }

        /// <summary>
        /// Draws a rectangle object pf specified height and width
        /// </summary>
        /// <param name="c"></param>
        /// <param name="x_ins">x insert coord (bottom left)</param>
        /// <param name="y_ins">y insert coord (bottom left)</param>
        /// <param name="width">width of the rectangle</param>
        /// <param name="height">height of the rectangle</param>
        /// <param name="stroke">color of the line</param>
        /// <param name="thickness">thickness of the rectangle</param>
        /// <param name="ltype">linetype of the line</param>
        public static void DrawRectangle(Canvas c, double x_ins, double y_ins, double width, double height, Brush stroke, double thickness = 1.0, Linetypes ltype = Linetypes.LINETYPE_SOLID)
        {
            double x1 = x_ins;
            double y1 = y_ins;
            double x2 = x_ins + width;
            double y2 = y_ins;
            double x3 = x_ins + width;
            double y3 = y_ins + height;
            double x4 = x_ins;
            double y4 = y_ins + height;
            DrawingHelpers.DrawLine(c, x1, y1, x2, y2, stroke, thickness, ltype);
            DrawingHelpers.DrawLine(c, x2, y2, x3, y3, stroke, thickness, ltype);
            DrawingHelpers.DrawLine(c, x3, y3, x4, y4, stroke, thickness, ltype);
            DrawingHelpers.DrawLine(c, x4, y4, x1, y1, stroke, thickness, ltype);
        }
    }
}
