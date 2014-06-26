﻿using NUnit.Framework;
using OpenLR.Binary;
using OpenLR.Binary.Decoders;
using OpenLR.Locations;
using OpenLR.Model;
using OpenLR.OsmSharp.Decoding;
using OpenLR.OsmSharp.Osm;
using OsmSharp.Collections.Tags;
using OsmSharp.Collections.Tags.Index;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router.Dykstra;
using OsmSharp.Routing.Osm.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLR.Tests.Referenced.Osm
{
    /// <summary>
    /// Contains tests for decoding/encoding a OpenLR line location to a referenced line location.
    /// </summary>
    [TestFixture]
    public class ReferencedPointAlongLineDecoderTests
    {
        /// <summary>
        /// A simple referenced point along line location decoding test.
        /// </summary>
        [Test]
        public void DecodeReferencedPointAlongLineLocation()
        {
            double delta = 0.0001;

            // build the location to decode.
            var location = new PointAlongLineLocation();
            location.First = new LocationReferencePoint();
            location.First.Coordinate = new Coordinate() { Latitude = 49.60597, Longitude = 6.12829 };
            location.First.DistanceToNext = 92;
            location.First.FuntionalRoadClass = FunctionalRoadClass.Frc2;
            location.First.FormOfWay = FormOfWay.MultipleCarriageWay;
            location.First.LowestFunctionalRoadClassToNext = FunctionalRoadClass.Frc2;
            location.Last = new LocationReferencePoint();
            location.Last.Coordinate = new Coordinate() { Latitude = 49.60521, Longitude = 6.12779 };
            location.Last.DistanceToNext = 10;
            location.Last.FuntionalRoadClass = FunctionalRoadClass.Frc2;
            location.Last.FormOfWay = FormOfWay.MultipleCarriageWay;
            location.PositiveOffset = 28;
            location.Orientation = Orientation.NoOrientation;
            location.SideOfRoad = SideOfRoad.Left;

            // build a graph to decode onto.
            var tags = new TagsTableCollectionIndex();
            var graph = new DynamicGraphRouterDataSource<LiveEdge>(tags);
            uint vertex1 = graph.AddVertex(49.60597f, 6.12829f);
            uint vertex2 = graph.AddVertex(49.60521f, 6.12779f);
            graph.AddArc(vertex1, vertex2, new LiveEdge()
            {
                Coordinates = null,
                Distance = 10,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            }, null);
            graph.AddArc(vertex2, vertex1, new LiveEdge()
            {
                Coordinates = null,
                Distance = 10,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            }, null);

            // decode the location
            var decoder = new PointAlongLineDecoder();
            var router = new DykstraRoutingLive();
            var mainDecoder = new ReferencedOsmDecoder(graph, new BinaryDecoder());
            var referencedDecoder = new ReferencedPointAlongLineDecoder<LiveEdge>(mainDecoder, decoder, graph, router);
            var referencedLocation = referencedDecoder.Decode(location);

            // confirm result.
            Assert.IsNotNull(referencedLocation);
            Assert.IsNotNull(referencedLocation.Edge);
            Assert.AreEqual(vertex1, referencedLocation.VertexFrom);
            Assert.AreEqual(vertex2, referencedLocation.VertexTo);
            var longitudeReference = (location.First.Coordinate.Longitude - location.Last.Coordinate.Longitude) * ((double)location.PositiveOffset / (double)location.First.DistanceToNext) + location.Last.Coordinate.Longitude;
            var latitudeReference = (location.First.Coordinate.Latitude - location.Last.Coordinate.Latitude) * ((double)location.PositiveOffset / (double)location.First.DistanceToNext) + location.Last.Coordinate.Latitude;
            Assert.AreEqual(longitudeReference, referencedLocation.Longitude, delta);
            Assert.AreEqual(latitudeReference, referencedLocation.Latitude, delta);
        }
    }
}