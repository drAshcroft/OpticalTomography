using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImageViewer;
using System.IO;
using System.Threading;
using MathHelpLib;

namespace ImageViewer
{

    /// <summary>
    /// this library has two states.  one has all the images in memory, with the option to save a certain set, and the other 
    /// maintains all the images on disk, with every generation saved.
    /// </summary>
    public class ImageLibrary
    {
        private ImageHolder[] mImageLibrary = null;
        private string[] mOriginalFilenames = null;


        private bool mPersistInMemory = false;
        private string mTempFolder = "";

        private object[] Locks = null;
        private int[] mImageGeneration;
        private int[] mSavedInGeneration;
        private object CriticalSectionLock = new object();

        private int SaveGeneration = -1;
        private bool DoSave = false;
        private string SaveFilename = "";
        private string SaveExten = "";
        //private object CriticalSectionLock = new object();
        /// <summary>
        /// Builds a threadsafe library of images.  Images are loaded when requested by default to spread out the loading 
        /// persist in memory does not do anything at the moment.
        /// </summary>
        /// <param name="OriginalFilenames"></param>
        /// <param name="PersistInMemory"></param>
        public ImageLibrary(string[] OriginalFilenames, bool PersistInMemory, string TempFolder)
        {
            mOriginalFilenames = OriginalFilenames;
            mImageLibrary = new ImageHolder[mOriginalFilenames.Length];
            mPersistInMemory = PersistInMemory;
            mTempFolder = TempFolder ;

            Locks = new object[mOriginalFilenames.Length];
            mImageGeneration = new int[mOriginalFilenames.Length];
            mSavedInGeneration = new int[mOriginalFilenames.Length];
            for (int i = 0; i < Locks.Length; i++)
            {
                Locks[i] = new object();
                mSavedInGeneration[i] = -1;
                mImageGeneration[i] = 0;
            }
        }


      
        /// <summary>
        /// returns the requested image.  If the image has not been loaded yet, it loads the requested image and then returns requested image
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ImageHolder this[int index]
        {
            get
            {
                if (File.Exists(mOriginalFilenames[index]) == false)
                    Console.WriteLine("Waiting for PP to be created");
                lock (Locks[index])
                {
                    if (mPersistInMemory == true )
                    {
                        if (mImageLibrary[index] == null)
                        {
                            while (File.Exists(mOriginalFilenames[index]) == false)
                            {
                                Thread.Sleep(100);
                            }
                            mImageLibrary[index] = MathHelpLib.MathHelpsFileLoader.Load_Bitmap(mOriginalFilenames[index]);
                        }
                        return mImageLibrary[index];
                    }
                    else
                    {
                        if (mImageGeneration[index] == 0)
                        {
                            while (File.Exists(mOriginalFilenames[index]) == false)
                            {
                                Thread.Sleep(100);
                            }

                            return MathHelpLib.MathHelpsFileLoader.Load_Bitmap(mOriginalFilenames[index]);
                        }
                        else
                        {
                            while (File.Exists(mOriginalFilenames[index]) == false)
                            {
                                Thread.Sleep(100);
                            }

                            string filename = string.Format("{0}_{3}_{1:0000}.{2}", mTempFolder + "temp", index, "raw", mImageGeneration[index]-1);
                            return MathHelpLib.MathHelpsFileLoader.Load_Bitmap(filename, 1);
                        }
                    }
                }
            }

            set
            {
                lock (CriticalSectionLock)
                {
                    lock (Locks[index])
                    {
                        if (mPersistInMemory == true )
                        {
                            mImageLibrary[index] = value;
                            mImageGeneration[index]++;
                        }
                        else
                        {
                            string filename = string.Format("{0}_{3}_{1:0000}.{2}", mTempFolder + "temp", index, "raw", mImageGeneration[index]);
                            value.Save(filename);
                            mImageGeneration[index]++;
                        }

                        if (DoSave == true && SaveGeneration == mImageGeneration[index])
                        {
                            mImageLibrary[index].Save(string.Format("{0}_{3}_{1:0000}.{2}", SaveFilename, index, SaveExten, SaveGeneration));
                            mSavedInGeneration[index] = mImageGeneration[index];
                        }
                    }
                }
            }

        }

        public int Count
        {
            get { return mImageLibrary.Length; }
        }

        public int GetImageGeneration(int ImageIndex)
        {
            lock (CriticalSectionLock )
            {
                return mImageGeneration[ImageIndex];
            }
        }

        /// <summary>
        /// This function is designed for threaded operation.  It will set a flag that will save all the images
        /// that are placed into the image library to disk.  Once this generation is set, then the library will stop
        /// saving.  Must be called before the first image of the current generation is put into the library.
        /// </summary>
        /// <param name="FilePattern">This shows the filepattern to be used to save i.e.  c:\fileDir\CenteringImage.bmp The index numbers will be automatically added </param>
        public void SaveImageGenerationForward(string FilePattern)
        {
            lock (CriticalSectionLock)
            {

                SaveGeneration = mImageGeneration[0] + 1;
                SaveFilename = Path.GetFileNameWithoutExtension(FilePattern);
                SaveExten = Path.GetExtension(FilePattern);
                DoSave = true;
            }
        }

        /// <summary>
        /// This function will save all images that are in the library that are at a certain generation.
        /// </summary>
        /// <param name="ImageGeneration"></param>
        /// <param name="FilePattern"></param>
        public void SaveImageGenerationExisting(int ImageGeneration, string FilePattern)
        {
            lock (CriticalSectionLock)
            {
                SaveGeneration = ImageGeneration;
                SaveFilename = Path.GetDirectoryName(FilePattern ) + "\\" + Path.GetFileNameWithoutExtension(FilePattern);
                SaveExten = Path.GetExtension(FilePattern);
                for (int i = 0; i < mOriginalFilenames.Length; i++)
                {
                    if (mSavedInGeneration[i] != ImageGeneration && mImageGeneration[i]==ImageGeneration )
                    {
                        mImageLibrary[i].Save(string.Format("{0}_{3}_{1:0000}.{2}", SaveFilename, i, SaveExten, ImageGeneration));
                        mSavedInGeneration[i] = ImageGeneration;
                    }
                }

            }
        }
    }
}
