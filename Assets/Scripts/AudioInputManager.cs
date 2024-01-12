using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NAudio.Wave;
using System;
using System.Linq;
using TMPro;

public class AudioInputManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private int frameSize = 4096;
    [SerializeField]
    private int samplingFrequency = 44100;
    private Chromagram c;
    private ChordDetector cd;
    private WaveInEvent waveIn;
    void Start()
    {
        cd = new ChordDetector();
        c = new Chromagram(frameSize, samplingFrequency);
        double[] inputAudioFrame = new double[frameSize];
        //Initialise audio input
        waveIn = new WaveInEvent
        {
            DeviceNumber = 0,
            WaveFormat = new WaveFormat(rate: 44100, bits: 16, channels: 1),
        };
        //When data is available, process audio frame
        waveIn.DataAvailable += (sender, e) =>
        {
            //shorts are 2 bytes, so divide length of buffer (array of bytes) by 2 to get number of shorts
            int bufferSize = e.Buffer.Length / 2;
            short[] values = new short[bufferSize]; 
            Buffer.BlockCopy(e.Buffer, 0, values, 0, e.BytesRecorded);
            if (frameSize > bufferSize)
            {
                //Rolling window buffer
                for (int i = 0; i < frameSize - bufferSize; i++)
                {
                    inputAudioFrame[i] = inputAudioFrame[i + bufferSize];
                }
                int n = 0;
                for (int i = frameSize - bufferSize; i < frameSize; i++)
                {
                    inputAudioFrame[i] = values[n];
                    n++;
                }
            }
            //If buffer is larger than frame size, only take the last frameSize samples
            else
            {
                for (int i = 0; i < frameSize; i++)
                {
                    inputAudioFrame[i] = values[i];
                }
            }
            c.processAudioFrame(inputAudioFrame);
        };
        waveIn.RecordingStopped += (sender, e) =>
        {
            if (e.Exception == null)
            {
                Debug.Log("Stopped recording");
            }
            else
            {
                Debug.LogError(string.Format("Stopped recording: exception={0}", e.Exception.ToString()));
            }
        };
        waveIn.StartRecording();
    }

    // Update is called once per frame
    void Update()
    {   //Only detect chord if chromagram has finished calculating
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
