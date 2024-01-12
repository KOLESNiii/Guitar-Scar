using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FftSharp;
using System;

//Class to calculate chromagram from audio input
public class Chromagram
{
    private double[] buffer;
    private double[] magnitudeSpectrum;
    private double[] chromagram = new double[12];
    private double[] downsampledInputAudioFrame;
    private double referenceFrequency;
    private double[] noteFrequencies = new double[12];
    private int bufferSize;
    private int inputAudioFrameSize;
    private int samplingFrequency;
    private int downsampledInputAudioFrameSize;
    private int numHarmonics;
    private int numOctaves;
    private int numBinsToSearch;
    private int numSamplesSinceLastCalculation;
    private int chromaCalculationInterval;
    private bool chromaReady;
    public Chromagram(int frameSize, int samplingFrequency)
    {
        inputAudioFrameSize = frameSize;
        this.samplingFrequency = samplingFrequency;
        referenceFrequency = 130.81278265;
        bufferSize = 8192;
        numHarmonics = 3;
        numOctaves = 3;
        numBinsToSearch = 2;
        //Initialises base note frequencies
        for (int i = 0; i < 12; i++)
        {
            noteFrequencies[i] = referenceFrequency * Mathf.Pow(2f,((float) i) / 12);
        }
        buffer = new double[bufferSize];
        //Initialises chromagram to 0
        for (int i = 0; i < 12; i++)
        {
            chromagram[i] = 0.0;
        }
        magnitudeSpectrum = new double[(bufferSize / 2) + 1];
        setSamplingFrequency(this.samplingFrequency);
        setInputAudioFrameSize(this.inputAudioFrameSize);
        numSamplesSinceLastCalculation = 0;
        chromaCalculationInterval = 2048;
        chromaReady = false;
    }

    //Processes audio frame and calculates chromagram if enough samples have been processed
    public void processAudioFrame(double[] inputAudioFrame)
    {
        chromaReady = false;
        downSampleFrame(inputAudioFrame);
        //rolling window buffer
        for (int i = 0; i < bufferSize - downsampledInputAudioFrameSize; i++) 
        {
            buffer[i] = buffer[i + downsampledInputAudioFrameSize];
        }
        int n = 0;
        for (int i = bufferSize - downsampledInputAudioFrameSize; i < bufferSize; i++)
        {
            buffer[i] = downsampledInputAudioFrame[n];
            n++;
        }
        //logic for slowing down chromagram calculation
        numSamplesSinceLastCalculation += inputAudioFrameSize;
        if (numSamplesSinceLastCalculation >= chromaCalculationInterval)
        {
            calculateChromagram();
            numSamplesSinceLastCalculation = 0;
        }
    }

    public void setInputAudioFrameSize(int frameSize)
    {
        inputAudioFrameSize = frameSize;
        downsampledInputAudioFrame = new double[inputAudioFrameSize / 4];
        downsampledInputAudioFrameSize = downsampledInputAudioFrame.Length;
    }

    public void setSamplingFrequency(int samplingFrequency)
    {
        this.samplingFrequency = samplingFrequency;
    }

    public void setChromaCalculationInterval(int numSamples)
    {
        chromaCalculationInterval = numSamples;
    }

    public double[] getChromagram()
    {
        setNotReady();
        return chromagram;
    }

    public bool isReady()
    {
        return chromaReady;
    }
    //Calculates chromagram from magnitude spectrum
    private void calculateChromagram()
    {
        calculateMagnitudeSpectrum();
        double divisorRatio = (double)samplingFrequency / 4.0 / bufferSize;
        for (int n = 0; n < 12; n++)
        {
            double chromaSum = 0.0;
            //Calculates chroma for each note by iterating through octaves and harmonics
            for (int octave = 1; octave <= numOctaves; octave++)
            {
                double noteSum = 0.0;
                for (int harmonic = 1; harmonic <= numHarmonics; harmonic++)
                {
                    int centreBin = round(noteFrequencies[n] * octave * harmonic / divisorRatio); //central bin for note
                    int minBin = centreBin - numBinsToSearch * harmonic; //minimum and maximum bins to search for note
                    int maxBin = centreBin + numBinsToSearch * harmonic;
                    double maxValue = 0.0;
                    for (int k = minBin; k <= maxBin; k++) //gets all magnitudes at and near note
                    {
                        if (magnitudeSpectrum[k] > maxValue)
                        {
                            maxValue = magnitudeSpectrum[k];
                        }
                    }
                    noteSum += maxValue / harmonic; //sums magnitudes
                }
                chromaSum += noteSum;
            }
            chromagram[n] = chromaSum;
        }
        chromaReady = true;
    }
    //Calculates magnitude spectrum from buffer, using FFT
    private void calculateMagnitudeSpectrum()
    {
        var window = new FftSharp.Windows.Hamming();
        var input = window.Apply(buffer);
        var output = FftSharp.FFT.Forward(input);
        var tempOutput = FftSharp.FFT.Magnitude(output);
        magnitudeSpectrum = new double[bufferSize / 2 + 1];
        for (int i = 0; i < bufferSize / 2 + 1; i++)
        {
            magnitudeSpectrum[i] =Mathf.Sqrt((float)tempOutput[i]);
        }

    }
    //Downsamples audio frame to 1/4 of original size
    private void downSampleFrame(double[] inputAudioFrame)
    {
        double[] filteredFrame = new double[inputAudioFrameSize];
        float b0,b1,b2,a1,a2;
        float x_1,x_2,y_1,y_2;

        b0 = 0.2929f;
        b1 = 0.5858f;
        b2 = 0.2929f;
        a1 = -0.0000f;
        a2 = 0.1716f;

        x_1 = 0;
        x_2 = 0;
        y_1 = 0;
        y_2 = 0;

        for (int i = 0; i < inputAudioFrameSize; i++)
        {
            filteredFrame[i] = b0 * inputAudioFrame[i] + b1 * x_1 + b2 * x_2 - a1 * y_1 - a2 * y_2;
            x_2 = x_1;
            x_1 = (float)inputAudioFrame[i];
            y_2 = y_1;
            y_1 = (float)filteredFrame[i];
        }

        for (int i = 0; i < inputAudioFrameSize / 4; i++)
        {
            downsampledInputAudioFrame[i] = filteredFrame[i * 4];
        }
    }

    private int round(double value)
    {
        return Mathf.FloorToInt((float)value + 0.5f);
    }

    public void setNotReady()
    {
        chromaReady = false;
    }
}
