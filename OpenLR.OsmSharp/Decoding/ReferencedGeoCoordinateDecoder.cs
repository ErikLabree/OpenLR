﻿using OpenLR.Locations;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLR.OsmSharp.Decoding
{
    /// <summary>
    /// Represents a referenced geo coordinate location decoder.
    /// </summary>
    /// <typeparam name="TEdge"></typeparam>
    public class ReferencedGeoCoordinateDecoder<TEdge> : ReferencedDecoder<ReferencedGeoCoordinate, GeoCoordinateLocation, TEdge>
        where TEdge : IDynamicGraphEdgeData
    {
        /// <summary>
        /// Creates a geo coordinate location referenced decoder.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="router"></param>
        public ReferencedGeoCoordinateDecoder(OpenLR.Decoding.Decoder rawDecoder, DynamicGraphRouterDataSource<TEdge> graph, IBasicRouter<TEdge> router)
            : base(rawDecoder, graph, router)
        {

        }

        /// <summary>
        /// Decodes an OpenLR-encoded unreferenced raw OpenLR location into a referenced Location.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public override ReferencedGeoCoordinate Decode(GeoCoordinateLocation location)
        {
            return new ReferencedGeoCoordinate()
            {
                Latitude = location.Coordinate.Latitude,
                Longitude = location.Coordinate.Longitude
            };
        }
    }
}