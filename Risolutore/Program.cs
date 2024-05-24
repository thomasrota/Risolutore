using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using System.Threading;
using Newtonsoft.Json;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.DevTools;

namespace RisoluDestinazionere
{
	public class Node
	{
		public int Id { get; set; }
		public string Label { get; set; }
		public string Color { get; set; }
	}

	public class Edge
	{
		public int Sorgente { get; set; }
		public int Destinazione { get; set; }
		public string Peso { get; set; }
	}
	public class GraphData
	{
		public List<Node> Nodes { get; set; }
		public List<Edge> Edges { get; set; }

		public GraphData(List<Node> nodes, List<Edge> edges)
		{
			Nodes = nodes;
			Edges = edges;
		}
	}
	class Program
    {
        static void Main(string[] args)
        {
			bool continua = true;
			string link, codiceGrafo;
			bool LSP = false;
            do {
	            Console.Write("Tipologia esercizio da risolvere\n1 - Link State Protocol\n2 - Distance Vector\n3 - Bellman-Ford\n0 - Esci\n\nScelta: ");
	            switch (Console.ReadLine())
	            {
	                case "1":
						LSP = true;
		                link = "https://www.embedware.it/sistemi/grafi/LSP/";
		                codiceGrafo = null;
		                string jsLSP = OpenLink(link, codiceGrafo, LSP);
		                //
		                GraphData graphDataLSP = DeserializeGraphData(jsLSP);
						//
						// Output the deserialized data
						Console.WriteLine("Nodes:");
						foreach (var node in graphDataLSP.Nodes)
						{
							Console.WriteLine($"ID: {node.Id}, Label: {node.Label}, Color: {node.Color}");
						}

						Console.WriteLine("\nEdges:");
						foreach (var edge in graphDataLSP.Edges)
						{
							Console.WriteLine($"From: {edge.Sorgente}, To: {edge.Destinazione}, Label: {edge.Peso}");
						}
						Console.ReadKey();
						break;
	                case "2":
		                link = "https://www.embedware.it/sistemi/grafi/DV/";
						Console.Write("Inserisci codice grafo: ");
						codiceGrafo = Console.ReadLine();
						string jsDV = OpenLink(link, codiceGrafo, LSP);
						//
		                GraphData graphDataDV = DeserializeGraphData(jsDV);
						//
						Console.ReadKey();
						break;
	                case "3":
		                link = "https://www.embedware.it/sistemi/grafi/bellman/";
		                Console.Write("Inserisci codice grafo: ");
		                codiceGrafo = Console.ReadLine();
		                string jsBF = OpenLink(link, codiceGrafo, LSP);
		                //
		                GraphData graphDataBF = DeserializeGraphData(jsBF);
						//
		                Console.ReadKey();
						break;
	                case "0":
		                continua = false;
						break;
	                default:
	                    Console.WriteLine("Scelta non valida");
						break;
	            }
				Console.Clear();
            }
            while (continua);
        }

        static string OpenLink(string l, string cG, bool isLSP)
        {
			//var driver = new OpenQA.Selenium.Chrome.ChromeDriver();
			var driver = new OpenQA.Selenium.Edge.EdgeDriver();
			IWebElement scriptElement;
			driver.Navigate().GoToUrl(l);
			if (!isLSP)
			{
				// inserire codice grafo nella textbox
				driver.FindElement(By.Name("codice")).SendKeys(cG);
				// cliccare sul pulsante "Genera"
				driver.FindElement(By.CssSelector("input[type='submit'][value='genera']")).Click();
			}
			// attesa caricamento pagina
			WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
			wait.Until(driver => driver.FindElements(By.TagName("script")).Count > 1);
			// recuperare il contenuto del javascript della pagina
			if(!isLSP)
				scriptElement = driver.FindElements(By.TagName("script"))[2];
			else
				scriptElement = driver.FindElements(By.TagName("script"))[1];
			string scriptText = scriptElement.GetAttribute("innerHTML");
			return scriptText;
        }

        static GraphData DeserializeGraphData(string script)
        {
	        var nodesMatch = Regex.Match(script, @"var\s+nodes\d*\s*=\s*(\[.*?\])", RegexOptions.Singleline);
	        var edgesMatch = Regex.Match(script, @"var\s+edges\d*\s*=\s*(\[.*?\])", RegexOptions.Singleline);

	        if (nodesMatch.Success && edgesMatch.Success)
	        {
		        string nodesJson = nodesMatch.Groups[1].Value;
		        string edgesJson = edgesMatch.Groups[1].Value;

		        List<Node> nodes = JsonConvert.DeserializeObject<List<Node>>(nodesJson, new JsonSerializerSettings
		        {
			        // Ignore missing properties during deserialization
			        MissingMemberHandling = MissingMemberHandling.Ignore
		        });
		        List<Edge> edges = JsonConvert.DeserializeObject<List<Edge>>(edgesJson);

		        return new GraphData(nodes, edges);
	        }
	        else
	        {
		        throw new Exception("Failed Destinazione deserialize graph data.");
	        }
        }
	}
}