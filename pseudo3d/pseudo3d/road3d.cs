using System;
using System.Collections.Generic;
using PixelEngine;

namespace pseudo3d
{
    class road3d : Game {
        float carPos = 0.0f;
        Sprite[] carSprites = new Sprite[3] { Sprite.Load("carLeft.png"), Sprite.Load("car.png"), Sprite.Load("carRight.png") };
        float distance = 0;
        List<trackPeice> track = new List<trackPeice>();
        float curvature = 0.0f;
        float speed = 0;
        float trackCurvature = 0;
        float playerCurvature = 0;
        bool stats = false;
        float trackDistance;
        float lapTime = 0;

        public override void OnCreate() {
            track.Add(new trackPeice(0.0f, 10.0f)); //short section for start/finish line
            track.Add(new trackPeice(0.0f, 200.0f));
            track.Add(new trackPeice(1.0f, 200.0f));
            track.Add(new trackPeice(0.0f, 400.0f));
            track.Add(new trackPeice(-1.0f, 100.0f));
            track.Add(new trackPeice(0.0f, 200.0f));
            track.Add(new trackPeice(-1.0f, 200.0f));
            track.Add(new trackPeice(1.0f, 200.0f));
            track.Add(new trackPeice(0.2f, 500.0f));
            track.Add(new trackPeice(0.0f, 200.0f));

            foreach (trackPeice peice in track) {
                trackDistance += peice.distance;
            }
        }

        public override void OnUpdate(float delta) {
            lapTime += delta;
            if (GetKey(Key.Up).Down) { speed += 2f * delta; }
            else { speed -= 1f * delta; }
            int carDir = 0;
            if (GetKey(Key.Left).Down) { playerCurvature -= 0.7f*delta; carDir = -1; }
            if (GetKey(Key.Right).Down){ playerCurvature += 0.7f*delta; carDir = 1; }
            if (GetKey(Key.Tab).Pressed) { stats = !stats; }

            if (Math.Abs(playerCurvature - trackCurvature) >= 0.8f) { speed -= 5f * delta; }

            speed = Clamp(speed, 0, 1);
            distance += (70f*speed)*delta;

            //get point on track
            float offset = 0;
            int trackSection = 0;

            if (distance >= trackDistance) { distance -= trackDistance; lapTime = 0; } // lap completed

            while (trackSection < track.Count && offset <= distance) {
                offset += track[trackSection].distance; trackSection++;
            }

            float targetCurvature = track[trackSection-1].curve;
            float trackCurveDiff = (targetCurvature-curvature)*delta*speed;
            curvature += trackCurveDiff;

            trackCurvature += (curvature)*delta*speed;

            for (int y = 0; y < ScreenHeight / 2; y++) {
                //draw sky gradient
                DrawLine(new Point(0, y), new Point(ScreenWidth, y), new Pixel(0, (byte)Map(y, 0, ScreenHeight/2, 0, 128), (byte)Map(y, 0, ScreenHeight/2, 128, 256)));
            }
            //draw scenery
            for (int x = 0; x < ScreenWidth; x++) {
                int hillHeight = (int)Math.Abs(Sin(x*0.01f+trackCurvature)*16);
                for (int y = (ScreenHeight/2)-hillHeight; y < ScreenHeight/2; y++) {
                    Draw(x, y, Pixel.Presets.DarkYellow);
                }
            }

            for (int y = 0; y < ScreenHeight/2; y++) {
                for (int x = 0; x < ScreenWidth; x++) {
                    float perspective = (float)y/(ScreenHeight/2f);

                    float middlePoint = 0.5f+curvature*Power(1f-perspective, 3);
                    float roadWidth = 0.1f + perspective * 0.8f;
                    float clipWidth = roadWidth*0.15f;

                    roadWidth *= 0.5f;
                    int leftGrass = (int)((middlePoint-roadWidth-clipWidth)*ScreenWidth);
                    int leftClip  = (int)((middlePoint-roadWidth)*ScreenWidth);
                    int rightGrass = (int)((middlePoint+roadWidth+clipWidth)*ScreenWidth);
                    int rightClip  = (int)((middlePoint+roadWidth)*ScreenWidth);

                    int row = ScreenHeight / 2 + y;

                    Pixel grassColour = Sin(20f * Power(1.0f-perspective, 3)+distance*0.1f) > 0.0f ? Pixel.Presets.Green : Pixel.Presets.DarkGreen;
                    Pixel clipColour = Sin(80f * Power(1.0f-perspective, 3)+distance*0.1f) > 0.0f ? Pixel.Presets.Red : Pixel.Presets.White;
                    Pixel roadColour = (trackSection-1) == 0 ? Pixel.Presets.White : Pixel.Presets.Grey;

                    if (x >= 0 && x < leftGrass) { Draw(x, row, grassColour); } //left grass
                    if (x >= leftGrass && x < leftClip) { Draw(x, row, clipColour); } //left clip
                    if (x >= leftClip && x < rightClip) { Draw(x, row, roadColour); } //road
                    if (x >= rightClip && x < rightGrass) { Draw(x, row, clipColour); } //right clip
                    if (x >= rightGrass && x < ScreenWidth) { Draw(x, row, grassColour); } //right grass
                }
            }

            //draw car
            carPos = playerCurvature - trackCurvature;
            int carDrawPos = (int)(ScreenWidth/2 + ((int)(ScreenWidth*carPos)/2.0f))-7; //sprite is 14 wide
            DrawSprite(new Point(carDrawPos, 80), carSprites[carDir+1]);

            //write stats
            if (FrameCount % 30 == 0 && stats) {
                Console.Clear();
                Console.WriteLine("Distance: " + distance.ToString());
                Console.WriteLine("Target curvature: " + curvature.ToString());
                Console.WriteLine("Player curvature: " + playerCurvature.ToString());
                Console.WriteLine("Player speed: " + speed.ToString());
                Console.WriteLine("Track curvature: " + trackCurvature.ToString());
            }
            DrawText(Point.Origin, "Lap time: " + Math.Round(lapTime, 2).ToString() + "s", Pixel.Presets.White);
        }

        struct trackPeice {
            public float curve;
            public float distance;
            public trackPeice(float _curve, float _distance) { curve = _curve; distance = _distance; }
        }
    }
}