using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace AEngine
{
    public class MeshAsset : Asset
    {
        protected List<Vector3> NormalsList { get; private set; }
        protected List<Triangle> TriangleList { get; private set; }
        protected List<Vector2> UvList { get; private set; }

        private void LoadObjFile(string fileName)
        {
            var vertexList = new List<Vector3>();
            var trianglesVertexList = new List<int[]>();
            foreach (var line in File.ReadLines(fileName))
            {
                var items = line.Replace("  ", " ").Split(' ');
                // vertex
                if (items[0] == "v")
                {
                    vertexList.Add(new Vector3(
                        float.Parse(items[1], CultureInfo.InvariantCulture),
                        float.Parse(items[2], CultureInfo.InvariantCulture),
                        float.Parse(items[3], CultureInfo.InvariantCulture) - 1f
                        )
                        );
                }
                if (items[0] == "vt")
                {
                    UvList.Add(new Vector2(
                        float.Parse(items[1], CultureInfo.InvariantCulture),
                        float.Parse(items[2], CultureInfo.InvariantCulture)
                        )
                        );
                }
                if (items[0] == "vn")
                {
                    NormalsList.Add(new Vector3(
                        float.Parse(items[1], CultureInfo.InvariantCulture),
                        float.Parse(items[2], CultureInfo.InvariantCulture),
                        float.Parse(items[3], CultureInfo.InvariantCulture)
                        )
                        );
                }
                if (items[0] == "f")
                {
                    var v1 = items[1].Split('/');
                    var v2 = items[2].Split('/');
                    var v3 = items[3].Split('/');
                    trianglesVertexList.Add(new[]
                    {
                        // vertexes
                        int.Parse(v1[0]) - 1,
                        int.Parse(v2[0]) - 1,
                        int.Parse(v3[0]) - 1,
                        // materials
                        int.Parse(v1[1]) - 1,
                        int.Parse(v2[1]) - 1,
                        int.Parse(v3[1]) - 1
                    });
                    //// normals
                    //int.Parse(v1[2]) - 1,
                    //int.Parse(v2[2]) - 1,
                    //int.Parse(v3[2]) - 1
                }
            }

            foreach (var args in trianglesVertexList)
            {
                var vertex1 = new Vertex3(vertexList[args[0]], UvList[args[3]]);
                var vertex2 = new Vertex3(vertexList[args[1]], UvList[args[4]]);
                var vertex3 = new Vertex3(vertexList[args[2]], UvList[args[5]]);
                var triangle = new Triangle(
                    null,
                    vertex1, vertex2, vertex3
                    /*normalsList[args[5]],
                    normalsList[args[6]],
                    normalsList[args[7]]*/
                    );
                TriangleList.Add(triangle);
            }
        }

        public MeshAsset(string fileName) : base(fileName)
        {
            UvList = new List<Vector2>();
            NormalsList = new List<Vector3>();
            TriangleList = new List<Triangle>();
            LoadObjFile(FileName);
        }

        public void LoadInto(Mesh mesh)
        {
            mesh.UvList = UvList.ToList();
            mesh.NormalsList = NormalsList.ToList();
            mesh.TriangleList = TriangleList.ConvertAll(triangle =>
            {
                triangle = triangle.Clone();
                triangle.Owner = mesh;
                return triangle;
            });
        }
    }
}
