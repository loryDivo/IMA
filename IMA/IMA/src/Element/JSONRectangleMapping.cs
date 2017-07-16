using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;

namespace IMA.src
{

    /*
     * Oggetto per creare un file in formato JSON
     * con all'interno l'ID delle varie bounding box e le coordinate ricavate
    */ 
    public class JSONRectangleMapping
    {
        private String idInfo;
        private String rectangleCoordinateInfo;
        private String leftTopCoordinateInfo;
        private String leftBottomCoordinateInfo;
        private String rightTopCoordinateInfo;
        private String rightBottomCoordinateInfo;
        private String X;
        private string Y;

        private String idRectangle;
        private SKPoint leftTopRectangleCoordinate;
        private SKPoint leftBottomRectangleCoordinate;
        private SKPoint rightTopRectangleCoordinate;
        private SKPoint rightBottomRectangleCoordinate;

        public JSONRectangleMapping(String idRectangle,
                                SKPoint leftTopCoordinate, SKPoint leftBottomCoordinate,
                                SKPoint rightTopCoordinate, SKPoint rightBottomCoordinate)
        {
            idInfo = "\"ID Rectangle\"";
            rectangleCoordinateInfo = "\"Rectangle Coordinate\"";
            leftTopCoordinateInfo = "\"Left Top Rectangle Coordinate\"";
            leftBottomCoordinateInfo = "\"Left Bottom Rectangle Coordinate\"";
            rightTopCoordinateInfo = "\"Right Top Rectangle Coordinate\"";
            rightBottomCoordinateInfo = "\"Right Bottom Rectangle Coordinate\"";
            X = "\"X\"";
            Y = "\"Y\"";

            this.idRectangle = "\"" + idRectangle + "\"";
            leftTopRectangleCoordinate = leftTopCoordinate;
            leftBottomRectangleCoordinate = leftBottomCoordinate;
            rightTopRectangleCoordinate = rightTopCoordinate;
            rightBottomRectangleCoordinate = rightBottomCoordinate;
        }

        /*
         * Creazione stringa in formato JSON 
         */
        public String CreateJSONString()
        {
            StringBuilder str = new StringBuilder();
            str.Append("{" + idInfo + ":" + idRectangle + "," + rectangleCoordinateInfo + ":" + "["
                + "{" + leftTopCoordinateInfo + ":" + "{" + X + ":" + leftTopRectangleCoordinate.X + "," + Y + ":" + leftTopRectangleCoordinate.Y + "}" + "},"
                + "{" + leftBottomCoordinateInfo + ":" + "{" + X + ":" + leftBottomRectangleCoordinate.X + "," + Y + ":" + leftBottomRectangleCoordinate.Y + "}" + "},"
                + "{" + rightTopCoordinateInfo + ":" + "{" + X + ":" + rightTopRectangleCoordinate.X + "," + Y + ":" + rightTopRectangleCoordinate.Y + "}" + "},"
                + "{" + rightBottomCoordinateInfo + ":" + "{" + X + ":" + rightBottomRectangleCoordinate.X + "," + Y + ":" + rightBottomRectangleCoordinate.Y + "}" + "}"
                + "]" + "}");
            return str.ToString();
        }

    }
}
