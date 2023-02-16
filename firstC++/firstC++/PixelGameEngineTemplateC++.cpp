// PixelGameEngineTemplateC++.cpp : This file contains the 'main' function. Program execution begins and ends there.
#define OLC_PGE_APPLICATION
#include "olcPixelGameEngine.h"

class Example : public olc::PixelGameEngine {
public:
    Example()
    {
        sAppName = "Example";
    }

public:
    bool OnUserCreate() override
    {
        // Called once at the start, so create things here
        return true;
    }

    bool OnUserUpdate(float fElapsedTime) override
    {
        // called once per frame
        for (int y = 0; y < ScreenHeight(); y++)
            for (int x = 0; x < ScreenWidth(); x++)
                Draw(x, y, olc::Pixel(rand() % 255, rand() % 255, rand() % 255));
        return !GetKey(olc::ESCAPE).bPressed; //if true don't close, if false, close
    }
};

int main() {
    Example demo;
    if (demo.Construct(256, 240, 4, 4))
        demo.Start();

    return 0;
}
// Run program: Ctrl + F5 or Debug > Start Without Debugging menu
// Debug program: F5 or Debug > Start Debugging menu