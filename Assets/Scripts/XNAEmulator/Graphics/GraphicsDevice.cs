using System;
using System.Collections.Generic;
using System.Linq;
using ClassicUO.Renderer;
using UnityEngine;
using Debug = System.Diagnostics.Debug;
using UnityGraphics = UnityEngine.Graphics;

namespace Microsoft.Xna.Framework.Graphics
{
    public class GraphicsDevice
    {
        Viewport viewport = new Viewport();

        public GraphicsDevice( )
        {
            // TODO: Complete member initialization
            
            viewport = new Viewport( 0, 0, UnityEngine.Screen.width, UnityEngine.Screen.height );
        }

        public Viewport Viewport
        {
            get { return viewport; }
            set { viewport = value; }
        }

        public Rectangle ScissorRectangle { get; set; }
        public BlendState BlendState { get; set; }
        public DepthStencilState DepthStencilState { get; set; }
        public RasterizerState RasterizerState { get; set; }
        public IndexBuffer Indices { get; set; }

        public Texture2D[] Textures = new Texture2D[3];
        private VertexBuffer m_VertexBuffer;


        internal void Clear( Color color )
        {

        }
        /*internal void DrawIndexedPrimitives( PrimitiveType primitiveType, int baseVertex, int minVertexIndex, int numVertices, int startIndex, int primitiveCount )
        {
            throw new NotImplementedException();
        }*/
        internal void SetRenderTarget( RenderTarget2D renderTarget )
        {
           // XNATest.Draw.Enqueue( new XNATest.SetRenderTextureDrawCall( renderTarget ) );
           if ( renderTarget != null )
           {
               // GL.PopMatrix();
                   
               UnityEngine.Graphics.SetRenderTarget( renderTarget.UnityTexture as RenderTexture );
               GL.Clear(true,true, UnityEngine.Color.black );
               //GL.PushMatrix();
               GL.LoadPixelMatrix( 0, renderTarget.UnityTexture.width, renderTarget.UnityTexture.height, 0 );

           }
           else
           {
               //GL.PopMatrix();
               UnityEngine.Graphics.SetRenderTarget( null );
               //  GL.PushMatrix();
               GL.LoadPixelMatrix( 0, Screen.width, Screen.height, 0 );
           }


        }


        public void Clear(ClearOptions depthBuffer, Vector4 vectorClear, int i, int i1)
        {
            GL.Clear(true,true, UnityEngine.Color.black );

        }

        public void Present()
        {
            //throw new NotImplementedException();
            ResetPools();
        }
        public void ResetPools()
        {
            _materialPool.Reset();
            _meshPool.Reset();
        }
        /// <summary>
        /// Draw geometry by indexing into the vertex buffer.
        /// </summary>
        /// <param name="primitiveType">
        /// The type of primitives in the index buffer.
        /// </param>
        /// <param name="baseVertex">
        /// Used to offset the vertex range indexed from the vertex buffer.
        /// </param>
        /// <param name="minVertexIndex">
        /// A hint of the lowest vertex indexed relative to baseVertex.
        /// </param>
        /// <param name="numVertices">
        /// A hint of the maximum vertex indexed.
        /// </param>
        /// <param name="startIndex">
        /// The index within the index buffer to start drawing from.
        /// </param>
        /// <param name="primitiveCount">
        /// The number of primitives to render from the index buffer.
        /// </param>
        public void DrawIndexedPrimitives(
            PrimitiveType primitiveType,
            int baseVertex,
            int minVertexIndex,
            int numVertices,
            int startIndex,
            int primitiveCount
        )
        {
            
            
            //postrender??
            //material.SetPass(0);
            var mat = GetMat(Textures[0]);
            mat.SetBuffer("_vertices", m_VertexBuffer.Buffer);
            mat.SetBuffer("_indices", Indices.Buffer);

                mat.SetPass(0);
            UnityEngine.Graphics.DrawProceduralNow(MeshTopology.Triangles,numVertices);
            
           /* var verts = m_VertexBuffer.Data.Skip(baseVertex).Take(numVertices).ToArray();
            //Texture is in Textures[0]
            //XNATest.Draw.Enqueue(new XNATest.IndexedPrimativeDrawCall(Textures[0],verts,primitiveCount));
            
            for ( int i = 0; i < verts.Length; i++ )
            {
                verts[i].TextureCoordinate.Y = 1 - verts[i].TextureCoordinate.Y;
            }
            var testmesh = GetMesh(primitiveCount);
            testmesh.Populate( verts, verts.Length );
             //if( lastMaterial  != MainTexure )
            {
                GetMat(Textures[0]).SetPass(0);
                // MainTexure.SetPass( 0 );
                //lastMaterial = MainTexure;

            }
           UnityEngine.Graphics.DrawMeshNow( testmesh.Mesh, UnityEngine.Vector3.zero, UnityEngine.Quaternion.identity );*/
        }


        public void SetVertexBuffer(VertexBuffer vertexBuffer)
        {
            m_VertexBuffer = vertexBuffer;
        }
        private readonly MeshPool _meshPool = new MeshPool();
        public Material GetMat(Texture2D text)
        {
            return _materialPool.Get( text );
        }

        private readonly MaterialPool _materialPool = new MaterialPool();
        public MeshHolder GetMesh(int primCCount)
        {
            return _meshPool.Get( primCCount );
        }
    }
    
    
    
    
    
    
     public class GraphicsDevice22 : IDisposable
    {
        public Texture2D[] Textures = new Texture2D[3];
        private Matrix4x4 _matrix;
        private Matrix4x4 _baseMatrix;

        internal GraphicsDevice22( Viewport viewport )
        {
            Viewport = viewport;
        }

        public GraphicsDevice22(  )
        {
        }

        private Viewport _viewport;
        public Viewport Viewport
        {
            get { return _viewport; }
            internal set
            {
                _viewport = value;
                _baseMatrix = Matrix4x4.TRS( new UnityEngine.Vector3( -_viewport.Width / 2, _viewport.Height / 2, 0 ), UnityEngine.Quaternion.identity, new UnityEngine.Vector3( 1, -1, 1 ) );
            }
        }
        public Matrix Matrix
        {
            set
            {
               
               // _matrix = _baseMatrix * value;
            }
        }
        public MeshHolder GetMesh(int primCCount)
        {
            return _meshPool.Get( primCCount );
        }
        public Material GetMat(Texture2D text)
        {
            return _materialPool.Get( text );
        }

        private readonly MaterialPool _materialPool = new MaterialPool();
        private readonly MeshPool _meshPool = new MeshPool();

        public void DrawUserIndexedPrimitives( PrimitiveType primitiveType, VertexPositionColorTexture[] vertexData, int vertexOffset, int numVertices, short[] indexData, int indexOffset, int primitiveCount, VertexDeclaration vertexDeclaration )
        {
            Debug.Assert( vertexData != null && vertexData.Length > 0, "The vertexData must not be null or zero length!" );
            Debug.Assert( indexData != null && indexData.Length > 0, "The indexData must not be null or zero length!" );

            var material = _materialPool.Get( Textures[0] );

            var mesh = _meshPool.Get( primitiveCount / 2 );
            mesh.Populate( vertexData, numVertices );

            UnityGraphics.DrawMesh( mesh.Mesh, _matrix, material, 0 );
        }

        internal void DrawIndexedPrimitives( PrimitiveType primitiveType, int baseVertex, int minVertexIndex, int numVertices, int startIndex, int primitiveCount )
        {
            throw new NotImplementedException();
        }

        public void ResetPools()
        {
            _materialPool.Reset();
            _meshPool.Reset();
        }

        public void Clear( Color color )
        {
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

       

       

        internal void SetRenderTarget( RenderTarget2D renderTarget )
        {
            throw new NotImplementedException();
        }
    }
    public class MeshHolder
        {
            public readonly int SpriteCount;
            public readonly Mesh Mesh;

            public readonly UnityEngine.Vector3[] Vertices;
            public readonly UnityEngine.Vector2[] UVs;
            public readonly Color32[] Colors;

            public MeshHolder( int spriteCount )
            {
                Mesh = new Mesh();
                Mesh.MarkDynamic(); //Seems to be a win on wp8

                SpriteCount = NextPowerOf2( spriteCount );
                int vCount = SpriteCount * 4;

                Vertices = new UnityEngine.Vector3[vCount];
                UVs = new UnityEngine.Vector2[vCount];
                Colors = new Color32[vCount];

                //Put some random crap in this so we can just set the triangles once
                //if these are not populated then unity totally fucks up our mesh and never draws it
                for ( var i = 0; i < vCount; i++ )
                {
                    Vertices[i] = new UnityEngine.Vector3( 1, i );
                    UVs[i] = new UnityEngine.Vector2( 0, i );
                    Colors[i] = new Color32( 255, 255, 255, 255 );
                }

                var triangles = new int[SpriteCount * 6];
                for ( var i = 0; i < SpriteCount; i++ )
                {
                    /*
					 *  TL    TR
					 *   0----1 0,1,2,3 = index offsets for vertex indices
					 *   |   /| TL,TR,BL,BR are vertex references in SpriteBatchItem.
					 *   |  / |
					 *   | /  |
					 *   |/   |
					 *   2----3
					 *  BL    BR
					 */
                    // Triangle 1
                    triangles[i * 6 + 0] = i * 4;
                    triangles[i * 6 + 1] = i * 4 + 1;
                    triangles[i * 6 + 2] = i * 4 + 2;
                    // Triangle 2
                    triangles[i * 6 + 3] = i * 4 + 1;
                    triangles[i * 6 + 4] = i * 4 + 3;
                    triangles[i * 6 + 5] = i * 4 + 2;
                }

                Mesh.vertices = Vertices;
                Mesh.uv = UVs;
                Mesh.colors32 = Colors;
                Mesh.triangles = triangles;
            }
            
            internal void Populate( ClassicUO.Renderer.PositionNormalTextureColor[] vertexData, int numVertices )
            {
                for ( int i = 0; i < numVertices; i++ )
                {
                    var p = vertexData[i].Position;
                    Vertices[i] = new UnityEngine.Vector3( p.X, p.Y, p.Z );

                    var uv = vertexData[i].TextureCoordinate;
                    UVs[i] = new UnityEngine.Vector2( uv.X, /*1 - */uv.Y );

                    var c = vertexData[i].Normal;
                    // Colors[i] = new Color32( c.R, c.G, c.B, c.A );
                }
                //we could clearly less if we remembered how many we used last time
                Array.Clear( Vertices, numVertices, Vertices.Length - numVertices );

                Mesh.vertices = Vertices;
                Mesh.uv = UVs;
                //Mesh.colors32 = Colors;
            }
                 public void Populate( ClassicUO.Renderer.SpriteVertex[] vertexData, int numVertices )
            {
                for ( int i = 0; i < numVertices; i++ )
                {
                    var p = vertexData[i].Position;
                    Vertices[i] = new UnityEngine.Vector3( p.X, p.Y, p.Z );

                    var uv = vertexData[i].TextureCoordinate;
                    UVs[i] = new UnityEngine.Vector2( uv.X, /*1 - */uv.Y );

                    var c = vertexData[i].Normal;
                   // Colors[i] = new Color32( c.R, c.G, c.B, c.A );
                }
                //we could clearly less if we remembered how many we used last time
                Array.Clear( Vertices, numVertices, Vertices.Length - numVertices );

                Mesh.vertices = Vertices;
                Mesh.uv = UVs;
                //Mesh.colors32 = Colors;
            }


            public void Populate( VertexPositionColorTexture[] vertexData, int numVertices )
            {
                for ( int i = 0; i < numVertices; i++ )
                {
                    var p = vertexData[i].Position;
                    Vertices[i] = new UnityEngine.Vector3( p.X, p.Y, p.Z );

                    var uv = vertexData[i].TextureCoordinate;
                    UVs[i] = new UnityEngine.Vector2( uv.X, 1 - uv.Y );

                    var c = vertexData[i].Color;
                    Colors[i] = new Color32( c.R, c.G, c.B, c.A );
                }
                //we could clearly less if we remembered how many we used last time
                Array.Clear( Vertices, numVertices, Vertices.Length - numVertices );

                Mesh.vertices = Vertices;
                Mesh.uv = UVs;
                Mesh.colors32 = Colors;
            }

            public int NextPowerOf2( int minimum )
            {
                int result = 1;

                while ( result < minimum )
                    result *= 2;

                return result;
            }
        }

    public class MaterialPool
    {
        private class MaterialHolder
        {
            public readonly Material Material;
            public readonly Texture2D Texture2D;

            public MaterialHolder( Material material, Texture2D texture2D )
            {
                Material = material;
                Texture2D = texture2D;
            }
        }

        private readonly List<MaterialHolder> _materials = new List<MaterialHolder>();
        private int _index;
        private readonly Shader _shader = Shader.Find( "Unlit/HueShader" );

        private MaterialHolder Create( Texture2D texture )
        {
            var mat = new Material( _shader );
            mat.mainTexture = texture.UnityTexture;
            mat.SetTexture( "_HueTex", texture.GraphicDevice.Textures[1].UnityTexture );
            mat.renderQueue += _materials.Count;
            return new MaterialHolder( mat, texture );
        }

        public Material Get( Texture2D texture )
        {
            while ( _index < _materials.Count )
            {
                if ( _materials[_index].Texture2D == texture )
                {
                    _index++;
                    return _materials[_index - 1].Material;
                }

                _index++;
            }

            var material = Create( texture );
            _materials.Add( material );
            _index++;
            return _materials[_index - 1].Material;
        }

        public void Reset()
        {
            _index = 0;
        }
    }
    public class MeshPool
        {
            private List<MeshHolder> _unusedMeshes = new List<MeshHolder>();
            private List<MeshHolder> _usedMeshes = new List<MeshHolder>();

            private List<MeshHolder> _otherMeshes = new List<MeshHolder>();
            //private int _index;

            /// <summary>
            /// get a mesh with at least this many triangles
            /// </summary>
            public MeshHolder Get( int spriteCount )
            {
                MeshHolder best = null;
                int bestIndex = -1;
                for ( int i = 0; i < _unusedMeshes.Count; i++ )
                {
                    var unusedMesh = _unusedMeshes[i];
                    if ( ( best == null || best.SpriteCount > unusedMesh.SpriteCount ) && unusedMesh.SpriteCount >= spriteCount )
                    {
                        best = unusedMesh;
                        bestIndex = i;
                    }
                }
                if ( best == null )
                {
                    best = new MeshHolder( spriteCount );
                }
                else
                {
                    _unusedMeshes.RemoveAt( bestIndex );
                }
                _usedMeshes.Add( best );

                return best;
            }

            public void Reset()
            {
                //Double Buffer our Meshes (Doesnt seem to be a win on wp8)
                //Ref http://forum.unity3d.com/threads/118723-Huge-performance-loss-in-Mesh-CreateVBO-for-dynamic-meshes-IOS

                //meshes from last frame are now unused
                _unusedMeshes.AddRange( _otherMeshes );
                _otherMeshes.Clear();

                //swap our use meshes and the now empty other meshes
                var temp = _otherMeshes;
                _otherMeshes = _usedMeshes;
                _usedMeshes = temp;
            }
        }
    
    
    
    
}