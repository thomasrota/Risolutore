using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using System.Threading;
using Newtonsoft.Json;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Edge;

namespace Risolutore
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
						//OutputGraphData(graphDataLSP);
						LinkStateProtocol(graphDataLSP);
						break;
					case "2":
						link = "https://www.embedware.it/sistemi/grafi/DV/";
						Console.Write("Inserisci codice grafo: ");
						codiceGrafo = Console.ReadLine();
						string jsDV = OpenLink(link, codiceGrafo, LSP);
						GraphData graphDataDV = DeserializeGraphData(jsDV);
						//OutputGraphData(graphDataDV);
						DistanceVector(graphDataDV);
						break;
					case "3":
						link = "https://www.embedware.it/sistemi/grafi/bellman/";
						Console.Write("Inserisci codice grafo: ");
						codiceGrafo = Console.ReadLine();
						string jsBF = OpenLink(link, codiceGrafo, LSP);
						GraphData graphDataBF = DeserializeGraphData(jsBF);
						//OutputGraphData(graphDataBF);
						BellmanFord(graphDataBF);
						break;
					case "0":
						Environment.Exit(0);
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
		static void LinkStateProtocol(GraphData graphData)
		{
			var routers = graphData.Nodes.ToDictionary(n => n.Id, n => new Dictionary<int, int>());
			var lsps = new Dictionary<int, Dictionary<int, int>>();

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

				if (!routers[u].ContainsKey(v))
				{
					routers[u][v] = weight;
				}

				if (!routers[v].ContainsKey(u))
				{
					routers[v][u] = weight;
				}
			}

			foreach (var router in routers)
			{
				Console.WriteLine(
					$"Link State Packet for Router {graphData.Nodes.First(n => n.Id == router.Key).Label}:");
				foreach (var link in router.Value)
				{
					Console.WriteLine(
						$"  To {graphData.Nodes.First(n => n.Id == link.Key).Label}: Cost = {link.Value}");
				}

				Console.WriteLine();
			}

			foreach (var router in routers)
			{
				lsps[router.Key] = new Dictionary<int, int>(router.Value);
			}

			var distanceTable = new int[graphData.Nodes.Count, graphData.Nodes.Count];
			for (int i = 0; i < graphData.Nodes.Count; i++)
			{
				for (int j = 0; j < graphData.Nodes.Count; j++)
				{
					if (i == j)
					{
						distanceTable[i, j] = 0;
					}
					else if (lsps.ContainsKey(i) && lsps[i].ContainsKey(j))
					{
						distanceTable[i, j] = lsps[i][j];
					}
					else
					{
						distanceTable[i, j] = int.MaxValue;
					}
				}
			}

			for (int k = 0; k < graphData.Nodes.Count; k++)
			{
				for (int i = 0; i < graphData.Nodes.Count; i++)
				{
					for (int j = 0; j < graphData.Nodes.Count; j++)
					{
						if (distanceTable[i, k] != int.MaxValue && distanceTable[k, j] != int.MaxValue &&
							distanceTable[i, k] + distanceTable[k, j] < distanceTable[i, j])
						{
							distanceTable[i, j] = distanceTable[i, k] + distanceTable[k, j];
						}
					}
				}
			}

			var nodeLabels = graphData.Nodes.Select(n => n.Label).ToArray();
			Console.Write("\t");
			foreach (var label in nodeLabels)
			{
				Console.Write(label + "\t");
			}

			Console.WriteLine();

			for (int i = 0; i < graphData.Nodes.Count; i++)
			{
				Console.Write(nodeLabels[i] + "\t");
				for (int j = 0; j < graphData.Nodes.Count; j++)
				{
					if (distanceTable[i, j] == int.MaxValue)
					{
						Console.Write("\t");
					}
					else
					{
						Console.Write(distanceTable[i, j] + "\t");
					}
				}

				Console.WriteLine();
			}

			Console.ReadKey();
		}

		static void DistanceVector(GraphData graphData)
		{
			Dictionary<int, Dictionary<int, int>> distances = new Dictionary<int, Dictionary<int, int>>();
			Dictionary<int, Dictionary<int, int>> nextHops = new Dictionary<int, Dictionary<int, int>>();

			foreach (var node in graphData.Nodes)
			{
				distances[node.Id] = new Dictionary<int, int>();
				nextHops[node.Id] = new Dictionary<int, int>();

				foreach (var targetNode in graphData.Nodes)
				{
					if (node.Id == targetNode.Id)
					{
						distances[node.Id][targetNode.Id] = 0;
						nextHops[node.Id][targetNode.Id] = node.Id;
					}
					else
					{
						distances[node.Id][targetNode.Id] = int.MaxValue;
						nextHops[node.Id][targetNode.Id] = -1;
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
					Console.WriteLine($"Invalid weight for edge from {u} to {v}. Skipping this edge.");
					continue;
				}

				distances[u][v] = weight;
				nextHops[u][v] = v;

				// Assuming the graph is undirected
				distances[v][u] = weight;
				nextHops[v][u] = u;
			}

			bool changed;
			do
			{
				changed = false;
				foreach (var node in graphData.Nodes)
				{
					foreach (var edge in graphData.Edges)
					{
						int u = edge.From;
						int v = edge.To;
						int weight;

						if (string.IsNullOrEmpty(edge.Label) || !int.TryParse(edge.Label, out weight))
						{
							continue;
						}

						foreach (var targetNode in graphData.Nodes)
						{
							if (distances[v][targetNode.Id] != int.MaxValue &&
								distances[u][targetNode.Id] > distances[v][targetNode.Id] + weight)
							{
								distances[u][targetNode.Id] = distances[v][targetNode.Id] + weight;
								nextHops[u][targetNode.Id] = v;
								changed = true;
							}

							if (distances[u][targetNode.Id] != int.MaxValue &&
								distances[v][targetNode.Id] > distances[u][targetNode.Id] + weight)
							{
								distances[v][targetNode.Id] = distances[u][targetNode.Id] + weight;
								nextHops[v][targetNode.Id] = u;
								changed = true;
							}
						}
					}
				}
			} while (changed);

			foreach (var node in graphData.Nodes)
			{
				Console.WriteLine($"Routing table di {node.Label}:");
				foreach (var targetNode in graphData.Nodes)
				{
					if (node.Id == targetNode.Id)
					{
						Console.WriteLine($"To {targetNode.Label}: Local");
					}
					else
					{
						string costLabel = distances[node.Id][targetNode.Id] == int.MaxValue ? "∞" : distances[node.Id][targetNode.Id].ToString();
						string nextHopLabel = nextHops[node.Id][targetNode.Id] == -1 ? "N/A" : graphData.Nodes.First(n => n.Id == nextHops[node.Id][targetNode.Id]).Label;
						Console.WriteLine($"To {targetNode.Label}: Costo = {costLabel}, Next hop = {nextHopLabel}");
					}
				}
				Console.WriteLine();
			}

			Console.ReadKey();
		}


		static void BellmanFord(GraphData graphData)
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
				string distanceLabel = distances[node.Id] == int.MaxValue ? "∞" : distances[node.Id].ToString();
				Console.WriteLine($"Nodo {node.Label}: {distanceLabel}");
			}

			Console.WriteLine("\nPredecessori:");
			foreach (var node in graphData.Nodes)
			{
				var predecessorLabel = predecessors[node.Id] == -1 ? "N/A" : graphData.Nodes.First(n => n.Id == predecessors[node.Id]).Label;
				Console.WriteLine($"Nodo {node.Label}: {predecessorLabel}");
			}

			Console.WriteLine("\nShortest Paths from the source node:");
			foreach (var node in graphData.Nodes)
			{
				if (node.Id != startNodeId)
				{
					Console.WriteLine($"Shortest path to Nodo {node.Label}:");
					Console.Write($"  Path: {startNode.Label}");

					int current = node.Id;
					while (predecessors[current] != -1)
					{
						Console.Write($" -> {graphData.Nodes.First(n => n.Id == predecessors[current]).Label}");
						current = predecessors[current];
					}

					string distanceLabel = distances[node.Id] == int.MaxValue ? "∞" : distances[node.Id].ToString();
					Console.WriteLine($"\n  Distance: {distanceLabel}");
					Console.WriteLine();
				}
			}

			Console.ReadKey();
		}
	}
}
