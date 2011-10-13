﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VDS.RDF.Query;
using VDS.RDF.Query.Inference;
using VDS.RDF.Query.Inference.Pellet;
using VDS.RDF.Query.Inference.Pellet.Services;

namespace VDS.RDF.Test
{
    [TestClass()]
    public class PelletTests
    {
        public const String PelletTestServer = "http://ps.clarkparsia.com/";

        [TestMethod()]
        public void PelletKBListTest()
        {
            try
            {
                PelletServer server = new PelletServer(PelletTestServer);

                foreach (KnowledgeBase kb in server.KnowledgeBases)
                {
                    Console.WriteLine(kb.Name);
                    Console.WriteLine("Services:");
                    foreach (PelletService svc in kb.Services)
                    {
                        Console.WriteLine("  " + svc.Name);
                        Console.WriteLine("    Uri: " + svc.Endpoint.Uri.ToString());
                        Console.WriteLine("    HTTP Methods: " + String.Join(",", svc.Endpoint.HttpMethods.ToArray()));
                        Console.WriteLine("    Response MIME Types: " + String.Join(",", svc.MimeTypes.ToArray()));
                    }
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                TestTools.ReportError("Error", ex, true);
            }
        }

        [TestMethod()]
        public void PelletQueryTest()
        {
            try
            {
                PelletServer server = new PelletServer(PelletTestServer);
                Type svcType = typeof(QueryService);

                foreach (KnowledgeBase kb in server.KnowledgeBases)
                {
                    if (kb.SupportsService(svcType)) 
                    {
                        Console.WriteLine(kb.Name + " supports SPARQL Query");
                        QueryService q = (QueryService)kb.GetService(svcType);
                        Object results = q.Query("SELECT * WHERE {?s a ?type} LIMIT 10");
                        if (results is SparqlResultSet)
                        {
                            TestTools.ShowResults(results);
                        }
                        else
                        {
                            Console.WriteLine("Unexpected Result type from Query Service");
                        }
                    } 
                    else 
                    {
                        Console.WriteLine(kb.Name + " does not support the Query Service");
                    }
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                TestTools.ReportError("Error", ex, true);
            }
        }

        [TestMethod()]
        public void PelletRealizeTest()
        {
            try
            {
                PelletServer server = new PelletServer(PelletTestServer);
                Type svcType = typeof(RealizeService);

                foreach (KnowledgeBase kb in server.KnowledgeBases)
                {
                    if (kb.SupportsService(svcType))
                    {
                        Console.WriteLine(kb.Name + " supports Realize");
                        RealizeService svc = (RealizeService)kb.GetService(svcType);
                        IGraph g = svc.Realize();
                        TestTools.ShowGraph(g);
                    }
                    else
                    {
                        Console.WriteLine(kb.Name + " does not support the Realize Service");
                    }
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                TestTools.ReportError("Error", ex, true);
            }
        }

        [TestMethod()]
        public void PelletConsistencyTest()
        {
            try
            {
                PelletServer server = new PelletServer(PelletTestServer);
                Type svcType = typeof(ConsistencyService);

                foreach (KnowledgeBase kb in server.KnowledgeBases)
                {
                    if (kb.SupportsService(svcType))
                    {
                        Console.WriteLine(kb.Name + " supports Consistency");
                        ConsistencyService svc = (ConsistencyService)kb.GetService(svcType);
                        Console.WriteLine("Consistency: " + svc.IsConsistent().ToString());
                    }
                    else
                    {
                        Console.WriteLine(kb.Name + " does not support the Consistency Service");
                    }
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                TestTools.ReportError("Error", ex, true);
            }
        }

        [TestMethod()]
        public void PelletSearchTest()
        {
            try
            {
                PelletServer server = new PelletServer(PelletTestServer);
                Type svcType = typeof(SearchService);

                foreach (KnowledgeBase kb in server.KnowledgeBases)
                {
                    if (kb.SupportsService(svcType))
                    {
                        Console.WriteLine(kb.Name + " supports Search");
                        SearchService svc = (SearchService)kb.GetService(svcType);
                        foreach (SearchServiceResult result in svc.Search("cabernet"))
                        {
                            Console.WriteLine(result.ToString());
                        }
                    }
                    else
                    {
                        Console.WriteLine(kb.Name + " does not support the Search Service");
                    }
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                TestTools.ReportError("Error", ex, true);
            }
        }

        [TestMethod()]
        public void PelletNamespaceTest()
        {
            try
            {
                PelletServer server = new PelletServer(PelletTestServer);
                Type svcType = typeof(NamespaceService);

                foreach (KnowledgeBase kb in server.KnowledgeBases)
                {
                    if (kb.SupportsService(svcType))
                    {
                        Console.WriteLine(kb.Name + " supports Namespaces");
                        NamespaceService svc = (NamespaceService)kb.GetService(svcType);
                        NamespaceMapper nsmap = svc.GetNamespaces();
                        foreach (String prefix in nsmap.Prefixes)
                        {
                            Console.WriteLine(prefix + ": " + nsmap.GetNamespaceUri(prefix).ToString());
                        }
                    }
                    else
                    {
                        Console.WriteLine(kb.Name + " does not support the Search Service");
                    }
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                TestTools.ReportError("Error", ex, true);
            }
        }
    }
}
