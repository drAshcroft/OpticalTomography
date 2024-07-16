using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

namespace MathHelpLib._3DStuff
{
    public class VolumeRenderable : Mogre.SimpleRenderable , IDisposable
    {
        int mSlices;
        float mSize;
        float mRadius;
        Mogre.Matrix3 mFakeOrientation;
        string mTexture;
        Mogre.TextureUnitState mUnit;

        public VolumeRenderable(int nSlices, float size, string texture)
        {
            mSlices = nSlices;
            mSize = size;
            mTexture = texture;

            mRadius =(float)System.Math.Sqrt (size * size + size * size + size * size) / 2.0f;
            mBox =new  Mogre.AxisAlignedBox(-size, -size, -size, size, size, size);

            // No shadows
            setCastShadows(false);

            initialise();
        }
        public void Dispose()
        {
            // Remove private material
            MaterialManager.getSingleton().remove(mTexture);
            // need to release IndexData and vertexData created for renderable
            mRenderOp.indexData = null;
            mRenderOp.vertexData = null;

        }

        void _notifyCurrentCamera(Camera cam)
        {
            MovableObject._notifyCurrentCamera(cam);

            // Fake orientation toward camera
            Vector3 zVec = getParentNode()._getDerivedPosition() - cam.getDerivedPosition();
            zVec.normalise();
            Vector3 fixedAxis = cam.getDerivedOrientation() * Vector3.UNIT_Y;

            Vector3 xVec = fixedAxis.crossProduct(zVec);
            xVec.normalise();

            Vector3 yVec = zVec.crossProduct(xVec);
            yVec.normalise();

            Quaternion oriQuat;
            oriQuat.FromAxes(xVec, yVec, zVec);

            oriQuat.ToRotationMatrix(mFakeOrientation);

            Matrix3 tempMat;
            Quaternion q = getParentNode()._getDerivedOrientation().UnitInverse() * oriQuat;
            q.ToRotationMatrix(tempMat);

            Matrix4 rotMat = Matrix4.IDENTITY;
            rotMat = tempMat;
            rotMat.setTrans(Vector3(0.5f, 0.5f, 0.5f));
            mUnit.setTextureTransform(rotMat);
        }



        void getWorldTransforms(Matrix4 xform)
        {
            Matrix4 destMatrix = (Matrix4.IDENTITY); // this initialisation is needed

            Vector3 position = getParentNode()._getDerivedPosition();
            Vector3 scale = getParentNode()._getDerivedScale();
            Matrix3 scale3x3 = Matrix3.ZERO;
            scale3x3[0][0] = scale.x;
            scale3x3[1][1] = scale.y;
            scale3x3[2][2] = scale.z;

            destMatrix = mFakeOrientation * scale3x3;
            destMatrix.setTrans(position);

            xform = destMatrix;
        }

        void initialise()
        {
            // Create geometry
            int nvertices = mSlices * 4; // n+1 planes
            int elemsize = 3 * 3;
            int dsize = elemsize * nvertices;
            int x;

            IndexData* idata = new IndexData();
            VertexData* vdata = new VertexData();

            // Create  structures
            float vertices = new float[dsize];

            float[][] coords = new float[][] {
		{0.0f, 0.0f},
		{0.0f, 1.0f},
		{1.0f, 0.0f},
		{1.0f, 1.0f}
	};

            for (x = 0; x < mSlices; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    float xcoord = coords[y][0] - 0.5;
                    float ycoord = coords[y][1] - 0.5;
                    float zcoord = -((float)x / (float)(mSlices - 1) - 0.5f);
                    // 1.0f .. a/(a+1)
                    // coordinate
                    vertices[x * 4 * elemsize + y * elemsize + 0] = xcoord * (mSize / 2.0f);
                    vertices[x * 4 * elemsize + y * elemsize + 1] = ycoord * (mSize / 2.0f);
                    vertices[x * 4 * elemsize + y * elemsize + 2] = zcoord * (mSize / 2.0f);
                    // normal
                    vertices[x * 4 * elemsize + y * elemsize + 3] = 0.0f;
                    vertices[x * 4 * elemsize + y * elemsize + 4] = 0.0f;
                    vertices[x * 4 * elemsize + y * elemsize + 5] = 1.0f;
                    // tex
                    vertices[x * 4 * elemsize + y * elemsize + 6] = xcoord * sqrtf(3.0f);
                    vertices[x * 4 * elemsize + y * elemsize + 7] = ycoord * sqrtf(3.0f);
                    vertices[x * 4 * elemsize + y * elemsize + 8] = zcoord * sqrtf(3.0f);
                }
            }
            ushort[] faces = new ushort[mSlices * 6];
            for (x = 0; x < mSlices; x++)
            {
                faces[x * 6 + 0] = x * 4 + 0;
                faces[x * 6 + 1] = x * 4 + 1;
                faces[x * 6 + 2] = x * 4 + 2;
                faces[x * 6 + 3] = x * 4 + 1;
                faces[x * 6 + 4] = x * 4 + 2;
                faces[x * 6 + 5] = x * 4 + 3;
            }
            // Setup buffers
            vdata->vertexStart = 0;
            vdata->vertexCount = nvertices;

            VertexDeclaration decl = vdata.vertexDeclaration;
            VertexBufferBinding bind = vdata.vertexBufferBinding;

            int offset = 0;
            decl.addElement(0, offset, VET_FLOAT3, VES_POSITION);
            offset += VertexElement.getTypeSize(VET_FLOAT3);
            decl.addElement(0, offset, VET_FLOAT3, VES_NORMAL);
            offset += VertexElement.getTypeSize(VET_FLOAT3);
            decl.addElement(0, offset, VET_FLOAT3, VES_TEXTURE_COORDINATES);
            offset += VertexElement.getTypeSize(VET_FLOAT3);

            HardwareVertexBufferSharedPtr vbuf =
            HardwareBufferManager.getSingleton().createVertexBuffer(
                offset, nvertices, HardwareBuffer.HBU_STATIC_WRITE_ONLY);

            bind.setBinding(0, vbuf);

            vbuf.writeData(0, vbuf.getSizeInBytes(), vertices, true);

            HardwareIndexBufferSharedPtr ibuf = HardwareBufferManager.getSingleton().
                createIndexBuffer(
                    HardwareIndexBuffer.IT_16BIT,
                    mSlices * 6,
                    HardwareBuffer.HBU_STATIC_WRITE_ONLY);

            idata.indexBuffer = ibuf;
            idata.indexCount = mSlices * 6;
            idata.indexStart = 0;
            ibuf.writeData(0, ibuf.getSizeInBytes(), faces, true);

            // Delete temporary buffers
            vertices = null;
            faces = null;

            // Now make the render operation
            mRenderOp.operationType = Ogre.RenderOperation.OT_TRIANGLE_LIST;
            mRenderOp.indexData = idata;
            mRenderOp.vertexData = vdata;
            mRenderOp.useIndexes = true;

            // Create a brand new private material
            if (!ResourceGroupManager.getSingleton().resourceGroupExists("VolumeRenderable"))
            {
                ResourceGroupManager.getSingleton().createResourceGroup("VolumeRenderable");
            }
            MaterialPtr material =
                MaterialManager.getSingleton().create(mTexture, "VolumeRenderable",
                    false, 0); // Manual, loader

            // Remove pre-created technique from defaults
            material.removeAllTechniques();

            // Create a techinique and a pass and a texture unit
            Technique technique = material.createTechnique();
            Pass pass = technique.createPass();
            TextureUnitState textureUnit = pass.createTextureUnitState();

            // Set pass parameters
            pass.setSceneBlending(SBT_TRANSPARENT_ALPHA);
            pass.setDepthWriteEnabled(false);
            pass.setCullingMode(CULL_NONE);
            pass.setLightingEnabled(false);

            // Set texture unit parameters
            textureUnit.setTextureAddressingMode(TextureUnitState.TAM_CLAMP);
            textureUnit.setTextureName(mTexture, TEX_TYPE_3D);
            textureUnit.setTextureFiltering(TFO_TRILINEAR);

            mUnit = textureUnit;
            m_pMaterial = material;
        }

        public float getBoundingRadius()
        {
            return mRadius;
        }
        public float getSquaredViewDepth(Camera cam)
        {
            Vector3 min, max, mid, dist;

            min = mBox.getMinimum();
            max = mBox.getMaximum();
            mid = ((min - max) * 0.5) + min;
            dist = cam.getDerivedPosition() - mid;

            return dist.squaredLength();
        }
    }
}
