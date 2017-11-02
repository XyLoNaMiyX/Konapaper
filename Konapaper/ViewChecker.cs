using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konapaper
{
    public static class ViewChecker
    {
        static string viewedFile = Path.Combine(Settings.ApplicationFolder, "viewed");

        static List<int> viewedIds;

        public static void View(params int[] ids)
        {
            viewedIds.AddRange(ids);
        }

        public static async Task<List<KonachanImage>> GetLatestUnviewedImages()
        {
            // if we have never seen any id, then all the unviewed ids are ALL (return empty)
            if (viewedIds.Count == 0)
                return new List<KonachanImage>();

            // declare variables
            var unviewedImages = new List<KonachanImage>();
            var viewedMax = viewedIds.Max();
            var konachan = new Konachan();
            bool broke = false;

            // start searching
            while (!broke)
            {
                var images = await konachan.GetImages();
                foreach (var image in images)
                {
                    // if the image id is larger than the latest viewed, it's new
                    if (image.ID > viewedMax)
                        unviewedImages.Add(image);

                    // otherwise, we're done
                    else
                    {
                        broke = true;
                        break;
                    }
                }

                ++konachan.Page;
            }

            return unviewedImages;
        }

        static ViewChecker()
        {
            viewedIds = new List<int>();
            Load();
        }

        public static void Load()
        {
            viewedIds.Clear();
            if (!File.Exists(viewedFile))
                return;

            viewedIds.Capacity = (int)(new FileInfo(viewedFile).Length / sizeof(int));

            // load ids
            using (var fs = new FileStream(viewedFile, FileMode.Open))
            using (var br = new BinaryReader(fs))
                while (fs.Position != fs.Length)
                    viewedIds.Add(br.ReadInt32());
        }

        public static void Save()
        {
            // clear duplicated ids
            var hashSet = new HashSet<int>(viewedIds);

            // save ids
            using (var fs = new FileStream(viewedFile, FileMode.Create))
            using (var bw = new BinaryWriter(fs))
                foreach (var id in hashSet)
                    bw.Write(id);
        }
    }
}
