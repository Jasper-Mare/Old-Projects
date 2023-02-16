using System;
using System.Linq;
using static System.MathF;
using System.Numerics;
using System.Collections.Generic;
using System.Threading.Tasks;
using VoronoiLib;

namespace CityGenerator2 {
    class City {
        readonly public float CityW, CityH; //in metres
        Random rng = new Random();

        public List<(Vector2 pos, Raylib_cs.Color color)> VoroniRegions;
        public List<(int a, int b)> VoroniEdges;
        public List<Vector2> VoroniVertexes;
        float VoroniDensity = 50f/(1000*1000); //voroni regions per metre squared

        public List<Road> roads;
        public List<int> junctions;

        public City(float W, float H) {
            CityW = W; CityH = H;
            VoroniRegions = new List<(Vector2 pos, Raylib_cs.Color color)>();
            VoroniEdges = new List<(int a, int b)>();
            VoroniVertexes = new List<Vector2>();
            roads = new List<Road>();
            junctions = new List<int>();
        }
        public void Create() {
            List<VoronoiLib.Structures.FortuneSite> sites = new List<VoronoiLib.Structures.FortuneSite>();
            //scatter voroni region nodes
            int numVoronis = (int)(CityW*CityH*VoroniDensity);
            for (int i = 0; i < numVoronis; i++) {
                VoroniRegions.Add((new Vector2((float)(rng.NextDouble()*CityW), (float)(rng.NextDouble()*CityH)), Raylib_cs.Raylib.ColorFromHSV(rng.Next(0,360), (float)rng.NextDouble()*0.5f+0.5f, (float)rng.NextDouble()*0.5f+0.5f) ));
                sites.Add(new VoronoiLib.Structures.FortuneSite(VoroniRegions[VoroniRegions.Count-1].pos.X, VoroniRegions[VoroniRegions.Count-1].pos.Y));
            }

            List<VoronoiLib.Structures.VEdge> edges = FortunesAlgorithm.Run(sites, 0,0, CityW, CityH).ToList();
            Dictionary<int, List<int>> connections = new Dictionary<int, List<int>>();

            //parse diagram data
            for (int i = 0; i < edges.Count; i++) {
                Vector2 a = new Vector2((float)edges[i].Start.X, (float)edges[i].Start.Y), 
                        b = new Vector2((float)edges[i].End.X, (float)edges[i].End.Y);
                int ia, ib;
                if (VoroniVertexes.Contains(a)) {
                    ia = VoroniVertexes.IndexOf(a);
                } else {
                    ia = VoroniVertexes.Count;
                    connections[ia] = new List<int>();
                    VoroniVertexes.Add(a);
                }

                if (VoroniVertexes.Contains(b)) {
                    ib = VoroniVertexes.IndexOf(b);
                } else {
                    ib = VoroniVertexes.Count;
                    connections[ib] = new List<int>();
                    VoroniVertexes.Add(b);
                }
                connections[ia].Add(ib);
                connections[ib].Add(ia);
                VoroniEdges.Add((ia,ib));
            }

            { //sprinkle in junctions
                int numJunctions = rng.Next((int)(VoroniVertexes.Count*0.2f), (int)(VoroniVertexes.Count*0.6f));
                List<int> unvisited = Enumerable.Range(0, VoroniVertexes.Count).ToList();
                for (int i = 0; i < numJunctions; i++) {
                    int j = unvisited[rng.Next(unvisited.Count)];
                    unvisited.Remove(j);
                    junctions.Add(j);
                }
            }

            { //connect junctions with roads (rework)
                foreach (int junction in junctions) {
                    //search for junction to connect to
                    List<int> visitedNodes = new List<int>();
                    visitedNodes.Add(junction);
                    bool done = false;

                    int prevNode = junction;
                    while (!done) {
                        int currentNode = visitedNodes.Last();
                        List<int> currentNodesNeghibours = connections[currentNode].Where(i => i != junction && i != prevNode).ToList();
                        if (currentNodesNeghibours.Count == 0) { break; }
                        visitedNodes.Add(currentNodesNeghibours[rng.Next(currentNodesNeghibours.Count)]);
                        prevNode = currentNode;
                        currentNode = visitedNodes.Last();

                        done = junctions.Contains(currentNode);
                    }

                    if (!done) { continue; }

                    Road road = new Road((RoadType)rng.Next(4));
                    road.startJunction = junction;
                    road.nodes.Add((VoroniVertexes[junction], Vector2.Normalize(VoroniVertexes[junction]-VoroniVertexes[visitedNodes[1]]) ));
                    for (int i = 1; i < visitedNodes.Count-1; i++) {
                        Vector2 pos = VoroniVertexes[visitedNodes[i]],
                                prevPos = road.nodes[i-1].pos,
                                nextPos = VoroniVertexes[visitedNodes[i+1]],
                                dir = Vector2.Lerp(Vector2.Normalize(prevPos-pos), Vector2.Normalize(pos-nextPos), 0.5f);
                        road.nodes.Add((pos, dir));
                    }
                    road.nodes.Add((VoroniVertexes[visitedNodes.Last()], Vector2.Normalize(VoroniVertexes[visitedNodes[^2]]-VoroniVertexes[visitedNodes.Last()]) ));
                    road.endJunction = visitedNodes.Last();
                    roads.Add(road);
                }
            }


        }
    }

    class Road {
        public readonly RoadType type;
        public List<(Vector2 pos, Vector2 dir)> nodes;
        public int startJunction, endJunction;
        private List<Vector2> drawPoints;

        public Road(RoadType type) {
            this.type = type;
            nodes = new List<(Vector2 pos, Vector2 dir)>();
        }

        public Vector2[] getDrawPoints() {
            if (!(drawPoints is null) && drawPoints.Count > 0) { return drawPoints.ToArray(); }

            drawPoints = new List<Vector2>();
            for (int i = 0; i < nodes.Count-1; i++) {
                Vector2 p1 = nodes[i].pos, p2 = nodes[i+1].pos; 
                float len = (p1-p2).Length()*2f;
                Vector2 m1 = nodes[i].dir*len, m2 = nodes[i+1].dir*len;

                for (float t = 0; t < 1; t += 0.1f) {
                    drawPoints.Add((2*t*t*t-3*t*t+1)*p1+(t*t*t-2*t*t+t)*m1+(-2*t*t*t+3*t*t)*p2+(t*t*t-t*t)*m2);
                }
            }

            return drawPoints.ToArray();
        }

    }
    public enum RoadType {
        Boulevard = 0, Avenue = 1, Street = 2, Passage = 3
    }


}
