﻿// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Math.Primitives;

namespace OsmSharp.Math.Geo
{
    /// <summary>
    /// Represents a geo coordinate bounding box.
    /// 
    /// 0: longitude.
    /// 1: latitude.
    /// </summary>
    public class GeoCoordinateBox : GenericRectangleF2D<GeoCoordinate>
    {
        /// <summary>
        /// Creates a new box.
        /// </summary>
        /// <param name="points"></param>
        public GeoCoordinateBox(IList<GeoCoordinate> points)
            : base(points)
        {

        }

        /// <summary>
        /// Creates a new box.
        /// </summary>
        /// <param name="points"></param>
        public GeoCoordinateBox(GeoCoordinate[] points)
            : base(points)
        {

        }

        /// <summary>
        /// Creates a new box.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public GeoCoordinateBox(GeoCoordinate a, GeoCoordinate b)
            :base(a,b)
        {

        }
        
        #region Calculations

        /// <summary>
        /// Generates a random point within this box.
        /// </summary>
        /// <returns></returns>
        public GeoCoordinate GenerateRandomIn()
        {
            return this.GenerateRandomIn(new System.Random());
        }

        /// <summary>
        /// Generates a random point within this box.
        /// </summary>
        /// <param name="rand"></param>
        /// <returns></returns>
        public GeoCoordinate GenerateRandomIn(System.Random rand)
        {
            double lat = (double)rand.NextDouble() * this.DeltaLat;
            double lon = (double)rand.NextDouble() * this.DeltaLon;

            return new GeoCoordinate(this.MinLat + lat,
                this.MinLon + lon);
        }

        /// <summary>
        /// Returns a scaled version of this bounding box.
        /// </summary>
        /// <param name="factor"></param>
        /// <returns></returns>
        public GeoCoordinateBox Scale(double factor)
        {
            if (factor <= 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            GeoCoordinate center = this.Center;

            double diff_lat = (this.DeltaLat * factor) / 2.0;
            double diff_lon = (this.DeltaLon * factor) / 2.0;

            return new GeoCoordinateBox(
                new GeoCoordinate(
                center.Latitude - diff_lat,
                center.Longitude - diff_lon),
                new GeoCoordinate(
                center.Latitude + diff_lat,
                center.Longitude + diff_lon));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns the maximum longitude.
        /// </summary>
        public double MaxLon
        {
            get
            {
                return this.Max[0];
            }
        }

        /// <summary>
        /// Returns the maximum latitude.
        /// </summary>
        public double MaxLat
        {
            get
            {
                return this.Max[1];
            }
        }

        /// <summary>
        /// Returns the minimum longitude.
        /// </summary>
        public double MinLon
        {
            get
            {
                return this.Min[0];
            }
        }

        /// <summary>
        /// Returns the minimum latitude.
        /// </summary>
        public double MinLat
        {
            get
            {
                return this.Min[1];
            }
        }

        /// <summary>
        /// Returns the topleft coordinate.
        /// </summary>
        public GeoCoordinate TopLeft
        {
            get { return new GeoCoordinate(this.MinLon, this.MaxLat); }
        }

        /// <summary>
        /// Returns the topleft coordinate.
        /// </summary>
        public GeoCoordinate TopRight
        {
            get { return new GeoCoordinate(this.MaxLon, this.MaxLat); }
        }

        /// <summary>
        /// Returns the topleft coordinate.
        /// </summary>
        public GeoCoordinate BottomLeft
        {
            get { return new GeoCoordinate(this.MinLon, this.MinLat); }
        }

        /// <summary>
        /// Returns the topleft coordinate.
        /// </summary>
        public GeoCoordinate BottomRight
        {
            get { return new GeoCoordinate(this.MaxLon, this.MinLat); }
        }

        /// <summary>
        /// Returns the width on this box.
        /// </summary>
        public double DeltaLon
        {
            get
            {
                return this.Delta[0];
            }
        }  
        
        /// <summary>
        /// Returns the height on this box.
        /// </summary>
        public double DeltaLat
        {
            get
            {
                return this.Delta[1];
            }
        }

        /// <summary>
        /// Returns the center of this box.
        /// </summary>
        public GeoCoordinate Center 
        {
            get
            {
                return new GeoCoordinate(
                    (this.MaxLat + this.MinLat) / 2f,
                    (this.MaxLon + this.MinLon) / 2f);
            }
        }

        #endregion

        #region Operators

        /// <summary>
        /// Adds two bounding boxes together yielding as result the smallest box that surrounds both.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static GeoCoordinateBox operator +(GeoCoordinateBox a, GeoCoordinateBox b)
        {
            List<GeoCoordinate> points = new List<GeoCoordinate>();
            points.AddRange(a.Corners);
            points.AddRange(b.Corners);
            
            return new GeoCoordinateBox(points);
        }

        /// <summary>
        /// Subtracts two bounding boxes; returns the box that represents the overlap between the two.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static GeoCoordinateBox operator -(GeoCoordinateBox a, GeoCoordinateBox b)
        {
            if (a.Overlaps(b))
            {
                // sort all longitudes.
                List<double> longitudes = new List<double>();
                longitudes.Add(a.MinLon);
                longitudes.Add(a.MaxLon);
                longitudes.Add(b.MinLon);
                longitudes.Add(b.MaxLon);

                longitudes.Sort();

                // sort all latitudes.
                List<double> latitudes = new List<double>();
                latitudes.Add(a.MinLat);
                latitudes.Add(a.MaxLat);
                latitudes.Add(b.MinLat);
                latitudes.Add(b.MaxLat);

                latitudes.Sort();

                return new GeoCoordinateBox(
                    new GeoCoordinate(latitudes[1], longitudes[1]),
                    new GeoCoordinate(latitudes[2], longitudes[2]));
            }
            return null;
        }

        #endregion

        #region Factory

        /// <summary>
        /// Creates a new point.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        protected override GeoCoordinate CreatePoint(double[] values)
        {
            return new GeoCoordinate(values);
        }

        /// <summary>
        /// Creates a new line.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="isSegment1"></param>
        /// <param name="isSegment2"></param>
        /// <returns></returns>
        protected override GenericLineF2D<GeoCoordinate> CreateLine(
            GeoCoordinate point1,
            GeoCoordinate point2,
            bool isSegment1,
            bool isSegment2)
        {
            return new GeoCoordinateLine(point1, point2, isSegment1, isSegment2);
        }

        /// <summary>
        /// Creates a new rectangle
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        protected override GenericRectangleF2D<GeoCoordinate> CreateRectangle(GeoCoordinate[] points)
        {
            return new GeoCoordinateBox(points);
        }

        /// <summary>
        /// Creates a new polygon.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        protected override GenericPolygonF2D<GeoCoordinate> CreatePolygon(GeoCoordinate[] points)
        {
            return new GeoCoordinatePolygon(points);
        }

        #endregion


        /// <summary>
        /// Calculates the intersection between this box and the given box.
        /// </summary>
        /// <param name="box">Box.</param>
        public GeoCoordinateBox Intersection(GeoCoordinateBox box)
        {// get the highest minimums and the lowest maximums.
            double minX = System.Math.Max(this.Min[0], box.Min[0]);
            double minY = System.Math.Max(this.Min[1], box.Min[1]);
            double maxX = System.Math.Min(this.Max[0], box.Max[0]);
            double maxY = System.Math.Min(this.Max[1], box.Max[1]);

            if (minX <= maxX && minY <= maxY)
            {
                return new GeoCoordinateBox(new GeoCoordinate(minY, minX), new GeoCoordinate(maxY, maxX));
            }
            return null;
        }

        /// <summary>
        /// Calculates the union of this box and the given box or the box that encompasses both original boxes.
        /// </summary>
        /// <param name="box">Box.</param>
        public GeoCoordinateBox Union(GeoCoordinateBox box)
        {// get the lowest minimums and the highest maximums.
            double minX = System.Math.Min(this.Min[0], box.Min[0]);
            double minY = System.Math.Min(this.Min[1], box.Min[1]);
            double maxX = System.Math.Max(this.Max[0], box.Max[0]);
            double maxY = System.Math.Max(this.Max[1], box.Max[1]);

            return new GeoCoordinateBox(new GeoCoordinate(minY, minX), new GeoCoordinate(maxY, maxX));
        }


        /// <summary>
        /// Resizes this bounding box with the given delta.
        /// </summary>
        /// <param name="delta"></param>
        /// <returns></returns>
        public GeoCoordinateBox Resize(double delta)
        {
            return new GeoCoordinateBox(
                new GeoCoordinate(this.MaxLat + delta, this.MaxLon + delta),
                new GeoCoordinate(this.MinLat - delta, this.MinLon - delta));
        }
    }
}