﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Store;
using VDS.RDF.Query;
using VDS.RDF.Query.FullText;
using VDS.RDF.Query.FullText.Indexing;
using VDS.RDF.Query.FullText.Indexing.Lucene;
using VDS.RDF.Query.FullText.Schema;
using VDS.RDF.Query.FullText.Search;
using VDS.RDF.Query.FullText.Search.Lucene;
using VDS.RDF.Query.Optimisation;
using LucVersion = Lucene.Net.Util.Version;

namespace VDS.RDF.Configuration
{
    public class FullTextObjectFactory
        : IObjectFactory
    {
        private const String LuceneSubjectsIndexer = "VDS.RDF.Query.FullText.Indexing.Lucene.LuceneSubjectsIndexer",
                             LuceneObjectsIndexer = "VDS.RDF.Query.FullText.Indexing.Lucene.LuceneObjectsIndexer",
                             LucenePredicatesIndexer = "VDS.RDF.Query.FullText.Indexing.Lucene.LucenePredicatesIndexer",
                             DefaultIndexSchema = "VDS.RDF.Query.FullText.Schema.DefaultIndexSchema",
                             LuceneSearchProvider = "VDS.RDF.Query.FullText.Search.Lucene.LuceneSearchProvider",
                             FullTextOptimiser = "VDS.RDF.Query.Optimisation.FullTextOptimiser";

        private readonly Type _luceneAnalyzerType = typeof(Analyzer);
        private readonly Type _luceneDirectoryType = typeof(Directory);


        public bool TryLoadObject(IGraph g, INode objNode, Type targetType, out object obj)
        {
            obj = null;

            INode index = g.CreateUriNode(new Uri(FullTextHelper.FullTextConfigurationNamespace + "index"));
            //INode indexer = g.CreateUriNode(new Uri(FullTextHelper.FullTextConfigurationNamespace + "indexer"));
            INode searcher = g.CreateUriNode(new Uri(FullTextHelper.FullTextConfigurationNamespace + "searcher"));
            INode analyzer = g.CreateUriNode(new Uri(FullTextHelper.FullTextConfigurationNamespace + "analyzer"));
            INode schema = g.CreateUriNode(new Uri(FullTextHelper.FullTextConfigurationNamespace + "schema"));
            INode version = g.CreateUriNode(new Uri(FullTextHelper.FullTextConfigurationNamespace + "version"));

            Object tempIndex, tempAnalyzer, tempSchema;
            int ver = 2900;

            switch (targetType.FullName)
            {
                case DefaultIndexSchema:
                    obj = new DefaultIndexSchema();
                    break;

                case FullTextOptimiser:
                    //Need to get the Search Provider
                    INode providerNode = ConfigurationLoader.GetConfigurationNode(g, objNode, searcher);
                    if (providerNode == null) throw new DotNetRdfConfigurationException("Unable to load the Full Text Optimiser specified by the Node '" + objNode.ToString() + "' as there was no value specified for the required dnr-ft:searcher property");
                    Object tempSearcher = ConfigurationLoader.LoadObject(g, providerNode);
                    if (tempSearcher is IFullTextSearchProvider)
                    {
                        obj = new FullTextOptimiser((IFullTextSearchProvider)tempSearcher);
                    }
                    else
                    {
                        throw new DotNetRdfConfigurationException("Unable to load the Full Text Optimiser specified by the Node '" + objNode.ToString() + "' as the value specified for the dnr-ft:searcher property pointed to an object which could not be loaded as a type that implements the required IFullTextSearchProvider interface");
                    }
                    break;

                case LuceneObjectsIndexer:
                case LucenePredicatesIndexer:
                case LuceneSubjectsIndexer:
                case LuceneSearchProvider:
                    //For any Lucene Indexer/Search Provider need to know the Index, Analyzer and Schema to be used

                    //First of all check the version
                    ver = ConfigurationLoader.GetConfigurationInt32(g, objNode, version, 2900);

                    //Then get the Index
                    tempIndex = ConfigurationLoader.GetConfigurationNode(g, objNode, index);
                    if (tempIndex == null) throw new DotNetRdfConfigurationException("Unable to load the Lucene Indexer specified by the Node '" + objNode.ToString() + "' as there was no value specified for the required dnr-ft:index property");
                    tempIndex = ConfigurationLoader.LoadObject(g, (INode)tempIndex);
                    if (tempIndex is Directory)
                    {
                        //Next get the Analyzer (assume Standard if none specified)
                        tempAnalyzer = ConfigurationLoader.GetConfigurationNode(g, objNode, analyzer);
                        if (tempAnalyzer == null)
                        {
                            tempAnalyzer = new StandardAnalyzer(this.GetLuceneVersion(ver));
                        } 
                        else 
                        {
                            tempAnalyzer = ConfigurationLoader.LoadObject(g, (INode)tempAnalyzer);
                        }

                        if (tempAnalyzer is Analyzer)
                        {
                            //Finally get the Schema (assume Default if none specified)
                            tempSchema = ConfigurationLoader.GetConfigurationNode(g, objNode, schema);
                            if (tempSchema == null)
                            {
                                tempSchema = new DefaultIndexSchema();
                            }
                            else
                            {
                                tempSchema = ConfigurationLoader.LoadObject(g, (INode)tempSchema);
                            }

                            if (tempSchema is IFullTextIndexSchema)
                            {
                                //Now we can create the Object
                                switch (targetType.FullName)
                                {
                                    case LuceneObjectsIndexer:
                                        obj = new LuceneObjectsIndexer((Directory)tempIndex, (Analyzer)tempAnalyzer, (IFullTextIndexSchema)tempSchema);
                                        break;
                                    case LucenePredicatesIndexer:
                                        obj = new LucenePredicatesIndexer((Directory)tempIndex, (Analyzer)tempAnalyzer, (IFullTextIndexSchema)tempSchema);
                                        break;
                                    case LuceneSubjectsIndexer:
                                        obj = new LuceneSubjectsIndexer((Directory)tempIndex, (Analyzer)tempAnalyzer, (IFullTextIndexSchema)tempSchema);
                                        break;
                                    case LuceneSearchProvider:
                                        obj = new LuceneSearchProvider(this.GetLuceneVersion(ver), (Directory)tempIndex, (Analyzer)tempAnalyzer, (IFullTextIndexSchema)tempSchema);
                                        break;
                                }
                            }
                            else
                            {
                                throw new DotNetRdfConfigurationException("Unable to load the Lucene Indexer specified by the Node '" + objNode.ToString() + "' as the value given for the dnr-ft:schema property pointed to an Object which could not be loaded as a type that implements the required IFullTextIndexSchema interface");
                            }
                        }
                        else
                        {
                            throw new DotNetRdfConfigurationException("Unable to load the Lucene Indexer specified by the Node '" + objNode.ToString() + "' as the value given for the dnr-ft:analyzer property pointed to an Object which could not be loaded as a type that derives from the required Lucene.Net.Analysis.Analyzer type");
                        }
                    }
                    else
                    {
                        throw new DotNetRdfConfigurationException("Unable to load the Lucene Indexer specified by the Node '" + objNode.ToString() + "' as the value given for the dnr-ft:index property pointed to an Object which could not be loaded as a type that derives from the required Lucene.Net.Store.Directory type");
                    }
                    break;

                default:
                    if (this._luceneAnalyzerType.IsAssignableFrom(targetType))
                    {
                        if (targetType.GetConstructor(new Type[] { typeof(LucVersion) }) != null)
                        {
                            obj = Activator.CreateInstance(targetType, new Object[] { this.GetLuceneVersion(ver) });
                        }
                        else
                        {
                            obj = Activator.CreateInstance(targetType);
                        }
                    }
                    else if (this._luceneDirectoryType.IsAssignableFrom(targetType))
                    {
                        String dir = ConfigurationLoader.GetConfigurationString(g, objNode, ConfigurationLoader.CreateConfigurationNode(g, ConfigurationLoader.PropertyFromFile));
                        if (dir != null)
                        {
                            obj = Activator.CreateInstance(targetType, new Object[] { dir });
                        }
                        else
                        {
                            obj = Activator.CreateInstance(targetType);
                        }
                    }
                    break;
            }

            return (obj != null);
        }

        public bool CanLoadObject(Type t)
        {
            switch (t.FullName)
            {
                case DefaultIndexSchema:
                case FullTextOptimiser:
                case LuceneObjectsIndexer:
                case LucenePredicatesIndexer:
                case LuceneSearchProvider:
                case LuceneSubjectsIndexer:
                    return true;

                default:
                    if (this._luceneAnalyzerType.IsAssignableFrom(t)) return true;
                    if (this._luceneDirectoryType.IsAssignableFrom(t)) return true;
                    return false;
            }
        }

        private LucVersion GetLuceneVersion(int ver)
        {
            switch (ver)
            {
                case 2000:
                    return LucVersion.LUCENE_20;
                case 2100:
                    return LucVersion.LUCENE_21;
                case 2200:
                    return LucVersion.LUCENE_22;
                case 2300:
                    return LucVersion.LUCENE_23;
                case 2400:
                    return LucVersion.LUCENE_24;
                case 2900:
                    return LucVersion.LUCENE_29;
                default:
                    return LucVersion.LUCENE_29;
            }
        }
    }
}