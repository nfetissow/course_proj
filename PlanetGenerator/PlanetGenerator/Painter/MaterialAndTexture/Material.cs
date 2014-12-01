using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STLParserProject
{
    class MaterialList
    {
        private static List<Material> materials;
        private static MaterialList instance = null;
        private MaterialList()
        {
            materials = new List<Material>();
            Image img = Image.FromFile("Textures/chessboardTexture.BMP");
            img.RotateFlip(RotateFlipType.RotateNoneFlipX);
            materials.Add(new Material("Шахматная доска",new OpticalCharacteristics(0.1, 0.5, 0.4, 15),
                                      new CylindricalTexture(new FastBitmap(img, img.Size))));
            img = Image.FromFile("Textures/chessboardTexture.BMP");
            img.RotateFlip(RotateFlipType.RotateNoneFlipX);
            materials.Add(new Material("Шахматная доска", new OpticalCharacteristics(0.1, 0.5, 0.4, 15),
                                      new CylindricalTexture(new FastBitmap(img, img.Size))));
            img = Image.FromFile("Textures/metalTexture.jpg");
            img.RotateFlip(RotateFlipType.RotateNoneFlipX);
            materials.Add(new Material("Железо", new OpticalCharacteristics(0.1, 0.9, 0.5, 40),
                                      new CylindricalTexture(new FastBitmap(img, img.Size))));
            img = Image.FromFile("Textures/woodTexture.jpg");
            img.RotateFlip(RotateFlipType.RotateNoneFlipX);
            materials.Add(new Material("Дерево", new OpticalCharacteristics(0.1, 0.4, 0.2, 10),
                                      new CylindricalTexture(new FastBitmap(img, img.Size))));

            img = Image.FromFile("Textures/woodLightTexture.jpg");
            img.RotateFlip(RotateFlipType.RotateNoneFlipX);
            materials.Add(new Material("Светлое дерево", new OpticalCharacteristics(0.1, 0.5, 0.4, 20),
                                      new CylindricalTexture(new FastBitmap(img, img.Size))));

            img = Image.FromFile("Textures/marbleTexture.jpg");
            img.RotateFlip(RotateFlipType.RotateNoneFlipX);
            materials.Add(new Material("Мрамор", new OpticalCharacteristics(0.1, 0.4, 0.2, 5),
                                      new CylindricalTexture(new FastBitmap(img, img.Size))));

            img = Image.FromFile("Textures/marbleTexture.jpg");

            Bitmap bmp = new Bitmap(1000, 1000);
            using (Graphics gfx = Graphics.FromImage(bmp))
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(unchecked((int)0xFFD4AF37))))
            {
                gfx.FillRectangle(brush, 0, 0, bmp.Width, bmp.Height);
            }
//             img.RotateFlip(RotateFlipType.RotateNoneFlipX);
            materials.Add(new Material("Серебро", new OpticalCharacteristics(0.2, 0.8, 0.7, 5),
                                      new CylindricalTexture(new FastBitmap((Image)bmp, bmp.Size))));

            img = Image.FromFile("Textures/goldTexture.jpg");
            img.RotateFlip(RotateFlipType.RotateNoneFlipX);
            materials.Add(new Material("Золото", new OpticalCharacteristics(0.2, 0.7, 0.3, 5),
                                      new CylindricalTexture(new FastBitmap(img, img.Size))));
        }

        public static List<Material> Materials
        {
            get
            {
                if (instance == null)
                    instance = new MaterialList();
                return MaterialList.materials;
            }
        }
    }

    class Material
    {
        public OpticalCharacteristics Optics { get; set; }
        public CylindricalTexture Texture { get; set; }
        public string Name { get; set; }

        public Material(string name, OpticalCharacteristics optics, CylindricalTexture texture)
        {
            this.Name = name;
            this.Optics = optics;
            this.Texture = texture;
        }


    }
}
