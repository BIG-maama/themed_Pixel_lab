//using System;
//using System.Drawing;
//using System.Drawing.Drawing2D;

//namespace Homwore
//{
//    /// <summary>
//    /// Graphics extension methods for common drawing operations
//    /// </summary>
//    public static class GraphicsExtensions
//    {
//        /// <summary>
//        /// Draws a rectangle with rounded corners
//        /// </summary>
//        public static void DrawRoundedRectangle(this Graphics graphics, Pen pen, Rectangle rect, int radius)
//        {
//            if (graphics == null) throw new ArgumentNullException(nameof(graphics));
//            if (pen == null) throw new ArgumentNullException(nameof(pen));

//            using (var path = GetRoundedRectanglePath(rect, radius))
//            {
//                graphics.DrawPath(pen, path);
//            }
//        }

//        /// <summary>
//        /// Fills a rectangle with rounded corners
//        /// </summary>
//        public static void FillRoundedRectangle(this Graphics graphics, Brush brush, Rectangle rect, int radius)
//        {
//            if (graphics == null) throw new ArgumentNullException(nameof(graphics));
//            if (brush == null) throw new ArgumentNullException(nameof(brush));

//            using (var path = GetRoundedRectanglePath(rect, radius))
//            {
//                graphics.FillPath(brush, path);
//            }
//        }

//        /// <summary>
//        /// Creates a GraphicsPath for a rounded rectangle
//        /// </summary>
//        private static GraphicsPath GetRoundedRectanglePath(Rectangle rect, int radius)
//        {
//            var path = new GraphicsPath();
//            int diameter = radius * 2;

//            // Clamp radius to half the smallest dimension
//            int boundedRadius = Math.Min(radius, Math.Min(rect.Width, rect.Height) / 2);
//            diameter = boundedRadius * 2;

//            // Top-left arc
//            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
//            // Top line
//            path.AddLine(rect.X + boundedRadius, rect.Y, rect.Right - boundedRadius, rect.Y);
//            // Top-right arc
//            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
//            // Right line
//            path.AddLine(rect.Right, rect.Y + boundedRadius, rect.Right, rect.Bottom - boundedRadius);
//            // Bottom-right arc
//            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
//            // Bottom line
//            path.AddLine(rect.Right - boundedRadius, rect.Bottom, rect.X + boundedRadius, rect.Bottom);
//            // Bottom-left arc
//            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
//            // Left line
//            path.AddLine(rect.X, rect.Bottom - boundedRadius, rect.X, rect.Y + boundedRadius);
//            path.CloseFigure();

//            return path;
//        }
//    }
//}
