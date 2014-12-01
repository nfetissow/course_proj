using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STLParserProject
{
//     delegate void IntValueChanged (int newValue);
    delegate void LightningAmpChanged(int lightningSourceNum, double newValue);
    delegate double LightningSelectedChanged(int lightningSourceNum);
    delegate void FigureSelectedChanged(int figureNum, Material material, int selI, int selJ);
    delegate void FigureMaterialChanged(Material material);


    class UIData
    {
        
        //
        public static bool ShowThreadLines { get; set; }
        public static int ThreadCounts { get; set; }
        public static bool FPSWriteInFile { get; set; }
        //
        public static int CameraNum { get; set; }
        //
        public static double LightSourceAmp { get; set; }
        public static Figure SelectedFigure { get; set; }
        public static Point SelectedFigurePos { get; set; }
        // events
    }
}
