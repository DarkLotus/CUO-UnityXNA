using UnityEngine;
using System.Collections;
using ClassicUO;
using ClassicUO.Utility.Logging;
using System;
using System.Collections.Generic;
using ClassicUO.Renderer;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Microsoft.Xna.Framework;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using Vector4 = UnityEngine.Vector4;

public class XNATest : MonoBehaviour {
    Engine game;
	
	public  float updateInterval = 0.5F;
 
	private float accum   = 0; // FPS accumulated over the interval
	private int   frames  = 0; // Frames drawn over the interval
	private float timeleft; // Left time for current interval
	float fps;
	// Use this for initialization
	void Start () 
    {
		Application.targetFrameRate = 240;
        MainTexure = new Material( Shader.Find( "Unlit/HueShader" ) );
        MainTexure.mainTexture = new UnityEngine.Texture2D( 8192, 8192 );
        // Add an audio source and tell the media player to use it for playing sounds
        Microsoft.Xna.Framework.Media.MediaPlayer.AudioSource = gameObject.AddComponent<AudioSource>();
        dev = new GraphicsDevice22();
        Log.Start( LogTypes.All );
        Engine.Run(new string[]{});
        game = Engine.Instance;
        //game.Initialize();

        //game.Run();
		timeleft = updateInterval;
        //Application.targetFrameRate = 30;







    }

	// Update is called once per frame
	void Update ()
	{

		if (game == null)
			return;

        if (Screen.width != Engine.WindowWidth)
            Engine.WindowWidth = Screen.width;
        if (Screen.height != Engine.WindowHeight)
            Engine.WindowHeight = Screen.height;
        
        float deltaTime = Time.deltaTime;
        if(deltaTime > 0.050f)
        {
            deltaTime = 0.050f;
        }
        //Debug.Log(deltaTime);
        long microseconds = (int)(deltaTime * 1000000);
        long ticks = microseconds * 10;
        game.Tick((uint)ticks );

    }

    public abstract class DrawCall
    {
        public Texture Texture { get; set; }

    }
    public class StandardDrawCall : DrawCall
    {

        public Rect SourceRect { get; set; } = new Rect( 0, 0, 1, 1 );
        /// <summary>
        /// In pixels
        /// </summary>
        public Rect ScreenRect { get; set; }
        public Material Material { get; set; }
    }
    private static GraphicsDevice22 dev;
    internal class IndexedPrimativeDrawCall : XNATest.DrawCall
    {
	    public IndexedPrimativeDrawCall(Microsoft.Xna.Framework.Graphics.Texture2D texture2D, PositionNormalTextureColor[] vertices, int primitiveCount)
	    {
		    PrimativeCount = primitiveCount;
		    Texture = texture2D.UnityTexture;
		    Verts = new PositionNormalTextureColor[vertices.Length];
			vertices.CopyTo( Verts, 0 );
            for ( int i = 0; i < vertices.Length; i++ )
            {
                Verts[i].TextureCoordinate.Y = 1 - Verts[i].TextureCoordinate.Y;
            }
            Position = new Vector3( vertices[0].Position.X, vertices[0].Position.Y );
            Material = dev.GetMat( texture2D ); //new Material( Shader.Find( "Unlit/Transparent" ) );// 

            //TODO
            Material.SetVector( "_Hue", new Vector4( vertices[0].Hue.X, vertices[0].Hue.Y, vertices[0].Hue.Z, 0 ) );
	    }

	    public int PrimativeCount { get; set; }
	    public Material Material { get; set; }

	    public Vector3 Position { get; set; }

	    public PositionNormalTextureColor[] Verts { get; set; }
    }
    public class MeshDrawCall : DrawCall
    {
        public MeshDrawCall( Microsoft.Xna.Framework.Graphics.Texture2D texture, SpriteVertex[] vertices )
        {
            Verts = new SpriteVertex[vertices.Length];
            vertices.CopyTo( Verts, 0 );
            for ( int i = 0; i < 4; i++ )
            {
                Verts[i].TextureCoordinate.Y = 1 - Verts[i].TextureCoordinate.Y;
            }



                if ( vertices[0].Hue == Microsoft.Xna.Framework.Vector3.Zero && texture.Width == 44 && texture.Height == 44)
            {
                if ( TextureIndex.ContainsKey( texture.UnityTexture as UnityEngine.Texture2D ) )
                    Index = TextureIndex[texture.UnityTexture as UnityEngine.Texture2D];
                else
                {
                    int nextIndex = 0;
                    if ( TextureIndex.Values.Any() )
                        nextIndex = TextureIndex.Values.Last() + 1;
                    TextureIndex.Add( texture.UnityTexture as UnityEngine.Texture2D, nextIndex );
                    Graphics.CopyTexture( texture.UnityTexture, 0, 0, 0, 0, texture.Width, texture.Height, MainTexure.mainTexture, 0, 0, (int)( nextIndex % 186.1818181818182f ) * 44, (int)( nextIndex / 186.1818181818182f ) * 44 );
                    Index = nextIndex;
                }

                for ( int i = 0; i < 4; i++ )
                {

                    Verts[i].TextureCoordinate.X *= 0.00537109375f;
                    Verts[i].TextureCoordinate.Y *= 0.00537109375f;
                    Verts[i].TextureCoordinate.X += ( Index / 186.1818181818182f );
                    Verts[i].TextureCoordinate.Y += ( Index % 186.1818181818182f );
                    // Verts[i].TextureCoordinate.Y = ( Verts[i].TextureCoordinate.Y / 186f ) + ( (Index / 186) * 44f / 8192f );

                    // Verts[i].TextureCoordinate.X = ( Verts[i].TextureCoordinate.X / 186f );// + ( (Index % 186 ) * 0.0001220703125f );


                    //Verts[i].TextureCoordinate.Y = ( Verts[i].TextureCoordinate.Y / 186f );// + ( ( Index / 186 ) * 0.0001220703125f );

                }

            }
            else
            {
                Material = dev.GetMat( texture ); //new Material( Shader.Find( "Unlit/Transparent" ) );// 

                //TODO
                Material.SetVector( "_Hue", new Vector4( vertices[0].Hue.X, vertices[0].Hue.Y, vertices[0].Hue.Z, 0 ) );
            }




            //  Material.mainTexture = texture.UnityTexture;
            Pos = new Vector3( vertices[0].Pos.x, vertices[0].Pos.y );
           // var mesh = dev.GetMesh( 1 );
           // mesh.Populate( vertices, vertices.Length );
           // Mesh = mesh.Mesh;

            
        }
        public SpriteVertex[] Verts;

        public Material Material { get; set; }
        //public Matrix4x4 Matrix { get; internal set; }
       // public Mesh Mesh { get; internal set; }

        public Vector3 Pos { get; set; }
        public Quaternion Rotation { get; set; } = Quaternion.identity;
        public int Index = -1;
    }
    public class SetRenderTextureDrawCall : DrawCall
    {
        
        public SetRenderTextureDrawCall(RenderTarget2D text)
        {
            Texture = text?.UnityTexture;
        }
    }

    public static Queue<DrawCall> Draw = new Queue<DrawCall>();


    public static Material MainTexure;
    public static Dictionary<UnityEngine.Texture2D, int> TextureIndex = new Dictionary<UnityEngine.Texture2D, int>();
    private void OnPostRender()
    {
	    
	    if (game == null)
		    return;
        //dev.ResetPools();
        //game.DrawUnity( Time.deltaTime );
        

        //GL.PushMatrix();
        GL.LoadPixelMatrix( 0, Screen.width, Screen.height, 0 );

        game.Render();
        //GL.PopMatrix();
        return;
        
        var cam = GameObject.FindWithTag( "MainCamera" ).GetComponent<Camera>();
        var curRT = cam.activeTexture;
        GL.Color( new UnityEngine.Color( 0, 0, 0, 1 ) );
       // GL.Begin( GL.TRIANGLE_STRIP );
        //MainTexure.SetPass( 0 );
        var mesh = dev.GetMesh( 1 );
        int cnt = 0;
        var Draw2 = new Queue<DrawCall>();
        Material lastMaterial = MainTexure; MainTexure.SetPass( 0 );
        while ( Draw.Count > 0 )
        {
            var call = Draw.Dequeue();
            if(call is SetRenderTextureDrawCall rtcall) {
               // GL.End();
                if ( rtcall.Texture != null )
                {
                    // GL.PopMatrix();
                   
                    Graphics.SetRenderTarget( rtcall.Texture as RenderTexture );
                    GL.Clear(true,true, UnityEngine.Color.black );
                    //GL.PushMatrix();
                    GL.LoadPixelMatrix( 0, rtcall.Texture.width, rtcall.Texture.height, 0 );

                }
                else
                {
                    //GL.PopMatrix();
                    Graphics.SetRenderTarget( curRT );
                  //  GL.PushMatrix();
                    GL.LoadPixelMatrix( 0, Screen.width, Screen.height, 0 );
                }
              //  GL.Begin( GL.TRIANGLE_STRIP );
                //MainTexure.SetPass( 0 );
                cnt = 0;
            }
            else if(call is StandardDrawCall sdcall)
            {
                //Draw2.Enqueue( sdcall );
               // GL.End();
             Graphics.DrawTexture( sdcall.ScreenRect, sdcall.Texture, sdcall.SourceRect, 0, 0, 0, 0, null );
                // GL.Begin( GL.TRIANGLE_STRIP );
                // MainTexure.SetPass( 0 );
                lastMaterial = null;
                cnt = 0;
            }
            else if (call is IndexedPrimativeDrawCall icall)
            {
	            
	            var testmesh = dev.GetMesh(icall.PrimativeCount);
		            testmesh.Populate( icall.Verts, icall.Verts.Length );
	         // if( lastMaterial  != MainTexure )
	         {
		         icall.Material.SetPass(0);
		           // MainTexure.SetPass( 0 );
		            //lastMaterial = MainTexure;

	            }
	          Graphics.DrawMeshNow( testmesh.Mesh, Vector3.zero, Quaternion.identity );

            }
            else if (call is MeshDrawCall mcall)
            {

                
                mesh.Populate( mcall.Verts, mcall.Verts.Length );
                if ( mcall.Index == -1 )
                {
                    mcall.Material.SetPass( 0 );
                    lastMaterial = null;
                }     
                else if( lastMaterial  != MainTexure )
                {
                    MainTexure.SetPass( 0 );
                    lastMaterial = MainTexure;

                }

                /*if(mcall.Index == -1)
                    Graphics.DrawMesh( mesh.Mesh, Vector3.zero, mcall.Rotation, mcall.Material, 0 );
                else
                    Graphics.DrawMesh( mesh.Mesh, Vector3.zero, mcall.Rotation, MainTexure, 0 );*/
                Graphics.DrawMeshNow( mesh.Mesh, Vector3.zero, mcall.Rotation );
                
                
                continue;
                //Graphics.DrawMeshNow( mcall.Mesh, Vector3.zero, mcall.Rotation );   

                /*GL.Begin( GL.QUADS );
                 GL.Color( new UnityEngine.Color( 0, 0, 0, 1 ) );

                 //mcall.Material.SetPass( 0 );
                 //Graphics.DrawMeshNow( mcall.Mesh, Vector3.zero, mcall.Rotation );   
                 // GL.TexCoord( mcall.Verts[0].TextureCoordinate.ToVec3() );
                 GL.Vertex( mcall.Verts[0].Position.ToVec3());

                 // GL.TexCoord( mcall.Verts[2].TextureCoordinate.ToVec3() );
                  GL.Vertex( mcall.Verts[2].Position.ToVec3() );

                 // GL.TexCoord( mcall.Verts[3].TextureCoordinate.ToVec3() );
                  GL.Vertex( mcall.Verts[3].Position.ToVec3() );

                 // GL.TexCoord( mcall.Verts[1].TextureCoordinate.ToVec3() );
                  GL.Vertex( mcall.Verts[1].Position.ToVec3() );
                 GL.End();*/

                if ( cnt > 0 )
                {
                    GL.TexCoord( mcall.Verts[2].TextureCoordinate.ToVec3() );
                    GL.Vertex( mcall.Verts[0].Position.ToVec3() );
                }

               GL.TexCoord( mcall.Verts[2].TextureCoordinate.ToVec3() );
                GL.Vertex( mcall.Verts[0].Position.ToVec3() );
                

                GL.TexCoord( mcall.Verts[3].TextureCoordinate.ToVec3() );
                GL.Vertex( mcall.Verts[1].Position.ToVec3() );

                GL.TexCoord( mcall.Verts[0].TextureCoordinate.ToVec3() );
                GL.Vertex( mcall.Verts[2].Position.ToVec3() );

                GL.TexCoord( mcall.Verts[1].TextureCoordinate.ToVec3() );
                GL.Vertex( mcall.Verts[3].Position.ToVec3() );



                GL.TexCoord( mcall.Verts[1].TextureCoordinate.ToVec3() );
                GL.Vertex( mcall.Verts[3].Position.ToVec3() );
                cnt++;

            }
        }
       // GL.End();


        GL.PopMatrix();
       // if(TextureIndex.Count > 20)
       // System.IO.File.WriteAllBytes( "woo.png", ( MainTexure.mainTexture as UnityEngine.Texture2D ).EncodeToPNG() );
    }
   
		

}
