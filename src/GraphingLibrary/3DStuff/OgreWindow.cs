using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using System.Drawing;
using MathHelpLib._3DStuff;
using System.Threading;
using System.Windows.Forms;


namespace MathHelpLib
{
    public class OgreWindow
    {
        static object CriticalSection = new object();

        public Root root;
        public SceneManager sceneMgr;

        protected Camera camera;
        protected Viewport viewport;
        protected RenderWindow window;
        protected Point position;
        protected IntPtr hWnd;

        TexturePtr ptex;
        SimpleRenderable vrend;

        List<SceneNode> nodes = new List<SceneNode>();
       
        private static Thread ChangeTexture;
        /// <summary>
        /// Signal to texture thread that it should quit
        /// </summary>
        private static bool KeepRunning;
        private static bool sBufferUpdating;


        public OgreWindow(Point origin, IntPtr hWnd)
        {
            position = origin;
            this.hWnd = hWnd;
        }

        public void InitMogre()
        {

            //----------------------------------------------------- 
            // 1 enter ogre 
            //----------------------------------------------------- 
            root = new Root();


            ResourceGroupManager.Singleton.AddResourceLocation("./media", "FileSystem", ResourceGroupManager.DEFAULT_RESOURCE_GROUP_NAME);
            //----------------------------------------------------- 
            // 3 Configures the application and creates the window
            //----------------------------------------------------- 
            bool foundit = false;
            foreach (RenderSystem rs in root.GetAvailableRenderers())
            {
                root.RenderSystem = rs;
                String rname = root.RenderSystem.Name;
                if (rname == "Direct3D9 Rendering Subsystem")//"Direct3D9 Rendering Subsystem"
                {
                    foundit = true;
                    break;
                }
            }

            if (!foundit)
                return; //we didn't find it... Raise exception?

            //we found it, we might as well use it!
            root.RenderSystem.SetConfigOption("Full Screen", "No");
            root.RenderSystem.SetConfigOption("Video Mode", "640 x 480 @ 32-bit colour");

            root.Initialise(false);
            NameValuePairList misc = new NameValuePairList();
            misc["externalWindowHandle"] = hWnd.ToString();
            window = root.CreateRenderWindow("Simple Mogre Form Window", 0, 0, false, misc);
            ResourceGroupManager.Singleton.InitialiseAllResourceGroups();

            //----------------------------------------------------- 
            // 4 Create the SceneManager
            // 
            //		ST_GENERIC = octree
            //		ST_EXTERIOR_CLOSE = simple terrain
            //		ST_EXTERIOR_FAR = nature terrain (depreciated)
            //		ST_EXTERIOR_REAL_FAR = paging landscape
            //		ST_INTERIOR = Quake3 BSP
            //----------------------------------------------------- 
            sceneMgr = root.CreateSceneManager(SceneType.ST_GENERIC, "SceneMgr");
            sceneMgr.AmbientLight = new ColourValue(0.5f, 0.5f, 0.5f);

            // Create a light
            Light l = sceneMgr.CreateLight("MainLight");
            // Accept default settings: point light, white diffuse, just set position
            // NB I could attach the light to a SceneNode if I wanted it to move automatically with
            //  other objects, but I don't
            l.Position = new Vector3(20, 80, 50);

            //----------------------------------------------------- 
            // 5 Create the camera 
            //----------------------------------------------------- 
            camera = sceneMgr.CreateCamera("SimpleCamera");
            camera.Position = new Vector3(0f, 0f, 100f);
            // Look back along -Z
            camera.LookAt(new Vector3(0f, 0f, 0f));
            camera.NearClipDistance = 5;


            viewport = window.AddViewport(camera);
            viewport.BackgroundColour = new ColourValue(0.0f, 0.0f, 0.0f, 1.0f);


            /* Entity ent = sceneMgr.CreateEntity("ogre", "ogrehead.mesh");
             SceneNode node = sceneMgr.RootSceneNode.CreateChildSceneNode("ogreNode");
             node.AttachObject(ent);*/

            MaterialPtr material = MaterialManager.Singleton.Create("Test/ColourTest",
                                             ResourceGroupManager.DEFAULT_RESOURCE_GROUP_NAME);
            material.GetTechnique(0).GetPass(0).VertexColourTracking =
                                                              (int)TrackVertexColourEnum.TVC_AMBIENT;

        }

        public void CreateMesh(string name, Point3D[] Vertexs, int[] TriangleIndices, Point3D[] Normals)
        {
            MeshBuilderHelper mbh = new MeshBuilderHelper(name, "General", false, 0, (uint)Vertexs.Length);


            UInt32 offPos = mbh.AddElement(VertexElementType.VET_FLOAT3,
                                           VertexElementSemantic.VES_POSITION).Offset;
            UInt32 offNorm = mbh.AddElement(VertexElementType.VET_FLOAT3,
                                            VertexElementSemantic.VES_NORMAL).Offset;

            UInt32 offDiff = mbh.AddElement(VertexElementType.VET_FLOAT3,
                                            VertexElementSemantic.VES_DIFFUSE).Offset;

            UInt32 offTex = mbh.AddElement(VertexElementType.VET_FLOAT3,
                                            VertexElementSemantic.VES_TEXTURE_COORDINATES).Offset;


            mbh.CreateVertexBuffer((uint)Vertexs.Length, HardwareBuffer.Usage.HBU_STATIC_WRITE_ONLY);
            Point3D cross;
            for (uint i = 0; i < Vertexs.Length; i++)
            {
                mbh.SetVertFloat(i, offPos, (float)Vertexs[i].X, (float)Vertexs[i].Y, (float)Vertexs[i].Z);      //position
                cross = Normals[i];
                mbh.SetVertFloat(i, offNorm, (float)cross.X, (float)cross.Y, (float)cross.Z);
                mbh.SetVertFloat(i, offDiff, 1, 1, 1);      //color

                double angle = System.Math.Atan2(Vertexs[i].X, Vertexs[i].Y) / 2 / System.Math.PI;
                if (angle < 0) angle = 1 + angle;

                mbh.SetVertFloat(i, offTex, (float)angle, (float)(Vertexs[i].Z / 25d));
            }

            uint TriangleCount = (uint)((double)TriangleIndices.Length / 3d);


            mbh.CreateIndexBuffer(TriangleCount, HardwareIndexBuffer.IndexType.IT_16BIT,
                                  HardwareBuffer.Usage.HBU_STATIC_WRITE_ONLY);

            uint cTriangle = 0;
            for (uint i = 0; i < TriangleIndices.Length; i += 3)
            {
                mbh.SetIndex16bit(cTriangle, (UInt16)TriangleIndices[i], (UInt16)TriangleIndices[i + 1], (UInt16)TriangleIndices[i + 2]);
                cTriangle++;
            }


            MeshPtr m = mbh.SetMaterial("Examples/TransparentTest");//SetMaterial("Test/ColourTest");
            m._setBounds(new AxisAlignedBox(-20.0f, -20.0f, -20.0f, 20.0f, 20.0f, 20.0f), false);
            m._setBoundingSphereRadius((float)System.Math.Sqrt(10.0f * 10.0f + 10.0f * 10.0f));


            Entity ent1 = sceneMgr.CreateEntity("MarchingMesh" + name, name);
            ent1.CastShadows = true;

            SceneNode node1 = sceneMgr.RootSceneNode.CreateChildSceneNode("MarchingMeshNode" + name);
            node1.AttachObject(ent1);
            nodes.Add(node1);
        }

        public void CreateCloud(int TextWidth, int TextHeight, int TextDepth, float ViewSize, uint nSlices)
        {
            // Create dynamic texture
            ptex = TextureManager.Singleton.CreateManual(
             "DynaTex", "General", TextureType.TEX_TYPE_3D, (uint)TextWidth, (uint)TextHeight, (uint)TextDepth, 0, PixelFormat.PF_A8R8G8B8);

            SceneNode snode = sceneMgr.RootSceneNode.CreateChildSceneNode(new Vector3(0, 0, 0));

            vrend = new VolumeRenderable(nSlices, ViewSize, "DynaTex");
            snode.AttachObject(vrend);
            nodes.Add(snode);
        }

        /// <summary>
        /// Generates a white cloud with the transparency dependant on the intensity
        /// </summary>
        /// <param name="Intensities"></param>
        public unsafe void GenerateCloud(double[, ,] Intensities)
        {

            HardwarePixelBufferSharedPtr buffer = ptex.GetBuffer(0, 0);

            buffer.Lock(HardwareBuffer.LockOptions.HBL_NORMAL);
            PixelBox pb = buffer.CurrentLock;

            UInt32* pbptr = (UInt32*)(pb.data);
            for (uint z = pb.box.front; z < pb.box.back; z++)
            {
                for (uint y = pb.box.top; y < pb.box.bottom; y++)
                {
                    for (uint x = pb.box.left; x < pb.box.right; x++)
                    {
                        byte val = (byte)(255 * (float)(Intensities[x, y, z] / 3f));
                        PixelUtil.PackColour(val, val, val, val, PixelFormat.PF_A8R8G8B8, pbptr + x);
                    }
                    pbptr += pb.rowPitch;
                }
                pbptr += pb.SliceSkip;
            }
            buffer.Unlock();
        }


        public unsafe void GenerateCloudUnthreaded(double[, ,] Intensities, List<Thresholds> ThresholdList)
        {

            HardwarePixelBufferSharedPtr buffer = ptex.GetBuffer(0, 0);
            lock (CriticalSection)
            {
                sBufferUpdating = true;
                buffer.Lock(HardwareBuffer.LockOptions.HBL_NORMAL);
                PixelBox pb = buffer.CurrentLock;

                UInt32* pbptr = (UInt32*)(pb.data);

                bool Found;
                for (uint z = pb.box.front; z < pb.box.back && KeepRunning; z++)
                {
                    for (uint y = pb.box.top; y < pb.box.bottom && KeepRunning; y++)
                    {
                        for (uint x = pb.box.left; x < pb.box.right && KeepRunning; x++)
                        {
                            byte val = (byte)(255 * (float)(Intensities[x, y, z] / 3f));
                            Found = false;
                            for (int i = 0; i < ThresholdList.Count; i++)
                            {
                                if (ThresholdList[i].IsInRange(Intensities[x, y, z]))
                                {
                                    PixelUtil.PackColour(ThresholdList[i].Red, ThresholdList[i].Green, ThresholdList[i].Blue, val, PixelFormat.PF_A8R8G8B8, pbptr + x);
                                    Found = true;
                                    break;
                                }
                            }
                            if (!Found)
                            {
                                PixelUtil.PackColour(0, 0, 0, 0, PixelFormat.PF_A8R8G8B8, pbptr + x);
                            }

                        }
                        pbptr += pb.rowPitch;
                    }
                    pbptr += pb.SliceSkip;
                }
                buffer.Unlock();
                sBufferUpdating = false;
            }



        }
        /// <summary>
        /// Generates a white cloud with the transparency dependant on the intensity
        /// </summary>
        /// <param name="Intensities">Normalized Intensity distribution</param>
        public unsafe void GenerateCloud(double[, ,] Intensities, List<Thresholds> ThresholdList)
        {



            if (ChangeTexture != null)
            {
                while (ChangeTexture.ThreadState == ThreadState.Running)
                {
                    KeepRunning = false;
                    Thread.Sleep(100);
                    Application.DoEvents();
                }
            }

            KeepRunning = true;
         
            ChangeTexture = new Thread(delegate()
                             {
                                 GenerateCloudUnthreaded(Intensities, ThresholdList);
                             }
                            );

            ChangeTexture.Start();
        }

        /// <summary>
        /// Generates a colored cloud with the transparency dependant on the intensity
        /// </summary>
        /// <param name="Colors"></param>
        public unsafe void GenerateCloud(Int32[, ,] Colors)
        {
            HardwarePixelBufferSharedPtr buffer = ptex.GetBuffer(0, 0);

            buffer.Lock(HardwareBuffer.LockOptions.HBL_NORMAL);
            PixelBox pb = buffer.CurrentLock;

            UInt32* pbptr = (UInt32*)(pb.data);
            for (uint z = pb.box.front; z < pb.box.back; z++)
            {
                for (uint y = pb.box.top; y < pb.box.bottom; y++)
                {
                    for (uint x = pb.box.left; x < pb.box.right; x++)
                    {
                        Int32 Value = Colors[x, y, z];
                        byte* pValue = (byte*)&Value;
                        byte transparency = (byte)((double)(pValue[0] + pValue[1] + pValue[2]) / 3d);
                        PixelUtil.PackColour(pValue[0], pValue[1], pValue[2], transparency, PixelFormat.PF_A8R8G8B8, pbptr + x);
                    }
                    pbptr += pb.rowPitch;
                }
                pbptr += pb.SliceSkip;
            }
            buffer.Unlock();
        }

        public void Paint()
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (i % 2 == 0)
                    nodes[i].Yaw((Mogre.Radian)(.1), Node.TransformSpace.TS_WORLD);
                else
                    nodes[i].Yaw((Mogre.Radian)(-.1), Node.TransformSpace.TS_WORLD);
            }
            /*  Time += .1;
              camera.Position = (new Vector3(0f, (float)(-100 * System.Math.Sin(Time)), (float)(100 * System.Math.Cos(Time))));
              camera.LookAt(new Vector3(0f, 0f, 0f));
              if (vrend != null)
                  vrend._notifyCurrentCamera(camera);*/

            lock (root)
            {
                if (sBufferUpdating == false)
                    root.RenderOneFrame();
            }
        }

        public void Dispose()
        {
            if (root != null)
            {
                root.Dispose();
                root = null;
            }
        }
    }
}
