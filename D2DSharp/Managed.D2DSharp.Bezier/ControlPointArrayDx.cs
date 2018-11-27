﻿/* 
* ControlPointArray.cs
* 
* Authors: 
*  Dmitry Kolchev <dmitrykolchev@msn.com>
*  
* Copyright (C) 2010 Dmitry Kolchev
*
* This sourcecode is licenced under The GNU Lesser General Public License
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
* OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
* MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
* NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
* DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
* OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
* USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using Managed.Graphics;
using Managed.Graphics.Direct2D;
using System;
using System.Collections.Generic;

namespace Managed.D2DSharp.Bezier
{
    public class ControlPointArrayDx
    {
        private Vector4[] _points;

        private ControlPointArrayDx(int count)
        {
            _points = new Vector4[count];
        }
        public static ControlPointArrayDx Generate(int count, float minX, float maxX, float minY, float maxY, float minZ, float maxZ, float pointOfView, SizeF viewPortSize)
        {
            ControlPointArrayDx p = new ControlPointArrayDx(count);
            p.PointOfView = pointOfView;
            p.ViewPortSize = viewPortSize;
            p.Generate(minX, maxX, minY, maxY, minZ, maxZ);
            return p;
        }
        public IEnumerable<Vector4> Points
        {
            get { return _points; }
        }
        public int Count
        {
            get { return _points.Length; }
        }
        private void Generate(float minX, float maxX, float minY, float maxY, float minZ, float maxZ)
        {
            Random random = new Random();
            for (int index = 0; index < _points.Length; ++index)
            {
                _points[index] = new Vector4(
                    (float)(minX + random.NextDouble() * (maxX - minX)),
                    (float)(minY + random.NextDouble() * (maxY - minY)),
                    (float)(minZ + random.NextDouble() * (maxZ - minZ)),
                    1);
            }
        }
        public ControlPointArrayDx Reduce(float t)
        {
            ControlPointArrayDx result = new ControlPointArrayDx(Count - 1);
            result.PointOfView = PointOfView;
            result.ViewPortSize = ViewPortSize;
            result.Transform = Transform;
            int count = Count;
            for (var index = 0; index < count - 1; ++index)
            {
                result._points[index] = Vector4.Lerp(_points[index], _points[index + 1], t);
            }
            return result;
        }

        public float PointOfView { get; set; }
        public SizeF ViewPortSize { get; set; }
        public Matrix4x4 Transform { get; set; }

        private PointF Project(Vector4 p)
        {
            Vector4 s = p;
            p = p * Transform;
            double t = -PointOfView / (p.Z - PointOfView);
            double x = p.X * t + ViewPortSize.Width / 2;
            double y = p.Y * t + ViewPortSize.Height / 2;
            return new PointF((float)x, (float)y);
        }
        public Geometry CreateGeometry(Direct2DFactory factory)
        {
            PathGeometry geometry = factory.CreatePathGeometry();

            //using (GeometrySink sink = geometry.Open())
            //{
            //    sink.BeginFigure(Project(this._points[0]), FigureBegin.Hollow);
            //    for (int index = 1; index < this._points.Length; ++index)
            //    {
            //        sink.AddLine(Project(this._points[index]));
            //    }
            //    sink.EndFigure(FigureEnd.Open);
            //    sink.Close();
            //}
            List<Geometry> list = new List<Geometry>();
            //list.Add(geometry);

            for (int index = 0; index < _points.Length; ++index)
            {
                EllipseGeometry ellipse = factory.CreateEllipseGeometry(new Ellipse(Project(_points[index]), 5, 5));
                list.Add(ellipse);
            }

            GeometryGroup group = factory.CreateGeometryGroup(FillMode.Winding, list.ToArray());

            return group;
        }

    }
}
