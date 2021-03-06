﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using QuickGraph;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;

namespace examples.Support
{
    internal static class Visualizer
    {
        public static void Visualize<TVertex, TEdge>(this IVertexAndEdgeListGraph<TVertex, TEdge> graph,
            string fileName, string dir = @"c:\temp\")
            where TEdge : IEdge<TVertex>
        {
            Visualize(graph, fileName, NoOpEdgeFormatter, dir);
        }        
        
        public static void Visualize<TVertex, TEdge>(this IVertexAndEdgeListGraph<TVertex, TEdge> graph,
            string fileName, FormatEdgeAction<TVertex, TEdge> edgeFormatter, string dir /*@"c:\temp\"*/)
            where TEdge : IEdge<TVertex>
        {
            var fullFileName = Path.Combine(dir, fileName);
            var viz = new GraphvizAlgorithm<TVertex, TEdge>(graph);

            viz.FormatVertex += VizFormatVertex;

            viz.FormatEdge += edgeFormatter;

            viz.Generate(new FileDotEngine(), fullFileName);
        }
        
        static void NoOpEdgeFormatter<TVertex, TEdge>(object sender, FormatEdgeEventArgs<TVertex, TEdge> e)
            where TEdge : IEdge<TVertex>
        {
            // noop
        }
        
        public static string ToDotNotation<TVertex, TEdge>(this IVertexAndEdgeListGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            var viz = new GraphvizAlgorithm<TVertex, TEdge>(graph);
            viz.FormatVertex += VizFormatVertex;
            return viz.Generate(new DotPrinter(), "");
        }

        static void VizFormatVertex<TVertex>(object sender, FormatVertexEventArgs<TVertex> e)
        {
            e.VertexFormatter.Label = e.Vertex.ToString();
        }
    }

    public sealed class FileDotEngine : IDotEngine
    {
        public string Run(GraphvizImageType imageType, string dot, string outputFileName)
        {
            string output = outputFileName;
            File.WriteAllText(output, dot);

            // assumes dot.exe is on the path:
            //var args = string.Format(@"{0} -Tjpg -O", output);
            //output = "Colors";
            var args = string.Format(@"{0} -Tpng -O", output);
            string path = Directory.GetCurrentDirectory();
            System.Diagnostics.Process.Start("C:\\Program Files (x86)\\Graphviz2.38\\bin\\dot.exe", args);
            System.Diagnostics.Process.Start("C:\\Program Files (x86)\\Graphviz2.38\\bin\\dot.exe", path+"\\color -Tpng -O"); //c:\\temp
            return output;
            //"C:\\Users\\1\\Dropbox\\Navicon\\Common\\NeuronEnsemble\\NeuronEnsemble\\bin\\Debug"
        }
    }
        
    public sealed class DotPrinter : IDotEngine
    {
        public string Run(GraphvizImageType imageType, string dot, string outputFileName)
        {
            return dot;
        }
    }
}
