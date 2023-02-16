using System;
using System.Collections.Generic;
using PixelEngine;

namespace pseudo3d
{
    class parallax : Game {
        Sprite[] layers = new Sprite[] { Sprite.Load("smallHills.png"), Sprite.Load("hills.png"), 
            Sprite.Load("clouds1.png"), Sprite.Load("mountains.png"), Sprite.Load("clouds2.png")};
        float[] layerPos = new float[5];
        public override void OnCreate() {
            PixelMode = Pixel.Mode.Alpha;
        }

        public override void OnUpdate(float delta) {
            for (int l = 0; l < 5; l++) {
                layerPos[l] += delta*5f*(5-l);
                layerPos[l] %= 600;
            }

            //draw
            for (int y = 0; y < 300; y++) { DrawLine(new Point(0,y), new Point(300,y), new Pixel(0, (byte)((y/300f)*128), (byte)((y/300f)*256))); }
            DrawSprite(new Point((int)layerPos[4], 0), layers[4]); DrawSprite(new Point((int)layerPos[4]-600,0), layers[4]);
            DrawSprite(new Point((int)layerPos[3], 0), layers[3]); DrawSprite(new Point((int)layerPos[3]-600,0), layers[3]);
            DrawSprite(new Point((int)layerPos[2], 0), layers[2]); DrawSprite(new Point((int)layerPos[2]-600,0), layers[2]);
            DrawSprite(new Point((int)layerPos[1], 0), layers[1]); DrawSprite(new Point((int)layerPos[1]-600,0), layers[1]);
            DrawSprite(new Point((int)layerPos[0], 0), layers[0]); DrawSprite(new Point((int)layerPos[0]-600,0), layers[0]);
        }
    }
}