using UnityEngine;
using System.Collections;
using ClassicUO;
using ClassicUO.Utility.Logging;
using System;
using System.Collections.Generic;
using ClassicUO.Renderer;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

public class XNATest : MonoBehaviour {
    GameLoop game;
	DrawQueue drawQueue;
	
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
        drawQueue = new DrawQueue();
        Log.Start( LogTypes.All );
        game = new GameLoop();
        game.DrawQueue = drawQueue;
        game.Initialize();

        game.Run();
		timeleft = updateInterval;
        //Application.targetFrameRate = 30;







    }

	// Update is called once per frame
	void Update () {

		float deltaTime = Time.deltaTime;
		if(deltaTime > 0.050f)
		{
			deltaTime = 0.050f;
		}
        //Debug.Log(deltaTime);
        game.Tick( Time.deltaTime );
        drawQueue.Clear();

        timeleft -= Time.deltaTime;
	    accum += Time.timeScale/Time.deltaTime;
	    ++frames;
	    
	    // Interval ended - update GUI text and start new interval
	    if( timeleft <= 0.0 )
	    {
	        // display two fractional digits (f2 format)
	  	  fps = accum/frames;
		
	 	   //  DebugConsole.Log(format,level);
	        timeleft = updateInterval;
	        accum = 0.0F;
	        frames = 0;
	    }
			
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
        
        public SetRenderTextureDrawCall(RenderTexture text)
        {
            Texture = text;
        }
    }

    public static Queue<DrawCall> Draw = new Queue<DrawCall>();

    public static Microsoft.Xna.Framework.Graphics.Texture2D HueTexture { get; internal set; }


    public static Material MainTexure;
    public static Dictionary<UnityEngine.Texture2D, int> TextureIndex = new Dictionary<UnityEngine.Texture2D, int>();
    private void OnPostRender()
    {
        dev.ResetPools();
        game.DrawUnity( Time.deltaTime );
        

        GL.PushMatrix();
        GL.LoadPixelMatrix( 0, Screen.width, Screen.height, 0 );
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
                MainTexure.SetPass( 0 );
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
    void OnGUIII()
    {
        // Draw sprites from SpriteBatch.Draw()
		for (int i = 0; i < drawQueue.LastSpriteQueue.Length; i++) 
		{
			DrawSpriteCall call = drawQueue.LastSpriteQueue[i];

			float x = call.Position.X;
			float y = call.Position.Y;
			x -= call.Origin.X;
			y -= call.Origin.Y;
			float width = call.Texture2D.UnityTexture.width;
			float height = call.Texture2D.UnityTexture.height;
			//GUI.color = new Color(call.Color.X,	call.Color.Y, call.Color.Z, call.Color.W);
			
			Rect sourceRect = new Rect(0,0, 1,1);
			
			if(call.Source != null)
			{
				sourceRect.x = call.Source.Value.X / width;
				sourceRect.y = call.Source.Value.Y / height;
				sourceRect.width = call.Source.Value.Width / width;
				sourceRect.height = call.Source.Value.Height / height;
			}
			
			if(call.SpriteEffects == Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally)
			{
				sourceRect.x = 1-sourceRect.x;
				sourceRect.width *= -1;
			}
			else if(call.SpriteEffects == Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipVertically)
			{
				sourceRect.y = 1-sourceRect.y;
				sourceRect.height *= -1;
			}
            //if ( call.Texture2D.RenderTexture != null )
            //{
           //     GUI.DrawTextureWithTexCoords( new Rect( x, y, width * Mathf.Abs( sourceRect.width ), height * Mathf.Abs( sourceRect.height ) ), call.Texture2D.RenderTexture, sourceRect );
           // }
            else
            GUI.DrawTextureWithTexCoords(new Rect(x,y,width * Mathf.Abs(sourceRect.width),height * Mathf.Abs(sourceRect.height)), call.Texture2D.UnityTexture, sourceRect);
		}
		
        // Draw strings from SpriteBatch.DrawString()
        for (int i = 0; i < drawQueue.LastStringQueue.Length; i++)
        {
            DrawStringCall call = drawQueue.LastStringQueue[i];
     
			//GUI.color = new Color(call.Color.X,	call.Color.Y, call.Color.Z, call.Color.W);
			
            Vector2 size = GUI.skin.label.CalcSize(new GUIContent(call.Value));
            GUI.Label(new Rect(call.Position.X, call.Position.Y, size.x, size.y), call.Value);
        }

        //GUIStyle style = new GUIStyle();
        //style.font = Font.

        //GUILayout.Label()
		//GUI.color = Color.black;
        //GUILayout.Label(string.Format("{0} fps, {1} sprite draw calls", fps, drawQueue.LastSpriteQueue.Length));
    }
		

}
