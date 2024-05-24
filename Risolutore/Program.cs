using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using System.Threading;
using Newtonsoft.Json;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Edge;

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
		public int From { get; set; }
		public int To { get; set; }
		public string Label { get; set; }
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
			do
			{
				Console.Write("Tipologia esercizio da risolvere\n1 - Link State Protocol\n2 - Distance Vector\n3 - Bellman-Ford\n0 - Esci\n\nScelta: ");
				switch (Console.ReadLine())
				{
					case "1":
						LSP = true;
						link = "https://www.embedware.it/sistemi/grafi/LSP/";
						codiceGrafo = null;
						string jsLSP = OpenLink(link, codiceGrafo, LSP);
						GraphData graphDataLSP = DeserializeGraphData(jsLSP);
						OutputGraphData(graphDataLSP);
						break;
					case "2":
						link = "https://www.embedware.it/sistemi/grafi/DV/";
						Console.Write("Inserisci codice grafo: ");
						codiceGrafo = Console.ReadLine();
						string jsDV = OpenLink(link, codiceGrafo, LSP);
						GraphData graphDataDV = DeserializeGraphData(jsDV);
						OutputGraphData(graphDataDV);
						break;
					case "3":
						link = "https://www.embedware.it/sistemi/grafi/bellman/";
						Console.Write("Inserisci codice grafo: ");
						codiceGrafo = Console.ReadLine();
						string jsBF = OpenLink(link, codiceGrafo, LSP);
						GraphData graphDataBF = DeserializeGraphData(jsBF);
						OutputGraphData(graphDataBF);
						RunBellmanFord(graphDataBF);
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
			scriptElement = isLSP ? driver.FindElements(By.TagName("script"))[1] : driver.FindElements(By.TagName("script"))[2];
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
					MissingMemberHandling = MissingMemberHandling.Ignore
				});
				List<Edge> edges = JsonConvert.DeserializeObject<List<Edge>>(edgesJson);

				return new GraphData(nodes, edges);
			}
			else
			{
				throw new Exception("Failed to deserialize graph data.");
			}
		}

		static void OutputGraphData(GraphData graphData)
		{
			Console.WriteLine("Nodes:");
			foreach (var node in graphData.Nodes)
			{
				Console.WriteLine($"Label: {node.Label}, ID: {node.Id}, Color: {node.Color}");
			}

			Console.WriteLine("\nEdges:");
			foreach (var edge in graphData.Edges)
			{
				var fromNode = graphData.Nodes.First(n => n.Id == edge.From);
				var toNode = graphData.Nodes.First(n => n.Id == edge.To);
				Console.WriteLine($"From: {fromNode.Label}, To: {toNode.Label}, Label: {edge.Label}");
			}
			Console.ReadKey();
		}

		static void RunBellmanFord(GraphData graphData)
		{
			Node startNode = graphData.Nodes.FirstOrDefault(n => n.Color.Equals("lime", StringComparison.OrdinalIgnoreCase));

			if (startNode == null)
			{
				Console.WriteLine("No node with color 'lime' found. Cannot run Bellman-Ford algorithm.");
				return;
			}

			int startNodeId = startNode.Id;
			int numNodes = graphData.Nodes.Count;
			Dictionary<int, int> distances = new Dictionary<int, int>();
			Dictionary<int, int> predecessors = new Dictionary<int, int>();

			foreach (var node in graphData.Nodes)
			{
				distances[node.Id] = int.MaxValue;
				predecessors[node.Id] = -1;
			}
			distances[startNodeId] = 0;

			for (int i = 0; i < numNodes - 1; i++)
			{
				foreach (var edge in graphData.Edges)
				{
					int u = edge.From;
					int v = edge.To;
					int weight;

					if (string.IsNullOrEmpty(edge.Label) || !int.TryParse(edge.Label, out weight))
					{
						Console.WriteLine($"Invalid weight for edge from {u} to {v}. Skipping this edge.");
						continue;
					}

					if (distances[u] != int.MaxValue && distances[u] + weight < distances[v])
					{
						distances[v] = distances[u] + weight;
						predecessors[v] = u;
					}
				}
			}

			foreach (var edge in graphData.Edges)
			{
				int u = edge.From;
				int v = edge.To;
				int weight;

				if (string.IsNullOrEmpty(edge.Label) || !int.TryParse(edge.Label, out weight))
				{
					continue; // Skip invalid weight edges
				}

				if (distances[u] != int.MaxValue && distances[u] + weight < distances[v])
				{
					Console.WriteLine("Il grafo contiene un ciclo di peso negativo.");
					return;
				}
			}

			Console.WriteLine("\nDistanze dal nodo di partenza:");
			foreach (var node in graphData.Nodes)
			{
				Console.WriteLine($"Nodo {node.Label}: {distances[node.Id]}");
			}

			Console.WriteLine("\nPredecessori:");
			foreach (var node in graphData.Nodes)
			{
				var predecessorLabel = predecessors[node.Id] == -1 ? "N/A" : graphData.Nodes.First(n => n.Id == predecessors[node.Id]).Label;
				Console.WriteLine($"Nodo {node.Label}: {predecessorLabel}");
			}

			Console.ReadKey();
		}
	}
}
