using UnityEngine;
using System.Collections;
using ClassicUO;
using ClassicUO.Utility.Logging;
using System;
using System.Collections.Generic;
using ClassicUO.Renderer;
using Microsoft.Xna.Framework.Graphics;

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

        // Add an audio source and tell the media player to use it for playing sounds
        Microsoft.Xna.Framework.Media.MediaPlayer.AudioSource = gameObject.AddComponent<AudioSource>();

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

    public class MeshDrawCall : DrawCall
    {
        private static GraphicsDevice22 dev = new GraphicsDevice22();
        public MeshDrawCall( Microsoft.Xna.Framework.Graphics.Texture2D texture, SpriteVertex[] vertices )
        {
            Material = dev.GetMat( texture ); //new Material( Shader.Find( "Unlit/Transparent" ) );// 
            Material.SetVector( "_Hue", new Vector4( vertices[0].Hue.X, vertices[0].Hue.Y, vertices[0].Hue.Z, 0 ) );

            //  Material.mainTexture = texture.UnityTexture;
            Pos = new Vector3( vertices[0].Pos.x, vertices[0].Pos.y );
           // var mesh = dev.GetMesh( 1 );
           // mesh.Populate( vertices, vertices.Length );
           // Mesh = mesh.Mesh;

            Verts = new SpriteVertex[vertices.Length];
            vertices.CopyTo(Verts,0);
        }
        public SpriteVertex[] Verts;

        public Material Material { get; set; }
        //public Matrix4x4 Matrix { get; internal set; }
        public Mesh Mesh { get; internal set; }

        public Vector3 Pos { get; set; }
        public Quaternion Rotation { get; set; } = Quaternion.identity;
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

    private void OnPostRenvder()
    {
        GL.PushMatrix();
        //mcall.Material.SetPass( 0 );
        GL.LoadPixelMatrix( 0, Screen.width, Screen.height, 0 );
      /*  GL.Begin( GL.TRIANGLE_STRIP );
        GL.Color( new UnityEngine.Color( 0, 0, 0, 1 ) );
        GL.Vertex3( 250f, 50f, 0 );
        GL.Vertex3( 0, 50f, 0 );
        GL.Vertex3( 250f, 250f, 0 );
        GL.Vertex3( 0, 250f, 0 );
        GL.End();*/
        GL.Begin( GL.QUADS );

        GL.Color( new UnityEngine.Color( 0, 0, 0, 1 ) );

        GL.Vertex3( 100, 100, 0 );
        GL.Vertex3( 100, 200, 0 );
        GL.Vertex3( 200, 200, 0 );
        GL.Vertex3( 200, 100, 0 );
        GL.End();
        GL.PopMatrix();
    }

    private void OnPostRender()
    {
        game.DrawUnity( Time.deltaTime );


        GL.PushMatrix();
        GL.LoadPixelMatrix( 0, Screen.width, Screen.height, 0 );
        var cam = GameObject.FindWithTag( "MainCamera" ).GetComponent<Camera>();
        var curRT = cam.activeTexture;
        GL.Color( new UnityEngine.Color( 0, 0, 0, 1 ) );
        int cnt = 0;
        while ( Draw.Count > 0 )
        {
            var call = Draw.Dequeue();
            if(call is SetRenderTextureDrawCall rtcall) {
                if ( rtcall.Texture != null )
                {
                   // GL.PopMatrix();

                    Graphics.SetRenderTarget( rtcall.Texture as RenderTexture );
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
            }
            else if(call is StandardDrawCall sdcall)
            {
              Graphics.DrawTexture( sdcall.ScreenRect, sdcall.Texture, sdcall.SourceRect, 0, 0, 0, 0, sdcall.Material );
            }
            else if (call is MeshDrawCall mcall)
            {
                mcall.Material.SetPass( 0 );
                // Graphics.DrawMeshNow( mcall.Mesh, Vector3.zero, mcall.Rotation );
                //continue;
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
                GL.Begin( GL.TRIANGLE_STRIP );
               

               GL.TexCoord( mcall.Verts[2].TextureCoordinate.ToVec3() );
                GL.Vertex( mcall.Verts[0].Position.ToVec3() );

                GL.TexCoord( mcall.Verts[3].TextureCoordinate.ToVec3() );
                GL.Vertex( mcall.Verts[1].Position.ToVec3() );

                GL.TexCoord( mcall.Verts[0].TextureCoordinate.ToVec3() );
                GL.Vertex( mcall.Verts[2].Position.ToVec3() );

                GL.TexCoord( mcall.Verts[1].TextureCoordinate.ToVec3() );
                GL.Vertex( mcall.Verts[3].Position.ToVec3() );
              
                GL.End();

            }
        }
        GL.PopMatrix();
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
