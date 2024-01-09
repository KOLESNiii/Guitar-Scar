using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NAudio.Wave;
using System;

public class AudioInputManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private int frameSize = 512;
    [SerializeField]
    private int samplingFrequency = 44100;
    private int samplesCaptured = 0;
    private Chromagram c;
    private ChordDetector cd;
    private WaveInEvent waveIn;
    void Start()
    {
        cd = new ChordDetector();
        c = new Chromagram(frameSize, samplingFrequency);
        double[] inputAudioFrame = new double[frameSize];
        waveIn = new WaveInEvent();
        waveIn.DeviceNumber = 0;
        waveIn.WaveFormat = new WaveFormat(samplingFrequency, 1);
        waveIn.DataAvailable += (sender, e) =>
        {
            float[] floatData = new float[e.BytesRecorded / 4];
            Buffer.BlockCopy(e.Buffer, 0, floatData, 0, e.BytesRecorded);
            int remainingSamples = Math.Min(frameSize - samplesCaptured, floatData.Length);
            Array.Copy(floatData, 0, inputAudioFrame, samplesCaptured, remainingSamples);
            samplesCaptured += remainingSamples;
            if (samplesCaptured >= frameSize)
            {
                samplesCaptured = 0;
                c.processAudioFrame(inputAudioFrame);
            }
        };
        waveIn.StartRecording();
    }

    // Update is called once per frame
    void Update()
    {
        if (c.isReady())
        {
            double[] chromagram = c.getChromagram();
            cd.detectChord(chromagram);
        }
    }

    void OnApplicationQuit()
    {
        waveIn.StopRecording();
    }
}
