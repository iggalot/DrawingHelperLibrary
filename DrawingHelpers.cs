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

        public const double DEFAULT_DIM_TEXT_HT = 15;
        public const double DEFAULT_DIM_LEADER_HEIGHT = 30;
        public const double DEFAULT_DIM_LEADER_DROP_PERCENT = 0.3;
        public const double DEFAULT_DIM_LEADER_GAP = 5;
        public const double DEFAULT_DIM_LEADER_EXT = 10;
        public static Brush DEFAULT_DIM_LEADER_COLOR = Brushes.Green;
        public static Linetypes DEFAULT_DIM_LINETYPE = Linetypes.LINETYPE_SOLID;

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
        /// Draws a default aligned dimension with standard values
        /// </summary>
        /// <param name="c">cnavas object</param>
        /// <param name="ins_x">1st point x-coord</param>
        /// <param name="ins_y">1st point y-coord</param>
        /// <param name="end_x">2nd point x-coord</param>
        /// <param name="end_y">2nd point y-coord</param>
        /// <param name="text">text to display on the dimension</param>
        /// <param name="text_height"> height of the text</param>
        public static void DrawDimensionAligned(Canvas c, double ins_x, double ins_y, double end_x, double end_y, string text, double text_height)
        {
            DrawDimensionAligned(c, ins_x, ins_y, end_x, end_y, text, text_height,
                DEFAULT_DIM_LEADER_HEIGHT, DEFAULT_DIM_LEADER_DROP_PERCENT, DEFAULT_DIM_LEADER_GAP, DEFAULT_DIM_LEADER_EXT,
                DEFAULT_DIM_LEADER_COLOR, DEFAULT_DIM_LINETYPE);
        }


        /// <summary>
        /// Generic function to draw a vertical dimension line object to the right of the object
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
        /// <param name="text_ht">text height of the dimension object</param>
        /// <param name="ltype">linetype style to draw</param>
        public static void DrawDimensionAligned(Canvas c, double ins_x, double ins_y, double end_x, double end_y, string text, double text_ht,
            double dim_leader_height, double dim_leader_drop_percent, double dim_leader_gap, double dim_leader_ext,
            Brush color, Linetypes ltype)
        {
            // If the points are the same, no need to draw a dimension
            if ((ins_x == end_x) && (ins_y == end_y))
                return;

            // standardize the points so point 1 is always left of point 2
            double temp_x, temp_y;
            if (ins_x > end_x)
            {
                temp_x = ins_x;
                temp_y = ins_y;
                ins_x = end_x;
                ins_y = end_y;
                end_x = temp_x;
                end_y = temp_y;
            }
            // else if its a vertical dimension, make the bottom most point to be point 1
            else if (ins_x == end_x)
            {
                if (ins_y > end_y)
                {
                    temp_x = ins_x;
                    temp_y = ins_y;
                    ins_x = end_x;
                    ins_y = end_y;
                    end_x = temp_x;
                    end_y = temp_y;
                }
            }

            // determine the dimension parameters
            double angle = Math.Atan((end_y - ins_y) / (end_x - ins_x));
            double text_gap = 0.5 * text_ht;

            //// Display the dimleader #'s (FOR DEBUGGING)
            //DrawingHelpers.DrawText(c, ins_x, ins_y, 0, "1", Brushes.Black, text_ht);
            //DrawingHelpers.DrawText(c, end_x, end_y, 0, "2", Brushes.Black, text_ht);

            // the first dim leader line
            double x_dimleader1_ins = ins_x + dim_leader_gap * Math.Sin(angle);
            double y_dimleader1_ins = ins_y - dim_leader_gap * Math.Cos(angle);
            double x_dimleader1_end = ins_x + (dim_leader_height + dim_leader_ext) * Math.Sin(angle);
            double y_dimleader1_end = ins_y - (dim_leader_height + dim_leader_ext) * Math.Cos(angle);

            // the second dim leader
            double x_dimleader2_ins = end_x + dim_leader_gap * Math.Sin(angle);
            double y_dimleader2_ins = end_y - dim_leader_gap * Math.Cos(angle);
            double x_dimleader2_end = end_x + (dim_leader_height + dim_leader_ext) * Math.Sin(angle);
            double y_dimleader2_end = end_y - (dim_leader_height + dim_leader_ext) * Math.Cos(angle);

            // the text break line
            double x_dimtext1_ins = ins_x + dim_leader_height * Math.Sin(angle);
            double y_dimtext1_ins = ins_y - dim_leader_height * Math.Cos(angle);
            double x_dimtext1_end = end_x + (dim_leader_height) * Math.Sin(angle);
            double y_dimtext1_end = end_y - (dim_leader_height) * Math.Cos(angle);

            double middle_pt_x = 0.5 * (x_dimtext1_ins + x_dimtext1_end);
            double middle_pt_y = 0.5 * (y_dimtext1_ins + y_dimtext1_end);

            // if the dimension is down and to right (or up and to left), shift the text position
            double text_pt_x = middle_pt_x + text_gap * Math.Sin(angle);
            double text_pt_y = (angle <= 0) ? middle_pt_y + (text_gap) * Math.Cos(angle) : middle_pt_y - text_ht * 0.5 - text_gap * Math.Cos(angle);

            // Dimension leader 1.
            DrawingHelpers.DrawLine(c, x_dimleader1_ins, y_dimleader1_ins, x_dimleader1_end, y_dimleader1_end, Brushes.Green, 1, ltype);

            // Dimension leader 2.
            DrawingHelpers.DrawLine(c, x_dimleader2_ins, y_dimleader2_ins, x_dimleader2_end, y_dimleader2_end, Brushes.Green, 1, ltype);

            // Dimension text line.
            DrawingHelpers.DrawLine(c, x_dimtext1_ins, y_dimtext1_ins, x_dimtext1_end, y_dimtext1_end, Brushes.Green, 1, ltype);

            DrawingHelpers.DrawText(c, text_pt_x, text_pt_y, 0, text, Brushes.Green, text_ht);
        }

        /// <summary>
        /// Several test cases for display of aligned dimensions.
        /// </summary>
        /// <param name="canvas"></param>
        public static void AlignedDimensionTests(Canvas canvas)
        {
            double x1 = 100;
            double y1 = 100;
            double x2 = 200;
            double y2 = 50;
            DrawingHelpers.DrawLine(canvas, x1, y1, x2, y2, Brushes.Black, 1, Linetypes.LINETYPE_DASHED);
            DrawingHelpers.DrawDimensionAligned(canvas, x1, y1, x2, y2, "TEST", 15);

            DrawingHelpers.DrawLine(canvas, x1, y1, x2, y2, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
            DrawingHelpers.DrawDimensionAligned(canvas, x2 + 200, y2, x1 + 200, y1, "TEST", 15);

            DrawingHelpers.DrawLine(canvas, x1, y1, x2, y2, Brushes.Blue, 1, Linetypes.LINETYPE_DASHED);
            DrawingHelpers.DrawDimensionAligned(canvas, x1, y1 + 100, x2, y2 + 300, "TEST", 15);

            DrawingHelpers.DrawLine(canvas, x1, y1, x2, y2, Brushes.Green, 1, Linetypes.LINETYPE_DASHED);
            DrawingHelpers.DrawDimensionAligned(canvas, x2 + 200, y2 + 300, x1 + 200, y1 + 100, "TEST", 15);

            // Vertical points
            DrawingHelpers.DrawLine(canvas, x1 + 400, y1, x1 + 400, y2, Brushes.Green, 1, Linetypes.LINETYPE_DASHED);
            DrawingHelpers.DrawDimensionAligned(canvas, x1 + 400, y1, x1 + 400, y2, "TEST", 15);

            // Vertical points
            DrawingHelpers.DrawLine(canvas, x1 + 400, y1, x1 + 400, y2, Brushes.Green, 1, Linetypes.LINETYPE_DASHED);
            DrawingHelpers.DrawDimensionAligned(canvas, x1 + 600, y2, x1 + 600, y1, "TEST", 15);
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

        /// <summary>
        /// Draws a rectangle object pf specified height based on the input of the two points on the base of the rectangle
        /// </summary>
        /// <param name="c"></param>
        /// <param name="x_ins">x insert coord (bottom left)</param>
        /// <param name="y_ins">y insert coord (bottom left)</param>
        /// <param name="width">width of the rectangle</param>
        /// <param name="height">height of the rectangle</param>
        /// <param name="stroke">color of the line</param>
        /// <param name="thickness">thickness of the rectangle</param>
        /// <param name="ltype">linetype of the line</param>
        public static void DrawRectangleAligned_Base(Canvas c, double ins_x, double ins_y, double end_x, double end_y, double ht, Brush stroke, double thickness = 1.0, Linetypes ltype = Linetypes.LINETYPE_SOLID)
        {
            // If the points are the same, no need to draw a dimension
            if ((ins_x == end_x) && (ins_y == end_y))
                return;

            // standardize the points so point 1 is always left of point 2
            double temp_x, temp_y;
            if (ins_x > end_x)
            {
                temp_x = ins_x;
                temp_y = ins_y;
                ins_x = end_x;
                ins_y = end_y;
                end_x = temp_x;
                end_y = temp_y;
            }
            // else if its a vertical dimension, make the bottom most point to be point 1
            else if (ins_x == end_x)
            {
                if (ins_y > end_y)
                {
                    temp_x = ins_x;
                    temp_y = ins_y;
                    ins_x = end_x;
                    ins_y = end_y;
                    end_x = temp_x;
                    end_y = temp_y;
                }
            }

            double angle = Math.Atan((end_y - ins_y) / (end_x - ins_x));

            double x1 = ins_x;
            double y1 = ins_y;
            double x2 = end_x;
            double y2 = end_y;
            double x3 = x2 - ht * Math.Sin(angle);
            double y3 = y2 - ht * Math.Cos(angle);
            double x4 = x1 - ht * Math.Sin(angle); ;
            double y4 = y1 - ht * Math.Cos(angle);
            DrawingHelpers.DrawLine(c, x1, y1, x2, y2, stroke, thickness, ltype);
            DrawingHelpers.DrawLine(c, x2, y2, x3, y3, stroke, thickness, ltype);
            DrawingHelpers.DrawLine(c, x3, y3, x4, y4, stroke, thickness, ltype);
            DrawingHelpers.DrawLine(c, x4, y4, x1, y1, stroke, thickness, ltype);

            DrawingHelpers.DrawCircle(c, x1, y1, Brushes.Blue, Brushes.Black, 10, 1);
            DrawingHelpers.DrawCircle(c, x2, y2, Brushes.Red, Brushes.Black, 10, 1);
        }

        public static void DrawRectangleFilledAligned_Base(Canvas c, double ins_x, double ins_y, double end_x, double end_y, double ht, Brush stroke, Brush fill, double thickness = 1.0, Linetypes ltype = Linetypes.LINETYPE_SOLID)
        {
            // If the points are the same, no need to draw a dimension
            if ((ins_x == end_x) && (ins_y == end_y))
                return;

            // standardize the points so point 1 is always left of point 2
            double temp_x, temp_y;
            if (ins_x > end_x)
            {
                temp_x = ins_x;
                temp_y = ins_y;
                ins_x = end_x;
                ins_y = end_y;
                end_x = temp_x;
                end_y = temp_y;
            }
            // else if its a vertical dimension, make the bottom most point to be point 1
            else if (ins_x == end_x)
            {
                if (ins_y > end_y)
                {
                    temp_x = ins_x;
                    temp_y = ins_y;
                    ins_x = end_x;
                    ins_y = end_y;
                    end_x = temp_x;
                    end_y = temp_y;
                }
            }

            double angle = Math.Atan((end_y - ins_y) / (end_x - ins_x));

            double x1 = ins_x;
            double y1 = ins_y;
            double x2 = end_x;
            double y2 = end_y;
            double x3 = x2 + ht * Math.Sin(angle);
            double y3 = y2 - ht * Math.Cos(angle);
            double x4 = x1 + ht * Math.Sin(angle); ;
            double y4 = y1 - ht * Math.Cos(angle);
            DrawingHelpers.DrawLine(c, x1, y1, x2, y2, stroke, thickness, ltype);
            DrawingHelpers.DrawLine(c, x2, y2, x3, y3, stroke, thickness, ltype);
            DrawingHelpers.DrawLine(c, x3, y3, x4, y4, stroke, thickness, ltype);
            DrawingHelpers.DrawLine(c, x4, y4, x1, y1, stroke, thickness, ltype);


            //// Mark point 1 and 2 (testing)
            //DrawingHelpers.DrawCircle(c, x1, y1, Brushes.Blue, Brushes.Black, 10, 1);
            //DrawingHelpers.DrawCircle(c, x2, y2, Brushes.Red, Brushes.Black, 10, 1);

            // Split the rectangle intwo two triangles and draw the fill for each
            DrawTriangleFilled(c, x1, y1, x2, y2, x3, y3, stroke, fill);
            DrawTriangleFilled(c, x1, y1, x3, y3, x4, y4, stroke, fill);

        }

        public static void DrawTriangle(Canvas c, double x1, double y1, double x2, double y2, double x3, double y3, Brush stroke)
        {
            DrawLine(c, x1, y1, x2, y2, stroke);
            DrawLine(c, x2, y2, x3, y3, stroke);
            DrawLine(c, x1, y1, x3, y3, stroke);
        }

        public static void DrawTriangleFilled(Canvas c, double x1, double y1, double x2, double y2, double x3, double y3, Brush stroke, Brush fill)
        {

            DrawLine(c, x1, y1, x2, y2, stroke);
            DrawLine(c, x2, y2, x3, y3, stroke);
            DrawLine(c, x1, y1, x3, y3, stroke);

            double t1x, t2x, y, minx, maxx, t1xp, t2xp;
            bool changed1 = false;
            bool changed2 = false;
            double signx1, signx2, dx1, dy1, dx2, dy2;
            double e1, e2;

            // sort the verticies
            double tempx;
            double tempy;


            // y1 below y2
            if (y1 > y2)
            {
                tempx = x1;
                x1 = x2;
                x2 = tempx;

                tempy = y1;
                y1 = y2;
                y2 = tempy;
            }

            // new y1 below y3
            if (y1 > y3)
            {
                tempx = x1;
                x1 = x3;
                x3 = tempx;

                tempy = y1;
                y1 = y3;
                y3 = tempy;
            }

            // new y2 below new y3
            if (y2 > y3)
            {
                tempx = x2;
                x2 = x3;
                x3 = tempx;

                tempy = y2;
                y2 = y3;
                y3 = tempy;
            }

            // check for trivial case of bottom flat triangle
            if(y2 == y3)
            {
                FillBottomFlatTriangle(c, x1, y1, x2, y2, x3, y3, fill);
            } 
            // check for tribial case of top flat triangle
            else if (y1 == y2)
            {
                FillTopFlatTriangle(c, x1, y1, x2, y2, x3, y3, fill);
            }
            // general case -- split into a flat top and flat bottom cases
            else
            {
                double x4 = (x1 + (y2 - y1) / (y3 - y1) * (x3 - x1));
                double y4 = y2;
                FillBottomFlatTriangle(c, x1, y1, x2, y2, x4, y4, fill);
                FillTopFlatTriangle(c, x2, y2, x4, y4, x3, y3, fill);
            }
        }

        private static void FillBottomFlatTriangle(Canvas c, double x1, double y1, double x2, double y2, double x3, double y3, Brush fill)
        {
            double invslope12 = (x2 - x1) / (y2 - y1);  // 1/m for line 1-2
            double invslope13 = (x3 - x1) / (y3 - y1);  // 1/m for line 1-3

            double curx1 = x1;
            double curx2 = x1;

            for (double i = y1; i <= y2; i++)
            {
               DrawLine(c, curx1, i, curx2, i, fill);
                curx1 += invslope12;
                curx2 += invslope13;
            }
        }

        private static void FillTopFlatTriangle(Canvas c, double x1, double y1, double x2, double y2, double x3, double y3, Brush fill)
        {
            // Flat top triangle
            double invslope13 = (x3 - x1) / (y3 - y1);  // 1/m for line 1-2
            double invslope23 = (x3 - x2) / (y3 - y2);  // 1/m for line 1-3

            double curx1 = x3;
            double curx2 = x3;

            for (double i = y3; i > y1; i--)
            {
                DrawLine(c, curx1, i, curx2, i, fill);
                curx1 -= invslope13;
                curx2 -= invslope23;
            }
        }
    }
}
